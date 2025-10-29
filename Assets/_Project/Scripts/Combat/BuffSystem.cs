using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

/// <summary>
/// Buff/Debuff系统 - 管理角色的状态效果
/// </summary>
public class BuffSystem : MonoBehaviour
{
    [Header("Buff设置")]
    [SerializeField] private List<Buff> activeBuffs = new List<Buff>();
    [SerializeField] private bool showBuffIcons = true;

    // Buff事件
    public event Action<Buff> OnBuffAdded;
    public event Action<Buff> OnBuffRemoved;
    public event Action<Buff> OnBuffExpired;

    private Actor owner;

    private void Awake()
    {
        owner = GetComponent<Actor>();
    }

    private void Update()
    {
        UpdateBuffs(Time.deltaTime);
    }

    /// <summary>
    /// 添加Buff
    /// </summary>
    public void AddBuff(BuffType type, float duration, float intensity = 1f, string source = "")
    {
        // 检查是否已存在相同类型的Buff
        Buff existingBuff = activeBuffs.Find(b => b.Type == type);
        if (existingBuff != null)
        {
            // 刷新持续时间
            existingBuff.RefreshDuration(duration);
            existingBuff.Intensity = Mathf.Max(existingBuff.Intensity, intensity);
            Debug.Log($"<color=cyan>刷新Buff: {type} (强度: {intensity})</color>");
        }
        else
        {
            // 创建新Buff
            Buff newBuff = new Buff(type, duration, intensity, source);
            activeBuffs.Add(newBuff);
            ApplyBuffEffect(newBuff);

            Debug.Log($"<color=green>添加Buff: {type} (强度: {intensity}, 持续: {duration}s)</color>");
            OnBuffAdded?.Invoke(newBuff);

            // 通知Actor触发UI更新
            if (owner != null)
            {
                owner.NotifyBuffChanged(type, true);
            }
        }
    }

    /// <summary>
    /// 移除Buff
    /// </summary>
    public void RemoveBuff(BuffType type)
    {
        Buff buffToRemove = activeBuffs.Find(b => b.Type == type);
        if (buffToRemove != null)
        {
            RemoveBuff(buffToRemove);
        }
    }

    /// <summary>
    /// 移除指定Buff
    /// </summary>
    private void RemoveBuff(Buff buff)
    {
        if (buff == null) return;

        RemoveBuffEffect(buff);
        activeBuffs.Remove(buff);

        Debug.Log($"<color=yellow>移除Buff: {buff.Type}</color>");
        OnBuffRemoved?.Invoke(buff);

        // 通知Actor触发UI更新
        if (owner != null)
        {
            owner.NotifyBuffChanged(buff.Type, false);
        }
    }

    /// <summary>
    /// 更新所有Buff
    /// </summary>
    private void UpdateBuffs(float deltaTime)
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            Buff buff = activeBuffs[i];
            buff.Update(deltaTime);

            if (buff.IsExpired)
            {
                RemoveBuff(buff);
                OnBuffExpired?.Invoke(buff);
            }
        }
    }

    /// <summary>
    /// 应用Buff效果
    /// </summary>
    private void ApplyBuffEffect(Buff buff)
    {
        if (owner == null) return;

        // 注意：由于Hero和Mob的属性是只读的，我们使用临时加成的方法
        // 在获取属性时通过GetBuffBonus方法计算临时加成

        switch (buff.Type)
        {
            case BuffType.Strength:
            case BuffType.Agility:
            case BuffType.Defense:
                // 这些属性加成将在获取时动态计算
                Debug.Log($"应用{buff.Type} Buff，强度: {buff.Intensity}");
                break;

            case BuffType.Poison:
                // 毒伤将在Update中处理
                break;

            case BuffType.Regeneration:
                // 再生将在Update中处理
                break;

            case BuffType.Haste:
                // 加速效果（可以在移动系统中处理）
                break;

            case BuffType.Slow:
                // 减速效果
                break;
        }
    }

    /// <summary>
    /// 移除Buff效果
    /// </summary>
    private void RemoveBuffEffect(Buff buff)
    {
        if (owner == null) return;

        switch (buff.Type)
        {
            case BuffType.Strength:
            case BuffType.Agility:
            case BuffType.Defense:
                // 这些属性加成将在获取时动态移除
                Debug.Log($"移除{buff.Type} Buff，强度: {buff.Intensity}");
                break;
        }
    }

    /// <summary>
    /// 获取指定类型的Buff强度
    /// </summary>
    public float GetBuffIntensity(BuffType type)
    {
        Buff buff = activeBuffs.Find(b => b.Type == type);
        return buff != null ? buff.Intensity : 0f;
    }

    /// <summary>
    /// 检查是否有指定类型的Buff
    /// </summary>
    public bool HasBuff(BuffType type)
    {
        return activeBuffs.Exists(b => b.Type == type);
    }

    /// <summary>
    /// 清除所有Buff
    /// </summary>
    public void ClearAllBuffs()
    {
        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            RemoveBuff(activeBuffs[i]);
        }
    }

    /// <summary>
    /// 获取属性类型的Buff总加成值
    /// </summary>
    public int GetAttributeBonus(BuffType attributeType)
    {
        if (attributeType != BuffType.Strength &&
            attributeType != BuffType.Agility &&
            attributeType != BuffType.Defense)
        {
            return 0;
        }

        var attributeBuffs = activeBuffs.Where(b => b.Type == attributeType).ToList();
        float totalBonus = 0f;

        foreach (Buff buff in attributeBuffs)
        {
            switch (attributeType)
            {
                case BuffType.Strength:
                    totalBonus += 5 * buff.Intensity;
                    break;
                case BuffType.Agility:
                    totalBonus += 3 * buff.Intensity;
                    break;
                case BuffType.Defense:
                    totalBonus += 2 * buff.Intensity;
                    break;
            }
        }

        return (int)totalBonus;
    }

    /// <summary>
    /// 获取所有激活的Buff
    /// </summary>
    public List<Buff> GetActiveBuffs()
    {
        return new List<Buff>(activeBuffs);
    }
}

