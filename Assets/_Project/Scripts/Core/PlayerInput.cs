using UnityEngine;

/// <summary>
/// 玩家输入处理 - 处理键盘输入并控制Hero移动（回合制系统）
/// Hero内部处理平滑运动和动画播放
/// </summary>
public class PlayerInput : MonoBehaviour
{
    private Hero hero;
    private Level currentLevel;
    
    [Header("运动设置")]
    [SerializeField] private float moveCooldown = 0.2f; // 每次移动之间的最小延迟
    private float moveTimer = 0f;
    
    private void Start()
    {
        // 获取Hero引用
        hero = GameManager.Instance?.Hero;
        if (hero == null)
        {
            Debug.LogError("PlayerInput: Hero not found in GameManager!");
        }
        
        // 获取当前关卡
        currentLevel = LevelManager.Instance?.CurrentLevel;
        if (currentLevel == null)
        {
            Debug.LogError("PlayerInput: Current level not found!");
        }
        
        moveTimer = moveCooldown; // 初始允许立即移动
    }
    
    private void Update()
    {
        if (hero == null || currentLevel == null)
            return;
        
        // 更新移动冷却计时
        moveTimer -= Time.deltaTime;
        
        // 检测方向键输入
        Vector2Int moveDirection = Vector2Int.zero;
        
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveDirection = new Vector2Int(0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveDirection = new Vector2Int(0, -1);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveDirection = new Vector2Int(-1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveDirection = new Vector2Int(1, 0);
        }
        
        // 如果有移动输入且冷却时间已过，执行移动
        if (moveDirection != Vector2Int.zero && moveTimer <= 0f)
        {
            Vector2Int targetPos = hero.pos + moveDirection;
            
            // 尝试移动（Hero内部会处理平滑动画）
            bool moved = hero.TryMoveTo(targetPos, currentLevel);
            
            if (moved)
            {
                // 移动成功，重置冷却计时
                moveTimer = moveCooldown;
            }
        }
        
        // 检测E键攻击
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryAttackNearbyEnemy();
        }
    }
    
    /// <summary>
    /// 尝试攻击附近的敌人
    /// </summary>
    private void TryAttackNearbyEnemy()
    {
        if (hero == null)
            return;
        
        // 搜索附近的Mob
        Mob[] allMobs = FindObjectsByType<Mob>(FindObjectsSortMode.None);
        Mob nearestMob = null;
        int nearestDistance = int.MaxValue;
        
        foreach (Mob mob in allMobs)
        {
            int distance = CombatCalculator.CalculateDistance(hero, mob);
            if (distance <= 1 && distance < nearestDistance)  // 相邻位置
            {
                nearestMob = mob;
                nearestDistance = distance;
            }
        }
        
        if (nearestMob != null)
        {
            // 启动战斗
            TurnManager.Instance.StartBattle(hero, nearestMob);
        }
        else
        {
            Debug.Log("附近没有敌人！");
        }
    }
}

