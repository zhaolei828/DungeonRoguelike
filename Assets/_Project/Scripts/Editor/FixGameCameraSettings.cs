using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// 修复Game场景中Main Camera的设置
/// </summary>
public class FixGameCameraSettings
{
    [MenuItem("Tools/Dungeon/Fix Game Camera Settings")]
    static void FixCamera()
    {
        string gameScenePath = "Assets/_Project/Scenes/Game.unity/Game.unity";
        string currentScenePath = EditorSceneManager.GetActiveScene().path;
        
        // 打开Game场景
        var scene = EditorSceneManager.OpenScene(gameScenePath, OpenSceneMode.Single);
        
        // 等待一帧
        EditorApplication.delayCall += () =>
        {
            // 查找Main Camera
            Camera mainCamera = Object.FindFirstObjectByType<Camera>();
            if (mainCamera != null)
            {
                Debug.Log("找到Main Camera，修改设置...");
                
                // 设置为正交模式
                mainCamera.orthographic = true;
                mainCamera.orthographicSize = 25f;
                mainCamera.nearClipPlane = 0.1f;
                mainCamera.farClipPlane = 1000f;
                
                // 设置相机位置
                mainCamera.transform.position = new Vector3(14f, 14f, -10f);
                
                Debug.Log("✓ 相机已修复");
                Debug.Log($"  - Orthographic: {mainCamera.orthographic}");
                Debug.Log($"  - OrthographicSize: {mainCamera.orthographicSize}");
                Debug.Log($"  - Position: {mainCamera.transform.position}");
            }
            else
            {
                Debug.LogError("未找到Main Camera！");
            }
            
            // 标记为已修改并保存
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            
            // 返回原场景
            if (!string.IsNullOrEmpty(currentScenePath))
            {
                EditorApplication.delayCall += () =>
                {
                    EditorSceneManager.OpenScene(currentScenePath, OpenSceneMode.Single);
                };
            }
        };
    }
}
