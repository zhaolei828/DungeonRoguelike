using UnityEditor;
using UnityEngine;

/// <summary>
/// 强制Unity重新编译脚本
/// </summary>
public class ForceRecompile : MonoBehaviour
{
    [MenuItem("Tools/Force Recompile Scripts")]
    public static void RecompileScripts()
    {
        Debug.Log("强制重新编译所有脚本...");
        AssetDatabase.Refresh();
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        Debug.Log("重新编译请求已发送！");
    }
}

