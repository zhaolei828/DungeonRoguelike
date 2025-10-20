using UnityEngine;

/// <summary>
/// Actor 基类 - 所有可交互角色的基类（Hero、敌人等）
/// </summary>
public abstract class Actor : MonoBehaviour
{
    [Header("基础属性")]
    [SerializeField] protected int hp = 20;
    [SerializeField] protected int maxHp = 20;
    
    // 位置
    public Vector2Int pos = Vector2Int.zero;
    
    // 属性
    public int Hp => hp;
    public int MaxHp => maxHp;
    public Vector2Int Position => pos;
    
    protected virtual void Start()
    {
        // 初始化位置
        pos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
    }
    
    /// <summary>
    /// 受到伤害
    /// </summary>
    public virtual void TakeDamage(int damage)
    {
        hp = Mathf.Max(0, hp - damage);
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {hp}/{maxHp}");
        
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
    }
    
    /// <summary>
    /// 死亡
    /// </summary>
    public virtual void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        Destroy(gameObject);
    }
}
