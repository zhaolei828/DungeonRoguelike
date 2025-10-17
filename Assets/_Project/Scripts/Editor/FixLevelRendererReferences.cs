using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;

/// <summary>
/// 修复LevelRenderer的Tilemap引用
/// </summary>
public class FixLevelRendererReferences : EditorWindow
{
    [MenuItem("DungeonRoguelike/Fix LevelRenderer References")]
    public static void FixReferences()
    {
        // 查找LevelRenderer
        LevelRenderer renderer = FindFirstObjectByType<LevelRenderer>();
        if (renderer == null)
        {
            Debug.LogError("❌ 未找到LevelRenderer!");
            return;
        }
        
        // 查找所有Tilemap
        Tilemap[] tilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        
        Tilemap groundTilemap = null;
        Tilemap wallTilemap = null;
        Tilemap decorationTilemap = null;
        
        foreach (Tilemap tilemap in tilemaps)
        {
            string name = tilemap.gameObject.name.ToLower();
            
            if (name.Contains("ground"))
            {
                groundTilemap = tilemap;
                Debug.Log($"✓ 找到GroundTilemap: {tilemap.gameObject.name}");
            }
            else if (name.Contains("wall"))
            {
                wallTilemap = tilemap;
                Debug.Log($"✓ 找到WallTilemap: {tilemap.gameObject.name}");
            }
            else if (name.Contains("decoration"))
            {
                decorationTilemap = tilemap;
                Debug.Log($"✓ 找到DecorationTilemap: {tilemap.gameObject.name}");
            }
        }
        
        if (groundTilemap == null || wallTilemap == null || decorationTilemap == null)
        {
            Debug.LogError("❌ 未找到所有Tilemap！");
            return;
        }
        
        // 使用SerializedObject来修改并保存
        SerializedObject so = new SerializedObject(renderer);
        
        so.FindProperty("groundTilemap").objectReferenceValue = groundTilemap;
        so.FindProperty("wallTilemap").objectReferenceValue = wallTilemap;
        so.FindProperty("decorationTilemap").objectReferenceValue = decorationTilemap;
        
        so.ApplyModifiedProperties();
        
        // 标记场景为脏
        EditorUtility.SetDirty(renderer);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(renderer.gameObject.scene);
        
        Debug.Log("✅ LevelRenderer引用已修复并保存！");
        
        EditorUtility.DisplayDialog("修复完成", 
            "LevelRenderer的Tilemap引用已修复！\n\n现在可以Play测试了。", 
            "确定");
    }
}

