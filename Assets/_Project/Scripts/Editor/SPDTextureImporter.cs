using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// SPD素材自动导入配置
/// 自动将SPD的PNG设置为正确的导入参数
/// </summary>
public class SPDTextureImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        // 只处理Art/Tiles目录下的贴图
        if (!assetPath.Contains("Assets/_Project/Art/Tiles"))
            return;
        
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        
        // SPD像素艺术设置
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Multiple;
        textureImporter.spritePixelsPerUnit = 16f; // SPD使用16x16 tiles
        textureImporter.filterMode = FilterMode.Point; // 像素完美
        textureImporter.textureCompression = TextureImporterCompression.Uncompressed; // 不压缩
        textureImporter.maxTextureSize = 4096;
        textureImporter.mipmapEnabled = false; // 禁用mipmap保持像素清晰
        
        Debug.Log($"[SPD导入] 已配置: {Path.GetFileName(assetPath)}");
    }
}

/// <summary>
/// SPD Sprite切割工具
/// </summary>
public class SPDSpriteSlicerWindow : EditorWindow
{
    [MenuItem("DungeonRoguelike/SPD Tools/Auto Slice Sprites")]
    public static void ShowWindow()
    {
        GetWindow<SPDSpriteSlicerWindow>("SPD Sprite Slicer");
    }
    
    private string lastResult = "";
    
    private void OnGUI()
    {
        GUILayout.Label("SPD Sprite自动切割工具", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        if (GUILayout.Button("切割所有地形Tile (16x16)", GUILayout.Height(40)))
        {
            SliceAllTiles();
        }
        
        GUILayout.Space(10);
        
        // 显示上次执行结果
        if (!string.IsNullOrEmpty(lastResult))
        {
            EditorGUILayout.HelpBox(lastResult, MessageType.Info);
            GUILayout.Space(10);
        }
        
        GUILayout.Label("说明:", EditorStyles.helpBox);
        EditorGUILayout.HelpBox(
            "此工具会自动将所有地形贴图按16x16网格切割。\n" +
            "确保已将SPD的PNG文件复制到:\n" +
            "Assets/_Project/Art/Tiles/Environment/\n\n" +
            "✅ 可以多次执行，不会有问题！\n" +
            "📊 查看Console窗口获取详细日志", 
            MessageType.Info);
        
        if (GUILayout.Button("打开Console查看详细日志", GUILayout.Height(30)))
        {
            EditorWindow.GetWindow(System.Type.GetType("UnityEditor.ConsoleWindow,UnityEditor"));
        }
    }
    
    private void SliceAllTiles()
    {
        string[] paths = {
            "Assets/_Project/Art/Tiles/Environment/tiles_sewers.png",
            "Assets/_Project/Art/Tiles/Environment/tiles_prison.png",
            "Assets/_Project/Art/Tiles/Environment/tiles_caves.png",
            "Assets/_Project/Art/Tiles/Environment/tiles_city.png",
            "Assets/_Project/Art/Tiles/Environment/tiles_halls.png",
            "Assets/_Project/Art/Tiles/Environment/terrain_features.png"
        };
        
        int successCount = 0;
        int failCount = 0;
        
        foreach (string path in paths)
        {
            if (File.Exists(path))
            {
                if (SliceSprite(path, 16, 16))
                {
                    successCount++;
                    Debug.Log($"✓ 切割成功: {Path.GetFileName(path)}");
                }
                else
                {
                    failCount++;
                    Debug.LogWarning($"✗ 切割失败: {Path.GetFileName(path)}");
                }
            }
            else
            {
                Debug.LogWarning($"⚠ 文件不存在: {path}");
                failCount++;
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        lastResult = $"✅ 切割完成！\n成功: {successCount}/{paths.Length}\n失败: {failCount}";
        
        EditorUtility.DisplayDialog("切割完成", 
            $"✅ 成功切割: {successCount}/{paths.Length}\n" +
            $"❌ 失败: {failCount}\n\n" +
            $"查看Console窗口获取详细信息", 
            "确定");
    }
    
    private bool SliceSprite(string path, int tileWidth, int tileHeight)
    {
        try
        {
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
                return false;
            
            // 确保是Multiple模式
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.isReadable = true;
            
            // 应用设置
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
            
            // 加载贴图获取尺寸
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (texture == null)
                return false;
            
            int columns = texture.width / tileWidth;
            int rows = texture.height / tileHeight;
            
            // 创建Sprite数组
            System.Collections.Generic.List<SpriteMetaData> spritesheet = 
                new System.Collections.Generic.List<SpriteMetaData>();
            
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    SpriteMetaData smd = new SpriteMetaData();
                    smd.pivot = new Vector2(0.5f, 0.5f);
                    smd.alignment = (int)SpriteAlignment.Center;
                    smd.name = $"{Path.GetFileNameWithoutExtension(path)}_{y}_{x}";
                    
                    // Unity的Sprite坐标系是从底部开始的
                    smd.rect = new Rect(
                        x * tileWidth, 
                        texture.height - (y + 1) * tileHeight, 
                        tileWidth, 
                        tileHeight
                    );
                    
                    spritesheet.Add(smd);
                }
            }
            
            // 应用切割
            importer.spritesheet = spritesheet.ToArray();
            EditorUtility.SetDirty(importer);
            importer.SaveAndReimport();
            
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"切割失败 {path}: {e.Message}");
            return false;
        }
    }
}

