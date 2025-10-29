using UnityEngine;

/// <summary>
/// 战斗UI测试脚本 - 用于测试战斗UI系统的各项功能
/// </summary>
public class TestBattleUI : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool autoTest = false;
    [SerializeField] private float testInterval = 2f;

    private float testTimer = 0f;
    private int testStep = 0;

    private void Update()
    {
        if (!autoTest)
            return;

        testTimer += Time.deltaTime;
        if (testTimer >= testInterval)
        {
            testTimer = 0f;
            ExecuteTestStep();
        }
    }

    private void ExecuteTestStep()
    {
        switch (testStep)
        {
            case 0:
                TestHealthBarUpdate();
                break;
            case 1:
                TestDamageNumbers();
                break;
            case 2:
                TestBuffIcons();
                break;
            case 3:
                TestBattlePanel();
                break;
            case 4:
                TestCriticalDamage();
                break;
            case 5:
                TestShieldAbsorb();
                break;
            default:
                testStep = -1;
                Debug.Log("<color=green>✓ 所有测试完成！</color>");
                break;
        }

        testStep++;
    }

    /// <summary>
    /// 测试血条更新
    /// </summary>
    [ContextMenu("Test/1. Health Bar Update")]
    public void TestHealthBarUpdate()
    {
        Debug.Log("<color=cyan>测试：血条更新</color>");

        Hero hero = FindObjectOfType<Hero>();
        if (hero != null)
        {
            hero.TakeDamage(5, false);
            Debug.Log($"Hero受到5点伤害，当前HP: {hero.Hp}/{hero.MaxHp}");
        }
        else
        {
            Debug.LogWarning("未找到Hero！");
        }
    }

    /// <summary>
    /// 测试伤害数字
    /// </summary>
    [ContextMenu("Test/2. Damage Numbers")]
    public void TestDamageNumbers()
    {
        Debug.Log("<color=cyan>测试：伤害数字显示</color>");

        if (DamageNumberManager.Instance == null)
        {
            Debug.LogError("DamageNumberManager未找到！");
            return;
        }

        Vector3 testPos = Camera.main.transform.position + Camera.main.transform.forward * 5f;

        // 测试普通伤害
        DamageNumberManager.Instance.ShowDamage(testPos, 10, DamageNumber.DamageType.Normal);
        Debug.Log("显示普通伤害：10");

        // 测试暴击
        DamageNumberManager.Instance.ShowDamage(testPos + Vector3.right, 20, DamageNumber.DamageType.Critical);
        Debug.Log("显示暴击伤害：20");

        // 测试治疗
        DamageNumberManager.Instance.ShowDamage(testPos + Vector3.left, 15, DamageNumber.DamageType.Heal);
        Debug.Log("显示治疗：15");
    }

    /// <summary>
    /// 测试Buff图标
    /// </summary>
    [ContextMenu("Test/3. Buff Icons")]
    public void TestBuffIcons()
    {
        Debug.Log("<color=cyan>测试：Buff图标显示</color>");

        Hero hero = FindObjectOfType<Hero>();
        if (hero == null || hero.BuffSystem == null)
        {
            Debug.LogWarning("未找到Hero或BuffSystem！");
            return;
        }

        // 添加多个Buff
        hero.BuffSystem.AddBuff(BuffType.Strength, 10f, 2f, "测试");
        hero.BuffSystem.AddBuff(BuffType.Shield, 8f, 1f, "测试");
        hero.BuffSystem.AddBuff(BuffType.Haste, 5f, 1f, "测试");

        Debug.Log("已添加3个Buff：力量、护盾、加速");
    }

    /// <summary>
    /// 测试战斗面板
    /// </summary>
    [ContextMenu("Test/4. Battle Panel")]
    public void TestBattlePanel()
    {
        Debug.Log("<color=cyan>测试：战斗信息面板</color>");

        BattleInfoPanel panel = FindObjectOfType<BattleInfoPanel>();
        if (panel == null)
        {
            Debug.LogWarning("未找到BattleInfoPanel！请先创建。");
            return;
        }

        panel.Show();
        panel.UpdateTurnIndicator("测试回合", Color.yellow);
        panel.AddBattleLog("这是一条测试日志", Color.white);
        panel.AddBattleLog("Hero 对 老鼠 造成 15 伤害", Color.red);
        panel.AddBattleLog("暴击！", Color.yellow);

        Debug.Log("战斗面板已显示并添加测试日志");
    }

    /// <summary>
    /// 测试暴击伤害
    /// </summary>
    [ContextMenu("Test/5. Critical Damage")]
    public void TestCriticalDamage()
    {
        Debug.Log("<color=cyan>测试：暴击伤害</color>");

        Hero hero = FindObjectOfType<Hero>();
        if (hero != null)
        {
            hero.TakeDamage(15, true); // 暴击
            Debug.Log("Hero受到15点暴击伤害！");
        }
    }

    /// <summary>
    /// 测试护盾吸收
    /// </summary>
    [ContextMenu("Test/6. Shield Absorb")]
    public void TestShieldAbsorb()
    {
        Debug.Log("<color=cyan>测试：护盾吸收</color>");

        Hero hero = FindObjectOfType<Hero>();
        if (hero == null || hero.BuffSystem == null)
        {
            Debug.LogWarning("未找到Hero或BuffSystem！");
            return;
        }

        // 添加护盾
        hero.BuffSystem.AddBuff(BuffType.Shield, 10f, 3f, "测试");
        Debug.Log("已添加护盾Buff（强度3）");

        // 造成伤害测试护盾吸收
        hero.TakeDamage(20, false);
        Debug.Log("Hero受到20点伤害（护盾应吸收部分）");
    }

    /// <summary>
    /// 完整战斗流程测试
    /// </summary>
    [ContextMenu("Test/Full Battle Flow")]
    public void TestFullBattleFlow()
    {
        Debug.Log("<color=cyan>=== 开始完整战斗流程测试 ===</color>");

        Hero hero = FindObjectOfType<Hero>();
        Mob[] mobs = FindObjectsOfType<Mob>();

        if (hero == null)
        {
            Debug.LogError("未找到Hero！");
            return;
        }

        if (mobs.Length == 0)
        {
            Debug.LogWarning("未找到怪物！请先生成怪物。");
            return;
        }

        Mob enemy = mobs[0];

        // 开始战斗
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.StartBattle(hero, enemy);
            Debug.Log($"战斗开始：{hero.name} vs {enemy.MobName}");
        }
        else
        {
            Debug.LogError("TurnManager未找到！");
        }
    }

    /// <summary>
    /// 清理所有Buff
    /// </summary>
    [ContextMenu("Test/Clear All Buffs")]
    public void ClearAllBuffs()
    {
        Hero hero = FindObjectOfType<Hero>();
        if (hero != null && hero.BuffSystem != null)
        {
            var buffs = hero.BuffSystem.GetActiveBuffs();
            foreach (var buff in buffs.ToArray())
            {
                hero.BuffSystem.RemoveBuff(buff.Type);
            }
            Debug.Log("已清除所有Buff");
        }
    }

    /// <summary>
    /// 重置Hero血量
    /// </summary>
    [ContextMenu("Test/Reset Hero HP")]
    public void ResetHeroHP()
    {
        Hero hero = FindObjectOfType<Hero>();
        if (hero != null)
        {
            hero.Heal(hero.MaxHp);
            Debug.Log($"Hero血量已重置：{hero.Hp}/{hero.MaxHp}");
        }
    }
}

