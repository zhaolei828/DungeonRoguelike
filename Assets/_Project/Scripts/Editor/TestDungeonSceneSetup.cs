using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

/// <summary>
/// TestDungeon场景自动配置工具
/// </summary>
public class TestDungeonSceneSetup : EditorWindow
{
    [MenuItem("DungeonRoguelike/Setup TestDungeon Scene")]
    public static void SetupScene()
    {
        Debug.Log("=== 开始配置TestDungeon场景 ===");
        
        // 1. 确保有Grid
        Grid grid = FindObjectOfType<Grid>();
        if (grid == null)
        {
            GameObject gridGO = new GameObject("Grid");
            grid = gridGO.AddComponent<Grid>();
            Debug.Log("✓ 创建Grid");
        }
        
        // 2. 确保有Tilemaps
        SetupTilemap(grid.transform, "GroundTilemap", 0);
        SetupTilemap(grid.transform, "WallTilemap", 1);
        SetupTilemap(grid.transform, "DecorationTilemap", 2);
        
        // 3. 确保有相机
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            GameObject camGO = new GameObject("Main Camera");
            mainCam = camGO.AddComponent<Camera>();
            camGO.tag = "MainCamera";
            camGO.transform.position = new Vector3(0, 0, -10);
            mainCam.orthographic = true;
            mainCam.orthographicSize = 10;
            Debug.Log("✓ 创建Main Camera");
        }
        else
        {
            mainCam.orthographic = true;
            mainCam.orthographicSize = 10;
            Debug.Log("✓ 配置Main Camera");
        }
        
        // 4. 确保有GameSystems
        GameObject gameSystems = GameObject.Find("GameSystems");
        if (gameSystems == null)
        {
            gameSystems = new GameObject("GameSystems");
            Debug.Log("✓ 创建GameSystems");
        }
        
        // 添加核心管理器
        if (gameSystems.GetComponent<GameManager>() == null)
        {
            gameSystems.AddComponent<GameManager>();
            Debug.Log("✓ 添加GameManager");
        }
        
        if (gameSystems.GetComponent<LevelManager>() == null)
        {
            gameSystems.AddComponent<LevelManager>();
            Debug.Log("✓ 添加LevelManager");
        }
        
        // 5. 确保有LevelRenderer
        LevelRenderer renderer = FindObjectOfType<LevelRenderer>();
        if (renderer == null)
        {
            GameObject rendererGO = new GameObject("LevelRendererObject");
            renderer = rendererGO.AddComponent<LevelRenderer>();
            Debug.Log("✓ 创建LevelRenderer");
        }
        
        // 6. 确保有测试脚本
        DungeonGeneratorTest test = FindObjectOfType<DungeonGeneratorTest>();
        if (test == null)
        {
            GameObject testGO = new GameObject("DungeonGeneratorTest");
            test = testGO.AddComponent<DungeonGeneratorTest>();
            Debug.Log("✓ 创建DungeonGeneratorTest");
        }
        
        Debug.Log("=== TestDungeon场景配置完成！===");
        Debug.Log("现在可以点击Play按钮运行测试了");
        
        EditorUtility.DisplayDialog("场景配置完成", 
            "TestDungeon场景已经配置完成！\n\n" +
            "点击Play按钮即可看到地牢生成效果。\n\n" +
            "按G键可以重新生成地牢。", 
            "好的");
    }
    
    private static void SetupTilemap(Transform parent, string name, int sortingOrder)
    {
        Transform existing = parent.Find(name);
        if (existing == null)
        {
            GameObject tilemapGO = new GameObject(name);
            tilemapGO.transform.SetParent(parent);
            
            Tilemap tilemap = tilemapGO.AddComponent<Tilemap>();
            TilemapRenderer renderer = tilemapGO.AddComponent<TilemapRenderer>();
            renderer.sortingOrder = sortingOrder;
            
            Debug.Log($"✓ 创建{name}");
        }
        else
        {
            TilemapRenderer renderer = existing.GetComponent<TilemapRenderer>();
            if (renderer != null)
            {
                renderer.sortingOrder = sortingOrder;
            }
            Debug.Log($"✓ 配置{name}");
        }
    }
}

