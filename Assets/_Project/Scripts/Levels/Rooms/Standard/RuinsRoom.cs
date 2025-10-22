using UnityEngine;

/// <summary>
/// 废墟房间 - 使用patch生成破碎的墙壁和瓦砾
/// </summary>
public class RuinsRoom : PatchRoom
{
    protected override float FillPercent => 0.5f;   // 50%的墙/瓦砾
    protected override int ClusteringPasses => 3;    // 3次聚类
    protected override bool EnsurePath => true;      // 必须保证连通
    
    protected override bool PaintTerrain(Level level)
    {
        // 先绘制基础地形
        base.PaintTerrain(level);
        
        // 在墙区域添加变化
        for (int x = left + 1; x < right; x++)
        {
            for (int y = top + 1; y < bottom; y++)
            {
                int patchIdx = XYToPatchIdx(x - left - 1, y - top - 1);
                
                if (patchIdx >= 0 && patchIdx < patch.Length)
                {
                    if (patch[patchIdx])  // 墙区域
                    {
                        // 10%概率变成书架（代表废墟中的残存物品）
                        if (Random.value < 0.1f)
                        {
                            level.SetTerrain(x, y, Terrain.Bookshelf);
                        }
                    }
                }
            }
        }
        
        return true;
    }
}

