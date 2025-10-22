using UnityEngine;

/// <summary>
/// 柱子房间 - 使用patch生成柱子阵列
/// </summary>
public class PillarsRoom : PatchRoom
{
    protected override float FillPercent => 0.35f;  // 35%的柱子
    protected override int ClusteringPasses => 5;    // 5次聚类，形成更聚集的柱子
    protected override bool EnsurePath => true;      // 必须保证连通
    
    protected override bool PaintTerrain(Level level)
    {
        // 先绘制基础地形
        base.PaintTerrain(level);
        
        // 将墙区域变成雕像（代表柱子）
        for (int x = left + 1; x < right; x++)
        {
            for (int y = top + 1; y < bottom; y++)
            {
                int patchIdx = XYToPatchIdx(x - left - 1, y - top - 1);
                
                if (patchIdx >= 0 && patchIdx < patch.Length)
                {
                    if (patch[patchIdx])  // 墙区域
                    {
                        // 80%概率变成雕像（柱子）
                        if (Random.value < 0.8f)
                        {
                            level.SetTerrain(x, y, Terrain.Statue);
                        }
                    }
                }
            }
        }
        
        return true;
    }
}

