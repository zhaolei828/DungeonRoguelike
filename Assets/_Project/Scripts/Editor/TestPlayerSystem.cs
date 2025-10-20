using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

/// <summary>
/// 玩家系统自动测试工具
/// </summary>
public class TestPlayerSystem : EditorWindow
{
    [MenuItem("Tools/Test/Test Player System")]
    public static void ShowWindow()
    {
        GetWindow<TestPlayerSystem>("Test Player System");
    }

    private void OnGUI()
    {
        GUILayout.Label("Week 5 - 玩家系统测试", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("1. 验证Game场景配置"))
        {
            ValidateGameScene();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("2. 进入Play模式测试"))
        {
            EnterPlayMode();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("3. 检查Hero组件"))
        {
            CheckHeroComponents();
        }

        GUILayout.Space(5);

        if (GUILayout.Button("4. 查看测试指南"))
        {
            OpenTestGuide();
        }
    }

    private void ValidateGameScene()
    {
        string gameScenePath = "Assets/_Project/Scenes/Game.unity/Game.unity";
        Scene gameScene = EditorSceneManager.OpenScene(gameScenePath, OpenSceneMode.Single);

        string report = "=== Game场景配置验证 ===\n\n";
        int passCount = 0;
        int totalCount = 0;

        // 检查GameController
        totalCount++;
        GameObject gameController = GameObject.Find("GameController");
        if (gameController != null && gameController.GetComponent<GameInitializer>() != null)
        {
            report += "✓ GameController + GameInitializer\n";
            passCount++;
        }
        else
        {
            report += "✗ GameController 或 GameInitializer 缺失\n";
        }

        // 检查GameSystems
        totalCount++;
        GameObject gameSystems = GameObject.Find("GameSystems");
        if (gameSystems != null)
        {
            bool hasLevelManager = gameSystems.GetComponent<LevelManager>() != null;
            bool hasGameManager = gameSystems.GetComponent<GameManager>() != null;
            if (hasLevelManager && hasGameManager)
            {
                report += "✓ GameSystems + LevelManager + GameManager\n";
                passCount++;
            }
            else
            {
                report += "✗ GameSystems 缺少组件\n";
            }
        }
        else
        {
            report += "✗ GameSystems 缺失\n";
        }

        // 检查Main Camera
        totalCount++;
        Camera mainCamera = Camera.main;
        if (mainCamera != null && mainCamera.orthographic)
        {
            report += "✓ Main Camera (Orthographic)\n";
            passCount++;
        }
        else
        {
            report += "✗ Main Camera 配置错误\n";
        }

        // 检查LevelRenderer
        totalCount++;
        LevelRenderer renderer = FindFirstObjectByType<LevelRenderer>();
        if (renderer != null)
        {
            report += "✓ LevelRenderer\n";
            passCount++;
        }
        else
        {
            report += "✗ LevelRenderer 缺失\n";
        }

        // 检查Tilemaps
        totalCount++;
        GameObject groundTilemap = GameObject.Find("GroundTilemap");
        GameObject wallTilemap = GameObject.Find("WallTilemap");
        GameObject decorationTilemap = GameObject.Find("DecorationTilemap");
        if (groundTilemap != null && wallTilemap != null && decorationTilemap != null)
        {
            report += "✓ Tilemaps (Ground, Wall, Decoration)\n";
            passCount++;
        }
        else
        {
            report += "✗ Tilemaps 缺失\n";
        }

        report += $"\n通过: {passCount}/{totalCount}\n";

        if (passCount == totalCount)
        {
            report += "\n<color=green>✓ 场景配置完整，可以进入Play模式测试！</color>";
        }
        else
        {
            report += "\n<color=red>✗ 场景配置不完整，请先修复上述问题。</color>";
        }

        Debug.Log(report);
        EditorUtility.DisplayDialog("场景验证", report.Replace("<color=green>", "").Replace("</color>", "").Replace("<color=red>", ""), "确定");
    }

    private void EnterPlayMode()
    {
        string gameScenePath = "Assets/_Project/Scenes/Game.unity/Game.unity";
        EditorSceneManager.OpenScene(gameScenePath, OpenSceneMode.Single);
        EditorApplication.isPlaying = true;
        Debug.Log("<color=cyan>=== 进入Play模式，请使用WASD/方向键测试Hero移动 ===</color>");
    }

    private void CheckHeroComponents()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorUtility.DisplayDialog("提示", "请先进入Play模式！", "确定");
            return;
        }

        Hero hero = FindFirstObjectByType<Hero>();
        if (hero == null)
        {
            EditorUtility.DisplayDialog("错误", "未找到Hero！请检查HeroSpawner是否正常工作。", "确定");
            return;
        }

        string report = "=== Hero组件检查 ===\n\n";
        report += $"职业: {hero.Class}\n";
        report += $"等级: {hero.Level}\n";
        report += $"经验: {hero.Experience}\n";
        report += $"生命: {hero.Hp}/{hero.MaxHp}\n";
        report += $"法力: {hero.Mana}/{hero.MaxMana}\n";
        report += $"护甲: {hero.Armor}\n";
        report += $"力量: {hero.Strength}\n";
        report += $"智力: {hero.Intelligence}\n";
        report += $"意志: {hero.Willpower}\n";
        report += $"位置: {hero.pos}\n";

        Debug.Log(report);
        EditorUtility.DisplayDialog("Hero组件", report, "确定");
    }

    private void OpenTestGuide()
    {
        string guidePath = "Docs/Week5_玩家系统测试指南.md";
        System.Diagnostics.Process.Start(guidePath);
    }
}

