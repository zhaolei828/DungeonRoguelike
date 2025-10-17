# 开源Roguelike项目移植到Unity完全指南

> 如何将其他语言/引擎的优秀Roguelike项目移植到Unity

---

## 🎯 为什么要移植而不是直接用Unity项目？

### 优势
```
1. ✅ 学习经典设计
   → 很多经典Roguelike有20+年打磨
   → 设计理念经过时间验证

2. ✅ 代码质量更高
   → 非Unity项目通常更注重代码结构
   → 没有Unity特定的坏习惯

3. ✅ 算法更清晰
   → C/Java代码更容易理解算法本质
   → 没有Unity API干扰

4. ✅ 学到更多
   → 理解跨平台开发
   → 锻炼代码翻译能力
```

### 劣势
```
⚠️ 需要额外工作量
⚠️ 需要理解两套代码
⚠️ 可能遇到不兼容的设计
```

---

## 📋 推荐项目详细分析

### 🏆 最推荐：Shattered Pixel Dungeon（Java → Unity/C#）

#### 项目信息
```
GitHub: 00-Evan/shattered-pixel-dungeon
语言: Java (libGDX框架)
Unity目标版本: 2022.3 LTS
移植难度: ⭐⭐⭐☆☆（中等）
预计时间: 4-6周（核心功能）
```

#### 为什么选这个项目？

**1. 语法几乎相同**
```java
// Java代码
public class Hero extends Char {
    private int strength = 10;
    
    public void damage(int dmg) {
        HP -= dmg;
        if (HP <= 0) {
            die();
        }
    }
}

// 移植到C#（几乎一样！）
public class Hero : Char {
    private int strength = 10;
    
    public void Damage(int dmg) {
        HP -= dmg;
        if (HP <= 0) {
            Die();
        }
    }
}

主要差异：
- Java的extends → C#的 :
- 方法名：Java小写开头 → C#大写开头（可选）
- 其他基本一致！
```

**2. 功能完整**
```
包含系统：
✅ 完整的地牢生成
✅ 4个职业
✅ 装备系统（武器、护甲、戒指等）
✅ 药水和卷轴
✅ 怪物AI
✅ Boss战
✅ 陷阱和环境互动
✅ 存档系统
✅ 成就系统
```

**3. 代码结构优秀**
```
com.shatteredpixel.shatteredpixeldungeon/
├── actors/          # 角色（玩家、敌人）
├── items/           # 道具
├── levels/          # 关卡生成
├── mechanics/       # 游戏机制
├── scenes/          # 场景管理
├── sprites/         # 精灵图
├── ui/              # UI
└── windows/         # 窗口

这个结构可以直接映射到Unity！
```

---

## 🛠️ 移植流程（以Shattered Pixel Dungeon为例）

### 阶段1：准备工作（Day 1-2）

#### Step 1: 下载和研究原项目
```bash
# 克隆项目
git clone https://github.com/00-Evan/shattered-pixel-dungeon.git

# 用IntelliJ IDEA或VS Code打开
# 运行看看游戏是什么样的
```

#### Step 2: 分析项目结构
```
创建一个文档：项目结构分析.txt

记录：
1. 核心类的作用（Hero、Dungeon、Level等）
2. 关键算法位置（地图生成在哪个文件）
3. 数据存储方式（装备属性如何定义）
4. 状态管理（游戏状态机）
```

#### Step 3: 创建Unity项目
```
Unity Hub → New Project
├── 模板：2D (URP)
├── 项目名：ShatteredPixelDungeon-Unity
└── Unity版本：2022.3 LTS

导入Package：
├── Cinemachine
├── Input System
└── 2D Tilemap Editor
```

#### Step 4: 提取美术资源
```
Shattered Pixel Dungeon的资源是开源的！

位置：assets/sprites/

提取：
1. 复制所有PNG到Unity的Assets/Sprites/
2. 设置为Sprite (2D and UI)
3. Pixels Per Unit: 根据原游戏设置

注意：
资源使用遵守项目的License（通常是GPL）
```

---

### 阶段2：核心系统移植（Week 1-2）

#### 系统1: 角色系统（Day 1-3）

