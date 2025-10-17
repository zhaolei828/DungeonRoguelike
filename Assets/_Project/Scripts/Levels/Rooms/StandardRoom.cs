using UnityEngine;

/// <summary>
/// 标准矩形房间
/// 最常见的房间类型，随机大小的矩形空间
/// </summary>
public class StandardRoom : Room
{
    private int minWidth = 4;
    private int maxWidth = 8;
    private int minHeight = 4;
    private int maxHeight = 8;
    
    /// <summary>
    /// 构造函数（使用默认大小范围）
    /// </summary>
    public StandardRoom() { }
    
    /// <summary>
    /// 构造函数（自定义大小范围）
    /// </summary>
    public StandardRoom(int minSize, int maxSize)
    {
        this.minWidth = minSize;
        this.maxWidth = maxSize;
        this.minHeight = minSize;
        this.maxHeight = maxSize;
    }
    
    /// <summary>
    /// 设置房间大小（随机）
    /// </summary>
    public override void SetSize()
    {
        int width = Random.Range(minWidth, maxWidth + 1);
        int height = Random.Range(minHeight, maxHeight + 1);
        Resize(width, height);
    }
    
    /// <summary>
    /// 绘制房间：填充地板，周围添加墙壁
    /// </summary>
    public override bool Paint(Level level)
    {
        // 使用Painter工具类填充地板
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
        
        return true;
    }
}

