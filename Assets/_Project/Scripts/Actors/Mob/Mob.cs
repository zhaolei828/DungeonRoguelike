using UnityEngine;
using System.Collections;

/// <summary>
/// Mob (怪物) 基类 - 所有敌对角色的基类
/// </summary>
public class Mob : Actor
{
    [Header("怪物属性")]
    [SerializeField] private string mobName = "Mob";
    [SerializeField] private int level = 1;
    [SerializeField] private int experienceReward = 10;
    [SerializeField] private int attackPower = 5;
    [SerializeField] private int defense = 1;
    [SerializeField] private int agility = 2;
    
    // 属性
    public string MobName => mobName;
    public int Level => level;
    public int ExperienceReward => experienceReward;
    public int AttackPower => attackPower;
    public int Defense => defense;
    public int Agility => agility;
    
    /// <summary>
    /// 设置怪物属性（由派生类在Start中使用）
    /// </summary>
    protected void SetMobProperties(string name, int newMaxHp, int newAttackPower, int newDefense, int newAgility, int newExp)
    {
        mobName = name;
        maxHp = newMaxHp;
        hp = newMaxHp;
        attackPower = newAttackPower;
        defense = newDefense;
        agility = newAgility;
        experienceReward = newExp;
    }
    
    // AI行为
    protected IAIBehavior aiBehavior;
    protected bool isInCombat = false;
    protected Hero targetHero = null;
    
    protected override void Start()
    {
        base.Start();
        
        // 获取AI行为组件
        aiBehavior = GetComponent<IAIBehavior>();
        if (aiBehavior == null)
        {
            Debug.LogWarning($"{MobName}: No AI behavior found, using default BasicAI");
            // 默认使用BasicAI
            gameObject.AddComponent<BasicAI>();
            aiBehavior = GetComponent<IAIBehavior>();
        }
        
        Debug.Log($"<color=cyan>✓ {MobName} (Lv.{level}) 生成 - HP: {hp}/{maxHp}, 攻击力: {attackPower}</color>");
    }
    
    private void Update()
    {
        if (!isInCombat && aiBehavior != null)
        {
            // 搜索附近的敌人
            if (TryFindNearbyHero())
            {
                isInCombat = true;
                Debug.Log($"<color=yellow>{MobName} 发现了敌人！</color>");
            }
        }
        
        if (isInCombat && aiBehavior != null && targetHero != null)
        {
            // 执行AI行为
            Level currentLevel = LevelManager.Instance?.CurrentLevel;
            if (currentLevel != null)
            {
                aiBehavior.Act(this, targetHero, currentLevel);
            }
        }
    }
    
    /// <summary>
    /// 尝试在附近找到Hero
    /// </summary>
    private bool TryFindNearbyHero()
    {
        if (targetHero != null)
            return true;
        
        Hero hero = GameManager.Instance?.Hero;
        if (hero != null)
        {
            // 计算与Hero的距离
            int distanceX = Mathf.Abs(pos.x - hero.pos.x);
            int distanceY = Mathf.Abs(pos.y - hero.pos.y);
            int distance = distanceX + distanceY; // 曼哈顿距离
            
            if (distance <= 20) // 发现范围20格
            {
                targetHero = hero;
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 尝试攻击目标
    /// </summary>
    public bool TryAttack(Mob target)
    {
        if (target == null)
            return false;
        
        // 计算伤害
        int damage = CombatCalculator.CalculateDamage(this, target);
        
        target.TakeDamage(damage);
        
        Debug.Log($"<color=red>{MobName} 对 {target.MobName} 造成 {damage} 伤害</color>");
        
        return true;
    }
    
    /// <summary>
    /// 尝试攻击Hero目标 (重载)
    /// </summary>
    public bool TryAttack(Hero target)
    {
        if (target == null)
            return false;
        
        // 计算伤害
        int damage = CombatCalculator.CalculateDamage(this, target);
        
        target.TakeDamage(damage);
        
        Debug.Log($"<color=red>{MobName} 对 Hero 造成 {damage} 伤害</color>");
        
        return true;
    }
    
    /// <summary>
    /// 尝试移动
    /// </summary>
    public bool TryMoveTo(Vector2Int targetPos, Level level)
    {
        if (level != null && level.IsPassable(targetPos))
        {
            pos = targetPos;
            transform.position = new Vector3(targetPos.x + 0.5f, targetPos.y + 0.5f, 0);
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// 受到伤害
    /// </summary>
    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        
        if (hp > 0 && !isInCombat)
        {
            isInCombat = true;
        }
    }
    
    /// <summary>
    /// 死亡
    /// </summary>
    public override void Die()
    {
        Debug.Log($"<color=green>{MobName} 被击败了！获得 {experienceReward} 经验值</color>");
        
        // 给Hero奖励经验
        if (GameManager.Instance?.Hero != null)
        {
            GameManager.Instance.Hero.GainExperience(experienceReward);
        }
        
        base.Die();
    }
    
    /// <summary>
    /// 返回到非战斗状态
    /// </summary>
    public void ExitCombat()
    {
        isInCombat = false;
        targetHero = null;
    }
}
