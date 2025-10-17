# Build Settings 配置指南

## 📋 场景添加顺序

请按照以下步骤配置Build Settings：

### 1. 打开Build Settings
- 菜单栏：`File` → `Build Settings`
- 快捷键：`Ctrl + Shift + B`

### 2. 添加场景（按顺序）

将以下场景拖入"Scenes In Build"列表，**顺序非常重要**：

```
✅ 0. Boot           (Assets/_Project/Scenes/Boot.unity/Boot.unity)
✅ 1. MainMenu       (Assets/_Project/Scenes/MainMenu.unity/MainMenu.unity)
✅ 2. Game           (Assets/_Project/Scenes/Game.unity/Game.unity)
```

### 3. 验证配置

确认Build Settings中：
- ☑ **Boot** 的索引是 **0**（启动场景）
- ☑ **MainMenu** 的索引是 **1**
- ☑ **Game** 的索引是 **2**
- ☑ 所有场景都被勾选✅

### 4. 测试流程

1. **从Boot场景运行**：
   ```
   Boot → 初始化Managers → 自动跳转MainMenu
   ```

2. **点击"New Game"**：
   ```
   MainMenu → 加载Game场景 → 自动生成地牢
   ```

3. **验证控制台输出**：
   ```
   [GameManager] Initialized
   [LevelManager] Initialized
   [TurnManager] Initialized
   [SceneLoader] Loading scene: MainMenu
   [MainMenuUI] 开始新游戏
   [GameInitializer] === 游戏初始化开始 ===
   [LevelManager] Generating level 1
   [GameInitializer] === 游戏初始化完成 ===
   ```

---

## 🎮 平台设置

### PC开发测试
- **Platform**: Windows
- **Architecture**: x86_64

### 移动端发布（Week 10+）
- **Platform**: Android / iOS
- **Texture Compression**: ASTC
- **Graphics API**: Vulkan (Android) / Metal (iOS)

---

## ⚠️ 注意事项

### 1. TestDungeon场景
- ❌ **不要添加到Build Settings**
- ✅ 仅用于开发测试

### 2. 场景路径
- 注意场景实际路径是：
  ```
  Assets/_Project/Scenes/Boot.unity/Boot.unity
  ```
  而不是 `Assets/_Project/Scenes/Boot.unity`

### 3. 首次运行
- 请确保**从Boot场景启动游戏**
- 直接运行Game场景会因为Managers未初始化而报错

---

## 🔧 常见问题

### Q1: 场景加载失败？
**A**: 检查：
1. 场景是否添加到Build Settings？
2. 场景名称是否正确（区分大小写）？
3. 场景索引顺序是否正确？

### Q2: Manager找不到？
**A**: 确保：
1. 从Boot场景启动
2. GameSystems对象上有所有Manager组件
3. 所有Manager继承自Singleton且有DontDestroyOnLoad

### Q3: 地牢不显示？
**A**: 检查：
1. LevelRenderer组件是否添加？
2. Tilemap引用是否正确配置？
3. Tile资源是否已分配？

---

## ✅ 配置完成标志

当你完成配置后，应该能够：

1. ✅ 从Boot场景启动，自动跳转到MainMenu
2. ✅ 点击"New Game"按钮，进入游戏场景
3. ✅ 看到自动生成的地牢地图
4. ✅ 控制台没有错误，只有正常的初始化日志

---

**配置时间：** 约2-3分钟  
**下一步：** Week 5 - 实现玩家系统

