using UnityEngine;

/// <summary>
/// 战斗计算器 - 处理所有与战斗相关的计算
/// </summary>
public static class CombatCalculator
{
    /// <summary>
    /// 计算攻击者对防守者的伤害
    /// 公式: (攻击力 + 力量修正 + Buff修正) × (1 + 暴击倍数) - 防御减免
    /// </summary>
    public static int CalculateDamage(Mob attacker, Mob defender)
    {
        if (attacker == null || defender == null)
            return 0;

        // 基础伤害
        int baseDamage = attacker.AttackPower;

        // Buff效果修正
        if (attacker.TryGetComponent(out BuffSystem attackerBuffs))
        {
            // 力量Buff增加伤害
            int strengthBonus = attackerBuffs.GetAttributeBonus(BuffType.Strength) * 2;
            baseDamage += strengthBonus;

            // 狂暴Buff增加伤害
            float furyBonus = attackerBuffs.GetBuffIntensity(BuffType.Fury) * 3f;
            baseDamage += (int)furyBonus;
        }

        // 暴击判定
        float critChance = attacker.Agility * 0.02f; // 每点敏捷增加2%暴击率
        float critMultiplier = 1f;

        if (Random.value < critChance)
        {
            critMultiplier = 1.5f; // 暴击伤害提高50%
            Debug.Log($"<color=yellow>暴击！伤害提升50%</color>");
        }

        // 最终伤害 = (基础伤害 × 暴击倍数) - 防御减免
        int finalDamage = Mathf.Max(1, (int)(baseDamage * critMultiplier) - defender.Defense);

        return finalDamage;
    }
    
    /// <summary>
    /// 计算Hero对Mob的伤害
    /// </summary>
    public static int CalculateDamage(Hero attacker, Mob defender)
    {
        if (attacker == null || defender == null)
            return 0;

        // Hero的攻击力基于力量属性
        int baseDamage = 5 + attacker.Strength / 2;

        // Buff效果修正
        if (attacker.TryGetComponent(out BuffSystem attackerBuffs))
        {
            // 力量Buff增加伤害
            int strengthBonus = attackerBuffs.GetAttributeBonus(BuffType.Strength) * 3;
            baseDamage += strengthBonus;

            // 狂暴Buff增加伤害
            float furyBonus = attackerBuffs.GetBuffIntensity(BuffType.Fury) * 4f;
            baseDamage += (int)furyBonus;

            // 祝福Buff增加伤害
            float blessingBonus = attackerBuffs.GetBuffIntensity(BuffType.Blessing) * 2f;
            baseDamage += (int)blessingBonus;
        }

        // 暴击判定
        float critChance = attacker.Strength * 0.03f; // 每点力量增加3%暴击率
        float critMultiplier = 1f;

        if (Random.value < critChance)
        {
            critMultiplier = 1.8f; // Hero暴击伤害提高80%
            Debug.Log($"<color=yellow>英雄暴击！伤害提升80%</color>");
        }

        // 最终伤害 = (基础伤害 × 暴击倍数) - 防御减免
        int finalDamage = Mathf.Max(1, (int)(baseDamage * critMultiplier) - defender.Defense);

        return finalDamage;
    }
    
    /// <summary>
    /// 计算Mob对Hero的伤害 (重载)
    /// </summary>
    public static int CalculateDamage(Mob attacker, Hero defender)
    {
        if (attacker == null || defender == null)
            return 0;

        // Mob的基础伤害基于攻击力属性
        int baseDamage = attacker.AttackPower;

        // Buff效果修正
        if (attacker.TryGetComponent(out BuffSystem attackerBuffs))
        {
            // 力量Buff增加伤害
            int strengthBonus = attackerBuffs.GetAttributeBonus(BuffType.Strength) * 2;
            baseDamage += strengthBonus;

            // 狂暴Buff增加伤害
            float furyBonus = attackerBuffs.GetBuffIntensity(BuffType.Fury) * 3f;
            baseDamage += (int)furyBonus;
        }

        // 暴击判定
        float critChance = attacker.Agility * 0.02f; // 每点敏捷增加2%暴击率
        float critMultiplier = 1f;

        if (Random.value < critChance)
        {
            critMultiplier = 1.5f; // Mob暴击伤害提高50%
            Debug.Log($"<color=yellow>暴击！伤害提升50%</color>");
        }

        // 最终伤害 = (基础伤害 × 暴击倍数) - 防御减免
        // Hero的防御基于Armor属性
        int finalDamage = Mathf.Max(1, (int)(baseDamage * critMultiplier) - defender.Armor);

        return finalDamage;
    }
    
    /// <summary>
    /// 计算闪躲概率
    /// </summary>
    public static bool TryDodge(Actor defender)
    {
        if (defender is Mob mob)
        {
            float dodgeChance = mob.Agility * 0.01f; // 每点敏捷增加1%闪躲率
            if (Random.value < dodgeChance)
            {
                Debug.Log($"<color=green>闪躲成功！</color>");
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// 计算获得的经验值
    /// 公式: 基础经验 × (怪物等级 / Hero等级) × (1 + 难度倍数)
    /// </summary>
    public static int CalculateExperience(Mob mob, Hero hero)
    {
        if (mob == null || hero == null)
            return 0;
        
        float levelRatio = (float)mob.Level / hero.Level;
        int experienceGain = (int)(mob.ExperienceReward * levelRatio * 1.2f);
        
        return Mathf.Max(1, experienceGain);
    }
    
    /// <summary>
    /// 计算两个角色之间的曼哈顿距离
    /// </summary>
    public static int CalculateDistance(Actor a, Actor b)
    {
        if (a == null || b == null)
            return 999;
        
        int distanceX = Mathf.Abs(a.pos.x - b.pos.x);
        int distanceY = Mathf.Abs(a.pos.y - b.pos.y);
        
        return distanceX + distanceY;
    }
}
