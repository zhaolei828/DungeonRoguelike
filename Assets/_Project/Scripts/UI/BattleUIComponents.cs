// Unity Battle UI System - All Components in One File
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Buff图标 - 显示单个Buff的图标和剩余时间
/// </summary>
public class BuffIcon : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI durationText;
    
    [Header("配置")]
    [SerializeField] private BuffIconConfig iconConfig;
    
    private BuffType buffType;
    private float remainingTime;
    
    private void Awake()
    {
        // 如果没有配置，尝试从Resources加载
        if (iconConfig == null)
        {
            iconConfig = Resources.Load<BuffIconConfig>("BuffIconConfig");
        }
    }
    
    /// <summary>
    /// 设置Buff图标
    /// </summary>
    public void Setup(BuffType type, float duration, Sprite icon = null)
    {
        buffType = type;
        remainingTime = duration;
        
        // 设置图标
        if (iconImage != null)
        {
            // 优先使用传入的icon
            if (icon != null)
            {
                iconImage.sprite = icon;
                iconImage.color = Color.white; // 重置颜色
            }
            // 其次使用配置中的icon
            else if (iconConfig != null && iconConfig.HasIcon(type))
            {
                iconImage.sprite = iconConfig.GetBuffIcon(type);
                iconImage.color = Color.white; // 重置颜色
            }
            // 最后使用纯色作为fallback
            else
            {
                iconImage.sprite = null;
                iconImage.color = GetBuffColor(type);
            }
        }
        
        // 初始化时间显示
        UpdateDurationText();
    }
    
    private void Update()
    {
        remainingTime -= Time.deltaTime;
        
        if (remainingTime <= 0)
        {
            Destroy(gameObject);
            return;
        }
        
        // 更新剩余时间显示
        UpdateDurationText();
    }
    
    /// <summary>
    /// 更新持续时间文本
    /// </summary>
    private void UpdateDurationText()
    {
        if (durationText != null)
        {
            int seconds = Mathf.CeilToInt(remainingTime);
            durationText.text = seconds.ToString();
            
            // 时间快结束时变红
            if (seconds <= 3)
            {
                durationText.color = Color.red;
            }
            else
            {
                durationText.color = Color.white;
            }
        }
    }
    
    /// <summary>
    /// 根据Buff类型返回颜色
    /// </summary>
    private Color GetBuffColor(BuffType type)
    {
        switch(type)
        {
            case BuffType.Strength:
                return new Color(1f, 0.3f, 0.3f); // 红色 - 力量
            case BuffType.Shield:
                return new Color(0.3f, 0.8f, 1f); // 青蓝色 - 护盾
            case BuffType.Poison:
                return new Color(0.8f, 0.2f, 0.8f); // 紫色 - 毒
            case BuffType.Regeneration:
                return new Color(0.3f, 1f, 0.3f); // 绿色 - 再生
            case BuffType.Haste:
                return new Color(1f, 1f, 0.3f); // 黄色 - 加速
            case BuffType.Slow:
                return new Color(0.6f, 0.6f, 0.8f); // 淡蓝色 - 减速
            case BuffType.Weakness:
                return new Color(0.5f, 0.5f, 0.5f); // 灰色 - 虚弱
            case BuffType.Bleeding:
                return new Color(0.8f, 0f, 0f); // 深红色 - 流血
            case BuffType.Burning:
                return new Color(1f, 0.4f, 0f); // 橙红色 - 燃烧
            case BuffType.Frozen:
                return new Color(0.5f, 0.8f, 1f); // 冰蓝色 - 冰冻
            case BuffType.Paralysis:
                return new Color(1f, 1f, 0f); // 黄色 - 麻痹
            case BuffType.Blind:
                return new Color(0.2f, 0.2f, 0.2f); // 深灰色 - 失明
            case BuffType.Confusion:
                return new Color(0.8f, 0.5f, 0.8f); // 粉紫色 - 混乱
            case BuffType.Sleep:
                return new Color(0.4f, 0.4f, 0.6f); // 深蓝色 - 睡眠
            case BuffType.Charmed:
                return new Color(1f, 0.7f, 0.8f); // 粉红色 - 魅惑
            case BuffType.Terror:
                return new Color(0.3f, 0f, 0.3f); // 深紫色 - 恐惧
            case BuffType.Invisibility:
                return new Color(0.8f, 0.8f, 0.8f, 0.5f); // 半透明 - 隐身
            case BuffType.Agility:
                return new Color(0.5f, 1f, 0.5f); // 亮绿色 - 敏捷
            case BuffType.Defense:
                return new Color(0.6f, 0.6f, 0.3f); // 土黄色 - 防御
            default:
                return Color.white;
        }
    }
    
    /// <summary>
    /// 获取Buff类型
    /// </summary>
    public BuffType GetBuffType()
    {
        return buffType;
    }
}