**原Java代码分析**：
```java
// com/shatteredpixel/.../actors/Char.java
public class Char extends Actor {
    public int HP = 1;
    public int HT = 1;  // Health Total
    public int pos = 0; // Position in level
    
    public boolean attack(Char enemy) {
        // 攻击逻辑
    }
    
    public void damage(int dmg, Object src) {
        // 受伤逻辑
    }
}
```

**Unity移植（C#）**：
```csharp
// Assets/Scripts/Actors/Char.cs
using UnityEngine;

public class Char : Actor
{
    [Header("生命值")]
    public int HP = 1;
    public int HT = 1;  // Health Total
    
    [Header("位置")]
    public int pos = 0; // 在关卡中的位置（格子索引）
    
    [Header("组件引用")]
    protected SpriteRenderer spriteRenderer;
    protected Animator animator;
    
    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    
    public virtual bool Attack(Char enemy)
    {
        // 攻击逻辑
        int damage = Random.Range(1, 5); // 简化版
        enemy.Damage(damage, this);
        return true;
    }
    
    public virtual void Damage(int dmg, object src)
    {
        HP -= dmg;
        
        // Unity特有：播放受伤动画
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
        
        if (HP <= 0)
        {
            Die();
        }
    }
    
    protected virtual void Die()
    {
        // 死亡处理
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        
        // 延迟销毁
        Destroy(gameObject, 1f);
    }
}
```

**移植要点**：
```
1. 保持原有的逻辑结构
2. 添加Unity特有的功能（动画、组件等）
3. 用Unity的坐标系统替换格子位置
4. 添加可视化（Inspector面板）
```

---

#### 系统2: 地牢生成（Day 4-7）

**原Java代码**（简化版）：
```java
// com/shatteredpixel/.../levels/RegularLevel.java
public class RegularLevel extends Level {
    @Override
    protected boolean build() {
        // 创建房间
        ArrayList<Room> rooms = new ArrayList<>();
        
        // 主房间
        Room entrance = new Room();
        entrance.set(/* 设置尺寸和位置 */);
        rooms.add(entrance);
        
        // 连接房间
        connectRooms(rooms);
        
        // 放置门和通道
        placeDoors(rooms);
        
        return true;
    }
}
```

**Unity移植思路**：

