using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏场景一键设置工具
/// </summary>
public class GameSceneSetup : EditorWindow
{
    [MenuItem("Tools/DungeonRoguelike/一键设置游戏场景")]
    public static void ShowWindow()
    {
        GetWindow<GameSceneSetup>("场景设置");
    }
    
    [MenuItem("Tools/DungeonRoguelike/快速设置当前场景 %&S")] // Ctrl+Alt+S
    public static void QuickSetup()
    {
        if (EditorUtility.DisplayDialog("快速设置", "是否为当前场景添加所有必需组件？", "确定", "取消"))
        {
            SetupCurrentScene();
        }
    }
    
    private void OnGUI()
    {
        EditorGUILayout.LabelField("游戏场景一键设置", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "此工具将为当前场景自动添加:\n" +
            "• LevelManager（关卡管理）\n" +
            "• HealthBarManager（血条管理）\n" +
            "• DamageNumberManager（伤害数字）\n" +
            "• CameraFollow（摄像机跟随）\n" +
            "• Grid（网格系统）",
            MessageType.Info);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("设置当前场景", GUILayout.Height(40)))
        {
            SetupCurrentScene();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("单独设置:", EditorStyles.boldLabel);
        
        if (GUILayout.Button("只添加管理器"))
        {
            SetupManagers();
        }
        
        if (GUILayout.Button("只设置摄像机"))
        {
            SetupCamera();
        }
        
        if (GUILayout.Button("只添加Grid"))
        {
            SetupGrid();
        }
    }
    
    private static void SetupCurrentScene()
    {
        Debug.Log("<color=cyan>开始设置场景...</color>");
        
        // 1. 设置管理器
        SetupManagers();
        
        // 2. 设置摄像机
        SetupCamera();
        
        // 3. 设置Grid
        SetupGrid();
        
        // 4. 标记场景为已修改
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        
        Debug.Log("<color=green>✓ 场景设置完成！</color>");
        EditorUtility.DisplayDialog("完成", "场景设置完成！\n现在可以按Play测试游戏。", "确定");
    }
    
    private static void SetupManagers()
    {
        // 查找或创建Managers容器
        GameObject managersGO = GameObject.Find("Managers");
        if (managersGO == null)
        {
            managersGO = new GameObject("Managers");
            Debug.Log("✓ 创建 Managers GameObject");
        }
        
        // 添加LevelManager
        if (managersGO.GetComponent<LevelManager>() == null)
        {
            managersGO.AddComponent<LevelManager>();
            Debug.Log("✓ 添加 LevelManager");
        }
        
        // 查找或创建UIManagers
        GameObject uiManagersGO = GameObject.Find("UIManagers");
        if (uiManagersGO == null)
        {
            uiManagersGO = new GameObject("UIManagers");
            Debug.Log("✓ 创建 UIManagers GameObject");
        }
        
        // 添加HealthBarManager
        if (uiManagersGO.GetComponent<HealthBarManager>() == null)
        {
            uiManagersGO.AddComponent<HealthBarManager>();
            Debug.Log("✓ 添加 HealthBarManager");
        }
        
        // 添加DamageNumberManager
        if (uiManagersGO.GetComponent<DamageNumberManager>() == null)
        {
            uiManagersGO.AddComponent<DamageNumberManager>();
            Debug.Log("✓ 添加 DamageNumberManager");
        }
        
        // 查找或创建GameManager
        GameObject gameManagerGO = GameObject.Find("GameManager");
        if (gameManagerGO == null)
        {
            gameManagerGO = new GameObject("GameManager");
            if (gameManagerGO.GetComponent<GameManager>() == null)
            {
                gameManagerGO.AddComponent<GameManager>();
                Debug.Log("✓ 创建 GameManager");
            }
        }
        
        // 添加RespawnManager到Managers
        if (managersGO.GetComponent<RespawnManager>() == null)
        {
            RespawnManager respawnManager = managersGO.AddComponent<RespawnManager>();
            Debug.Log("✓ 添加 RespawnManager");
            
            // 设置默认值
            SerializedObject so = new SerializedObject(respawnManager);
            so.FindProperty("respawnDelay").floatValue = 2f;
            so.FindProperty("enableRespawn").boolValue = true;
            so.FindProperty("respawnHealthPercent").floatValue = 1f;
            so.ApplyModifiedProperties();
        }
    }
    
    private static void SetupCamera()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogWarning("未找到Main Camera!");
            return;
        }
        
        // 添加CameraFollow
        if (mainCamera.GetComponent<CameraFollow>() == null)
        {
            CameraFollow cameraFollow = mainCamera.gameObject.AddComponent<CameraFollow>();
            Debug.Log("✓ 添加 CameraFollow");
            
            // 设置默认值
            SerializedObject so = new SerializedObject(cameraFollow);
            so.FindProperty("autoFindHero").boolValue = true;
            so.FindProperty("smoothSpeed").floatValue = 5f;
            so.FindProperty("offset").vector3Value = new Vector3(0, 0, -10);
            so.ApplyModifiedProperties();
        }
        
        // 设置摄像机为Orthographic
        if (!mainCamera.orthographic)
        {
            mainCamera.orthographic = true;
            mainCamera.orthographicSize = 8;
            Debug.Log("✓ 设置摄像机为正交模式");
        }
    }
    
    private static void SetupGrid()
    {
        // 查找或创建Grid
        GameObject gridGO = GameObject.Find("Grid");
        if (gridGO == null)
        {
            gridGO = new GameObject("Grid");
            Grid grid = gridGO.AddComponent<Grid>();
            grid.cellSize = new Vector3(1, 1, 0);
            Debug.Log("✓ 创建 Grid");
            
            // 创建Tilemap
            GameObject tilemapGO = new GameObject("Tilemap");
            tilemapGO.transform.SetParent(gridGO.transform);
            tilemapGO.AddComponent<UnityEngine.Tilemaps.Tilemap>();
            tilemapGO.AddComponent<UnityEngine.Tilemaps.TilemapRenderer>();
            Debug.Log("✓ 创建 Tilemap");
        }
    }
}

