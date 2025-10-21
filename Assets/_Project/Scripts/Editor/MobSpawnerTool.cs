using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

/// <summary>
/// Mob生成工具 - 在编辑器中快速生成怪物
/// </summary>
public class MobSpawnerTool
{
    [MenuItem("Tools/Dungeon/Spawn Mob/Rat")]
    public static void SpawnRat()
    {
        SpawnMob<Rat>("老鼠", "Rat");
    }
    
    [MenuItem("Tools/Dungeon/Spawn Mob/Bat")]
    public static void SpawnBat()
    {
        SpawnMob<Bat>("蝙蝠", "Bat");
    }
    
    [MenuItem("Tools/Dungeon/Spawn Mob/Spider")]
    public static void SpawnSpider()
    {
        SpawnMob<Spider>("蜘蛛", "Spider");
    }
    
    [MenuItem("Tools/Dungeon/Spawn Mob/Goblin")]
    public static void SpawnGoblin()
    {
        SpawnMob<Goblin>("地精", "Goblin");
    }
    
    [MenuItem("Tools/Dungeon/Spawn Mob/Orc")]
    public static void SpawnOrc()
    {
        SpawnMob<Orc>("兽人", "Orc");
    }
    
    [MenuItem("Tools/Dungeon/Spawn Mob/Random Mix (5个怪物混合)")]
    public static void SpawnRandomMix()
    {
        Level currentLevel = Object.FindAnyObjectByType<Level>();
        
        // 在5个不同位置生成不同类型的怪物
        SpawnMobAtRandomPosition<Rat>("老鼠", currentLevel);
        SpawnMobAtRandomPosition<Bat>("蝙蝠", currentLevel);
        SpawnMobAtRandomPosition<Spider>("蜘蛛", currentLevel);
        SpawnMobAtRandomPosition<Goblin>("地精", currentLevel);
        SpawnMobAtRandomPosition<Orc>("兽人", currentLevel);
        
        string message = currentLevel != null 
            ? "已生成5个怪物混合小队！\n按Play进入游戏体验战斗系统。"
            : "已生成5个怪物混合小队！\n位置随机分布在场景中。\n按Play后需要手动调整位置。";
        
        Debug.Log("<color=green>✓ 已生成5个怪物混合小队</color>");
        EditorUtility.DisplayDialog("成功", message, "确定");
    }
    
    /// <summary>
    /// 生成指定类型的怪物
    /// </summary>
    private static void SpawnMob<T>(string mobName, string prefabName) where T : Mob
    {
        // 检查是否在编辑模式下
        if (EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("提示", "请先停止Play模式！", "确定");
            return;
        }
        
        Level currentLevel = Object.FindAnyObjectByType<Level>();
        if (currentLevel == null)
        {
            EditorUtility.DisplayDialog("错误", "场景中未找到Level组件！", "确定");
            return;
        }
        
        // 创建怪物GameObject
        GameObject mobGO = new GameObject(mobName);
        
        // 添加Mob组件
        Mob mob = mobGO.AddComponent<T>();
        
        // 添加SpriteRenderer用于显示
        SpriteRenderer spriteRenderer = mobGO.AddComponent<SpriteRenderer>();
        // 尝试加载sprite（如果没有，Mob.Start()会创建占位符）
        spriteRenderer.sprite = TryLoadMobSprite(prefabName);
        
        // 添加BoxCollider2D用于碰撞
        BoxCollider2D collider = mobGO.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1, 1);
        collider.isTrigger = true;
        
        // 在随机位置生成
        Vector2Int randomPos = GetRandomPassablePosition(currentLevel);
        mobGO.transform.position = new Vector3(randomPos.x + 0.5f, randomPos.y + 0.5f, 0);
        
        Debug.Log($"<color=green>✓ 已生成 {mobName} at ({randomPos.x}, {randomPos.y})</color>");
        
        // 标记场景为已修改
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }
    
    /// <summary>
    /// 在随机可通行位置生成怪物
    /// </summary>
    private static void SpawnMobAtRandomPosition<T>(string mobName, Level level) where T : Mob
    {
        GameObject mobGO = new GameObject(mobName);
        Mob mob = mobGO.AddComponent<T>();
        
        SpriteRenderer spriteRenderer = mobGO.AddComponent<SpriteRenderer>();
        // 尝试加载sprite（如果没有，Mob.Start()会创建占位符）
        spriteRenderer.sprite = TryLoadMobSprite(typeof(T).Name);
        
        BoxCollider2D collider = mobGO.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1, 1);
        collider.isTrigger = true;
        
        Vector2Int randomPos = GetRandomPassablePosition(level);
        mobGO.transform.position = new Vector3(randomPos.x + 0.5f, randomPos.y + 0.5f, 0);
    }
    
    /// <summary>
    /// 尝试从Resources或AssetDatabase加载怪物sprite
    /// </summary>
    private static Sprite TryLoadMobSprite(string mobTypeName)
    {
        // 尝试从多个可能的路径加载
        string[] possiblePaths = new string[]
        {
            $"Assets/_Project/Art/Sprites/Enemies/{mobTypeName}.png",
            $"Assets/_Project/Sprites/Enemies/{mobTypeName}.png",
            $"Assets/_Project/Art/Sprites/Characters/{mobTypeName}.png",
        };
        
        foreach (string path in possiblePaths)
        {
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite != null)
            {
                Debug.Log($"<color=cyan>✓ 加载怪物sprite: {path}</color>");
                return sprite;
            }
        }
        
        // 没找到sprite，返回null（Mob.Start()会创建占位符）
        Debug.Log($"<color=yellow>⚠ 未找到 {mobTypeName} 的sprite，将使用占位符</color>");
        return null;
    }
    
    /// <summary>
    /// 获取一个随机的可通行位置
    /// </summary>
    private static Vector2Int GetRandomPassablePosition(Level level)
    {
        // 如果没有Level，使用简单的随机位置
        if (level == null)
        {
            return new Vector2Int(
                Random.Range(5, 20),
                Random.Range(5, 20)
            );
        }
        
        Vector2Int pos;
        int attempts = 0;
        
        do
        {
            pos = new Vector2Int(
                Random.Range(5, level.Width - 5),
                Random.Range(5, level.Height - 5)
            );
            attempts++;
        } while (!level.IsPassable(pos) && attempts < 100);
        
        if (attempts >= 100)
        {
            Debug.LogWarning("未能找到可通行位置，使用默认位置");
            pos = new Vector2Int(level.Width / 2, level.Height / 2);
        }
        
        return pos;
    }
}
