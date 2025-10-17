using UnityEngine;

/// <summary>
/// 草地绘制器
/// 添加各种类型的草地和植被
/// </summary>
public static class GrassPainter
{
    /// <summary>
    /// 在房间内随机添加高草
    /// </summary>
    public static void AddHighGrass(Level level, Room room, float density = 0.1f)
    {
        for (int x = room.left + 1; x < room.right; x++)
        {
            for (int y = room.top + 1; y < room.bottom; y++)
            {
                if (level.GetTerrain(x, y) == Terrain.Floor && Random.value < density)
                {
                    Painter.Set(level, x, y, Terrain.HighGrass);
                }
            }
        }
    }
    
    /// <summary>
    /// 在房间边缘添加草丛
    /// </summary>
    public static void AddGrassPatches(Level level, Room room, int patchCount = 3)
    {
        for (int i = 0; i < patchCount; i++)
        {
            int centerX = Random.Range(room.left + 2, room.right - 1);
            int centerY = Random.Range(room.top + 2, room.bottom - 1);
            int patchSize = Random.Range(2, 4);
            
            for (int x = centerX - patchSize; x <= centerX + patchSize; x++)
            {
                for (int y = centerY - patchSize; y <= centerY + patchSize; y++)
                {
                    if (x > room.left && x < room.right && 
                        y > room.top && y < room.bottom &&
                        level.GetTerrain(x, y) == Terrain.Floor)
                    {
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));
                        if (distance <= patchSize && Random.value < 0.6f)
                        {
                            Painter.Set(level, x, y, Terrain.HighGrass);
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 填充整个房间为草地
    /// </summary>
    public static void FillGrass(Level level, Room room)
    {
        Painter.Fill(level, room, Terrain.Grass);
    }
    
    /// <summary>
    /// 在房间内添加发光蘑菇
    /// </summary>
    public static void AddGlowingGrass(Level level, Room room, int count = 5)
    {
        for (int i = 0; i < count; i++)
        {
            int x = Random.Range(room.left + 1, room.right);
            int y = Random.Range(room.top + 1, room.bottom);
            
            if (level.GetTerrain(x, y) == Terrain.Floor)
            {
                Painter.Set(level, x, y, Terrain.EmberFloor);
            }
        }
    }
}

