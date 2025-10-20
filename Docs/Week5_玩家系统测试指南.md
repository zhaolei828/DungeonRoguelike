# Week 5 - 玩家系统测试指南

## 已实现功能

### 1. Hero类（英雄）
- ✅ 基础属性系统（HP、Mana、Level、Experience）
- ✅ 5种职业（Warrior、Mage、Rogue、Huntress、Cleric）
- ✅ 职业特定属性初始化
- ✅ 经验系统和升级机制
- ✅ 移动逻辑

### 2. Actor基类
- ✅ 所有角色的基类
- ✅ 通用属性（Position、HP）
- ✅ 抽象方法（Act、TakeDamage、Die）

### 3. HeroSpawner（英雄生成器）
- ✅ 在地牢入口生成Hero
- ✅ 临时视觉表示（黄色方块）
- ✅ 自动初始化Hero属性

### 4. PlayerInput（玩家输入）
- ✅ WASD / 方向键控制移动
- ✅ 碰撞检测（不可通行区域阻挡）
- ✅ 实时Transform位置更新

### 5. CameraFollow（相机跟随）
- ✅ 平滑跟随Hero
- ✅ 可调节跟随速度
- ✅ 自动初始化

## 测试步骤

### 步骤1：打开Game场景
1. 在Unity中打开 `Assets/_Project/Scenes/Game.unity/Game.unity`
2. 确保场景中有以下GameObject：
   - `GameController`（带有GameInitializer组件）
   - `GameSystems`（带有LevelManager、GameManager组件）
   - `Main Camera`
   - `Grid` -> `GroundTilemap`, `WallTilemap`, `DecorationTilemap`
   - `LevelRenderer`（带有LevelRenderer组件）

### 步骤2：进入Play模式
1. 点击Unity顶部的 **Play** 按钮
2. 观察Console输出，应该看到：
   ```
   === Game Scene Initialized ===
   === Level 1 Generated ===
   Hero sprite created (temporary yellow square)
   Hero spawned at entrance: (x, y)
   === Hero Spawned: Warrior ===
   Hero initialized: Warrior Level 1
   CameraFollow: Target set to Hero
   === Camera Follow Enabled ===
   ```

### 步骤3：测试Hero移动
1. 使用 **WASD** 或 **方向键** 控制Hero移动
2. 观察：
   - ✅ Hero（黄色方块）应该在地图上移动
   - ✅ 相机应该平滑跟随Hero
   - ✅ Hero无法穿过墙壁（灰色Tile）
   - ✅ Hero只能在地板（棕色Tile）上移动
   - ✅ Console会输出 `Hero moved to (x, y)`

### 步骤4：验证职业属性
1. 在Hierarchy中选择 `Hero_Warrior` GameObject
2. 在Inspector中查看Hero组件：
   - `Hero Class`: Warrior
   - `Level`: 1
   - `Experience`: 0
   - `Mana`: 100
   - `Max Mana`: 100
   - `Armor`: 5
   - `Strength`: 15
   - `Intelligence`: 10
   - `Willpower`: 10
   - `Hp`: 30
   - `Max Hp`: 30

### 步骤5：测试相机跟随
1. 移动Hero到地图边缘
2. 观察相机是否平滑跟随
3. 可以在 `Main Camera` 的 `CameraFollow` 组件中调整 `Follow Speed`（0-1之间）

## 已知限制（临时）

1. **Hero视觉表示**：目前使用临时的黄色方块，后续会替换为SPD原版像素艺术
2. **输入系统**：目前只支持键盘，后续会添加触屏支持（虚拟摇杆）
3. **动画系统**：目前没有移动动画，后续会添加
4. **战斗系统**：尚未实现，Hero无法攻击或受到伤害
5. **物品系统**：尚未实现，Hero无法拾取或使用物品

## 下一步计划

根据Week 5规划，接下来将实现：
1. **Hero视觉资源**：从SPD提取Hero的Sprite并集成
2. **移动动画**：添加Hero的移动动画
3. **UI系统**：显示Hero的HP、Mana、Level等信息
4. **触屏输入**：添加虚拟摇杆支持

## 故障排除

### 问题1：Hero没有生成
**症状**：Play模式下看不到黄色方块
**解决方案**：
1. 检查Console是否有错误
2. 确认 `GameInitializer` 组件已添加到 `GameController`
3. 确认 `LevelManager` 和 `GameManager` 已添加到 `GameSystems`

### 问题2：Hero无法移动
**症状**：按WASD/方向键没有反应
**解决方案**：
1. 检查Console是否有 "PlayerInput: Hero not found" 错误
2. 确认 `PlayerInput` GameObject已在运行时创建
3. 检查 `Hero.pos` 是否正确初始化

### 问题3：相机不跟随
**症状**：移动Hero时相机不动
**解决方案**：
1. 检查 `Main Camera` 是否有 `CameraFollow` 组件
2. 确认 `CameraFollow.target` 已设置为Hero的Transform
3. 调整 `followSpeed` 参数（默认0.1）

### 问题4：Hero穿墙
**症状**：Hero可以走进墙壁
**解决方案**：
1. 检查 `Level.IsPassable()` 方法是否正确实现
2. 确认 `TerrainProperties.IsPassable()` 对 `Terrain.Wall` 返回 `false`
3. 检查地牢生成是否正确（墙壁是否正确标记为 `Terrain.Wall`）

## 性能指标

- **FPS**: 应该稳定在60 FPS（PC）
- **内存占用**: < 200 MB（当前阶段）
- **输入延迟**: < 16ms（1帧）
- **相机跟随延迟**: 可调节（默认约100ms）

## 测试完成标准

- ✅ Hero在地牢入口正确生成
- ✅ Hero可以使用WASD/方向键移动
- ✅ Hero无法穿过墙壁
- ✅ 相机平滑跟随Hero
- ✅ Console无错误或警告
- ✅ Hero属性在Inspector中正确显示
- ✅ 移动流畅，无卡顿

---

**最后更新**: 2025-10-20
**测试版本**: Week 5 - Player System v1.0

