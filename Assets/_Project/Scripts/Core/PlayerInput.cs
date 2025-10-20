using UnityEngine;

/// <summary>
/// 玩家输入处理 - 处理键盘输入并控制Hero移动
/// </summary>
public class PlayerInput : MonoBehaviour
{
    private Hero hero;
    private Level currentLevel;
    
    private void Start()
    {
        // 获取Hero引用
        hero = GameManager.Instance?.Hero;
        if (hero == null)
        {
            Debug.LogError("PlayerInput: Hero not found in GameManager!");
        }
        
        // 获取当前关卡
        currentLevel = LevelManager.Instance?.CurrentLevel;
        if (currentLevel == null)
        {
            Debug.LogError("PlayerInput: Current level not found!");
        }
    }
    
    private void Update()
    {
        if (hero == null || currentLevel == null)
            return;
        
        // 检测方向键输入
        Vector2Int moveDirection = Vector2Int.zero;
        
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            moveDirection = new Vector2Int(0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveDirection = new Vector2Int(0, -1);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveDirection = new Vector2Int(-1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveDirection = new Vector2Int(1, 0);
        }
        
        // 如果有移动输入，执行移动
        if (moveDirection != Vector2Int.zero)
        {
            Vector2Int targetPos = hero.pos + moveDirection;
            hero.MoveTo(targetPos, currentLevel);
            
            // 更新Hero的Transform位置
            UpdateHeroTransform();
        }
    }
    
    /// <summary>
    /// 更新Hero的Transform位置以匹配其逻辑位置
    /// </summary>
    private void UpdateHeroTransform()
    {
        if (hero != null)
        {
            // 将逻辑坐标转换为世界坐标（Tilemap中心对齐）
            hero.transform.position = new Vector3(hero.pos.x + 0.5f, hero.pos.y + 0.5f, 0);
        }
    }
}

