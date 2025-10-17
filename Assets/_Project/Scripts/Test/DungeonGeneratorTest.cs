using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 地牢生成测试脚本
/// 用于测试和验证地牢生成系统
/// </summary>
public class DungeonGeneratorTest : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private int testDepth = 1;
    [SerializeField] private bool autoGenerate = false;
    [SerializeField] private float autoGenerateInterval = 2f;
    
    [Header("UI引用（可选）")]
    [SerializeField] private Button generateButton;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI statsText;
    
    private float lastGenerateTime;
    private int generateCount = 0;
    private float totalGenerateTime = 0f;
    
    private void Start()
    {
        if (generateButton != null)
        {
            generateButton.onClick.AddListener(OnGenerateButtonClicked);
        }
        
        UpdateInfoText();
        
        // 自动生成一次地牢（用于测试）
        Invoke(nameof(GenerateLevel), 0.5f);
    }
    
    private void Update()
    {
        // 自动生成模式
        if (autoGenerate && Time.time - lastGenerateTime >= autoGenerateInterval)
        {
            GenerateLevel();
            lastGenerateTime = Time.time;
        }
        
        // 快捷键
        if (Input.GetKeyDown(KeyCode.G))
        {
            GenerateLevel();
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearLevel();
        }
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            testDepth = Mathf.Clamp(testDepth + 1, 1, 25);
            UpdateInfoText();
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            testDepth = Mathf.Clamp(testDepth - 1, 1, 25);
            UpdateInfoText();
        }
    }
    
    /// <summary>
    /// 生成关卡
    /// </summary>
    public void GenerateLevel()
    {
        if (LevelManager.Instance == null)
        {
            Debug.LogError("LevelManager not found!");
            return;
        }
        
        float startTime = Time.realtimeSinceStartup;
        
        Debug.Log($"=== Generating Level {testDepth} ===");
        
        try
        {
            LevelManager.Instance.GenerateNewLevel(testDepth);
            
            float generateTime = (Time.realtimeSinceStartup - startTime) * 1000f;
            generateCount++;
            totalGenerateTime += generateTime;
            
            Debug.Log($"Level generated in {generateTime:F2}ms");
            
            UpdateStatsText(generateTime);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error generating level: {e.Message}\n{e.StackTrace}");
        }
    }
    
    /// <summary>
    /// 清理关卡
    /// </summary>
    public void ClearLevel()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.ClearCurrentLevel();
            Debug.Log("Level cleared");
        }
        
        if (LevelRenderer.Instance != null)
        {
            LevelRenderer.Instance.ClearAllTilemaps();
        }
        
        UpdateStatsText(0);
    }
    
    /// <summary>
    /// 按钮点击事件
    /// </summary>
    private void OnGenerateButtonClicked()
    {
        GenerateLevel();
    }
    
    /// <summary>
    /// 更新信息文本
    /// </summary>
    private void UpdateInfoText()
    {
        if (infoText != null)
        {
            infoText.text = $"Depth: {testDepth}\nPress G to generate\nPress C to clear\nUp/Down to change depth";
        }
    }
    
    /// <summary>
    /// 更新统计文本
    /// </summary>
    private void UpdateStatsText(float lastGenerateTime)
    {
        if (statsText != null)
        {
            float avgTime = generateCount > 0 ? totalGenerateTime / generateCount : 0;
            
            statsText.text = $"Generated: {generateCount} times\n" +
                           $"Last: {lastGenerateTime:F2}ms\n" +
                           $"Average: {avgTime:F2}ms\n" +
                           $"Current Depth: {testDepth}";
        }
    }
    
    /// <summary>
    /// 运行性能测试
    /// </summary>
    [ContextMenu("Run Performance Test")]
    public void RunPerformanceTest()
    {
        Debug.Log("=== Running Performance Test ===");
        
        int testCount = 10;
        float totalTime = 0f;
        int successCount = 0;
        
        for (int i = 0; i < testCount; i++)
        {
            float startTime = Time.realtimeSinceStartup;
            
            try
            {
                LevelManager.Instance.GenerateNewLevel(Random.Range(1, 26));
                float time = (Time.realtimeSinceStartup - startTime) * 1000f;
                totalTime += time;
                successCount++;
                
                Debug.Log($"Test {i + 1}/{testCount}: {time:F2}ms");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Test {i + 1} failed: {e.Message}");
            }
        }
        
        float avgTime = successCount > 0 ? totalTime / successCount : 0;
        Debug.Log($"=== Performance Test Complete ===");
        Debug.Log($"Success Rate: {successCount}/{testCount}");
        Debug.Log($"Average Time: {avgTime:F2}ms");
        Debug.Log($"Total Time: {totalTime:F2}ms");
    }
    
    /// <summary>
    /// 测试所有深度
    /// </summary>
    [ContextMenu("Test All Depths")]
    public void TestAllDepths()
    {
        Debug.Log("=== Testing All Depths ===");
        
        for (int depth = 1; depth <= 25; depth++)
        {
            try
            {
                float startTime = Time.realtimeSinceStartup;
                LevelManager.Instance.GenerateNewLevel(depth);
                float time = (Time.realtimeSinceStartup - startTime) * 1000f;
                
                Debug.Log($"Depth {depth}: OK ({time:F2}ms)");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Depth {depth}: FAILED - {e.Message}");
            }
        }
        
        Debug.Log("=== All Depths Test Complete ===");
    }
}