/// <summary>
/// 战斗日志 - 管理战斗消息的显示
/// </summary>
public class BattleLog : MonoBehaviour
{
    [Header("设置")]
    [SerializeField] private Transform logContainer;
    [SerializeField] private GameObject logEntryPrefab;
    [SerializeField] private int maxEntries = 5;
    [SerializeField] private bool autoCreateEntry = true;
    
    private Queue<GameObject> logEntries = new Queue<GameObject>();
    
    /// <summary>
    /// 添加日志条目
    /// </summary>
    public void AddLogEntry(string message, Color color = default)
    {
        if (color == default) 
            color = Color.white;
        
        GameObject entry = CreateLogEntry(message, color);
        if (entry == null)
            return;
            
        logEntries.Enqueue(entry);
        
        // 移除超出数量的旧日志
        while (logEntries.Count > maxEntries)
        {
            GameObject oldEntry = logEntries.Dequeue();
            if (oldEntry != null)
                Destroy(oldEntry);
        }
    }
    
    /// <summary>
    /// 清空所有日志
    /// </summary>
    public void Clear()
    {
        while (logEntries.Count > 0)
        {
            GameObject entry = logEntries.Dequeue();
            if (entry != null)
                Destroy(entry);
        }
    }
    
    /// <summary>
    /// 创建日志条目
    /// </summary>
    private GameObject CreateLogEntry(string message, Color color)
    {
        GameObject entry;
        
        if (logEntryPrefab != null)
        {
            // 使用预制体
            entry = Instantiate(logEntryPrefab, logContainer);
        }
        else if (autoCreateEntry)
        {
            // 自动创建
            entry = CreateLogEntryProgrammatically(message, color);
        }
        else
        {
            Debug.LogWarning("BattleLog: 没有预制体且自动创建已禁用");
            return null;
        }
        
        // 设置文本和颜色
        TextMeshProUGUI text = entry.GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = message;
            text.color = color;
        }
        
        return entry;
    }
    
    /// <summary>
    /// 程序化创建日志条目
    /// </summary>
    private GameObject CreateLogEntryProgrammatically(string message, Color color)
    {
        GameObject entry = new GameObject("LogEntry");
        entry.transform.SetParent(logContainer, false);
        
        // 添加TextMeshProUGUI组件
        TextMeshProUGUI text = entry.AddComponent<TextMeshProUGUI>();
        text.text = message;
        text.color = color;
        text.fontSize = 14;
        text.alignment = TextAlignmentOptions.Left;
        
        // 设置RectTransform
        RectTransform rect = entry.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);
        rect.sizeDelta = new Vector2(0, 20);
        
        return entry;
    }
}

