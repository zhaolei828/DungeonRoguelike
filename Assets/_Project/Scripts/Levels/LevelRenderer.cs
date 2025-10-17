using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

/// <summary>
/// �ؿ���Ⱦ��
/// ����Level�ĵ���������Ⱦ��Unity Tilemap
/// </summary>
public class LevelRenderer : Singleton<LevelRenderer>
{
    [Header("Tilemap����")]
    [SerializeField] private Tilemap groundTilemap;      // �ذ��
    [SerializeField] private Tilemap wallTilemap;        // ǽ�ڲ�
    [SerializeField] private Tilemap decorationTilemap;  // װ�β�
    
    [Header("Tile��Դ")]
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
    
    // Tileӳ���ֵ�
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
    /// ��ʼ�����ε�Tile��ӳ��
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
            { Terrain.Decoration, wallDecoTile }, // װ����ʹ��wallDecoTile
            { Terrain.Empty, null } // �յ��β���Ⱦ
        };
    }
    
    /// <summary>
    /// ��Ⱦ�ؿ�
    /// </summary>
    public void RenderLevel(Level level)
    {
        if (level == null)
        {
            Debug.LogError("Cannot render null level");
            return;
        }
        
        // �������Tilemap
        ClearAllTilemaps();
        
        // ���Tilemap�Ƿ�����
        if (!ValidateTilemaps())
        {
            Debug.LogError("Tilemaps not properly configured!");
            return;
        }
        
        Debug.Log($"Rendering level: {level.Width}x{level.Height}");
        
        // ��Ⱦ���е���
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
    /// ��Ⱦ��������
    /// </summary>
    private void RenderTerrain(int x, int y, Terrain terrain)
    {
        Vector3Int tilePos = new Vector3Int(x, y, 0);
        TileBase tile = GetTileForTerrain(terrain);
        
        // ���ݵ�������ѡ����ʵ�Tilemap��
        Tilemap targetTilemap = GetTargetTilemap(terrain);
        
        if (targetTilemap != null && tile != null)
        {
            targetTilemap.SetTile(tilePos, tile);
        }
    }
    
    /// <summary>
    /// ��ȡ���ζ�Ӧ��Tile
    /// </summary>
    private TileBase GetTileForTerrain(Terrain terrain)
    {
        if (terrainToTileMap.TryGetValue(terrain, out TileBase tile))
        {
            return tile;
        }
        
        // Ĭ�Ϸ��صذ�Tile
        return floorTile;
    }
    
    /// <summary>
    /// ���ݵ�������ѡ��Ŀ��Tilemap
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
    /// �������Tilemap
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
    /// ��֤Tilemap����
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
    /// ����Tilemap���ã���������ʱ���ã�
    /// </summary>
    public void SetTilemaps(Tilemap ground, Tilemap wall, Tilemap decoration)
    {
        groundTilemap = ground;
        wallTilemap = wall;
        decorationTilemap = decoration;
    }
    
    /// <summary>
    /// ������Ⱦ�������Ż��汾��
    /// </summary>
    public void RenderLevelOptimized(Level level)
    {
        if (level == null || !ValidateTilemaps())
            return;
            
        ClearAllTilemaps();
        
        // ʹ��SetTilesBlock��������
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
                
                // �������ͷ��䵽��ͬ����
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
        
        // ��������Tiles
        BoundsInt bounds = new BoundsInt(0, 0, 0, width, height, 1);
        groundTilemap.SetTilesBlock(bounds, groundTiles);
        wallTilemap.SetTilesBlock(bounds, wallTiles);
        decorationTilemap.SetTilesBlock(bounds, decoTiles);
        
        Debug.Log("Level rendered with optimization");
    }
}

