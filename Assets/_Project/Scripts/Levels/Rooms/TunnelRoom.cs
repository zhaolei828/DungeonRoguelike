using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 走廊房间
/// 用于连接其他房间的通道，1格宽的线性房间，支持拐角
/// </summary>
public class TunnelRoom : Room
{
    // 走廊路径点
    private List<Vector2Int> path = new List<Vector2Int>();
    
    /// <summary>
    /// 构造函数
    /// </summary>
    public TunnelRoom() { }
    
    /// <summary>
    /// 设置走廊路径
    /// </summary>
    public void SetPath(List<Vector2Int> pathPoints)
    {
        path = new List<Vector2Int>(pathPoints);
        
        if (path.Count > 0)
        {
            // 根据路径计算边界
            left = path[0].x;
            right = path[0].x;
            top = path[0].y;
            bottom = path[0].y;
            
            foreach (Vector2Int point in path)
            {
                if (point.x < left) left = point.x;
                if (point.x > right) right = point.x;
                if (point.y < top) top = point.y;
                if (point.y > bottom) bottom = point.y;
            }
        }
    }
    
    /// <summary>
    /// 从起点到终点创建走廊
    /// </summary>
    public void CreatePath(Vector2Int start, Vector2Int end)
    {
        path.Clear();
        
        Vector2Int current = start;
        path.Add(current);
        
        // 先水平移动，再垂直移动（L型走廊）
        while (current.x != end.x)
        {
            current.x += current.x < end.x ? 1 : -1;
            path.Add(current);
        }
        
        while (current.y != end.y)
        {
            current.y += current.y < end.y ? 1 : -1;
            path.Add(current);
        }
        
        // 更新边界
        SetPath(path);
    }
    
    /// <summary>
    /// 绘制走廊
    /// </summary>
    public override bool Paint(Level level)
    {
        foreach (Vector2Int point in path)
        {
            // 检查边界
            if (point.x >= 0 && point.x < level.Width && 
                point.y >= 0 && point.y < level.Height)
            {
                Painter.Set(level, point.x, point.y, Terrain.Floor);
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// 获取路径的起点
    /// </summary>
    public Vector2Int GetStart()
    {
        return path.Count > 0 ? path[0] : Vector2Int.zero;
    }
    
    /// <summary>
    /// 获取路径的终点
    /// </summary>
    public Vector2Int GetEnd()
    {
        return path.Count > 0 ? path[path.Count - 1] : Vector2Int.zero;
    }
}

