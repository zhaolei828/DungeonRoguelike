using UnityEngine;
using UnityEditor;

/// <summary>
/// 关卡调试器
/// 在Scene视图中可视化房间边界、连接和地形信息
/// </summary>
[CustomEditor(typeof(Level), true)]
public class LevelDebugger : Editor
{
    private bool showRoomBounds = true;
    private bool showRoomConnections = true;
    private bool showRoomLabels = true;
    private bool showTerrainHeatmap = false;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Debug Options", EditorStyles.boldLabel);
        
        showRoomBounds = EditorGUILayout.Toggle("Show Room Bounds", showRoomBounds);
        showRoomConnections = EditorGUILayout.Toggle("Show Connections", showRoomConnections);
        showRoomLabels = EditorGUILayout.Toggle("Show Room Labels", showRoomLabels);
        showTerrainHeatmap = EditorGUILayout.Toggle("Show Terrain Heatmap", showTerrainHeatmap);
        
        EditorGUILayout.Space();
        
        Level level = (Level)target;
        
        if (GUILayout.Button("Generate Level"))
        {
            level.Generate();
            SceneView.RepaintAll();
        }
        
        if (GUILayout.Button("Clear Level"))
        {
            // 触发清理
            SceneView.RepaintAll();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Level Info", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Size: {level.Width} x {level.Height}");
        EditorGUILayout.LabelField($"Depth: {level.Depth}");
        EditorGUILayout.LabelField($"Type: {level.LevelType}");
        EditorGUILayout.LabelField($"Generated: {level.IsGenerated}");
    }
    
    private void OnSceneGUI()
    {
        Level level = (Level)target;
        
        if (!level.IsGenerated)
            return;
        
        // 绘制房间边界
        if (showRoomBounds)
        {
            DrawRoomBounds(level);
        }
        
        // 绘制房间连接
        if (showRoomConnections)
        {
            DrawRoomConnections(level);
        }
        
        // 绘制房间标签
        if (showRoomLabels)
        {
            DrawRoomLabels(level);
        }
        
        // 绘制地形热力图
        if (showTerrainHeatmap)
        {
            DrawTerrainHeatmap(level);
        }
    }
    
    private void DrawRoomBounds(Level level)
    {
        // 这里需要访问Level的rooms列表
        // 由于rooms是protected，我们暂时跳过
        // 实际实现时需要在Level类中添加public的Rooms属性
        Handles.color = Color.green;
        
        // 示例：绘制入口和出口
        Vector2Int entrance = level.EntrancePos;
        Vector2Int exit = level.ExitPos;
        
        // 绘制入口
        Vector3 entranceWorld = new Vector3(entrance.x, entrance.y, 0);
        Handles.DrawWireCube(entranceWorld, Vector3.one * 2);
        Handles.Label(entranceWorld + Vector3.up, "Entrance", EditorStyles.whiteLargeLabel);
        
        // 绘制出口
        Vector3 exitWorld = new Vector3(exit.x, exit.y, 0);
        Handles.color = Color.red;
        Handles.DrawWireCube(exitWorld, Vector3.one * 2);
        Handles.Label(exitWorld + Vector3.up, "Exit", EditorStyles.whiteLargeLabel);
    }
    
    private void DrawRoomConnections(Level level)
    {
        Handles.color = Color.yellow;
        
        // 绘制入口到出口的连线
        Vector3 entranceWorld = new Vector3(level.EntrancePos.x, level.EntrancePos.y, 0);
        Vector3 exitWorld = new Vector3(level.ExitPos.x, level.ExitPos.y, 0);
        Handles.DrawLine(entranceWorld, exitWorld);
    }
    
    private void DrawRoomLabels(Level level)
    {
        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.normal.textColor = Color.white;
        labelStyle.fontSize = 10;
        
        // 绘制关卡信息
        Vector3 centerWorld = new Vector3(level.Width / 2f, level.Height / 2f, 0);
        Handles.Label(centerWorld, $"Level {level.Depth}\n{level.LevelType}", EditorStyles.whiteLargeLabel);
    }
    
    private void DrawTerrainHeatmap(Level level)
    {
        // 简化版热力图：不同地形用不同颜色
        for (int x = 0; x < level.Width; x += 2) // 每隔2格采样，减少绘制量
        {
            for (int y = 0; y < level.Height; y += 2)
            {
                Terrain terrain = level.GetTerrain(x, y);
                Color color = GetColorForTerrain(terrain);
                color.a = 0.3f;
                
                Vector3 pos = new Vector3(x, y, 0);
                Handles.color = color;
                Handles.DrawSolidDisc(pos, Vector3.forward, 0.4f);
            }
        }
    }
    
    private Color GetColorForTerrain(Terrain terrain)
    {
        switch (terrain)
        {
            case Terrain.Wall: return Color.gray;
            case Terrain.Floor: return Color.white;
            case Terrain.Water: return Color.blue;
            case Terrain.Grass:
            case Terrain.HighGrass: return Color.green;
            case Terrain.Entrance: return Color.cyan;
            case Terrain.Exit: return Color.magenta;
            case Terrain.Trap: return Color.red;
            default: return Color.black;
        }
    }
}

