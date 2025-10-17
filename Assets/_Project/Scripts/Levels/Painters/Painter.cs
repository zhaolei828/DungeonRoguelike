using UnityEngine;

/// <summary>
/// 地形绘制工具类
/// 提供各种地形绘制方法
/// </summary>
public static class Painter
{
    /// <summary>
    /// 填充整个房间为指定地形
    /// </summary>
    public static void Fill(Level level, Room room, Terrain terrain)
    {
        Fill(level, room.left, room.top, room.Width, room.Height, terrain);
    }
    
    /// <summary>
    /// 填充指定矩形区域
    /// </summary>
    public static void Fill(Level level, int x, int y, int width, int height, Terrain terrain)
    {
        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                if (i >= 0 && i < level.Width && j >= 0 && j < level.Height)
                {
                    Set(level, i, j, terrain);
                }
            }
        }
    }
    
    /// <summary>
    /// 设置单个格子的地形
    /// </summary>
    public static void Set(Level level, int x, int y, Terrain terrain)
    {
        level.SetTerrain(x, y, terrain);
    }
    
    /// <summary>
    /// 设置单个格子的地形（Vector2Int版本）
    /// </summary>
    public static void Set(Level level, Vector2Int pos, Terrain terrain)
    {
        Set(level, pos.x, pos.y, terrain);
    }
    
    /// <summary>
    /// 绘制椭圆形区域
    /// </summary>
    public static void FillEllipse(Level level, Room room, Terrain terrain)
    {
        FillEllipse(level, room.left, room.top, room.Width, room.Height, terrain);
    }
    
    /// <summary>
    /// 绘制椭圆形区域
    /// </summary>
    public static void FillEllipse(Level level, int x, int y, int width, int height, Terrain terrain)
    {
        float centerX = x + width / 2f;
        float centerY = y + height / 2f;
        float radiusX = width / 2f;
        float radiusY = height / 2f;
        
        for (int i = x; i < x + width; i++)
        {
            for (int j = y; j < y + height; j++)
            {
                // 椭圆方程：((x-cx)/rx)^2 + ((y-cy)/ry)^2 <= 1
                float dx = (i - centerX) / radiusX;
                float dy = (j - centerY) / radiusY;
                
                if (dx * dx + dy * dy <= 1)
                {
                    Set(level, i, j, terrain);
                }
            }
        }
    }
    
    /// <summary>
    /// 绘制直线
    /// </summary>
    public static void DrawLine(Level level, Vector2Int from, Vector2Int to, Terrain terrain)
    {
        // 使用Bresenham直线算法
        int x0 = from.x;
        int y0 = from.y;
        int x1 = to.x;
        int y1 = to.y;
        
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;
        
        while (true)
        {
            Set(level, x0, y0, terrain);
            
            if (x0 == x1 && y0 == y1) break;
            
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
    
    /// <summary>
    /// 绘制矩形边框
    /// </summary>
    public static void DrawRect(Level level, Room room, Terrain terrain)
    {
        DrawRect(level, room.left, room.top, room.Width, room.Height, terrain);
    }
    
    /// <summary>
    /// 绘制矩形边框
    /// </summary>
    public static void DrawRect(Level level, int x, int y, int width, int height, Terrain terrain)
    {
        // 顶部和底部
        for (int i = x; i < x + width; i++)
        {
            Set(level, i, y, terrain);
            Set(level, i, y + height - 1, terrain);
        }
        
        // 左侧和右侧
        for (int j = y + 1; j < y + height - 1; j++)
        {
            Set(level, x, j, terrain);
            Set(level, x + width - 1, j, terrain);
        }
    }
    
    /// <summary>
    /// 在房间内随机放置地形
    /// </summary>
    public static void FillRandom(Level level, Room room, Terrain terrain, float probability)
    {
        for (int x = room.left; x <= room.right; x++)
        {
            for (int y = room.top; y <= room.bottom; y++)
            {
                if (Random.value < probability)
                {
                    Set(level, x, y, terrain);
                }
            }
        }
    }
}

