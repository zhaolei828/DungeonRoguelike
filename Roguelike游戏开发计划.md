# Unity Roguelike地牢探索游戏 - 完整开发计划

> 类似《进入地牢》《元气骑士》的2D地牢探索游戏

---

## 🎯 项目目标

**游戏类型**：2D Roguelike地牢探索  
**开发周期**：4-6周MVP（最小可玩版本）  
**技术栈**：Unity 2022.3 LTS + C#  

---

## 📋 第一阶段：环境搭建（第1天）

### 1. 安装Unity 2022.3 LTS
```
下载地址：Unity Hub → Installs → Add
版本选择：2022.3.x LTS（最新的LTS版本）
```

### 2. 创建项目
```
模板：2D (URP)  - Universal Render Pipeline
项目名：MyDungeonCrawler
```

### 3. 导入必备插件（Package Manager）
```
必装：
├── Cinemachine（相机系统）
├── 2D Tilemap Editor（地图编辑）
└── Input System（新输入系统）

推荐：
├── 2D Sprite（精灵图工具）
└── 2D Animation（动画工具）
```

### 4. 项目结构设置
```
Assets/
├── _Project/                    # 主项目文件夹
│   ├── Scripts/                # 代码
│   │   ├── Core/              # 核心系统
│   │   ├── Player/            # 玩家相关
│   │   ├── Enemy/             # 敌人相关
│   │   ├── Map/               # 地图生成
│   │   ├── Items/             # 道具系统
│   │   └── UI/                # UI系统
│   ├── Prefabs/               # 预制体
│   │   ├── Player/
│   │   ├── Enemies/
│   │   ├── Rooms/
│   │   └── Items/
│   ├── Sprites/               # 图片资源
│   ├── Animations/            # 动画
│   ├── Scenes/                # 场景
│   └── ScriptableObjects/     # 配置数据
└── Plugins/                    # 第三方插件
```

---

## 🎮 第二阶段：核心玩法开发（第1周）

### Day 1-2: 玩家控制器

**目标**：实现可以八方向移动和攻击的角色

```csharp
// 文件：Scripts/Player/PlayerController.cs
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("移动设置")]
    [SerializeField] private float moveSpeed = 5f;
    
    [Header("战斗设置")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.5f;
    
    private Vector2 moveInput;
    private Rigidbody2D rb;
    private Animator animator;
    private float lastAttackTime;
    
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    
    void FixedUpdate()
    {
        // 移动
        rb.velocity = moveInput * moveSpeed;
        
        // 更新动画
        if (moveInput != Vector2.zero)
        {
            animator.SetFloat("MoveX", moveInput.x);
            animator.SetFloat("MoveY", moveInput.y);
            animator.SetBool("IsMoving", true);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }
    
    // Input System回调
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    
    public void OnAttack()
    {
        if (Time.time - lastAttackTime < attackCooldown)
            return;
            
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");
        
        // 攻击检测
        PerformAttack();
    }
    
    private void PerformAttack()
    {
        Vector2 attackDirection = moveInput != Vector2.zero ? moveInput : Vector2.down;
        Vector2 attackPosition = (Vector2)transform.position + attackDirection * attackRange;
        
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPosition, 0.5f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<Enemy>()?.TakeDamage(attackDamage);
            }
        }
    }
}
```

**创建Input Actions**：
```
1. 右键 → Create → Input Actions
2. 命名为 "PlayerInputActions"
3. 添加：
   - Move (Value, Vector2)
   - Attack (Button)
4. Generate C# Class
```

---

### Day 3-4: 简单地图生成

**目标**：生成随机的房间布局

