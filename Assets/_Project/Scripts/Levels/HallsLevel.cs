using UnityEngine;

/// <summary>
/// 大厅关卡 (21-25层)
/// 特点：最复杂的布局、最多的装饰、终极挑战
/// </summary>
public class HallsLevel : RegularLevel
{
    [Header("大厅特殊设置")]
    // [SerializeField] private float grandDecorationChance = 0.6f; // 宏伟装饰概率（预留）
    [SerializeField] private float specialRoomChance = 0.3f;     // 特殊房间概率
    [SerializeField] private float treasureRoomChance = 0.2f;    // 宝藏房间概率
    
    #region Level Generation Overrides
    
    protected override void AdjustSizeForType(LevelType type)
    {
        // 大厅关卡最宏伟
        width = 48;
        height = 48;
        
        // 更少但更大的房间
        minRooms = 8;
        maxRooms = 12;
        minRoomSize = 8;
        maxRoomSize = 15;
    }
    
    public override bool Generate()
    {
        if (!base.Generate())
            return false;
        
        AddHallsFeatures();
        return true;
    }
    
    private void AddHallsFeatures()
    {
        AddGrandDecorations();
        AddSpecialRooms();
        AddTreasureRooms();
        AddHallsStructures();
        
        Debug.Log("Added halls-specific features");
    }
    
    /// <summary>
    /// 添加宏伟装饰
    /// </summary>
    private void AddGrandDecorations()
    {
        foreach (Vector2Int roomCenter in roomPositions)
        {
            CreateGrandRoom(roomCenter);
        }
    }
    
    /// <summary>
    /// 创建宏伟房间
    /// </summary>
    /// <param name="center">房间中心</param>
    private void CreateGrandRoom(Vector2Int center)
    {
        // 在房间中心放置特殊雕像
        if (GetTerrain(center) == Terrain.Floor)
        {
            SetTerrain(center, Terrain.StatueSP);
        }
        
        // 创建对称的装饰布局
        CreateSymmetricalDecorations(center);
        
        // 添加基座环绕
        CreatePedestalRing(center);
    }
    
    /// <summary>
    /// 创建对称装饰
    /// </summary>
    /// <param name="center">中心位置</param>
    private void CreateSymmetricalDecorations(Vector2Int center)
    {
        // 创建十字形装饰
        Vector2Int[] crossPositions = {
            center + Vector2Int.up * 2,
            center + Vector2Int.down * 2,
            center + Vector2Int.left * 2,
            center + Vector2Int.right * 2
        };
        
        foreach (Vector2Int pos in crossPositions)
        {
            if (IsValidPosition(pos) && GetTerrain(pos) == Terrain.Floor)
            {
                SetTerrain(pos, Terrain.Statue);
            }
        }
        
        // 创建对角装饰
        Vector2Int[] diagonalPositions = {
            center + new Vector2Int(3, 3),
            center + new Vector2Int(-3, 3),
            center + new Vector2Int(3, -3),
            center + new Vector2Int(-3, -3)
        };
        
        foreach (Vector2Int pos in diagonalPositions)
        {
            if (IsValidPosition(pos) && GetTerrain(pos) == Terrain.Floor)
            {
                if (levelRandom.Next(100) < 70)
                {
                    SetTerrain(pos, Terrain.Bookshelf);
                }
            }
        }
    }
    
    /// <summary>
    /// 创建基座环绕
    /// </summary>
    /// <param name="center">中心位置</param>
    private void CreatePedestalRing(Vector2Int center)
    {
        int radius = 4;
        
        for (int angle = 0; angle < 360; angle += 45)
        {
            float radian = angle * Mathf.Deg2Rad;
            int x = Mathf.RoundToInt(center.x + Mathf.Cos(radian) * radius);
            int y = Mathf.RoundToInt(center.y + Mathf.Sin(radian) * radius);
            
            Vector2Int pos = new Vector2Int(x, y);
            
            if (IsValidPosition(pos) && GetTerrain(pos) == Terrain.Floor)
            {
                if (levelRandom.Next(100) < 60)
                {
                    SetTerrain(pos, Terrain.Pedestal);
                }
            }
        }
    }
    
