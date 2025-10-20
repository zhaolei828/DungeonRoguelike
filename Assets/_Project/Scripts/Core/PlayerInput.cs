using UnityEngine;

/// <summary>
/// 玩家输入处理 - 处理键盘输入并控制Hero移动（回合制系统）
/// </summary>
public class PlayerInput : MonoBehaviour
{
    private Hero hero;
    private Level currentLevel;
    
    [Header("运动设置")]
    [SerializeField] private float moveCooldown = 0.2f; // 每次移动之间的最小延迟
    private float moveTimer = 0f;
    
    [Header("动画设置")]
    [SerializeField] private bool useMovementAnimation = true; // 是否使用运动动画
    
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
            
            // 尝试移动
            bool moved = hero.TryMoveTo(targetPos, currentLevel);
            
            if (moved)
            {
                // 移动成功，重置冷却计时
                moveTimer = moveCooldown;
                
                // 更新Hero的Transform位置
                UpdateHeroTransform();
                
                // 触发移动动画
                if (useMovementAnimation)
                {
                    HeroAnimator animator = hero.GetComponent<HeroAnimator>();
                    if (animator != null)
                    {
                        animator.SetAnimationByDirection(moveDirection);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 更新Hero的Transform位置以匹配其逻辑位置
    /// </summary>
    private void UpdateHeroTransform()
    {
        if (hero != null)
        {
            // 将逻辑坐标转换为世界坐标（Tilemap中心对齐）
            hero.transform.position = new Vector3(hero.pos.x + 0.5f, hero.pos.y + 0.5f, 0);
        }
    }
}

