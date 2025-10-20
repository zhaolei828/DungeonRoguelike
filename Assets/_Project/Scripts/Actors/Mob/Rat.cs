using UnityEngine;

/// <summary>
/// Rat (老鼠) - 最基础的怪物
/// 属性: HP 20, 攻击力 3, 防御 1, 敏捷 2
/// 经验: 10
/// 战略: BasicAI - 直线追击
/// </summary>
public class Rat : Mob
{
    protected override void Start()
    {
        // 设置老鼠的属性
        SetMobProperties("老鼠", 20, 3, 1, 2, 10);
        
        // 添加BasicAI行为
        if (GetComponent<IAIBehavior>() == null)
        {
            gameObject.AddComponent<BasicAI>();
        }
        
        base.Start();
    }
}
