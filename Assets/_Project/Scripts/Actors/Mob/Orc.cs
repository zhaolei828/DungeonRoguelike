using UnityEngine;

/// <summary>
/// Orc (兽人) - 防御型怪物
/// 属性: HP 40, 攻击力 8, 防御 4, 敏捷 2
/// 经验: 35
/// 战略: BasicAI - 强力追击
/// </summary>
public class Orc : Mob
{
    protected override void Start()
    {
        mobName = "兽人";
        hp = 40;
        maxHp = 40;
        
        if (GetComponent<IAIBehavior>() == null)
        {
            gameObject.AddComponent<BasicAI>();
        }
        
        base.Start();
    }
}
