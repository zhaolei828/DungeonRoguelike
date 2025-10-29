using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 回合管理器 - 管理战斗中的回合顺序和执行
/// </summary>
public class TurnManager : Singleton<TurnManager>
{
    [Header("战斗状态")]
    [SerializeField] private bool isInBattle = false;
    [SerializeField] private Hero currentHero;
    [SerializeField] private Mob currentEnemy;
    
    // 战斗参与者列表 (按敏捷度排序)
    private List<Actor> turnOrder = new List<Actor>();
    private int currentTurnIndex = 0;
    
    [Header("战斗参数")]
    [SerializeField] private float turnDuration = 1f; // 每回合的时间限制
    private float turnTimer = 0f;

    [Header("UI引用")]
    private BattleInfoPanel battleInfoPanel;
    
    // 战斗状态枚举
    public enum BattleState
    {
        Idle,           // 非战斗状态
        Starting,       // 战斗开始
        HeroTurn,       // Hero回合
        EnemyTurn,      // 敌人回合
        Ending,         // 战斗结束
        HeroVictory,    // Hero胜利
        HeroDefeat      // Hero失败
    }
    
    private BattleState currentState = BattleState.Idle;
    public BattleState CurrentState => currentState;
    public bool IsInBattle => isInBattle;
    
    protected override void Awake()
    {
        base.Awake();
        
        // 查找BattleInfoPanel
        battleInfoPanel = FindObjectOfType<BattleInfoPanel>();
        if (battleInfoPanel == null)
        {
            Debug.LogWarning("TurnManager: 未找到BattleInfoPanel，战斗UI将不显示");
        }
    }

    /// <summary>
    /// 开始战斗
    /// </summary>
    public void StartBattle(Hero hero, Mob enemy)
    {
        if (isInBattle)
        {
            Debug.LogWarning("已经在战斗中！");
            return;
        }
        
        currentHero = hero;
        currentEnemy = enemy;
        isInBattle = true;
        currentState = BattleState.Starting;
        
        Debug.Log($"<color=cyan>⚔️ 战斗开始！{hero.gameObject.name} vs {enemy.MobName}</color>");
        
        // 显示战斗面板
        if (battleInfoPanel != null)
        {
            battleInfoPanel.Show();
            battleInfoPanel.ClearLog();
            battleInfoPanel.AddBattleLog($"战斗开始！{hero.gameObject.name} vs {enemy.MobName}", Color.yellow);
        }
        
        // 初始化回合顺序
        InitializeTurnOrder();
        
        // 开始第一个回合
        StartNextTurn();
    }
    
    /// <summary>
    /// 初始化回合顺序（按敏捷度排序）
    /// </summary>
    private void InitializeTurnOrder()
    {
        turnOrder.Clear();
        turnOrder.Add(currentHero);
        turnOrder.Add(currentEnemy);
        
        // 按敏捷度排序（敏捷度高的先手）
        // Hero的敏捷度通过Strength属性计算
        turnOrder.Sort((a, b) =>
        {
            int aAgility = 10; // Hero默认敏捷度
            int bAgility = currentEnemy.Agility;
            
            if (a is Mob mobA)
                aAgility = mobA.Agility;
            
            return bAgility.CompareTo(aAgility); // 降序排列
        });
        
        currentTurnIndex = 0;
    }
    
    /// <summary>
    /// 开始下一个回合
    /// </summary>
    private void StartNextTurn()
    {
        if (!isInBattle)
            return;
        
        currentTurnIndex = (currentTurnIndex + 1) % turnOrder.Count;
        turnTimer = 0f;
        
        Actor currentActor = turnOrder[currentTurnIndex];
        
        if (currentActor is Hero hero)
        {
            currentState = BattleState.HeroTurn;
            Debug.Log("<color=yellow>▶ Hero的回合</color>");
            
            // 更新UI
            if (battleInfoPanel != null)
            {
                battleInfoPanel.UpdateTurnIndicator("Hero的回合", Color.green);
            }
        }
        else if (currentActor is Mob mob)
        {
            currentState = BattleState.EnemyTurn;
            Debug.Log($"<color=yellow>▶ {mob.MobName} 的回合</color>");
            
            // 更新UI
            if (battleInfoPanel != null)
            {
                battleInfoPanel.UpdateTurnIndicator($"{mob.MobName}的回合", Color.red);
            }
            
            // AI自动执行
            ExecuteEnemyTurn(mob);
        }
    }
    
