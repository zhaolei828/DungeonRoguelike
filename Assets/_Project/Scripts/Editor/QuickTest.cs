using UnityEngine;
using UnityEditor;

public class QuickTest : EditorWindow
{
    [MenuItem("DungeonRoguelike/Quick Test")]
    public static void ShowWindow()
    {
        Debug.Log("=== Quick Test Started ===");
        
        try
        {
            // 创建测试
            GameObject testGO = new GameObject("TestObj");
            Debug.Log("Created test GameObject");
            
            // 尝试生成地牢
            if (LevelManager.Instance != null)
            {
                Debug.Log("LevelManager exists, attempting generation...");
                LevelManager.Instance.GenerateNewLevel(1);
                Debug.Log("Generation completed!");
                
                if (LevelManager.Instance.CurrentLevel != null)
                {
                    var level = LevelManager.Instance.CurrentLevel;
                    Debug.Log($"SUCCESS! Level size: {level.Width}x{level.Height}");
                }
            }
            else
            {
                Debug.LogError("No LevelManager found!");
            }
            
            // 清理
            DestroyImmediate(testGO);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Test failed: {e.Message}\n{e.StackTrace}");
        }
        
        Debug.Log("=== Quick Test Finished ===");
    }
}

