using UnityEngine;
using System.Collections;

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
        
        // 延迟生成英雄和初始化系统，确保所有组件都已就绪
        StartCoroutine(InitializeGameSystems());
    }
    
    /// <summary>
    /// 初始化游戏系统
    /// </summary>
    private IEnumerator InitializeGameSystems()
    {
        // 等待一帧，确保所有Awake和Start都执行完毕
        yield return null;
        
        // 查找或创建HeroSpawner
        HeroSpawner spawner = FindFirstObjectByType<HeroSpawner>();
        if (spawner == null)
        {
            GameObject spawnerGO = new GameObject("HeroSpawner");
            spawner = spawnerGO.AddComponent<HeroSpawner>();
        }
        
        // 生成英雄
        Hero hero = spawner.SpawnHero(HeroClass.Warrior);
        
        if (hero != null)
        {
            // 将Hero注册到GameManager
            GameManager.Instance.Hero = hero;
            
            Debug.Log($"<color=green>=== Hero Spawned: {hero.Class} at {hero.pos} ===</color>");
            
            // 再等待一帧，确保Hero完全初始化
            yield return null;
            
            // 添加玩家输入组件
            GameObject inputGO = FindFirstObjectByType<PlayerInput>()?.gameObject;
            if (inputGO == null)
            {
                inputGO = new GameObject("PlayerInput");
                inputGO.AddComponent<PlayerInput>();
            }
            
            // 为主相机添加跟随组件
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                CameraFollow cameraFollow = mainCamera.gameObject.GetComponent<CameraFollow>();
                if (cameraFollow == null)
                {
                    cameraFollow = mainCamera.gameObject.AddComponent<CameraFollow>();
                }
                cameraFollow.SetTarget(hero.transform);
                
                // 立即将相机移动到Hero位置
                mainCamera.transform.position = new Vector3(hero.pos.x + 0.5f, hero.pos.y + 0.5f, -10f);
                
                Debug.Log("<color=cyan>=== Camera Follow Enabled ===</color>");
            }
            else
            {
                Debug.LogWarning("Main Camera not found, camera follow not enabled.");
            }
        }
        else
        {
            Debug.LogError("Failed to spawn hero!");
        }
    }
}
