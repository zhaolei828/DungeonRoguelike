using UnityEngine;
using System.Collections;

/// <summary>
/// 重生管理器 - 处理Hero死亡和重生
/// </summary>
public class RespawnManager : MonoBehaviour
{
    private static RespawnManager _instance;
    public static RespawnManager Instance => _instance;
    
    [Header("重生设置")]
    [SerializeField] private float respawnDelay = 2f;
    [SerializeField] private bool enableRespawn = true;
    [SerializeField] private int maxRespawnCount = -1; // -1表示无限
    
    [Header("重生位置")]
    [SerializeField] private Vector2Int defaultRespawnPos = new Vector2Int(5, 5);
    [SerializeField] private bool findSafePosition = true;
    
    [Header("重生属性")]
    [SerializeField] private float respawnHealthPercent = 1f; // 重生时HP百分比
    [SerializeField] private bool clearDebuffs = true;
    
    private int currentRespawnCount = 0;
    private Vector2Int lastCheckpoint = Vector2Int.zero;
    private Coroutine currentRespawnCoroutine; // 当前重生协程
    private bool isRespawning = false; // 是否正在重生
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
    }
    
    /// <summary>
    /// Hero死亡时调用
    /// </summary>
    public void OnHeroDied(Hero hero)
    {
        // 如果已经在重生中，忽略
        if (isRespawning)
        {
            Debug.Log("<color=yellow>⚠ 重生流程已在进行中，忽略重复调用</color>");
            return;
        }
        
        if (!enableRespawn)
        {
            Debug.Log("<color=red>游戏结束！</color>");
            ShowGameOverUI();
            return;
        }
        
        // 检查重生次数限制
        if (maxRespawnCount >= 0 && currentRespawnCount >= maxRespawnCount)
        {
            Debug.Log($"<color=red>已达到最大重生次数 ({maxRespawnCount})</color>");
            ShowGameOverUI();
            return;
        }
        
        // 停止之前的重生协程（如果有）
        if (currentRespawnCoroutine != null)
        {
            StopCoroutine(currentRespawnCoroutine);
            Debug.Log("<color=yellow>⚠ 停止了之前的重生协程</color>");
        }
        
        // 开始重生流程
        isRespawning = true;
        currentRespawnCoroutine = StartCoroutine(RespawnCoroutine(hero));
    }
    
    private IEnumerator RespawnCoroutine(Hero hero)
    {
        Debug.Log("<color=yellow>Hero已死亡，准备重生...</color>");
        
        // 等待延迟
        yield return new WaitForSeconds(respawnDelay);
        
        // 查找重生位置
        Vector2Int respawnPos = FindRespawnPosition();
        
        // 重生Hero
        RespawnHero(hero, respawnPos);
        
        currentRespawnCount++;
        Debug.Log($"<color=green>✓ Hero重生成功！重生次数: {currentRespawnCount}</color>");
        
        // 重生完成，重置标记
        isRespawning = false;
        currentRespawnCoroutine = null;
    }
    
    private void RespawnHero(Hero hero, Vector2Int position)
    {
        // 如果Hero GameObject已被销毁，重新创建
        if (hero == null || hero.gameObject == null)
        {
            CreateNewHero(position);
            return;
        }
        
        // 恢复位置
        hero.pos = position;
        hero.transform.position = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
        
        // 直接设置HP（重生时）
        int respawnHP = Mathf.RoundToInt(hero.MaxHp * respawnHealthPercent);
        System.Reflection.FieldInfo hpField = typeof(Actor).GetField("hp", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (hpField != null)
        {
            hpField.SetValue(hero, respawnHP);
        }
        
        // 重新创建血条（因为Die时销毁了）
        if (HealthBarManager.Instance != null)
        {
            System.Reflection.FieldInfo healthBarField = typeof(Actor).GetField("healthBar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (healthBarField != null)
            {
                HealthBar newHealthBar = HealthBarManager.Instance.CreateHealthBar(hero.transform, hero.MaxHp, new Vector3(0, 0.8f, 0));
                healthBarField.SetValue(hero, newHealthBar);
                Debug.Log($"<color=cyan>✓ 重新创建Hero血条</color>");
            }
            else
            {
                Debug.LogError("RespawnManager: 无法找到healthBar字段！");
            }
        }
        else
        {
            Debug.LogError("RespawnManager: HealthBarManager.Instance为null！");
        }
        
        // 重新激活GameObject（如果被禁用）
        if (!hero.gameObject.activeSelf)
        {
            hero.gameObject.SetActive(true);
        }
        
        // 播放重生特效（待实现）
        PlayRespawnEffect(hero.transform.position);
        
        Debug.Log($"<color=cyan>Hero重生at ({position.x}, {position.y}), HP: {hero.Hp}/{hero.MaxHp}</color>");
    }
    
    private void CreateNewHero(Vector2Int position)
    {
        // 查找HeroSpawner
        HeroSpawner spawner = FindAnyObjectByType<HeroSpawner>();
        if (spawner == null)
        {
            GameObject spawnerGO = new GameObject("HeroSpawner");
            spawner = spawnerGO.AddComponent<HeroSpawner>();
        }
        
        // 生成新Hero（只传入职业参数）
        Hero newHero = spawner.SpawnHero(HeroClass.Warrior);
        
        if (newHero != null)
        {
            // 设置到指定位置
            newHero.pos = position;
            newHero.transform.position = new Vector3(position.x + 0.5f, position.y + 0.5f, 0);
            
            // 设置HP（新Hero已经是满血，如果需要调整再设置）
            if (respawnHealthPercent < 1f)
            {
                int respawnHP = Mathf.RoundToInt(newHero.MaxHp * respawnHealthPercent);
                System.Reflection.FieldInfo hpField = typeof(Actor).GetField("hp", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (hpField != null)
                {
                    hpField.SetValue(newHero, respawnHP);
                }
            }
            
            Debug.Log($"<color=cyan>创建新Hero at ({position.x}, {position.y})</color>");
        }
    }
    
    private Vector2Int FindRespawnPosition()
    {
        // 优先使用检查点
        if (lastCheckpoint != Vector2Int.zero)
        {
            return lastCheckpoint;
        }
        
        // 如果需要寻找安全位置
        if (findSafePosition && LevelManager.Instance?.CurrentLevel != null)
        {
            Level currentLevel = LevelManager.Instance.CurrentLevel;
            
            // 尝试在起始房间附近找一个安全位置
            for (int attempt = 0; attempt < 50; attempt++)
            {
                int x = Random.Range(3, Mathf.Min(10, currentLevel.Width - 3));
                int y = Random.Range(3, Mathf.Min(10, currentLevel.Height - 3));
                Vector2Int pos = new Vector2Int(x, y);
                
                if (currentLevel.IsPassable(pos) && IsSafePosition(pos))
                {
                    return pos;
                }
            }
        }
        
        // 使用默认位置
        return defaultRespawnPos;
    }
    
    private bool IsSafePosition(Vector2Int pos)
    {
        // 检查位置是否在地图范围内
        Level currentLevel = LevelManager.Instance?.CurrentLevel;
        if (currentLevel == null)
            return false;
        
        if (!currentLevel.IsValidPosition(pos.x, pos.y))
            return false;
        
        // 检查是否可通行
        if (!currentLevel.IsPassable(pos.x, pos.y))
            return false;
        
        // 检查是否可从入口到达（防止重生在封闭区域）
        Vector2Int entrance = currentLevel.EntrancePos;
        if (entrance != Vector2Int.zero && !PathFinder.IsReachable(currentLevel, entrance, pos))
        {
            Debug.LogWarning($"位置 {pos} 从入口 {entrance} 不可达，跳过");
            return false;
        }
        
        // 检查附近是否有敌人
        Mob[] mobs = FindObjectsByType<Mob>(FindObjectsSortMode.None);
        foreach (Mob mob in mobs)
        {
            if (mob != null)
            {
                float distance = Vector2Int.Distance(pos, mob.pos);
                if (distance < 3f) // 距离敌人至少3格
                {
                    return false;
                }
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// 设置检查点
    /// </summary>
    public void SetCheckpoint(Vector2Int position)
    {
        lastCheckpoint = position;
        Debug.Log($"<color=cyan>✓ 设置检查点: ({position.x}, {position.y})</color>");
    }
    
    /// <summary>
    /// 清除检查点
    /// </summary>
    public void ClearCheckpoint()
    {
        lastCheckpoint = Vector2Int.zero;
    }
    
    /// <summary>
    /// 重置重生次数
    /// </summary>
    public void ResetRespawnCount()
    {
        currentRespawnCount = 0;
    }
    
    private void PlayRespawnEffect(Vector3 position)
    {
        // TODO: 添加重生特效
        Debug.Log($"<color=yellow>✨ 播放重生特效 at {position}</color>");
    }
    
    private void ShowGameOverUI()
    {
        // TODO: 显示游戏结束UI
        Debug.Log("<color=red>═══════════════════════════</color>");
        Debug.Log("<color=red>       GAME OVER</color>");
        Debug.Log("<color=red>═══════════════════════════</color>");
    }
}
