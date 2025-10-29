# SPD UI素材快速开始指南

**3步完成配置，总共4分钟**（使用自动映射）

---

## 📋 前置条件

- ✅ SPD UI素材已导入到`Assets/_Project/Art/UI/`
- ✅ 编译无错误

---

## 🚀 快速步骤

### 步骤0: 切割buffs.png（1分钟）⭐ 必须先做

```
Unity菜单 → Tools → SPD → Slice Buffs Sprite
→ 选择切片大小（默认8x8，推荐）
→ 点击"切割 buffs.png (8x8)"
→ 等待3秒
→ 完成！
```

**切片大小说明**：
- **8x8** - SPD原始大小，推荐（128x64 → 128个Sprite）
- 16x16 - 放大版本（128x64 → 32个Sprite）
- 可以用滑块调整4-16之间的任意值

**验证**：
- 点击"检查切割状态"按钮
- 应该显示"✅ 已切割！"和Sprite数量
- 在Project窗口选择`buffs.png`，Inspector中能看到多个子Sprite（buff_0, buff_1...）

---

### 步骤1: 创建配置文件（2分钟）

```
Unity菜单 → Tools → SPD → Create UI Configs
→ 点击"创建所有配置"
→ 等待10秒
→ 完成！
```

**自动完成的操作**：
- ✅ 创建`BuffIconConfig.asset`
- ✅ 创建`UIThemeConfig.asset`
- ✅ 自动生成19个Buff图标映射条目
- ✅ 自动加载面板素材

**验证**：
- `Assets/_Project/Resources/BuffIconConfig.asset` 存在
- `Assets/_Project/Resources/UIThemeConfig.asset` 存在

---

### 步骤2: 配置Buff图标（1分钟）⭐ 自动配置

**方法1：自动映射（推荐，1分钟）**

```
Unity菜单 → Tools → SPD → Auto Map Buff Icons
→ 点击"自动查找配置和素材"
→ 点击"自动映射Buff图标"
→ 完成！
```

**自动完成的操作**：
- ✅ 根据SPD标准布局自动分配19个Buff图标
- ✅ 自动保存配置
- ✅ 显示映射结果

---

**方法2：手动配置（5分钟）**

如果自动映射不满意，可以手动调整：

```
1. 打开 Assets/_Project/Resources/BuffIconConfig.asset
2. 展开"Buff Icons"列表（19个条目）
3. 展开 Assets/_Project/Art/UI/buffs.png（看到buff_0, buff_1...）
4. 拖拽Sprite到对应的Buff类型
5. 保存
```

**提示**：
- 自动映射后可以手动微调
- 不确定的可以先不分配
- 可以随时修改

---

## ✅ 完成检查

配置完成后，检查以下项目：

- [ ] `buffs.png`已切割（Inspector中有多个子Sprite）
- [ ] `BuffIconConfig.asset`已创建
- [ ] `UIThemeConfig.asset`已创建
- [ ] 至少配置了5个Buff图标
- [ ] 编译无错误

---

## 🧪 测试

```
1. 运行Game场景
2. 添加TestBattleUI组件到任意GameObject
3. 右键组件 → Test → Test Buff Icons
4. 观察Buff图标显示
```

**预期效果**：
- ✅ 配置过的Buff显示SPD图标
- ✅ 未配置的Buff显示纯色方块
- ✅ 倒计时文字清晰可见

---

## 🎨 Buff图标映射参考

根据SPD的buffs.png，推荐映射：

| Sprite | Buff类型 | 描述 |
|--------|---------|------|
| buff_0 | Strength | 红色拳头 - 力量增强 |
| buff_1 | Shield | 蓝色盾牌 - 护盾 |
| buff_2 | Poison | 紫色毒液 - 中毒 |
| buff_3 | Regeneration | 绿色心形 - 再生 |
| buff_4 | Haste | 黄色闪电 - 加速 |
| buff_5 | Slow | 蓝色龟壳 - 减速 |
| buff_6 | Weakness | 灰色向下箭头 - 虚弱 |
| buff_7 | Bleeding | 深红色血滴 - 流血 |
| buff_8 | Burning | 橙色火焰 - 燃烧 |
| buff_9 | Frozen | 冰蓝色雪花 - 冰冻 |

**注意**：实际索引可能不同，请根据实际切割结果调整。

---

## 🐛 常见问题

### Q: 找不到"Slice Buffs Sprite"菜单？
A: 
1. 确保`BuffsSpriteSlicerTool.cs`在Editor文件夹
2. 等待Unity重新编译
3. 重启Unity编辑器

### Q: 切割后Inspector中看不到子Sprite？
A: 
1. 点击buffs.png旁边的小箭头展开
2. 或在Inspector中查看Sprite Mode是否为Multiple
3. 重新运行切割工具

### Q: 配置文件创建失败？
A: 
1. 确保`Assets/_Project/Resources/`文件夹存在
2. 如果不存在，手动创建
3. 重新运行创建工具

### Q: Buff图标不显示？
A: 
1. 确保buffs.png已切割
2. 确保BuffIconConfig在Resources文件夹
3. 确保已分配图标到对应的Buff类型
4. 运行测试验证

---

## 📚 详细文档

如需更多信息，请查看：

- **详细配置**: `Docs/SPD_UI素材配置指南.md`
- **技术详解**: `Docs/SPD_UI素材集成完成.md`
- **导入说明**: `Docs/SPD_UI素材导入指南.md`

---

## 🎉 完成！

配置完成后，你的战斗UI将拥有：
- ✅ 专业的像素风格Buff图标
- ✅ 统一的UI主题
- ✅ 更好的视觉体验

**现在可以在游戏中测试效果了！** 🚀