```csharp
// 文件：Scripts/Map/DungeonGenerator.cs
using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator : MonoBehaviour
{
    [Header("地图设置")]
    [SerializeField] private int minRooms = 5;
    [SerializeField] private int maxRooms = 10;
    [SerializeField] private Vector2Int roomSize = new Vector2Int(10, 10);
    
    [Header("房间预制体")]
    [SerializeField] private GameObject[] roomPrefabs;
    [SerializeField] private GameObject startRoomPrefab;
    [SerializeField] private GameObject bossRoomPrefab;
    
    private List<Room> rooms = new List<Room>();
    private Grid grid;
    
    void Start()
    {
        GenerateDungeon();
    }
    
    public void GenerateDungeon()
    {
        // 清空旧地图
        ClearDungeon();
        
        int roomCount = Random.Range(minRooms, maxRooms + 1);
        
        // 生成起始房间
        Room startRoom = CreateRoom(Vector2Int.zero, startRoomPrefab);
        rooms.Add(startRoom);
        
        // 生成其他房间
        for (int i = 1; i < roomCount - 1; i++)
        {
            Vector2Int newPosition = FindValidRoomPosition();
            Room newRoom = CreateRoom(newPosition, GetRandomRoomPrefab());
            rooms.Add(newRoom);
        }
        
        // 生成Boss房间
        Vector2Int bossPosition = FindValidRoomPosition();
        Room bossRoom = CreateRoom(bossPosition, bossRoomPrefab);
        rooms.Add(bossRoom);
        
        // 连接房间（生成走廊）
        ConnectRooms();
    }
    
    private Room CreateRoom(Vector2Int gridPosition, GameObject prefab)
    {
        Vector3 worldPosition = new Vector3(
            gridPosition.x * roomSize.x, 
            gridPosition.y * roomSize.y, 
            0
        );
        
        GameObject roomObj = Instantiate(prefab, worldPosition, Quaternion.identity, transform);
        Room room = roomObj.GetComponent<Room>();
        room.GridPosition = gridPosition;
        
        return room;
    }
    
    private Vector2Int FindValidRoomPosition()
    {
        int attempts = 100;
        while (attempts > 0)
        {
            // 从现有房间随机选择一个
            Room baseRoom = rooms[Random.Range(0, rooms.Count)];
            
            // 在四个方向随机选择一个
            Vector2Int[] directions = {
                Vector2Int.up, Vector2Int.down,
                Vector2Int.left, Vector2Int.right
            };
            Vector2Int direction = directions[Random.Range(0, 4)];
            Vector2Int newPosition = baseRoom.GridPosition + direction;
            
            // 检查位置是否已被占用
            if (!IsPositionOccupied(newPosition))
            {
                return newPosition;
            }
            
            attempts--;
        }
        
        // 如果找不到，返回一个随机位置
        return new Vector2Int(Random.Range(-5, 5), Random.Range(-5, 5));
    }
    
    private bool IsPositionOccupied(Vector2Int position)
    {
        foreach (var room in rooms)
        {
            if (room.GridPosition == position)
                return true;
        }
        return false;
    }
    
    private GameObject GetRandomRoomPrefab()
    {
        return roomPrefabs[Random.Range(0, roomPrefabs.Length)];
    }
    
    private void ConnectRooms()
    {
        // 简化版：只连接相邻的房间
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            CreateCorridor(rooms[i], rooms[i + 1]);
        }
    }
    
    private void CreateCorridor(Room from, Room to)
    {
        // 这里可以实现走廊生成逻辑
        // 简单版本：只是打开相邻的门
        Vector2Int direction = to.GridPosition - from.GridPosition;
        
        if (direction == Vector2Int.up)
        {
            from.OpenDoor(Direction.North);
            to.OpenDoor(Direction.South);
        }
        else if (direction == Vector2Int.down)
        {
            from.OpenDoor(Direction.South);
            to.OpenDoor(Direction.North);
        }
        else if (direction == Vector2Int.right)
        {
            from.OpenDoor(Direction.East);
            to.OpenDoor(Direction.West);
        }
        else if (direction == Vector2Int.left)
        {
            from.OpenDoor(Direction.West);
            to.OpenDoor(Direction.East);
        }
    }
    
    private void ClearDungeon()
    {
        foreach (var room in rooms)
        {
            if (room != null)
                Destroy(room.gameObject);
        }
        rooms.Clear();
    }
}

// 房间方向枚举
public enum Direction { North, South, East, West }
```

