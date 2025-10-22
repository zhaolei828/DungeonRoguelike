using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/// <summary>
/// 地图连通性诊断工具
/// 用于可视化和调试地图连通性问题
/// </summary>
public class LevelConnectivityDiagnostics : EditorWindow
{
    private Level currentLevel;
    private ConnectivityValidation lastValidation;
    private bool[,] reachableMap;
    private Vector2 scrollPos;
    
    [MenuItem("DungeonRoguelike/Diagnostics/Level Connectivity")]
    public static void ShowWindow()
    {
        GetWindow<LevelConnectivityDiagnostics>("地图连通性诊断");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("地图连通性诊断工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // 查找当前关卡
        if (GUILayout.Button("查找当前关卡", GUILayout.Height(30)))
        {
            FindCurrentLevel();
        }
        
        if (currentLevel == null)
        {
            EditorGUILayout.HelpBox("未找到关卡！请确保场景中有Level组件。", MessageType.Warning);
            return;
        }
        
        EditorGUILayout.LabelField("当前关卡", currentLevel.name);
        EditorGUILayout.LabelField("尺寸", $"{currentLevel.Width} x {currentLevel.Height}");
        EditorGUILayout.LabelField("入口", currentLevel.EntrancePos.ToString());
        EditorGUILayout.LabelField("出口", currentLevel.ExitPos.ToString());
        
        EditorGUILayout.Space();
        
        // 验证按钮
        if (GUILayout.Button("验证连通性", GUILayout.Height(40)))
        {
            ValidateConnectivity();
        }
        
        EditorGUILayout.Space();
        
        // 显示验证结果
        if (lastValidation != null)
        {
            DrawValidationResults();
        }
        
        EditorGUILayout.Space();
        
        // 可视化按钮
        if (GUILayout.Button("在Scene视图中可视化", GUILayout.Height(30)))
        {
            VisualizeInScene();
        }
        
        // 修复按钮
        if (lastValidation != null && !lastValidation.isValid)
        {
            EditorGUILayout.Space();
            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("尝试自动修复", GUILayout.Height(40)))
            {
                AttemptAutoFix();
            }
            GUI.backgroundColor = Color.white;
        }
    }
    
    private void FindCurrentLevel()
    {
        currentLevel = Object.FindAnyObjectByType<Level>();
        
        if (currentLevel == null)
        {
            Debug.LogWarning("场景中未找到Level组件！");
        }
        else
        {
            Debug.Log($"找到关卡: {currentLevel.name}");
        }
    }
    
    private void ValidateConnectivity()
    {
        if (currentLevel == null)
        {
            Debug.LogError("没有当前关卡！");
            return;
        }
        
        Vector2Int entrance = currentLevel.EntrancePos;
        Vector2Int exit = currentLevel.ExitPos;
        
        Debug.Log($"开始验证连通性... 入口: {entrance}, 出口: {exit}");
        
        lastValidation = PathFinder.ValidateConnectivity(currentLevel, entrance, exit, 0.7f);
        
        Debug.Log(lastValidation.ToString());
        
        // 构建可达地图用于可视化
        reachableMap = PathFinder.FloodFill(currentLevel, entrance);
        
        Repaint();
    }
    
    private void DrawValidationResults()
    {
        EditorGUILayout.LabelField("验证结果", EditorStyles.boldLabel);
        
        // 状态
        GUIStyle statusStyle = new GUIStyle(EditorStyles.label);
        statusStyle.fontStyle = FontStyle.Bold;
        statusStyle.fontSize = 14;
        
        if (lastValidation.isValid)
        {
            statusStyle.normal.textColor = Color.green;
            EditorGUILayout.LabelField("✓ 连通性验证通过", statusStyle);
        }
        else
        {
            statusStyle.normal.textColor = Color.red;
            EditorGUILayout.LabelField("✗ 连通性验证失败", statusStyle);
        }
        
        EditorGUILayout.Space();
        
        // 详细信息
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(200));
        
        EditorGUILayout.LabelField("错误信息", lastValidation.errorMessage);
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("入口可通行", lastValidation.entrancePassable ? "✓" : "✗");
        EditorGUILayout.LabelField("出口可通行", lastValidation.exitPassable ? "✓" : "✗");
        EditorGUILayout.LabelField("入口到出口可达", lastValidation.entranceToExitReachable ? "✓" : "✗");
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("总可通行格子", lastValidation.totalPassableTiles.ToString());
        EditorGUILayout.LabelField("可达格子", lastValidation.reachableTiles.ToString());
        EditorGUILayout.LabelField("可达百分比", $"{lastValidation.reachablePercent:P}");
        EditorGUILayout.Space();
        
        EditorGUILayout.LabelField("孤立区域数", lastValidation.isolatedRegionCount.ToString());
        
