using UnityEngine;

/// <summary>
/// 关卡生成器
/// 负责统一管理各种类型关卡的生成
/// </summary>
public class LevelGenerator : MonoBehaviour
{
    [Header("生成设置")]
    [SerializeField] private bool useDebugSeed = false;
    [SerializeField] private int debugSeed = 12345;
    
    // 生成统计
    private int _generatedLevels = 0;
    private float _totalGenerationTime = 0f;
    
    #region Properties
    
    /// <summary>
    /// 已生成的关卡数量
    /// </summary>
    public int GeneratedLevels => _generatedLevels;
    
    /// <summary>
    /// 平均生成时间
    /// </summary>
    public float AverageGenerationTime => _generatedLevels > 0 ? _totalGenerationTime / _generatedLevels : 0f;
    
    #endregion
    
    #region Level Generation
    
    /// <summary>
    /// 生成关卡
    /// </summary>
    /// <param name="level">要生成的关卡</param>
    /// <returns>是否生成成功</returns>
    public bool GenerateLevel(Level level)
    {
        if (level == null)
        {
            Debug.LogError("Cannot generate null level");
            return false;
        }
        
        if (!level.IsInitialized)
        {
            Debug.LogError($"Level {level.name} is not initialized");
            return false;
        }
        
        Debug.Log($"Starting generation for {level.name} (Type: {level.LevelType}, Depth: {level.Depth})");
        
        float startTime = Time.realtimeSinceStartup;
        
        // 设置随机种子
        SetGenerationSeed(level.Depth, level.LevelType);
        
        // 生成关卡
        bool success = level.Generate();
        
        // 记录生成时间
        float generationTime = Time.realtimeSinceStartup - startTime;
        _totalGenerationTime += generationTime;
        _generatedLevels++;
        
        if (success)
        {
            Debug.Log($"Level generation completed in {generationTime:F3}s");
            
            // 后处理
            PostProcessLevel(level);
        }
        else
        {
            Debug.LogError($"Level generation failed for {level.name}");
        }
        
        return success;
    }
    
    /// <summary>
    /// 设置生成种子
    /// </summary>
    /// <param name="depth">深度</param>
    /// <param name="levelType">关卡类型</param>
    private void SetGenerationSeed(int depth, LevelType levelType)
    {
        int seed;
        
        if (useDebugSeed)
        {
            seed = debugSeed;
            Debug.Log($"Using debug seed: {seed}");
        }
        else
        {
            // 基于深度和类型生成种子
            seed = depth * 1000 + (int)levelType * 100 + System.DateTime.Now.Millisecond;
        }
        
        Random.InitState(seed);
        Debug.Log($"Generation seed set to: {seed}");
    }
    
    /// <summary>
    /// 后处理关卡
    /// </summary>
    /// <param name="level">关卡</param>
    private void PostProcessLevel(Level level)
    {
        // 验证关卡
        ValidateLevel(level);
        
        // 优化关卡
        OptimizeLevel(level);
        
        Debug.Log($"Post-processing completed for {level.name}");
    }
    
    /// <summary>
    /// 验证关卡
    /// </summary>
    /// <param name="level">关卡</param>
    private void ValidateLevel(Level level)
    {
        // 检查入口和出口
        if (level.EntrancePos == Vector2Int.zero && level.ExitPos == Vector2Int.zero)
        {
            Debug.LogWarning($"Level {level.name} has no entrance or exit defined");
        }
        
        // 检查可通行区域
        int passableCount = 0;
        for (int x = 0; x < level.Width; x++)
        {
            for (int y = 0; y < level.Height; y++)
            {
                if (level.IsPassable(x, y))
                {
                    passableCount++;
                }
            }
        }
        
        float passableRatio = (float)passableCount / (level.Width * level.Height);
        
        if (passableRatio < 0.1f)
        {
            Debug.LogWarning($"Level {level.name} has very low passable area ratio: {passableRatio:P}");
        }
        else if (passableRatio > 0.8f)
        {
            Debug.LogWarning($"Level {level.name} has very high passable area ratio: {passableRatio:P}");
        }
        
        Debug.Log($"Level validation: {passableCount} passable tiles ({passableRatio:P})");
    }
    
    /// <summary>
    /// 优化关卡
    /// </summary>
    /// <param name="level">关卡</param>
    private void OptimizeLevel(Level level)
    {
        // 这里可以添加各种优化逻辑
        // 比如：
        // - 移除孤立的区域
        // - 优化路径连通性
        // - 平衡难度分布
        // - 优化性能相关的结构
        
        Debug.Log($"Level optimization completed for {level.name}");
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// 创建指定类型的关卡
    /// </summary>
    /// <param name="levelType">关卡类型</param>
    /// <param name="depth">深度</param>
    /// <param name="parent">父对象</param>
    /// <returns>创建的关卡</returns>
    public Level CreateLevel(LevelType levelType, int depth, Transform parent = null)
    {
        GameObject levelObject = new GameObject($"Level_{depth}_{levelType}");
        
        if (parent != null)
        {
            levelObject.transform.SetParent(parent);
        }
        
        Level level = null;
        
        // 根据类型添加对应的组件
        switch (levelType)
        {
            case LevelType.Sewers:
                level = levelObject.AddComponent<SewerLevel>();
                break;
            case LevelType.Prison:
                level = levelObject.AddComponent<PrisonLevel>();
                break;
            case LevelType.Caves:
                level = levelObject.AddComponent<CavesLevel>();
                break;
            case LevelType.City:
                level = levelObject.AddComponent<CityLevel>();
                break;
            case LevelType.Halls:
                level = levelObject.AddComponent<HallsLevel>();
                break;
            default:
                level = levelObject.AddComponent<RegularLevel>();
                break;
        }
        
        // 初始化关卡
        level.Initialize(depth, levelType);
        
        Debug.Log($"Created level: {levelType} at depth {depth}");
        
        return level;
    }
    
    /// <summary>
    /// 获取生成统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    public string GetGenerationStats()
    {
        return $"Level Generation Stats:\n" +
               $"Generated Levels: {_generatedLevels}\n" +
               $"Total Generation Time: {_totalGenerationTime:F2}s\n" +
               $"Average Generation Time: {AverageGenerationTime:F3}s";
    }
    
    /// <summary>
    /// 重置统计信息
    /// </summary>
    public void ResetStats()
    {
        _generatedLevels = 0;
        _totalGenerationTime = 0f;
        
        Debug.Log("Generation stats reset");
    }
    
    #endregion
    
    #region Debug
    
    /// <summary>
    /// 测试生成所有类型的关卡
    /// </summary>
    [ContextMenu("Test Generate All Level Types")]
    public void TestGenerateAllLevelTypes()
    {
        LevelType[] allTypes = {
            LevelType.Sewers,
            LevelType.Prison,
            LevelType.Caves,
            LevelType.City,
            LevelType.Halls
        };
        
        foreach (LevelType type in allTypes)
        {
            Debug.Log($"Testing generation for {type}...");
            
            Level testLevel = CreateLevel(type, 1, transform);
            bool success = GenerateLevel(testLevel);
            
            Debug.Log($"{type} generation: {(success ? "SUCCESS" : "FAILED")}");
            
            if (testLevel != null)
            {
                DestroyImmediate(testLevel.gameObject);
            }
        }
        
        Debug.Log("All level type tests completed");
        Debug.Log(GetGenerationStats());
    }
    
    #endregion
}
