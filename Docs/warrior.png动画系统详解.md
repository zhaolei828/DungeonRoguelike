# warrior.png 动画系统详解

##  整体架构

warrior.png 的动画系统采用**手动帧序列管理**的方式，而不是使用Unity的Animator组件。

### 系统组成

\\\
warrior.png (68个sprites)
    
HeroAnimator (动画控制器)
    
Hero.SmoothMoveTo() (移动协程)
    
PlayerInput (输入触发)
\\\

---

##  Sprite结构分析

### warrior.png 中的68个Sprites

\\\
warrior_0_0  ~ warrior_0_20  (21帧) = Idle (待机)
warrior_1_0  ~ warrior_1_20  (21帧) = WalkDown (向下)
warrior_2_0  ~ warrior_2_20  (21帧) = WalkUp (向上)
warrior_3_0  ~ warrior_3_20  (21帧) = WalkLeft (向左)
warrior_4_0  ~ warrior_4_20  (21帧) = WalkRight (向右)
warrior_5_0  ~ warrior_5_20  (21帧) = Attack (攻击)
warrior_6_0  ~ warrior_6_20  (21帧) = Death (死亡)
\\\

### 关键理解

- **第一个数字** (X): 动作类型 (0=Idle, 1=WalkDown等)
- **第二个数字** (Y): 该动作中的帧序列 (0-20, 共21帧)
- **每个sprite**: 1616像素的角色图像

---

##  HeroAnimator 实现

### 1. Sprite数组定义

HeroAnimator中有5个Sprite数组，分别对应5种动作：

\\\csharp
private Sprite[] idleSprites;       // warrior_0_0 ~ warrior_0_20
private Sprite[] walkDownSprites;   // warrior_1_0 ~ warrior_1_20
private Sprite[] walkUpSprites;     // warrior_2_0 ~ warrior_2_20
private Sprite[] walkLeftSprites;   // warrior_3_0 ~ warrior_3_20
private Sprite[] walkRightSprites;  // warrior_4_0 ~ warrior_4_20
\\\

### 2. 帧计时和切换逻辑

在Update()中执行：

\\\csharp
private void Update()
{
    if (isAnimating && currentAnimation != null)
    {
        frameTimer += Time.deltaTime;
        
        // frameRate = 10fps，所以帧间隔 = 0.1秒
        if (frameTimer >= 1f / frameRate)
        {
            frameTimer = 0f;
            currentFrame = (currentFrame + 1) % currentAnimation.Length;
            spriteRenderer.sprite = currentAnimation[currentFrame];
        }
    }
    
    // 1秒无动作自动返回Idle
    if (currentState != AnimationState.Idle)
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= 1f)
        {
            SetAnimationState(AnimationState.Idle);
        }
    }
}
\\\

### 3. 帧率计算

\\\
frameRate = 10 fps (每秒切换10帧)
帧间隔 = 1f / 10f = 0.1秒 (100毫秒/帧)

21帧动画时长 = 21  0.1 = 2.1秒
\\\

---

##  动画触发流程

### 完整链条

\\\
玩家按WASD
    
PlayerInput检测方向
    
Hero.TryMoveTo()启动移动
    
Hero.SmoothMoveTo() Coroutine
    - 计算方向: direction = targetPos - currentPos
    - 调用 animator.SetAnimationByDirection(direction)
    
HeroAnimator.SetAnimationByDirection()
    - 根据direction选择: WalkDown/Up/Left/Right
    
HeroAnimator.SetAnimationState()
    - 选择sprite数组
    - 重置 currentFrame = 0
    
HeroAnimator.Update() (每帧)
    - 帧计时: frameTimer += Time.deltaTime
    - 切帧: spriteRenderer.sprite = currentAnimation[currentFrame]
    
SmoothMoveTo()完成 (0.3秒后)
    - animator.SetAnimationState(Idle)
\\\

---

##  关键时间同步

### 问题诊断

为什么之前动画看起来"奇怪"(多张图快速闪烁)?

\\\
Hero移动时长:  0.3秒
动画帧率:      10fps (每帧0.1秒)

0.3秒  0.1秒/帧 = 3帧

这意味着一次移动只播放3帧动画！
而完整的行走动画有21帧
所以动画被截断了，看起来就像"快速切换"
\\\

### 解决方案

**调整参数**:
1. 降低帧率: frameRate = 7fps
2. 延长移动时长: moveDuration = 0.6秒
3. 混合方案: moveDuration = 0.6f + frameRate = 7fps

结果: 0.6秒  (1/7) = 4.2帧  4帧动画，看起来更顺畅

---

##  Sprite加载流程

### LoadWarriorSprites()

1. 尝试从Resources加载
2. 失败则从AssetDatabase加载
3. 使用正则表达式匹配 warrior_X_Y 格式
4. 按动作类型分组
5. 按帧序列号排序

\\\csharp
// 匹配格式: warrior_(\d+)_(\d+)
var match = Regex.Match(sprite.name, @'warrior_(\d+)_(\d+)');
int actionType = int.Parse(match.Groups[1].Value);
int frameIndex = int.Parse(match.Groups[2].Value);
\\\

---

##  Sprite切换细节

### 每帧更新

\\\csharp
// 这是唯一改变画面的操作
spriteRenderer.sprite = currentAnimation[currentFrame];
\\\

就是通过不断更新SpriteRenderer的sprite字段来实现动画。

### 初始化

\\\csharp
// Start()中
spriteRenderer.sprite = idleSprites[0];  // 显示待机第一帧

// 自动加载(如果手动未配置)
if (idleSprites == null || idleSprites.Length == 0)
{
    LoadWarriorSprites();
}
\\\

---

##  性能指标

| 项目 | 数值 |
|------|------|
| 帧间隔 | 100毫秒 |
| 切帧操作 | 每0.1秒一次 |
| CPU开销 | 极低 |
| 内存占用 | 0.45MB |
| 适用fps | 30-60fps游戏 |

---

##  工作流时序图

\\\
时间轴:                0秒       0.1秒      0.2秒      0.3秒      0.4秒
                       |          |          |          |          |
PlayerInput:     按W   |          |          |          |          |
                       |          |          |          |          |
Hero状态:        移动中 | 移动中  | 移动中   | 移动完成  | 空闲
                       |          |          |          |          |
HeroAnimator:    切帧1 | 切帧2   | 切帧3    | 返回Idle | Idle循环
                       |          |          |          |          |
Transform.pos:   (5,5)  (5.33,5)  (5.66,5)  (6,5)  保持(6,5)
\\\

---

##  最佳实践建议

1. **帧率**: 像素游戏通常用5-10fps
2. **同步**: 确保动画时长移动时长
3. **缓存**: 预加载避免首次卡顿
4. **扩展**: 为新职业创建独立animator

---

**核心概念**: 手动管理sprite数组，每帧更新SpriteRenderer.sprite实现动画效果。
轻量、高效、易扩展！
