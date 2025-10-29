using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 血条UI组件 - 显示角色的HP和Buff图标
/// </summary>
public class HealthBar : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Canvas canvas;
    
    [Header("Buff图标")]
    [SerializeField] private Transform buffIconContainer;
    [SerializeField] private GameObject buffIconPrefab;
    [SerializeField] private bool autoCreateBuffContainer = true;
    
    [Header("颜色设置")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 0.3f;
    
    [Header("显示设置")]
    [SerializeField] private Vector3 offset = new Vector3(0, 1.5f, 0);
    [SerializeField] private bool hideWhenFull = false;
    [SerializeField] private float smoothSpeed = 5f;
    
    private Transform target;
    private float currentDisplayHP;
    private float maxHP;
    private Dictionary<BuffType, BuffIcon> activeBuffIcons = new Dictionary<BuffType, BuffIcon>();
    
    private void Awake()
    {
        // 确保Canvas设置正确
        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
        }
        
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
        }
        
        // 确保Slider存在
        if (hpSlider == null)
        {
            hpSlider = GetComponentInChildren<Slider>();
        }
        
        // 获取Fill Image
        if (fillImage == null && hpSlider != null)
        {
            fillImage = hpSlider.fillRect?.GetComponent<Image>();
        }
    }
    
    /// <summary>
    /// 初始化血条
    /// </summary>
    public void Initialize(Transform target, float maxHP)
    {
        this.target = target;
        this.maxHP = maxHP;
        this.currentDisplayHP = maxHP;
        
        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = maxHP;
        }
        
        UpdateColor(1f);
        UpdateVisibility(true);
        
        // 初始化Buff图标容器
        InitializeBuffIconContainer();
        
        Debug.Log($"<color=green>HealthBar.Initialize完成！target: {target.name}, maxHP: {maxHP}, offset: {offset}</color>");
    }

    /// <summary>
    /// 初始化Buff图标容器
    /// </summary>
    private void InitializeBuffIconContainer()
    {
        if (buffIconContainer == null && autoCreateBuffContainer)
        {
            // 自动创建Buff图标容器
            GameObject containerGO = new GameObject("BuffIconContainer");
            containerGO.transform.SetParent(transform, false);
            
            RectTransform rect = containerGO.AddComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, 30); // 血条上方30像素
            rect.sizeDelta = new Vector2(100, 20);
            
            // 添加水平布局组
            HorizontalLayoutGroup layout = containerGO.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 2;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            
            buffIconContainer = containerGO.transform;
        }
    }
    
    /// <summary>
    /// 更新血条显示
    /// </summary>
    public void UpdateHealth(float currentHP)
    {
        if (hpSlider == null) return;
        
        // 平滑更新显示的HP
        currentDisplayHP = Mathf.Lerp(currentDisplayHP, currentHP, Time.deltaTime * smoothSpeed);
        hpSlider.value = currentDisplayHP;
        
        // 更新颜色
        float healthPercent = currentHP / maxHP;
        UpdateColor(healthPercent);
        
        // 更新可见性
        if (hideWhenFull)
        {
            UpdateVisibility(healthPercent < 0.99f);
        }
    }
    
    /// <summary>
    /// 立即设置HP（不平滑）
    /// </summary>
    public void SetHealthImmediate(float currentHP)
    {
        if (hpSlider == null) return;
        
        currentDisplayHP = currentHP;
        hpSlider.value = currentHP;
        
        float healthPercent = currentHP / maxHP;
        UpdateColor(healthPercent);
        
        if (hideWhenFull)
        {
            UpdateVisibility(healthPercent < 0.99f);
        }
    }
    
    /// <summary>
    /// 更新颜色（根据HP百分比）
    /// </summary>
    private void UpdateColor(float healthPercent)
    {
        if (fillImage == null) return;
        
        if (healthPercent <= lowHealthThreshold)
        {
            fillImage.color = lowHealthColor;
        }
        else
        {
            fillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, 
                (healthPercent - lowHealthThreshold) / (1f - lowHealthThreshold));
        }
    }
    
    /// <summary>
    /// 更新可见性
    /// </summary>
    private void UpdateVisibility(bool visible)
    {
        if (canvas != null)
        {
            canvas.enabled = visible;
        }
    }
    
    private void LateUpdate()
    {
        // 跟随目标位置
        if (target != null)
        {
            transform.position = target.position + offset;
            
            // 始终面向摄像机
            if (Camera.main != null)
            {
                transform.rotation = Camera.main.transform.rotation;
            }
        }
    }
    
    /// <summary>
    /// 显示Buff图标
    /// </summary>
    public void ShowBuffIcon(BuffType type, float duration)
    {
        // 如果已存在，先移除
        if (activeBuffIcons.ContainsKey(type))
        {
            HideBuffIcon(type);
        }
        
        GameObject iconGO = CreateBuffIcon(type, duration);
        if (iconGO == null)
            return;
            
        BuffIcon icon = iconGO.GetComponent<BuffIcon>();
        if (icon != null)
        {
            activeBuffIcons[type] = icon;
        }
    }
    
    /// <summary>
    /// 隐藏Buff图标
    /// </summary>
    public void HideBuffIcon(BuffType type)
    {
        if (activeBuffIcons.TryGetValue(type, out BuffIcon icon))
        {
            if (icon != null)
                Destroy(icon.gameObject);
                
            activeBuffIcons.Remove(type);
        }
    }
    
    /// <summary>
    /// 创建Buff图标
    /// </summary>
    private GameObject CreateBuffIcon(BuffType type, float duration)
    {
        if (buffIconContainer == null)
        {
            Debug.LogWarning("HealthBar: Buff图标容器未初始化");
            return null;
        }
        
        GameObject iconGO;
        
        if (buffIconPrefab != null)
        {
            // 使用预制体
            iconGO = Instantiate(buffIconPrefab, buffIconContainer);
        }
        else
        {
            // 程序化创建
            iconGO = CreateBuffIconProgrammatically(type, duration);
        }
        
        // 设置BuffIcon组件
        BuffIcon icon = iconGO.GetComponent<BuffIcon>();
        if (icon == null)
            icon = iconGO.AddComponent<BuffIcon>();
            
        icon.Setup(type, duration);
        
        return iconGO;
    }
    
    /// <summary>
    /// 程序化创建Buff图标
    /// </summary>
    private GameObject CreateBuffIconProgrammatically(BuffType type, float duration)
    {
        GameObject iconGO = new GameObject($"BuffIcon_{type}");
        iconGO.transform.SetParent(buffIconContainer, false);
        
        // 添加Image作为背景
        Image bgImage = iconGO.AddComponent<Image>();
        bgImage.color = Color.white;
        
        // 设置RectTransform
        RectTransform rect = iconGO.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(20, 20);
        
        // 添加时间文本
        GameObject textGO = new GameObject("DurationText");
        textGO.transform.SetParent(iconGO.transform, false);
        
        TMPro.TextMeshProUGUI text = textGO.AddComponent<TMPro.TextMeshProUGUI>();
        text.fontSize = 10;
        text.alignment = TMPro.TextAlignmentOptions.Center;
        text.color = Color.white;
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        return iconGO;
    }
    
    private void OnDestroy()
    {
        // 清理Buff图标
        foreach (var icon in activeBuffIcons.Values)
        {
            if (icon != null)
                Destroy(icon.gameObject);
        }
        activeBuffIcons.Clear();
        
        // 清理
        target = null;
    }
}

