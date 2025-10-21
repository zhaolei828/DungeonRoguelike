# SPD资源和设计借鉴指南

> 从Shattered Pixel Dungeon项目中学习和借鉴

---

## 📦 可用资源清单

### 1. Sprites（精灵图）
**位置**: `core/src/main/assets/sprites/`
**文件数**: 75个PNG文件

**已导入**:
- ✅ 怪物sprite（老鼠、蝙蝠、地精等）
- ✅ 角色sprite（warrior.png）

**待导入**:
- ⚠️ 物品sprite（items.png - 24KB，包含所有物品）
- ⚠️ UI图标（item_icons.png）
- ⚠️ 其他角色（mage, rogue, huntress等）
- ⚠️ Boss sprite（goo, tengu, king等）
- ⚠️ 环境元素（各种装饰）

### 2. Effects（特效）
**位置**: `core/src/main/assets/effects/`
**文件数**: 6个PNG文件

**包含**:
- 粒子效果
- 法术特效
- 爆炸效果

### 3. Environment（环境）
**位置**: `core/src/main/assets/environment/`
**文件数**: 23个PNG文件

**包含**:
- tiles_sewers.png（下水道地砖）
- tiles_prison.png（监狱地砖）
- tiles_caves.png（洞穴地砖）
- tiles_city.png（城市地砖）
- tiles_halls.png（大厅地砖）
- terrain_features.png（地形特征）
- water0-4.png（水动画帧）

### 4. UI素材
**位置**: `core/src/main/assets/interfaces/`
**文件数**: 20个PNG文件

**包含**:
- 按钮
- 边框
- 背景
- 图标

### 5. 音效
**位置**: `core/src/main/assets/sounds/`
**文件数**: 67个MP3文件

**包含**:
- 攻击音效
- 脚步声
- 魔法音效
- UI音效

### 6. 音乐
**位置**: `core/src/main/assets/music/`
**文件数**: 31个MP3文件

---

## 🎨 设计借鉴要点

### 1. 美术风格
- **像素艺术**: 16x16基础尺寸
- **颜色风格**: 暗色调 + 高对比度
- **动画**: 简单但流畅（4-8帧）

### 2. UI设计
```
SPD的UI特点:
- 简洁明了
- 信息密度高
- 易于操作
- 深色主题

可借鉴:
✅ 物品栏布局
✅ 快捷栏设计
✅ 状态图标
✅ 消息日志
```

### 3. 游戏机制

#### 回合制系统
```java
// SPD的回合系统
- 玩家行动 → 处理 → 怪物行动 → 处理
- 行动有优先级
- 饥饿系统每回合消耗
```

#### 战斗公式
```java
// 伤害计算（简化版）
damage = attacker.damageRoll() - defender.drAdjusted()

// 命中率
accuracy = attacker.accuracy() - defender.evasion()

// 暴击
critical = Random.Float() < critChance
```

#### 物品系统
```java
// 物品分类
- 武器（Weapon）
- 护甲（Armor）
- 药水（Potion）
- 卷轴（Scroll）
- 戒指（Ring）
- 神器（Artifact）
- 食物（Food）
```

---

## 🛠️ 导入工具使用

### 方法1: 使用SPD Resource Importer
```
1. Unity菜单 → Tools → DungeonRoguelike → SPD Resource Importer
2. 设置SPD项目路径
3. 选择要导入的资源类型
4. 点击"开始导入"
```

### 方法2: 使用一键场景设置
```
1. 打开Game.unity场景
2. Unity菜单 → Tools → DungeonRoguelike → 一键设置游戏场景
3. 或按快捷键 Ctrl+Alt+S
4. 点击"设置当前场景"
```

---

## 📋 资源映射表

### 怪物Sprite映射
| Unity类名 | SPD文件名 | 说明 |
|-----------|-----------|------|
| Rat       | rat.png   | 老鼠 |
| Bat       | bat.png   | 蝙蝠 |
| Spider    | spinner.png | 蜘蛛（SPD中叫spinner） |
| Goblin    | gnoll.png | 地精（SPD中叫gnoll） |
| Orc       | brute.png | 兽人（SPD中叫brute） |
| Skeleton  | skeleton.png | 骷髅 |
| Ghost     | ghost.png | 幽灵 |
| Golem     | golem.png | 石头人 |

### 地形Sprite映射
| 地形类型 | SPD文件名 | 大小 |
|---------|-----------|------|
| 下水道  | tiles_sewers.png | 512x512 |
| 监狱    | tiles_prison.png | 512x512 |
| 洞穴    | tiles_caves.png | 512x512 |
| 城市    | tiles_city.png | 512x512 |
| 大厅    | tiles_halls.png | 512x512 |

### 物品Sprite
**items.png** (24KB)
- 包含所有物品的sprite
- 需要切片导入
- 每个物品16x16像素

---

## 🎯 推荐导入顺序

### 第一阶段（已完成）✅
1. ✅ 基础怪物sprite
2. ✅ 角色sprite（warrior）

### 第二阶段（推荐）
3. ⚠️ 物品sprite（items.png）
4. ⚠️ 地形tile（tiles_*.png）
5. ⚠️ UI素材

### 第三阶段（可选）
6. ⚠️ 特效sprite
7. ⚠️ 音效文件
8. ⚠️ 其他角色和Boss

---

## 💡 使用建议

### 开发阶段
```
优先级：
1. 游戏机制 > 美术资源
2. 核心玩法 > 视觉效果
3. 可玩性 > 完整度

当前状态：
- 已有基础怪物和角色
- 已有地图生成
- 已有战斗系统
→ 建议现在导入物品和地形
```

### 资源优化
```
导入时注意：
1. Texture Type: Sprite (2D and UI)
2. Pixels Per Unit: 16（SPD标准）
3. Filter Mode: Point (no filter)
4. Compression: None（保持像素风格）
5. Max Size: 根据需要设置
```

---

## 📚 参考资料

### SPD源代码结构
```
shattered-pixel-dungeon/
├── core/src/main/java/com/shatteredpixel/shatteredpixeldungeon/
│   ├── actors/          # 角色系统
│   ├── items/           # 物品系统
│   ├── levels/          # 关卡生成
│   ├── mechanics/       # 游戏机制
│   └── ui/              # UI系统
└── core/src/main/assets/ # 资源文件
```

### 关键类参考
```java
// 角色基类
Char.java - 对应我们的Actor.cs

// 英雄
Hero.java - 对应我们的Hero.cs

// 怪物
Mob.java - 对应我们的Mob.cs

// 关卡
Level.java - 对应我们的Level.cs

// 物品
Item.java - 待实现
```

---

## ⚠️ 版权说明

SPD使用GPL-3.0 License：
- ✅ 可以使用资源
- ✅ 可以修改代码
- ⚠️ 如果商用，项目也需要开源
- ⚠️ 需要保留版权声明

**建议**:
- 学习阶段：自由使用
- 商业项目：考虑替换为原创资源

---

**最后更新**: 2025-10-21
**工具版本**: v1.0

