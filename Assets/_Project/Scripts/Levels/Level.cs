using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 关卡基类
/// 所有关卡类型的基类
/// </summary>
public abstract class Level : MonoBehaviour
{
    [Header("关卡基础设置")]
    [SerializeField] protected int width = 32;
    [SerializeField] protected int height = 32;
    [SerializeField] protected int depth = 1;
    [SerializeField] protected LevelType levelType = LevelType.Sewers;
    
    // 地形数据
    protected Terrain[,] terrainMap;
    protected Terrain[] mapAs1DArray; // 1D数组视图（用于兼容Room系统）
    
    // 关卡对象引用
    protected Transform tilemapContainer;
    protected Transform actorContainer;
    protected Transform itemContainer;
    
    // 关卡状态
    protected bool _isGenerated = false;
    protected bool _isInitialized = false;
    
    // 特殊位置
    protected Vector2Int entrancePos = Vector2Int.zero;
    protected Vector2Int exitPos = Vector2Int.zero;
    protected List<Vector2Int> roomPositions = new List<Vector2Int>();
    
    // 房间列表（用于生成系统）
    protected List<Room> rooms = new List<Room>();
    
    // 事件
    public System.Action<Level> OnLevelGenerated;
    public System.Action<Level> OnLevelCleared;
    
    #region Properties
    
    /// <summary>
    /// 关卡宽度
    /// </summary>
    public int Width => width;
    
    /// <summary>
    /// 关卡高度
    /// </summary>
    public int Height => height;
    
    /// <summary>
    /// 关卡深度
    /// </summary>
    public int Depth => depth;
    
    /// <summary>
    /// 关卡类型
    /// </summary>
    public LevelType LevelType => levelType;
    
    /// <summary>
    /// 是否已生成
    /// </summary>
    public bool IsGenerated => _isGenerated;
    
    /// <summary>
    /// 是否已初始化
    /// </summary>
    public bool IsInitialized => _isInitialized;
    
    /// <summary>
    /// 入口位置
    /// </summary>
    public Vector2Int EntrancePos 
    { 
        get => entrancePos;
        set => entrancePos = value;
    }
    
    /// <summary>
    /// 出口位置
    /// </summary>
    public Vector2Int ExitPos 
    { 
        get => exitPos;
        set => exitPos = value;
    }
    
    /// <summary>
    /// 房间位置列表
    /// </summary>
    public List<Vector2Int> RoomPositions => new List<Vector2Int>(roomPositions);
    
