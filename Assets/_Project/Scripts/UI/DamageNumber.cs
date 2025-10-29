using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 伤害数字 - 显示伤害并飘动淡出
/// </summary>
[RequireComponent(typeof(Canvas))]
public class DamageNumber : MonoBehaviour
{
    /// <summary>
    /// 伤害类型枚举
    /// </summary>
    public enum DamageType
    {
        Normal,     // 普通伤害
        Critical,   // 暴击伤害
        Heal,       // 治疗
        Shield,     // 护盾吸收
        Poison      // 毒伤
    }

    [Header("UI组件")]
    [SerializeField] private Text damageText;
    [SerializeField] private Canvas canvas;
    
    [Header("动画设置")]
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private float lifetime = 1.5f;
    [SerializeField] private Vector3 randomOffset = new Vector3(0.3f, 0.3f, 0);
    
    [Header("颜色设置")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color criticalColor = Color.yellow;
    [SerializeField] private Color healColor = Color.green;
    [SerializeField] private Color shieldColor = new Color(0.3f, 0.8f, 1f); // 青蓝色
    [SerializeField] private Color poisonColor = new Color(0.8f, 0.2f, 0.8f); // 紫色
    
    private float currentLifetime;
    private Vector3 velocity;
    private Color currentColor;
    
    private void Awake()
    {
        if (canvas == null)
        {
            canvas = GetComponent<Canvas>();
        }
        
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = Camera.main;
        }
        
        if (damageText == null)
        {
            damageText = GetComponentInChildren<Text>();
        }
    }
    
    /// <summary>
    /// 显示伤害数字（新版本，支持DamageType）
    /// </summary>
    public void Show(int damage, DamageType type = DamageType.Normal)
    {
        if (damageText == null) return;
        
        // 设置文本
        damageText.text = damage.ToString();
        
        // 根据类型设置颜色和样式
        switch (type)
        {
            case DamageType.Normal:
                currentColor = normalColor;
                break;
                
            case DamageType.Critical:
                currentColor = criticalColor;
                damageText.text += "!";
                damageText.fontSize = (int)(damageText.fontSize * 1.3f);
                break;
                
            case DamageType.Heal:
                currentColor = healColor;
                damageText.text = "+" + damageText.text;
                break;
                
            case DamageType.Shield:
                currentColor = shieldColor;
                damageText.text = damageText.text + " 盾";
                break;
                
            case DamageType.Poison:
                currentColor = poisonColor;
                damageText.text = damageText.text + " 毒";
                break;
        }
        
        damageText.color = currentColor;
        
        // 添加随机偏移
        Vector3 randomDir = new Vector3(
            Random.Range(-randomOffset.x, randomOffset.x),
            Random.Range(0, randomOffset.y),
            Random.Range(-randomOffset.z, randomOffset.z)
        );
        
        velocity = (Vector3.up * floatSpeed) + randomDir;
        
        // 开始动画
        currentLifetime = lifetime;
        StartCoroutine(AnimateCoroutine());
    }

    /// <summary>
    /// 显示伤害数字（旧版本兼容，保留向后兼容性）
    /// </summary>
    public void Show(int damage, bool isCritical = false, bool isHeal = false)
    {
        DamageType type = DamageType.Normal;
        if (isHeal)
            type = DamageType.Heal;
        else if (isCritical)
            type = DamageType.Critical;
            
        Show(damage, type);
    }
    
    private IEnumerator AnimateCoroutine()
    {
        while (currentLifetime > 0)
        {
            // 向上飘动
            transform.position += velocity * Time.deltaTime;
            
            // 淡出
            currentLifetime -= Time.deltaTime;
            float alpha = Mathf.Clamp01(currentLifetime / lifetime);
            
            if (damageText != null)
            {
                Color color = currentColor;
                color.a = alpha;
                damageText.color = color;
            }
            
            // 面向摄像机
            if (Camera.main != null)
            {
                transform.rotation = Camera.main.transform.rotation;
            }
            
            yield return null;
        }
        
        // 销毁
        Destroy(gameObject);
    }
}

