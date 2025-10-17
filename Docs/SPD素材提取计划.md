# SPD素材提取计划

## 📂 SPD素材位置

**项目路径**: `D:\Program Files\Unity\Hub\Project\shattered-pixel-dungeon`  
**素材根目录**: `core/src/main/assets/`

---

## 🎨 核心素材清单

### 1. 地形Tiles（优先级最高）⭐⭐⭐

**位置**: `environment/`

#### 基础地形
- `tiles_sewers.png` - 下水道地形（1-5层）
- `tiles_prison.png` - 监狱地形（6-10层）
- `tiles_caves.png` - 洞穴地形（11-15层）
- `tiles_city.png` - 城市地形（16-20层）
- `tiles_halls.png` - 大厅地形（21-25层）

#### 特殊地形
- `terrain_features.png` - 地形特征（门、陷阱、草等）
- `visual_grid.png` - 视觉网格
- `wall_blocking.png` - 墙壁遮挡

#### 水动画
- `water0.png` ~ `water4.png` - 水面动画（5帧）

#### 自定义地形
- `custom_tiles/caves_boss.png` - Boss房地形
- `custom_tiles/caves_quest.png` - 任务房地形
- `custom_tiles/city_boss.png`
- `custom_tiles/halls_special.png`
- `custom_tiles/prison_exit.png`
- `custom_tiles/prison_quest.png`
- `custom_tiles/sewer_boss.png`
- `custom_tiles/weak_floor.png` - 脆弱地板

---

### 2. 角色Sprites（优先级高）⭐⭐

**位置**: `sprites/`

#### 玩家职业
- `warrior.png` - 战士
- `mage.png` - 法师
- `rogue.png` - 盗贼
- `huntress.png` - 女猎手
- `duelist.png` - 决斗者

#### 基础怪物（前10层）
- `rat.png` - 老鼠（1-3层）
- `snake.png` - 蛇（1-5层）
- `gnoll.png` - 豺狼人（2-5层）
- `crab.png` - 螃蟹（3-5层）
- `swarm.png` - 虫群（4-5层）
- `goo.png` - 史莱姆Boss（5层）
- `skeleton.png` - 骷髅（6-9层）
- `thief.png` - 小偷（6-10层）
- `guard.png` - 守卫（7-10层）
- `necromancer.png` - 亡灵法师（8-10层）
- `bat.png` - 蝙蝠（6-10层）

#### 中级怪物（11-20层）
- `spinner.png` - 蜘蛛
- `dm100.png` - DM-100
- `elemental.png` - 元素
- `warlock.png` - 术士
- `monk.png` - 僧侣
- `golem.png` - 魔像

#### Boss怪物
- `goo.png` - 下水道Boss
- `tengu.png` - 监狱Boss
- `dm300.png` - 洞穴Boss
- `king.png` - 城市Boss
- `yog.png` + `yog_fists.png` - 最终Boss

#### NPC
- `shopkeeper.png` - 商人
- `blacksmith.png` - 铁匠
- `wandmaker.png` - 魔杖师
- `ratking.png` - 鼠王
- `ghost.png` - 幽灵

---

### 3. 物品Icons（优先级高）⭐⭐

**位置**: `sprites/`

- `items.png` - 主要物品图集
- `item_icons.png` - 物品图标

---

### 4. UI和特效（优先级中）⭐

**位置**: 
- `interfaces/` - UI元素
- `effects/` - 特效
- `fonts/` - 字体

---

## 🚀 实施步骤

### 阶段1：基础地形导入（今天完成）

1. **复制地形文件**
   ```
   源: shattered-pixel-dungeon/core/src/main/assets/environment/
   目标: DungeonRoguelike/Assets/_Project/Art/Tiles/Environment/
   ```

2. **需要复制的文件**:
   - tiles_sewers.png
   - tiles_prison.png
   - tiles_caves.png
   - tiles_city.png
   - tiles_halls.png
   - terrain_features.png
   - water0-4.png (5个文件)

