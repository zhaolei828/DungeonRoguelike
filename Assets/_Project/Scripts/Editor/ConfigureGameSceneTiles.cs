using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 在Game.unity场景中配置LevelRenderer的Tile引用
/// </summary>
public class ConfigureGameSceneTiles : EditorWindow
{
    [MenuItem("Tools/Dungeon/Configure Game Scene Tiles")]
    static void ShowWindow()
    {
        var window = GetWindow<ConfigureGameSceneTiles>("配置Game场景Tiles");
        window.minSize = new Vector2(400, 300);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("在Game.unity场景中配置Tile引用", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "此工具将在Game.unity场景中自动为LevelRenderer配置Tile引用。\n\n" +
            "操作步骤：\n" +
            "1. 点击下面的按钮\n" +
            "2. 工具会自动打开Game.unity场景\n" +
            "3. 配置所有必需的Tile\n" +
            "4. 保存场景并返回",
            MessageType.Info);

        GUILayout.Space(10);

        if (GUILayout.Button("配置Game场景中的Tiles", GUILayout.Height(40)))
        {
            ConfigureGameSceneTiles_Internal();
        }
    }

    static void ConfigureGameSceneTiles_Internal()
    {
        // 保存当前场景
        string currentScenePath = EditorSceneManager.GetActiveScene().path;
        
        // 打开Game.unity场景
        string gameScenePath = "Assets/_Project/Scenes/Game.unity/Game.unity";
        EditorSceneManager.OpenScene(gameScenePath, OpenSceneMode.Single);
        
        // 等待一帧让场景加载完成
        EditorApplication.delayCall += () =>
        {
            ConfigureTilesInCurrentScene();
            
            // 保存Game场景
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
            
            // 返回原场景
            EditorApplication.delayCall += () =>
            {
                if (!string.IsNullOrEmpty(currentScenePath))
                {
                    EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
                }
                
                EditorUtility.DisplayDialog("完成", 
                    "Game.unity场景中的Tiles已配置完成！", 
                    "确定");
            };
        };
    }

    static void ConfigureTilesInCurrentScene()
    {
        // 查找LevelRenderer
        LevelRenderer renderer = Object.FindFirstObjectByType<LevelRenderer>();
        if (renderer == null)
        {
            Debug.LogWarning("场景中未找到LevelRenderer组件！");
            return;
        }

        // 定义Tile路径
        string basePath = "Assets/_Project/Art/Tiles/TileAssets/SewersTilePalette/";
        string floorTilePath = basePath + "tiles_sewers_0_0.asset";
        string wallTilePath = basePath + "tiles_sewers_4_0.asset";
        string entranceTilePath = basePath + "tiles_sewers_1_6.asset";
        string exitTilePath = basePath + "tiles_sewers_1_7.asset";

        // 加载Tile Assets
        Tile floorTile = AssetDatabase.LoadAssetAtPath<Tile>(floorTilePath);
        Tile wallTile = AssetDatabase.LoadAssetAtPath<Tile>(wallTilePath);
        Tile entranceTile = AssetDatabase.LoadAssetAtPath<Tile>(entranceTilePath);
        Tile exitTile = AssetDatabase.LoadAssetAtPath<Tile>(exitTilePath);

        // 使用SerializedObject设置字段
        SerializedObject serializedRenderer = new SerializedObject(renderer);
        
        int configuredCount = 0;
        
        if (floorTile != null)
        {
            SerializedProperty floorProp = serializedRenderer.FindProperty("floorTile");
            if (floorProp != null)
            {
                floorProp.objectReferenceValue = floorTile;
                configuredCount++;
                Debug.Log("✓ Floor Tile 配置成功");
            }
        }
        else
        {
            Debug.LogWarning("未找到Floor Tile: " + floorTilePath);
        }

        if (wallTile != null)
        {
            SerializedProperty wallProp = serializedRenderer.FindProperty("wallTile");
            if (wallProp != null)
            {
                wallProp.objectReferenceValue = wallTile;
                configuredCount++;
                Debug.Log("✓ Wall Tile 配置成功");
            }
        }
        else
        {
            Debug.LogWarning("未找到Wall Tile: " + wallTilePath);
        }

        if (entranceTile != null)
        {
            SerializedProperty entranceProp = serializedRenderer.FindProperty("entranceTile");
            if (entranceProp != null)
            {
                entranceProp.objectReferenceValue = entranceTile;
                configuredCount++;
                Debug.Log("✓ Entrance Tile 配置成功");
            }
        }
        else
        {
            Debug.LogWarning("未找到Entrance Tile: " + entranceTilePath);
        }

        if (exitTile != null)
        {
            SerializedProperty exitProp = serializedRenderer.FindProperty("exitTile");
            if (exitProp != null)
            {
                exitProp.objectReferenceValue = exitTile;
                configuredCount++;
                Debug.Log("✓ Exit Tile 配置成功");
            }
        }
        else
        {
            Debug.LogWarning("未找到Exit Tile: " + exitTilePath);
        }

        // 应用修改
        serializedRenderer.ApplyModifiedProperties();
        
        // 标记为已修改
        EditorUtility.SetDirty(renderer);
        
        Debug.Log($"<color=green>Game.unity场景 - Tiles配置完成！成功配置 {configuredCount}/4 个Tile。</color>");
    }
}
