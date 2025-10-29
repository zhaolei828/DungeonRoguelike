using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 战斗UI面板创建工具 - 一键创建完整的BattleInfoPanel
/// </summary>
public class BattleUIPanelCreator : EditorWindow
{
    [MenuItem("Tools/UI/Create Battle Info Panel")]
    public static void ShowWindow()
    {
        GetWindow<BattleUIPanelCreator>("创建战斗UI面板");
    }

    private void OnGUI()
    {
        GUILayout.Label("战斗UI面板创建工具", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "此工具将在当前场景的Canvas下创建完整的BattleInfoPanel UI结构。\n" +
            "包括：回合指示器、战斗日志容器、CanvasGroup等所有必要组件。",
            MessageType.Info
        );

        GUILayout.Space(10);

        if (GUILayout.Button("创建 Battle Info Panel", GUILayout.Height(40)))
        {
            CreateBattleInfoPanel();
        }

        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "创建后请检查：\n" +
            "1. BattleInfoPanel组件的引用是否正确\n" +
            "2. Canvas的Render Mode设置\n" +
            "3. 文本字体和大小",
            MessageType.Warning
        );
    }

    private static void CreateBattleInfoPanel()
    {
        // 查找Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            EditorUtility.DisplayDialog("错误", "场景中未找到Canvas！请先创建Canvas。", "确定");
            return;
        }

        // 检查是否已存在BattleInfoPanel
        BattleInfoPanel existingPanel = FindObjectOfType<BattleInfoPanel>();
        if (existingPanel != null)
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "警告",
                "场景中已存在BattleInfoPanel，是否覆盖？",
                "覆盖",
                "取消"
            );

            if (!overwrite)
                return;

            DestroyImmediate(existingPanel.gameObject);
        }

        // 创建主面板
        GameObject panelRoot = new GameObject("BattleInfoPanel");
        panelRoot.transform.SetParent(canvas.transform, false);

        // 添加RectTransform
        RectTransform panelRect = panelRoot.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0, 0);
        panelRect.anchorMax = new Vector2(1, 1);
        panelRect.sizeDelta = Vector2.zero;

        // 添加CanvasGroup
        CanvasGroup canvasGroup = panelRoot.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        // 添加BattleInfoPanel组件
        BattleInfoPanel battlePanel = panelRoot.AddComponent<BattleInfoPanel>();

        // 创建内容容器
        GameObject contentRoot = new GameObject("PanelRoot");
        contentRoot.transform.SetParent(panelRoot.transform, false);
        RectTransform contentRect = contentRoot.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0, 0);
        contentRect.anchorMax = new Vector2(1, 1);
        contentRect.sizeDelta = Vector2.zero;

        // 创建回合指示器
        GameObject turnIndicator = new GameObject("TurnIndicator");
        turnIndicator.transform.SetParent(contentRoot.transform, false);
        
        RectTransform turnRect = turnIndicator.AddComponent<RectTransform>();
        turnRect.anchorMin = new Vector2(0.5f, 1f);
        turnRect.anchorMax = new Vector2(0.5f, 1f);
        turnRect.pivot = new Vector2(0.5f, 1f);
        turnRect.anchoredPosition = new Vector2(0, -20);
        turnRect.sizeDelta = new Vector2(300, 50);

        TextMeshProUGUI turnText = turnIndicator.AddComponent<TextMeshProUGUI>();
        turnText.text = "Hero的回合";
        turnText.fontSize = 24;
        turnText.alignment = TextAlignmentOptions.Center;
        turnText.color = Color.white;

        // 添加阴影
        Shadow turnShadow = turnIndicator.AddComponent<Shadow>();
        turnShadow.effectColor = Color.black;
        turnShadow.effectDistance = new Vector2(2, -2);

        // 创建战斗日志容器
        GameObject logContainer = new GameObject("BattleLogContainer");
        logContainer.transform.SetParent(contentRoot.transform, false);

        RectTransform logContainerRect = logContainer.AddComponent<RectTransform>();
        logContainerRect.anchorMin = new Vector2(1f, 0f);
        logContainerRect.anchorMax = new Vector2(1f, 1f);
        logContainerRect.pivot = new Vector2(1f, 0.5f);
        logContainerRect.anchoredPosition = new Vector2(-20, 0);
        logContainerRect.sizeDelta = new Vector2(300, 0);

        // 添加背景
        Image logBg = logContainer.AddComponent<Image>();
        logBg.color = new Color(0, 0, 0, 0.5f);

        // 添加Vertical Layout Group
        VerticalLayoutGroup logLayout = logContainer.AddComponent<VerticalLayoutGroup>();
        logLayout.padding = new RectOffset(10, 10, 10, 10);
        logLayout.spacing = 5;
        logLayout.childAlignment = TextAnchor.UpperLeft;
        logLayout.childControlWidth = true;
        logLayout.childControlHeight = false;
        logLayout.childForceExpandWidth = true;
        logLayout.childForceExpandHeight = false;

        // 添加Content Size Fitter
        ContentSizeFitter logFitter = logContainer.AddComponent<ContentSizeFitter>();
        logFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // 添加BattleLog组件
        BattleLog battleLog = logContainer.AddComponent<BattleLog>();

        // 设置BattleInfoPanel的引用
        SerializedObject serializedPanel = new SerializedObject(battlePanel);
        serializedPanel.FindProperty("panelRoot").objectReferenceValue = contentRoot;
        serializedPanel.FindProperty("turnIndicatorText").objectReferenceValue = turnText;
        serializedPanel.FindProperty("battleLog").objectReferenceValue = battleLog;
        serializedPanel.FindProperty("canvasGroup").objectReferenceValue = canvasGroup;
        serializedPanel.ApplyModifiedProperties();

        // 设置BattleLog的引用
        SerializedObject serializedLog = new SerializedObject(battleLog);
        serializedLog.FindProperty("logContainer").objectReferenceValue = logContainer.transform;
        serializedLog.FindProperty("maxEntries").intValue = 5;
        serializedLog.FindProperty("autoCreateEntry").boolValue = true;
        serializedLog.ApplyModifiedProperties();

        // 选中创建的对象
        Selection.activeGameObject = panelRoot;

        // 标记场景为脏
        EditorUtility.SetDirty(panelRoot);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
        );

        Debug.Log("<color=green>✓ BattleInfoPanel 创建成功！</color>");
        EditorUtility.DisplayDialog(
            "成功",
            "BattleInfoPanel已创建！\n\n请检查组件引用是否正确。",
            "确定"
        );
    }
}

