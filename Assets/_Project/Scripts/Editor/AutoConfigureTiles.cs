using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 自动配置LevelRenderer的Tile引用（启动时执行）
/// </summary>
[InitializeOnLoad]
public class AutoConfigureTiles
{
    [MenuItem("Tools/Dungeon/Auto Configure Tiles Now")]
    static void ConfigureNow()
    {
        ConfigureSewersTiles();
    }

    static void ConfigureSewersTiles()
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
        
        // 标记为已修改并保存
        EditorUtility.SetDirty(renderer);
        
        // 保存场景
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();

        Debug.Log($"<color=green>LevelRenderer Tiles配置完成！成功配置 {configuredCount}/4 个Tile。</color>");
    }
}

