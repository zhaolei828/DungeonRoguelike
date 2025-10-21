using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 怪物生成器 - 在地图生成后自动随机生成怪物
/// </summary>
public class MobSpawner : MonoBehaviour
{
    [Header("生成配置")]
    [SerializeField] private int minMobs = 3;
    [SerializeField] private int maxMobs = 8;
    [SerializeField] private int spawnAttempts = 100; // 每个怪物的最大生成尝试次数
    
    [Header("怪物类型权重")]
    [SerializeField] private MobSpawnWeight[] mobWeights = new MobSpawnWeight[]
    {
        new MobSpawnWeight { mobType = MobType.Rat, weight = 30 },
        new MobSpawnWeight { mobType = MobType.Bat, weight = 25 },
        new MobSpawnWeight { mobType = MobType.Spider, weight = 20 },
        new MobSpawnWeight { mobType = MobType.Goblin, weight = 15 },
        new MobSpawnWeight { mobType = MobType.Orc, weight = 10 }
    };
    
    [Header("生成限制")]
    [SerializeField] private float minDistanceFromPlayer = 5f; // 距离玩家的最小距离
    [SerializeField] private bool spawnOnLevelGeneration = true; // 是否在地图生成时自动生成
    
    private Level currentLevel;
    private List<Mob> spawnedMobs = new List<Mob>();
    
    /// <summary>
    /// 在Level生成完成后自动生成怪物
    /// </summary>
    public void SpawnMobsForLevel(Level level)
    {
        if (level == null)
        {
            Debug.LogError("MobSpawner: Level is null!");
            return;
        }
        
        currentLevel = level;
        
        // 清理之前生成的怪物
        ClearSpawnedMobs();
        
        // 随机决定生成多少怪物
        int mobCount = Random.Range(minMobs, maxMobs + 1);
        
        Debug.Log($"<color=yellow>🎲 MobSpawner: 开始生成 {mobCount} 个怪物...</color>");
        
        // 生成怪物
        for (int i = 0; i < mobCount; i++)
        {
            MobType mobType = SelectRandomMobType();
            Mob mob = SpawnMob(mobType);
            
            if (mob != null)
            {
                spawnedMobs.Add(mob);
            }
        }
        
        Debug.Log($"<color=green>✓ MobSpawner: 成功生成 {spawnedMobs.Count}/{mobCount} 个怪物</color>");
    }
    
    /// <summary>
    /// 根据权重随机选择怪物类型
    /// </summary>
    private MobType SelectRandomMobType()
    {
        // 计算总权重
        int totalWeight = 0;
        foreach (var weight in mobWeights)
        {
            totalWeight += weight.weight;
        }
        
        // 随机选择
        int randomValue = Random.Range(0, totalWeight);
        int currentWeight = 0;
        
        foreach (var weight in mobWeights)
        {
            currentWeight += weight.weight;
            if (randomValue < currentWeight)
            {
                return weight.mobType;
            }
        }
        
        return MobType.Rat; // 默认返回老鼠
    }
    
    /// <summary>
    /// 生成指定类型的怪物
    /// </summary>
    private Mob SpawnMob(MobType mobType)
    {
        // 查找可用位置
        Vector2Int spawnPos = FindValidSpawnPosition();
        
        if (spawnPos == Vector2Int.zero && !currentLevel.IsPassable(spawnPos))
        {
            Debug.LogWarning($"MobSpawner: 无法找到有效位置生成 {mobType}");
            return null;
        }
        
        // 创建怪物GameObject
        string mobName = GetMobName(mobType);
        GameObject mobGO = new GameObject(mobName);
        
        // 添加对应的Mob组件
        Mob mob = AddMobComponent(mobGO, mobType);
        
        if (mob == null)
        {
            Destroy(mobGO);
            return null;
        }
        
        // 设置位置
        mob.pos = spawnPos;
        mobGO.transform.position = new Vector3(spawnPos.x + 0.5f, spawnPos.y + 0.5f, 0);
        
        // 添加SpriteRenderer（Mob.Start()会处理sprite加载）
        if (mobGO.GetComponent<SpriteRenderer>() == null)
        {
            mobGO.AddComponent<SpriteRenderer>();
        }
        
        // 添加碰撞体
        BoxCollider2D collider = mobGO.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1, 1);
        collider.isTrigger = true;
        
