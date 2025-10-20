using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 自动在Game.unity场景中配置LevelRenderer的Tile引用（启动时执行）
/// </summary>
[InitializeOnLoad]
public class AutoConfigureGameSceneTiles
{
    [MenuItem("Tools/Dungeon/Auto Configure Game Scene Tiles Now")]
    static void ConfigureNow()
    {
        string gameScenePath = "Assets/_Project/Scenes/Game.unity/Game.unity";
        string currentScenePath = EditorSceneManager.GetActiveScene().path;
        
        // 打开Game场景
        var scene = EditorSceneManager.OpenScene(gameScenePath, OpenSceneMode.Single);
        
        // 配置Tiles
        ConfigureTilesInScene();
        
        // 保存Game场景
        EditorSceneManager.SaveScene(scene);
        
        // 返回原场景
        if (!string.IsNullOrEmpty(currentScenePath))
        {
            EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
        }
        
        Debug.Log("Game.unity场景Tiles配置完成！");
    }

    static void ConfigureTilesInScene()
    {
        LevelRenderer renderer = Object.FindFirstObjectByType<LevelRenderer>();
        if (renderer == null)
        {
            Debug.LogWarning("Game.unity场景中未找到LevelRenderer组件！");
            return;
        }

        string basePath = "Assets/_Project/Art/Tiles/TileAssets/SewersTilePalette/";
        
        Tile floorTile = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "tiles_sewers_0_0.asset");
        Tile wallTile = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "tiles_sewers_4_0.asset");
        Tile entranceTile = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "tiles_sewers_1_6.asset");
        Tile exitTile = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "tiles_sewers_1_7.asset");

        SerializedObject serializedRenderer = new SerializedObject(renderer);
        
        if (floorTile != null)
        {
            serializedRenderer.FindProperty("floorTile").objectReferenceValue = floorTile;
            Debug.Log("✓ Floor Tile 配置成功");
        }

        if (wallTile != null)
        {
            serializedRenderer.FindProperty("wallTile").objectReferenceValue = wallTile;
            Debug.Log("✓ Wall Tile 配置成功");
        }

        if (entranceTile != null)
        {
            serializedRenderer.FindProperty("entranceTile").objectReferenceValue = entranceTile;
            Debug.Log("✓ Entrance Tile 配置成功");
        }

        if (exitTile != null)
        {
            serializedRenderer.FindProperty("exitTile").objectReferenceValue = exitTile;
            Debug.Log("✓ Exit Tile 配置成功");
        }

        serializedRenderer.ApplyModifiedProperties();
        EditorUtility.SetDirty(renderer);
    }
}
