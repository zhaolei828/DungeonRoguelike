using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// SPD资源导入工具 - 从Shattered Pixel Dungeon项目导入资源
/// </summary>
public class SPDResourceImporter : EditorWindow
{
    private string spdProjectPath = @"D:\Program Files\Unity\Hub\Project\shattered-pixel-dungeon";
    private bool importSprites = true;
    private bool importEffects = true;
    private bool importUI = true;
    private bool importSounds = false; // 音效文件较大，默认不导入
    
    private Vector2 scrollPosition;
    private string statusMessage = "";
    
    [MenuItem("Tools/DungeonRoguelike/SPD Resource Importer")]
    public static void ShowWindow()
    {
        GetWindow<SPDResourceImporter>("SPD资源导入器");
    }
    
    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        EditorGUILayout.LabelField("SPD资源导入工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        // SPD项目路径
        EditorGUILayout.LabelField("SPD项目路径:", EditorStyles.label);
        spdProjectPath = EditorGUILayout.TextField(spdProjectPath);
        
        if (GUILayout.Button("浏览文件夹"))
        {
            string path = EditorUtility.OpenFolderPanel("选择SPD项目文件夹", "", "");
            if (!string.IsNullOrEmpty(path))
            {
                spdProjectPath = path;
            }
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("选择要导入的资源:", EditorStyles.boldLabel);
        
        importSprites = EditorGUILayout.Toggle("导入Sprite（怪物、物品等）", importSprites);
        importEffects = EditorGUILayout.Toggle("导入特效", importEffects);
        importUI = EditorGUILayout.Toggle("导入UI素材", importUI);
        importSounds = EditorGUILayout.Toggle("导入音效（较大）", importSounds);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("开始导入", GUILayout.Height(40)))
        {
            ImportResources();
        }
        
        if (!string.IsNullOrEmpty(statusMessage))
        {
            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(statusMessage, MessageType.Info);
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    private void ImportResources()
    {
        string assetsPath = Path.Combine(spdProjectPath, "core", "src", "main", "assets");
        
        if (!Directory.Exists(assetsPath))
        {
            statusMessage = "错误: 找不到SPD资源文件夹！\n请检查路径是否正确。";
            EditorUtility.DisplayDialog("错误", statusMessage, "确定");
            return;
        }
        
        statusMessage = "开始导入资源...\n";
        int totalImported = 0;
        
        try
        {
            // 导入Sprites
            if (importSprites)
            {
                int count = ImportSprites(assetsPath);
                totalImported += count;
                statusMessage += $"✓ 导入 {count} 个Sprite文件\n";
            }
            
            // 导入Effects
            if (importEffects)
            {
                int count = ImportEffects(assetsPath);
                totalImported += count;
                statusMessage += $"✓ 导入 {count} 个特效文件\n";
            }
            
            // 导入UI
            if (importUI)
            {
                int count = ImportUI(assetsPath);
                totalImported += count;
                statusMessage += $"✓ 导入 {count} 个UI文件\n";
            }
            
            // 导入Sounds
            if (importSounds)
            {
                int count = ImportSounds(assetsPath);
                totalImported += count;
                statusMessage += $"✓ 导入 {count} 个音效文件\n";
            }
            
            statusMessage += $"\n总计导入 {totalImported} 个文件！";
            
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("完成", statusMessage, "确定");
        }
        catch (System.Exception e)
        {
            statusMessage = $"导入失败: {e.Message}";
            EditorUtility.DisplayDialog("错误", statusMessage, "确定");
        }
    }
    
    private int ImportSprites(string assetsPath)
    {
        string sourcePath = Path.Combine(assetsPath, "sprites");
        string targetPath = Path.Combine(Application.dataPath, "_Project", "Art", "Sprites", "SPD");
        
        return CopyDirectory(sourcePath, targetPath, "*.png");
    }
    
    private int ImportEffects(string assetsPath)
    {
        string sourcePath = Path.Combine(assetsPath, "effects");
        string targetPath = Path.Combine(Application.dataPath, "_Project", "Art", "Effects");
        
        return CopyDirectory(sourcePath, targetPath, "*.png");
    }
    
    private int ImportUI(string assetsPath)
    {
        string sourcePath = Path.Combine(assetsPath, "interfaces");
        string targetPath = Path.Combine(Application.dataPath, "_Project", "Art", "UI");
        
        return CopyDirectory(sourcePath, targetPath, "*.png");
    }
    
    private int ImportSounds(string assetsPath)
    {
        string sourcePath = Path.Combine(assetsPath, "sounds");
        string targetPath = Path.Combine(Application.dataPath, "_Project", "Audio", "Sounds");
        
        return CopyDirectory(sourcePath, targetPath, "*.mp3");
    }
    
    private int CopyDirectory(string sourcePath, string targetPath, string searchPattern)
    {
        if (!Directory.Exists(sourcePath))
        {
            Debug.LogWarning($"源路径不存在: {sourcePath}");
            return 0;
        }
        
        // 创建目标文件夹
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }
        
        // 复制文件
        string[] files = Directory.GetFiles(sourcePath, searchPattern, SearchOption.AllDirectories);
        int count = 0;
        
        foreach (string sourceFile in files)
        {
            string fileName = Path.GetFileName(sourceFile);
            string destFile = Path.Combine(targetPath, fileName);
            
            File.Copy(sourceFile, destFile, true);
            count++;
        }
        
        return count;
    }
}

