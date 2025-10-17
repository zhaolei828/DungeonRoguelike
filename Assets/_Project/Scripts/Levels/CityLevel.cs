using UnityEngine;

/// <summary>
/// 城市关卡 (16-20层)
/// 特点：更复杂的布局、更多装饰、城市风格
/// </summary>
public class CityLevel : RegularLevel
{
    [Header("城市特殊设置")]
    [SerializeField] private float decorationChance = 0.4f;  // 装饰概率
    [SerializeField] private float alchemyChance = 0.15f;    // 炼金台概率
    [SerializeField] private float crystalDoorChance = 0.2f; // 水晶门概率
    
    #region Level Generation Overrides
    
    protected override void AdjustSizeForType(LevelType type)
    {
        // 城市关卡最大最复杂
        width = 40;
        height = 40;
        
        // 更多更大的房间
        minRooms = 12;
        maxRooms = 18;
        minRoomSize = 5;
        maxRoomSize = 10;
    }
    
    public override bool Generate()
    {
        if (!base.Generate())
            return false;
        
        AddCityFeatures();
        return true;
    }
    
    private void AddCityFeatures()
    {
        AddCityDecorations();
        AddAlchemyLabs();
        AddCrystalDoors();
        AddCityStructures();
        
        Debug.Log("Added city-specific features");
    }
    
    /// <summary>
    /// 添加城市装饰
    /// </summary>
    private void AddCityDecorations()
    {
        // 在每个房间中添加丰富的装饰
        foreach (Vector2Int roomCenter in roomPositions)
        {
            AddRoomDecorations(roomCenter);
        }
    }
    
    /// <summary>
    /// 为房间添加装饰
    /// </summary>
    /// <param name="roomCenter">房间中心</param>
    private void AddRoomDecorations(Vector2Int roomCenter)
    {
        // 在房间中添加多种装饰
        for (int i = 0; i < 5; i++) // 每个房间最多5个装饰
        {
            if (levelRandom.Next(100) < decorationChance * 100)
            {
                Vector2Int decorPos = roomCenter + new Vector2Int(
                    levelRandom.Next(-3, 4),
                    levelRandom.Next(-3, 4)
                );
                
                if (IsValidPosition(decorPos) && GetTerrain(decorPos) == Terrain.Floor)
                {
                    Terrain decoration = GetRandomCityDecoration();
                    SetTerrain(decorPos, decoration);
                }
            }
        }
    }
    
    /// <summary>
    /// 获取随机城市装饰
    /// </summary>
    /// <returns>装饰类型</returns>
    private Terrain GetRandomCityDecoration()
    {
        Terrain[] cityDecorations = {
            Terrain.Statue,
            Terrain.StatueSP,
            Terrain.Bookshelf,
            Terrain.Pedestal,
            Terrain.Sign
        };
        
        return cityDecorations[levelRandom.Next(cityDecorations.Length)];
    }
    
    /// <summary>
    /// 添加炼金实验室
    /// </summary>
    private void AddAlchemyLabs()
    {
        int labCount = Mathf.RoundToInt(roomPositions.Count * alchemyChance);
        
        for (int i = 0; i < labCount && i < roomPositions.Count; i++)
        {
            Vector2Int labCenter = roomPositions[i];
            CreateAlchemyLab(labCenter);
        }
    }
    
    /// <summary>
    /// 创建炼金实验室
    /// </summary>
    /// <param name="center">实验室中心</param>
    private void CreateAlchemyLab(Vector2Int center)
    {
        // 放置炼金台
        if (GetTerrain(center) == Terrain.Floor)
        {
            SetTerrain(center, Terrain.Alchemy);
        }
        
        // 在周围放置书架和基座
        Vector2Int[] positions = {
            center + Vector2Int.up,
            center + Vector2Int.down,
            center + Vector2Int.left,
            center + Vector2Int.right
        };
        
        foreach (Vector2Int pos in positions)
        {
            if (IsValidPosition(pos) && GetTerrain(pos) == Terrain.Floor)
            {
                if (levelRandom.Next(100) < 60)
                {
                    Terrain furniture = levelRandom.Next(100) < 70 ? Terrain.Bookshelf : Terrain.Pedestal;
                    SetTerrain(pos, furniture);
                }
            }
        }
    }
    
