using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

/// <summary>
/// 自动配置LevelRenderer的Tile引用
/// </summary>
public class ConfigureLevelRendererTiles : EditorWindow
{
    [MenuItem("Tools/Dungeon/Configure LevelRenderer Tiles")]
    static void ShowWindow()
    {
        var window = GetWindow<ConfigureLevelRendererTiles>("配置LevelRenderer Tiles");
        window.minSize = new Vector2(400, 300);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("配置LevelRenderer的Tile引用", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "此工具将自动为LevelRenderer配置Sewers区域的Tile引用：\n" +
            "- Floor Tile: tiles_sewers_0_0 (地板)\n" +
            "- Wall Tile: tiles_sewers_4_0 (墙壁)\n" +
            "- Entrance Tile: tiles_sewers_1_6 (入口楼梯)\n" +
            "- Exit Tile: tiles_sewers_1_7 (出口楼梯)",
            MessageType.Info);

        GUILayout.Space(10);

        if (GUILayout.Button("自动配置Sewers Tiles", GUILayout.Height(40)))
        {
            ConfigureSewersTiles();
        }

        GUILayout.Space(20);
        
        if (GUILayout.Button("验证配置", GUILayout.Height(30)))
        {
            ValidateConfiguration();
        }
    }

    private void ConfigureSewersTiles()
    {
        // 查找LevelRenderer
        LevelRenderer renderer = FindFirstObjectByType<LevelRenderer>();
        if (renderer == null)
        {
            EditorUtility.DisplayDialog("错误", "场景中未找到LevelRenderer组件！", "确定");
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

        // 验证Tile是否加载成功
        int successCount = 0;
        string errorMsg = "";

        if (floorTile == null)
            errorMsg += "- 未找到Floor Tile: " + floorTilePath + "\n";
        else
            successCount++;

        if (wallTile == null)
            errorMsg += "- 未找到Wall Tile: " + wallTilePath + "\n";
        else
            successCount++;

        if (entranceTile == null)
            errorMsg += "- 未找到Entrance Tile: " + entranceTilePath + "\n";
        else
            successCount++;

        if (exitTile == null)
            errorMsg += "- 未找到Exit Tile: " + exitTilePath + "\n";
        else
            successCount++;

        if (successCount < 4)
        {
            EditorUtility.DisplayDialog("警告", 
                "部分Tile未找到：\n" + errorMsg + 
                "\n已成功加载 " + successCount + "/4 个Tile。\n" +
                "请确保已运行'Tools/Dungeon/Auto Slice Sprites'工具。", 
                "确定");
        }

        // 使用SerializedObject设置字段
        SerializedObject serializedRenderer = new SerializedObject(renderer);
        
        if (floorTile != null)
        {
            SerializedProperty floorProp = serializedRenderer.FindProperty("floorTile");
            if (floorProp != null)
            {
                floorProp.objectReferenceValue = floorTile;
                Debug.Log("✓ Floor Tile 配置成功");
            }
        }

        if (wallTile != null)
        {
            SerializedProperty wallProp = serializedRenderer.FindProperty("wallTile");
            if (wallProp != null)
            {
                wallProp.objectReferenceValue = wallTile;
                Debug.Log("✓ Wall Tile 配置成功");
            }
        }

        if (entranceTile != null)
        {
            SerializedProperty entranceProp = serializedRenderer.FindProperty("entranceTile");
            if (entranceProp != null)
            {
                entranceProp.objectReferenceValue = entranceTile;
                Debug.Log("✓ Entrance Tile 配置成功");
            }
        }

        if (exitTile != null)
        {
            SerializedProperty exitProp = serializedRenderer.FindProperty("exitTile");
            if (exitProp != null)
            {
                exitProp.objectReferenceValue = exitTile;
                Debug.Log("✓ Exit Tile 配置成功");
            }
        }

        // 应用修改
        serializedRenderer.ApplyModifiedProperties();
        
        // 标记为已修改并保存
        EditorUtility.SetDirty(renderer);
        
        // 保存场景
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();

        EditorUtility.DisplayDialog("完成", 
            "LevelRenderer Tiles配置完成！\n" +
            "成功配置 " + successCount + "/4 个Tile。\n\n" +
            "现在可以进入Play模式测试地牢可视化效果。", 
            "确定");
    }

    private void ValidateConfiguration()
    {
        LevelRenderer renderer = FindFirstObjectByType<LevelRenderer>();
        if (renderer == null)
        {
            EditorUtility.DisplayDialog("错误", "场景中未找到LevelRenderer组件！", "确定");
            return;
        }

        SerializedObject serializedRenderer = new SerializedObject(renderer);
        string report = "LevelRenderer配置状态：\n\n";

        // 检查各个Tile字段
        SerializedProperty floorProp = serializedRenderer.FindProperty("floorTile");
        SerializedProperty wallProp = serializedRenderer.FindProperty("wallTile");
        SerializedProperty entranceProp = serializedRenderer.FindProperty("entranceTile");
        SerializedProperty exitProp = serializedRenderer.FindProperty("exitTile");

        report += "Tile资源：\n";
        report += "Floor Tile: " + (floorProp.objectReferenceValue != null ? "✓ 已配置" : "✗ 未配置") + "\n";
        report += "Wall Tile: " + (wallProp.objectReferenceValue != null ? "✓ 已配置" : "✗ 未配置") + "\n";
        report += "Entrance Tile: " + (entranceProp.objectReferenceValue != null ? "✓ 已配置" : "✗ 未配置") + "\n";
        report += "Exit Tile: " + (exitProp.objectReferenceValue != null ? "✓ 已配置" : "✗ 未配置") + "\n";

        // 检查Tilemap引用
        SerializedProperty groundProp = serializedRenderer.FindProperty("groundTilemap");
        SerializedProperty wallTilemapProp = serializedRenderer.FindProperty("wallTilemap");
        SerializedProperty decorationProp = serializedRenderer.FindProperty("decorationTilemap");

        report += "\nTilemap引用：\n";
        report += "Ground Tilemap: " + (groundProp.objectReferenceValue != null ? "✓ 已配置" : "✗ 未配置") + "\n";
        report += "Wall Tilemap: " + (wallTilemapProp.objectReferenceValue != null ? "✓ 已配置" : "✗ 未配置") + "\n";
        report += "Decoration Tilemap: " + (decorationProp.objectReferenceValue != null ? "✓ 已配置" : "✗ 未配置") + "\n";

        Debug.Log(report);
        EditorUtility.DisplayDialog("配置验证", report, "确定");
    }
}

