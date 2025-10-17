# SPD素材导入操作指南

## ✅ 已完成

1. ✅ **素材文件已复制**
   - 6个地形贴图已复制到 `Assets/_Project/Art/Tiles/Environment/`
   - 5个水动画帧已复制到 `Assets/_Project/Art/Tiles/Environment/Water/`
   
2. ✅ **自动导入脚本已创建**
   - `SPDTextureImporter.cs` - 自动配置导入设置
   - `SPDSpriteSlicerWindow.cs` - Sprite自动切割工具

---

## 🎯 现在需要你手动操作

### 步骤1: 验证素材导入设置

1. 打开Unity编辑器
2. 在Project窗口导航到: `Assets/_Project/Art/Tiles/Environment/`
3. 选择任意一个PNG文件（如`tiles_sewers.png`）
4. 查看Inspector面板，验证设置：
   ```
   Texture Type: Sprite (2D and UI)
   Sprite Mode: Multiple
   Pixels Per Unit: 16
   Filter Mode: Point (no filter)
   Compression: None
   Max Size: 4096
   Generate Mip Maps: 未勾选
   ```

如果设置不正确，`SPDTextureImporter`应该会自动修正它们。

---

### 步骤2: 运行Sprite切割工具

#### 方法1: 使用菜单（推荐）

1. 在Unity顶部菜单栏，找到 **`DungeonRoguelike`** 菜单
2. 选择 **`SPD Tools > Auto Slice Sprites`**
3. 在弹出的窗口中，点击 **"切割所有地形Tile (16x16)"** 按钮
4. 等待切割完成，会显示成功/失败数量

#### 方法2: 手动切割（如果菜单未出现）

对每个地形贴图执行以下操作：

1. 在Project中选择 `tiles_sewers.png`
2. 在Inspector中点击 **"Sprite Editor"** 按钮
3. 在Sprite Editor中：
   - 点击顶部的 **"Slice"** 下拉菜单
   - 选择 **"Grid By Cell Size"**
   - 设置 `Column & Row > C: 16, R: 16`
   - 点击 **"Slice"** 按钮
   - 点击 **"Apply"** 保存
4. 对以下文件重复此操作：
   - `tiles_prison.png`
   - `tiles_caves.png`
   - `tiles_city.png`
   - `tiles_halls.png`
   - `terrain_features.png`

---

### 步骤3: 创建Tile Palette（Unity 2D工具）

1. 在Project窗口右键，选择 **`Create > 2D > Tile Palette`**
2. 命名为 `SewersTilePalette`
3. 在Tile Palette窗口（`Window > 2D > Tile Palette`）：
   - 选择刚创建的Palette
   - 将 `tiles_sewers.png` 的所有子Sprite拖入Palette
   - Unity会自动创建Tile Assets

4. 重复创建更多Palette：
   - `PrisonTilePalette` - 使用 `tiles_prison.png`
   - `CavesTilePalette` - 使用 `tiles_caves.png`
   - `CityTilePalette` - 使用 `tiles_city.png`
   - `HallsTilePalette` - 使用 `tiles_halls.png`
   - `FeaturesTilePalette` - 使用 `terrain_features.png`

---

### 步骤4: 配置LevelRenderer

1. 在Hierarchy中找到或创建 `LevelRenderer` GameObject
2. 选择它，在Inspector中找到 `LevelRenderer` 组件
3. 展开 **"Tile资源"** 部分
4. 将创建好的Tile拖拽到对应的字段：

```
Floor Tile -> (从SewersTilePalette选择地板Tile)
Wall Tile -> (从SewersTilePalette选择墙Tile)
Water Tile -> (从Water文件夹选择water0.png的Sprite)
Grass Tile -> (从FeaturesTilePalette选择草Tile)
High Grass Tile -> (从FeaturesTilePalette选择高草Tile)
Entrance Tile -> (从FeaturesTilePalette选择入口Tile)
Exit Tile -> (从FeaturesTilePalette选择出口Tile)
Door Tile -> (从FeaturesTilePalette选择门Tile)
Trap Tile -> (从FeaturesTilePalette选择陷阱Tile)
Wall Deco Tile -> (从FeaturesTilePalette选择装饰Tile)
```

---

### 步骤5: 测试渲染

1. 打开 `TestDungeon` 场景
2. 点击Play按钮 ▶️
3. 按G键生成地牢
4. 现在应该能看到实际的地牢贴图了！

---

## 📐 SPD地形Tile索引参考

根据SPD源码，地形图集的布局：

### tiles_sewers.png (256x224 = 16x14 tiles)
```
行0-3: 基础地形（地板、墙、水、草等）
行4-7: 墙的变体和装饰
行8-11: 特殊地形
行12-13: 额外元素
```

### terrain_features.png (128x128 = 8x8 tiles)
```
包含：门、陷阱、书架、雕像、祭坛等
```

**常用Tile索引**（从左到右，从上到下）：
- **0**: Floor (地板)
- **1**: Empty (空)
- **2**: Grass (草)
- **4**: Wall (墙)
- **5**: Door (门)
- **16**: Water (水)
- **32**: Entrance (入口)
- **33**: Exit (出口)

具体索引需要在Sprite Editor中查看或通过SPD源码确认。

---

## 🎨 高级：创建Animated Tile（水动画）

1. 右键 **`Create > 2D > Tiles > Animated Tile`**
2. 命名为 `WaterAnimatedTile`
3. 在Inspector中：
   - **Number of Animated Sprites**: 5
   - **Animated Sprites**: 依次拖入 `water0` ~ `water4` 的Sprite
   - **Minimum Speed**: 0.5
   - **Maximum Speed**: 0.5
4. 在LevelRenderer中将 `Water Tile` 字段设置为这个Animated Tile

---

## ⚠️ 常见问题

### Q: Sprite看起来模糊
A: 确保 Filter Mode 设置为 `Point (no filter)`，并且 Compression 为 `None`

### Q: Tile大小不对
A: 检查 Pixels Per Unit 是否为 16

### Q: 切割后没有Sprite
A: 确保Sprite Mode为`Multiple`，然后重新运行Sprite Editor切割

### Q: Palette创建失败
A: 确保已安装 `2D Sprite` 和 `2D Tilemap Editor` 包（Package Manager）

---

## 📚 参考链接

- [SPD源码 - Level绘制](https://github.com/00-Evan/shattered-pixel-dungeon/blob/master/core/src/main/java/com/shatteredpixel/shatteredpixeldungeon/levels/Level.java)
- [Unity Tilemap文档](https://docs.unity3d.com/Manual/Tilemap.html)
- [Unity Sprite Editor](https://docs.unity3d.com/Manual/SpriteEditor.html)

---

**创建时间**: 2025-10-17  
**状态**: 🟢 准备就绪 - 请按步骤操作

🎨 完成这些步骤后，你的地牢就会有真实的SPD像素艺术了！

