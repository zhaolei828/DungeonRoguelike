using UnityEngine;

/// <summary>
/// 关卡坐标转换工具类
/// 处理格子坐标和世界坐标之间的转换
/// </summary>
public static class LevelCoord
{
    // 地图尺寸常量
    public const int DEFAULT_WIDTH = 32;
    public const int DEFAULT_HEIGHT = 32;
    
    // 格子大小
    public const float CELL_SIZE = 1f;
    
    #region 坐标转换 (Java风格 ↔ Unity风格)
    
    /// <summary>
    /// 将2D坐标转换为1D位置（Java风格）
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <param name="width">地图宽度</param>
    /// <returns>一维位置</returns>
    public static int CoordsToPos(int x, int y, int width = DEFAULT_WIDTH)
    {
        return x + y * width;
    }
    
    /// <summary>
    /// 将1D位置转换为2D坐标（Java风格）
    /// </summary>
    /// <param name="pos">一维位置</param>
    /// <param name="width">地图宽度</param>
    /// <returns>2D坐标</returns>
    public static Vector2Int PosToCoords(int pos, int width = DEFAULT_WIDTH)
    {
        return new Vector2Int(pos % width, pos / width);
    }
    
    /// <summary>
    /// 将格子坐标转换为世界坐标
    /// </summary>
    /// <param name="gridPos">格子坐标</param>
    /// <returns>世界坐标</returns>
    public static Vector3 GridToWorld(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x * CELL_SIZE, gridPos.y * CELL_SIZE, 0);
    }
    
    /// <summary>
    /// 将世界坐标转换为格子坐标
    /// </summary>
    /// <param name="worldPos">世界坐标</param>
    /// <returns>格子坐标</returns>
    public static Vector2Int WorldToGrid(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPos.x / CELL_SIZE),
            Mathf.RoundToInt(worldPos.y / CELL_SIZE)
        );
    }
    
    /// <summary>
    /// 将格子坐标转换为世界坐标（带偏移）
    /// </summary>
    /// <param name="gridPos">格子坐标</param>
    /// <param name="offset">偏移量</param>
    /// <returns>世界坐标</returns>
    public static Vector3 GridToWorld(Vector2Int gridPos, Vector3 offset)
    {
        return GridToWorld(gridPos) + offset;
    }
    
    #endregion
    
    #region 距离计算
    
    /// <summary>
    /// 计算两个格子坐标之间的曼哈顿距离
    /// </summary>
    /// <param name="from">起始坐标</param>
    /// <param name="to">目标坐标</param>
    /// <returns>曼哈顿距离</returns>
    public static int ManhattanDistance(Vector2Int from, Vector2Int to)
    {
        return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
    }
    
    /// <summary>
    /// 计算两个格子坐标之间的欧几里得距离
    /// </summary>
    /// <param name="from">起始坐标</param>
    /// <param name="to">目标坐标</param>
    /// <returns>欧几里得距离</returns>
    public static float EuclideanDistance(Vector2Int from, Vector2Int to)
    {
        return Vector2Int.Distance(from, to);
    }
    
    /// <summary>
    /// 计算两个格子坐标之间的切比雪夫距离（国际象棋距离）
    /// </summary>
    /// <param name="from">起始坐标</param>
    /// <param name="to">目标坐标</param>
    /// <returns>切比雪夫距离</returns>
    public static int ChebyshevDistance(Vector2Int from, Vector2Int to)
    {
        return Mathf.Max(Mathf.Abs(from.x - to.x), Mathf.Abs(from.y - to.y));
    }
    
    #endregion
    
    #region 方向计算
    
    /// <summary>
    /// 8方向向量数组
    /// </summary>
    public static readonly Vector2Int[] Directions8 = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // 上
        new Vector2Int(1, 1),   // 右上
        new Vector2Int(1, 0),   // 右
        new Vector2Int(1, -1),  // 右下
        new Vector2Int(0, -1),  // 下
        new Vector2Int(-1, -1), // 左下
        new Vector2Int(-1, 0),  // 左
        new Vector2Int(-1, 1)   // 左上
    };
    
    /// <summary>
    /// 4方向向量数组
    /// </summary>
    public static readonly Vector2Int[] Directions4 = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // 上
        new Vector2Int(1, 0),   // 右
        new Vector2Int(0, -1),  // 下
        new Vector2Int(-1, 0)   // 左
    };
    
    /// <summary>
    /// 获取从一个位置到另一个位置的方向向量
    /// </summary>
    /// <param name="from">起始位置</param>
    /// <param name="to">目标位置</param>
    /// <returns>方向向量（归一化）</returns>
    public static Vector2Int GetDirection(Vector2Int from, Vector2Int to)
    {
        Vector2Int diff = to - from;
        
        // 归一化到-1, 0, 1
        int x = diff.x == 0 ? 0 : (diff.x > 0 ? 1 : -1);
        int y = diff.y == 0 ? 0 : (diff.y > 0 ? 1 : -1);
        
        return new Vector2Int(x, y);
    }
    
    /// <summary>
    /// 获取最接近目标方向的8方向向量
    /// </summary>
    /// <param name="direction">目标方向</param>
    /// <returns>8方向中最接近的方向</returns>
    public static Vector2Int GetClosest8Direction(Vector2 direction)
    {
        if (direction.magnitude < 0.1f)
            return Vector2Int.zero;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // 将角度转换为0-360度
        if (angle < 0) angle += 360;
        
        // 每个方向45度
        int directionIndex = Mathf.RoundToInt(angle / 45f) % 8;
        
        // 调整索引顺序以匹配Directions8数组
        // Directions8: 上(0), 右上(1), 右(2), 右下(3), 下(4), 左下(5), 左(6), 左上(7)
        // 角度: 右(0°), 右上(45°), 上(90°), 左上(135°), 左(180°), 左下(225°), 下(270°), 右下(315°)
        int[] angleToDirection = { 2, 1, 0, 7, 6, 5, 4, 3 };
        
        return Directions8[angleToDirection[directionIndex]];
    }
    
    #endregion
    
    #region 边界检查
    
    /// <summary>
    /// 检查坐标是否在指定边界内
    /// </summary>
    /// <param name="pos">坐标</param>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <returns>是否在边界内</returns>
    public static bool IsInBounds(Vector2Int pos, int width = DEFAULT_WIDTH, int height = DEFAULT_HEIGHT)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }
    
    /// <summary>
    /// 检查坐标是否在矩形区域内
    /// </summary>
    /// <param name="pos">坐标</param>
    /// <param name="rect">矩形区域</param>
    /// <returns>是否在区域内</returns>
    public static bool IsInBounds(Vector2Int pos, RectInt rect)
    {
        return rect.Contains(pos);
    }
    
    /// <summary>
    /// 将坐标限制在边界内
    /// </summary>
    /// <param name="pos">坐标</param>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <returns>限制后的坐标</returns>
    public static Vector2Int ClampToBounds(Vector2Int pos, int width = DEFAULT_WIDTH, int height = DEFAULT_HEIGHT)
    {
        return new Vector2Int(
            Mathf.Clamp(pos.x, 0, width - 1),
            Mathf.Clamp(pos.y, 0, height - 1)
        );
    }
    
    #endregion
    
    #region 邻居获取
    
    /// <summary>
    /// 获取4方向邻居坐标
    /// </summary>
    /// <param name="pos">中心坐标</param>
    /// <param name="width">地图宽度</param>
    /// <param name="height">地图高度</param>
    /// <returns>有效的邻居坐标列表</returns>
    public static System.Collections.Generic.List<Vector2Int> GetNeighbors4(Vector2Int pos, int width = DEFAULT_WIDTH, int height = DEFAULT_HEIGHT)
    {
        var neighbors = new System.Collections.Generic.List<Vector2Int>();
        
        foreach (Vector2Int dir in Directions4)
        {
            Vector2Int neighbor = pos + dir;
            if (IsInBounds(neighbor, width, height))
            {
                neighbors.Add(neighbor);
            }
        }
        
        return neighbors;
    }
    
    /// <summary>
    /// 获取8方向邻居坐标
    /// </summary>
    /// <param name="pos">中心坐标</param>
    /// <param name="width">地图宽度</param>
    /// <param name="height">地图高度</param>
    /// <returns>有效的邻居坐标列表</returns>
    public static System.Collections.Generic.List<Vector2Int> GetNeighbors8(Vector2Int pos, int width = DEFAULT_WIDTH, int height = DEFAULT_HEIGHT)
    {
        var neighbors = new System.Collections.Generic.List<Vector2Int>();
        
        foreach (Vector2Int dir in Directions8)
        {
            Vector2Int neighbor = pos + dir;
            if (IsInBounds(neighbor, width, height))
            {
                neighbors.Add(neighbor);
            }
        }
        
        return neighbors;
    }
    
    #endregion
}