        if (lastValidation.isolatedRegionCount > 1)
        {
            EditorGUILayout.LabelField("孤立区域详情:");
            for (int i = 0; i < lastValidation.isolatedRegions.Count; i++)
            {
                var region = lastValidation.isolatedRegions[i];
                EditorGUILayout.LabelField($"  区域 {i + 1}", $"{region.Count} 格");
            }
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    private void VisualizeInScene()
    {
        if (currentLevel == null || reachableMap == null)
        {
            Debug.LogWarning("请先验证连通性！");
            return;
        }
        
        Debug.Log("在Scene视图中可视化连通性...");
        
        // 创建一个临时的可视化GameObject
        GameObject visualizer = GameObject.Find("ConnectivityVisualizer");
        if (visualizer == null)
        {
            visualizer = new GameObject("ConnectivityVisualizer");
        }
        
        // 清理旧的子对象
        while (visualizer.transform.childCount > 0)
        {
            DestroyImmediate(visualizer.transform.GetChild(0).gameObject);
        }
        
        // 为每个不可达的格子创建一个红色方块
        for (int x = 0; x < currentLevel.Width; x++)
        {
            for (int y = 0; y < currentLevel.Height; y++)
            {
                if (currentLevel.IsPassable(x, y) && !reachableMap[x, y])
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = $"Unreachable_{x}_{y}";
                    cube.transform.SetParent(visualizer.transform);
                    cube.transform.position = new Vector3(x + 0.5f, y + 0.5f, -0.5f);
                    cube.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                    
                    // 设置红色材质
                    Renderer renderer = cube.GetComponent<Renderer>();
                    Material mat = new Material(Shader.Find("Standard"));
                    mat.color = new Color(1f, 0f, 0f, 0.7f);
                    renderer.material = mat;
                }
            }
        }
        
        // 标记入口（绿色）
        Vector2Int entrance = currentLevel.EntrancePos;
        GameObject entranceMark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        entranceMark.name = "Entrance";
        entranceMark.transform.SetParent(visualizer.transform);
        entranceMark.transform.position = new Vector3(entrance.x + 0.5f, entrance.y + 0.5f, -0.5f);
        entranceMark.transform.localScale = Vector3.one * 1.5f;
        Renderer entranceRenderer = entranceMark.GetComponent<Renderer>();
        Material entranceMat = new Material(Shader.Find("Standard"));
        entranceMat.color = Color.green;
        entranceRenderer.material = entranceMat;
        
        // 标记出口（蓝色）
        Vector2Int exit = currentLevel.ExitPos;
        GameObject exitMark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        exitMark.name = "Exit";
        exitMark.transform.SetParent(visualizer.transform);
        exitMark.transform.position = new Vector3(exit.x + 0.5f, exit.y + 0.5f, -0.5f);
        exitMark.transform.localScale = Vector3.one * 1.5f;
        Renderer exitRenderer = exitMark.GetComponent<Renderer>();
        Material exitMat = new Material(Shader.Find("Standard"));
        exitMat.color = Color.blue;
        exitRenderer.material = exitMat;
        
        Debug.Log($"<color=cyan>可视化完成！红色=不可达，绿色=入口，蓝色=出口</color>");
        
        // 聚焦到可视化对象
        Selection.activeGameObject = visualizer;
        SceneView.lastActiveSceneView?.FrameSelected();
    }
    
    private void AttemptAutoFix()
    {
        if (currentLevel == null || lastValidation == null)
        {
            Debug.LogError("没有可修复的数据！");
            return;
        }
        
        Debug.Log("<color=yellow>尝试自动修复连通性问题...</color>");
        
        // 如果入口和出口不连通，创建一条路径
        if (!lastValidation.entranceToExitReachable)
        {
            Debug.Log("连接入口和出口...");
            ConnectTwoPoints(currentLevel.EntrancePos, currentLevel.ExitPos);
        }
        
        // 如果有孤立区域，连接它们
        if (lastValidation.isolatedRegionCount > 1)
        {
            Debug.Log($"连接 {lastValidation.isolatedRegionCount} 个孤立区域...");
            ConnectIsolatedRegions(lastValidation.isolatedRegions);
        }
        
        // 重新验证
        ValidateConnectivity();
        
        Debug.Log("<color=green>自动修复完成！</color>");
    }
    
    private void ConnectTwoPoints(Vector2Int from, Vector2Int to)
    {
        Vector2Int current = from;
        
        // L型路径
        while (current.x != to.x)
        {
            currentLevel.SetTerrain(current, Terrain.Floor);
            current.x += current.x < to.x ? 1 : -1;
        }
        
        while (current.y != to.y)
        {
            currentLevel.SetTerrain(current, Terrain.Floor);
            current.y += current.y < to.y ? 1 : -1;
        }
        
        currentLevel.SetTerrain(to, Terrain.Floor);
        
        Debug.Log($"创建连接路径：{from} -> {to}");
    }
    
    private void ConnectIsolatedRegions(List<List<Vector2Int>> regions)
    {
        if (regions.Count <= 1)
            return;
        
        // 将最大的区域作为主区域
        List<Vector2Int> mainRegion = regions[0];
        foreach (var region in regions)
        {
            if (region.Count > mainRegion.Count)
                mainRegion = region;
        }
        
        // 将其他区域连接到主区域
        foreach (var region in regions)
        {
            if (region == mainRegion)
                continue;
            
            // 找到该区域中离主区域最近的点
            Vector2Int closestInRegion = region[0];
            Vector2Int closestInMain = mainRegion[0];
            float minDistance = float.MaxValue;
            
            foreach (var pointInRegion in region)
            {
                foreach (var pointInMain in mainRegion)
                {
                    float distance = Vector2Int.Distance(pointInRegion, pointInMain);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestInRegion = pointInRegion;
                        closestInMain = pointInMain;
                    }
                }
            }
            
            // 连接这两个点
            ConnectTwoPoints(closestInRegion, closestInMain);
        }
    }
}

