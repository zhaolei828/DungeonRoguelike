using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

/// <summary>
/// 关卡渲染器
/// 将Level的地形数据渲染到Unity Tilemap
/// </summary>
public class LevelRenderer : Singleton<LevelRenderer>
{
    [Header("Tilemap设置")]
    [SerializeField] private Tilemap groundTilemap;      // 地面Tilemap
    [SerializeField] private Tilemap wallTilemap;        // 墙壁Tilemap
    [SerializeField] private Tilemap decorationTilemap;  // 装饰Tilemap
    
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
    
    // Tile映射表
    private Dictionary<Terrain, TileBase> terrainToTileMap;
    
    protected override void Awake()
    {
        base.Awake();
        AutoFindTilemaps();
        InitializeTileMapping();
    }
    
    /// <summary>
    /// 自动查找场景中的Tilemap
    /// </summary>
    private void AutoFindTilemaps()
    {
        // 如果已经手动分配，则不自动查找
        if (groundTilemap != null && wallTilemap != null && decorationTilemap != null)
            return;
        
        // 查找所有Tilemap
        Tilemap[] tilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        
        foreach (Tilemap tilemap in tilemaps)
        {
            string name = tilemap.gameObject.name.ToLower();
            
            if (name.Contains("ground") && groundTilemap == null)
            {
                groundTilemap = tilemap;
                Debug.Log($"自动找到GroundTilemap: {tilemap.gameObject.name}");
            }
            else if (name.Contains("wall") && wallTilemap == null)
            {
                wallTilemap = tilemap;
                Debug.Log($"自动找到WallTilemap: {tilemap.gameObject.name}");
            }
            else if (name.Contains("decoration") && decorationTilemap == null)
            {
                decorationTilemap = tilemap;
                Debug.Log($"自动找到DecorationTilemap: {tilemap.gameObject.name}");
            }
        }
        
        // 如果还是没找到，尝试按顺序分配
        if (tilemaps.Length >= 3 && (groundTilemap == null || wallTilemap == null || decorationTilemap == null))
        {
            if (groundTilemap == null) groundTilemap = tilemaps[0];
            if (wallTilemap == null && tilemaps.Length > 1) wallTilemap = tilemaps[1];
            if (decorationTilemap == null && tilemaps.Length > 2) decorationTilemap = tilemaps[2];
            
            Debug.LogWarning("未找到匹配名称的Tilemap，已按顺序自动分配");
        }
    }
    
    /// <summary>
    /// 初始化Tile映射表
    /// </summary>
    private void InitializeTileMapping()
    {
        terrainToTileMap = new Dictionary<Terrain, TileBase>
        {
            { Terrain.Floor, floorTile },
            { Terrain.Wall, wallTile },
            { Terrain.Water, waterTile },
            { Terrain.Grass, grassTile },
            { Terrain.HighGrass, highGrassTile },
            { Terrain.Entrance, entranceTile },
            { Terrain.Exit, exitTile },
            { Terrain.DoorOpen, doorTile },
            { Terrain.Trap, trapTile },
            { Terrain.Decoration, wallDecoTile }, // 装饰使用wallDecoTile
            { Terrain.Empty, null } // 空地不渲染
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
        
        // 清空所有Tilemap
        ClearAllTilemaps();
        
        // 验证Tilemap是否正确
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
    /// 渲染地形
    /// </summary>
    private void RenderTerrain(int x, int y, Terrain terrain)
    {
        Vector3Int tilePos = new Vector3Int(x, y, 0);
        TileBase tile = GetTileForTerrain(terrain);
        
        // 根据地形选择对应的Tilemap
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
        /// 根据地形选择对应的Tilemap
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
    /// 清空所有Tilemap
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
    /// 验证Tilemap是否正确
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
    /// 设置Tilemap引用
    /// </summary>
    public void SetTilemaps(Tilemap ground, Tilemap wall, Tilemap decoration)
    {
        groundTilemap = ground;
        wallTilemap = wall;
        decorationTilemap = decoration;
    }
    
    /// <summary>
    /// 渲染关卡（优化版本）
    /// </summary>
    public void RenderLevelOptimized(Level level)
    {
        if (level == null || !ValidateTilemaps())
            return;
            
        ClearAllTilemaps();
        
        // 使用SetTilesBlock批量设置Tile
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
                
                // 将Tile分配到对应的Tilemap
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
        
        // 设置Tilemap的Tile
        BoundsInt bounds = new BoundsInt(0, 0, 0, width, height, 1);
        groundTilemap.SetTilesBlock(bounds, groundTiles);
        wallTilemap.SetTilesBlock(bounds, wallTiles);
        decorationTilemap.SetTilesBlock(bounds, decoTiles);
        
        Debug.Log("Level rendered with optimization");
    }
}

