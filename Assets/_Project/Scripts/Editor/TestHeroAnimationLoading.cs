using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

/// <summary>
/// 测试Hero动画加载 - 验证warrior sprites是否能正确加载
/// </summary>
public class TestHeroAnimationLoading
{
    [MenuItem("Tools/Test/Load Game Scene and Test Hero Animation")]
    public static void TestHeroAnimation()
    {
        Debug.Log("<color=cyan>=== Starting Hero Animation Loading Test ===</color>");
        
        // 1. 打开Game.unity场景
        string gameScenePath = "Assets/_Project/Scenes/Game.unity/Game.unity";
        EditorSceneManager.OpenScene(gameScenePath, OpenSceneMode.Single);
        Debug.Log($"✓ Loaded scene: {gameScenePath}");
        
        // 2. 进入Play模式
        EditorApplication.isPlaying = true;
        
        // 3. 等待一段时间后检查Hero和动画
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlaying)
            {
                Debug.Log("<color=yellow>Waiting for Hero initialization...</color>");
                EditorApplication.delayCall += CheckHeroAnimation;
            }
        };
    }
    
    private static void CheckHeroAnimation()
    {
        // 等待1秒后检查
        EditorApplication.delayCall += () =>
        {
            if (!EditorApplication.isPlaying)
                return;
            
            Hero hero = FindFirstObjectByType<Hero>();
            if (hero != null)
            {
                Debug.Log("<color=green>✓ Hero found</color>");
                Debug.Log($"  Position: {hero.pos}");
                Debug.Log($"  Class: {hero.Class}");
                
                HeroAnimator animator = hero.GetComponent<HeroAnimator>();
                if (animator != null)
                {
                    Debug.Log("<color=green>✓ HeroAnimator found</color>");
                    
                    // 检查sprite数组
                    var idleSprites = animator.idleSprites;
                    var walkDownSprites = animator.walkDownSprites;
                    var walkUpSprites = animator.walkUpSprites;
                    var walkLeftSprites = animator.walkLeftSprites;
                    var walkRightSprites = animator.walkRightSprites;
                    
                    Debug.Log($"Animation sprites loaded:");
                    Debug.Log($"  Idle: {(idleSprites != null ? idleSprites.Length : 0)} frames");
                    Debug.Log($"  WalkDown: {(walkDownSprites != null ? walkDownSprites.Length : 0)} frames");
                    Debug.Log($"  WalkUp: {(walkUpSprites != null ? walkUpSprites.Length : 0)} frames");
                    Debug.Log($"  WalkLeft: {(walkLeftSprites != null ? walkLeftSprites.Length : 0)} frames");
                    Debug.Log($"  WalkRight: {(walkRightSprites != null ? walkRightSprites.Length : 0)} frames");
                    
                    if ((idleSprites == null || idleSprites.Length == 0) ||
                        (walkDownSprites == null || walkDownSprites.Length == 0))
                    {
                        Debug.LogWarning("<color=red>⚠ Some sprite arrays are not loaded!</color>");
                        Debug.Log("Attempting to manually load sprites...");
                        animator.LoadWarriorSprites();
                        
                        // 重新检查
                        Debug.Log($"After LoadWarriorSprites():");
                        Debug.Log($"  Idle: {(animator.idleSprites != null ? animator.idleSprites.Length : 0)} frames");
                        Debug.Log($"  WalkDown: {(animator.walkDownSprites != null ? animator.walkDownSprites.Length : 0)} frames");
                    }
                    else
                    {
                        Debug.Log("<color=green>✓ All sprite arrays loaded successfully!</color>");
                    }
                }
                else
                {
                    Debug.LogError("✗ HeroAnimator not found");
                }
            }
            else
            {
                Debug.LogWarning("⚠ Hero not found, may still be initializing...");
            }
        };
    }
}