    /// <summary>
    /// 添加特殊房间
    /// </summary>
    private void AddSpecialRooms()
    {
        int specialRoomCount = Mathf.RoundToInt(roomPositions.Count * specialRoomChance);
        
        for (int i = 0; i < specialRoomCount && i < roomPositions.Count; i++)
        {
            Vector2Int roomCenter = roomPositions[i];
            CreateSpecialRoom(roomCenter);
        }
    }
    
    /// <summary>
    /// 创建特殊房间
    /// </summary>
    /// <param name="center">房间中心</param>
    private void CreateSpecialRoom(Vector2Int center)
    {
        // 随机选择特殊房间类型
        int roomType = levelRandom.Next(3);
        
        switch (roomType)
        {
            case 0:
                CreateThroneRoom(center);
                break;
            case 1:
                CreateRitualRoom(center);
                break;
            case 2:
                CreateGuardRoom(center);
                break;
        }
    }
    
    /// <summary>
    /// 创建王座房间
    /// </summary>
    /// <param name="center">中心位置</param>
    private void CreateThroneRoom(Vector2Int center)
    {
        // 在中心放置王座（特殊雕像）
        if (GetTerrain(center) == Terrain.Floor)
        {
            SetTerrain(center, Terrain.StatueSP);
        }
        
        // 在前方放置地毯效果（用装饰地形）
        for (int i = 1; i <= 3; i++)
        {
            Vector2Int carpetPos = center + Vector2Int.down * i;
            if (IsValidPosition(carpetPos) && GetTerrain(carpetPos) == Terrain.Floor)
            {
                SetTerrain(carpetPos, Terrain.Decoration);
            }
        }
        
        // 两侧放置护卫雕像
        Vector2Int leftGuard = center + Vector2Int.left * 2;
        Vector2Int rightGuard = center + Vector2Int.right * 2;
        
        if (IsValidPosition(leftGuard) && GetTerrain(leftGuard) == Terrain.Floor)
            SetTerrain(leftGuard, Terrain.Statue);
        if (IsValidPosition(rightGuard) && GetTerrain(rightGuard) == Terrain.Floor)
            SetTerrain(rightGuard, Terrain.Statue);
    }
    
    /// <summary>
    /// 创建仪式房间
    /// </summary>
    /// <param name="center">中心位置</param>
    private void CreateRitualRoom(Vector2Int center)
    {
        // 在中心放置炼金台
        if (GetTerrain(center) == Terrain.Floor)
        {
            SetTerrain(center, Terrain.Alchemy);
        }
        
        // 创建五芒星形状的基座
        Vector2Int[] pentagramPositions = {
            center + new Vector2Int(0, 3),    // 上
            center + new Vector2Int(3, 1),    // 右上
            center + new Vector2Int(2, -2),   // 右下
            center + new Vector2Int(-2, -2),  // 左下
            center + new Vector2Int(-3, 1)    // 左上
        };
        
        foreach (Vector2Int pos in pentagramPositions)
        {
            if (IsValidPosition(pos) && GetTerrain(pos) == Terrain.Floor)
            {
                SetTerrain(pos, Terrain.Pedestal);
            }
        }
    }
    
    /// <summary>
    /// 创建守卫房间
    /// </summary>
    /// <param name="center">中心位置</param>
    private void CreateGuardRoom(Vector2Int center)
    {
        // 创建雕像阵列
        for (int dx = -2; dx <= 2; dx += 2)
        {
            for (int dy = -2; dy <= 2; dy += 2)
            {
                if (dx == 0 && dy == 0) continue; // 跳过中心
                
                Vector2Int statuePos = center + new Vector2Int(dx, dy);
                if (IsValidPosition(statuePos) && GetTerrain(statuePos) == Terrain.Floor)
                {
                    SetTerrain(statuePos, Terrain.Statue);
                }
            }
        }
    }
    