```csharp
// 文件：Scripts/Map/Room.cs
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector2Int GridPosition { get; set; }
    
    [Header("门对象")]
    [SerializeField] private GameObject northDoor;
    [SerializeField] private GameObject southDoor;
    [SerializeField] private GameObject eastDoor;
    [SerializeField] private GameObject westDoor;
    
    [Header("房间设置")]
    [SerializeField] private bool isCleared = false;
    
    public void OpenDoor(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                if (northDoor) northDoor.SetActive(true);
                break;
            case Direction.South:
                if (southDoor) southDoor.SetActive(true);
                break;
            case Direction.East:
                if (eastDoor) eastDoor.SetActive(true);
                break;
            case Direction.West:
                if (westDoor) westDoor.SetActive(true);
                break;
        }
    }
    
    public void CloseDoor(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                if (northDoor) northDoor.SetActive(false);
                break;
            case Direction.South:
                if (southDoor) southDoor.SetActive(false);
                break;
            case Direction.East:
                if (eastDoor) eastDoor.SetActive(false);
                break;
            case Direction.West:
                if (westDoor) westDoor.SetActive(false);
                break;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 玩家进入房间
            OnPlayerEnter();
        }
    }
    
    private void OnPlayerEnter()
    {
        if (!isCleared)
        {
            // 关闭所有门
            CloseDoor(Direction.North);
            CloseDoor(Direction.South);
            CloseDoor(Direction.East);
            CloseDoor(Direction.West);
            
            // 生成敌人
            SpawnEnemies();
        }
    }
    
    private void SpawnEnemies()
    {
        // 待实现：生成敌人逻辑
    }
    
    public void OnRoomCleared()
    {
        isCleared = true;
        // 打开所有门
        OpenDoor(Direction.North);
        OpenDoor(Direction.South);
        OpenDoor(Direction.East);
        OpenDoor(Direction.West);
    }
}
```

---

### Day 5-6: 敌人AI系统

**目标**：实现简单的追逐AI

```csharp
// 文件：Scripts/Enemy/Enemy.cs
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("属性")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private int currentHealth;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int damage = 1;
    
    [Header("AI设置")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float attackCooldown = 1f;
    
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private float lastAttackTime;
    
    private enum State { Idle, Chase, Attack }
    private State currentState = State.Idle;
    
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }
    
    void Update()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // 状态机
        switch (currentState)
        {
            case State.Idle:
                if (distanceToPlayer < detectionRange)
                {
                    currentState = State.Chase;
                }
                break;
                
            case State.Chase:
                if (distanceToPlayer <= attackRange)
                {
                    currentState = State.Attack;
                }
                else if (distanceToPlayer > detectionRange)
                {
                    currentState = State.Idle;
                }
                else
                {
                    ChasePlayer();
                }
                break;
                
            case State.Attack:
                if (distanceToPlayer > attackRange)
                {
                    currentState = State.Chase;
                }
                else
                {
                    AttackPlayer();
                }
                break;
        }
    }
    
    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        
        animator.SetBool("IsMoving", true);
    }
    
    private void AttackPlayer()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("IsMoving", false);
        
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            animator.SetTrigger("Attack");
            
            // 对玩家造成伤害
            player.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
    }
    
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        animator.SetTrigger("Hit");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    private void Die()
    {
        // 播放死亡动画
        animator.SetTrigger("Die");
        
        // 禁用碰撞和AI
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        rb.velocity = Vector2.zero;
        
        // 掉落道具（待实现）
        
        // 延迟销毁
        Destroy(gameObject, 1f);
    }
}
```

---

### Day 7: 玩家生命值系统

```csharp
// 文件：Scripts/Player/PlayerHealth.cs
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("生命值设置")]
    [SerializeField] private int maxHealth = 6;
    [SerializeField] private int currentHealth;
    
    [Header("事件")]
    public UnityEvent<int, int> OnHealthChanged; // 当前血量, 最大血量
    public UnityEvent OnPlayerDied;
    
    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }
    
    private void Die()
    {
        OnPlayerDied?.Invoke();
        // 触发游戏结束逻辑
        GameManager.Instance.GameOver();
    }
}
```

---

## 🎨 第三阶段：美术资源（第2周）

### 免费资源推荐

**像素风格资源**：
```
1. itch.io
   - 搜索: "pixel art dungeon"
   - 推荐: "Dungeon Tileset" by 0x72

2. OpenGameArt.org
   - 搜索: "roguelike tileset"

3. Kenney.nl
   - 免费的Roguelike资源包
```

**快速制作方案**：
```
Day 1-2: 导入基础Tileset
├── 地板、墙壁瓦片
├── 门、宝箱等道具
└── 设置Tilemap Palette

Day 3-4: 角色和敌人
├── 玩家精灵图和动画
├── 2-3种敌人精灵图
└── 攻击、受伤、死亡动画

Day 5-6: UI元素
├── 生命值条
├── 小地图
└── 暂停菜单

Day 7: 特效和音效
├── 攻击特效
├── 死亡特效
└── 背景音乐和音效
```

