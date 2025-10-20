using UnityEngine;

/// <summary>
/// Hero 生成器 - 在关卡入口生成英雄
/// </summary>
public class HeroSpawner : MonoBehaviour
{
    [SerializeField] private HeroClass defaultHeroClass = HeroClass.Warrior;
    [SerializeField] private GameObject heroPrefabReference; // 可选的预制体引用
    
    private Hero currentHero;
    
    private void Start()
    {
        SpawnHero();
    }
    
    /// <summary>
    /// 生成英雄
    /// </summary>
    public Hero SpawnHero(HeroClass? heroClass = null)
    {
        // 清除之前的英雄
        if (currentHero != null)
        {
            Destroy(currentHero.gameObject);
        }
        
        HeroClass classToUse = heroClass ?? defaultHeroClass;
        
        // 创建英雄 GameObject
        GameObject heroGO = new GameObject($"Hero_{classToUse}");
        heroGO.transform.SetParent(transform);
        
        // 添加 Hero 组件
        Hero hero = heroGO.AddComponent<Hero>();
        
        // 添加 Sprite Renderer（临时用彩色方块表示）
        SpriteRenderer spriteRenderer = heroGO.AddComponent<SpriteRenderer>();
        
        // 创建一个临时的彩色方块Sprite
        Texture2D texture = new Texture2D(16, 16);
        Color heroColor = Color.yellow; // 英雄用黄色
        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 16; x++)
            {
                texture.SetPixel(x, y, heroColor);
            }
        }
        texture.Apply();
        texture.filterMode = FilterMode.Point; // 像素风格
        
        Sprite heroSprite = Sprite.Create(texture, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        spriteRenderer.sprite = heroSprite;
        spriteRenderer.sortingOrder = 10; // 在地形上层显示
        
        Debug.Log("<color=yellow>Hero sprite created (temporary yellow square)</color>");
        
        // 添加 Box Collider
        BoxCollider2D collider = heroGO.AddComponent<BoxCollider2D>();
        collider.isTrigger = false;
        
        // 设置初始位置为地牢入口
        Level level = LevelManager.Instance.CurrentLevel;
        if (level != null)
        {
            Vector2Int entrancePos = level.EntrancePos;
            heroGO.transform.position = new Vector3(entrancePos.x + 0.5f, entrancePos.y + 0.5f, 0);
            Debug.Log($"<color=cyan>Hero spawned at entrance: {entrancePos}</color>");
        }
        else
        {
            Debug.LogWarning("No level found when spawning hero!");
            heroGO.transform.position = Vector3.zero;
        }
        
        currentHero = hero;
        return hero;
    }
    
    /// <summary>
    /// 获取当前英雄
    /// </summary>
    public Hero GetCurrentHero() => currentHero;
}
