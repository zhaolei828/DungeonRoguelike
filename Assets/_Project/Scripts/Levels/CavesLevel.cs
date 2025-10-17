using UnityEngine;

/// <summary>
/// 洞穴关卡 (11-15层)
/// 特点：熔岩、余烬地板、更自然的洞穴形状
/// </summary>
public class CavesLevel : RegularLevel
{
    [Header("洞穴特殊设置")]
    [SerializeField] private float lavaChance = 0.08f;       // 熔岩概率
    [SerializeField] private float emberChance = 0.2f;       // 余烬地板概率
    [SerializeField] private float chasmChance = 0.05f;      // 深渊概率
    
    #region Level Generation Overrides
    
    public override bool Generate()
    {
        if (!base.Generate())
            return false;
        
        AddCaveFeatures();
        return true;
    }
    
    private void AddCaveFeatures()
    {
        AddLavaAreas();
        AddEmberFloors();
        AddChasms();
        AddCaveDecorations();
        
        Debug.Log("Added cave-specific features");
    }
    
    /// <summary>
    /// 添加熔岩区域
    /// </summary>
    private void AddLavaAreas()
    {
        // 在一些房间中添加熔岩池
        foreach (Vector2Int roomCenter in roomPositions)
        {
            if (levelRandom.Next(100) < lavaChance * 200) // 增加房间中的熔岩概率
            {
                CreateLavaPool(roomCenter);
            }
        }
        
        // 在走廊中随机添加熔岩
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (GetTerrain(x, y) == Terrain.Floor && levelRandom.Next(100) < lavaChance * 100)
                {
                    SetTerrain(x, y, Terrain.Lava);
                }
            }
        }
    }
    
    /// <summary>
    /// 创建熔岩池
    /// </summary>
    /// <param name="center">中心位置</param>
    private void CreateLavaPool(Vector2Int center)
    {
        int poolSize = levelRandom.Next(1, 3);
        
        for (int dx = -poolSize; dx <= poolSize; dx++)
        {
            for (int dy = -poolSize; dy <= poolSize; dy++)
            {
                Vector2Int pos = center + new Vector2Int(dx, dy);
                
                if (IsValidPosition(pos) && GetTerrain(pos) == Terrain.Floor)
                {
                    if (levelRandom.Next(100) < 80)
                    {
                        SetTerrain(pos, Terrain.Lava);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 添加余烬地板
    /// </summary>
    private void AddEmberFloors()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (GetTerrain(x, y) == Terrain.Floor)
                {
                    // 在熔岩附近更容易出现余烬地板
                    float emberProbability = emberChance;
                    
                    if (IsNearLava(new Vector2Int(x, y)))
                    {
                        emberProbability *= 3f; // 熔岩附近3倍概率
                    }
                    
                    if (levelRandom.Next(100) < emberProbability * 100)
                    {
                        SetTerrain(x, y, Terrain.EmberFloor);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 检查是否靠近熔岩
    /// </summary>
    /// <param name="pos">位置</param>
    /// <returns>是否靠近熔岩</returns>
    private bool IsNearLava(Vector2Int pos)
    {
        for (int dx = -2; dx <= 2; dx++)
        {
            for (int dy = -2; dy <= 2; dy++)
            {
                Vector2Int checkPos = pos + new Vector2Int(dx, dy);
                if (IsValidPosition(checkPos) && GetTerrain(checkPos) == Terrain.Lava)
                {
                    return true;
                }
            }
        }
        return false;
    }
    
    /// <summary>
    /// 添加深渊
    /// </summary>
    private void AddChasms()
    {
        // 在一些位置添加小的深渊
        for (int x = 2; x < width - 2; x++)
        {
            for (int y = 2; y < height - 2; y++)
            {
                if (GetTerrain(x, y) == Terrain.Floor && levelRandom.Next(100) < chasmChance * 100)
                {
                    // 确保不在重要位置（入口出口附近）
                    if (Vector2Int.Distance(new Vector2Int(x, y), entrancePos) > 3 &&
                        Vector2Int.Distance(new Vector2Int(x, y), exitPos) > 3)
                    {
                        SetTerrain(x, y, Terrain.Chasm);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 添加洞穴装饰
    /// </summary>
    private void AddCaveDecorations()
    {
        // 添加更多雕像（代表洞穴中的钟乳石等）
        foreach (Vector2Int roomCenter in roomPositions)
        {
            if (levelRandom.Next(100) < 50)
            {
                Vector2Int statuePos = roomCenter + new Vector2Int(
                    levelRandom.Next(-2, 3),
                    levelRandom.Next(-2, 3)
                );
                
                if (IsValidPosition(statuePos) && GetTerrain(statuePos) == Terrain.Floor)
                {
                    SetTerrain(statuePos, Terrain.Statue);
                }
            }
        }
        
        // 添加一些基座（代表洞穴中的岩石堆）
        int pedestalCount = levelRandom.Next(3, 6);
        for (int i = 0; i < pedestalCount; i++)
        {
            Vector2Int pos = GetRandomValidPosition();
            if (GetTerrain(pos) == Terrain.Floor)
            {
                SetTerrain(pos, Terrain.Pedestal);
            }
        }
    }
    
    #endregion
    
    #region Cave-specific Methods
    
    /// <summary>
    /// 获取洞穴关卡统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    public string GetCaveInfo()
    {
        int lavaCount = 0;
        int emberCount = 0;
        int chasmCount = 0;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Terrain terrain = GetTerrain(x, y);
                switch (terrain)
                {
                    case Terrain.Lava:
                        lavaCount++;
                        break;
                    case Terrain.EmberFloor:
                        emberCount++;
                        break;
                    case Terrain.Chasm:
                        chasmCount++;
                        break;
                }
            }
        }
        
        return $"Cave Level Stats:\n" +
               $"Lava tiles: {lavaCount}\n" +
               $"Ember floors: {emberCount}\n" +
               $"Chasms: {chasmCount}";
    }
    
    #endregion
    
    #region Overrides
    
    protected override void AdjustSizeForType(LevelType type)
    {
        // 洞穴关卡更大更开放
        width = 36;
        height = 36;
    }
    
    public override string GetDebugInfo()
    {
        return base.GetDebugInfo() + "\n" + GetCaveInfo();
    }
    
    #endregion
}
