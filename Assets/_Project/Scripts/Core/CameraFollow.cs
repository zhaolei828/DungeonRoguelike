using UnityEngine;

/// <summary>
/// 相机跟随 - 使相机平滑跟随Hero
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("跟随设置")]
    [Tooltip("跟随速度（0-1之间，越大越快）")]
    public float followSpeed = 0.1f;
    
    [Tooltip("相机Z轴偏移")]
    public float zOffset = -10f;
    
    private Transform target;
    
    private void Start()
    {
        // 获取Hero的Transform作为跟随目标
        Hero hero = GameManager.Instance?.Hero;
        if (hero != null)
        {
            target = hero.transform;
            Debug.Log("<color=cyan>CameraFollow: Target set to Hero</color>");
        }
        else
        {
            Debug.LogWarning("CameraFollow: Hero not found, camera will not follow.");
        }
    }
    
    private void LateUpdate()
    {
        if (target == null)
            return;
        
        // 计算目标位置
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y, zOffset);
        
        // 平滑移动到目标位置
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed);
    }
    
    /// <summary>
    /// 设置跟随目标
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}

