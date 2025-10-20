using UnityEngine;

/// <summary>
/// AI行为接口 - 定义所有AI的标准行为
/// </summary>
public interface IAIBehavior
{
    /// <summary>
    /// 执行AI行为
    /// </summary>
    /// <param name="self">自身Mob</param>
    /// <param name="target">目标Hero</param>
    /// <param name="level">当前关卡</param>
    void Act(Mob self, Hero target, Level level);
    
    /// <summary>
    /// 决策-是否应该攻击
    /// </summary>
    bool ShouldAttack(Mob self, Hero target);
    
    /// <summary>
    /// 决策-是否应该逃跑
    /// </summary>
    bool ShouldFlee(Mob self, Hero target);
}
