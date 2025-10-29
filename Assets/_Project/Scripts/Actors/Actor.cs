using UnityEngine;
using System.Collections.Generic;
using System;

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

    // Buff系统
    protected BuffSystem buffSystem;

    // UI事件
    public event Action<int, int, bool> OnHealthChanged; // (currentHp, maxHp, isCritical)
    public event Action<BuffType, bool> OnBuffChanged; // (buffType, isAdded)

    // 属性
    public int Hp => hp;
    public int MaxHp => maxHp;
    public Vector2Int Position => pos;
    public BuffSystem BuffSystem => buffSystem;
    
    protected virtual void Start()
    {
        // 初始化位置
        pos = new Vector2Int((int)transform.position.x, (int)transform.position.y);

        // 初始化Buff系统
        buffSystem = GetComponent<BuffSystem>();
        if (buffSystem == null)
        {
            buffSystem = gameObject.AddComponent<BuffSystem>();
        }

        // 创建血条
        if (showHealthBar)
        {
            if (HealthBarManager.Instance != null)
            {
                healthBar = HealthBarManager.Instance.CreateHealthBar(transform, maxHp, healthBarOffset);
                if (healthBar != null)
                {
                    Debug.Log($"<color=cyan>✓ {gameObject.name} 血条创建成功</color>");
                }
                else
                {
                    Debug.LogError($"{gameObject.name}: CreateHealthBar返回null！");
                }
            }
            else
            {
                Debug.LogError($"{gameObject.name}: HealthBarManager.Instance为null，无法创建血条！");
            }
        }
    }
    
    /// <summary>
    /// 受到伤害
    /// </summary>
    public virtual void TakeDamage(int damage, bool isCritical = false)
    {
        // 检查护盾Buff
        if (buffSystem != null && buffSystem.HasBuff(BuffType.Shield))
        {
            float shieldStrength = buffSystem.GetBuffIntensity(BuffType.Shield) * 10f;
            int shieldedDamage = Mathf.Min(damage, (int)shieldStrength);
            damage -= shieldedDamage;

            if (shieldedDamage > 0)
            {
                Debug.Log($"<color=cyan>护盾吸收了 {shieldedDamage} 点伤害！</color>");

                // 显示护盾吸收数字
                if (DamageNumberManager.Instance != null)
                {
                    Vector3 shieldPos = transform.position + Vector3.up * 0.7f;
                    DamageNumberManager.Instance.ShowDamage(shieldPos, shieldedDamage, DamageNumber.DamageType.Shield);
                }

                // 减少护盾强度
                Buff shieldBuff = buffSystem.GetActiveBuffs().Find(b => b.Type == BuffType.Shield);
                if (shieldBuff != null)
                {
                    shieldBuff.Intensity -= shieldedDamage / 10f;
                    if (shieldBuff.Intensity <= 0)
                    {
                        buffSystem.RemoveBuff(BuffType.Shield);
                    }
                }
            }
        }

        // 检查虚弱效果
        if (buffSystem != null && buffSystem.HasBuff(BuffType.Weakness))
        {
            float weaknessMultiplier = 1f + buffSystem.GetBuffIntensity(BuffType.Weakness) * 0.2f;
            damage = (int)(damage * weaknessMultiplier);
            Debug.Log($"<color=red>虚弱效果使伤害增加 {((weaknessMultiplier - 1f) * 100f):F0}%！</color>");
        }

        if (damage <= 0)
        {
            Debug.Log($"<color=green>{gameObject.name} 完全免疫了这次攻击！</color>");
            return;
        }

        hp = Mathf.Max(0, hp - damage);
        Debug.Log($"{gameObject.name} took {damage} damage. HP: {hp}/{maxHp}");

        // 显示伤害数字
        if (DamageNumberManager.Instance != null)
        {
            Vector3 damagePos = transform.position + Vector3.up * 0.5f;
            DamageNumber.DamageType damageType = isCritical ? DamageNumber.DamageType.Critical : DamageNumber.DamageType.Normal;
            DamageNumberManager.Instance.ShowDamage(damagePos, damage, damageType);
        }

        // 更新血条
        if (healthBar != null)
        {
            healthBar.UpdateHealth(hp);
        }

        // 触发UI事件
        OnHealthChanged?.Invoke(hp, maxHp, isCritical);

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

        // 触发UI事件
        OnHealthChanged?.Invoke(hp, maxHp, false);
    }

    /// <summary>
    /// 通知Buff变化（由BuffSystem调用）
    /// </summary>
    public void NotifyBuffChanged(BuffType type, bool isAdded)
    {
        OnBuffChanged?.Invoke(type, isAdded);
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
            healthBar = null;
        }
        
        // 如果是Hero，触发重生系统
        if (this is Hero hero)
        {
            if (RespawnManager.Instance != null)
            {
                RespawnManager.Instance.OnHeroDied(hero);
                return; // 不销毁Hero，由重生系统处理
            }
        }
        
        // 非Hero或没有重生系统，直接销毁
        Destroy(gameObject);
    }
    
    private void Update()
    {
        // 处理持续Buff效果
        if (buffSystem != null)
        {
            ProcessContinuousBuffEffects();
        }

        // 每帧更新血条（平滑动画）
        if (healthBar != null)
        {
            healthBar.UpdateHealth(hp);
        }
    }

    /// <summary>
    /// 处理持续Buff效果（毒伤、再生等）
    /// </summary>
    private void ProcessContinuousBuffEffects()
    {
        List<Buff> activeBuffs = buffSystem.GetActiveBuffs();
        float deltaTime = Time.deltaTime;

        foreach (Buff buff in activeBuffs)
        {
            switch (buff.Type)
            {
                case BuffType.Poison:
                    // 毒伤：每3秒造成一次伤害
                    if (buff.RemainingTime % 3f < deltaTime)
                    {
                        int poisonDamage = Mathf.Max(1, (int)(2 * buff.Intensity));
                        hp = Mathf.Max(0, hp - poisonDamage);
                        Debug.Log($"<color=green>{buff.Type} 造成 {poisonDamage} 伤害！HP: {hp}/{maxHp}</color>");

                        if (DamageNumberManager.Instance != null)
                        {
                            Vector3 damagePos = transform.position + Vector3.up * 0.5f;
                            DamageNumberManager.Instance.ShowDamage(damagePos, poisonDamage, false);
                        }
                    }
                    break;

                case BuffType.Regeneration:
                    // 再生：每2秒恢复一次生命
                    if (buff.RemainingTime % 2f < deltaTime)
                    {
                        int healAmount = Mathf.Max(1, (int)(3 * buff.Intensity));
                        hp = Mathf.Min(hp + healAmount, maxHp);
                        Debug.Log($"<color=green>{buff.Type} 恢复 {healAmount} 生命！HP: {hp}/{maxHp}</color>");

                        if (DamageNumberManager.Instance != null)
                        {
                            Vector3 healPos = transform.position + Vector3.up * 0.5f;
                            DamageNumberManager.Instance.ShowHeal(healPos, healAmount);
                        }
                    }
                    break;

                case BuffType.Bleeding:
                    // 流血：每4秒造成一次伤害
                    if (buff.RemainingTime % 4f < deltaTime)
                    {
                        int bleedDamage = Mathf.Max(1, (int)(1 * buff.Intensity));
                        hp = Mathf.Max(0, hp - bleedDamage);
                        Debug.Log($"<color=red>{buff.Type} 造成 {bleedDamage} 伤害！HP: {hp}/{maxHp}</color>");

                        if (DamageNumberManager.Instance != null)
                        {
                            Vector3 damagePos = transform.position + Vector3.up * 0.5f;
                            DamageNumberManager.Instance.ShowDamage(damagePos, bleedDamage, false);
                        }
                    }
                    break;

                case BuffType.Burning:
                    // 燃烧：每1.5秒造成一次伤害
                    if (buff.RemainingTime % 1.5f < deltaTime)
                    {
                        int burnDamage = Mathf.Max(1, (int)(1.5f * buff.Intensity));
                        hp = Mathf.Max(0, hp - burnDamage);
                        Debug.Log($"<color=orange>{buff.Type} 造成 {burnDamage} 伤害！HP: {hp}/{maxHp}</color>");

                        if (DamageNumberManager.Instance != null)
                        {
                            Vector3 damagePos = transform.position + Vector3.up * 0.5f;
                            DamageNumberManager.Instance.ShowDamage(damagePos, burnDamage, false);
                        }
                    }
                    break;
            }
        }
    }
}
