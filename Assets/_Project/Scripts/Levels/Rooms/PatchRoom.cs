using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// PatchRoom - 使用Patch系统生成有机图案的房间
/// 核心特性：在绘制时验证连通性，防止房间内出现死胡同
/// 参考SPD的PatchRoom.java实现
/// </summary>
public abstract class PatchRoom : StandardRoom
{
    protected bool[] patch;  // patch数组，true=墙，false=地板
    
    /// <summary>
    /// 填充率（墙的比例，0-1）
    /// </summary>
    protected abstract float FillPercent { get; }
    
    /// <summary>
    /// 聚类迭代次数（越多越聚集）
    /// </summary>
    protected abstract int ClusteringPasses { get; }
    
    /// <summary>
    /// 是否确保路径连通（推荐true）
    /// </summary>
    protected abstract bool EnsurePath { get; }
    
    /// <summary>
    /// 是否清理对角边缘（让图案更整洁）
    /// </summary>
    protected virtual bool CleanEdges => true;
    
    /// <summary>
    /// 绘制房间
    /// </summary>
    public override bool Paint(Level level)
    {
        if (EnsurePath)
        {
            // 使用连通性检查生成patch
            SetupPatchWithPathfinding(level);
        }
        else
        {
            // 直接生成patch，不检查连通性
            patch = Patch.Generate(Width - 2, Height - 2, FillPercent, ClusteringPasses, true);
        }
        
        // 清理对角边缘
        if (CleanEdges)
        {
            CleanDiagonalEdges();
        }
        
        // 绘制地形
        return PaintTerrain(level);
    }
    
    /// <summary>
    /// 使用PathFinder验证连通性的patch生成
    /// 这是防止死胡同的核心算法！
    /// </summary>
    private void SetupPatchWithPathfinding(Level level)
    {
        int attempts = 0;
        float fill = FillPercent;
        bool valid = false;
        
        int patchWidth = Width - 2;
        int patchHeight = Height - 2;
        
        Debug.Log($"<color=cyan>[PatchRoom] 开始生成 {GetType().Name}，尺寸 {patchWidth}x{patchHeight}，填充率 {fill:F2}</color>");
        
        do
        {
            // 步骤1: 生成patch图案
            patch = Patch.Generate(patchWidth, patchHeight, fill, ClusteringPasses, true);
            
            // 步骤2: 找到起点（优先门附近）
            int startIdx = GetPatchStartPoint();
            
            // 步骤3: 确保门附近可通行
            ClearDoorAreas();
            
            // 步骤4: 构建可通行数组
            bool[] walkable = GetWalkableArea();
            
            // 步骤5: 使用BFS验证连通性
            int[] distance = PathFinder.BuildDistanceMapLocal(startIdx, walkable, patchWidth, patchHeight);
            
            // 步骤6: 检查是否有孤立的可通行区域
            valid = true;
            for (int i = 0; i < patch.Length; i++)
            {
                // 如果是可通行区域（!patch[i]）但距离为无穷大，说明孤立了
                if (!patch[i] && distance[i] == int.MaxValue)
                {
                    valid = false;
                    int x = i % patchWidth;
                    int y = i / patchWidth;
                    Debug.LogWarning($"<color=yellow>[PatchRoom] 发现孤立区域 at ({x},{y})</color>");
                    break;
                }
            }
            
            attempts++;
            
            // 步骤7: 如果失败次数过多，降低填充率重试
            if (!valid && attempts > 100)
            {
                fill -= 0.01f;
                attempts = 0;
                Debug.LogWarning($"<color=yellow>[PatchRoom] 降低填充率到 {fill:F2}</color>");
                
                // 如果填充率太低，放弃
                if (fill < 0.1f)
                {
                    Debug.LogError($"<color=red>[PatchRoom] 填充率过低，生成失败！</color>");
                    // 生成一个简单的空房间
                    patch = new bool[patchWidth * patchHeight];
                    break;
                }
            }
            
        } while (!valid);
        
        if (valid)
        {
            Debug.Log($"<color=green>[PatchRoom] {GetType().Name} 生成成功！耗时 {attempts} 次尝试</color>");
        }
    }
    
    /// <summary>
    /// 获取patch的起点（用于BFS）
    /// 优先选择门附近的位置
    /// </summary>
    private int GetPatchStartPoint()
    {
        int patchWidth = Width - 2;
        int patchHeight = Height - 2;
        
        // 如果有门，从门内侧开始
        foreach (var door in connected)
        {
            if (door == null) continue;
            
            // 计算门在patch坐标系中的位置
            int localX = door.position.x - left - 1;
            int localY = door.position.y - top - 1;
            
            // 门在边界上，我们需要找到门内侧的位置
            if (door.position.x == left)
            {
                localX = 0;  // 左边界，往右
            }
            else if (door.position.x == right)
            {
                localX = patchWidth - 1;  // 右边界，往左
            }
            else if (door.position.y == top)
            {
                localY = 0;  // 上边界，往下
            }
            else if (door.position.y == bottom)
            {
                localY = patchHeight - 1;  // 下边界，往上
            }
            
            // 确保在范围内
            localX = Mathf.Clamp(localX, 0, patchWidth - 1);
            localY = Mathf.Clamp(localY, 0, patchHeight - 1);
            
            return localX + localY * patchWidth;
        }
        
        // 如果没有门，从中心开始
        int centerX = patchWidth / 2;
        int centerY = patchHeight / 2;
        return centerX + centerY * patchWidth;
    }
    
