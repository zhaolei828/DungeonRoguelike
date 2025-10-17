using UnityEngine;

/// <summary>
/// 门/连接点类型枚举
/// </summary>
public enum DoorType
{
    EMPTY,      // 空门（开放通道）
    DOOR,       // 普通门
    LOCKED,     // 锁定的门
    HIDDEN,     // 隐藏门
    BARRICADE,  // 路障
    CRYSTAL     // 水晶门
}

/// <summary>
/// 门类：表示房间之间的连接点
/// </summary>
public class Door
{
    // 连接的两个房间
    public Room room1;
    public Room room2;
    
    // 门的位置（格子坐标）
    public Vector2Int position;
    
    // 门的类型
    public DoorType type;
    
    /// <summary>
    /// 构造函数
    /// </summary>
    public Door(Room room1, Room room2, Vector2Int position, DoorType type = DoorType.EMPTY)
    {
        this.room1 = room1;
        this.room2 = room2;
        this.position = position;
        this.type = type;
        
        // 将门添加到两个房间的连接列表
        room1?.AddConnection(this);
        room2?.AddConnection(this);
    }
    
    /// <summary>
    /// 获取门连接的另一个房间
    /// </summary>
    public Room GetOtherRoom(Room currentRoom)
    {
        if (currentRoom == room1)
            return room2;
        else if (currentRoom == room2)
            return room1;
        return null;
    }
    
    /// <summary>
    /// 检查门是否连接指定的两个房间
    /// </summary>
    public bool Connects(Room r1, Room r2)
    {
        return (room1 == r1 && room2 == r2) || (room1 == r2 && room2 == r1);
    }
}

