# warrior.png Sprite索引与路径指南

##  文件位置

### 物理路径 (硬盘上的位置)
\\\
D:\Program Files\Unity\Hub\Project\DungeonRoguelike\
 Assets\
     _Project\
         Art\
             Sprites\
                 Characters\
                     warrior.png           主sprite文件
                     warrior.png.meta      Unity元数据
\\\

### 项目中的路径 (在Unity中使用)
\\\
Assets/_Project/Art/Sprites/Characters/warrior.png
\\\

### 代码中的引用
\\\csharp
// 在HeroAnimator中
// 方法1: 从Resources加载
Sprite[] allSprites = Resources.LoadAll<Sprite>(\"Characters/warrior\");

// 方法2: 从AssetDatabase加载
#if UNITY_EDITOR
string path = \"Assets/_Project/Art/Sprites/Characters/warrior.png\";
Object[] objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(path);
Sprite[] sprites = System.Array.FindAll(objects, obj => obj is Sprite)
    .Select(obj => obj as Sprite).ToArray();
#endif
\\\

---

##  68个Sprites完整索引

### 组织方式: warrior_X_Y
- **X**: 动作类型 (0-6)
- **Y**: 帧序列 (0-20)
- **总数**: 7种动作  21帧 = 147个命名 (但只有68个实际sprites)

### 完整索引表

| 动作类型 | 范围 | 帧数 | 说明 |
|---------|------|------|------|
| 0 | warrior_0_0 ~ warrior_0_20 | 21帧 | Idle (待机) |
| 1 | warrior_1_0 ~ warrior_1_20 | 21帧 | WalkDown (向下走) |
| 2 | warrior_2_0 ~ warrior_2_20 | 21帧 | WalkUp (向上走) |
| 3 | warrior_3_0 ~ warrior_3_20 | 21帧 | WalkLeft (向左走) |
| 4 | warrior_4_0 ~ warrior_4_20 | 21帧 | WalkRight (向右走) |
| 5 | warrior_5_0 ~ warrior_5_20 | 21帧 | Attack (攻击) |
| 6 | warrior_6_0 ~ warrior_6_20 | 21帧 | Death (死亡) |

### 按名称列出所有68个Sprites

\\\
动作0 (Idle - 待机):
  warrior_0, warrior_0_0, warrior_0_1, warrior_0_2, ..., warrior_0_20

动作1 (WalkDown - 向下走):
  warrior_1, warrior_1_0, warrior_1_1, warrior_1_2, ..., warrior_1_20

动作2 (WalkUp - 向上走):
  warrior_2, warrior_2_0, warrior_2_1, warrior_2_2, ..., warrior_2_20

动作3 (WalkLeft - 向左走):
  warrior_3, warrior_3_0, warrior_3_1, warrior_3_2, ..., warrior_3_20

动作4 (WalkRight - 向右走):
  warrior_4, warrior_4_0, warrior_4_1, warrior_4_2, ..., warrior_4_20

动作5 (Attack - 攻击):
  warrior_5, warrior_5_0, warrior_5_1, warrior_5_2, ..., warrior_5_20

动作6 (Death - 死亡):
  warrior_6, warrior_6_0, warrior_6_1, warrior_6_2, ..., warrior_6_20
\\\

---

##  如何在Unity中查看

### 方法1: 项目窗口直接查看
1. 打开Project窗口 (Ctrl+Shift+P)
2. 导航到: Assets > _Project > Art > Sprites > Characters
3. 点击warrior.png
4. 在Inspector中可以看到所有68个sprites的列表

### 方法2: 代码中动态加载
\\\csharp
// 在HeroAnimator中自动加载
public void LoadWarriorSprites()
{
    Sprite[] allSprites = Resources.LoadAll<Sprite>(\"Characters/warrior\");
    
    foreach (Sprite sprite in allSprites)
    {
        Debug.Log(sprite.name);  // 输出sprite名称
    }
}
\\\

Console输出:
\\\
warrior_0
warrior_0_0
warrior_0_1
...
warrior_6_20
\\\

### 方法3: 使用检查工具
\\\
Tools > Test > Load Game Scene and Test Hero Animation
\\\

这个工具会在Console显示加载的sprite详情。

---

##  Sprite属性

每个sprite的属性:
- **Sprite Mode**: Multiple (多个sprite)
- **Pixels Per Unit**: 16
- **Filter Mode**: Point (no filter)
- **Compression**: None
- **大小**: 1616 像素

---

##  在代码中使用

### 例子1: 获取Idle第一帧
\\\csharp
Sprite[] idleSprites = LoadSpritesForAction(0);
spriteRenderer.sprite = idleSprites[0];  // warrior_0_0
\\\

### 例子2: 播放行走动画
\\\csharp
Sprite[] walkDownSprites = LoadSpritesForAction(1);
// 循环播放 warrior_1_0 到 warrior_1_20
for (int i = 0; i < walkDownSprites.Length; i++)
{
    spriteRenderer.sprite = walkDownSprites[i];
    yield return new WaitForSeconds(0.1f);  // 10fps
}
\\\

### 例子3: 按名称获取特定sprite
\\\csharp
Sprite spriteByName = Resources.Load<Sprite>(\"Characters/warrior_5_10\");
// 获取Attack动作的第10帧
\\\

---

##  文件信息

| 属性 | 值 |
|------|-----|
| 路径 | Assets/_Project/Art/Sprites/Characters/warrior.png |
| 完整路径 | D:\Program Files\Unity\Hub\Project\DungeonRoguelike\Assets\_Project\Art\Sprites\Characters\warrior.png |
| 文件大小 | 3.43 KB |
| Sprite总数 | 68个 |
| 图片分辨率 | 1616像素/sprite |
| Meta文件 | warrior.png.meta |
| 最后修改 | 2025/10/16 10:36:44 |

---

##  相关文件结构

\\\
Assets/_Project/
 Art/
    Sprites/
        Characters/
           warrior.png           Hero sprites
           warrior.png.meta
        Tiles/
            TileAssets/           地形tiles

 Scripts/
    Actors/
        Hero/
            Hero.cs
            HeroAnimator.cs       加载和使用warrior sprites
            HeroClass.cs

 Scenes/
     Game.unity
     TestDungeon.unity
\\\

---

##  总结

### warrior.png 的sprite命名逻辑

\\\
warrior_X_Y
     X (0-6):   动作类型
        0: Idle (待机)
        1: WalkDown (向下走)
        2: WalkUp (向上走)
        3: WalkLeft (向左走)
        4: WalkRight (向右走)
        5: Attack (攻击)
        6: Death (死亡)
    
     Y (0-20):  该动作中的帧序列 (0 = 第1帧, 20 = 第21帧)
\\\

### 快速访问指南

**在Unity编辑器中**:
- Project窗口: Assets > _Project > Art > Sprites > Characters > warrior.png

**在代码中**:
- Resources路径: Characters/warrior
- AssetDatabase路径: Assets/_Project/Art/Sprites/Characters/warrior.png
- 单个sprite: warrior_0_0 (名称直接使用)

**在HeroAnimator中**:
- 自动加载所有68个sprites
- 按动作类型分类到5个数组
- 按帧序列号排序

---

现在你知道所有的sprite文件都在这个位置了！
