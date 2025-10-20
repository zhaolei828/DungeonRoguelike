using UnityEngine;

/// <summary>
/// 基础AI - 直线追击并攻击
/// 策略: 看到敌人 → 靠近 → 攻击
/// </summary>
public class BasicAI : MonoBehaviour, IAIBehavior
{
    [Header("AI参数")]
    [SerializeField] private float moveTimer = 0f;
    [SerializeField] private float moveCooldown = 0.3f; // 与Hero.moveDuration一致
    
    public void Act(Mob self, Hero target, Level level)
    {
        if (self == null || target == null || level == null)
            return;
        
        // 更新冷却计时
        moveTimer -= Time.deltaTime;
        
        // 计算与目标的距离
        int distanceX = target.pos.x - self.pos.x;
        int distanceY = target.pos.y - self.pos.y;
        
        // 检查是否相邻 (可以攻击)
        if (Mathf.Abs(distanceX) <= 1 && Mathf.Abs(distanceY) <= 1 && (distanceX != 0 || distanceY != 0))
        {
            // 相邻时攻击
            if (moveTimer <= 0f && ShouldAttack(self, target))
            {
                self.TryAttack(target);
                moveTimer = moveCooldown;
            }
        }
        else if (moveTimer <= 0f)
        {
            // 不相邻时靠近目标
            Vector2Int moveDirection = GetMoveDirection(distanceX, distanceY);
            Vector2Int targetPos = self.pos + moveDirection;
            
            if (self.TryMoveTo(targetPos, level))
            {
                moveTimer = moveCooldown;
            }
        }
    }
    
    public bool ShouldAttack(Mob self, Hero target)
    {
        // 基础AI总是攻击
        return true;
    }
    
    public bool ShouldFlee(Mob self, Hero target)
    {
        // 基础AI不逃跑
        return false;
    }
    
    /// <summary>
    /// 获取移动方向 (优先水平/垂直移动)
    /// </summary>
    private Vector2Int GetMoveDirection(int distanceX, int distanceY)
    {
        if (Mathf.Abs(distanceX) > Mathf.Abs(distanceY))
        {
            // 优先水平移动
            return new Vector2Int(distanceX > 0 ? 1 : -1, 0);
        }
        else if (distanceY != 0)
        {
            // 垂直移动
            return new Vector2Int(0, distanceY > 0 ? 1 : -1);
        }
        
        return Vector2Int.zero;
    }
}
