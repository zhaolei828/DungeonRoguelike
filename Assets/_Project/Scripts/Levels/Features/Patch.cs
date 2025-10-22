using UnityEngine;

/// <summary>
/// Patch生成器 - 使用细胞自动机生成有机图案
/// 参考SPD的Patch.java实现
/// </summary>
public static class Patch
{
    /// <summary>
    /// 生成patch图案（细胞自动机）
    /// </summary>
    /// <param name="width">宽度</param>
    /// <param name="height">高度</param>
    /// <param name="fill">填充率 (0-1)，表示墙的比例</param>
    /// <param name="passes">聚类迭代次数，越多越聚集</param>
    /// <param name="diagonals">是否考虑对角线邻居</param>
    /// <returns>patch数组，true=墙/障碍物，false=地板/可走</returns>
    public static bool[] Generate(int width, int height, float fill, int passes, bool diagonals)
    {
        if (width <= 0 || height <= 0)
        {
            Debug.LogError($"Invalid patch size: {width}x{height}");
            return new bool[0];
        }
        
        if (fill < 0f || fill > 1f)
        {
            Debug.LogWarning($"Fill rate {fill} out of range [0,1], clamping");
            fill = Mathf.Clamp01(fill);
        }
        
        int size = width * height;
        bool[] patch = new bool[size];
        
        // 步骤1: 随机初始化
        for (int i = 0; i < size; i++)
        {
            patch[i] = Random.value < fill;
        }
        
        // 步骤2: 细胞自动机聚类
        // 通过多次迭代，让相邻的格子趋向于相同状态，形成聚集效果
        for (int pass = 0; pass < passes; pass++)
        {
            bool[] next = new bool[size];
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int idx = x + y * width;
                    int neighbors = CountNeighbors(patch, x, y, width, height, diagonals);
                    
                    // 细胞自动机规则：
                    // - 如果周围墙多，自己也变成墙（聚集效果）
                    // - 如果周围地板多，自己也变成地板
                    if (diagonals)
                    {
                        // 8邻居：如果>=5个是墙，自己也变成墙
                        next[idx] = neighbors >= 5;
                    }
                    else
                    {
                        // 4邻居：如果>=2个是墙，自己也变成墙
                        next[idx] = neighbors >= 2;
                    }
                }
            }
            
            patch = next;
        }
        
        return patch;
    }
    
    /// <summary>
    /// 统计邻居中墙的数量
    /// </summary>
    private static int CountNeighbors(bool[] patch, int x, int y, int width, int height, bool diagonals)
    {
        int count = 0;
        
        // 邻居偏移量
        int[] dx, dy;
        
        if (diagonals)
        {
            // 8方向（包括对角线）
            dx = new[] { -1, 0, 1, -1, 1, -1, 0, 1 };
            dy = new[] { -1, -1, -1, 0, 0, 1, 1, 1 };
        }
        else
        {
            // 4方向（上下左右）
            dx = new[] { 0, 0, -1, 1 };
            dy = new[] { 1, -1, 0, 0 };
        }
        
        for (int i = 0; i < dx.Length; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];
            
            // 边界检查
            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            {
                // 在范围内，检查是否是墙
                if (patch[nx + ny * width])
                    count++;
            }
            else
            {
                // 边界外视为墙（这样边缘会更规整）
                count++;
            }
        }
        
        return count;
    }
    
    /// <summary>
    /// 生成简单的矩形patch（用于测试）
    /// </summary>
    public static bool[] GenerateRectangle(int width, int height, int borderWidth = 1)
    {
        bool[] patch = new bool[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int idx = x + y * width;
                
                // 边界是墙
                bool isBorder = x < borderWidth || x >= width - borderWidth ||
                                y < borderWidth || y >= height - borderWidth;
                
                patch[idx] = isBorder;
            }
        }
        
        return patch;
    }
    
    /// <summary>
    /// 生成棋盘格patch（用于测试）
    /// </summary>
    public static bool[] GenerateCheckerboard(int width, int height, int cellSize = 2)
    {
        bool[] patch = new bool[width * height];
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int idx = x + y * width;
                
                // 棋盘格模式
                int cellX = x / cellSize;
                int cellY = y / cellSize;
                patch[idx] = (cellX + cellY) % 2 == 0;
            }
        }
        
        return patch;
    }
    
    /// <summary>
    /// 可视化patch（用于调试）
    /// </summary>
    public static string VisualizePatch(bool[] patch, int width, int height)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        
        sb.AppendLine($"Patch {width}x{height}:");
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int idx = x + y * width;
                sb.Append(patch[idx] ? "█" : "░");
            }
            sb.AppendLine();
        }
        
        return sb.ToString();
    }
    
    /// <summary>
    /// 计算patch的填充率
    /// </summary>
    public static float CalculateFillRate(bool[] patch)
    {
        if (patch == null || patch.Length == 0)
            return 0f;
        
        int wallCount = 0;
        foreach (bool cell in patch)
        {
            if (cell) wallCount++;
        }
        
        return (float)wallCount / patch.Length;
    }
}

