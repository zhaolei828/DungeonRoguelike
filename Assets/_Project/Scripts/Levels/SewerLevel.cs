using UnityEngine;

/// <summary>
/// 下水道关卡 (1-5层)
/// 特点：有水、高草、简单布局
/// </summary>
public class SewerLevel : RegularLevel
{
    [Header("下水道特殊设置")]
    [SerializeField] private float waterChance = 0.15f;      // 水的生成概率
    [SerializeField] private float grassChance = 0.25f;     // 草的生成概率
    [SerializeField] private float highGrassChance = 0.1f;  // 高草的生成概率
    
    #region Level Generation Overrides
    
    /// <summary>
    /// 应用下水道特有的Painter装饰
    /// </summary>
    protected override void ApplyPainters()
    {
        base.ApplyPainters();
        
        // 使用SewerPainter添加下水道特色
        SewerPainter.Paint(this, rooms);
        
        Debug.Log("Applied Sewer painter decorations");
    }
    
    /// <summary>
    /// 添加下水道特有元素
    /// </summary>
    private void AddSewerFeatures()
    {
        AddWaterAreas();
        AddGrassAreas();
        AddSewerDecorations();
        
        Debug.Log("Added sewer-specific features");
    }
    
    /// <summary>
    /// 添加水域
    /// </summary>
    private void AddWaterAreas()
    {
        // 在一些房间中添加水
        foreach (Vector2Int roomCenter in roomPositions)
        {
            if (levelRandom.Next(100) < waterChance * 100)
            {
                // 在房间中创建小水池
                CreateWaterPool(roomCenter);
            }
        }
        
        // 在走廊中随机添加水坑
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (GetTerrain(x, y) == Terrain.Floor && levelRandom.Next(100) < waterChance * 50)
                {
                    SetTerrain(x, y, Terrain.Water);
                }
            }
        }
    }
    
    /// <summary>
    /// 创建水池
    /// </summary>
    /// <param name="center">中心位置</param>
    private void CreateWaterPool(Vector2Int center)
    {
        int poolSize = levelRandom.Next(1, 4); // 1-3格的水池
        
        for (int dx = -poolSize; dx <= poolSize; dx++)
        {
            for (int dy = -poolSize; dy <= poolSize; dy++)
            {
                Vector2Int pos = center + new Vector2Int(dx, dy);
                
                if (IsValidPosition(pos) && GetTerrain(pos) == Terrain.Floor)
                {
                    // 中心是深水，边缘是浅水
                    if (Mathf.Abs(dx) + Mathf.Abs(dy) <= 1)
                    {
                        SetTerrain(pos, Terrain.DeepWater);
                    }
                    else if (levelRandom.Next(100) < 70)
                    {
                        SetTerrain(pos, Terrain.Water);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 添加草地区域
    /// </summary>
    private void AddGrassAreas()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (GetTerrain(x, y) == Terrain.Floor)
                {
                    float random = levelRandom.Next(100) / 100f;
                    
                    if (random < highGrassChance)
                    {
                        SetTerrain(x, y, Terrain.HighGrass);
                    }
                    else if (random < grassChance)
                    {
                        SetTerrain(x, y, Terrain.Grass);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 添加下水道装饰
    /// </summary>
    private void AddSewerDecorations()
    {
        // 在一些位置添加水井
        int wellCount = levelRandom.Next(1, 3);
        for (int i = 0; i < wellCount; i++)
        {
            Vector2Int pos = GetRandomValidPosition();
            if (GetTerrain(pos) == Terrain.Floor)
            {
                SetTerrain(pos, Terrain.WellWater);
            }
        }
        
        // 添加一些路障（代表下水道的管道等）
        foreach (Vector2Int roomCenter in roomPositions)
        {
            if (levelRandom.Next(100) < 30) // 30%概率
            {
                Vector2Int barrierPos = roomCenter + new Vector2Int(
                    levelRandom.Next(-2, 3),
                    levelRandom.Next(-2, 3)
                );
                
                if (IsValidPosition(barrierPos) && GetTerrain(barrierPos) == Terrain.Floor)
                {
                    SetTerrain(barrierPos, Terrain.Barricade);
                }
            }
        }
    }
    
    #endregion
    
    #region Sewer-specific Methods
    
    /// <summary>
    /// 检查位置是否适合放置水
    /// </summary>
    /// <param name="pos">位置</param>
    /// <returns>是否适合</returns>
    private bool IsSuitableForWater(Vector2Int pos)
    {
        if (!IsValidPosition(pos) || GetTerrain(pos) != Terrain.Floor)
            return false;
        
        // 不要在入口出口附近放水
        if (Vector2Int.Distance(pos, entrancePos) < 2 || Vector2Int.Distance(pos, exitPos) < 2)
            return false;
        
        return true;
    }
    
    /// <summary>
    /// 获取下水道关卡的特殊信息
    /// </summary>
    /// <returns>特殊信息</returns>
    public string GetSewerInfo()
    {
        int waterCount = 0;
        int grassCount = 0;
        int highGrassCount = 0;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Terrain terrain = GetTerrain(x, y);
                switch (terrain)
                {
                    case Terrain.Water:
                    case Terrain.DeepWater:
                        waterCount++;
                        break;
                    case Terrain.Grass:
                        grassCount++;
                        break;
                    case Terrain.HighGrass:
                        highGrassCount++;
                        break;
                }
            }
        }
        
        return $"Sewer Level Stats:\n" +
               $"Water tiles: {waterCount}\n" +
               $"Grass tiles: {grassCount}\n" +
               $"High grass tiles: {highGrassCount}";
    }
    
    #endregion
    
    #region Overrides
    
    protected override void AdjustSizeForType(LevelType type)
    {
        // 下水道关卡相对简单
        width = 28;
        height = 28;
    }
    
    public override string GetDebugInfo()
    {
        return base.GetDebugInfo() + "\n" + GetSewerInfo();
    }
    
    #endregion
}
