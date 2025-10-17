using UnityEngine;

/// <summary>
/// 入口房间
/// 玩家进入该层时的起始位置
/// </summary>
public class EntranceRoom : Room
{
    /// <summary>
    /// 设置房间大小（固定5x5）
    /// </summary>
    public override void SetSize()
    {
        Resize(5, 5);
    }
    
    /// <summary>
    /// 绘制入口房间
    /// </summary>
    public override bool Paint(Level level)
    {
        // 填充地板
        Painter.Fill(level, this, Terrain.Floor);
        
        // 绘制墙壁边界
        for (int x = left; x <= right; x++)
        {
            Painter.Set(level, x, top, Terrain.Wall);
            Painter.Set(level, x, bottom, Terrain.Wall);
        }
        
        for (int y = top + 1; y < bottom; y++)
        {
            Painter.Set(level, left, y, Terrain.Wall);
            Painter.Set(level, right, y, Terrain.Wall);
        }
        
        // 在中心位置放置入口楼梯
        Vector2Int center = Center;
        Painter.Set(level, center.x, center.y, Terrain.Entrance);
        
        // 设置英雄的初始位置
        level.EntrancePos = center;
        
        return true;
    }
}

