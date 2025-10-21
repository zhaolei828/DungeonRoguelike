using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 血条管理器 - 负责创建和管理血条
/// </summary>
public class HealthBarManager : MonoBehaviour
{
    private static HealthBarManager _instance;
    public static HealthBarManager Instance => _instance;
    
    [Header("血条预制体设置")]
    [SerializeField] private GameObject healthBarPrefab;
    
    [Header("自动创建设置")]
    [SerializeField] private bool autoCreatePrefab = true;
    [SerializeField] private Vector2 barSize = new Vector2(1f, 0.15f);
    [SerializeField] private Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    
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
    /// 为角色创建血条
    /// </summary>
    public HealthBar CreateHealthBar(Transform target, float maxHP, Vector3 offset)
    {
        GameObject healthBarGO;
        
        if (healthBarPrefab != null)
        {
            // 使用预制体
            healthBarGO = Instantiate(healthBarPrefab, target.position + offset, Quaternion.identity);
        }
        else if (autoCreatePrefab)
        {
            // 自动创建
            healthBarGO = CreateHealthBarProgrammatically();
        }
        else
        {
            Debug.LogError("HealthBarManager: No prefab and autoCreate is disabled!");
            return null;
        }
        
        // 设置父对象（可选）
        healthBarGO.transform.SetParent(target);
        
        // 获取HealthBar组件
        HealthBar healthBar = healthBarGO.GetComponent<HealthBar>();
        if (healthBar == null)
        {
            healthBar = healthBarGO.AddComponent<HealthBar>();
        }
        
        // 初始化
        healthBar.Initialize(target, maxHP);
        
        return healthBar;
    }
    
    /// <summary>
    /// 程序化创建血条GameObject
    /// </summary>
    private GameObject CreateHealthBarProgrammatically()
    {
        // 创建根对象
        GameObject healthBarGO = new GameObject("HealthBar");
        
        // 添加Canvas
        Canvas canvas = healthBarGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        // 设置Canvas大小
        RectTransform canvasRect = healthBarGO.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(barSize.x, barSize.y);
        canvasRect.localScale = Vector3.one * 0.01f; // 缩小以适应世界空间
        
        // 添加CanvasScaler
        CanvasScaler scaler = healthBarGO.AddComponent<CanvasScaler>();
        scaler.dynamicPixelsPerUnit = 10;
        
        // 创建背景
        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(healthBarGO.transform, false);
        
        Image bgImage = bgGO.AddComponent<Image>();
        bgImage.color = backgroundColor;
        
        RectTransform bgRect = bgGO.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        // 创建Slider
        GameObject sliderGO = new GameObject("Slider");
        sliderGO.transform.SetParent(healthBarGO.transform, false);
        
        Slider slider = sliderGO.AddComponent<Slider>();
        slider.transition = Selectable.Transition.None;
        
        RectTransform sliderRect = sliderGO.GetComponent<RectTransform>();
        sliderRect.anchorMin = Vector2.zero;
        sliderRect.anchorMax = Vector2.one;
        sliderRect.sizeDelta = Vector2.zero;
        
        // 创建Fill Area
        GameObject fillAreaGO = new GameObject("Fill Area");
        fillAreaGO.transform.SetParent(sliderGO.transform, false);
        
        RectTransform fillAreaRect = fillAreaGO.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = Vector2.zero;
        fillAreaRect.anchorMax = Vector2.one;
        fillAreaRect.sizeDelta = new Vector2(-10, -10);
        
        // 创建Fill
        GameObject fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(fillAreaGO.transform, false);
        
        Image fillImage = fillGO.AddComponent<Image>();
        fillImage.color = Color.green;
        fillImage.type = Image.Type.Filled;
        fillImage.fillMethod = Image.FillMethod.Horizontal;
        
        RectTransform fillRect = fillGO.GetComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.sizeDelta = Vector2.zero;
        
        // 设置Slider引用
        slider.fillRect = fillRect;
        slider.minValue = 0;
        slider.maxValue = 100;
        slider.value = 100;
        
        return healthBarGO;
    }
}