```csharp
// Assets/Scripts/Levels/LevelGenerator.cs
using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("地图设置")]
    [SerializeField] private int width = 32;
    [SerializeField] private int height = 32;
    [SerializeField] private int minRooms = 5;
    [SerializeField] private int maxRooms = 10;
    
    [Header("房间预制体")]
    [SerializeField] private GameObject floorTilePrefab;
    [SerializeField] private GameObject wallTilePrefab;
    [SerializeField] private GameObject doorPrefab;
    
    [Header("Tilemap")]
    [SerializeField] private Grid grid;
    
    // 地图数据（0=空, 1=地板, 2=墙）
    private int[,] map;
    private List<Room> rooms = new List<Room>();
    
    public void GenerateLevel()
    {
        // 1. 初始化地图
        map = new int[width, height];
        
        // 2. 创建房间
        CreateRooms();
        
        // 3. 连接房间
        ConnectRooms();
        
        // 4. 放置墙壁
        PlaceWalls();
        
        // 5. 实例化瓦片
        InstantiateTiles();
    }
    
    private void CreateRooms()
    {
        int roomCount = Random.Range(minRooms, maxRooms + 1);
        
        for (int i = 0; i < roomCount; i++)
        {
            // 随机房间大小
            int roomWidth = Random.Range(3, 8);
            int roomHeight = Random.Range(3, 8);
            
            // 随机位置（确保不重叠）
            Vector2Int position = FindValidRoomPosition(roomWidth, roomHeight);
            
            // 创建房间
            Room room = new Room(position, roomWidth, roomHeight);
            rooms.Add(room);
            
            // 在地图上标记
            MarkRoomOnMap(room);
        }
    }
    
    private Vector2Int FindValidRoomPosition(int w, int h)
    {
        int attempts = 100;
        while (attempts > 0)
        {
            int x = Random.Range(1, width - w - 1);
            int y = Random.Range(1, height - h - 1);
            
            if (IsAreaEmpty(x, y, w, h))
            {
                return new Vector2Int(x, y);
            }
            
            attempts--;
        }
        
        return new Vector2Int(1, 1);
    }
    
    private bool IsAreaEmpty(int x, int y, int w, int h)
    {
        // 检查区域是否为空（包括1格边界）
        for (int i = x - 1; i < x + w + 1; i++)
        {
            for (int j = y - 1; j < y + h + 1; j++)
            {
                if (i < 0 || i >= width || j < 0 || j >= height)
                    continue;
                    
                if (map[i, j] != 0)
                    return false;
            }
        }
        return true;
    }
    
    private void MarkRoomOnMap(Room room)
    {
        for (int x = room.Position.x; x < room.Position.x + room.Width; x++)
        {
            for (int y = room.Position.y; y < room.Position.y + room.Height; y++)
            {
                map[x, y] = 1; // 1 = 地板
            }
        }
    }
    
    private void ConnectRooms()
    {
        // 简化版：连接相邻房间
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            CreateCorridor(rooms[i], rooms[i + 1]);
        }
    }
    
    private void CreateCorridor(Room from, Room to)
    {
        Vector2Int fromCenter = from.Center;
        Vector2Int toCenter = to.Center;
        
        // L形走廊
        // 先水平移动
        int x = fromCenter.x;
        while (x != toCenter.x)
        {
            map[x, fromCenter.y] = 1;
            x += (x < toCenter.x) ? 1 : -1;
        }
        
        // 再垂直移动
        int y = fromCenter.y;
        while (y != toCenter.y)
        {
            map[toCenter.x, y] = 1;
            y += (y < toCenter.y) ? 1 : -1;
        }
    }
    
    private void PlaceWalls()
    {
        // 在地板周围放置墙壁
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (map[x, y] == 1) // 如果是地板
                {
                    // 检查8个方向
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            if (dx == 0 && dy == 0) continue;
                            
                            int nx = x + dx;
                            int ny = y + dy;
                            
                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                if (map[nx, ny] == 0)
                                {
                                    map[nx, ny] = 2; // 2 = 墙
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    
    private void InstantiateTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x, y, 0);
                
                if (map[x, y] == 1) // 地板
                {
                    Instantiate(floorTilePrefab, position, Quaternion.identity, transform);
                }
                else if (map[x, y] == 2) // 墙
                {
                    Instantiate(wallTilePrefab, position, Quaternion.identity, transform);
                }
            }
        }
    }
}

// 房间类
[System.Serializable]
public class Room
{
    public Vector2Int Position;
    public int Width;
    public int Height;
    
    public Room(Vector2Int pos, int w, int h)
    {
        Position = pos;
        Width = w;
        Height = h;
    }
    
    public Vector2Int Center
    {
        get { return new Vector2Int(Position.x + Width / 2, Position.y + Height / 2); }
    }
}
```

**对比原项目**：
```
原Java项目（Shattered PD）：
- 用一维数组存地图：terrain[]
- 用整数表示位置：pos = x + y * width

Unity版本改进：
- 用二维数组：map[x,y]（更直观）
- 用Vector2Int表示位置
- 直接可视化（实例化瓦片）
```

---

#### 系统3: 物品系统（Week 2）

**从Java移植Item类**：

```csharp
// Assets/Scripts/Items/Item.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class Item : ScriptableObject
{
    [Header("基础信息")]
    public string itemName;
    public Sprite icon;
    public string description;
    
    [Header("属性")]
    public ItemType type;
    public int level = 1;
    public int quantity = 1;
    
    [Header("效果")]
    public int healAmount = 0;
    public int damageBonus = 0;
    public int armorBonus = 0;
    
    // 使用物品
    public virtual bool DoUse(Hero hero)
    {
        // 基础使用逻辑
        return false;
    }
    
    // 装备物品（武器、护甲）
    public virtual bool DoEquip(Hero hero)
    {
        return false;
    }
}

public enum ItemType
{
    Weapon,
    Armor,
    Potion,
    Scroll,
    Ring,
    Misc
}
```

**这样设计的好处**：
```
✅ Unity的ScriptableObject
   → 可以在Inspector中直接编辑
   → 不需要重新编译
   
✅ 比Java的硬编码灵活
   → Java: 每个物品一个类
   → Unity: 一个Item配置多个实例
```

