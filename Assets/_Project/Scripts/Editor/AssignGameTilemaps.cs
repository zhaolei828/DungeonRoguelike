using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// 为Game场景的LevelRenderer分配Tilemap引用
/// </summary>
public class AssignGameTilemaps
{
    [MenuItem("Tools/Dungeon/Assign Game Tilemaps to LevelRenderer")]
    static void AssignTilemaps()
    {
        string gameScenePath = "Assets/_Project/Scenes/Game.unity/Game.unity";
        string currentScenePath = EditorSceneManager.GetActiveScene().path;
        
        // 打开Game场景
        var scene = EditorSceneManager.OpenScene(gameScenePath, OpenSceneMode.Single);
        
        EditorApplication.delayCall += () =>
        {
            // 查找所有必要的组件
            LevelRenderer renderer = Object.FindFirstObjectByType<LevelRenderer>();
            Tilemap[] tilemaps = Object.FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
            
            if (renderer == null)
            {
                Debug.LogError("未找到LevelRenderer！");
                return;
            }
            
            if (tilemaps.Length < 3)
            {
                Debug.LogError($"Tilemap数量不足，找到{tilemaps.Length}个，需要3个");
                return;
            }
            
            // 查找对应的Tilemaps
            Tilemap groundTilemap = null;
            Tilemap wallTilemap = null;
            Tilemap decorationTilemap = null;
            
            foreach (Tilemap tm in tilemaps)
            {
                string name = tm.gameObject.name.ToLower();
                if (name.Contains("ground"))
                    groundTilemap = tm;
                else if (name.Contains("wall"))
                    wallTilemap = tm;
                else if (name.Contains("decoration"))
                    decorationTilemap = tm;
            }
            
            // 如果按名称没找全，按顺序分配
            if (groundTilemap == null) groundTilemap = tilemaps[0];
            if (wallTilemap == null && tilemaps.Length > 1) wallTilemap = tilemaps[1];
            if (decorationTilemap == null && tilemaps.Length > 2) decorationTilemap = tilemaps[2];
            
            // 使用SerializedObject设置
            SerializedObject serializedRenderer = new SerializedObject(renderer);
            
            if (groundTilemap != null)
            {
                serializedRenderer.FindProperty("groundTilemap").objectReferenceValue = groundTilemap;
                Debug.Log($"✓ Ground Tilemap: {groundTilemap.gameObject.name}");
            }
            
            if (wallTilemap != null)
            {
                serializedRenderer.FindProperty("wallTilemap").objectReferenceValue = wallTilemap;
                Debug.Log($"✓ Wall Tilemap: {wallTilemap.gameObject.name}");
            }
            
            if (decorationTilemap != null)
            {
                serializedRenderer.FindProperty("decorationTilemap").objectReferenceValue = decorationTilemap;
                Debug.Log($"✓ Decoration Tilemap: {decorationTilemap.gameObject.name}");
            }
            
            serializedRenderer.ApplyModifiedProperties();
            EditorUtility.SetDirty(renderer);
            
            // 保存
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            
            Debug.Log("<color=green>Game场景Tilemaps已分配！</color>");
            
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
}
