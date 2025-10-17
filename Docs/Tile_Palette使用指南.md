# Tile Palette 创建和使用指南

## 🎯 什么是Tile Palette？

Tile Palette（瓦片调色板）是Unity的2D工具，就像Photoshop的画笔面板一样。你可以：
- 选择不同的Tile（地板、墙壁、装饰等）
- 在场景中"绘制"地牢布局
- 快速设计关卡

**但是**：我们的项目是**程序生成**地牢，所以Tile Palette主要用于：
1. **可视化所有Tile** - 查看有哪些Tile可用
2. **手动设计特殊房间** - Boss房、商店等
3. **测试Tile效果** - 看看Tile渲染是否正确

---

## 🚀 自动创建Tile Palette（最简单！）

### 步骤1: 先切割Sprite

在Unity顶部菜单：
```
DungeonRoguelike > SPD Tools > Auto Slice Sprites
```

点击 **"切割所有地形Tile (16x16)"** 按钮，等待完成。

---

### 步骤2: 创建Tile Palette

在Unity顶部菜单：
```
DungeonRoguelike > SPD Tools > Create Tile Palettes
```

点击 **"🎨 创建所有Tile Palette"** 按钮。

会自动创建6个Palette：
- ✅ SewersTilePalette
- ✅ PrisonTilePalette
- ✅ CavesTilePalette
- ✅ CityTilePalette
- ✅ HallsTilePalette
- ✅ FeaturesTilePalette

完成后会弹出提示框！

---

### 步骤3: 查看创建的Palette

在Project窗口导航到：
```
Assets/_Project/Art/Tiles/Palettes/
```

你会看到所有创建好的Palette prefab文件。

---

## 🎨 如何使用Tile Palette（可选）

如果你想手动绘制地图或测试Tile效果：

### 打开Tile Palette窗口

Unity顶部菜单：
```
Window > 2D > Tile Palette
```

### 选择一个Palette

在Tile Palette窗口顶部的下拉菜单中选择，例如：`SewersTilePalette`

### 绘制工具

Tile Palette窗口有几个工具按钮：

| 图标 | 名称 | 功能 | 快捷键 |
|------|------|------|--------|
| 🖊️ | Paint | 绘制Tile | B |
| 🧹 | Erase | 擦除Tile | D |
| 🪣 | Fill | 填充区域 | G |
| 👁️ | Pick | 选择已有Tile | I |
| 📦 | Box | 矩形选择绘制 | U |

### 绘制步骤

1. 在Hierarchy中选择你的Tilemap（如`GroundTilemap`）
2. 在Tile Palette中选择一个Tile
3. 在Scene视图中点击即可绘制

---

## 🔧 配置LevelRenderer使用Tile

现在最重要的是让`LevelRenderer`使用这些Tile来渲染程序生成的地牢！

### 步骤1: 找到Tile Assets

导航到：
```
Assets/_Project/Art/Tiles/TileAssets/SewersTilePalette/
或
Assets/_Project/Art/Tiles/TileAssets/FeaturesTilePalette/
```

这里有数百个Tile asset文件。

### 步骤2: 识别需要的Tile

你需要找到对应的Tile：

#### 常用Tile（大致位置）：

**从 SewersTilePalette 文件夹：**
- **Floor Tile** - 找名字类似 `tiles_sewers_0_0` 或 `tiles_sewers_0_1` 的（第一行的Tile通常是地板）
- **Wall Tile** - 找名字类似 `tiles_sewers_4_0` 的（墙壁Tile，通常是深色的）

**从 FeaturesTilePalette 文件夹：**
- **Grass Tile** - 找名字包含草图案的Tile
- **Door Tile** - 找名字包含门图案的Tile
- **Entrance Tile** - 找名字包含入口/楼梯向上的Tile
- **Exit Tile** - 找名字包含出口/楼梯向下的Tile

**从 Water 文件夹：**
- **Water Tile** - 使用 `water0.png` 的Sprite（或创建Animated Tile）

### 步骤3: 配置LevelRenderer

1. 在Hierarchy中找到或创建 `LevelRenderer` GameObject
2. 如果没有LevelRenderer组件，添加它
3. 在Inspector中展开 **"Tile资源"** 部分
4. 将找到的Tile拖拽到对应字段：

