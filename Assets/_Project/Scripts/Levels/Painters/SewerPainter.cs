using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 下水道绘制器
/// 添加下水道特有的地形元素：水域、高草、特殊装饰
/// </summary>
public static class SewerPainter
{
    /// <summary>
    /// 应用下水道风格到关卡
    /// </summary>
    public static void Paint(Level level, List<Room> rooms)
    {
        foreach (Room room in rooms)
        {
            if (room is StandardRoom)
            {
                // 20%概率添加水坑
                if (Random.value < 0.2f)
                {
                    AddWaterPool(level, room);
                }
                
                // 15%概率添加高草
                if (Random.value < 0.15f)
                {
                    AddGrass(level, room);
                }
            }
        }
        
        // 在一些房间添加装饰墙
        DecorateWalls(level, rooms);
    }
    
    /// <summary>
    /// 添加水坑
    /// </summary>
    private static void AddWaterPool(Level level, Room room)
    {
        // 在房间中心区域创建小水坑
        int poolSize = Random.Range(2, 4);
        int centerX = (room.left + room.right) / 2;
        int centerY = (room.top + room.bottom) / 2;
        
        for (int x = centerX - poolSize / 2; x <= centerX + poolSize / 2; x++)
        {
            for (int y = centerY - poolSize / 2; y <= centerY + poolSize / 2; y++)
            {
                if (x > room.left && x < room.right && y > room.top && y < room.bottom)
                {
                    // 使用椭圆形状
                    float dx = (x - centerX) / (float)(poolSize / 2);
                    float dy = (y - centerY) / (float)(poolSize / 2);
                    if (dx * dx + dy * dy <= 1)
                    {
                        Painter.Set(level, x, y, Terrain.Water);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 添加高草
    /// </summary>
    private static void AddGrass(Level level, Room room)
    {
        // 在房间边缘添加高草
        int grassCount = Random.Range(3, 8);
        
        for (int i = 0; i < grassCount; i++)
        {
            int x = Random.Range(room.left + 1, room.right);
            int y = Random.Range(room.top + 1, room.bottom);
            
            // 检查是否是地板
            if (level.GetTerrain(x, y) == Terrain.Floor)
            {
                Painter.Set(level, x, y, Terrain.HighGrass);
                
                // 30%概率在周围也添加草
                if (Random.value < 0.3f && x + 1 < room.right && level.GetTerrain(x + 1, y) == Terrain.Floor)
                    Painter.Set(level, x + 1, y, Terrain.HighGrass);
                if (Random.value < 0.3f && y + 1 < room.bottom && level.GetTerrain(x, y + 1) == Terrain.Floor)
                    Painter.Set(level, x, y + 1, Terrain.HighGrass);
            }
        }
    }
    
    /// <summary>
    /// 装饰墙壁
    /// </summary>
    private static void DecorateWalls(Level level, List<Room> rooms)
    {
        foreach (Room room in rooms)
        {
            if (room is StandardRoom || room is EntranceRoom || room is ExitRoom)
            {
                // 40%概率给墙壁添加装饰
                if (Random.value < 0.4f)
                {
                    for (int x = room.left; x <= room.right; x++)
                    {
                        if (level.GetTerrain(x, room.top) == Terrain.Wall && Random.value < 0.5f)
                            Painter.Set(level, x, room.top, Terrain.Wall);
                        if (level.GetTerrain(x, room.bottom) == Terrain.Wall && Random.value < 0.5f)
                            Painter.Set(level, x, room.bottom, Terrain.Wall);
                    }
                    
                    for (int y = room.top + 1; y < room.bottom; y++)
                    {
                        if (level.GetTerrain(room.left, y) == Terrain.Wall && Random.value < 0.5f)
                            Painter.Set(level, room.left, y, Terrain.Wall);
                        if (level.GetTerrain(room.right, y) == Terrain.Wall && Random.value < 0.5f)
                            Painter.Set(level, room.right, y, Terrain.Wall);
                    }
                }
            }
        }
    }
}