3. **Unity导入设置**:
   - Texture Type: Sprite (2D and UI)
   - Sprite Mode: Multiple
   - Pixels Per Unit: 16 (SPD是16x16像素)
   - Filter Mode: Point (no filter)
   - Compression: None
   - Max Size: 4096 (或更高)

4. **切割Sprite**:
   - 使用Sprite Editor
   - Grid By Cell Size: 16x16
   - 自动切割所有Tile

5. **创建Tile Assets**:
   - Create > 2D > Tiles > Tile
   - 为每个地形类型创建对应的Tile
   - 分配到LevelRenderer

---

### 阶段2：角色Sprite导入（Week 5）

1. 先导入玩家职业sprite
2. 导入前10层基础怪物
3. 配置动画Animator

---

### 阶段3：物品和UI导入（Week 7-8）

1. 导入items.png和item_icons.png
2. 切割为单个物品sprite
3. 创建物品预制体

---

## 📐 SPD技术规格

### 像素规格
- **基础Tile大小**: 16x16 像素
- **角色Sprite**: 通常12x15或16x16
- **物品Icon**: 16x16
- **大型怪物**: 可能32x32或更大

### 图集规格
- **tiles_sewers.png**: 256x224 (16x14 tiles)
- **tiles_prison.png**: 256x224
- **tiles_caves.png**: 256x224
- **tiles_city.png**: 256x224
- **tiles_halls.png**: 256x224
- **terrain_features.png**: 128x128 (8x8 tiles)

### 地形Tile索引（参考）
SPD使用索引系统，每个Tile在图集中的位置：
```
0  = Floor
1  = Empty
2  = Grass
3  = Empty Deco
4  = Wall
5  = Door
...
```

---

## 🎮 Unity配置

### Tilemap Palette创建
1. **SewersTilePalette** - 使用tiles_sewers.png
2. **PrisonTilePalette** - 使用tiles_prison.png
3. **CavesTilePalette** - 使用tiles_caves.png
4. **CityTilePalette** - 使用tiles_city.png
5. **HallsTilePalette** - 使用tiles_halls.png
6. **FeaturesTilePalette** - 使用terrain_features.png

### LevelRenderer Tile映射
```csharp
Terrain.Floor -> SewerFloorTile
Terrain.Wall -> SewerWallTile
Terrain.Water -> WaterTile (animated)
Terrain.Grass -> GrassTile
Terrain.DoorOpen -> DoorTile
...
```

---

## ⚠️ 注意事项

### 版权和许可
- SPD是GPL-3.0开源项目
- 你的项目必须遵守GPL-3.0许可
- 建议在你的README中注明素材来源

### 质量保证
- 所有Tile必须使用Point Filter（像素完美）
- 不要压缩PNG（保持原始质量）
- 保持16x16的像素网格对齐

### 性能优化
- 使用Sprite Atlas打包
- 启用Mipmap可能会模糊像素艺术，应关闭
- 使用适当的Sorting Layer

---

## 📊 进度追踪

- [ ] 阶段1: 基础地形Tile导入
  - [ ] 复制PNG文件
  - [ ] Unity导入设置
  - [ ] Sprite切割
  - [ ] 创建Tile Assets
  - [ ] 配置LevelRenderer
  - [ ] 测试渲染

- [ ] 阶段2: 角色Sprite导入（Week 5）
  - [ ] 玩家职业
  - [ ] 基础怪物

- [ ] 阶段3: 物品导入（Week 7-8）
  - [ ] 物品图集
  - [ ] 物品Icon

- [ ] 阶段4: UI和特效（Week 9+）
  - [ ] UI元素
  - [ ] 粒子特效

---

**创建时间**: 2025-10-17  
**负责人**: AI Assistant  
**项目**: DungeonRoguelike - Unity移植  
**参考**: Shattered Pixel Dungeon v2.5.4

🎨 准备开始提取素材，让地牢可视化！

