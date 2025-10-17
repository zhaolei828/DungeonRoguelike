using UnityEngine;

/// <summary>
/// 关卡管理器
/// 负责关卡生成、切换、清理
/// </summary>
public class LevelManager : Singleton<LevelManager>
{
    [Header("关卡设置")]
    [SerializeField] private Transform levelParent;
    
    // 当前关卡引用
    private Level _currentLevel;
    private LevelGenerator _levelGenerator;
    
    // 事件
    public System.Action<Level> OnLevelGenerated;
    public System.Action OnLevelCleared;
    
    #region Properties
    
    /// <summary>
    /// 当前关卡
    /// </summary>
    public Level CurrentLevel
    {
        get => _currentLevel;
        private set
        {
            _currentLevel = value;
            
            // 同步到GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CurrentLevel = _currentLevel;
            }
        }
    }
    
    /// <summary>
    /// 关卡生成器
    /// </summary>
    public LevelGenerator LevelGenerator
    {
        get
        {
            if (_levelGenerator == null)
            {
                _levelGenerator = GetComponent<LevelGenerator>();
                if (_levelGenerator == null)
                {
                    _levelGenerator = gameObject.AddComponent<LevelGenerator>();
                }
            }
            return _levelGenerator;
        }
    }
    
    #endregion
    
    #region Unity Lifecycle
    
    protected override void Awake()
    {
        base.Awake();
        
        // 如果没有指定父对象，创建一个
        if (levelParent == null)
        {
            GameObject levelContainer = new GameObject("LevelContainer");
            levelParent = levelContainer.transform;
            levelParent.SetParent(transform);
        }
    }
    
    private void Start()
    {
        // 如果在游戏场景且没有关卡，生成第一层
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Game" && CurrentLevel == null)
        {
            GenerateNewLevel(1);
        }
    }
    
    #endregion
    
    #region Level Management
    
    /// <summary>
    /// 生成新关卡
    /// </summary>
    /// <param name="depth">地牢深度</param>
    public void GenerateNewLevel(int depth)
    {
        Debug.Log($"Generating level for depth {depth}");
        
        // 清理旧关卡
        ClearCurrentLevel();
        
        // 生成新关卡
        Level newLevel = CreateLevelForDepth(depth);
        
        if (newLevel != null)
        {
            // 执行关卡生成
            bool generated = newLevel.Generate();
            
            if (generated)
            {
                // 渲染关卡到Tilemap
                if (LevelRenderer.Instance != null)
                {
                    LevelRenderer.Instance.RenderLevel(newLevel);
                }
                else
                {
                    Debug.LogWarning("LevelRenderer not found, level will not be rendered");
                }
                
                CurrentLevel = newLevel;
                OnLevelGenerated?.Invoke(CurrentLevel);
                
                Debug.Log($"Level {depth} generated and rendered successfully");
            }
            else
            {
                Debug.LogError($"Failed to generate level {depth}");
                Destroy(newLevel.gameObject);
            }
        }
        else
        {
            Debug.LogError($"Failed to create level object for depth {depth}");
        }
    }
    
    /// <summary>
    /// 根据深度创建关卡
    /// </summary>
    /// <param name="depth">深度</param>
    /// <returns>生成的关卡</returns>
    private Level CreateLevelForDepth(int depth)
    {
        // 根据深度决定关卡类型
        LevelType levelType = GetLevelTypeForDepth(depth);
        
        // 创建关卡GameObject
        GameObject levelObject = new GameObject($"Level_{depth}_{levelType}");
        levelObject.transform.SetParent(levelParent);
        
        // 添加对应的Level组件
        Level level = null;
        
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
                // 默认使用RegularLevel
                level = levelObject.AddComponent<RegularLevel>();
                break;
        }
        
        // 初始化关卡
        if (level != null)
        {
            level.Initialize(depth, levelType);
            
            // 使用LevelGenerator生成关卡内容
            bool generated = LevelGenerator.GenerateLevel(level);
            
            if (!generated)
            {
                Debug.LogError("Level generation failed!");
                Destroy(levelObject);
                return null;
            }
        }
        
        return level;
    }
    
    /// <summary>
    /// 根据深度获取关卡类型
    /// </summary>
    /// <param name="depth">深度</param>
    /// <returns>关卡类型</returns>
    private LevelType GetLevelTypeForDepth(int depth)
    {
        // Shattered Pixel Dungeon的区域划分：
        // 1-5: Sewers
        // 6-10: Prison
        // 11-15: Caves
        // 16-20: City
        // 21-25: Halls
        
        if (depth <= 5) return LevelType.Sewers;
        if (depth <= 10) return LevelType.Prison;
        if (depth <= 15) return LevelType.Caves;
        if (depth <= 20) return LevelType.City;
        if (depth <= 25) return LevelType.Halls;
        
        // 超过25层，循环使用Halls
        return LevelType.Halls;
    }
    
    /// <summary>
    /// 清理当前关卡
    /// </summary>
    public void ClearCurrentLevel()
    {
        if (CurrentLevel != null)
        {
            Debug.Log("Clearing current level");
            
            // 清理关卡
            CurrentLevel.Cleanup();
            
            // 销毁关卡GameObject
            if (CurrentLevel.gameObject != null)
            {
                Destroy(CurrentLevel.gameObject);
            }
            
            CurrentLevel = null;
            OnLevelCleared?.Invoke();
        }
    }
    
    /// <summary>
    /// 重新生成当前关卡
    /// </summary>
    public void RegenerateCurrentLevel()
    {
        if (GameManager.Instance != null)
        {
            int currentDepth = GameManager.Instance.CurrentDepth;
            GenerateNewLevel(currentDepth);
        }
    }
    
    #endregion
    
    #region Utilities
    
    /// <summary>
    /// 获取关卡边界
    /// </summary>
    /// <returns>关卡边界</returns>
    public Bounds GetLevelBounds()
    {
        if (CurrentLevel != null)
        {
            return CurrentLevel.GetBounds();
        }
        
        // 默认边界
        return new Bounds(Vector3.zero, Vector3.one * 32);
    }
    
    #endregion
}

/// <summary>
/// 关卡类型枚举
/// </summary>
public enum LevelType
{
    Sewers,     // 下水道 (1-5层)
    Prison,     // 监狱 (6-10层)
    Caves,      // 洞穴 (11-15层)
    City,       // 城市 (16-20层)
    Halls,      // 大厅 (21-25层)
    Special     // 特殊关卡
}