    /// <summary>
    /// 地图数据（1D数组访问，用于Room系统）
    /// </summary>
    public Terrain[] Map 
    { 
        get
        {
            // 创建或更新1D数组视图
            if (mapAs1DArray == null || mapAs1DArray.Length != width * height)
            {
                mapAs1DArray = new Terrain[width * height];
            }
            
            // 同步2D数组到1D数组
            if (terrainMap != null)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        mapAs1DArray[LevelCoord.CoordsToPos(x, y, width)] = terrainMap[x, y];
                    }
                }
            }
            
            return mapAs1DArray;
        }
        set
        {
            mapAs1DArray = value;
            
            // 同步1D数组到2D数组
            if (terrainMap == null)
            {
                terrainMap = new Terrain[width, height];
            }
            
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int pos = LevelCoord.CoordsToPos(x, y, width);
                    if (pos < mapAs1DArray.Length)
                    {
                        terrainMap[x, y] = mapAs1DArray[pos];
                    }
                }
            }
        }
    }
    
    #endregion
    
    #region Unity Lifecycle
    
    protected virtual void Awake()
    {
        // 初始化地形数组
        terrainMap = new Terrain[width, height];
        
        // 创建容器
        CreateContainers();
    }
    
    #endregion
    
    #region Initialization
    
    /// <summary>
    /// 初始化关卡
    /// </summary>
    /// <param name="levelDepth">关卡深度</param>
    /// <param name="type">关卡类型</param>
    public virtual void Initialize(int levelDepth, LevelType type)
    {
        depth = levelDepth;
        levelType = type;
        
        // 根据类型调整尺寸
        AdjustSizeForType(type);
        
        // 重新初始化地形数组
        terrainMap = new Terrain[width, height];
        
        _isInitialized = true;
        
        Debug.Log($"Level initialized: Depth {depth}, Type {type}, Size {width}x{height}");
    }
    
    /// <summary>
    /// 根据关卡类型调整尺寸
    /// </summary>
    /// <param name="type">关卡类型</param>
    protected virtual void AdjustSizeForType(LevelType type)
    {
        // 不同类型的关卡可能有不同的尺寸
        // 这里使用统一尺寸，子类可以重写
        width = 32;
        height = 32;
    }
    
    /// <summary>
    /// 创建容器对象
    /// </summary>
    protected virtual void CreateContainers()
    {
        // Tilemap容器
        if (tilemapContainer == null)
        {
            GameObject tilemapObj = new GameObject("Tilemap");
            tilemapContainer = tilemapObj.transform;
            tilemapContainer.SetParent(transform);
        }
        
        // Actor容器
        if (actorContainer == null)
        {
            GameObject actorObj = new GameObject("Actors");
            actorContainer = actorObj.transform;
            actorContainer.SetParent(transform);
        }
        
        // Item容器
        if (itemContainer == null)
        {
            GameObject itemObj = new GameObject("Items");
            itemContainer = itemObj.transform;
            itemContainer.SetParent(transform);
        }
    }
    
    #endregion
    
    #region Terrain Management
    
    /// <summary>
    /// 获取指定位置的地形
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <returns>地形类型</returns>
    public virtual Terrain GetTerrain(int x, int y)
    {
        if (!IsValidPosition(x, y))
            return Terrain.Wall; // 边界外视为墙壁
        
        return terrainMap[x, y];
    }
    
    /// <summary>
    /// 获取指定位置的地形
    /// </summary>
    /// <param name="pos">位置</param>
    /// <returns>地形类型</returns>
    public virtual Terrain GetTerrain(Vector2Int pos)
    {
        return GetTerrain(pos.x, pos.y);
    }
    
    /// <summary>
    /// 设置指定位置的地形
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <param name="terrain">地形类型</param>
    public virtual void SetTerrain(int x, int y, Terrain terrain)
    {
        if (!IsValidPosition(x, y))
            return;
        
        terrainMap[x, y] = terrain;
        
        // 通知地形变化（可以用于更新Tilemap等）
        HandleTerrainChanged(x, y, terrain);
    }
    
    /// <summary>
    /// 设置指定位置的地形
    /// </summary>
    /// <param name="pos">位置</param>
    /// <param name="terrain">地形类型</param>
    public virtual void SetTerrain(Vector2Int pos, Terrain terrain)
    {
        SetTerrain(pos.x, pos.y, terrain);
    }
    
    /// <summary>
    /// 地形变化时调用
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <param name="terrain">新地形</param>
    protected virtual void HandleTerrainChanged(int x, int y, Terrain terrain)
    {
        // 子类可以重写此方法来处理地形变化
        // 比如更新Tilemap、触发特效等
    }
    
    #endregion
    
    #region Position Validation
    
    /// <summary>
    /// 检查位置是否有效
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <returns>是否有效</returns>
    public virtual bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }
    
    /// <summary>
    /// 检查位置是否有效
    /// </summary>
    /// <param name="pos">位置</param>
    /// <returns>是否有效</returns>
    public virtual bool IsValidPosition(Vector2Int pos)
    {
        return IsValidPosition(pos.x, pos.y);
    }
    
    /// <summary>
    /// 检查位置是否可通行
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <returns>是否可通行</returns>
    public virtual bool IsPassable(int x, int y)
    {
        if (!IsValidPosition(x, y))
            return false;
        
        return TerrainProperties.IsPassable(GetTerrain(x, y));
    }
    
    /// <summary>
    /// 检查位置是否可通行
    /// </summary>
    /// <param name="pos">位置</param>
    /// <returns>是否可通行</returns>
    public virtual bool IsPassable(Vector2Int pos)
    {
        return IsPassable(pos.x, pos.y);
    }
    
    /// <summary>
    /// 检查位置是否透明（不阻挡视线）
    /// </summary>
    /// <param name="x">X坐标</param>
    /// <param name="y">Y坐标</param>
    /// <returns>是否透明</returns>
    public virtual bool IsTransparent(int x, int y)
    {
        if (!IsValidPosition(x, y))
            return false;
        
        return TerrainProperties.IsTransparent(GetTerrain(x, y));
    }
    
    /// <summary>
    /// 检查位置是否透明
    /// </summary>
    /// <param name="pos">位置</param>
    /// <returns>是否透明</returns>
    public virtual bool IsTransparent(Vector2Int pos)
    {
        return IsTransparent(pos.x, pos.y);
    }
    
    #endregion
    
    #region Level Generation
    
    /// <summary>
    /// 生成关卡（抽象方法，子类必须实现）
    /// </summary>
    /// <returns>是否生成成功</returns>
    public abstract bool Generate();
    
    /// <summary>
    /// 标记关卡生成完成
    /// </summary>
    protected virtual void MarkAsGenerated()
    {
        _isGenerated = true;
        OnLevelGenerated?.Invoke(this);
        
        Debug.Log($"Level generation completed: {name}");
    }
    
    /// <summary>
    /// 清理关卡
    /// </summary>
    public virtual void Cleanup()
    {
        // 清理所有子对象
        if (tilemapContainer != null)
        {
            DestroyImmediate(tilemapContainer.gameObject);
        }
        
        if (actorContainer != null)
        {
            DestroyImmediate(actorContainer.gameObject);
        }
        
        if (itemContainer != null)
        {
            DestroyImmediate(itemContainer.gameObject);
        }
        
        // 重置状态
        _isGenerated = false;
        roomPositions.Clear();
        
        OnLevelCleared?.Invoke(this);
        
        Debug.Log($"Level cleaned up: {name}");
    }
    
    #endregion
    
    #region Utilities
    
    /// <summary>
    /// 获取关卡边界
    /// </summary>
    /// <returns>边界</returns>
    public virtual Bounds GetBounds()
    {
        Vector3 center = new Vector3(width * 0.5f, height * 0.5f, 0);
        Vector3 size = new Vector3(width, height, 1);
        return new Bounds(center, size);
    }
    
    /// <summary>
    /// 获取随机有效位置
    /// </summary>
    /// <returns>随机位置</returns>
    public virtual Vector2Int GetRandomValidPosition()
    {
        for (int attempts = 0; attempts < 100; attempts++)
        {
            int x = Random.Range(1, width - 1);
            int y = Random.Range(1, height - 1);
            
            if (IsPassable(x, y))
            {
                return new Vector2Int(x, y);
            }
        }
        
        // 如果找不到，返回中心位置
        return new Vector2Int(width / 2, height / 2);
    }
    
    /// <summary>
    /// 获取关卡中心位置
    /// </summary>
    /// <returns>中心位置</returns>
    public virtual Vector2Int GetCenterPosition()
    {
        return new Vector2Int(width / 2, height / 2);
    }
    
    /// <summary>
    /// 设置入口位置
    /// </summary>
    /// <param name="pos">入口位置</param>
    public virtual void SetEntrancePosition(Vector2Int pos)
    {
        entrancePos = pos;
        SetTerrain(pos, Terrain.Entrance);
    }
    
    /// <summary>
    /// 设置出口位置
    /// </summary>
    /// <param name="pos">出口位置</param>
    public virtual void SetExitPosition(Vector2Int pos)
    {
        exitPos = pos;
        SetTerrain(pos, Terrain.Exit);
    }
    
    #endregion
    
    #region Debug
    
    /// <summary>
    /// 获取关卡调试信息
    /// </summary>
    /// <returns>调试信息</returns>
    public virtual string GetDebugInfo()
    {
        return $"Level: {name}\n" +
               $"Type: {levelType}\n" +
               $"Depth: {depth}\n" +
               $"Size: {width}x{height}\n" +
               $"Generated: {IsGenerated}\n" +
               $"Initialized: {IsInitialized}\n" +
               $"Entrance: {entrancePos}\n" +
               $"Exit: {exitPos}\n" +
               $"Rooms: {roomPositions.Count}";
    }
    
    #endregion
}