    /// <summary>
    /// 添加水晶门
    /// </summary>
    private void AddCrystalDoors()
    {
        // 将一些普通门替换为水晶门
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (GetTerrain(x, y) == Terrain.DoorClosed)
                {
                    if (levelRandom.Next(100) < crystalDoorChance * 100)
                    {
                        SetTerrain(x, y, Terrain.CrystalDoor);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 添加城市结构
    /// </summary>
    private void AddCityStructures()
    {
        // 创建一些特殊的城市结构
        CreatePlazas();
        CreateLibraries();
    }
    
    /// <summary>
    /// 创建广场
    /// </summary>
    private void CreatePlazas()
    {
        // 在一些大房间中创建广场效果
        foreach (Vector2Int roomCenter in roomPositions)
        {
            if (levelRandom.Next(100) < 30) // 30%概率
            {
                CreatePlaza(roomCenter);
            }
        }
    }
    
    /// <summary>
    /// 创建广场
    /// </summary>
    /// <param name="center">广场中心</param>
    private void CreatePlaza(Vector2Int center)
    {
        // 在中心放置特殊雕像
        if (GetTerrain(center) == Terrain.Floor)
        {
            SetTerrain(center, Terrain.StatueSP);
        }
        
        // 在四角放置装饰
        Vector2Int[] corners = {
            center + new Vector2Int(2, 2),
            center + new Vector2Int(-2, 2),
            center + new Vector2Int(2, -2),
            center + new Vector2Int(-2, -2)
        };
        
        foreach (Vector2Int corner in corners)
        {
            if (IsValidPosition(corner) && GetTerrain(corner) == Terrain.Floor)
            {
                SetTerrain(corner, Terrain.Statue);
            }
        }
    }
    
    /// <summary>
    /// 创建图书馆
    /// </summary>
    private void CreateLibraries()
    {
        int libraryCount = levelRandom.Next(1, 3);
        
        for (int i = 0; i < libraryCount && i < roomPositions.Count; i++)
        {
            Vector2Int libraryCenter = roomPositions[roomPositions.Count - 1 - i]; // 从后面选择
            CreateLibrary(libraryCenter);
        }
    }
    
    /// <summary>
    /// 创建图书馆
    /// </summary>
    /// <param name="center">图书馆中心</param>
    private void CreateLibrary(Vector2Int center)
    {
        // 在房间中放置大量书架
        for (int dx = -3; dx <= 3; dx++)
        {
            for (int dy = -3; dy <= 3; dy++)
            {
                Vector2Int pos = center + new Vector2Int(dx, dy);
                
                if (IsValidPosition(pos) && GetTerrain(pos) == Terrain.Floor)
                {
                    if (levelRandom.Next(100) < 70) // 70%概率放置书架
                    {
                        SetTerrain(pos, Terrain.Bookshelf);
                    }
                }
            }
        }
    }
    
    #endregion
    
    #region City-specific Methods
    
    /// <summary>
    /// 获取城市关卡统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    public string GetCityInfo()
    {
        int decorationCount = 0;
        int alchemyCount = 0;
        int crystalDoorCount = 0;
        int bookshelfCount = 0;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Terrain terrain = GetTerrain(x, y);
                switch (terrain)
                {
                    case Terrain.Statue:
                    case Terrain.StatueSP:
                    case Terrain.Pedestal:
                    case Terrain.Sign:
                        decorationCount++;
                        break;
                    case Terrain.Alchemy:
                        alchemyCount++;
                        break;
                    case Terrain.CrystalDoor:
                        crystalDoorCount++;
                        break;
                    case Terrain.Bookshelf:
                        bookshelfCount++;
                        break;
                }
            }
        }
        
        return $"City Level Stats:\n" +
               $"Decorations: {decorationCount}\n" +
               $"Alchemy labs: {alchemyCount}\n" +
               $"Crystal doors: {crystalDoorCount}\n" +
               $"Bookshelves: {bookshelfCount}";
    }
    
    #endregion
    
    #region Overrides
    
    public override string GetDebugInfo()
    {
        return base.GetDebugInfo() + "\n" + GetCityInfo();
    }
    
    #endregion
}