```
Floor Tile → 地板Tile
Wall Tile → 墙壁Tile
Water Tile → 水Tile
Grass Tile → 草Tile
High Grass Tile → 高草Tile
Entrance Tile → 入口Tile
Exit Tile → 出口Tile
Door Tile → 门Tile
Trap Tile → 陷阱Tile
Wall Deco Tile → 装饰Tile
```

---

## 🎯 快速测试方法

如果你不想慢慢找Tile，可以**随便拖几个**先测试！

1. 从 `SewersTilePalette` 文件夹随便选一个Tile
2. 拖到 `Floor Tile` 字段
3. 再选一个不同的Tile，拖到 `Wall Tile` 字段
4. Play测试，按G键

至少你能看到地牢有**不同颜色的地板和墙壁**了！

---

## 🖼️ 可视化参考

### Tile命名规则

SPD的Tile按网格位置命名：
```
tiles_sewers_Y_X.asset

Y = 行号（从上到下，从0开始）
X = 列号（从左到右，从0开始）
```

例如：
- `tiles_sewers_0_0` = 第1行第1列
- `tiles_sewers_0_4` = 第1行第5列
- `tiles_sewers_4_0` = 第5行第1列

### 推荐的Tile索引（参考SPD源码）

以下是SPD源码中常用的Tile索引（行_列）：

#### tiles_sewers.png
```
0_0 ~ 0_15: 第1行 - 基础地板
4_0 ~ 4_15: 第5行 - 墙壁（重要！）
2_0 ~ 2_15: 第3行 - 草地和水
```

#### terrain_features.png
```
0_5: 开门
0_6: 关门
1_0: 陷阱
3_2: 入口楼梯
3_3: 出口楼梯
```

**注意**：这只是参考！最准确的方法是：
1. 在Unity中双击PNG文件
2. 在Sprite Editor中查看每个Sprite的图案
3. 找到你想要的图案

---

## 🆘 常见问题

### Q: 为什么我看不到"DungeonRoguelike"菜单？
**A**: 等待Unity完成脚本编译。查看Unity底部状态栏，等待编译完成。

### Q: 创建Palette时报错"贴图未切割"
**A**: 先运行 `SPD Tools > Auto Slice Sprites`，等待完成后再创建Palette。

### Q: 我不知道哪个Tile是什么
**A**: 
1. 在Project窗口选择一个Tile asset
2. 看Inspector面板的预览图
3. 或者双击PNG文件，在Sprite Editor中查看

### Q: 配置好了但按G键还是看不到地图
**A**: 检查：
1. LevelRenderer的Tilemap字段是否已分配（GroundTilemap/WallTilemap等）
2. Tile字段是否至少分配了Floor和Wall
3. Console是否有错误

### Q: 地图显示但都是同一个颜色
**A**: 你可能把同一个Tile分配给了Floor和Wall，重新分配不同的Tile。

---

## 📚 进阶：创建水动画Tile

如果你想让水有动画效果：

1. 右键 `Project > Create > 2D > Tiles > Animated Tile`
2. 命名为 `WaterAnimatedTile`
3. 选择它，在Inspector中：
   - **Number of Animated Sprites**: 5
   - **Minimum Speed**: 0.5
   - **Maximum Speed**: 0.5
4. 展开 **Animated Sprites**，依次拖入：
   - Element 0: `water0` Sprite
   - Element 1: `water1` Sprite
   - Element 2: `water2` Sprite
   - Element 3: `water3` Sprite
   - Element 4: `water4` Sprite
5. 将这个Animated Tile拖到LevelRenderer的 `Water Tile` 字段

现在水会有流动动画了！🌊

---

## ✅ 总结

**必须做的**：
1. ✅ 运行 `Auto Slice Sprites` 切割贴图
2. ✅ 运行 `Create Tile Palettes` 创建Palette
3. ✅ 配置LevelRenderer的Tile字段（至少Floor和Wall）
4. ✅ Play测试，按G键

**可选的**：
- 使用Tile Palette手动绘制地图
- 创建水动画Tile
- 仔细找准确的Tile索引

---

**创建时间**: 2025-10-17  
**难度**: ⭐⭐ (自动化工具让它变简单了！)

🎨 按这个指南操作，你的地牢很快就能显示真实的SPD像素艺术了！

