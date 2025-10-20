using UnityEditor;
using UnityEngine;

/// <summary>
/// Hero Sprite自动导入配置工具
/// </summary>
public class HeroSpriteImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        // 只处理Characters文件夹下的sprite
        if (assetPath.Contains("Art/Sprites/Characters"))
        {
            TextureImporter importer = (TextureImporter)assetImporter;
            
            // 基础设置
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.spritePixelsPerUnit = 16;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 4096;
            importer.mipmapEnabled = false;
            
            // 高级设置
            importer.alphaIsTransparency = true;
            importer.npotScale = TextureImporterNPOTScale.None;
            
            Debug.Log($"✓ 自动配置Hero Sprite: {assetPath}");
        }
    }
}

/// <summary>
/// Hero Sprite切片工具
/// </summary>
public class HeroSpriteSlicer : EditorWindow
{
    [MenuItem("Tools/Hero/Slice Hero Sprites")]
    public static void ShowWindow()
    {
        GetWindow<HeroSpriteSlicer>("Hero Sprite Slicer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Hero Sprite 切片工具", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("切片 warrior.png"))
        {
            SliceWarriorSprite();
        }
        
        GUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "Warrior sprite布局：\n" +
            "- 每个动画帧: 12x15 像素\n" +
            "- 布局: 横向排列\n" +
            "- 顺序: Idle, Walk Down, Walk Up, Walk Left, Walk Right",
            MessageType.Info);
    }

    private void SliceWarriorSprite()
    {
        string path = "Assets/_Project/Art/Sprites/Characters/warrior.png";
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        
        if (importer == null)
        {
            EditorUtility.DisplayDialog("错误", "未找到 warrior.png！", "确定");
            return;
        }

        // 读取纹理尺寸
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (texture == null)
        {
            EditorUtility.DisplayDialog("错误", "无法加载纹理！", "确定");
            return;
        }

        int width = texture.width;
        int height = texture.height;
        
        Debug.Log($"Warrior sprite尺寸: {width}x{height}");

        // SPD的warrior.png通常是横向排列的动画帧
        // 每帧12x15像素
        int frameWidth = 12;
        int frameHeight = 15;
        int framesPerRow = width / frameWidth;
        int rows = height / frameHeight;

        Debug.Log($"检测到 {framesPerRow} 帧/行, {rows} 行");

        // 创建sprite元数据
        var spritesheet = new SpriteMetaData[framesPerRow * rows];
        int index = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < framesPerRow; col++)
            {
                SpriteMetaData smd = new SpriteMetaData();
                smd.name = $"warrior_{row}_{col}";
                smd.rect = new Rect(
                    col * frameWidth,
                    height - (row + 1) * frameHeight, // Unity的Y轴是从下往上
                    frameWidth,
                    frameHeight
                );
                smd.pivot = new Vector2(0.5f, 0.5f);
                smd.alignment = (int)SpriteAlignment.Center;
                
                spritesheet[index++] = smd;
            }
        }

        // 应用切片
        importer.spritesheet = spritesheet;
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        Debug.Log($"✓ warrior.png 切片完成！共 {spritesheet.Length} 个sprite");
        EditorUtility.DisplayDialog("完成", 
            $"warrior.png 切片完成！\n共生成 {spritesheet.Length} 个sprite帧", 
            "确定");
    }
}

