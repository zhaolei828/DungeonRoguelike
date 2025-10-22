using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 常规关卡
/// 使用标准的房间+走廊生成算法
/// </summary>
public class RegularLevel : Level
{
    [Header("常规关卡设置")]
    [SerializeField] protected int minRooms = 6;
    [SerializeField] protected int maxRooms = 12;
    [SerializeField] protected int minRoomSize = 4;
    [SerializeField] protected int maxRoomSize = 8;
    
    // 生成相关
    protected System.Random levelRandom;
    
    #region Level Generation
    
    /// <summary>
    /// 生成关卡（新版：使用Builder系统）
    /// </summary>
    /// <returns>是否生成成功</returns>
    public override bool Generate()
    {
        if (!IsInitialized)
        {
            Debug.LogError("Level not initialized before generation");
            return false;
        }
        
        Debug.Log($"Generating RegularLevel: Depth {depth}, Type {levelType}");
        
        // 初始化随机数生成器
        InitializeRandom();
        
        // 步骤1: 初始化地图数组（全部为墙壁）
        FillWithWalls();
        
        // 步骤2: 选择Builder并生成房间布局
        ILevelBuilder builder = CreateBuilder();
        rooms = builder.Build(this);
        
        Debug.Log($"Builder generated {rooms.Count} rooms");
        
        // 步骤3: 绘制房间到地图
        PaintRooms();
        
        // 步骤4: 应用Painter进行装饰
        ApplyPainters();
        
        // 步骤5: 后处理
        PostProcess();
        
        // 标记生成完成
        MarkAsGenerated();
        
        return true;
    }
    
    /// <summary>
    /// 初始化随机数生成器
    /// </summary>
    private void InitializeRandom()
    {
        // 使用深度作为种子，确保相同深度生成相同关卡
        int seed = depth * 1000 + (int)levelType * 100;
        levelRandom = new System.Random(seed);
        
        Debug.Log($"Level seed: {seed}");
    }
    
    /// <summary>
    /// 用墙壁填充整个地图
    /// </summary>
    protected void FillWithWalls()
    {
        Map = new Terrain[width * height];
        for (int i = 0; i < Map.Length; i++)
        {
            Map[i] = Terrain.Wall;
        }
    }
    
    /// <summary>
    /// 创建Builder（子类可重写以使用不同的Builder）
    /// </summary>
    protected virtual ILevelBuilder CreateBuilder()
    {
        return new LoopBuilder(minRooms, maxRooms, minRoomSize, maxRoomSize);
    }
    
    /// <summary>
    /// 绘制所有房间
    /// </summary>
    protected void PaintRooms()
    {
        Debug.Log($"[PaintRooms] Painting {rooms.Count} rooms:");
        
        foreach (Room room in rooms)
        {
            Debug.Log($"  {room.GetRoomType()}: bounds({room.left},{room.top},{room.right},{room.bottom}) = {room.Width}x{room.Height}");
            bool success = room.Paint(this);
            if (!success)
            {
                Debug.LogWarning($"  Failed to paint {room.GetRoomType()}!");
            }
        }
    }
    
    /// <summary>
    /// 应用装饰Painter（子类可重写以添加区域特色）
    /// </summary>
    protected virtual void ApplyPainters()
    {
        // 基础版本不添加任何装饰
        // 子类（如SewerLevel）会重写此方法添加特定装饰
    }
    
