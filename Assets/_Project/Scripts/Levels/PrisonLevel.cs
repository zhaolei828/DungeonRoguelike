using UnityEngine;

/// <summary>
/// 监狱关卡 (6-10层)
/// 特点：更多的门、牢房、更复杂的布局
/// </summary>
public class PrisonLevel : RegularLevel
{
    [Header("监狱特殊设置")]
    [SerializeField] private float doorChance = 0.3f;        // 门的生成概率
    [SerializeField] private float lockedDoorChance = 0.1f;  // 锁定门的概率
    [SerializeField] private float cellChance = 0.4f;        // 牢房的概率
    
    #region Level Generation Overrides
    
    public override bool Generate()
    {
        if (!base.Generate())
            return false;
        
        AddPrisonFeatures();
        return true;
    }
    
    private void AddPrisonFeatures()
    {
        AddDoors();
        AddPrisonCells();
        AddPrisonDecorations();
        
        Debug.Log("Added prison-specific features");
    }
    
    /// <summary>
    /// 添加门
    /// </summary>
    private void AddDoors()
    {
        // 在房间入口添加门
        foreach (Vector2Int roomCenter in roomPositions)
        {
            AddDoorsToRoom(roomCenter);
        }
    }
    
    /// <summary>
    /// 为房间添加门
    /// </summary>
    /// <param name="roomCenter">房间中心</param>
    private void AddDoorsToRoom(Vector2Int roomCenter)
    {
        // 查找房间边界上的地板（潜在的门位置）
        for (int dx = -4; dx <= 4; dx++)
        {
            for (int dy = -4; dy <= 4; dy++)
            {
                Vector2Int pos = roomCenter + new Vector2Int(dx, dy);
                
                if (IsValidPosition(pos) && GetTerrain(pos) == Terrain.Floor)
                {
                    // 检查是否是房间入口（一边是墙，另一边是地板）
                    if (IsPotentialDoorPosition(pos))
                    {
                        if (levelRandom.Next(100) < doorChance * 100)
                        {
                            Terrain doorType = levelRandom.Next(100) < lockedDoorChance * 100 
                                ? Terrain.LockedDoor 
                                : Terrain.DoorClosed;
                            
                            SetTerrain(pos, doorType);
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 检查是否是潜在的门位置
    /// </summary>
    /// <param name="pos">位置</param>
    /// <returns>是否适合放门</returns>
    private bool IsPotentialDoorPosition(Vector2Int pos)
    {
        // 检查四个方向，如果有墙壁相邻且有通道，则适合放门
        Vector2Int[] directions = LevelCoord.Directions4;
        
        int wallCount = 0;
        int floorCount = 0;
        
        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = pos + dir;
            if (IsValidPosition(checkPos))
            {
                Terrain terrain = GetTerrain(checkPos);
                if (terrain == Terrain.Wall)
                    wallCount++;
                else if (terrain == Terrain.Floor)
                    floorCount++;
            }
        }
        
        // 门应该连接两个区域
        return wallCount >= 2 && floorCount >= 2;
    }
    
    /// <summary>
    /// 添加监狱牢房
    /// </summary>
    private void AddPrisonCells()
    {
        // 将一些房间改造成牢房
        int cellCount = Mathf.RoundToInt(roomPositions.Count * cellChance);
        
        for (int i = 0; i < cellCount && i < roomPositions.Count; i++)
        {
            Vector2Int cellCenter = roomPositions[i];
            CreatePrisonCell(cellCenter);
        }
    }
    
    /// <summary>
    /// 创建监狱牢房
    /// </summary>
    /// <param name="center">牢房中心</param>
    private void CreatePrisonCell(Vector2Int center)
    {
        // 在牢房中添加一些特殊元素
        
        // 添加床（用雕像代表）
        Vector2Int bedPos = center + new Vector2Int(
            levelRandom.Next(-1, 2),
            levelRandom.Next(-1, 2)
        );
        
        if (IsValidPosition(bedPos) && GetTerrain(bedPos) == Terrain.Floor)
        {
            SetTerrain(bedPos, Terrain.Statue); // 用雕像代表床
        }
        
        // 可能添加一些书架
        if (levelRandom.Next(100) < 30)
        {
            Vector2Int bookPos = center + new Vector2Int(
                levelRandom.Next(-2, 3),
                levelRandom.Next(-2, 3)
            );
            
            if (IsValidPosition(bookPos) && GetTerrain(bookPos) == Terrain.Floor)
            {
                SetTerrain(bookPos, Terrain.Bookshelf);
            }
        }
    }
    
    /// <summary>
    /// 添加监狱装饰
    /// </summary>
    private void AddPrisonDecorations()
    {
        // 添加更多的雕像（代表各种监狱设施）
        foreach (Vector2Int roomCenter in roomPositions)
        {
            if (levelRandom.Next(100) < 40)
            {
                Vector2Int decorPos = roomCenter + new Vector2Int(
                    levelRandom.Next(-2, 3),
                    levelRandom.Next(-2, 3)
                );
                
                if (IsValidPosition(decorPos) && GetTerrain(decorPos) == Terrain.Floor)
                {
                    SetTerrain(decorPos, Terrain.StatueSP);
                }
            }
        }
        
        // 添加一些路障（代表监狱的栅栏等）
        int barrierCount = levelRandom.Next(2, 5);
        for (int i = 0; i < barrierCount; i++)
        {
            Vector2Int pos = GetRandomValidPosition();
            if (GetTerrain(pos) == Terrain.Floor)
            {
                SetTerrain(pos, Terrain.Barricade);
            }
        }
    }
    
    #endregion
    
    #region Prison-specific Methods
    
    /// <summary>
    /// 获取监狱关卡统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    public string GetPrisonInfo()
    {
        int doorCount = 0;
        int lockedDoorCount = 0;
        int cellFurnitureCount = 0;
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Terrain terrain = GetTerrain(x, y);
                switch (terrain)
                {
                    case Terrain.DoorClosed:
                    case Terrain.DoorOpen:
                        doorCount++;
                        break;
                    case Terrain.LockedDoor:
                        lockedDoorCount++;
                        break;
                    case Terrain.Statue:
                    case Terrain.StatueSP:
                    case Terrain.Bookshelf:
                        cellFurnitureCount++;
                        break;
                }
            }
        }
        
        return $"Prison Level Stats:\n" +
               $"Doors: {doorCount}\n" +
               $"Locked doors: {lockedDoorCount}\n" +
               $"Cell furniture: {cellFurnitureCount}";
    }
    
    #endregion
    
    #region Overrides
    
    protected override void AdjustSizeForType(LevelType type)
    {
        // 监狱关卡稍大一些，更复杂
        width = 32;
        height = 32;
    }
    
    public override string GetDebugInfo()
    {
        return base.GetDebugInfo() + "\n" + GetPrisonInfo();
    }
    
    #endregion
}
