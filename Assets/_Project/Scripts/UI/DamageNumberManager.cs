using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 伤害数字管理器 - 负责创建和管理伤害数字
/// </summary>
public class DamageNumberManager : MonoBehaviour
{
    private static DamageNumberManager _instance;
    public static DamageNumberManager Instance => _instance;
    
    [Header("伤害数字预制体")]
    [SerializeField] private GameObject damageNumberPrefab;
    
    [Header("自动创建设置")]
    [SerializeField] private bool autoCreatePrefab = true;
    [SerializeField] private int fontSize = 24;
    [Tooltip("推荐手动分配字体以获得更好显示效果。留空将使用Unity内置的LegacyRuntime字体")]
    [SerializeField] private Font font;
    
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        Debug.Log($"<color=green>✓✓✓ DamageNumberManager 已初始化！Instance: {_instance != null}</color>");
    }
    
    /// <summary>
    /// 显示伤害数字
    /// </summary>
    public void ShowDamage(Vector3 position, int damage, bool isCritical = false)
    {
        GameObject damageNumberGO = CreateDamageNumber(position);
        
        DamageNumber damageNumber = damageNumberGO.GetComponent<DamageNumber>();
        if (damageNumber != null)
        {
            damageNumber.Show(damage, isCritical, false);
        }
    }
    
    /// <summary>
    /// 显示治疗数字
    /// </summary>
    public void ShowHeal(Vector3 position, int amount)
    {
        GameObject damageNumberGO = CreateDamageNumber(position);
        
        DamageNumber damageNumber = damageNumberGO.GetComponent<DamageNumber>();
        if (damageNumber != null)
        {
            damageNumber.Show(amount, false, true);
        }
    }
    
    /// <summary>
    /// 创建伤害数字GameObject
    /// </summary>
    private GameObject CreateDamageNumber(Vector3 position)
    {
        GameObject damageNumberGO;
        
        if (damageNumberPrefab != null)
        {
            // 使用预制体
            damageNumberGO = Instantiate(damageNumberPrefab, position, Quaternion.identity);
        }
        else if (autoCreatePrefab)
        {
            // 自动创建
            damageNumberGO = CreateDamageNumberProgrammatically(position);
        }
        else
        {
            Debug.LogError("DamageNumberManager: No prefab and autoCreate is disabled!");
            return null;
        }
        
        return damageNumberGO;
    }
    
    /// <summary>
    /// 程序化创建伤害数字GameObject
    /// </summary>
    private GameObject CreateDamageNumberProgrammatically(Vector3 position)
    {
        // 创建根对象
        GameObject damageNumberGO = new GameObject("DamageNumber");
        damageNumberGO.transform.position = position;
        
        // 添加Canvas
        Canvas canvas = damageNumberGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        // 设置Canvas大小
        RectTransform canvasRect = damageNumberGO.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(2, 1);
        canvasRect.localScale = Vector3.one * 0.01f;
        
        // 创建Text
        GameObject textGO = new GameObject("Text");
        textGO.transform.SetParent(damageNumberGO.transform, false);
        
        Text text = textGO.AddComponent<Text>();
        text.font = font != null ? font : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.text = "0";
        
        // 添加阴影效果
        Shadow shadow = textGO.AddComponent<Shadow>();
        shadow.effectColor = Color.black;
        shadow.effectDistance = new Vector2(1, -1);
        
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        // 添加DamageNumber组件
        DamageNumber damageNumber = damageNumberGO.AddComponent<DamageNumber>();
        
        return damageNumberGO;
    }
}

