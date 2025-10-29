using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// Buffs.png Sprite切割工具 - 快速切割为8x8的Sprite
/// </summary>
public class BuffsSpriteSlicerTool : EditorWindow
{
    private int spriteSize = 8; // SPD的Buff图标是8x8像素

    [MenuItem("Tools/SPD/Slice Buffs Sprite")]
    public static void ShowWindow()
    {
        GetWindow<BuffsSpriteSlicerTool>("Buffs切割工具");
    }

    private void OnGUI()
    {
        GUILayout.Label("Buffs.png 切割工具", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "SPD的Buff图标是8x8像素。\n" +
            "buffs.png (128x64) 将切割为 16x8 = 128个Sprite。\n" +
            "切割后可以在Inspector中看到多个子Sprite。",
            MessageType.Info
        );

        GUILayout.Space(10);

        // 切片大小选择
        EditorGUILayout.LabelField("切片大小", EditorStyles.boldLabel);
        spriteSize = EditorGUILayout.IntSlider("像素", spriteSize, 4, 16);
        
        GUILayout.Space(5);

        if (GUILayout.Button($"切割 buffs.png ({spriteSize}x{spriteSize})", GUILayout.Height(40)))
        {
            SliceBuffsSprite();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("检查切割状态"))
        {
            CheckSliceStatus();
        }
    }

    private void SliceBuffsSprite()
    {
        // 查找buffs.png
        string[] possiblePaths = new string[]
        {
            "Assets/_Project/Art/UI/buffs.png",
            "Assets/_Project/Art/UI/SPD/buffs.png"
        };

        string buffsPath = null;
        foreach (string path in possiblePaths)
        {
            if (File.Exists(path))
            {
                buffsPath = path;
                break;
            }
        }

        if (buffsPath == null)
        {
            EditorUtility.DisplayDialog(
                "错误",
                "未找到buffs.png文件！\n\n请确保文件在以下位置之一：\n" +
                "- Assets/_Project/Art/UI/buffs.png\n" +
                "- Assets/_Project/Art/UI/SPD/buffs.png",
                "确定"
            );
            return;
        }

        Debug.Log($"<color=cyan>找到buffs.png: {buffsPath}</color>");

        // 获取TextureImporter
        TextureImporter importer = AssetImporter.GetAtPath(buffsPath) as TextureImporter;
        if (importer == null)
        {
            EditorUtility.DisplayDialog("错误", "无法获取TextureImporter！", "确定");
            return;
        }

        // 配置导入设置
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple; // 关键：设置为Multiple
        importer.spritePixelsPerUnit = spriteSize; // 使用切片大小作为PPU
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = 2048;
        importer.mipmapEnabled = false;

        Debug.Log("<color=cyan>导入设置已配置</color>");

        // 先保存导入设置
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        // 等待导入完成
        AssetDatabase.Refresh();

        // 重新获取importer
        importer = AssetImporter.GetAtPath(buffsPath) as TextureImporter;

        // 加载纹理获取尺寸
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(buffsPath);
        if (texture == null)
        {
            EditorUtility.DisplayDialog("错误", "无法加载纹理！", "确定");
            return;
        }

        int width = texture.width;
        int height = texture.height;
        int cols = width / spriteSize;
        int rows = height / spriteSize;

        Debug.Log($"<color=cyan>纹理尺寸: {width}x{height}, 切片大小: {spriteSize}x{spriteSize}, 将切割为 {cols}x{rows} = {cols * rows} 个Sprite</color>");

        // 创建Sprite切割数据
        var spritesheet = new SpriteMetaData[cols * rows];

        int index = 0;
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < cols; x++)
            {
                SpriteMetaData smd = new SpriteMetaData();
                smd.name = $"buff_{index}";
                // Unity的纹理坐标从左下角开始，所以需要翻转Y坐标
                smd.rect = new Rect(x * spriteSize, (rows - 1 - y) * spriteSize, spriteSize, spriteSize);
                smd.pivot = new Vector2(0.5f, 0.5f);
                smd.alignment = (int)SpriteAlignment.Center;
                
                spritesheet[index] = smd;
                index++;
            }
        }

        // 应用切割
        importer.spritesheet = spritesheet;
        
        EditorUtility.SetDirty(importer);
        importer.SaveAndReimport();

        // 刷新Asset Database
        AssetDatabase.Refresh();

        // 显示结果
        EditorUtility.DisplayDialog(
            "完成",
            $"buffs.png已成功切割！\n\n" +
            $"纹理尺寸: {width}x{height}\n" +
            $"切片大小: {spriteSize}x{spriteSize}\n" +
            $"切割数量: {spritesheet.Length} 个Sprite ({cols}列 x {rows}行)\n" +
            $"命名: buff_0 到 buff_{spritesheet.Length - 1}\n\n" +
            $"请在Project窗口中选择buffs.png，\n" +
            $"在Inspector中可以看到所有子Sprite。",
            "确定"
        );

        Debug.Log($"<color=green>✓ buffs.png已切割为 {spritesheet.Length} 个Sprite</color>");

        // 选中并高亮buffs.png
        Object asset = AssetDatabase.LoadAssetAtPath<Object>(buffsPath);
        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset);
    }

    private void CheckSliceStatus()
    {
        string[] possiblePaths = new string[]
        {
            "Assets/_Project/Art/UI/buffs.png",
            "Assets/_Project/Art/UI/SPD/buffs.png"
        };

        string buffsPath = null;
        foreach (string path in possiblePaths)
        {
            if (File.Exists(path))
            {
                buffsPath = path;
                break;
            }
        }

        if (buffsPath == null)
        {
            EditorUtility.DisplayDialog("错误", "未找到buffs.png文件！", "确定");
            return;
        }

        TextureImporter importer = AssetImporter.GetAtPath(buffsPath) as TextureImporter;
        if (importer == null)
        {
            EditorUtility.DisplayDialog("错误", "无法获取TextureImporter！", "确定");
            return;
        }

        // 加载所有子Sprite
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(buffsPath);
        int spriteCount = 0;
        foreach (Object obj in sprites)
        {
            if (obj is Sprite)
            {
                spriteCount++;
            }
        }

        string status = $"文件: {buffsPath}\n\n" +
                       $"Sprite模式: {importer.spriteImportMode}\n" +
                       $"子Sprite数量: {spriteCount}\n" +
                       $"Pixels Per Unit: {importer.spritePixelsPerUnit}\n" +
                       $"Filter Mode: {importer.filterMode}\n\n";

        if (importer.spriteImportMode == SpriteImportMode.Multiple && spriteCount > 1)
        {
            status += "✅ 已切割！";
        }
        else
        {
            status += "❌ 未切割，请点击\"切割 buffs.png\"按钮。";
        }

        EditorUtility.DisplayDialog("切割状态", status, "确定");
        Debug.Log($"<color=cyan>Buffs.png状态: {status}</color>");
    }
}

