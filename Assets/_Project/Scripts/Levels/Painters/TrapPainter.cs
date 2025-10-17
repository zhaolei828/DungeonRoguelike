using UnityEngine;

/// <summary>
/// 陷阱绘制器
/// 在地牢中放置各种陷阱
/// </summary>
public static class TrapPainter
{
    /// <summary>
    /// 在房间内随机放置陷阱
    /// </summary>
    public static void AddRandomTraps(Level level, Room room, int trapCount = 2)
    {
        int placed = 0;
        int attempts = 0;
        int maxAttempts = 50;
        
        while (placed < trapCount && attempts < maxAttempts)
        {
            int x = Random.Range(room.left + 2, room.right - 1);
            int y = Random.Range(room.top + 2, room.bottom - 1);
            
            // 确保陷阱放在地板上，且不在入口/出口附近
            if (level.GetTerrain(x, y) == Terrain.Floor)
            {
                Vector2Int pos = new Vector2Int(x, y);
                float distToEntrance = Vector2Int.Distance(pos, level.EntrancePos);
                float distToExit = Vector2Int.Distance(pos, level.ExitPos);
                
                if (distToEntrance > 3 && distToExit > 3)
                {
                    // 随机选择陷阱类型
                    Terrain trapType = GetRandomTrapType();
                    Painter.Set(level, x, y, trapType);
                    placed++;
                }
            }
            
            attempts++;
        }
    }
    
    /// <summary>
    /// 在走廊中放置陷阱
    /// </summary>
    public static void AddTunnelTraps(Level level, TunnelRoom tunnel, float trapChance = 0.15f)
    {
        // 在走廊的路径上随机放置陷阱
        for (int x = tunnel.left; x <= tunnel.right; x++)
        {
            for (int y = tunnel.top; y <= tunnel.bottom; y++)
            {
                if (level.GetTerrain(x, y) == Terrain.Floor && Random.value < trapChance)
                {
                    Terrain trapType = GetRandomTrapType();
                    Painter.Set(level, x, y, trapType);
                }
            }
        }
    }
    
    /// <summary>
    /// 放置特定类型的陷阱
    /// </summary>
    public static void PlaceTrap(Level level, int x, int y, Terrain trapType)
    {
        if (level.GetTerrain(x, y) == Terrain.Floor)
        {
            Painter.Set(level, x, y, trapType);
        }
    }
    
    /// <summary>
    /// 随机获取陷阱类型
    /// </summary>
    private static Terrain GetRandomTrapType()
    {
        float rand = Random.value;
        
        if (rand < 0.3f)
            return Terrain.Trap;
        else if (rand < 0.5f)
            return Terrain.Trap;
        else if (rand < 0.7f)
            return Terrain.Trap;
        else if (rand < 0.85f)
            return Terrain.Trap;
        else
            return Terrain.Trap;
    }
    
    /// <summary>
    /// 创建陷阱环（围绕某个点）
    /// </summary>
    public static void AddTrapRing(Level level, Vector2Int center, int radius)
    {
        for (int x = center.x - radius; x <= center.x + radius; x++)
        {
            for (int y = center.y - radius; y <= center.y + radius; y++)
            {
                float distance = Vector2Int.Distance(new Vector2Int(x, y), center);
                
                // 只在特定半径的环上放置陷阱
                if (Mathf.Abs(distance - radius) < 0.5f && level.GetTerrain(x, y) == Terrain.Floor)
                {
                    Painter.Set(level, x, y, GetRandomTrapType());
                }
            }
        }
    }
}

