using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 场景加载管理器
/// 负责场景切换和加载进度显示
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [Header("启动设置")]
    [SerializeField] private bool autoLoadNextScene = true;
    [SerializeField] private float delayBeforeLoad = 0.5f;
    [SerializeField] private string nextSceneName = "MainMenu";
    
    private void Start()
    {
        if (autoLoadNextScene)
        {
            StartCoroutine(LoadSceneAfterDelay());
        }
    }
    
    /// <summary>
    /// 延迟后加载场景
    /// </summary>
    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        LoadScene(nextSceneName);
    }
    
    /// <summary>
    /// 加载场景（同步）
    /// </summary>
    public void LoadScene(string sceneName)
    {
        Debug.Log($"Loading scene: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }
    
    /// <summary>
    /// 加载场景（异步，带进度）
    /// </summary>
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
    }
    
    /// <summary>
    /// 异步加载场景协程
    /// </summary>
    private IEnumerator LoadSceneAsyncCoroutine(string sceneName)
    {
        Debug.Log($"Loading scene async: {sceneName}");
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // 等待场景加载完成
        while (!asyncLoad.isDone)
        {
            // 加载进度 (0 到 0.9)
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            Debug.Log($"Loading progress: {progress * 100}%");
            
            // 这里可以更新进度条UI
            // OnLoadingProgress?.Invoke(progress);
            
            yield return null;
        }
        
        Debug.Log($"Scene {sceneName} loaded successfully");
    }
    
    /// <summary>
    /// 重新加载当前场景
    /// </summary>
    public void ReloadCurrentScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        LoadScene(currentScene.name);
    }
    
    /// <summary>
    /// 加载游戏主场景
    /// </summary>
    public void LoadGameScene()
    {
        LoadScene("Game");
    }
    
    /// <summary>
    /// 加载主菜单场景
    /// </summary>
    public void LoadMainMenu()
    {
        LoadScene("MainMenu");
    }
    
    /// <summary>
    /// 退出游戏
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

