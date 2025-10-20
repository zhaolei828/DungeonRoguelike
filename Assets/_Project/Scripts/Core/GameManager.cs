// GameManager.cs - 游戏核心管理器，负责游戏状态和数据管理
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 游戏状态枚举
/// </summary>
public enum GameState
{
    MainMenu,    // 主菜单
    Playing,     // 游戏中
    Paused,      // 暂停
    GameOver,    // 游戏结束
    Victory      // 胜利
}

/// <summary>
/// 游戏总管理器
/// 负责游戏状态管理、场景切换、核心数据维护
/// </summary>
public class GameManager : Singleton<GameManager>
{
    [Header("游戏状态")]
    [SerializeField] private GameState _currentState = GameState.MainMenu;
    
    [Header("游戏数据")]
    [SerializeField] private int _currentDepth = 1;
    [SerializeField] private int _score = 0;
    [SerializeField] private int _gold = 0;
    
    // 核心引用
    private Level _currentLevel;
    
    // 事件
    public System.Action<GameState> OnGameStateChanged;
    public System.Action<int> OnDepthChanged;
    public System.Action<int> OnScoreChanged;
    public System.Action<int> OnGoldChanged;
    
    #region Properties
    
    /// <summary>
    /// 当前游戏状态
    /// </summary>
    public GameState CurrentState
    {
        get => _currentState;
        private set
        {
            if (_currentState != value)
            {
                _currentState = value;
                OnGameStateChanged?.Invoke(_currentState);
                Debug.Log($"Game State Changed: {_currentState}");
            }
        }
    }
    
    /// <summary>
    /// 当前地牢深度
    /// </summary>
    public int CurrentDepth
    {
        get => _currentDepth;
        private set
        {
            if (_currentDepth != value)
            {
                _currentDepth = value;
                OnDepthChanged?.Invoke(_currentDepth);
            }
        }
    }
    
    /// <summary>
    /// 当前分数
    /// </summary>
    public int Score
    {
        get => _score;
        private set
        {
            if (_score != value)
            {
                _score = value;
                OnScoreChanged?.Invoke(_score);
            }
        }
    }
    
    /// <summary>
    /// 当前金币
    /// </summary>
    public int Gold
    {
        get => _gold;
        private set
        {
            if (_gold != value)
            {
                _gold = value;
                OnGoldChanged?.Invoke(_gold);
            }
        }
    }
    
    /// <summary>
    /// 当前英雄
    /// </summary>
    public Hero Hero { get; set; }
    
    /// <summary>
    /// 当前关卡
    /// </summary>
    public Level CurrentLevel
    {
        get => _currentLevel;
        set => _currentLevel = value;
    }
    
    #endregion
    
    #region Unity Lifecycle
    
    protected override void Awake()
    {
        base.Awake();
        
        // 初始化游戏
        InitializeGame();
    }
    
    private void Start()
    {
        // 如果在游戏场景启动，切换到Playing状态
        if (SceneManager.GetActiveScene().name == "Game")
        {
            CurrentState = GameState.Playing;
        }
    }
    
    #endregion
    
    #region Game Flow
    
    /// <summary>
    /// 初始化游戏
    /// </summary>
    private void InitializeGame()
    {
        // 设置帧率
        Application.targetFrameRate = 60;
        
        // 初始化数据
        ResetGameData();
        
        Debug.Log("GameManager initialized");
    }
    
    /// <summary>
    /// 开始新游戏
    /// </summary>
    /// <param name="heroClass">英雄职业</param>
    public void StartNewGame(HeroClass heroClass = HeroClass.Warrior)
    {
        Debug.Log($"Starting new game with {heroClass}");
        
        // 重置游戏数据
        ResetGameData();
        
        // 切换到游戏场景
        CurrentState = GameState.Playing;
        
        // 如果不在游戏场景，加载游戏场景
        if (SceneManager.GetActiveScene().name != "Game")
        {
            SceneManager.LoadScene("Game");
        }
        else
        {
            // 已经在游戏场景，直接开始
            OnGameSceneLoaded();
        }
    }
    
    /// <summary>
    /// 游戏场景加载完成
    /// </summary>
    private void OnGameSceneLoaded()
    {
        // 这里会在场景加载后由LevelManager调用
        Debug.Log("Game scene loaded, ready to play");
    }
    
    /// <summary>
    /// 下一层
    /// </summary>
    public void NextLevel()
    {
        CurrentDepth++;
        Debug.Log($"Descending to depth {CurrentDepth}");
        
        // 通知LevelManager生成新关卡
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.GenerateNewLevel(CurrentDepth);
        }
    }
    
    /// <summary>
    /// 游戏结束
    /// </summary>
    public void GameOver()
    {
        Debug.Log("Game Over!");
        CurrentState = GameState.GameOver;
        
        // 这里可以添加游戏结束逻辑
        // 比如显示结算界面、保存分数等
    }
    
    /// <summary>
    /// 游戏胜利
    /// </summary>
    public void Victory()
    {
        Debug.Log("Victory!");
        CurrentState = GameState.Victory;
        
        // 这里可以添加胜利逻辑
    }
    
    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentState = GameState.Paused;
            Time.timeScale = 0f;
        }
    }
    
    /// <summary>
    /// 恢复游戏
    /// </summary>
    public void ResumeGame()
    {
        if (CurrentState == GameState.Paused)
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
        }
    }
    
    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void ReturnToMainMenu()
    {
        CurrentState = GameState.MainMenu;
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    
    #endregion
    
    #region Data Management
    
    /// <summary>
    /// 重置游戏数据
    /// </summary>
    private void ResetGameData()
    {
        CurrentDepth = 1;
        Score = 0;
        Gold = 0;
        Hero = null;
        _currentLevel = null;
    }
    
    /// <summary>
    /// 添加分数
    /// </summary>
    /// <param name="points">分数</param>
    public void AddScore(int points)
    {
        Score += points;
    }
    
    /// <summary>
    /// 添加金币
    /// </summary>
    /// <param name="amount">金币数量</param>
    public void AddGold(int amount)
    {
        Gold += amount;
    }
    
    /// <summary>
    /// 消费金币
    /// </summary>
    /// <param name="amount">金币数量</param>
    /// <returns>是否成功消费</returns>
    public bool SpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            return true;
        }
        return false;
    }
    
    #endregion
}

/// <summary>
/// 英雄职业枚举
/// </summary>
public enum HeroClass
{
    Warrior,    // 战士
    Mage,       // 法师
    Rogue,      // 盗贼
    Huntress,   // 猎人
    Cleric      // 牧师
}
