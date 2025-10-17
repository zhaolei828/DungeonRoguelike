using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 房间基类
/// 所有房间类型的抽象基类，定义房间的基本属性和行为
/// </summary>
public abstract class Room
{
    // 房间边界（使用格子坐标）
    public int left;
    public int top;
    public int right;
    public int bottom;
    
    // 房间连接点列表
    public List<Door> connected = new List<Door>();
    
    // 房间尺寸属性
    public int Width => right - left + 1;
    public int Height => bottom - top + 1;
    
    // 房间中心点
    public Vector2Int Center => new Vector2Int(
        (left + right) / 2,
        (top + bottom) / 2
    );
    
    // 房间面积
    public int Area => Width * Height;
    
    /// <summary>
    /// 设置房间大小（由子类实现）
    /// </summary>
    public virtual void SetSize()
    {
        // 默认实现为空，子类可重写
    }
    
    /// <summary>
    /// 设置房间位置
    /// </summary>
    public virtual void SetPos(int left, int top)
    {
        this.left = left;
        this.top = top;
        // right和bottom需要在SetSize后设置
    }
    
    /// <summary>
    /// 调整房间大小
    /// </summary>
    public void Resize(int newWidth, int newHeight)
    {
        right = left + newWidth - 1;
        bottom = top + newHeight - 1;
    }
    
    /// <summary>
    /// 检查此房间是否与另一个房间重叠（包含1格边距）
    /// </summary>
    public bool Intersect(Room other)
    {
        return !(left - 1 > other.right || 
                 right + 1 < other.left || 
                 top - 1 > other.bottom || 
                 bottom + 1 < other.top);
    }
    
    /// <summary>
    /// 检查点是否在房间内
    /// </summary>
    public bool Inside(Vector2Int point)
    {
        return point.x >= left && point.x <= right && 
               point.y >= top && point.y <= bottom;
    }
    
    /// <summary>
    /// 添加连接点
    /// </summary>
    public void AddConnection(Door door)
    {
        connected.Add(door);
    }
    
    /// <summary>
    /// 获取与指定房间的连接门
    /// </summary>
    public Door GetConnectionTo(Room other)
    {
        foreach (Door door in connected)
        {
            if (door.GetOtherRoom(this) == other)
                return door;
        }
        return null;
    }
    
    /// <summary>
    /// 绘制房间到地图（由子类实现具体逻辑）
    /// </summary>
    /// <returns>是否绘制成功</returns>
    public abstract bool Paint(Level level);
    
    /// <summary>
    /// 获取房间类型名称（用于调试）
    /// </summary>
    public virtual string GetRoomType()
    {
        return GetType().Name;
    }
}