/// <summary>
/// 战斗信息面板 - 统一管理战斗UI的显示
/// </summary>
public class BattleInfoPanel : MonoBehaviour
{
    [Header("引用")]
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private TextMeshProUGUI turnIndicatorText;
    [SerializeField] private BattleLog battleLog;
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("主题")]
    [SerializeField] private UIThemeConfig themeConfig;
    [SerializeField] private Image panelBackground;
    
    [Header("动画设置")]
    [SerializeField] private float fadeSpeed = 3f;
    
    private bool isVisible = false;
    private float targetAlpha = 0f;
    
    private void Awake()
    {
        // 初始化时隐藏面板
        if (panelRoot != null)
            panelRoot.SetActive(false);
            
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
            
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
        
        // 加载主题配置
        if (themeConfig == null)
        {
            themeConfig = Resources.Load<UIThemeConfig>("UIThemeConfig");
        }
        
        // 应用主题
        ApplyTheme();
    }
    
    /// <summary>
    /// 应用UI主题
    /// </summary>
    private void ApplyTheme()
    {
        if (themeConfig == null) return;
        
        // 设置面板背景
        if (panelBackground != null && themeConfig.statusPane != null)
        {
            panelBackground.sprite = themeConfig.statusPane;
            panelBackground.type = Image.Type.Sliced; // 使用九宫格拉伸
        }
        
        // 设置文字颜色
        if (turnIndicatorText != null)
        {
            turnIndicatorText.color = themeConfig.textColor;
        }
    }
    
    private void Update()
    {
        // 平滑淡入淡出
        if (canvasGroup != null)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        }
    }
    
    /// <summary>
    /// 显示面板
    /// </summary>
    public void Show()
    {
        if (panelRoot != null)
            panelRoot.SetActive(true);
            
        targetAlpha = 1f;
        isVisible = true;
        
        Debug.Log("<color=cyan>战斗信息面板显示</color>");
    }
    
    /// <summary>
    /// 隐藏面板
    /// </summary>
    public void Hide()
    {
        targetAlpha = 0f;
        isVisible = false;
        
        // 延迟禁用面板，等待淡出动画完成
        Invoke(nameof(DeactivatePanel), 1f / fadeSpeed);
        
        Debug.Log("<color=cyan>战斗信息面板隐藏</color>");
    }
    
    /// <summary>
    /// 禁用面板GameObject
    /// </summary>
    private void DeactivatePanel()
    {
        if (!isVisible && panelRoot != null)
            panelRoot.SetActive(false);
    }
    
    /// <summary>
    /// 更新回合指示器
    /// </summary>
    public void UpdateTurnIndicator(string text, Color color = default)
    {
        if (turnIndicatorText != null)
        {
            turnIndicatorText.text = text;
            if (color != default)
                turnIndicatorText.color = color;
        }
    }
    
    /// <summary>
    /// 添加战斗日志
    /// </summary>
    public void AddBattleLog(string message, Color color = default)
    {
        if (battleLog != null)
        {
            battleLog.AddLogEntry(message, color);
        }
        else
        {
            Debug.LogWarning($"BattleInfoPanel: BattleLog未设置，无法添加日志: {message}");
        }
    }
    
    /// <summary>
    /// 清空日志
    /// </summary>
    public void ClearLog()
    {
        if (battleLog != null)
            battleLog.Clear();
    }
    
    /// <summary>
    /// 检查面板是否可见
    /// </summary>
    public bool IsVisible()
    {
        return isVisible;
    }
}

// ============================================================================
// UI配置类
// ============================================================================

/// <summary>
/// Buff图标配置 - 管理SPD Buff图标素材的映射
/// </summary>
[CreateAssetMenu(fileName = "BuffIconConfig", menuName = "DungeonRoguelike/UI/Buff Icon Config")]
public class BuffIconConfig : ScriptableObject
{
    [System.Serializable]
    public class BuffIconMapping
    {
        public BuffType buffType;
        public Sprite icon;
        [Tooltip("如果没有提供Sprite，使用此颜色作为fallback")]
        public Color fallbackColor = Color.white;
    }

    [Header("Buff图标映射")]
    [Tooltip("从buffs.png切割出的Buff图标")]
    public List<BuffIconMapping> buffIcons = new List<BuffIconMapping>();

    [Header("默认图标")]
    public Sprite defaultBuffIcon;

    /// <summary>
    /// 获取指定Buff类型的图标
    /// </summary>
    public Sprite GetBuffIcon(BuffType type)
    {
        BuffIconMapping mapping = buffIcons.Find(m => m.buffType == type);
        if (mapping != null && mapping.icon != null)
        {
            return mapping.icon;
        }
        return defaultBuffIcon;
    }

    /// <summary>
    /// 获取指定Buff类型的fallback颜色
    /// </summary>
    public Color GetFallbackColor(BuffType type)
    {
        BuffIconMapping mapping = buffIcons.Find(m => m.buffType == type);
        if (mapping != null)
        {
            return mapping.fallbackColor;
        }
        return Color.white;
    }

