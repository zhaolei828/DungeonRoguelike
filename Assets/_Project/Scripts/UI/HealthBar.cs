using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 血条UI组件 - 显示角色的HP
/// </summary>
public class HealthBar : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Canvas canvas;
    
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
    
    private void OnDestroy()
    {
        // 清理
        target = null;
    }
}

