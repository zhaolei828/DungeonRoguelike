using UnityEditor;
using UnityEngine;

public class ForceRecompile : EditorWindow
{
    [MenuItem("Tools/Force Recompile")]
    static void Recompile()
    {
        Debug.Log("强制重新编译所有脚本...");
        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        Debug.Log("重新编译完成！");
    }
}

