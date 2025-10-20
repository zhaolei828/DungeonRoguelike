using UnityEngine;

/// <summary>
/// Hero 英雄类 - 玩家控制的主角
/// </summary>
public class Hero : Actor
{
    [Header("英雄属性")]
    [SerializeField] private HeroClass heroClass = HeroClass.Warrior;
    [SerializeField] private int level = 1;
    [SerializeField] private int experience = 0;
    [SerializeField] private int mana = 100;
    [SerializeField] private int maxMana = 100;
    
    [Header("英雄装备")]
    [SerializeField] private int armor = 0;
    [SerializeField] private int strength = 10;
    [SerializeField] private int intelligence = 10;
    [SerializeField] private int willpower = 10;
    
    // 属性
    public HeroClass Class => heroClass;
    public int Level => level;
    public int Experience => experience;
    public int Mana => mana;
    public int MaxMana => maxMana;
    public int Armor => armor;
    public int Strength => strength;
    public int Intelligence => intelligence;
    public int Willpower => willpower;
    
    protected override void Start()
    {
        base.Start();
        
        // 根据职业初始化属性
        InitializeByClass();
        
        Debug.Log($"Hero initialized: {heroClass} Level {level}");
    }
    
    /// <summary>
    /// 根据职业初始化属性
    /// </summary>
    private void InitializeByClass()
    {
        switch (heroClass)
        {
            case HeroClass.Warrior:
                hp = 30;
                maxHp = 30;
                armor = 5;
                strength = 15;
                break;
                
            case HeroClass.Mage:
                hp = 20;
                maxHp = 20;
                mana = 150;
                maxMana = 150;
                intelligence = 18;
                break;
                
            case HeroClass.Rogue:
                hp = 25;
                maxHp = 25;
                armor = 2;
                strength = 12;
                break;
                
            case HeroClass.Huntress:
                hp = 25;
                maxHp = 25;
                strength = 13;
                willpower = 12;
                break;
                
            case HeroClass.Cleric:
                hp = 25;
                maxHp = 25;
                mana = 100;
                maxMana = 100;
                willpower = 15;
                break;
        }
    }
    
    /// <summary>
    /// 增加经验
    /// </summary>
    public void GainExperience(int amount)
    {
        experience += amount;
        Debug.Log($"Hero gained {amount} experience. Total: {experience}");
        
        // 检查升级
        CheckLevelUp();
    }
    
    /// <summary>
    /// 检查是否升级
    /// </summary>
    private void CheckLevelUp()
    {
        int expForNextLevel = level * 100;
        if (experience >= expForNextLevel)
        {
            level++;
            maxHp += 5;
            hp = maxHp;
            maxMana += 10;
            mana = maxMana;
            strength += 2;
            intelligence += 2;
            willpower += 2;
            
            Debug.Log($"<color=yellow>Hero leveled up! Level: {level}</color>");
        }
    }
    
    /// <summary>
    /// 恢复法力
    /// </summary>
    public void RestoreMana(int amount)
    {
        mana = Mathf.Min(mana + amount, maxMana);
    }
    
    /// <summary>
    /// 消耗法力
    /// </summary>
    public bool ConsumeMana(int amount)
    {
        if (mana >= amount)
        {
            mana -= amount;
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// 装备物品增加属性
    /// </summary>
    public void EquipArmor(int armorValue)
    {
        armor += armorValue;
        Debug.Log($"Hero equipped armor. Total armor: {armor}");
    }
    
    /// <summary>
    /// 移动到指定位置
    /// </summary>
    public void MoveTo(Vector2Int targetPos, Level level)
    {
        if (level != null && level.IsPassable(targetPos))
        {
            // 计算移动方向
            Vector2Int direction = targetPos - pos;
            
            pos = targetPos;
            transform.position = new Vector3(targetPos.x + 0.5f, targetPos.y + 0.5f, 0);
            
            // 更新动画
            HeroAnimator animator = GetComponent<HeroAnimator>();
            if (animator != null)
            {
                animator.SetAnimationByDirection(direction);
            }
            
            Debug.Log($"Hero moved to {targetPos}");
        }
    }
}
