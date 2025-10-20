using UnityEngine;

/// <summary>
/// Game场景初始化器 - 负责游戏启动时的初始化
/// </summary>
public class GameInitializer : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("=== Game Scene Initialized ===");
        
        // 生成第一层地牢
        LevelManager.Instance.GenerateNewLevel(1);
        
        Debug.Log("=== Level 1 Generated ===");
        
        // 生成英雄
        SpawnHero();
    }
    
    /// <summary>
    /// 生成英雄
    /// </summary>
    private void SpawnHero()
    {
        // 创建 HeroSpawner
        GameObject spawnerGO = new GameObject("HeroSpawner");
        HeroSpawner spawner = spawnerGO.AddComponent<HeroSpawner>();
        
        // 生成英雄
        Hero hero = spawner.SpawnHero(HeroClass.Warrior);
        
        if (hero != null)
        {
            Debug.Log($"<color=green>=== Hero Spawned: {hero.Class} ===</color>");
        }
        else
        {
            Debug.LogError("Failed to spawn hero!");
        }
    }
}