---

### 阶段3：整合和优化（Week 3-4）

#### 整合检查清单

```
□ 核心玩法可玩
  ├─ □ 移动流畅
  ├─ □ 战斗有趣
  └─ □ 地牢生成合理

□ 系统完整性
  ├─ □ 道具可用
  ├─ □ 装备系统工作
  ├─ □ 敌人AI正常
  └─ □ 存档加载

□ Unity优化
  ├─ □ 对象池（敌人、特效）
  ├─ □ 资源打包
  └─ □ 性能稳定60fps
```

---

## 📊 其他项目移植难度对比

| 项目 | 原语言 | 移植难度 | 推荐度 | 原因 |
|-----|--------|---------|--------|------|
| **Shattered PD** | Java | ⭐⭐⭐☆☆ | ⭐⭐⭐⭐⭐ | 语法相似，功能完整 |
| **Brogue** | C | ⭐⭐⭐⭐☆ | ⭐⭐⭐⭐☆ | 算法经典，但需要理解C |
| **Pixel Dungeon** | Java | ⭐⭐☆☆☆ | ⭐⭐⭐⭐☆ | 比Shattered简单 |
| **DCSS** | C++ | ⭐⭐⭐⭐⭐ | ⭐⭐☆☆☆ | 太复杂，不建议全移植 |
| **HTML5游戏** | JS | ⭐⭐☆☆☆ | ⭐⭐⭐☆☆ | 语法差异大，但逻辑简单 |

---

## 🎯 移植策略选择

### 策略A：完整移植（不推荐）
```
工作量：巨大（3-6个月）
优势：功能最完整
劣势：学不到太多（只是翻译代码）
```

### 策略B：核心移植（推荐）⭐⭐⭐⭐⭐
```
工作量：适中（4-8周）
移植内容：
✅ 地图生成算法
✅ 战斗系统
✅ 基础道具系统
✅ 存档系统

不移植：
❌ 复杂的UI（自己用Unity做更好）
❌ 所有物品（太多了，选几个代表性的）
❌ 成就、商店等（后期添加）

优势：
✅ 学到核心算法
✅ 保持自己的风格
✅ 灵活调整
```

### 策略C：算法提取（最推荐新手）⭐⭐⭐⭐⭐
```
工作量：最小（1-2周）
只提取：
✅ 地图生成算法
✅ 寻路算法
✅ AI决策逻辑

其他自己实现：
🎨 UI完全自己设计
🎮 战斗系统简化版
📦 道具系统简化版

优势：
✅ 学到精髓
✅ 快速完成
✅ 完全掌控
```

---

## 💡 实用建议

### 移植时的注意事项

```
1. 不要一行行翻译
   → 理解设计意图
   → 用Unity的方式实现

2. 利用Unity的优势
   → ScriptableObject存数据
   → Prefab管理资源
   → Inspector可视化编辑

3. 简化复杂系统
   → 原项目可能有100种装备
   → 先做10种，验证系统可行

4. 记录学到的东西
   → 为什么这样设计？
   → 有什么可以改进的？
```

---

## 📚 推荐的移植顺序

### Week 1: 角色和移动
```
Java Hero类 → C# PlayerController
保持逻辑，添加Unity功能
```

### Week 2: 地图生成
```
Java Level生成 → C# LevelGenerator
核心算法保持，可视化方式改用Unity
```

### Week 3: 战斗和AI
```
Java战斗逻辑 → C# Combat + EnemyAI
简化版本，确保好玩
```

### Week 4: 道具和UI
```
Java物品系统 → C# Items + Inventory
UI完全重做（Unity UGUI）
```

---

## 🔗 有用资源

### Java到C#转换参考
```
在线工具：
- Tangible Software Solutions（Java to C#转换器）
- 虽然不完美，但能节省50%时间

文档：
- Microsoft官方：Java开发者的C#
- Unity Learn：从其他引擎过渡到Unity
```

---

**版本**：v1.0  
**更新时间**：2025-10-15  
**最适合**：有一定编程基础，想深入学习游戏设计的开发者