---

## 🔧 第四阶段：系统完善（第3周）

### 必要系统

**1. 道具系统**
```csharp
// ScriptableObject配置
[CreateAssetMenu(fileName = "NewItem", menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType type;
    public int value;
}

public enum ItemType { Weapon, HealthPotion, Key, Treasure }
```

**2. 游戏管理器**
```csharp
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("游戏状态")]
    public int currentLevel = 1;
    public int score = 0;
    public int coinsCollected = 0;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void RestartLevel()
    {
        // 重新生成地牢
        FindObjectOfType<DungeonGenerator>().GenerateDungeon();
    }
    
    public void NextLevel()
    {
        currentLevel++;
        RestartLevel();
    }
    
    public void GameOver()
    {
        // 显示游戏结束界面
        UIManager.Instance.ShowGameOverScreen();
    }
}
```

**3. UI系统**
```
- 主菜单
- 游戏HUD（生命值、金币等）
- 暂停菜单
- 游戏结束界面
```

---

## 📊 第五阶段：测试和优化（第4周）

### 测试清单

```
□ 玩家移动流畅性
□ 战斗手感
□ 地图生成的合理性
□ 敌人AI的挑战性
□ 性能优化（帧率稳定60fps）
□ UI交互流畅性
□ 音效和音乐平衡
```

### 优化建议

```
1. 对象池（敌人、子弹、特效）
2. 地图分块加载（如果地图很大）
3. 减少每帧的碰撞检测
4. 使用Sprite Atlas打包图片
```

---

## 🚀 扩展功能（未来迭代）

### MVP后可以添加的功能

```
第一批扩展（+2周）：
├── 更多敌人类型（5-10种）
├── Boss战设计
├── 装备系统
├── 技能系统
└── 成就系统

第二批扩展（+3周）：
├── 多个角色可选
├── 随机事件房间
├── 商店系统
├── 永久升级系统
└── 每日挑战

第三批扩展（+4周）：
├── 多人联机（使用Mirror）
├── 排行榜
├── 云存档
└── 更多地图主题
```

---

## 📚 学习资源

### 推荐教程

**YouTube/B站**：
```
1. "Unity 2D Roguelike Tutorial" - Brackeys
2. "程序化地牢生成" - 多个中文教程
3. "Unity像素游戏完整教程"
```

### 推荐文档

```
1. Unity 2D文档
2. Tilemap系统文档
3. Input System文档
```

---

## 🎯 里程碑检查点

### Week 1结束：
- [x] 可以控制角色移动和攻击
- [x] 生成简单的随机地图
- [x] 有基础的敌人AI

### Week 2结束：
- [x] 有完整的美术资源
- [x] 角色和敌人有动画
- [x] 基础UI完成

### Week 3结束：
- [x] 道具系统可用
- [x] 游戏有开始和结束
- [x] 关卡可以推进

### Week 4结束：
- [x] 游戏完整可玩
- [x] 没有明显Bug
- [x] 性能稳定

---

## 💡 开发建议

1. **先做核心循环**
   - 移动 → 战斗 → 探索 → 死亡/胜利
   - 确保这个循环好玩

2. **迭代开发**
   - 不要一次做太多功能
   - 每个功能都测试后再做下一个

3. **保持简单**
   - 先做最简版本
   - 能用就行，不要追求完美

4. **频繁测试**
   - 每天都玩一下自己的游戏
   - 让朋友试玩并收集反馈

---

## 📞 遇到问题？

**调试技巧**：
```
1. 使用Debug.Log打印信息
2. 使用Gizmos可视化（攻击范围、检测范围等）
3. 使用Unity Profiler检查性能
4. 保持代码简单，一次只做一件事
```

**常见问题**：
```
Q: 地图生成失败？
A: 检查房间预制体是否正确，Grid Position是否冲突

Q: 敌人不追玩家？
A: 检查Player的Tag是否为"Player"

Q: 攻击无效？
A: 检查Layer和碰撞矩阵设置
```

---

**版本**：v1.0  
**更新时间**：2025-10-15  
**适用Unity版本**：2022.3 LTS及以上

