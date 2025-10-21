using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 伤害数字 - 显示伤害并飘动淡出
/// </summary>
[RequireComponent(typeof(Canvas))]
public class DamageNumber : MonoBehaviour
{
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
    /// 显示伤害数字
    /// </summary>
    public void Show(int damage, bool isCritical = false, bool isHeal = false)
    {
        if (damageText == null) return;
        
        // 设置文本
        damageText.text = damage.ToString();
        
        // 设置颜色
        if (isHeal)
        {
            currentColor = healColor;
            damageText.text = "+" + damageText.text;
        }
        else if (isCritical)
        {
            currentColor = criticalColor;
            damageText.text += "!";
            damageText.fontSize = (int)(damageText.fontSize * 1.3f);
        }
        else
        {
            currentColor = normalColor;
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

