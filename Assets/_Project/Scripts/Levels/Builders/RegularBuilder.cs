using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 常规生成器
/// 简化版的地牢生成算法，用于特殊关卡
/// </summary>
public class RegularBuilder : ILevelBuilder
{
    // 生成参数
    private int minRooms = 4;
    private int maxRooms = 8;
    private int minRoomSize = 4;
    private int maxRoomSize = 7;
    
    private List<Room> rooms;
    private Level level;
    private System.Random random;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    public RegularBuilder()
    {
        random = new System.Random();
    }
    
    /// <summary>
    /// 构造函数（自定义参数）
    /// </summary>
    public RegularBuilder(int minRooms, int maxRooms, int minSize, int maxSize)
    {
        this.minRooms = minRooms;
        this.maxRooms = maxRooms;
        this.minRoomSize = minSize;
        this.maxRoomSize = maxSize;
        random = new System.Random();
    }
    
    /// <summary>
    /// 主生成方法
    /// </summary>
    public List<Room> Build(Level level)
    {
        this.level = level;
        rooms = new List<Room>();
        
        // 1. 创建入口
        CreateEntrance();
        
        // 2. 放置房间
        PlaceRooms();
        
        // 3. 创建出口
        CreateExit();
        
        // 4. 连接所有房间
        ConnectAll();
        
        return rooms;
    }
    
    /// <summary>
    /// 创建入口
    /// </summary>
    private void CreateEntrance()
    {
        EntranceRoom entrance = new EntranceRoom();
        entrance.SetSize();
        entrance.SetPos(level.Width / 2 - 2, 2);
        rooms.Add(entrance);
    }
    
    /// <summary>
    /// 放置房间
    /// </summary>
    private void PlaceRooms()
    {
        int roomsToPlace = random.Next(minRooms, maxRooms + 1);
        int attempts = 0;
        int maxAttempts = 300;
        
        while (rooms.Count < roomsToPlace + 1 && attempts < maxAttempts) // +1 for entrance
        {
            StandardRoom newRoom = new StandardRoom(minRoomSize, maxRoomSize);
            newRoom.SetSize();
            
            newRoom.SetPos(
                random.Next(2, level.Width - newRoom.Width - 2),
                random.Next(2, level.Height - newRoom.Height - 2)
            );
            
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
        
        Debug.Log($"RegularBuilder: Placed {rooms.Count} rooms");
    }
    
    /// <summary>
    /// 创建出口
    /// </summary>
    private void CreateExit()
    {
        ExitRoom exit = new ExitRoom();
        exit.SetSize();
        
        // 尝试将出口放在远离入口的位置
        Room entrance = rooms[0];
        int attempts = 0;
        do
        {
            exit.SetPos(
                random.Next(2, level.Width - 7),
                random.Next(2, level.Height - 7)
            );
            attempts++;
        } while (attempts < 30 && (Vector2Int.Distance(entrance.Center, exit.Center) < 12 || exit.Intersect(entrance)));
        
        // 检查与其他房间的重叠
        bool overlaps = true;
        attempts = 0;
        while (overlaps && attempts < 50)
        {
            overlaps = false;
            foreach (Room room in rooms)
            {
                if (exit.Intersect(room))
                {
                    overlaps = true;
                    exit.SetPos(
                        random.Next(2, level.Width - 7),
                        random.Next(level.Height / 2, level.Height - 7)
                    );
                    break;
                }
            }
            attempts++;
        }
        
        rooms.Add(exit);
    }
    
    /// <summary>
    /// 连接所有房间
    /// </summary>
    private void ConnectAll()
    {
        // 简单策略：按照Y坐标排序，从上到下连接
        List<Room> sortedRooms = new List<Room>(rooms);
        sortedRooms.Sort((a, b) => a.Center.y.CompareTo(b.Center.y));
        
        for (int i = 0; i < sortedRooms.Count - 1; i++)
        {
            Room current = sortedRooms[i];
            Room next = sortedRooms[i + 1];
            
            ConnectTwoRooms(current, next);
        }
    }
    
    /// <summary>
    /// 连接两个房间
    /// </summary>
    private void ConnectTwoRooms(Room room1, Room room2)
    {
        Vector2Int point1 = GetClosestPoint(room1, room2.Center);
        Vector2Int point2 = GetClosestPoint(room2, room1.Center);
        
        TunnelRoom tunnel = new TunnelRoom();
        tunnel.CreatePath(point1, point2);
        rooms.Add(tunnel);
        
        Door door = new Door(room1, room2, point1);
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
}

