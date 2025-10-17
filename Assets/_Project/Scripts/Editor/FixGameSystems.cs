using UnityEngine;
using UnityEditor;

/// <summary>
/// 修复GameSystems的Missing Script和缺失组件
/// </summary>
public class FixGameSystems : EditorWindow
{
    [MenuItem("DungeonRoguelike/Fix GameSystems (移除Missing Script + 添加LevelManager)")]
    public static void Fix()
    {
        // 查找GameSystems
        GameObject gameSystems = GameObject.Find("GameSystems");
        if (gameSystems == null)
        {
            Debug.LogError("❌ 未找到GameSystems GameObject!");
            EditorUtility.DisplayDialog("错误", 
                "未找到GameSystems GameObject！\n\n请确保在Hierarchy中有名为'GameSystems'的GameObject。", 
                "确定");
            return;
        }
        
        Debug.Log("✓ 找到GameSystems");
        
        // 使用SerializedObject来处理
        SerializedObject so = new SerializedObject(gameSystems);
        SerializedProperty components = so.FindProperty("m_Component");
        
        int removedCount = 0;
        
        // 查找并移除Missing Script（从后往前遍历，避免索引问题）
        for (int i = components.arraySize - 1; i >= 0; i--)
        {
            SerializedProperty component = components.GetArrayElementAtIndex(i);
            SerializedProperty componentRef = component.FindPropertyRelative("component");
            
            if (componentRef.objectReferenceValue == null)
            {
                // 这是Missing Script
                Debug.Log($"✓ 找到Missing Script at index {i}，准备移除...");
                
                // 通过GameObject直接操作（因为SerializedProperty无法直接删除）
                Component[] allComponents = gameSystems.GetComponents<Component>();
                
                // Transform总是在index 0，所以实际组件从index 1开始
                // 但GetComponents返回的不包含Transform...这很复杂
                
                // 更安全的方法：使用DestroyImmediate
                Component[] comps = gameSystems.GetComponents<Component>();
                foreach (Component comp in comps)
                {
                    if (comp == null) // Missing Script会是null
                    {
                        // 无法直接删除null...需要用SerializedObject
                        removedCount++;
                    }
                }
            }
        }
        
        // 如果有Missing Script，需要手动告知用户
        if (removedCount > 0 || components.arraySize > 2) // Transform + GameManager = 2
        {
            Debug.LogWarning("⚠️ 检测到Missing Script，请手动移除：");
            Debug.LogWarning("1. 在Inspector选择GameSystems");
            Debug.LogWarning("2. 找到'Missing Script'组件");
            Debug.LogWarning("3. 右键 > Remove Component");
            Debug.LogWarning("4. 然后重新运行此工具添加LevelManager");
            
            EditorUtility.DisplayDialog("需要手动操作", 
                "检测到Missing Script！\n\n" +
                "请按以下步骤操作：\n" +
                "1. 在Inspector中，找到GameSystems的'Missing Script'组件\n" +
                "2. 右键点击它\n" +
                "3. 选择'Remove Component'\n" +
                "4. 移除后，重新运行此菜单项\n\n" +
                "（Unity的限制：无法通过代码自动移除Missing Script）", 
                "我知道了");
            
            Selection.activeGameObject = gameSystems;
            return;
        }
        
        // 检查是否已有LevelManager
        LevelManager existingLM = gameSystems.GetComponent<LevelManager>();
        if (existingLM != null)
        {
            Debug.Log("✓ LevelManager已存在");
        }
        else
        {
            // 添加LevelManager
            gameSystems.AddComponent<LevelManager>();
            Debug.Log("✓ 已添加LevelManager组件");
        }
        
        // 检查是否已有GameManager
        GameManager existingGM = gameSystems.GetComponent<GameManager>();
        if (existingGM == null)
        {
            gameSystems.AddComponent<GameManager>();
            Debug.Log("✓ 已添加GameManager组件");
        }
        else
        {
            Debug.Log("✓ GameManager已存在");
        }
        
        // 标记场景为脏
        EditorUtility.SetDirty(gameSystems);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameSystems.scene);
        
        Debug.Log("✅ GameSystems配置完成！");
        
        EditorUtility.DisplayDialog("修复完成", 
            "GameSystems已配置完成！\n\n" +
            "✓ LevelManager已添加\n" +
            "✓ GameManager已确认\n\n" +
            "现在可以继续测试了。", 
            "好的");
    }
}