        Debug.Log($"<color=cyan>  • 生成 {mobName} at ({spawnPos.x}, {spawnPos.y})</color>");
        
        return mob;
    }
    
    /// <summary>
    /// 查找有效的生成位置
    /// </summary>
    private Vector2Int FindValidSpawnPosition()
    {
        Hero hero = GameManager.Instance?.Hero;
        Vector2Int heroPos = hero != null ? hero.pos : Vector2Int.zero;
        
        for (int attempt = 0; attempt < spawnAttempts; attempt++)
        {
            // 随机位置
            int x = Random.Range(2, currentLevel.Width - 2);
            int y = Random.Range(2, currentLevel.Height - 2);
            Vector2Int pos = new Vector2Int(x, y);
            
            // 检查是否可通行
            if (!currentLevel.IsPassable(pos))
                continue;
            
            // 检查是否距离玩家太近
            if (hero != null)
            {
                float distance = Vector2Int.Distance(pos, heroPos);
                if (distance < minDistanceFromPlayer)
                    continue;
            }
            
            // 检查是否已有其他怪物
            if (IsMobAtPosition(pos))
                continue;
            
            return pos;
        }
        
        // 如果找不到理想位置，返回任意可通行位置
        for (int x = 2; x < currentLevel.Width - 2; x++)
        {
            for (int y = 2; y < currentLevel.Height - 2; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (currentLevel.IsPassable(pos) && !IsMobAtPosition(pos))
                {
                    return pos;
                }
            }
        }
        
        return Vector2Int.zero;
    }
    
    /// <summary>
    /// 检查指定位置是否已有怪物
    /// </summary>
    private bool IsMobAtPosition(Vector2Int pos)
    {
        foreach (Mob mob in spawnedMobs)
        {
            if (mob != null && mob.pos == pos)
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// 添加对应类型的Mob组件
    /// </summary>
    private Mob AddMobComponent(GameObject go, MobType mobType)
    {
        switch (mobType)
        {
            case MobType.Rat:
                return go.AddComponent<Rat>();
            case MobType.Bat:
                return go.AddComponent<Bat>();
            case MobType.Spider:
                return go.AddComponent<Spider>();
            case MobType.Goblin:
                return go.AddComponent<Goblin>();
            case MobType.Orc:
                return go.AddComponent<Orc>();
            default:
                Debug.LogError($"MobSpawner: Unknown mob type {mobType}");
                return null;
        }
    }
    
    /// <summary>
    /// 获取怪物名称
    /// </summary>
    private string GetMobName(MobType mobType)
    {
        switch (mobType)
        {
            case MobType.Rat: return "老鼠";
            case MobType.Bat: return "蝙蝠";
            case MobType.Spider: return "蜘蛛";
            case MobType.Goblin: return "地精";
            case MobType.Orc: return "兽人";
            default: return "怪物";
        }
    }
    
    /// <summary>
    /// 清理已生成的怪物
    /// </summary>
    private void ClearSpawnedMobs()
    {
        foreach (Mob mob in spawnedMobs)
        {
            if (mob != null)
            {
                Destroy(mob.gameObject);
            }
        }
        spawnedMobs.Clear();
    }
    
    private void OnDestroy()
    {
        ClearSpawnedMobs();
    }
}

/// <summary>
/// 怪物类型枚举
/// </summary>
public enum MobType
{
    Rat,    // 老鼠
    Bat,    // 蝙蝠
    Spider, // 蜘蛛
    Goblin, // 地精
    Orc     // 兽人
}

/// <summary>
/// 怪物生成权重配置
/// </summary>
[System.Serializable]
public class MobSpawnWeight
{
    public MobType mobType;
    public int weight = 10;
}

