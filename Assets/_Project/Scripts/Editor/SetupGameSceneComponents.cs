using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 在Game.unity场景中设置LevelRenderer组件和配置Tiles
/// </summary>
public class SetupGameSceneComponents
{
    [MenuItem("Tools/Dungeon/Setup Game Scene LevelRenderer")]
    static void SetupGameScene()
    {
        string gameScenePath = "Assets/_Project/Scenes/Game.unity/Game.unity";
        string currentScenePath = EditorSceneManager.GetActiveScene().path;
        
        // 打开Game场景
        var scene = EditorSceneManager.OpenScene(gameScenePath, OpenSceneMode.Single);
        
        // 等待一帧让场景加载
        EditorApplication.delayCall += () =>
        {
            SetupLevelRenderer();
            
            // 保存Game场景
            EditorSceneManager.SaveScene(scene);
            
            // 返回原场景
            if (!string.IsNullOrEmpty(currentScenePath))
            {
                EditorApplication.delayCall += () =>
                {
                    EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
                };
            }
        };
    }

    static void SetupLevelRenderer()
    {
        // 查找或创建LevelRenderer
        LevelRenderer renderer = Object.FindFirstObjectByType<LevelRenderer>();
        if (renderer == null)
        {
            // 查找LevelRenderer GameObject
            Transform[] allTransforms = Object.FindObjectsByType<Transform>(FindObjectsSortMode.None);
            Transform levelRendererTransform = null;
            
            foreach (Transform t in allTransforms)
            {
                if (t.gameObject.name == "LevelRenderer")
                {
                    levelRendererTransform = t;
                    break;
                }
            }
            
            if (levelRendererTransform != null)
            {
                renderer = levelRendererTransform.gameObject.AddComponent<LevelRenderer>();
                Debug.Log("✓ LevelRenderer组件已添加");
            }
            else
            {
                Debug.LogError("未找到LevelRenderer GameObject！");
                return;
            }
        }

        // 配置Tiles
        string basePath = "Assets/_Project/Art/Tiles/TileAssets/SewersTilePalette/";
        
        Tile floorTile = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "tiles_sewers_0_0.asset");
        Tile wallTile = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "tiles_sewers_4_0.asset");
        Tile entranceTile = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "tiles_sewers_1_6.asset");
        Tile exitTile = AssetDatabase.LoadAssetAtPath<Tile>(basePath + "tiles_sewers_1_7.asset");

        SerializedObject serializedRenderer = new SerializedObject(renderer);
        
        int configuredCount = 0;
        
        if (floorTile != null)
        {
            serializedRenderer.FindProperty("floorTile").objectReferenceValue = floorTile;
            configuredCount++;
            Debug.Log("✓ Floor Tile 配置成功");
        }

        if (wallTile != null)
        {
            serializedRenderer.FindProperty("wallTile").objectReferenceValue = wallTile;
            configuredCount++;
            Debug.Log("✓ Wall Tile 配置成功");
        }

        if (entranceTile != null)
        {
            serializedRenderer.FindProperty("entranceTile").objectReferenceValue = entranceTile;
            configuredCount++;
            Debug.Log("✓ Entrance Tile 配置成功");
        }

        if (exitTile != null)
        {
            serializedRenderer.FindProperty("exitTile").objectReferenceValue = exitTile;
            configuredCount++;
            Debug.Log("✓ Exit Tile 配置成功");
        }

        serializedRenderer.ApplyModifiedProperties();
        EditorUtility.SetDirty(renderer);
        
        Debug.Log($"<color=green>Game.unity场景配置完成！成功配置 {configuredCount}/4 个Tile。</color>");
    }
}
