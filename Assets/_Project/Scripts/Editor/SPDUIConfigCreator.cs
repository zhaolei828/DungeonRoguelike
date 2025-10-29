using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// SPD UI配置创建工具 - 自动创建配置文件并切割Sprite
/// </summary>
public class SPDUIConfigCreator : EditorWindow
{
    [MenuItem("Tools/SPD/Create UI Configs")]
    public static void ShowWindow()
    {
        GetWindow<SPDUIConfigCreator>("SPD UI配置创建");
    }

    private void OnGUI()
    {
        GUILayout.Label("SPD UI配置创建工具", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "此工具将：\n" +
            "1. 创建BuffIconConfig配置文件\n" +
            "2. 创建UIThemeConfig配置文件\n" +
            "3. 自动切割buffs.png为16x16的Sprite\n" +
            "4. 配置所有UI素材的导入设置",
            MessageType.Info
        );

        GUILayout.Space(10);

        if (GUILayout.Button("创建所有配置", GUILayout.Height(40)))
        {
            CreateAllConfigs();
        }

        GUILayout.Space(10);

        EditorGUILayout.LabelField("单独操作", EditorStyles.boldLabel);

        if (GUILayout.Button("1. 切割buffs.png"))
        {
            SliceBuffsSprite();
        }

        if (GUILayout.Button("2. 创建BuffIconConfig"))
        {
            CreateBuffIconConfig();
        }

        if (GUILayout.Button("3. 创建UIThemeConfig"))
        {
            CreateUIThemeConfig();
        }

        if (GUILayout.Button("4. 配置所有UI素材"))
        {
            ConfigureAllUIAssets();
        }
    }

    private void CreateAllConfigs()
    {
        EditorUtility.DisplayProgressBar("创建配置", "正在处理...", 0f);

        try
        {
            // 1. 切割buffs.png
            EditorUtility.DisplayProgressBar("创建配置", "切割buffs.png...", 0.25f);
            SliceBuffsSprite();

            // 2. 配置UI素材
            EditorUtility.DisplayProgressBar("创建配置", "配置UI素材...", 0.5f);
            ConfigureAllUIAssets();

            // 3. 创建BuffIconConfig
            EditorUtility.DisplayProgressBar("创建配置", "创建BuffIconConfig...", 0.75f);
            CreateBuffIconConfig();

            // 4. 创建UIThemeConfig
            EditorUtility.DisplayProgressBar("创建配置", "创建UIThemeConfig...", 1f);
            CreateUIThemeConfig();

            EditorUtility.ClearProgressBar();

            EditorUtility.DisplayDialog(
                "完成",
                "所有配置已创建完成！\n\n" +
                "配置文件位置：\n" +
                "- Assets/_Project/Resources/BuffIconConfig.asset\n" +
                "- Assets/_Project/Resources/UIThemeConfig.asset\n\n" +
                "请在Inspector中手动分配Buff图标。",
                "确定"
            );

            Debug.Log("<color=green>✓ SPD UI配置创建完成！</color>");
        }
        catch (System.Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("错误", $"创建配置时出错：\n{e.Message}", "确定");
            Debug.LogError($"创建配置失败：{e}");
        }
    }

