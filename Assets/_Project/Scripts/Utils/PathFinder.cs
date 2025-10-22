using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 路径查找和连通性检查工具
/// 参考SPD的PathFinder实现
/// </summary>
public static class PathFinder
{
    // 四方向（上下左右）
    private static readonly Vector2Int[] DIRECTIONS_4 = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // 上
        new Vector2Int(0, -1),  // 下
        new Vector2Int(-1, 0),  // 左
        new Vector2Int(1, 0)    // 右
    };
    
    // 八方向（包括对角线）
    private static readonly Vector2Int[] DIRECTIONS_8 = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // 上
        new Vector2Int(0, -1),  // 下
        new Vector2Int(-1, 0),  // 左
        new Vector2Int(1, 0),   // 右
        new Vector2Int(-1, 1),  // 左上
        new Vector2Int(1, 1),   // 右上
        new Vector2Int(-1, -1), // 左下
        new Vector2Int(1, -1)   // 右下
    };
    
    /// <summary>
    /// 构建距离地图（BFS）
    /// 从起点开始，计算到所有可达位置的距离
    /// </summary>
    /// <param name="level">关卡</param>
    /// <param name="start">起点</param>
    /// <param name="maxDistance">最大距离（-1表示无限制）</param>
    /// <returns>距离地图，不可达的位置为int.MaxValue</returns>
    public static int[,] BuildDistanceMap(Level level, Vector2Int start, int maxDistance = -1)
    {
        int width = level.Width;
        int height = level.Height;
        int[,] distanceMap = new int[width, height];
        
        // 初始化所有位置为不可达
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                distanceMap[x, y] = int.MaxValue;
            }
        }
        
        // BFS队列
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);
        distanceMap[start.x, start.y] = 0;
        
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            int currentDistance = distanceMap[current.x, current.y];
            
            // 如果达到最大距离，停止扩展
            if (maxDistance >= 0 && currentDistance >= maxDistance)
                continue;
            
            // 检查四个方向
            foreach (Vector2Int dir in DIRECTIONS_4)
            {
                Vector2Int next = current + dir;
                
                // 检查边界
                if (!level.IsValidPosition(next.x, next.y))
                    continue;
                
                // 检查是否可通行
                if (!level.IsPassable(next.x, next.y))
                    continue;
                
                // 如果已经访问过，跳过
                if (distanceMap[next.x, next.y] != int.MaxValue)
                    continue;
                
                // 记录距离并加入队列
                distanceMap[next.x, next.y] = currentDistance + 1;
                queue.Enqueue(next);
            }
        }
        
        return distanceMap;
    }
    
    /// <summary>
    /// 检查两个位置是否连通
    /// </summary>
    /// <param name="level">关卡</param>
    /// <param name="from">起点</param>
    /// <param name="to">终点</param>
    /// <returns>是否连通</returns>
    public static bool IsReachable(Level level, Vector2Int from, Vector2Int to)
    {
        // 如果起点或终点不可通行，直接返回false
        if (!level.IsPassable(from.x, from.y) || !level.IsPassable(to.x, to.y))
            return false;
        
        // 构建距离地图
        int[,] distanceMap = BuildDistanceMap(level, from);
        
        // 检查终点是否可达
        return distanceMap[to.x, to.y] != int.MaxValue;
    }
    
    /// <summary>
    /// 获取所有可达的位置
    /// </summary>
    /// <param name="level">关卡</param>
    /// <param name="start">起点</param>
    /// <param name="maxDistance">最大距离（-1表示无限制）</param>
    /// <returns>所有可达位置的列表</returns>
    public static List<Vector2Int> GetReachablePositions(Level level, Vector2Int start, int maxDistance = -1)
    {
        List<Vector2Int> reachable = new List<Vector2Int>();
        int[,] distanceMap = BuildDistanceMap(level, start, maxDistance);
        
        for (int x = 0; x < level.Width; x++)
        {
            for (int y = 0; y < level.Height; y++)
            {
                if (distanceMap[x, y] != int.MaxValue)
                {
                    reachable.Add(new Vector2Int(x, y));
                }
            }
        }
        
        return reachable;
    }
    
    /// <summary>
    /// Flood Fill算法 - 标记所有连通的区域
    /// </summary>
    /// <param name="level">关卡</param>
    /// <param name="start">起点</param>
    /// <returns>连通区域标记（true=连通，false=不连通）</returns>
    public static bool[,] FloodFill(Level level, Vector2Int start)
    {
        int width = level.Width;
        int height = level.Height;
        bool[,] filled = new bool[width, height];
        
        // 如果起点不可通行，返回空区域
        if (!level.IsPassable(start.x, start.y))
            return filled;
        
        // BFS队列
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);
        filled[start.x, start.y] = true;
        
        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            
            // 检查四个方向
            foreach (Vector2Int dir in DIRECTIONS_4)
            {
                Vector2Int next = current + dir;
                
                // 检查边界
                if (!level.IsValidPosition(next.x, next.y))
                    continue;
                
                // 检查是否可通行
                if (!level.IsPassable(next.x, next.y))
                    continue;
                
                // 如果已经填充过，跳过
                if (filled[next.x, next.y])
                    continue;
                
                // 标记并加入队列
                filled[next.x, next.y] = true;
                queue.Enqueue(next);
            }
        }
        
        return filled;
    }
    
    /// <summary>
    /// 查找所有孤立的可通行区域
    /// </summary>
    /// <param name="level">关卡</param>
    /// <returns>孤立区域列表，每个区域是一个位置列表</returns>
    public static List<List<Vector2Int>> FindIsolatedRegions(Level level)
    {
        int width = level.Width;
        int height = level.Height;
        bool[,] visited = new bool[width, height];
        List<List<Vector2Int>> regions = new List<List<Vector2Int>>();
        
        // 遍历所有位置
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // 如果不可通行或已访问，跳过
                if (!level.IsPassable(x, y) || visited[x, y])
                    continue;
                
                // 从这个位置开始Flood Fill，找到一个连通区域
                List<Vector2Int> region = new List<Vector2Int>();
                Queue<Vector2Int> queue = new Queue<Vector2Int>();
                Vector2Int start = new Vector2Int(x, y);
                
                queue.Enqueue(start);
                visited[x, y] = true;
                region.Add(start);
                
                while (queue.Count > 0)
                {
                    Vector2Int current = queue.Dequeue();
                    
                    // 检查四个方向
                    foreach (Vector2Int dir in DIRECTIONS_4)
                    {
                        Vector2Int next = current + dir;
                        
                        // 检查边界
                        if (!level.IsValidPosition(next.x, next.y))
                            continue;
                        
                        // 检查是否可通行
                        if (!level.IsPassable(next.x, next.y))
                            continue;
                        
                        // 如果已经访问过，跳过
                        if (visited[next.x, next.y])
                            continue;
                        
                        // 标记并加入队列
                        visited[next.x, next.y] = true;
                        region.Add(next);
                        queue.Enqueue(next);
                    }
                }
                
                regions.Add(region);
            }
        }
        
        return regions;
    }
    
    /// <summary>
    /// 验证地图连通性
    /// </summary>
    /// <param name="level">关卡</param>
    /// <param name="entrance">入口位置</param>
    /// <param name="exit">出口位置</param>
    /// <param name="minReachablePercent">最小可达区域百分比（0-1）</param>
    /// <returns>验证结果</returns>
    public static ConnectivityValidation ValidateConnectivity(Level level, Vector2Int entrance, Vector2Int exit, float minReachablePercent = 0.8f)
    {
        ConnectivityValidation result = new ConnectivityValidation();
        
        // 1. 检查入口和出口是否可通行
        result.entrancePassable = level.IsPassable(entrance.x, entrance.y);
        result.exitPassable = level.IsPassable(exit.x, exit.y);
        
        if (!result.entrancePassable || !result.exitPassable)
        {
            result.isValid = false;
            result.errorMessage = "入口或出口不可通行";
            return result;
        }
        
        // 2. 检查入口到出口是否连通
        result.entranceToExitReachable = IsReachable(level, entrance, exit);
        
        if (!result.entranceToExitReachable)
        {
            result.isValid = false;
            result.errorMessage = "入口和出口不连通";
            return result;
        }
        
        // 3. 统计所有可通行的格子数量
        int totalPassable = 0;
        for (int x = 0; x < level.Width; x++)
        {
            for (int y = 0; y < level.Height; y++)
            {
                if (level.IsPassable(x, y))
                    totalPassable++;
            }
        }
        
        // 4. 从入口开始，统计可达的格子数量
        List<Vector2Int> reachableFromEntrance = GetReachablePositions(level, entrance);
        result.totalPassableTiles = totalPassable;
        result.reachableTiles = reachableFromEntrance.Count;
        result.reachablePercent = totalPassable > 0 ? (float)reachableFromEntrance.Count / totalPassable : 0f;
        
        // 5. 查找孤立区域
        result.isolatedRegions = FindIsolatedRegions(level);
        result.isolatedRegionCount = result.isolatedRegions.Count;
        
        // 6. 判断是否有效
        if (result.reachablePercent < minReachablePercent)
        {
            result.isValid = false;
            result.errorMessage = $"可达区域过小：{result.reachablePercent:P} < {minReachablePercent:P}";
            return result;
        }
        
        if (result.isolatedRegionCount > 1)
        {
            result.isValid = false;
            result.errorMessage = $"存在 {result.isolatedRegionCount} 个孤立区域";
            return result;
        }
        
        result.isValid = true;
        result.errorMessage = "地图连通性验证通过";
        return result;
    }
    
    /// <summary>
    /// 查找最近的可达位置
    /// </summary>
    /// <param name="level">关卡</param>
    /// <param name="from">起点</param>
    /// <param name="target">目标位置</param>
    /// <returns>最近的可达位置，如果目标本身可达则返回目标</returns>
    public static Vector2Int FindNearestReachable(Level level, Vector2Int from, Vector2Int target)
    {
        // 如果目标本身可达，直接返回
        if (IsReachable(level, from, target))
            return target;
        
        // 构建距离地图
        int[,] distanceMap = BuildDistanceMap(level, from);
        
        // 在目标周围螺旋搜索最近的可达位置
        int maxSearchRadius = Mathf.Max(level.Width, level.Height);
        
        for (int radius = 1; radius <= maxSearchRadius; radius++)
        {
            for (int dx = -radius; dx <= radius; dx++)
            {
                for (int dy = -radius; dy <= radius; dy++)
                {
                    // 只检查当前半径的边缘
                    if (Mathf.Abs(dx) != radius && Mathf.Abs(dy) != radius)
                        continue;
                    
                    Vector2Int pos = target + new Vector2Int(dx, dy);
                    
                    // 检查边界
                    if (!level.IsValidPosition(pos.x, pos.y))
                        continue;
                    
                    // 检查是否可达
                    if (distanceMap[pos.x, pos.y] != int.MaxValue)
                        return pos;
                }
            }
        }
        
        // 如果找不到，返回起点
        return from;
    }
}

/// <summary>
/// 连通性验证结果
/// </summary>
public class ConnectivityValidation
{
    public bool isValid = false;
    public string errorMessage = "";
    
    public bool entrancePassable = false;
    public bool exitPassable = false;
    public bool entranceToExitReachable = false;
    
    public int totalPassableTiles = 0;
    public int reachableTiles = 0;
    public float reachablePercent = 0f;
    
    public int isolatedRegionCount = 0;
    public List<List<Vector2Int>> isolatedRegions = new List<List<Vector2Int>>();
    
    public override string ToString()
    {
        return $"连通性验证: {(isValid ? "✓ 通过" : "✗ 失败")}\n" +
               $"  消息: {errorMessage}\n" +
               $"  入口可通行: {entrancePassable}\n" +
               $"  出口可通行: {exitPassable}\n" +
               $"  入口到出口可达: {entranceToExitReachable}\n" +
               $"  总可通行格子: {totalPassableTiles}\n" +
               $"  可达格子: {reachableTiles} ({reachablePercent:P})\n" +
               $"  孤立区域数: {isolatedRegionCount}";
    }
}

