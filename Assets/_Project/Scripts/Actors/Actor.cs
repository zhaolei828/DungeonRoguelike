using UnityEngine;

/// <summary>
/// Actor 基类 - 所有可交互角色的基类（Hero、敌人等）
/// </summary>
public abstract class Actor : MonoBehaviour
{
    [Header("基础属性")]
    [SerializeField] protected int hp = 20;
    [SerializeField] protected int maxHp = 20;
    
    [Header("UI设置")]
    [SerializeField] protected bool showHealthBar = true;
    [SerializeField] protected Vector3 healthBarOffset = new Vector3(0, 0.8f, 0);
    
    // 位置
    public Vector2Int pos = Vector2Int.zero;
    
    // UI组件
    protected HealthBar healthBar;
    
    // 属性
    public int Hp => hp;
    public int MaxHp => maxHp;
    public Vector2Int Position => pos;
    
    protected virtual void Start()
    {
        // 初始化位置
        pos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        
        // 创建血条
        if (showHealthBar && HealthBarManager.Instance != null)
        {
            healthBar = HealthBarManager.Instance.CreateHealthBar(transform, maxHp, healthBarOffset);
        }
    }
    
    /// <summary>
    /// 受到伤害
    /// </summary>
    public virtual void TakeDamage(int damage, bool isCritical = false)
    {
        hp = Mathf.Max(0, hp - damage);
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {hp}/{maxHp}");
        
        // 显示伤害数字
        if (DamageNumberManager.Instance != null)
        {
            Vector3 damagePos = transform.position + Vector3.up * 0.5f;
            DamageNumberManager.Instance.ShowDamage(damagePos, damage, isCritical);
        }
        
        // 更新血条
        if (healthBar != null)
        {
            healthBar.UpdateHealth(hp);
        }
        
        if (hp <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// 治疗
    /// </summary>
    public virtual void Heal(int amount)
    {
        hp = Mathf.Min(hp + amount, maxHp);
        Debug.Log($"{gameObject.name} healed {amount}. HP: {hp}/{maxHp}");
        
        // 显示治疗数字
        if (DamageNumberManager.Instance != null)
        {
            Vector3 healPos = transform.position + Vector3.up * 0.5f;
            DamageNumberManager.Instance.ShowHeal(healPos, amount);
        }
        
        // 更新血条
        if (healthBar != null)
        {
            healthBar.UpdateHealth(hp);
        }
    }
    
    /// <summary>
    /// 死亡
    /// </summary>
    public virtual void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        
        // 销毁血条
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }
        
        Destroy(gameObject);
    }
    
    private void Update()
    {
        // 每帧更新血条（平滑动画）
        if (healthBar != null)
        {
            healthBar.UpdateHealth(hp);
        }
    }
}