    /// <summary>
    /// 执行Hero的攻击
    /// </summary>
    public void ExecuteHeroAttack()
    {
        if (currentState != BattleState.HeroTurn || currentHero == null || currentEnemy == null)
            return;
        
        // 计算伤害
        int damage = CombatCalculator.CalculateDamage(currentHero, currentEnemy);
        bool isCritical = Random.value < (currentHero.Strength * 0.03f);
        
        // 应用伤害
        currentEnemy.TakeDamage(damage, isCritical);
        
        Debug.Log($"<color=red>✦ Hero 对 {currentEnemy.MobName} 造成 {damage} 伤害</color>");
        
        // 添加战斗日志
        if (battleInfoPanel != null)
        {
            string logMessage = $"Hero 对 {currentEnemy.MobName} 造成 {damage} 伤害";
            Color logColor = isCritical ? Color.yellow : Color.white;
            battleInfoPanel.AddBattleLog(logMessage, logColor);
        }
        
        // 检查敌人是否死亡
        if (currentEnemy.Hp <= 0)
        {
            EndBattle(BattleState.HeroVictory);
        }
        else
        {
            // 继续到下一个回合
            StartNextTurn();
        }
    }
    
    /// <summary>
    /// 执行敌人的回合
    /// </summary>
    private void ExecuteEnemyTurn(Mob enemy)
    {
        if (currentHero == null || !currentHero.gameObject.activeSelf)
        {
            EndBattle(BattleState.HeroDefeat);
            return;
        }
        
        // 计算伤害
        int damage = CombatCalculator.CalculateDamage(enemy, currentHero);
        bool isCritical = Random.value < (enemy.Agility * 0.02f);
        
        // 应用伤害到Hero
        currentHero.TakeDamage(damage, isCritical);
        
        Debug.Log($"<color=red>✦ {enemy.MobName} 对 Hero 造成 {damage} 伤害</color>");
        
        // 添加战斗日志
        if (battleInfoPanel != null)
        {
            string logMessage = $"{enemy.MobName} 对 Hero 造成 {damage} 伤害";
            Color logColor = isCritical ? Color.yellow : Color.white;
            battleInfoPanel.AddBattleLog(logMessage, logColor);
        }
        
        // 检查Hero是否死亡
        if (currentHero.Hp <= 0)
        {
            EndBattle(BattleState.HeroDefeat);
        }
        else
        {
            // 延迟后进入下一回合
            Invoke("StartNextTurn", 1f);
        }
    }
    
    /// <summary>
    /// 结束战斗
    /// </summary>
    private void EndBattle(BattleState finalState)
    {
        currentState = finalState;
        isInBattle = false;
        
        if (finalState == BattleState.HeroVictory)
        {
            Debug.Log($"<color=green>🎉 胜利！{currentEnemy.MobName} 被击败了！</color>");
            
            // 添加胜利日志
            if (battleInfoPanel != null)
            {
                battleInfoPanel.AddBattleLog($"胜利！{currentEnemy.MobName} 被击败了！", Color.green);
            }
        }
        else if (finalState == BattleState.HeroDefeat)
        {
            Debug.Log($"<color=red>💀 失败！Hero 被击败了...</color>");
            
            // 添加失败日志
            if (battleInfoPanel != null)
            {
                battleInfoPanel.AddBattleLog("失败！Hero 被击败了...", Color.red);
            }
        }
        
        // 延迟隐藏面板
        if (battleInfoPanel != null)
        {
            Invoke(nameof(HideBattlePanel), 2f);
        }
        
        turnOrder.Clear();
        currentTurnIndex = 0;
    }

    /// <summary>
    /// 隐藏战斗面板
    /// </summary>
    private void HideBattlePanel()
    {
        if (battleInfoPanel != null)
        {
            battleInfoPanel.Hide();
        }
    }
    
    /// <summary>
    /// Hero逃离战斗
    /// </summary>
    public void HeroFlee()
    {
        if (currentState != BattleState.HeroTurn)
            return;
        
        Debug.Log("<color=yellow>⊕ Hero 选择逃离！</color>");
        
        if (currentEnemy != null)
        {
            currentEnemy.ExitCombat();
        }
        
        EndBattle(BattleState.Ending);
    }
    
    private void Update()
    {
        if (!isInBattle)
            return;
        
        // 更新回合计时
        turnTimer += Time.deltaTime;
        
        // 回合超时 (可选，防止AI卡顿)
        if (turnTimer > turnDuration * 5)
        {
            if (currentState == BattleState.EnemyTurn)
            {
                StartNextTurn();
            }
        }
    }
}
