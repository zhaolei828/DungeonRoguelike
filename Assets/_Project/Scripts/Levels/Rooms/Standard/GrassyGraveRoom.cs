using UnityEngine;

/// <summary>
/// 草地墓地房间 - 使用patch生成有机的草地和墓碑图案
/// </summary>
public class GrassyGraveRoom : PatchRoom
{
    protected override float FillPercent => 0.45f;  // 45%的墙/墓碑
    protected override int ClusteringPasses => 4;    // 4次聚类，形成自然的聚集效果
    protected override bool EnsurePath => true;      // 必须保证连通
    
    protected override bool PaintTerrain(Level level)
    {
        // 先绘制基础地形（墙和地板）
        base.PaintTerrain(level);
        
        // 在地板上随机添加草地
        for (int x = left + 1; x < right; x++)
        {
            for (int y = top + 1; y < bottom; y++)
            {
                int patchIdx = XYToPatchIdx(x - left - 1, y - top - 1);
                
                if (patchIdx >= 0 && patchIdx < patch.Length)
                {
                    if (!patch[patchIdx])  // 地板区域
                    {
                        // 30%概率生成草地
                        if (Random.value < 0.3f)
                        {
                            level.SetTerrain(x, y, Terrain.Grass);
                        }
                    }
                    else  // 墙区域可以变成墓碑
                    {
                        // 20%概率变成墓碑而不是墙
                        if (Random.value < 0.2f)
                        {
                            level.SetTerrain(x, y, Terrain.Statue);  // 用雕像代表墓碑
                        }
                    }
                }
            }
        }
        
        return true;
    }
}

