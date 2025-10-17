using UnityEngine;

/// <summary>
/// 水域绘制器
/// 在指定区域添加水域
/// </summary>
public static class WaterPainter
{
    /// <summary>
    /// 在房间内填充水域
    /// </summary>
    public static void FillWater(Level level, Room room)
    {
        Painter.Fill(level, room, Terrain.Water);
    }
    
    /// <summary>
    /// 在房间内添加随机水坑
    /// </summary>
    public static void AddRandomPools(Level level, Room room, int poolCount, float poolDensity = 0.3f)
    {
        for (int i = 0; i < poolCount; i++)
        {
            int x = Random.Range(room.left + 1, room.right);
            int y = Random.Range(room.top + 1, room.bottom);
            
            int poolSize = Random.Range(2, 4);
            
            for (int px = x - poolSize; px <= x + poolSize; px++)
            {
                for (int py = y - poolSize; py <= y + poolSize; py++)
                {
                    if (px >= room.left && px <= room.right && 
                        py >= room.top && py <= room.bottom &&
                        Random.value < poolDensity)
                    {
                        if (level.GetTerrain(px, py) == Terrain.Floor)
                        {
                            Painter.Set(level, px, py, Terrain.Water);
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 创建椭圆形水池
    /// </summary>
    public static void AddEllipsePool(Level level, int centerX, int centerY, int radiusX, int radiusY)
    {
        for (int x = centerX - radiusX; x <= centerX + radiusX; x++)
        {
            for (int y = centerY - radiusY; y <= centerY + radiusY; y++)
            {
                float dx = (x - centerX) / (float)radiusX;
                float dy = (y - centerY) / (float)radiusY;
                
                if (dx * dx + dy * dy <= 1)
                {
                    Painter.Set(level, x, y, Terrain.Water);
                }
            }
        }
    }
}

