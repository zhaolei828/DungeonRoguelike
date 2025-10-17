using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 主菜单UI控制器
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    [Header("UI引用")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    
    [Header("文本")]
    [SerializeField] private TextMeshProUGUI titleText;
    
    private void Start()
    {
        // 绑定按钮事件
        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGameClicked);
            
        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
        
        // 检查是否有存档
        UpdateContinueButton();
    }
    
    /// <summary>
    /// 新游戏按钮
    /// </summary>
    private void OnNewGameClicked()
    {
        Debug.Log("开始新游戏");
        
        // 加载游戏场景
        if (FindObjectOfType<SceneLoader>() != null)
        {
            FindObjectOfType<SceneLoader>().LoadGameScene();
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }
    }
    
    /// <summary>
    /// 继续游戏按钮
    /// </summary>
    private void OnContinueClicked()
    {
        Debug.Log("继续游戏");
        
        // TODO: 加载存档
        // 暂时也跳转到游戏场景
        OnNewGameClicked();
    }
    
    /// <summary>
    /// 设置按钮
    /// </summary>
    private void OnSettingsClicked()
    {
        Debug.Log("打开设置");
        
        // TODO: 打开设置界面
    }
    
    /// <summary>
    /// 退出按钮
    /// </summary>
    private void OnQuitClicked()
    {
        Debug.Log("退出游戏");
        
        if (FindObjectOfType<SceneLoader>() != null)
        {
            FindObjectOfType<SceneLoader>().QuitGame();
        }
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
    
    /// <summary>
    /// 更新继续按钮状态
    /// </summary>
    private void UpdateContinueButton()
    {
        if (continueButton != null)
        {
            // TODO: 检查是否有存档
            bool hasSave = false; // PlayerPrefs.HasKey("SaveGame")
            continueButton.interactable = hasSave;
        }
    }
}

