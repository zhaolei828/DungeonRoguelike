# Week 6 战斗系统使用指南

##  快速开始

### 1. 在编辑器中生成怪物

在Game.unity或TestDungeon.unity场景中：

**菜单位置**: Tools > Dungeon > Spawn Mob > 选择怪物类型

`
Tools/Dungeon/Spawn Mob/
 Rat      (老鼠)
 Bat      (蝙蝠)
 Spider   (蜘蛛)
 Goblin   (地精)
 Orc      (兽人)
 Random Mix (生成5个混合小队)
`

### 2. 进入Play模式测试

点击Play进入游戏：
- 按WASD移动Hero
- 按E键发起战斗（靠近敌人时）

### 3. 战斗流程

\\\
Hero靠近敌人
    
按E键发起战斗
    
TurnManager按敏捷度排序回合
    
Hero回合: 按E键攻击 (或其他操作)
    
敌人回合: AI自动行动
    
重复直到一方死亡
\\\

---

##  5种怪物对比

| 怪物 | 血量 | 攻击 | 防御 | 敏捷 | 经验 | 特点 |
|------|------|------|------|------|------|------|
| 老鼠 | 20 | 3 | 1 | 2 | 10 | 最弱，新手练习 |
| 蝙蝠 | 15 | 4 | 0 | 4 | 15 | 敏捷，容易先手 |
| 蜘蛛 | 25 | 5 | 2 | 2 | 20 | 平衡型 |
| 地精 | 30 | 6 | 2 | 3 | 25 | 中等难度 |
| 兽人 | 40 | 8 | 4 | 2 | 35 | 困难boss |

---

##  战斗计算

### 伤害计算

\\\
伤害 = (攻击力  暴击倍数) - 防御
暴击率 = 敏捷度  2%
暴击伤害倍数 = 1.5倍 (敌人) / 1.8倍 (Hero)
\\\

### 经验获得

战斗胜利后Hero获得经验：
\\\
经验 = 怪物经验  (怪物等级 / Hero等级)
\\\

---

##  战斗系统架构

### 核心类

1. **Mob.cs** - 怪物基类
   - 属性: HP, Attack, Defense, Agility
   - 行为: TakeDamage(), Die(), TryAttack()

2. **TurnManager.cs** - 回合管理
   - 管理战斗状态
   - 回合顺序执行
   - 胜负判定

3. **CombatCalculator.cs** - 战斗计算
   - 伤害计算
   - 暴击/闪躲
   - 经验计算

4. **BasicAI.cs** - 基础AI
   - 敌人自动行动
   - 靠近并攻击Hero

### 流程图

\\\
PlayerInput (玩家输入)
    
E键  TryAttackNearbyEnemy()
    
检测附近敌人 (距离 <= 1)
    
TurnManager.StartBattle()
    
初始化回合顺序 (按敏捷度)
    
循环执行:
 Hero回合  按E键攻击
 计算伤害  CombatCalculator
 敌人受伤  TakeDamage()
 检查死亡  结束或继续
 敌人回合  AI自动执行
\\\

---

##  自定义怪物

### 快速创建新怪物

1. 创建派生类:

\\\csharp
public class Dragon : Mob
{
    protected override void Start()
    {
        SetMobProperties(\"龙\", 100, 15, 8, 3, 200);
        if (GetComponent<IAIBehavior>() == null)
        {
            gameObject.AddComponent<BasicAI>();
        }
        base.Start();
    }
}
\\\

2. 在MobSpawnerTool中添加菜单:

\\\csharp
[MenuItem(\"Tools/Dungeon/Spawn Mob/Dragon\")]
public static void SpawnDragon()
{
    SpawnMob<Dragon>(\"龙\", \"Dragon\");
}
\\\

3. 编译后就能在编辑器菜单中使用

---

##  AI行为系统

### 扩展AI

当前只有BasicAI (直线追击)

计划实现:
- **SkittishAI**: 看到敌人就逃跑
- **AggressiveAI**: 主动追击，频繁攻击
- **SmartAI**: 计算伤害，选择最优策略
- **GroupAI**: 与其他怪物配合

### 切换AI

\\\csharp
// 在Mob派生类中
if (GetComponent<IAIBehavior>() == null)
{
    gameObject.AddComponent<SmartAI>();  // 换成其他AI
}
\\\

---

##  调试技巧

### Console输出

战斗时查看Console获取详细信息:

\\\
 战斗开始！Hero vs 蝙蝠
 Hero的回合
 Hero 对 蝙蝠 造成 8 伤害
 蝙蝠 的回合
 蝙蝠 对 Hero 造成 3 伤害
 胜利！蝙蝠 被击败了！
\\\

### 检查战斗状态

\\\csharp
if (TurnManager.Instance.IsInBattle)
{
    Debug.Log(TurnManager.Instance.CurrentState);
}
\\\

---

##  下一步计划

### Week 6 剩余工作

- [ ] 添加战斗UI (血条, 伤害数字)
- [ ] 实现逃离战斗机制
- [ ] 添加战斗音效
- [ ] 怪物掉落物品
- [ ] 其他AI策略

### Week 7 计划

- 物品系统 (50种物品)
- 背包管理
- 装备系统
- 物品合成

---

##  已知问题

1. **Hero攻击**: 目前只能按E发起战斗，战斗中需要按E继续攻击
2. **AI延迟**: 敌人回合有1秒延迟，可配置
3. **Sprite缺失**: 怪物需要配置sprite显示
4. **无逃离**: 暂未实现逃离战斗

---

**总结**: 完整的回合制战斗系统已实现，可以进行基础的Hero vs Mob战斗！
