using UnityEngine;
using System.Linq;

/// <summary>
/// Hero动画控制器 - 管理Hero的移动动画
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class HeroAnimator : MonoBehaviour
{
    [Header("动画设置")]
    [SerializeField] private float frameRate = 10f; // 每秒帧数
    [SerializeField] private bool isAnimating = false;
    [SerializeField] private float idleTimeout = 1f; // Idle动画返回超时
    
    private float idleTimer = 0f;
    
    [Header("Sprite引用")]
    [SerializeField] private Sprite[] idleSprites;
    [SerializeField] private Sprite[] walkDownSprites;
    [SerializeField] private Sprite[] walkUpSprites;
    [SerializeField] private Sprite[] walkLeftSprites;
    [SerializeField] private Sprite[] walkRightSprites;
    
    private SpriteRenderer spriteRenderer;
    private Sprite[] currentAnimation;
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private Vector2 lastDirection = Vector2.down;
    
    // 动画状态
    public enum AnimationState
    {
        Idle,
        WalkDown,
        WalkUp,
        WalkLeft,
        WalkRight
    }
    
    private AnimationState currentState = AnimationState.Idle;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    private void Start()
    {
        // 默认显示Idle动画的第一帧
        if (idleSprites != null && idleSprites.Length > 0)
        {
            spriteRenderer.sprite = idleSprites[0];
        }
        
        // 自动加载warrior sprites（如果尚未加载）
        if ((idleSprites == null || idleSprites.Length == 0) &&
            (walkDownSprites == null || walkDownSprites.Length == 0))
        {
            LoadWarriorSprites();
        }
    }
    
    private void Update()
    {
        if (isAnimating && currentAnimation != null && currentAnimation.Length > 0)
        {
            frameTimer += Time.deltaTime;
            
            if (frameTimer >= 1f / frameRate)
            {
                frameTimer = 0f;
                currentFrame = (currentFrame + 1) % currentAnimation.Length;
                spriteRenderer.sprite = currentAnimation[currentFrame];
            }
        }
        
        // 如果在运动状态超过超时时间，自动返回Idle
        if (currentState != AnimationState.Idle && currentState != AnimationState.WalkDown && 
            currentState != AnimationState.WalkUp && currentState != AnimationState.WalkLeft && 
            currentState != AnimationState.WalkRight)
        {
            return;
        }
        
        if (currentState != AnimationState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleTimeout)
            {
                SetAnimationState(AnimationState.Idle);
                idleTimer = 0f;
            }
        }
        else
        {
            idleTimer = 0f;
        }
    }
    
    /// <summary>
    /// 设置动画状态
    /// </summary>
    public void SetAnimationState(AnimationState state)
    {
        if (currentState == state) return;
        
        currentState = state;
        currentFrame = 0;
        frameTimer = 0f;
        
        switch (state)
        {
            case AnimationState.Idle:
                currentAnimation = idleSprites;
                isAnimating = idleSprites != null && idleSprites.Length > 1;
                break;
            case AnimationState.WalkDown:
                currentAnimation = walkDownSprites;
                isAnimating = true;
                lastDirection = Vector2.down;
                break;
            case AnimationState.WalkUp:
                currentAnimation = walkUpSprites;
                isAnimating = true;
                lastDirection = Vector2.up;
                break;
            case AnimationState.WalkLeft:
                currentAnimation = walkLeftSprites;
                isAnimating = true;
                lastDirection = Vector2.left;
                break;
            case AnimationState.WalkRight:
                currentAnimation = walkRightSprites;
                isAnimating = true;
                lastDirection = Vector2.right;
                break;
        }
        
        // 立即显示第一帧
        if (currentAnimation != null && currentAnimation.Length > 0)
        {
            spriteRenderer.sprite = currentAnimation[0];
        }
    }
    
    /// <summary>
    /// 根据移动方向设置动画
    /// </summary>
    public void SetAnimationByDirection(Vector2Int direction)
    {
        if (direction == Vector2Int.zero)
        {
            SetAnimationState(AnimationState.Idle);
            return;
        }
        
        // 确定主要方向
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            // 水平移动
            if (direction.x > 0)
                SetAnimationState(AnimationState.WalkRight);
            else
                SetAnimationState(AnimationState.WalkLeft);
        }
        else
        {
            // 垂直移动
            if (direction.y > 0)
                SetAnimationState(AnimationState.WalkUp);
            else
                SetAnimationState(AnimationState.WalkDown);
        }
    }
    
    /// <summary>
    /// 停止动画并返回Idle
    /// </summary>
    public void StopAnimation()
    {
        SetAnimationState(AnimationState.Idle);
    }
    
    /// <summary>
    /// 自动加载Warrior sprite
    /// </summary>
    public void LoadWarriorSprites()
    {
        // warrior.png中的sprites按以下方式组织：
        // warrior_0_0 ~ warrior_0_20 = 动作0 (通常是Idle)
        // warrior_1_0 ~ warrior_1_20 = 动作1
        // warrior_2_0 ~ warrior_2_20 = 动作2
        // warrior_3_0 ~ warrior_3_20 = 动作3
        // warrior_4_0 ~ warrior_4_20 = 动作4
        // warrior_5_0 ~ warrior_5_20 = 动作5
        // warrior_6_0 ~ warrior_6_20 = 动作6
        
        // 从Resources或AssetDatabase加载sprite
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Characters/warrior");
        
        if (allSprites == null || allSprites.Length == 0)
        {
            Debug.LogWarning("未找到warrior sprites在Resources中，尝试从AssetDatabase加载...");
            allSprites = LoadSpritesFromAssetDatabase();
        }
        
        if (allSprites == null || allSprites.Length == 0)
        {
            Debug.LogError("无法加载warrior sprites！");
            return;
        }
        
        Debug.Log($"<color=cyan>✓ 加载了 {allSprites.Length} 个warrior sprite</color>");
        
        AssignSpritesToAnimations(allSprites);
    }
    
    /// <summary>
    /// 从AssetDatabase加载sprites
    /// </summary>
    private Sprite[] LoadSpritesFromAssetDatabase()
    {
#if UNITY_EDITOR
        string path = "Assets/_Project/Art/Sprites/Characters/warrior.png";
        Object[] objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
        Sprite[] sprites = System.Array.FindAll(objects, obj => obj is Sprite)
            .Select(obj => obj as Sprite).ToArray();
        
        if (sprites.Length > 0)
        {
            Debug.Log($"✓ 从AssetDatabase加载 {sprites.Length} 个sprites");
        }
        
        return sprites;
#else
        Debug.LogError("Cannot load sprites from AssetDatabase in Play mode");
        return null;
#endif
    }
    
    /// <summary>
    /// 将sprites分配到对应的动画数组
    /// </summary>
    private void AssignSpritesToAnimations(Sprite[] allSprites)
    {
        // 创建字典来存储按动作编号分组的sprites
        var actionSprites = new System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<Sprite>>();
        
        // 初始化字典
        for (int i = 0; i <= 6; i++)
        {
            actionSprites[i] = new System.Collections.Generic.List<Sprite>();
        }
        
        foreach (Sprite sprite in allSprites)
        {
            string name = sprite.name.ToLower();
            
            // 匹配 warrior_X_Y 格式（X=动作类型, Y=帧序列）
            var match = System.Text.RegularExpressions.Regex.Match(name, @"warrior_(\d+)_(\d+)");
            if (match.Success)
            {
                int actionType = int.Parse(match.Groups[1].Value);
                if (actionType >= 0 && actionType <= 6)
                {
                    actionSprites[actionType].Add(sprite);
                }
            }
        }
        
        // 按SPD的约定，假设：
        // 动作0 = Idle
        // 动作1 = WalkDown
        // 动作2 = WalkUp
        // 动作3 = WalkLeft
        // 动作4 = WalkRight
        // 动作5 = Attack
        // 动作6 = Death
        
        idleSprites = SortAndReturnSprites(actionSprites, 0);
        walkDownSprites = SortAndReturnSprites(actionSprites, 1);
        walkUpSprites = SortAndReturnSprites(actionSprites, 2);
        walkLeftSprites = SortAndReturnSprites(actionSprites, 3);
        walkRightSprites = SortAndReturnSprites(actionSprites, 4);
        
        Debug.Log($"<color=yellow>✓ 动画帧分配完成:</color>");
        Debug.Log($"  Idle: {idleSprites.Length} frames");
        Debug.Log($"  WalkDown: {walkDownSprites.Length} frames");
        Debug.Log($"  WalkUp: {walkUpSprites.Length} frames");
        Debug.Log($"  WalkLeft: {walkLeftSprites.Length} frames");
        Debug.Log($"  WalkRight: {walkRightSprites.Length} frames");
        
        // 如果某个方向没有sprite，使用idle作为备用
        if (idleSprites.Length == 0 && allSprites.Length > 0)
        {
            idleSprites = new Sprite[] { allSprites[0] };
        }
        
        if (walkDownSprites.Length == 0) walkDownSprites = idleSprites;
        if (walkUpSprites.Length == 0) walkUpSprites = idleSprites;
        if (walkLeftSprites.Length == 0) walkLeftSprites = idleSprites;
        if (walkRightSprites.Length == 0) walkRightSprites = idleSprites;
    }
    
    /// <summary>
    /// 从字典中提取指定动作的sprites并按帧序列排序
    /// </summary>
    private Sprite[] SortAndReturnSprites(System.Collections.Generic.Dictionary<int, System.Collections.Generic.List<Sprite>> actionSprites, int actionType)
    {
        if (!actionSprites.ContainsKey(actionType) || actionSprites[actionType].Count == 0)
        {
            return new Sprite[0];
        }
        
        // 按sprite名称中的帧序列号排序
        var sorted = actionSprites[actionType].OrderBy(s =>
        {
            var match = System.Text.RegularExpressions.Regex.Match(s.name, @"warrior_\d+_(\d+)");
            return match.Success ? int.Parse(match.Groups[1].Value) : 0;
        }).ToArray();
        
        return sorted;
    }
}

