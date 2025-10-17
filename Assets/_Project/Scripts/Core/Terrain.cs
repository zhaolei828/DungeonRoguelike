/// <summary>
/// 地形类型枚举
/// 对应Shattered Pixel Dungeon中的地形系统
/// </summary>
public enum Terrain
{
    // 基础地形
    Empty = 0,          // 空地/虚空
    Floor = 1,          // 地板
    Grass = 2,          // 草地
    
    // 墙壁
    Wall = 3,           // 普通墙壁
    DoorClosed = 4,     // 关闭的门
    DoorOpen = 5,       // 打开的门
    
    // 水体
    Water = 6,          // 水
    DeepWater = 7,      // 深水
    
    // 特殊地形
    Chasm = 8,          // 深渊/深坑
    HighGrass = 9,      // 高草
    
    // 楼梯和传送
    Entrance = 10,      // 入口
    Exit = 11,          // 出口
    StairsDown = 12,    // 下楼梯
    StairsUp = 13,      // 上楼梯
    
    // 陷阱相关
    SecretTrap = 14,    // 隐藏陷阱
    Trap = 15,          // 激活的陷阱
    InactiveTrap = 16,  // 失效的陷阱
    
    // 特殊功能
    Sign = 17,          // 标志
    WellWater = 18,     // 水井
    Statue = 19,        // 雕像
    StatueSP = 20,      // 特殊雕像
    Bookshelf = 21,     // 书架
    Barricade = 22,     // 路障
    Pedestal = 23,      // 基座
    
    // 区域特有地形
    EmberFloor = 24,    // 余烬地板（Caves）
    Lava = 25,          // 熔岩（Caves）
    
    // 锁定门
    LockedDoor = 26,    // 锁定的门
    CrystalDoor = 27,   // 水晶门
    
    // 其他
    Alchemy = 28,       // 炼金台
    
    // 自定义扩展（Unity版本特有）
    Decoration = 100,   // 装饰物
    Spawner = 101,      // 生成点
}

/// <summary>
/// 地形属性和工具方法
/// </summary>
public static class TerrainProperties
{
    /// <summary>
    /// 检查地形是否可通行
    /// </summary>
    /// <param name="terrain">地形类型</param>
    /// <returns>是否可通行</returns>
    public static bool IsPassable(Terrain terrain)
    {
        switch (terrain)
        {
            case Terrain.Floor:
            case Terrain.Grass:
            case Terrain.DoorOpen:
            case Terrain.Entrance:
            case Terrain.Exit:
            case Terrain.StairsDown:
            case Terrain.StairsUp:
            case Terrain.HighGrass:
            case Terrain.EmberFloor:
            case Terrain.Sign:
            case Terrain.WellWater:
            case Terrain.Pedestal:
            case Terrain.Alchemy:
            case Terrain.Decoration:
            case Terrain.Spawner:
                return true;
                
            case Terrain.Water:
            case Terrain.DeepWater:
                return true; // 可以游泳通过，但可能有特殊效果
                
            default:
                return false;
        }
    }
    
    /// <summary>
    /// 检查地形是否透明（不阻挡视线）
    /// </summary>
    /// <param name="terrain">地形类型</param>
    /// <returns>是否透明</returns>
    public static bool IsTransparent(Terrain terrain)
    {
        switch (terrain)
        {
            case Terrain.Wall:
            case Terrain.DoorClosed:
            case Terrain.LockedDoor:
            case Terrain.CrystalDoor:
            case Terrain.Statue:
            case Terrain.StatueSP:
            case Terrain.Bookshelf:
            case Terrain.Barricade:
                return false;
                
            default:
                return true;
        }
    }
    
    /// <summary>
    /// 检查地形是否是门
    /// </summary>
    /// <param name="terrain">地形类型</param>
    /// <returns>是否是门</returns>
    public static bool IsDoor(Terrain terrain)
    {
        return terrain == Terrain.DoorClosed || 
               terrain == Terrain.DoorOpen || 
               terrain == Terrain.LockedDoor || 
               terrain == Terrain.CrystalDoor;
    }
    
    /// <summary>
    /// 检查地形是否是楼梯
    /// </summary>
    /// <param name="terrain">地形类型</param>
    /// <returns>是否是楼梯</returns>
    public static bool IsStairs(Terrain terrain)
    {
        return terrain == Terrain.StairsDown || 
               terrain == Terrain.StairsUp || 
               terrain == Terrain.Entrance || 
               terrain == Terrain.Exit;
    }
    
    /// <summary>
    /// 检查地形是否是陷阱
    /// </summary>
    /// <param name="terrain">地形类型</param>
    /// <returns>是否是陷阱</returns>
    public static bool IsTrap(Terrain terrain)
    {
        return terrain == Terrain.SecretTrap || 
               terrain == Terrain.Trap || 
               terrain == Terrain.InactiveTrap;
    }
    
    /// <summary>
    /// 检查地形是否是水
    /// </summary>
    /// <param name="terrain">地形类型</param>
    /// <returns>是否是水</returns>
    public static bool IsWater(Terrain terrain)
    {
        return terrain == Terrain.Water || 
               terrain == Terrain.DeepWater || 
               terrain == Terrain.WellWater;
    }
    
    /// <summary>
    /// 检查地形是否危险（会造成伤害）
    /// </summary>
    /// <param name="terrain">地形类型</param>
    /// <returns>是否危险</returns>
    public static bool IsDangerous(Terrain terrain)
    {
        return terrain == Terrain.Chasm || 
               terrain == Terrain.Lava || 
               terrain == Terrain.Trap;
    }
    
    /// <summary>
    /// 获取地形的移动消耗
    /// </summary>
    /// <param name="terrain">地形类型</param>
    /// <returns>移动消耗（1.0为正常）</returns>
    public static float GetMovementCost(Terrain terrain)
    {
        switch (terrain)
        {
            case Terrain.HighGrass:
                return 1.5f; // 高草减慢移动
                
            case Terrain.Water:
                return 2.0f; // 水中移动较慢
                
            case Terrain.DeepWater:
                return 3.0f; // 深水移动很慢
                
            case Terrain.EmberFloor:
                return 1.2f; // 余烬地板稍慢
                
            default:
                return 1.0f; // 正常移动
        }
    }
    
    /// <summary>
    /// 获取地形的显示名称
    /// </summary>
    /// <param name="terrain">地形类型</param>
    /// <returns>显示名称</returns>
    public static string GetDisplayName(Terrain terrain)
    {
        switch (terrain)
        {
            case Terrain.Empty: return "虚空";
            case Terrain.Floor: return "地板";
            case Terrain.Grass: return "草地";
            case Terrain.Wall: return "墙壁";
            case Terrain.DoorClosed: return "门（关闭）";
            case Terrain.DoorOpen: return "门（打开）";
            case Terrain.Water: return "水";
            case Terrain.DeepWater: return "深水";
            case Terrain.Chasm: return "深渊";
            case Terrain.HighGrass: return "高草";
            case Terrain.Entrance: return "入口";
            case Terrain.Exit: return "出口";
            case Terrain.StairsDown: return "下楼梯";
            case Terrain.StairsUp: return "上楼梯";
            case Terrain.Trap: return "陷阱";
            case Terrain.WellWater: return "水井";
            case Terrain.Statue: return "雕像";
            case Terrain.Bookshelf: return "书架";
            case Terrain.Lava: return "熔岩";
            case Terrain.LockedDoor: return "锁定的门";
            case Terrain.Alchemy: return "炼金台";
            default: return terrain.ToString();
        }
    }
}
