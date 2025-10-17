using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 循环生成器
/// Shattered Pixel Dungeon的核心地牢生成算法
/// 创建带有循环路径的复杂地牢布局
/// </summary>
public class LoopBuilder : ILevelBuilder
{
    // 生成参数
    private int minRooms = 6;
    private int maxRooms = 12;
    private int minRoomSize = 4;
    private int maxRoomSize = 8;
    private float loopChance = 0.3f; // 30%循环概率
    
    // 生成状态
    private List<Room> rooms;
    private Level level;
    private System.Random random;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    public LoopBuilder()
    {
        random = new System.Random();
    }
    
    /// <summary>
    /// 构造函数（自定义参数）
    /// </summary>
    public LoopBuilder(int minRooms, int maxRooms, int minSize, int maxSize, float loopChance = 0.3f)
    {
        this.minRooms = minRooms;
        this.maxRooms = maxRooms;
        this.minRoomSize = minSize;
        this.maxRoomSize = maxSize;
        this.loopChance = loopChance;
        random = new System.Random();
    }
    
    /// <summary>
    /// 主生成方法
    /// </summary>
    public List<Room> Build(Level level)
    {
        this.level = level;
        rooms = new List<Room>();
        
        // 1. 初始化房间
        InitRooms();
        
        // 2. 放置房间
        PlaceRooms();
        
        // 3. 创建主路径
        CreateMainPath();
        
        // 4. 创建循环
        CreateLoops();
        
        // 5. 连接房间
        ConnectRooms();
        
        // 6. 清理孤立房间
        Cleanup();
        
        return rooms;
    }
    
    /// <summary>
    /// 步骤1: 初始化入口和出口房间
    /// </summary>
    private void InitRooms()
    {
        // 确保有足够的空间
        int safeMaxX = Mathf.Max(3, level.Width - 10);
        int safeMaxY = Mathf.Max(3, level.Height - 10);
        
        // 创建入口房间
        EntranceRoom entrance = new EntranceRoom();
        int entranceX = random.Next(2, safeMaxX);
        int entranceY = random.Next(2, safeMaxY);
        entrance.SetPos(entranceX, entranceY);
        entrance.SetSize();
        rooms.Add(entrance);
        
        // 创建出口房间
        ExitRoom exit = new ExitRoom();
        // 尝试将出口放在远离入口的位置
        int attempts = 0;
        do
        {
            int exitX = random.Next(2, safeMaxX);
            int exitY = random.Next(2, safeMaxY);
            exit.SetPos(exitX, exitY);
            exit.SetSize();
            attempts++;
        } while (attempts < 50 && (Vector2Int.Distance(entrance.Center, exit.Center) < 15 || exit.Intersect(entrance)));
        
        rooms.Add(exit);
        
        Debug.Log($"[InitRooms] Entrance: pos({entrance.left},{entrance.top}) bounds({entrance.left},{entrance.top},{entrance.right},{entrance.bottom}) size {entrance.Width}x{entrance.Height}");
        Debug.Log($"[InitRooms] Exit: pos({exit.left},{exit.top}) bounds({exit.left},{exit.top},{exit.right},{exit.bottom}) size {exit.Width}x{exit.Height}");
    }
    
    /// <summary>
    /// 步骤2: 放置标准房间
    /// </summary>
    private void PlaceRooms()
    {
        int roomsToPlace = random.Next(minRooms, maxRooms + 1);
        int attempts = 0;
        int maxAttempts = 500;
        
        // 确保安全边距计算有效
        int safeMargin = maxRoomSize + 2;
        int maxX = Mathf.Max(2, level.Width - safeMargin);
        int maxY = Mathf.Max(2, level.Height - safeMargin);
        
        while (rooms.Count < roomsToPlace + 2 && attempts < maxAttempts) // +2 for entrance and exit
        {
            StandardRoom newRoom = new StandardRoom(minRoomSize, maxRoomSize);
            
            // 先设置位置（使用安全边距）
            int posX = random.Next(1, maxX);
            int posY = random.Next(1, maxY);
            
            newRoom.SetPos(posX, posY);
            
            // 再设置大小（这会正确计算right和bottom）
            newRoom.SetSize();
            
            // 检查是否与现有房间重叠
            bool canPlace = true;
            foreach (Room existing in rooms)
            {
                if (newRoom.Intersect(existing))
                {
                    canPlace = false;
                    break;
                }
            }
            
            if (canPlace)
            {
                rooms.Add(newRoom);
            }
            
            attempts++;
        }
        
        Debug.Log($"[PlaceRooms] Placed {rooms.Count} rooms total");
    }
    
    /// <summary>
    /// 步骤3: 创建从入口到出口的主路径
    /// </summary>
    private void CreateMainPath()
    {
        if (rooms.Count < 2) return;
        
        Room entrance = rooms[0];
        Room exit = rooms[1];
        
        // 使用简化的路径查找
        List<Room> path = FindPath(entrance, exit);
        
        // 连接路径中的相邻房间
        for (int i = 0; i < path.Count - 1; i++)
        {
            ConnectTwoRooms(path[i], path[i + 1]);
        }
    }
    