    /// <summary>
    /// 确保门附近区域可通行
    /// 这样可以保证门能正常连接到房间内部
    /// </summary>
    private void ClearDoorAreas()
    {
        int patchWidth = Width - 2;
        
        foreach (var door in connected)
        {
            if (door == null) continue;
            
            // 计算门在patch坐标系中的位置
            int localX = door.position.x - left - 1;
            int localY = door.position.y - top - 1;
            
            // 根据门的位置，清空门内侧2格
            if (door.position.x == left)
            {
                // 左边界的门，清空右侧
                SafeSetPatch(localX + 1, localY, false);
                SafeSetPatch(localX + 2, localY, false);
            }
            else if (door.position.x == right)
            {
                // 右边界的门，清空左侧
                SafeSetPatch(localX - 1, localY, false);
                SafeSetPatch(localX - 2, localY, false);
            }
            else if (door.position.y == top)
            {
                // 上边界的门，清空下侧
                SafeSetPatch(localX, localY + 1, false);
                SafeSetPatch(localX, localY + 2, false);
            }
            else if (door.position.y == bottom)
            {
                // 下边界的门，清空上侧
                SafeSetPatch(localX, localY - 1, false);
                SafeSetPatch(localX, localY - 2, false);
            }
        }
    }
    
    /// <summary>
    /// 安全地设置patch值（带边界检查）
    /// </summary>
    private void SafeSetPatch(int x, int y, bool value)
    {
        int patchWidth = Width - 2;
        int patchHeight = Height - 2;
        
        if (x >= 0 && x < patchWidth && y >= 0 && y < patchHeight)
        {
            patch[x + y * patchWidth] = value;
        }
    }
    
    /// <summary>
    /// 获取可通行区域数组
    /// </summary>
    private bool[] GetWalkableArea()
    {
        bool[] walkable = new bool[patch.Length];
        for (int i = 0; i < patch.Length; i++)
        {
            walkable[i] = !patch[i];  // patch=true表示墙，walkable=false
        }
        return walkable;
    }
    
    /// <summary>
    /// 清理对角边缘
    /// 移除只通过对角线连接的patch区域，让图案更整洁
    /// </summary>
    private void CleanDiagonalEdges()
    {
        if (patch == null) return;
        
        int patchWidth = Width - 2;
        
        for (int i = 0; i < patch.Length - patchWidth; i++)
        {
            if (!patch[i]) continue;
            
            // 检查左下角
            if (i % patchWidth != 0)
            {
                int downLeft = i - 1 + patchWidth;
                if (downLeft < patch.Length && patch[downLeft])
                {
                    // 如果左下角是墙，但左边和下边都不是墙，则移除左下角
                    if (!(patch[i - 1] || patch[i + patchWidth]))
                    {
                        patch[downLeft] = false;
                    }
                }
            }
            
            // 检查右下角
            if ((i + 1) % patchWidth != 0)
            {
                int downRight = i + 1 + patchWidth;
                if (downRight < patch.Length && patch[downRight])
                {
                    // 如果右下角是墙，但右边和下边都不是墙，则移除右下角
                    if (!(patch[i + 1] || patch[i + patchWidth]))
                    {
                        patch[downRight] = false;
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// 绘制地形到Level
    /// 子类可以重写此方法添加额外装饰
    /// </summary>
    protected virtual bool PaintTerrain(Level level)
    {
        for (int x = left + 1; x < right; x++)
        {
            for (int y = top + 1; y < bottom; y++)
            {
                int patchIdx = XYToPatchIdx(x - left - 1, y - top - 1);
                
                if (patchIdx >= 0 && patchIdx < patch.Length)
                {
                    if (patch[patchIdx])
                    {
                        // 墙
                        level.SetTerrain(x, y, Terrain.Wall);
                    }
                    else
                    {
                        // 地板
                        level.SetTerrain(x, y, Terrain.Floor);
                    }
                }
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// 将房间坐标转换为patch索引
    /// </summary>
    protected int XYToPatchIdx(int x, int y)
    {
        int patchWidth = Width - 2;
        return x + y * patchWidth;
    }
    
    /// <summary>
    /// 填充patch区域为指定地形（便捷方法）
    /// </summary>
    protected void FillPatch(Level level, Terrain terrain)
    {
        for (int x = left + 1; x < right; x++)
        {
            for (int y = top + 1; y < bottom; y++)
            {
                int patchIdx = XYToPatchIdx(x - left - 1, y - top - 1);
                
                if (patchIdx >= 0 && patchIdx < patch.Length && patch[patchIdx])
                {
                    level.SetTerrain(x, y, terrain);
                }
            }
        }
    }
}

