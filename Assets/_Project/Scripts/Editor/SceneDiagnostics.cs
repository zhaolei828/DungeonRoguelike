using UnityEditor;
using UnityEngine;

/// <summary>
/// 场景诊断工具 - 检查所有必需的管理器是否存在
/// </summary>
public class SceneDiagnostics
{
    [MenuItem("Tools/DungeonRoguelike/诊断场景配置 %&d", false, 1)] // Ctrl+Alt+D
    public static void DiagnoseScene()
    {
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        Debug.Log("<color=cyan><b>开始场景配置诊断...</b></color>");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        bool allGood = true;
        
        // 1. 检查Managers
        GameObject managersGO = GameObject.Find("Managers");
        if (managersGO != null)
        {
            Debug.Log("<color=green>✓ Managers GameObject 存在</color>");
            
            // 检查LevelManager
            LevelManager levelManager = managersGO.GetComponent<LevelManager>();
            if (levelManager != null)
            {
                Debug.Log("<color=green>  ✓ LevelManager 已添加</color>");
            }
            else
            {
                Debug.LogError("<color=red>  ✗ LevelManager 缺失！</color>");
                allGood = false;
            }
            
            // 检查RespawnManager
            RespawnManager respawnManager = managersGO.GetComponent<RespawnManager>();
            if (respawnManager != null)
            {
                Debug.Log("<color=green>  ✓ RespawnManager 已添加</color>");
                Debug.Log($"    • Enable Respawn: {respawnManager.GetType().GetField("enableRespawn", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(respawnManager)}");
            }
            else
            {
                Debug.LogWarning("<color=yellow>  ⚠ RespawnManager 缺失（按Ctrl+Alt+S添加）</color>");
                allGood = false;
            }
        }
        else
        {
            Debug.LogError("<color=red>✗ Managers GameObject 不存在！</color>");
            allGood = false;
        }
        
        Debug.Log("");
        
        // 2. 检查UIManagers
        GameObject uiManagersGO = GameObject.Find("UIManagers");
        if (uiManagersGO != null)
        {
            Debug.Log("<color=green>✓ UIManagers GameObject 存在</color>");
            
            // 检查HealthBarManager
            HealthBarManager healthBarManager = uiManagersGO.GetComponent<HealthBarManager>();
            if (healthBarManager != null)
            {
                Debug.Log("<color=green>  ✓ HealthBarManager 已添加</color>");
                Debug.Log($"    • Instance: {HealthBarManager.Instance != null}");
            }
            else
            {
                Debug.LogError("<color=red>  ✗ HealthBarManager 缺失！</color>");
                allGood = false;
            }
            
            // 检查DamageNumberManager
            DamageNumberManager damageNumberManager = uiManagersGO.GetComponent<DamageNumberManager>();
            if (damageNumberManager != null)
            {
                Debug.Log("<color=green>  ✓ DamageNumberManager 已添加</color>");
                Debug.Log($"    • Instance: {DamageNumberManager.Instance != null}");
            }
            else
            {
                Debug.LogError("<color=red>  ✗ DamageNumberManager 缺失！</color>");
                allGood = false;
            }
        }
        else
        {
            Debug.LogError("<color=red>✗ UIManagers GameObject 不存在！</color>");
            allGood = false;
        }
        
        Debug.Log("");
        
        // 3. 检查GameManager
        GameObject gameManagerGO = GameObject.Find("GameManager");
        if (gameManagerGO != null)
        {
            Debug.Log("<color=green>✓ GameManager GameObject 存在</color>");
            
            GameManager gameManager = gameManagerGO.GetComponent<GameManager>();
            if (gameManager != null)
            {
                Debug.Log("<color=green>  ✓ GameManager 已添加</color>");
            }
            else
            {
                Debug.LogError("<color=red>  ✗ GameManager 组件缺失！</color>");
                allGood = false;
            }
        }
        else
        {
            Debug.LogWarning("<color=yellow>⚠ GameManager GameObject 不存在</color>");
        }
        
        Debug.Log("");
        
        // 4. 检查Main Camera
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Debug.Log("<color=green>✓ Main Camera 存在</color>");
            Debug.Log($"    • Tag: {mainCamera.tag}");
            
            CameraFollow cameraFollow = mainCamera.GetComponent<CameraFollow>();
            if (cameraFollow != null)
            {
                Debug.Log("<color=green>  ✓ CameraFollow 已添加</color>");
            }
            else
            {
                Debug.LogWarning("<color=yellow>  ⚠ CameraFollow 缺失（按Ctrl+Alt+S添加）</color>");
            }
        }
        else
        {
            Debug.LogError("<color=red>✗ Main Camera 不存在或Tag不正确！</color>");
            allGood = false;
        }
        
        Debug.Log("");
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
        
        if (allGood)
        {
            Debug.Log("<color=green><b>✓ 诊断完成：场景配置正确！</b></color>");
            Debug.Log("<color=cyan>可以按Play开始测试游戏。</color>");
        }
        else
        {
            Debug.LogWarning("<color=yellow><b>⚠ 诊断完成：发现配置问题！</b></color>");
            Debug.Log("<color=cyan>建议：按 Ctrl+Alt+S 一键修复所有问题</color>");
        }
        
        Debug.Log("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    }
}

