# Week 5 玩家系统 - 测试指南

## 🎯 测试目标

验证玩家系统的核心功能：
- ✅ 战士职业生成
- ✅ 点击移动
- ✅ 自动寻路
- ✅ 相机跟随
- ✅ HUD显示

---

## 📋 准备工作

### 1. 场景配置（必须）

#### A. Boot场景
1. 打开 `Boot.unity`
2. GameSystems已有3个Manager ✅
3. 添加SceneLoader组件 ⏳

#### B. Game场景
1. 打开 `Game.unity`
2. 确认以下GameObject存在：
   - ✅ Main Camera
   - ✅ Grid (3个Tilemap)
   - ✅ LevelRenderer
   - ✅ Canvas + EventSystem

3. **添加新组件**：

**Main Camera配置：**
- Add Component → `CameraController`
- 配置：
  - ✅ Auto Find Target
  - ✅ Auto Configure Bounds
  - Smooth Speed: `5`
  - Enable Bounds: ✅

**创建InputManager：**
- 创建空GameObject：`InputManager`
- Add Component → `InputManager`
- 配置：
  - Enable Debug Log: ✅（测试时）

**GameController配置：**
- 确认有 `GameInitializer` 组件
- 配置：
  - ✅ Auto Start Game
  - Starting Depth: `1`
  - Default Hero Class: `Warrior`
  - ✅ Enable Debug Log

---

## 🧪 测试步骤

### 测试1: 启动流程 ✅

**步骤：**
1. 打开Boot场景
2. 点击Play ▶️
3. 观察控制台输出

**预期结果：**
```
✅ [GameManager] Initialized
✅ [LevelManager] Initialized
✅ [TurnManager] Initialized
✅ [SceneLoader] Loading scene: MainMenu
```

---

### 测试2: 地牢生成 ✅

**步骤：**
1. 从MainMenu点击"New Game"（或直接运行Game场景）
2. 观察Scene视图和Game视图

**预期结果：**
```
✅ [GameInitializer] === 游戏初始化开始 ===
✅ [LevelManager] Generating level 1
✅ Generating RegularLevel: Depth 1
✅ Builder generated 6-12 rooms
✅ [GameInitializer] === 游戏初始化完成 ===
```

**视觉检查：**
- ✅ 地牢地图显示（如果有Tile资源）
- ✅ 房间和走廊清晰可见

---

### 测试3: Hero生成 ✅

**预期结果：**
```
✅ Warrior initialized: HP=25/25, STR=12
✅ 英雄初始化完成：Warrior at (x, y)
✅ HeroMovement: Subscribed to input events
✅ CameraController: Found hero at (x, y, 0)
```

**视觉检查：**
- ✅ Hero GameObject出现在入口位置
- ✅ 相机立即跳转到Hero位置

---

### 测试4: 点击移动 🎯

**步骤：**
1. 在Game视图中点击地板
2. 观察Hero是否移动

**预期结果：**
```
✅ Grid clicked: (x, y) -> pos xxx
✅ A* pathfinding successful
✅ Moved to xxx (x, y)
```

**视觉检查：**
- ✅ Hero平滑移动到目标位置
- ✅ 相机跟随Hero移动
- ✅ 移动路径合理（避开墙壁）

**测试用例：**
- [ ] 点击相邻格子 → 直接移动
- [ ] 点击远处格子 → 自动寻路
- [ ] 点击墙壁 → 移动到最近可达点
- [ ] 点击自己 → 等待（暂无反应）

---

### 测试5: 寻路系统 🎯

**步骤：**
1. 点击需要绕路的目标
2. 观察Scene视图的路径绘制

**预期结果：**
- ✅ 青色线条显示路径
- ✅ 路径避开所有墙壁
- ✅ 路径是最短路径
- ✅ 到达目标后路径消失

**测试用例：**
- [ ] 简单直线路径
- [ ] 需要转弯的路径
- [ ] 需要绕过房间的路径
- [ ] 走廊中的路径

---

### 测试6: 相机跟随 ✅

**步骤：**
1. 让Hero移动到地图各处
2. 观察相机行为

**预期结果：**
- ✅ 相机平滑跟随Hero
- ✅ 相机不超出地图边界
- ✅ 相机保持Hero在视野中心

**测试用例：**
- [ ] 移动到地图中心 → 正常跟随
- [ ] 移动到地图边缘 → 停在边界
- [ ] 快速移动 → 平滑插值
- [ ] 像素对齐 → 无模糊

---

### 测试7: HUD显示 ⏳

**步骤：**
1. 观察Canvas上的HUD元素
2. 让Hero受伤/治疗（暂无法测试）

**预期结果：**
- ✅ HP显示正确（25/25）
- ✅ Depth显示正确（1）
- ✅ 职业显示正确（Warrior）
- ⏳ HP条颜色正确（绿色）

**注：** HUD需要手动配置UI元素才能显示

---

## 🐛 已知问题

### 1. 缺少Tile资源
**现象：** 地牢不可见
**解决：** Week 6导入SPD精灵图

### 2. Hero无精灵图
**现象：** Hero是白色方块
**解决：** Week 6导入Hero精灵

### 3. HUD UI未创建
**现象：** 看不到HP条
**解决：** 手动在Canvas下创建UI

### 4. 回合制未完整实现
**现象：** 移动不消耗回合
**解决：** Week 6完善TurnManager

---

## ✅ 成功标准

### 基础功能（必须通过）
- [x] 启动无错误
- [x] 地牢正常生成
- [x] Hero正确初始化
- [x] 点击移动正常
- [x] 寻路算法正确
- [x] 相机跟随流畅

### 可选功能（Week 6+）
- [ ] 动画播放
- [ ] HUD完整显示
- [ ] 回合制完整
- [ ] 精灵图显示

---

## 📊 性能测试

### FPS测试
**步骤：**
1. Stats窗口（右上角）
2. 连续点击移动
3. 观察FPS

**预期：**
- PC: 60 FPS+
- 地图32x32无卡顿

### 内存测试
**预期：**
- 初始内存：<100MB
- 移动中内存：稳定

---

## 🔧 调试技巧

### 1. 启用调试日志
```csharp
// InputManager
Enable Debug Log: ✅

// HeroMovement  
Enable Debug Log: ✅

// GameInitializer
Enable Debug Log: ✅
```

### 2. Scene视图调试
- 选中Hero → 看到路径绘制（青色线）
- 选中Camera → 看到边界框（黄色）
- Gizmos按钮 → 显示/隐藏调试信息

### 3. 控制台过滤
```
搜索 "Hero" → 只看Hero相关日志
搜索 "Error" → 快速定位错误
```

---

## 📞 问题排查

### Q: Hero不移动？
**检查：**
1. InputManager是否存在？
2. HeroMovement是否添加？
3. Level是否生成成功？
4. 控制台是否有错误？

### Q: 路径绘制不显示？
**解决：**
- Scene视图，选中Hero
- HeroMovement组件，Draw Path = ✅

### Q: 相机不跟随？
**检查：**
1. CameraController是否添加？
2. Auto Find Target = ✅
3. Hero是否正确创建？

### Q: 找不到Level？
**解决：**
- 确保从Boot场景启动
- 或Game场景中有GameInitializer

---

## 🚀 下一步

测试通过后，准备进入：
- **Week 6**: 战斗系统
  - 5种基础怪物
  - 战斗计算
  - 简单AI

---

**测试文档版本**: v1.0  
**创建日期**: 2025-10-16  
**适用版本**: Week 5 Day 5

