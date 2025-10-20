using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

/// <summary>
/// 修复Game场景的Main Camera标签
/// </summary>
public class FixGameSceneCameraTag
{
    [MenuItem("Tools/Dungeon/Fix Game Scene Camera Tag")]
    public static void FixCameraTag()
    {
        string gameScenePath = "Assets/_Project/Scenes/Game.unity/Game.unity";
        EditorSceneManager.OpenScene(gameScenePath, OpenSceneMode.Single);

        Camera mainCamera = GameObject.Find("Main Camera")?.GetComponent<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

        // 设置tag为MainCamera
        mainCamera.gameObject.tag = "MainCamera";
        
        // 设置相机为Orthographic
        mainCamera.orthographic = true;
        mainCamera.orthographicSize = 10f; // 调小一点，更容易看到Hero
        
        // 保存场景
        EditorUtility.SetDirty(mainCamera.gameObject);
        EditorSceneManager.SaveOpenScenes();

        Debug.Log($"✓ Main Camera tag设置为MainCamera，orthographicSize设置为10");
    }
}

