using UnityEngine;

/// <summary>
/// 泛型单例基类
/// 用于创建单例管理器
/// </summary>
/// <typeparam name="T">继承自MonoBehaviour的类型</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    
    /// <summary>
    /// 单例实例
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    _instance = singletonObject.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
    
    /// <summary>
    /// 初始化时确保单例唯一性
    /// </summary>
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
