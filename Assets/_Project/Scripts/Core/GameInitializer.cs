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
    }
}
