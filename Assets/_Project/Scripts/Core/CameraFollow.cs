using UnityEngine;

/// <summary>
/// 摄像机跟随 - 平滑跟随Hero
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("跟随目标")]
    [SerializeField] private Transform target;
    [SerializeField] private bool autoFindHero = true;
    
    [Header("跟随设置")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;
    
    [Header("边界限制")]
    [SerializeField] private bool useBounds = false;
    [SerializeField] private Vector2 minBounds = new Vector2(-50, -50);
    [SerializeField] private Vector2 maxBounds = new Vector2(50, 50);
    
    private Camera cam;
    
    private void Start()
    {
        cam = GetComponent<Camera>();
        
        // 自动查找Hero
        if (autoFindHero && target == null)
        {
            FindHero();
        }
    }
    
    private void LateUpdate()
    {
        if (target == null)
        {
            if (autoFindHero)
            {
                FindHero();
            }
            return;
        }
        
        // 计算目标位置
        Vector3 desiredPosition = target.position + offset;
        
        // 选择性跟随
        if (!followX)
        {
            desiredPosition.x = transform.position.x;
        }
        if (!followY)
        {
            desiredPosition.y = transform.position.y;
        }
        
        // 应用边界限制
        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
        }
        
        // 平滑移动
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
    
    /// <summary>
    /// 查找Hero
    /// </summary>
    private void FindHero()
    {
        Hero hero = FindObjectOfType<Hero>();
        if (hero != null)
        {
            target = hero.transform;
            Debug.Log("<color=cyan>✓ CameraFollow: 找到Hero并开始跟随</color>");
        }
    }
    
    /// <summary>
    /// 设置跟随目标
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    
    /// <summary>
    /// 立即移动到目标位置（不平滑）
    /// </summary>
    public void SnapToTarget()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            
            if (useBounds)
            {
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minBounds.x, maxBounds.x);
                desiredPosition.y = Mathf.Clamp(desiredPosition.y, minBounds.y, maxBounds.y);
            }
            
            transform.position = desiredPosition;
        }
    }
}
