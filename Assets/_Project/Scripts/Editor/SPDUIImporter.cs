using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// SPD UI素材导入工具 - 从SPD项目导入UI素材
/// </summary>
public class SPDUIImporter : EditorWindow
{
    private string spdProjectPath = "";
    private bool importButtons = true;
    private bool importPanels = true;
    private bool importIcons = true;
    private bool importBars = true;

    [MenuItem("Tools/SPD/Import UI Assets")]
    public static void ShowWindow()
    {
        GetWindow<SPDUIImporter>("SPD UI素材导入");
    }

    private void OnGUI()
    {
        GUILayout.Label("SPD UI素材导入工具", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "此工具用于从Shattered Pixel Dungeon项目导入UI素材。\n" +
            "注意：当前战斗UI系统使用程序化创建，不依赖这些素材。\n" +
            "导入素材仅用于美化和增强视觉效果。",
            MessageType.Info
        );

        GUILayout.Space(10);

        // SPD项目路径
        EditorGUILayout.LabelField("SPD项目路径", EditorStyles.boldLabel);
        EditorGUILayout.BeginHorizontal();
        spdProjectPath = EditorGUILayout.TextField(spdProjectPath);
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            spdProjectPath = EditorUtility.OpenFolderPanel(
                "选择SPD项目根目录",
                "",
                ""
            );
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        // 导入选项
        EditorGUILayout.LabelField("导入选项", EditorStyles.boldLabel);
        importButtons = EditorGUILayout.Toggle("按钮素材", importButtons);
        importPanels = EditorGUILayout.Toggle("面板/窗口", importPanels);
        importIcons = EditorGUILayout.Toggle("图标集", importIcons);
        importBars = EditorGUILayout.Toggle("血条/进度条", importBars);

        GUILayout.Space(10);

        // 导入按钮
        GUI.enabled = !string.IsNullOrEmpty(spdProjectPath);
        if (GUILayout.Button("开始导入", GUILayout.Height(40)))
        {
            ImportUIAssets();
        }
        GUI.enabled = true;

        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "导入路径：Assets/_Project/Art/UI/SPD/\n" +
            "导入后需要手动配置Sprite切割和引用。",
            MessageType.Warning
        );

        GUILayout.Space(10);

        if (GUILayout.Button("打开导入指南文档"))
        {
            string guidePath = Path.Combine(Application.dataPath, "../Docs/SPD_UI素材导入指南.md");
            if (File.Exists(guidePath))
            {
                System.Diagnostics.Process.Start(guidePath);
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "未找到导入指南文档", "确定");
            }
        }
    }

    private void ImportUIAssets()
    {
        // 验证路径
        string interfacesPath = Path.Combine(spdProjectPath, "core/src/main/assets/interfaces");
        if (!Directory.Exists(interfacesPath))
        {
            EditorUtility.DisplayDialog(
                "错误",
                $"未找到interfaces文件夹！\n路径：{interfacesPath}\n\n请确保选择的是SPD项目根目录。",
                "确定"
            );
            return;
        }

        // 创建目标目录
        string targetPath = "Assets/_Project/Art/UI/SPD";
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        int importedCount = 0;
        int failedCount = 0;

        // 获取所有PNG文件
        string[] files = Directory.GetFiles(interfacesPath, "*.png");

        if (files.Length == 0)
        {
            EditorUtility.DisplayDialog(
                "警告",
                "interfaces文件夹中未找到PNG文件！",
                "确定"
            );
            return;
        }

        // 显示进度条
        for (int i = 0; i < files.Length; i++)
        {
            string file = files[i];
            string fileName = Path.GetFileName(file);

            EditorUtility.DisplayProgressBar(
                "导入UI素材",
                $"正在导入：{fileName}",
                (float)i / files.Length
            );

            try
            {
                // 根据文件名判断是否导入
                bool shouldImport = ShouldImportFile(fileName);

                if (shouldImport)
                {
                    string destPath = Path.Combine(targetPath, fileName);
                    File.Copy(file, destPath, true);
                    importedCount++;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"导入失败：{fileName}\n错误：{e.Message}");
                failedCount++;
            }
        }

        EditorUtility.ClearProgressBar();

        // 刷新Asset Database
        AssetDatabase.Refresh();

        // 自动配置导入设置
        ConfigureImportSettings(targetPath);

        // 显示结果
        string message = $"导入完成！\n\n" +
                        $"成功：{importedCount} 个文件\n" +
                        $"失败：{failedCount} 个文件\n\n" +
                        $"导入位置：{targetPath}";

        EditorUtility.DisplayDialog("导入完成", message, "确定");

        Debug.Log($"<color=green>✓ SPD UI素材导入完成！成功：{importedCount}，失败：{failedCount}</color>");
    }

    private bool ShouldImportFile(string fileName)
    {
        fileName = fileName.ToLower();

        if (importButtons && (fileName.Contains("button") || fileName.Contains("btn")))
            return true;

        if (importPanels && (fileName.Contains("chrome") || fileName.Contains("panel") || 
            fileName.Contains("window") || fileName.Contains("bg")))
            return true;

        if (importIcons && (fileName.Contains("icon") || fileName.Contains("buff")))
            return true;

        if (importBars && (fileName.Contains("bar") || fileName.Contains("hp") || 
            fileName.Contains("exp")))
            return true;

        // 默认导入所有
        return true;
    }

    private void ConfigureImportSettings(string targetPath)
    {
        string[] assets = Directory.GetFiles(targetPath, "*.png");

        foreach (string assetPath in assets)
        {
            // 转换为Unity相对路径
            string relativePath = assetPath.Replace(Application.dataPath, "Assets");
            relativePath = relativePath.Replace("\\", "/");

            TextureImporter importer = AssetImporter.GetAtPath(relativePath) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Multiple;
                importer.spritePixelsPerUnit = 16;
                importer.filterMode = FilterMode.Point;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.maxTextureSize = 2048;
                importer.mipmapEnabled = false;

                importer.SaveAndReimport();
            }
        }

        Debug.Log("<color=cyan>✓ 导入设置已自动配置</color>");
    }
}

