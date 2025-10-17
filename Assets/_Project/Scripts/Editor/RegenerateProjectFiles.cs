using UnityEditor;
using UnityEngine;

/// <summary>
/// 编辑器工具：重新生成项目文件
/// </summary>
public static class RegenerateProjectFiles
{
    [MenuItem("Tools/Regenerate Project Files")]
    public static void Regenerate()
    {
        Debug.Log("正在重新生成项目文件...");
        
        // 强制Unity同步项目文件
        AssetDatabase.Refresh();
        
        // 触发项目文件重新生成
        EditorApplication.ExecuteMenuItem("Assets/Open C# Project");
        
        Debug.Log("项目文件重新生成完成！");
    }
}

