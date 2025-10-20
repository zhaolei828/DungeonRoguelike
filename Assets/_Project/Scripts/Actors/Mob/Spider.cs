using UnityEngine;

/// <summary>
/// Spider (蜘蛛) - 平衡型怪物
/// 属性: HP 25, 攻击力 5, 防御 2, 敏捷 2
/// 经验: 20
/// 战略: BasicAI - 标准追击
/// </summary>
public class Spider : Mob
{
    protected override void Start()
    {
        mobName = "蜘蛛";
        hp = 25;
        maxHp = 25;
        
        if (GetComponent<IAIBehavior>() == null)
        {
            gameObject.AddComponent<BasicAI>();
        }
        
        base.Start();
    }
}
