# Week 5: 玩家系统实现规划

## 📋 目标概述

**核心目标**：实现可在地牢中移动的Warrior职业角色，完善回合制逻辑

**交付标准**：
- ✅ Warrior职业角色可视化渲染
- ✅ 基于格子的移动系统（点击移动）
- ✅ 回合制逻辑（玩家→敌人→环境）
- ✅ 基础动画（行走、待机）
- ✅ 相机跟随玩家
- ✅ 基础UI（HP显示）

---

## 🎯 实施计划（5天）

### Day 1: 角色类结构设计（4-5小时）
- ✅ Char.cs - 角色基类
- Hero.cs - 完善玩家类
- Warrior.cs - 战士职业

### Day 2: 移动系统实现（4-5小时）
- InputManager.cs - 输入管理
- PathFinder.cs - A*寻路
- 移动动画系统

### Day 3: 回合制系统完善（3-4小时）
- 完善TurnManager
- 实现Hero.Act()
- 输入阻塞机制

### Day 4: 视觉和相机（3-4小时）
- Pixel Perfect设置
- CameraController.cs
- 精灵图配置

### Day 5: 基础UI和集成（3-4小时）
- HeroHUD.cs
- 完整流程测试
- 优化调试

---

## 📦 交付清单

### 代码文件（约12个）
1. Char.cs - 角色基类（~500行）✅
2. Hero.cs - 完善版（~300行新增）
3. Warrior.cs - 战士职业（~150行）
4. InputManager.cs（~200行）
5. PathFinder.cs（~300行）
6. HeroAnimator.cs（~150行）
7. CameraController.cs（~100行）
8. HeroHUD.cs（~150行）

### 资源文件
- hero.png - 英雄精灵图
- Hero_Animator.controller
- Hero_Idle_*.anim
- Hero_Walk_*.anim

---

## ✅ 验收标准

1. **基础移动**：
   - ✅ 点击地板，玩家移动
   - ✅ 自动寻路
   - ✅ 移动动画
   
2. **回合制**：
   - ✅ 移动消耗回合
   - ✅ TurnManager调度
   
3. **视觉效果**：
   - ✅ 相机跟随
   - ✅ 像素完美
   
4. **UI显示**：
   - ✅ HP条显示
   - ✅ 层数显示

---

**预计时间**：17-22小时（5个工作日）  
**预计完成**：2025-10-21