    /// <summary>
    /// 生成基础结构
    /// </summary>
    /// <returns>是否成功</returns>
    private bool GenerateBasicStructure()
    {
        // 创建边界墙壁（已经在ClearTerrain中完成）
        
        // 在边界内创建一些基础地板区域
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                // 随机创建一些地板，为后续房间生成做准备
                if (levelRandom.Next(100) < 10) // 10%概率
                {
                    SetTerrain(x, y, Terrain.Floor);
                }
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// 生成房间
    /// </summary>
    /// <returns>是否成功</returns>
    private bool GenerateRooms()
    {
        roomPositions.Clear();
        
        int roomCount = levelRandom.Next(minRooms, maxRooms + 1);
        int attempts = 0;
        int maxAttempts = roomCount * 10;
        
        while (roomPositions.Count < roomCount && attempts < maxAttempts)
        {
            attempts++;
            
            // 随机房间尺寸
            int roomWidth = levelRandom.Next(minRoomSize, maxRoomSize + 1);
            int roomHeight = levelRandom.Next(minRoomSize, maxRoomSize + 1);
            
            // 随机房间位置
            int roomX = levelRandom.Next(2, width - roomWidth - 2);
            int roomY = levelRandom.Next(2, height - roomHeight - 2);
            
            // 检查是否与现有房间重叠
            if (IsRoomPositionValid(roomX, roomY, roomWidth, roomHeight))
            {
                // 创建房间
                CreateRoom(roomX, roomY, roomWidth, roomHeight);
                roomPositions.Add(new Vector2Int(roomX + roomWidth / 2, roomY + roomHeight / 2));
            }
        }
        
        Debug.Log($"Generated {roomPositions.Count} rooms");
        return roomPositions.Count >= minRooms;
    }
    
    /// <summary>
    /// 检查房间位置是否有效
    /// </summary>
    /// <param name="x">房间X坐标</param>
    /// <param name="y">房间Y坐标</param>
    /// <param name="w">房间宽度</param>
    /// <param name="h">房间高度</param>
    /// <returns>是否有效</returns>
    private bool IsRoomPositionValid(int x, int y, int w, int h)
    {
        // 检查边界
        if (x < 1 || y < 1 || x + w >= width - 1 || y + h >= height - 1)
            return false;
        
        // 检查与现有房间的重叠（包括1格缓冲区）
        for (int checkX = x - 1; checkX <= x + w; checkX++)
        {
            for (int checkY = y - 1; checkY <= y + h; checkY++)
            {
                if (IsValidPosition(checkX, checkY) && GetTerrain(checkX, checkY) == Terrain.Floor)
                {
                    return false;
                }
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// 创建房间
    /// </summary>
    /// <param name="x">房间X坐标</param>
    /// <param name="y">房间Y坐标</param>
    /// <param name="w">房间宽度</param>
    /// <param name="h">房间高度</param>
    private void CreateRoom(int x, int y, int w, int h)
    {
        for (int roomX = x; roomX < x + w; roomX++)
        {
            for (int roomY = y; roomY < y + h; roomY++)
            {
                SetTerrain(roomX, roomY, Terrain.Floor);
            }
        }
        
        Debug.Log($"Created room at ({x}, {y}) size {w}x{h}");
    }
    
    /// <summary>
    /// 生成走廊
    /// </summary>
    /// <returns>是否成功</returns>
    private bool GenerateCorridors()
    {
        if (roomPositions.Count < 2)
            return true; // 只有一个房间不需要走廊
        
        // 连接所有房间
        for (int i = 0; i < roomPositions.Count - 1; i++)
        {
            Vector2Int from = roomPositions[i];
            Vector2Int to = roomPositions[i + 1];
            
            CreateCorridor(from, to);
        }
        
        // 创建一些额外的连接以增加复杂性
        int extraConnections = levelRandom.Next(1, 3);
        for (int i = 0; i < extraConnections && roomPositions.Count > 2; i++)
        {
            int fromIndex = levelRandom.Next(roomPositions.Count);
            int toIndex = levelRandom.Next(roomPositions.Count);
            
            if (fromIndex != toIndex)
            {
                CreateCorridor(roomPositions[fromIndex], roomPositions[toIndex]);
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// 创建走廊
    /// </summary>
    /// <param name="from">起始位置</param>
    /// <param name="to">目标位置</param>
    private void CreateCorridor(Vector2Int from, Vector2Int to)
    {
        Vector2Int current = from;
        
        // 先水平移动
        while (current.x != to.x)
        {
            if (GetTerrain(current) == Terrain.Wall)
            {
                SetTerrain(current, Terrain.Floor);
            }
            
            current.x += current.x < to.x ? 1 : -1;
        }
        
        // 再垂直移动
        while (current.y != to.y)
        {
            if (GetTerrain(current) == Terrain.Wall)
            {
                SetTerrain(current, Terrain.Floor);
            }
            
            current.y += current.y < to.y ? 1 : -1;
        }
        
        // 确保目标位置也是地板
        if (GetTerrain(to) == Terrain.Wall)
        {
            SetTerrain(to, Terrain.Floor);
        }
    }
    
    /// <summary>
    /// 放置入口和出口
    /// </summary>
    private void PlaceEntranceAndExit()
    {
        if (roomPositions.Count == 0)
        {
            // 如果没有房间，在中心放置
            SetEntrancePosition(new Vector2Int(width / 2, height / 2));
            SetExitPosition(new Vector2Int(width / 2 + 1, height / 2));
            return;
        }
        
        // 入口放在第一个房间
        Vector2Int entrance = roomPositions[0];
        SetEntrancePosition(entrance);
        
        // 出口放在最后一个房间
        Vector2Int exit = roomPositions[roomPositions.Count - 1];
        SetExitPosition(exit);
        
        Debug.Log($"Placed entrance at {entrance}, exit at {exit}");
    }
    
    /// <summary>
    /// 后处理
    /// </summary>
    private void PostProcess()
    {
        // 1. 验证地图连通性
        ValidateAndFixConnectivity();
        
        // 2. 添加一些装饰性元素
        AddDecorations();
        
        // 3. 清理孤立的地板
        CleanupIsolatedFloors();
    }
    
    /// <summary>
    /// 验证并修复地图连通性
    /// </summary>
    private void ValidateAndFixConnectivity()
    {
        Debug.Log("<color=yellow>━━━ 开始验证地图连通性 ━━━</color>");
        
        // 查找入口和出口房间
        Room entranceRoom = null;
        Room exitRoom = null;
        
        foreach (Room room in rooms)
        {
            if (room is EntranceRoom)
                entranceRoom = room;
            else if (room is ExitRoom)
                exitRoom = room;
        }
        
        if (entranceRoom == null || exitRoom == null)
        {
            Debug.LogWarning("未找到入口或出口房间，跳过连通性验证");
            return;
        }
        
        // 获取入口和出口的中心位置
        Vector2Int entrancePos = new Vector2Int(
            (entranceRoom.left + entranceRoom.right) / 2,
            (entranceRoom.top + entranceRoom.bottom) / 2
        );
        
        Vector2Int exitPos = new Vector2Int(
            (exitRoom.left + exitRoom.right) / 2,
            (exitRoom.top + exitRoom.bottom) / 2
        );
        
        // 确保入口和出口位置可通行
        if (!IsPassable(entrancePos))
        {
            SetTerrain(entrancePos, Terrain.Floor);
            Debug.Log($"修复入口位置 {entrancePos} 为可通行");
        }
        
        if (!IsPassable(exitPos))
        {
            SetTerrain(exitPos, Terrain.Floor);
            Debug.Log($"修复出口位置 {exitPos} 为可通行");
        }
        
        // 设置入口和出口
        SetEntrancePosition(entrancePos);
        SetExitPosition(exitPos);
        
        // 验证连通性
        ConnectivityValidation validation = PathFinder.ValidateConnectivity(this, entrancePos, exitPos, 0.7f);
        
        Debug.Log($"<color=cyan>{validation}</color>");
        
        if (!validation.isValid)
        {
            Debug.LogWarning($"<color=red>地图连通性验证失败：{validation.errorMessage}</color>");
            
            // 尝试修复
            if (!validation.entranceToExitReachable)
            {
                Debug.Log("尝试连接入口和出口...");
                ConnectTwoPoints(entrancePos, exitPos);
                
                // 重新验证
                validation = PathFinder.ValidateConnectivity(this, entrancePos, exitPos, 0.7f);
                Debug.Log($"修复后验证结果：{validation.isValid}");
            }
            
            // 如果还有孤立区域，尝试连接它们
            if (validation.isolatedRegionCount > 1)
            {
                Debug.Log($"发现 {validation.isolatedRegionCount} 个孤立区域，尝试连接...");
                ConnectIsolatedRegions(validation.isolatedRegions);
                
                // 最终验证
                validation = PathFinder.ValidateConnectivity(this, entrancePos, exitPos, 0.7f);
                Debug.Log($"<color=yellow>最终验证结果：{validation.isValid}</color>");
            }
        }
        else
        {
            Debug.Log("<color=green>✓ 地图连通性验证通过！</color>");
        }
        
        Debug.Log("<color=yellow>━━━ 连通性验证完成 ━━━</color>");
    }
    
    /// <summary>
    /// 连接两个点，确保它们之间有通路
    /// </summary>
    private void ConnectTwoPoints(Vector2Int from, Vector2Int to)
    {
        Vector2Int current = from;
        
        // L型路径：先水平后垂直
        while (current.x != to.x)
        {
            SetTerrain(current, Terrain.Floor);
            current.x += current.x < to.x ? 1 : -1;
        }
        
        while (current.y != to.y)
        {
            SetTerrain(current, Terrain.Floor);
            current.y += current.y < to.y ? 1 : -1;
        }
        
        SetTerrain(to, Terrain.Floor);
        
        Debug.Log($"创建连接路径：{from} -> {to}");
    }
    
    /// <summary>
    /// 连接所有孤立区域
    /// </summary>
    private void ConnectIsolatedRegions(List<List<Vector2Int>> regions)
    {
        if (regions.Count <= 1)
            return;
        
        // 将最大的区域作为主区域
        List<Vector2Int> mainRegion = regions[0];
        foreach (var region in regions)
        {
            if (region.Count > mainRegion.Count)
                mainRegion = region;
        }
        
        // 将其他区域连接到主区域
        foreach (var region in regions)
        {
            if (region == mainRegion)
                continue;
            
            // 找到该区域中离主区域最近的点
            Vector2Int closestInRegion = region[0];
            Vector2Int closestInMain = mainRegion[0];
            float minDistance = float.MaxValue;
            
            foreach (var pointInRegion in region)
            {
                foreach (var pointInMain in mainRegion)
                {
                    float distance = Vector2Int.Distance(pointInRegion, pointInMain);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestInRegion = pointInRegion;
                        closestInMain = pointInMain;
                    }
                }
            }
            
            // 连接这两个点
            ConnectTwoPoints(closestInRegion, closestInMain);
            Debug.Log($"连接孤立区域：{closestInRegion} -> {closestInMain}");
        }
    }
    
    /// <summary>
    /// 添加装饰
    /// </summary>
    private void AddDecorations()
    {
        // 随机在房间中添加一些装饰
        foreach (Vector2Int roomCenter in roomPositions)
        {
            // 在房间中心附近随机放置装饰
            for (int i = 0; i < 3; i++)
            {
                int decorX = roomCenter.x + levelRandom.Next(-2, 3);
                int decorY = roomCenter.y + levelRandom.Next(-2, 3);
                
                if (IsValidPosition(decorX, decorY) && GetTerrain(decorX, decorY) == Terrain.Floor)
                {
                    if (levelRandom.Next(100) < 20) // 20%概率
                    {
                        // 随机选择装饰类型
                        Terrain decoration = levelRandom.Next(3) switch
                        {
                            0 => Terrain.Statue,
                            1 => Terrain.Bookshelf,
                            _ => Terrain.Sign
                        };
                        
                        SetTerrain(decorX, decorY, decoration);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 清理孤立的地板
    /// </summary>
    private void CleanupIsolatedFloors()
    {
        // 这里可以实现清理逻辑
        // 移除没有连接到主要区域的孤立地板
        // 为了简化，暂时跳过
    }
    
    #endregion
    
    #region Overrides
    
    protected override void AdjustSizeForType(LevelType type)
    {
        // 根据关卡类型调整参数
        switch (type)
        {
            case LevelType.Sewers:
                minRooms = 6;
                maxRooms = 10;
                minRoomSize = 4;
                maxRoomSize = 7;
                break;
                
            case LevelType.Prison:
                minRooms = 8;
                maxRooms = 12;
                minRoomSize = 3;
                maxRoomSize = 6;
                break;
                
            case LevelType.Caves:
                minRooms = 5;
                maxRooms = 9;
                minRoomSize = 5;
                maxRoomSize = 9;
                break;
                
            case LevelType.City:
                minRooms = 10;
                maxRooms = 15;
                minRoomSize = 4;
                maxRoomSize = 8;
                break;
                
            case LevelType.Halls:
                minRooms = 4;
                maxRooms = 8;
                minRoomSize = 6;
                maxRoomSize = 12;
                break;
        }
        
        base.AdjustSizeForType(type);
    }
    
    #endregion
}