    /// <summary>
    /// 添加宝藏房间
    /// </summary>
    private void AddTreasureRooms()
    {
        int treasureRoomCount = Mathf.RoundToInt(roomPositions.Count * treasureRoomChance);
        
        for (int i = 0; i < treasureRoomCount; i++)
        {
            if (i < roomPositions.Count)
            {
                Vector2Int roomCenter = roomPositions[roomPositions.Count - 1 - i];
                CreateTreasureRoom(roomCenter);
            }
        }
    }
    
    /// <summary>
    /// 创建宝藏房间
    /// </summary>
    /// <param name="center">房间中心</param>
    private void CreateTreasureRoom(Vector2Int center)
    {
        // 在中心放置特殊基座
        if (GetTerrain(center) == Terrain.Floor)
        {
            SetTerrain(center, Terrain.Pedestal);
        }
        
        // 周围放置宝箱效果（用雕像代表）
        Vector2Int[] chestPositions = {
            center + Vector2Int.up,
            center + Vector2Int.down,
            center + Vector2Int.left,
            center + Vector2Int.right
        };
        
        foreach (Vector2Int pos in chestPositions)
        {
            if (IsValidPosition(pos) && GetTerrain(pos) == Terrain.Floor)
            {
                if (levelRandom.Next(100) < 75)
                {
                    SetTerrain(pos, Terrain.StatueSP);
                }
            }
        }
    }
    
    /// <summary>
    /// 添加大厅结构
    /// </summary>
    private void AddHallsStructures()
    {
        // 创建宏伟的走廊
        CreateGrandCorridors();
        
        // 添加装饰柱子
        AddDecorativePillars();
    }
    
    /// <summary>
    /// 创建宏伟走廊
    /// </summary>
    private void CreateGrandCorridors()
    {
        // 在主要走廊中添加装饰
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (GetTerrain(x, y) == Terrain.Floor)
                {
                    // 检查是否是走廊（不在房间中心区域）
                    bool isInRoom = false;
                    foreach (Vector2Int roomCenter in roomPositions)
                    {
                        if (Vector2Int.Distance(new Vector2Int(x, y), roomCenter) < 4)
                        {
                            isInRoom = true;
                            break;
                        }
                    }
                    
                    if (!isInRoom && levelRandom.Next(100) < 15)
                    {
                        SetTerrain(x, y, Terrain.Decoration);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 添加装饰柱子
    /// </summary>
    private void AddDecorativePillars()
    {
        // 在走廊交叉点添加柱子
        for (int x = 4; x < width - 4; x += 6)
        {
            for (int y = 4; y < height - 4; y += 6)
            {
                if (GetTerrain(x, y) == Terrain.Floor)
                {
                    if (levelRandom.Next(100) < 40)
                    {
                        SetTerrain(x, y, Terrain.Statue);
                    }
                }
            }
        }
    }
    
    #endregion
    
    #region Halls-specific Methods
    
    /// <summary>
    /// 获取大厅关卡统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    public string GetHallsInfo()
    {
        int grandDecorationCount = 0;
        int specialStatueCount = 0;
        int alchemyCount = 0;
        int pedestalCount = 0;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Terrain terrain = GetTerrain(x, y);
                switch (terrain)
                {
                    case Terrain.Statue:
                    case Terrain.Bookshelf:
                    case Terrain.Decoration:
                        grandDecorationCount++;
                        break;
                    case Terrain.StatueSP:
                        specialStatueCount++;
                        break;
                    case Terrain.Alchemy:
                        alchemyCount++;
                        break;
                    case Terrain.Pedestal:
                        pedestalCount++;
                        break;
                }
            }
        }
        
        return $"Halls Level Stats:\n" +
               $"Grand decorations: {grandDecorationCount}\n" +
               $"Special statues: {specialStatueCount}\n" +
               $"Alchemy stations: {alchemyCount}\n" +
               $"Pedestals: {pedestalCount}";
    }
    
    #endregion
    
    #region Overrides
    
    public override string GetDebugInfo()
    {
        return base.GetDebugInfo() + "\n" + GetHallsInfo();
    }
    
    #endregion
}
