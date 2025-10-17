using UnityEngine;
using UnityEditor;

/// <summary>
/// 自动化测试地牢生成
/// </summary>
public class AutoTestDungeon
{
    [MenuItem("DungeonRoguelike/Auto Test Dungeon Generation")]
    public static void TestDungeonGeneration()
    {
        Debug.Log("=== 开始自动化测试地牢生成 ===");
        
        // 确保有LevelManager
        LevelManager levelManager = LevelManager.Instance;
        if (levelManager == null)
        {
            GameObject go = new GameObject("LevelManager");
            levelManager = go.AddComponent<LevelManager>();
            Debug.Log("✓ 创建LevelManager");
        }
        
        // 确保有LevelRenderer
        LevelRenderer renderer = LevelRenderer.Instance;
        if (renderer == null)
        {
            GameObject go = new GameObject("LevelRenderer");
            renderer = go.AddComponent<LevelRenderer>();
            Debug.Log("✓ 创建LevelRenderer");
        }
        
        // 测试生成不同深度的地牢
        int[] testDepths = { 1, 5, 10, 15, 20, 25 };
        int successCount = 0;
        int failCount = 0;
        
        foreach (int depth in testDepths)
        {
            try
            {
                Debug.Log($"\n--- 测试深度 {depth} ---");
                
                // 生成地牢
                System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                levelManager.GenerateNewLevel(depth);
                sw.Stop();
                
                Level currentLevel = levelManager.CurrentLevel;
                if (currentLevel != null)
                {
                    Debug.Log($"✓ 深度 {depth} 生成成功！");
                    Debug.Log($"  - 地图大小: {currentLevel.Width}x{currentLevel.Height}");
                    Debug.Log($"  - 生成耗时: {sw.ElapsedMilliseconds}ms");
                    successCount++;
                }
                else
                {
                    Debug.LogError($"✗ 深度 {depth} 生成失败：CurrentLevel为null");
                    failCount++;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"✗ 深度 {depth} 生成失败：{e.Message}");
                Debug.LogError($"  堆栈: {e.StackTrace}");
                failCount++;
            }
        }
        
        // 输出测试结果
        Debug.Log("\n=== 测试结果 ===");
        Debug.Log($"成功: {successCount}/{testDepths.Length}");
        Debug.Log($"失败: {failCount}/{testDepths.Length}");
        
        if (failCount == 0)
        {
            Debug.Log("🎉 所有测试通过！地牢生成系统完全正常！");
        }
        else
        {
            Debug.LogWarning($"⚠️ 有 {failCount} 个测试失败，请检查错误信息");
        }
    }
}

