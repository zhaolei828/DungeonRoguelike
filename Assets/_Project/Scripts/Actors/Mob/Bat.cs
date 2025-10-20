using UnityEngine;

/// <summary>
/// Bat (蝙蝠) - 敏捷型怪物
/// 属性: HP 15, 攻击力 4, 防御 0, 敏捷 4
/// 经验: 15
/// 战略: BasicAI - 快速追击
/// </summary>
public class Bat : Mob
{
    protected override void Start()
    {
        mobName = "蝙蝠";
        hp = 15;
        maxHp = 15;
        
        if (GetComponent<IAIBehavior>() == null)
        {
            gameObject.AddComponent<BasicAI>();
        }
        
        base.Start();
    }
}
