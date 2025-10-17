using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

/// <summary>
/// 关卡渲染器
/// 负责将Level的地形数据渲染到Unity Tilemap
/// </summary>
public class LevelRenderer : Singleton<LevelRenderer>
{
    [Header("Tilemap引用")]
    [SerializeField] private Tilemap groundTilemap;      // 地板层
    [SerializeField] private Tilemap wallTilemap;        // 墙壁层
    [SerializeField] private Tilemap decorationTilemap;  // 装饰层
    
    [Header("Tile资源")]
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase wallDecoTile;
    [SerializeField] private TileBase waterTile;
    [SerializeField] private TileBase grassTile;
    [SerializeField] private TileBase highGrassTile;
    [SerializeField] private TileBase entranceTile;
    [SerializeField] private TileBase exitTile;
    [SerializeField] private TileBase doorTile;
    [SerializeField] private TileBase trapTile;
    
    // Tile映射字典
    private Dictionary<Terrain, TileBase> terrainToTileMap;
    
    protected override void Awake()
    {
        base.Awake();
        InitializeTileMapping();
    }
    
    /// <summary>
    /// 初始化地形到Tile的映射
    /// </summary>
    private void InitializeTileMapping()
    {
        terrainToTileMap = new Dictionary<Terrain, TileBase>
        {
            { Terrain.Floor, floorTile },
            { Terrain.Wall, wallTile },
            { Terrain.WallDeco, wallDecoTile },
            { Terrain.Water, waterTile },
            { Terrain.Grass, grassTile },
            { Terrain.HighGrass, highGrassTile },
            { Terrain.Entrance, entranceTile },
            { Terrain.Exit, exitTile },
            { Terrain.DoorOpen, doorTile },
            { Terrain.Trap, trapTile },
            { Terrain.Empty, null } // 空地形不渲染
        };
    }
    
    /// <summary>
    /// 渲染关卡
    /// </summary>
    public void RenderLevel(Level level)
    {
        if (level == null)
        {
            Debug.LogError("Cannot render null level");
            return;
        }
        
        // 清除现有Tilemap
        ClearAllTilemaps();
        
        // 检查Tilemap是否配置
        if (!ValidateTilemaps())
        {
            Debug.LogError("Tilemaps not properly configured!");
            return;
        }
        
        Debug.Log($"Rendering level: {level.Width}x{level.Height}");
        
        // 渲染所有地形
        for (int x = 0; x < level.Width; x++)
        {
            for (int y = 0; y < level.Height; y++)
            {
                Terrain terrain = level.GetTerrain(x, y);
                RenderTerrain(x, y, terrain);
            }
        }
        
        Debug.Log("Level rendering completed");
    }
    
    /// <summary>
    /// 渲染单个地形
    /// </summary>
    private void RenderTerrain(int x, int y, Terrain terrain)
    {
        Vector3Int tilePos = new Vector3Int(x, y, 0);
        TileBase tile = GetTileForTerrain(terrain);
        
        // 根据地形类型选择合适的Tilemap层
        Tilemap targetTilemap = GetTargetTilemap(terrain);
        
        if (targetTilemap != null && tile != null)
        {
            targetTilemap.SetTile(tilePos, tile);
        }
    }
    
    /// <summary>
    /// 获取地形对应的Tile
    /// </summary>
    private TileBase GetTileForTerrain(Terrain terrain)
    {
        if (terrainToTileMap.TryGetValue(terrain, out TileBase tile))
        {
            return tile;
        }
        
        // 默认返回地板Tile
        return floorTile;
    }
    
    /// <summary>
    /// 根据地形类型选择目标Tilemap
    /// </summary>
    private Tilemap GetTargetTilemap(Terrain terrain)
    {
        switch (terrain)
        {
            case Terrain.Wall:
                return wallTilemap;
                
            case Terrain.Grass:
            case Terrain.HighGrass:
            case Terrain.EmberFloor:
                return decorationTilemap;
                
            case Terrain.Entrance:
            case Terrain.Exit:
            case Terrain.DoorOpen:
                return decorationTilemap;
                
            default:
                return groundTilemap;
        }
    }
    
    /// <summary>
    /// 清除所有Tilemap
    /// </summary>
    public void ClearAllTilemaps()
    {
        if (groundTilemap != null)
            groundTilemap.ClearAllTiles();
            
        if (wallTilemap != null)
            wallTilemap.ClearAllTiles();
            
        if (decorationTilemap != null)
            decorationTilemap.ClearAllTiles();
    }
    
    /// <summary>
    /// 验证Tilemap配置
    /// </summary>
    private bool ValidateTilemaps()
    {
        bool isValid = true;
        
        if (groundTilemap == null)
        {
            Debug.LogWarning("Ground Tilemap not assigned");
            isValid = false;
        }
        
        if (wallTilemap == null)
        {
            Debug.LogWarning("Wall Tilemap not assigned");
            isValid = false;
        }
        
        if (decorationTilemap == null)
        {
            Debug.LogWarning("Decoration Tilemap not assigned");
            isValid = false;
        }
        
        return isValid;
    }
    
    /// <summary>
    /// 设置Tilemap引用（用于运行时配置）
    /// </summary>
    public void SetTilemaps(Tilemap ground, Tilemap wall, Tilemap decoration)
    {
        groundTilemap = ground;
        wallTilemap = wall;
        decorationTilemap = decoration;
    }
    
    /// <summary>
    /// 批量渲染（性能优化版本）
    /// </summary>
    public void RenderLevelOptimized(Level level)
    {
        if (level == null || !ValidateTilemaps())
            return;
            
        ClearAllTilemaps();
        
        // 使用SetTilesBlock批量设置
        int width = level.Width;
        int height = level.Height;
        
        TileBase[] groundTiles = new TileBase[width * height];
        TileBase[] wallTiles = new TileBase[width * height];
        TileBase[] decoTiles = new TileBase[width * height];
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int index = y * width + x;
                Terrain terrain = level.GetTerrain(x, y);
                TileBase tile = GetTileForTerrain(terrain);
                
                // 根据类型分配到不同数组
                switch (GetTargetTilemap(terrain)?.name)
                {
                    case "GroundTilemap":
                        groundTiles[index] = tile;
                        break;
                    case "WallTilemap":
                        wallTiles[index] = tile;
                        break;
                    case "DecorationTilemap":
                        decoTiles[index] = tile;
                        break;
                }
            }
        }
        
        // 批量设置Tiles
        BoundsInt bounds = new BoundsInt(0, 0, 0, width, height, 1);
        groundTilemap.SetTilesBlock(bounds, groundTiles);
        wallTilemap.SetTilesBlock(bounds, wallTiles);
        decorationTilemap.SetTilesBlock(bounds, decoTiles);
        
        Debug.Log("Level rendered with optimization");
    }
}