    /// <summary>
    /// 检查是否有图标
    /// </summary>
    public bool HasIcon(BuffType type)
    {
        BuffIconMapping mapping = buffIcons.Find(m => m.buffType == type);
        return mapping != null && mapping.icon != null;
    }

    #if UNITY_EDITOR
    [ContextMenu("Auto Generate Mappings")]
    private void AutoGenerateMappings()
    {
        // 自动为所有BuffType创建映射条目
        buffIcons.Clear();
        
        foreach (BuffType type in System.Enum.GetValues(typeof(BuffType)))
        {
            BuffIconMapping mapping = new BuffIconMapping
            {
                buffType = type,
                fallbackColor = GetDefaultColor(type)
            };
            buffIcons.Add(mapping);
        }
        
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"<color=green>✓ 已生成 {buffIcons.Count} 个Buff图标映射条目</color>");
    }

    private Color GetDefaultColor(BuffType type)
    {
        // 使用BattleUIComponents中的颜色映射
        switch(type)
        {
            case BuffType.Strength: return new Color(1f, 0.3f, 0.3f);
            case BuffType.Shield: return new Color(0.3f, 0.8f, 1f);
            case BuffType.Poison: return new Color(0.8f, 0.2f, 0.8f);
            case BuffType.Regeneration: return new Color(0.3f, 1f, 0.3f);
            case BuffType.Haste: return new Color(1f, 1f, 0.3f);
            case BuffType.Slow: return new Color(0.6f, 0.6f, 0.8f);
            case BuffType.Weakness: return new Color(0.5f, 0.5f, 0.5f);
            case BuffType.Bleeding: return new Color(0.8f, 0f, 0f);
            case BuffType.Burning: return new Color(1f, 0.4f, 0f);
            case BuffType.Frozen: return new Color(0.5f, 0.8f, 1f);
            case BuffType.Paralysis: return new Color(1f, 1f, 0f);
            case BuffType.Blind: return new Color(0.2f, 0.2f, 0.2f);
            case BuffType.Confusion: return new Color(0.8f, 0.5f, 0.8f);
            case BuffType.Sleep: return new Color(0.4f, 0.4f, 0.6f);
            case BuffType.Charmed: return new Color(1f, 0.7f, 0.8f);
            case BuffType.Terror: return new Color(0.3f, 0f, 0.3f);
            case BuffType.Invisibility: return new Color(0.8f, 0.8f, 0.8f, 0.5f);
            case BuffType.Agility: return new Color(0.5f, 1f, 0.5f);
            case BuffType.Defense: return new Color(0.6f, 0.6f, 0.3f);
            default: return Color.white;
        }
    }
    #endif
}

/// <summary>
/// UI主题配置 - 管理SPD UI素材（面板、边框等）
/// </summary>
[CreateAssetMenu(fileName = "UIThemeConfig", menuName = "DungeonRoguelike/UI/UI Theme Config")]
public class UIThemeConfig : ScriptableObject
{
    [Header("面板背景")]
    [Tooltip("chrome.png - 窗口边框")]
    public Sprite panelChrome;
    
    [Tooltip("menu_pane.png - 菜单面板背景")]
    public Sprite menuPane;
    
    [Tooltip("status_pane.png - 状态面板背景")]
    public Sprite statusPane;
    
    [Tooltip("surface.png - 表面纹理")]
    public Sprite surface;

    [Header("按钮")]
    [Tooltip("menu_button.png - 菜单按钮")]
    public Sprite menuButton;
    
    [Tooltip("talent_button.png - 天赋按钮")]
    public Sprite talentButton;

    [Header("血条")]
    [Tooltip("boss_hp.png - Boss血条")]
    public Sprite bossHpBar;

    [Header("其他")]
    [Tooltip("shadow.png - 阴影效果")]
    public Sprite shadow;
    
    [Tooltip("toolbar.png - 工具栏")]
    public Sprite toolbar;

    [Header("颜色主题")]
    public Color primaryColor = new Color(0.2f, 0.2f, 0.3f);
    public Color secondaryColor = new Color(0.3f, 0.3f, 0.4f);
    public Color accentColor = new Color(0.8f, 0.6f, 0.2f);
    public Color textColor = Color.white;
}

