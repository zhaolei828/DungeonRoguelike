using UnityEditor;
using UnityEngine;

/// <summary>
/// 自动切片Warrior Sprite
/// </summary>
public class AutoSliceWarriorSprite
{
    [MenuItem("Tools/Hero/Auto Slice Warrior Sprite Now")]
    public static void SliceNow()
    {
        string path = "Assets/_Project/Art/Sprites/Characters/warrior.png";
        
        // 强制刷新资源
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        
        if (importer == null)
        {
            Debug.LogError("未找到 warrior.png 的 TextureImporter！");
            return;
        }

        // 确保设置正确
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 16;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.alphaIsTransparency = true;

        // 读取纹理
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (texture == null)
        {
            Debug.LogError("无法加载 warrior.png！");
            return;
        }

        int width = texture.width;
        int height = texture.height;
        
        Debug.Log($"<color=cyan>Warrior sprite 尺寸: {width}x{height}</color>");

        // SPD的warrior.png是横向排列的动画帧
        // 通常每帧12x15像素，但我们需要根据实际尺寸计算
        
        // 尝试不同的帧尺寸
        int frameWidth = 12;
        int frameHeight = 15;
        
        // 如果尺寸不匹配，尝试16x16
        if (width % 12 != 0 || height % 15 != 0)
        {
            frameWidth = 16;
            frameHeight = 16;
            Debug.Log($"使用16x16帧尺寸");
        }
        else
        {
            Debug.Log($"使用12x15帧尺寸");
        }

        int framesPerRow = width / frameWidth;
        int rows = height / frameHeight;

        Debug.Log($"<color=yellow>检测到 {framesPerRow} 帧/行, {rows} 行, 共 {framesPerRow * rows} 帧</color>");

        // 创建sprite元数据
        var spritesheet = new SpriteMetaData[framesPerRow * rows];
        int index = 0;

        // 定义动画状态名称
        string[] animStates = { "idle", "walk_down", "walk_up", "walk_left", "walk_right", "attack", "die" };
        int framesPerAnim = framesPerRow / animStates.Length;
        if (framesPerAnim == 0) framesPerAnim = 1;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < framesPerRow; col++)
            {
                // 确定动画状态
                int animIndex = col / framesPerAnim;
                int frameInAnim = col % framesPerAnim;
                string animName = animIndex < animStates.Length ? animStates[animIndex] : "extra";
                
                SpriteMetaData smd = new SpriteMetaData();
                smd.name = $"warrior_{animName}_{frameInAnim}";
                smd.rect = new Rect(
                    col * frameWidth,
                    height - (row + 1) * frameHeight, // Unity的Y轴从下往上
                    frameWidth,
                    frameHeight
                );
                smd.pivot = new Vector2(0.5f, 0.5f);
                smd.alignment = (int)SpriteAlignment.Center;
                
                spritesheet[index++] = smd;
                
                Debug.Log($"  Sprite {index}: {smd.name} at ({smd.rect.x}, {smd.rect.y})");
            }
        }

        // 应用切片
        importer.spritesheet = spritesheet;
        EditorUtility.SetDirty(importer);
        AssetDatabase.WriteImportSettingsIfDirty(path);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

        Debug.Log($"<color=green>✓ warrior.png 切片完成！共 {spritesheet.Length} 个sprite</color>");
        
        // 显示结果
        EditorUtility.DisplayDialog("完成", 
            $"warrior.png 切片完成！\n" +
            $"尺寸: {width}x{height}\n" +
            $"帧尺寸: {frameWidth}x{frameHeight}\n" +
            $"共生成 {spritesheet.Length} 个sprite帧", 
            "确定");
    }
}

