# SPD Buff图标映射表

**自动映射工具使用的标准映射关系**

---

## 📊 完整映射表

基于SPD的buffs.png（128x64，8x8切割，16列x8行=128个Sprite）

### 正面Buff（第1行）

| BuffType | Sprite索引 | Sprite名称 | 描述 | 颜色 |
|----------|-----------|-----------|------|------|
| Strength | 0 | buff_0 | 红色拳头 | 力量增强 |
| Shield | 1 | buff_1 | 蓝色盾牌 | 护盾保护 |
| Regeneration | 2 | buff_2 | 绿色心形 | 生命再生 |
| Haste | 3 | buff_3 | 黄色闪电 | 加速移动 |
| Agility | 4 | buff_4 | 绿色羽毛 | 敏捷提升 |
| Defense | 5 | buff_5 | 土黄色盾牌 | 防御提升 |
| Invisibility | 6 | buff_6 | 半透明幽灵 | 隐身状态 |

### 负面Buff（第2行）

| BuffType | Sprite索引 | Sprite名称 | 描述 | 颜色 |
|----------|-----------|-----------|------|------|
| Poison | 16 | buff_16 | 紫色毒液 | 中毒持续伤害 |
| Weakness | 17 | buff_17 | 灰色向下箭头 | 攻击力降低 |
| Slow | 18 | buff_18 | 蓝色龟壳 | 移动减速 |
| Bleeding | 19 | buff_19 | 深红色血滴 | 流血持续伤害 |
| Burning | 20 | buff_20 | 橙色火焰 | 燃烧持续伤害 |
| Frozen | 21 | buff_21 | 冰蓝色雪花 | 冰冻无法移动 |
| Paralysis | 22 | buff_22 | 黄色闪电 | 麻痹无法行动 |

### 控制Buff（第3行）

| BuffType | Sprite索引 | Sprite名称 | 描述 | 颜色 |
|----------|-----------|-----------|------|------|
| Blind | 32 | buff_32 | 黑色眼睛 | 失明无法看清 |
| Confusion | 33 | buff_33 | 紫色问号 | 混乱随机行动 |
| Sleep | 34 | buff_34 | 深蓝色Z | 睡眠无法行动 |
| Charmed | 35 | buff_35 | 粉色心形 | 魅惑被控制 |
| Terror | 36 | buff_36 | 深紫色骷髅 | 恐惧逃跑 |

---

## 🎨 布局说明

### buffs.png结构（8x8切割）

```
列数：16列（0-15）
行数：8行（0-7）
总计：128个Sprite

索引计算公式：
index = row * 16 + col

例如：
- buff_0  = 第0行第0列
- buff_1  = 第0行第1列
- buff_16 = 第1行第0列
- buff_32 = 第2行第0列
```

### 行分布

```
第0行（0-15）：  正面Buff（力量、护盾、再生等）
第1行（16-31）： 负面Buff（毒、虚弱、减速等）
第2行（32-47）： 控制Buff（失明、混乱、睡眠等）
第3-7行：        其他图标（未使用）
```

---

## 🔧 自定义映射

如果SPD的图标布局与你的不同，可以修改`BuffIconAutoMapper.cs`中的映射表：

```csharp
var mapping = new System.Collections.Generic.Dictionary<BuffType, int>
{
    { BuffType.Strength, 0 },      // 修改索引
    { BuffType.Shield, 1 },
    // ... 添加或修改映射
};
```

---

## 📝 使用说明

### 自动映射

使用`Tools > SPD > Auto Map Buff Icons`工具，会自动按照上表映射。

### 手动调整

如果自动映射的图标不满意：

1. 打开`BuffIconConfig.asset`
2. 找到对应的Buff类型
3. 手动选择更合适的Sprite
4. 保存

### 验证映射

在Unity中：
1. 打开`BuffIconConfig.asset`
2. 展开"Buff Icons"列表
3. 查看每个Buff类型的图标预览
4. 确认图标正确

---

## 🎯 常用Buff优先级

如果时间有限，优先配置这些常用Buff：

### 必配（5个）
1. **Strength** (力量) - buff_0
2. **Shield** (护盾) - buff_1
3. **Poison** (毒) - buff_16
4. **Regeneration** (再生) - buff_2
5. **Haste** (加速) - buff_3

### 推荐（5个）
6. **Weakness** (虚弱) - buff_17
7. **Slow** (减速) - buff_18
8. **Bleeding** (流血) - buff_19
9. **Burning** (燃烧) - buff_20
10. **Frozen** (冰冻) - buff_21

### 可选（9个）
11. **Paralysis** (麻痹) - buff_22
12. **Blind** (失明) - buff_32
13. **Confusion** (混乱) - buff_33
14. **Sleep** (睡眠) - buff_34
15. **Charmed** (魅惑) - buff_35
16. **Terror** (恐惧) - buff_36
17. **Agility** (敏捷) - buff_4
18. **Defense** (防御) - buff_5
19. **Invisibility** (隐身) - buff_6

---

## 🔍 图标识别技巧

### 按颜色识别

- **红色系**：力量、流血、燃烧（攻击相关）
- **蓝色系**：护盾、减速、冰冻（防御/控制）
- **绿色系**：再生、敏捷（恢复/增益）
- **黄色系**：加速、麻痹（速度相关）
- **紫色系**：毒、混乱（异常状态）
- **灰色系**：虚弱、失明（削弱）
- **粉色系**：魅惑（魅惑控制）

### 按形状识别

- **拳头**：力量
- **盾牌**：护盾、防御
- **心形**：再生、魅惑
- **闪电**：加速、麻痹
- **羽毛**：敏捷
- **液滴**：毒、流血
- **火焰**：燃烧
- **雪花**：冰冻
- **眼睛**：失明
- **问号**：混乱
- **Z字母**：睡眠
- **骷髅**：恐惧

---

## 📊 映射统计

- **总Buff类型**: 19个
- **已映射**: 19个
- **覆盖率**: 100%
- **使用的Sprite**: 19个（共128个可用）

---

## 🎉 完成

使用自动映射工具后，所有19个Buff类型都会被正确配置！

如果需要调整，参考本文档的映射表进行手动修改。

