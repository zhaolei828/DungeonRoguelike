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
        // 从Resources或AssetDatabase加载sprite
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Characters/warrior");
        
        if (allSprites == null || allSprites.Length == 0)
        {
            Debug.LogWarning("未找到warrior sprites，尝试从AssetDatabase加载...");
            allSprites = LoadSpritesFromAssetDatabase("Assets/_Project/Art/Sprites/Characters/warrior.png");
        }
        
        if (allSprites == null || allSprites.Length == 0)
        {
            Debug.LogError("无法加载warrior sprites！");
            return;
        }
        
        Debug.Log($"<color=cyan>加载了 {allSprites.Length} 个warrior sprite</color>");
        
        // 根据sprite名称分配到不同的动画数组
        // 假设命名格式: warrior_idle_0, warrior_walk_down_0, 等
        AssignSpritesToAnimations(allSprites);
    }
    
    /// <summary>
    /// 从AssetDatabase加载sprites
    /// </summary>
    private Sprite[] LoadSpritesFromAssetDatabase(string path)
    {
#if UNITY_EDITOR
        Object[] objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
        Sprite[] sprites = System.Array.FindAll(objects, obj => obj is Sprite)
            .Select(obj => obj as Sprite).ToArray();
        return sprites;
#else
        return null;
#endif
    }
    
    /// <summary>
    /// 将sprites分配到对应的动画数组
    /// </summary>
    private void AssignSpritesToAnimations(Sprite[] allSprites)
    {
        System.Collections.Generic.List<Sprite> idle = new System.Collections.Generic.List<Sprite>();
        System.Collections.Generic.List<Sprite> walkDown = new System.Collections.Generic.List<Sprite>();
        System.Collections.Generic.List<Sprite> walkUp = new System.Collections.Generic.List<Sprite>();
        System.Collections.Generic.List<Sprite> walkLeft = new System.Collections.Generic.List<Sprite>();
        System.Collections.Generic.List<Sprite> walkRight = new System.Collections.Generic.List<Sprite>();
        
        foreach (Sprite sprite in allSprites)
        {
            string name = sprite.name.ToLower();
            
            if (name.Contains("idle"))
                idle.Add(sprite);
            else if (name.Contains("walk_down") || name.Contains("down"))
                walkDown.Add(sprite);
            else if (name.Contains("walk_up") || name.Contains("up"))
                walkUp.Add(sprite);
            else if (name.Contains("walk_left") || name.Contains("left"))
                walkLeft.Add(sprite);
            else if (name.Contains("walk_right") || name.Contains("right"))
                walkRight.Add(sprite);
        }
        
        idleSprites = idle.ToArray();
        walkDownSprites = walkDown.ToArray();
        walkUpSprites = walkUp.ToArray();
        walkLeftSprites = walkLeft.ToArray();
        walkRightSprites = walkRight.ToArray();
        
        Debug.Log($"<color=yellow>动画帧分配: Idle={idleSprites.Length}, Down={walkDownSprites.Length}, Up={walkUpSprites.Length}, Left={walkLeftSprites.Length}, Right={walkRightSprites.Length}</color>");
        
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
}