/// <summary>
/// Buff类型枚举
/// </summary>
public enum BuffType
{
    Strength,       // 力量提升
    Agility,        // 敏捷提升
    Defense,        // 防御提升
    Poison,         // 中毒
    Regeneration,   // 再生
    Haste,          // 加速
    Slow,           // 减速
    Shield,         // 护盾
    Weakness,       // 虚弱
    Blind,          // 失明
    Confusion,      // 混乱
    Sleep,          // 睡眠
    Paralysis,      // 麻痹
    Bleeding,       // 流血
    Burning,        // 燃烧
    Frozen,         // 冰冻
    Charmed,        // 魅惑
    Terror,         // 恐惧
    Invisibility,   // 隐身
    Levitation,     // 漂浮
    MindVision,     // 心灵视野
    Fury,           // 狂暴
    Blessing,       // 祝福
    Curse,          // 诅咒
    Light,          // 发光
    Shadow,         // 暗影
    Roots,          // 缠绕
    Vertigo,        // 眩晕
    Hunger,         // 饥饿
    Satiation       // 饱食
}

/// <summary>
/// Buff类 - 表示单个状态效果
/// </summary>
[System.Serializable]
public class Buff
{
    [SerializeField] private BuffType type;
    [SerializeField] private float duration;
    [SerializeField] private float intensity;
    [SerializeField] private string source;
    [SerializeField] private float remainingTime;
    [SerializeField] private bool isExpired;

    public BuffType Type => type;
    public float Duration => duration;
    public float RemainingTime => remainingTime;
    public float Intensity
    {
        get => intensity;
        set => intensity = Mathf.Max(0.1f, value);
    }
    public string Source => source;
    public bool IsExpired => isExpired;
    public float NormalizedTime => duration > 0 ? remainingTime / duration : 0f;

    public Buff(BuffType type, float duration, float intensity = 1f, string source = "")
    {
        this.type = type;
        this.duration = duration;
        this.intensity = Mathf.Max(0.1f, intensity);
        this.source = source;
        this.remainingTime = duration;
        this.isExpired = false;
    }

    /// <summary>
    /// 更新Buff（每帧调用）
    /// </summary>
    public void Update(float deltaTime)
    {
        if (isExpired) return;

        remainingTime -= deltaTime;
        if (remainingTime <= 0f)
        {
            isExpired = true;
            remainingTime = 0f;
        }
    }

    /// <summary>
    /// 刷新持续时间
    /// </summary>
    public void RefreshDuration(float newDuration)
    {
        duration = newDuration;
        remainingTime = newDuration;
        isExpired = false;
    }

    /// <summary>
    /// 立即过期
    /// </summary>
    public void Expire()
    {
        isExpired = true;
        remainingTime = 0f;
    }

    /// <summary>
    /// 获取Buff描述
    /// </summary>
    public string GetDescription()
    {
        string effect = type switch
        {
            BuffType.Strength => $"力量 +{(int)(5 * intensity)}",
            BuffType.Agility => $"敏捷 +{(int)(3 * intensity)}",
            BuffType.Defense => $"防御 +{(int)(2 * intensity)}",
            BuffType.Poison => $"中毒 ({intensity:F1}/回合)",
            BuffType.Regeneration => $"再生 ({intensity:F1}/回合)",
            BuffType.Haste => $"加速 {intensity:P0}",
            BuffType.Slow => $"减速 {intensity:P0}",
            BuffType.Shield => $"护盾 {(int)(10 * intensity)}",
            BuffType.Weakness => $"虚弱 {intensity:P0}",
            BuffType.Bleeding => $"流血 ({intensity:F1}/回合)",
            BuffType.Burning => $"燃烧 ({intensity:F1}/回合)",
            BuffType.Frozen => "冰冻",
            BuffType.Paralysis => "麻痹",
            BuffType.Sleep => "睡眠",
            BuffType.Blind => "失明",
            BuffType.Invisibility => "隐身",
            _ => type.ToString()
        };

        return $"{effect} ({remainingTime:F1}s)";
    }
}