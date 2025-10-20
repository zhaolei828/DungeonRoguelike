# Week 6: 战斗系统实现计划

##  目标概述

实现游戏的基础战斗系统，使得Hero可以与怪物进行互动和战斗。

**预期成果**: 可执行的战斗流程 (发现怪物  开始战斗  计算伤害  胜利/死亡)

---

##  实现清单

### 1. 怪物系统基础 (1-2天)

- [ ] Mob基类继承Actor
  - [ ] 怪物属性: 类型, 等级, 经验值, 掉落物品
  - [ ] 怪物行为: Act(), TakeDamage(), Die()
  - [ ] 视觉表示: Sprite渲染

- [ ] 5种基础怪物实现
  - [ ] Rat (老鼠) - 血量20, 攻击力3
  - [ ] Bat (蝙蝠) - 血量15, 攻击力4, 移动速度快
  - [ ] Spider (蜘蛛) - 血量25, 攻击力5, 有毒伤害
  - [ ] Goblin (地精) - 血量30, 攻击力6, 会逃跑
  - [ ] Orc (兽人) - 血量40, 攻击力8, 防御力高

### 2. 战斗计算系统 (1-2天)

- [ ] CombatCalculator工具类
  - [ ] 伤害计算: 基础伤害 + 力量修正 - 防御减免
  - [ ] 暴击判定: 基于敏捷度计算暴击率
  - [ ] 闪躲计算: 基于敏捷度计算闪躲率
  - [ ] 经验获得: 基于怪物等级和血量

- [ ] 伤害类型支持
  - [ ] 物理伤害 (Physical)
  - [ ] 魔法伤害 (Magic)
  - [ ] 毒伤 (Poison)
  - [ ] 生命吸取 (LifeSteal)

### 3. AI行为系统 (2-3天)

- [ ] AIBehavior基类
  - [ ] 决策系统: 选择行动 (移动/攻击/技能/逃跑)
  - [ ] 优先级系统: 优先处理敌人 > 逃生 > 探索

- [ ] 简单AI实现 (5种怪物各一个)
  - [ ] BasicAI: 直线追击 + 攻击
  - [ ] SkittishAI: 看到敌人就逃跑
  - [ ] AggressiveAI: 主动追击并频繁攻击
  - [ ] SmartAI: 计算伤害/收益再行动
  - [ ] GroupAI: 尝试与其他怪物配合

### 4. 战斗流程整合 (2天)

- [ ] TurnManager组件
  - [ ] 回合顺序管理 (敏捷度决定)
  - [ ] 回合执行: Hero行动  Mobs行动
  - [ ] 战斗状态: 开始/进行中/结束

- [ ] 战斗触发
  - [ ] 与怪物相邻时自动进入战斗
  - [ ] 按E键或自动触发攻击
  - [ ] 战斗退出条件 (一方死亡或逃离)

### 5. UI和反馈 (1-2天)

- [ ] 战斗HUD显示
  - [ ] 双方血量条
  - [ ] 伤害浮动数字
  - [ ] 战斗日志

- [ ] 音效反馈
  - [ ] 攻击音效
  - [ ] 伤害音效
  - [ ] 胜利/失败音效

### 6. 测试和优化 (1天)

- [ ] 战斗逻辑测试
- [ ] AI行为验证
- [ ] 平衡性调整
- [ ] 性能优化

---

##  架构设计

### 类关系图

`
Actor (基类)
 Hero (玩家)
 Mob (怪物)
     Rat
     Bat
     Spider
     Goblin
     Orc

AIBehavior (接口)
 BasicAI
 SkittishAI
 AggressiveAI
 SmartAI
 GroupAI

CombatCalculator (工具类)
 CalculateDamage()
 CalculateCrit()
 CalculateDodge()
 CalculateExp()

TurnManager (管理器)
 GetNextTurn()
 ExecuteTurn()
 EndBattle()
`

### 文件结构

`
Assets/_Project/Scripts/
 Actors/
    Mob/
       Mob.cs (基类)
       Rat.cs
       Bat.cs
       Spider.cs
       Goblin.cs
       Orc.cs
    AI/
        AIBehavior.cs (接口)
        BasicAI.cs
        SkittishAI.cs
        AggressiveAI.cs
        SmartAI.cs
        GroupAI.cs
 Combat/
    CombatCalculator.cs
    DamageType.cs
    TurnManager.cs
    BattleState.cs
 Utils/
     MobSpawner.cs
`

---

##  预期数据表

### 怪物属性

| 怪物 | HP | 攻击 | 防御 | 敏捷 | 经验 | 掉落 |
|------|----|----|------|------|------|------|
| Rat | 20 | 3 | 1 | 2 | 10 | 金币 |
| Bat | 15 | 4 | 0 | 4 | 15 | 金币 |
| Spider | 25 | 5 | 2 | 2 | 20 | 毒药 |
| Goblin | 30 | 6 | 2 | 3 | 25 | 金币 |
| Orc | 40 | 8 | 4 | 2 | 35 | 盔甲 |

### 战斗计算公式

`
伤害 = (攻击力 + 力量修正) - 防御减免
暴击率 = 敏捷度 / 100
闪躲率 = 敏捷度 / 50
经验 = 怪物经验值  (怪物等级 / Hero等级)
`

---

##  实现顺序

1. **第1天**: 实现Mob基类 + 5种怪物
2. **第2天**: 实现CombatCalculator战斗计算
3. **第3天**: 实现BasicAI和其他AI行为
4. **第4天**: 集成TurnManager和战斗流程
5. **第5天**: 添加UI和音效反馈
6. **第6天**: 测试、平衡性调整、代码优化

---

##  完成检查清单

- [ ] Mob基类能正确继承Actor
- [ ] 5种怪物能正确生成和显示
- [ ] 战斗计算系统正确计算伤害
- [ ] AI能自主决策和行动
- [ ] TurnManager能正确管理战斗流程
- [ ] 战斗UI能实时更新
- [ ] 所有怪物都能被击败
- [ ] 代码编译无错误
- [ ] 编辑器工具完整
- [ ] 文档更新完成

---

**预计耗时**: 5-7天 | **目标完成**: 2025年10月27日