    /// <summary>
    /// 步骤4: 创建循环连接
    /// </summary>
    private void CreateLoops()
    {
        // 先收集所有要添加的走廊房间，避免在遍历时修改集合
        List<TunnelRoom> newTunnels = new List<TunnelRoom>();
        
        // 创建rooms的副本用于遍历
        List<Room> roomsCopy = new List<Room>(rooms);
        
        foreach (Room room in roomsCopy)
        {
            // 查找附近的房间
            List<Room> nearby = FindNearbyRooms(room, 10f);
            
            foreach (Room other in nearby)
            {
                // 如果还没有连接，并且随机通过，则创建连接
                if (room.GetConnectionTo(other) == null && Random.value < loopChance)
                {
                    TunnelRoom tunnel = ConnectTwoRoomsAndReturnTunnel(room, other);
                    if (tunnel != null)
                    {
                        newTunnels.Add(tunnel);
                    }
                }
            }
        }
        
        // 统一添加所有新的走廊房间
        rooms.AddRange(newTunnels);
    }
    
    /// <summary>
    /// 步骤5: 连接所有房间（确保连通性）
    /// </summary>
    private void ConnectRooms()
    {
        // 使用并查集确保所有房间连通
        HashSet<Room> connected = new HashSet<Room>();
        connected.Add(rooms[0]); // 从入口开始
        
        bool changed = true;
        while (changed && connected.Count < rooms.Count)
        {
            changed = false;
            
            foreach (Room room in new List<Room>(connected))
            {
                foreach (Door door in room.connected)
                {
                    Room other = door.GetOtherRoom(room);
                    if (other != null && !connected.Contains(other))
                    {
                        connected.Add(other);
                        changed = true;
                    }
                }
            }
        }
        
        // 连接未连接的房间 - 使用副本避免遍历时修改
        List<TunnelRoom> newTunnels = new List<TunnelRoom>();
        List<Room> roomsCopy = new List<Room>(rooms);
        
        foreach (Room room in roomsCopy)
        {
            if (!connected.Contains(room))
            {
                // 找到最近的已连接房间
                Room nearest = FindNearestRoom(room, new List<Room>(connected));
                if (nearest != null)
                {
                    TunnelRoom tunnel = ConnectTwoRoomsAndReturnTunnel(room, nearest);
                    if (tunnel != null)
                    {
                        newTunnels.Add(tunnel);
                    }
                    connected.Add(room);
                }
            }
        }
        
        // 统一添加所有新的走廊房间
        rooms.AddRange(newTunnels);
    }
    
    /// <summary>
    /// 步骤6: 清理孤立房间
    /// </summary>
    private void Cleanup()
    {
        // 移除没有连接的房间（不包括入口和出口）
        rooms.RemoveAll(room => 
            room.connected.Count == 0 && 
            !(room is EntranceRoom) && 
            !(room is ExitRoom)
        );
    }
    
    /// <summary>
    /// 连接两个房间（直接添加到rooms）
    /// </summary>
    private void ConnectTwoRooms(Room room1, Room room2)
    {
        TunnelRoom tunnel = ConnectTwoRoomsAndReturnTunnel(room1, room2);
        if (tunnel != null)
        {
            rooms.Add(tunnel);
        }
    }
    
    /// <summary>
    /// 连接两个房间并返回走廊（不添加到rooms）
    /// </summary>
    private TunnelRoom ConnectTwoRoomsAndReturnTunnel(Room room1, Room room2)
    {
        // 检查是否已连接
        if (room1.GetConnectionTo(room2) != null)
            return null;
        
        // 找到两个房间最近的点
        Vector2Int point1 = GetClosestPoint(room1, room2.Center);
        Vector2Int point2 = GetClosestPoint(room2, room1.Center);
        
        // 创建走廊
        TunnelRoom tunnel = new TunnelRoom();
        tunnel.CreatePath(point1, point2);
        
        // 创建门连接
        Door door = new Door(room1, room2, point1);
        
        return tunnel;
    }
    
    /// <summary>
    /// 获取房间上最接近目标点的点
    /// </summary>
    private Vector2Int GetClosestPoint(Room room, Vector2Int target)
    {
        int x = Mathf.Clamp(target.x, room.left + 1, room.right - 1);
        int y = Mathf.Clamp(target.y, room.top + 1, room.bottom - 1);
        return new Vector2Int(x, y);
    }
    
    /// <summary>
    /// 查找从起点到终点的路径
    /// </summary>
    private List<Room> FindPath(Room start, Room end)
    {
        List<Room> path = new List<Room>();
        path.Add(start);
        
        // 简化路径：添加中间房间
        List<Room> intermediate = rooms
            .Where(r => r != start && r != end && !(r is TunnelRoom))
            .OrderBy(r => Vector2Int.Distance(start.Center, r.Center))
            .Take(2)
            .ToList();
        
        path.AddRange(intermediate);
        path.Add(end);
        
        return path;
    }
    
    /// <summary>
    /// 查找附近的房间
    /// </summary>
    private List<Room> FindNearbyRooms(Room room, float maxDistance)
    {
        List<Room> nearby = new List<Room>();
        
        foreach (Room other in rooms)
        {
            if (other != room && !(other is TunnelRoom))
            {
                float distance = Vector2Int.Distance(room.Center, other.Center);
                if (distance <= maxDistance)
                {
                    nearby.Add(other);
                }
            }
        }
        
        return nearby;
    }
    
    /// <summary>
    /// 查找最近的房间
    /// </summary>
    private Room FindNearestRoom(Room room, List<Room> candidates)
    {
        Room nearest = null;
        float minDistance = float.MaxValue;
        
        foreach (Room candidate in candidates)
        {
            float distance = Vector2Int.Distance(room.Center, candidate.Center);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = candidate;
            }
        }
        
        return nearest;
    }
}

