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
        
        // 添加 Sprite Renderer（临时用基本方块表示）
        SpriteRenderer spriteRenderer = heroGO.AddComponent<SpriteRenderer>();
        spriteRenderer.color = Color.green;
        spriteRenderer.sortingOrder = 10; // 在地形上层显示
        
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
