using UnityEngine;

/// <summary>
/// Goblin (地精) - 狡猾型怪物
/// 属性: HP 30, 攻击力 6, 防御 2, 敏捷 3
/// 经验: 25
/// 战略: BasicAI - 追击与攻击
/// </summary>
public class Goblin : Mob
{
    protected override void Start()
    {
        SetMobProperties("地精", 30, 6, 2, 3, 25);
        
        if (GetComponent<IAIBehavior>() == null)
        {
            gameObject.AddComponent<BasicAI>();
        }
        
        base.Start();
    }
}