    private void SliceBuffsSprite()
    {
        string[] paths = new string[]
        {
            "Assets/_Project/Art/UI/buffs.png",
            "Assets/_Project/Art/UI/SPD/buffs.png"
        };

        string buffsPath = null;
        foreach (string path in paths)
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

        // 配置导入设置
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Multiple;
        importer.spritePixelsPerUnit = 16;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = 2048;
        importer.mipmapEnabled = false;

        // 自动切割为16x16
        var spritesheet = new SpriteMetaData[0];
        
        // 获取纹理尺寸
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(buffsPath);
        if (texture != null)
        {
            int cols = texture.width / 16;
            int rows = texture.height / 16;
            
            spritesheet = new SpriteMetaData[cols * rows];
            
            int index = 0;
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    SpriteMetaData smd = new SpriteMetaData
                    {
                        name = $"buff_{index}",
                        rect = new Rect(x * 16, (rows - 1 - y) * 16, 16, 16),
                        pivot = new Vector2(0.5f, 0.5f),
                        alignment = (int)SpriteAlignment.Center
                    };
                    spritesheet[index] = smd;
                    index++;
                }
            }
        }

        importer.spritesheet = spritesheet;
        importer.SaveAndReimport();

        Debug.Log($"<color=cyan>✓ buffs.png已切割为 {spritesheet.Length} 个Sprite</color>");
    }

    private void CreateBuffIconConfig()
    {
        string resourcesPath = "Assets/_Project/Resources";
        if (!Directory.Exists(resourcesPath))
        {
            Directory.CreateDirectory(resourcesPath);
        }

        string configPath = $"{resourcesPath}/BuffIconConfig.asset";

        // 检查是否已存在
        BuffIconConfig existingConfig = AssetDatabase.LoadAssetAtPath<BuffIconConfig>(configPath);
        if (existingConfig != null)
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "确认",
                "BuffIconConfig已存在，是否覆盖？",
                "覆盖",
                "取消"
            );

            if (!overwrite)
            {
                Debug.Log("已取消创建BuffIconConfig");
                return;
            }
        }

        // 创建配置
        BuffIconConfig config = ScriptableObject.CreateInstance<BuffIconConfig>();

        // 自动生成映射
        System.Reflection.MethodInfo method = typeof(BuffIconConfig).GetMethod(
            "AutoGenerateMappings",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );

        if (method != null)
        {
            method.Invoke(config, null);
        }

        // 保存
        AssetDatabase.CreateAsset(config, configPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 选中
        Selection.activeObject = config;
        EditorGUIUtility.PingObject(config);

        Debug.Log($"<color=green>✓ BuffIconConfig已创建：{configPath}</color>");
    }

    private void CreateUIThemeConfig()
    {
        string resourcesPath = "Assets/_Project/Resources";
        if (!Directory.Exists(resourcesPath))
        {
            Directory.CreateDirectory(resourcesPath);
        }

        string configPath = $"{resourcesPath}/UIThemeConfig.asset";

        // 检查是否已存在
        UIThemeConfig existingConfig = AssetDatabase.LoadAssetAtPath<UIThemeConfig>(configPath);
        if (existingConfig != null)
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "确认",
                "UIThemeConfig已存在，是否覆盖？",
                "覆盖",
                "取消"
            );

            if (!overwrite)
            {
                Debug.Log("已取消创建UIThemeConfig");
                return;
            }
        }

        // 创建配置
        UIThemeConfig config = ScriptableObject.CreateInstance<UIThemeConfig>();

        // 自动加载素材
        string[] uiPaths = new string[]
        {
            "Assets/_Project/Art/UI",
            "Assets/_Project/Art/UI/SPD"
        };

        foreach (string basePath in uiPaths)
        {
            if (config.panelChrome == null)
                config.panelChrome = AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/chrome.png");
            
            if (config.menuPane == null)
                config.menuPane = AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/menu_pane.png");
            
            if (config.statusPane == null)
                config.statusPane = AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/status_pane.png");
            
            if (config.surface == null)
                config.surface = AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/surface.png");
            
            if (config.menuButton == null)
                config.menuButton = AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/menu_button.png");
            
            if (config.talentButton == null)
                config.talentButton = AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/talent_button.png");
            
            if (config.bossHpBar == null)
                config.bossHpBar = AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/boss_hp.png");
            
            if (config.shadow == null)
                config.shadow = AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/shadow.png");
            
            if (config.toolbar == null)
                config.toolbar = AssetDatabase.LoadAssetAtPath<Sprite>($"{basePath}/toolbar.png");
        }

        // 保存
        AssetDatabase.CreateAsset(config, configPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 选中
        Selection.activeObject = config;
        EditorGUIUtility.PingObject(config);

        Debug.Log($"<color=green>✓ UIThemeConfig已创建：{configPath}</color>");
    }

    private void ConfigureAllUIAssets()
    {
        string[] uiPaths = new string[]
        {
            "Assets/_Project/Art/UI",
            "Assets/_Project/Art/UI/SPD"
        };

        int configuredCount = 0;

        foreach (string basePath in uiPaths)
        {
            if (!Directory.Exists(basePath))
                continue;

            string[] files = Directory.GetFiles(basePath, "*.png");

            foreach (string file in files)
            {
                string relativePath = file.Replace(Application.dataPath, "Assets").Replace("\\", "/");
                
                TextureImporter importer = AssetImporter.GetAtPath(relativePath) as TextureImporter;
                if (importer != null)
                {
                    importer.textureType = TextureImporterType.Sprite;
                    importer.spriteImportMode = SpriteImportMode.Single;
                    importer.spritePixelsPerUnit = 16;
                    importer.filterMode = FilterMode.Point;
                    importer.textureCompression = TextureImporterCompression.Uncompressed;
                    importer.maxTextureSize = 2048;
                    importer.mipmapEnabled = false;

                    importer.SaveAndReimport();
                    configuredCount++;
                }
            }
        }

        Debug.Log($"<color=cyan>✓ 已配置 {configuredCount} 个UI素材</color>");
    }
}

