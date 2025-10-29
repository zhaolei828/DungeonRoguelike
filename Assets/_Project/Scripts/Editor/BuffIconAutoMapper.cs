using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

/// <summary>
/// Buff图标自动映射工具 - 根据SPD标准布局自动配置Buff图标
/// </summary>
public class BuffIconAutoMapper : EditorWindow
{
    private BuffIconConfig config;
    private Sprite[] buffSprites;
    private string buffsPath = "Assets/_Project/Art/UI/buffs.png";

    [MenuItem("Tools/SPD/Auto Map Buff Icons")]
    public static void ShowWindow()
    {
        GetWindow<BuffIconAutoMapper>("Buff图标自动映射");
    }

    private void OnGUI()
    {
        GUILayout.Label("Buff图标自动映射工具", EditorStyles.boldLabel);
        GUILayout.Space(10);

        EditorGUILayout.HelpBox(
            "此工具将根据SPD的标准布局自动配置Buff图标。\n\n" +
            "前提条件：\n" +
            "1. buffs.png已切割为8x8的Sprite\n" +
            "2. BuffIconConfig.asset已创建\n\n" +
            "点击按钮后，工具会自动为19种Buff类型分配对应的图标。",
            MessageType.Info
        );

        GUILayout.Space(10);

        // 显示配置文件
        EditorGUILayout.LabelField("配置文件", EditorStyles.boldLabel);
        config = EditorGUILayout.ObjectField("BuffIconConfig", config, typeof(BuffIconConfig), false) as BuffIconConfig;

        GUILayout.Space(5);

        // 显示buffs.png路径
        EditorGUILayout.LabelField("Buffs图集", EditorStyles.boldLabel);
        buffsPath = EditorGUILayout.TextField("路径", buffsPath);

        GUILayout.Space(10);

        // 自动查找按钮
        if (GUILayout.Button("自动查找配置和素材"))
        {
            AutoFindAssets();
        }

        GUILayout.Space(10);

        // 自动映射按钮
        GUI.enabled = config != null;
        if (GUILayout.Button("自动映射Buff图标", GUILayout.Height(40)))
        {
            AutoMapBuffIcons();
        }
        GUI.enabled = true;

        GUILayout.Space(10);

        // 显示映射预览
        if (config != null)
        {
            EditorGUILayout.LabelField("当前配置状态", EditorStyles.boldLabel);
            int mappedCount = 0;
            foreach (var mapping in config.buffIcons)
            {
                if (mapping.icon != null)
                    mappedCount++;
            }
            EditorGUILayout.LabelField($"已配置: {mappedCount} / {config.buffIcons.Count}");
        }
    }

    private void AutoFindAssets()
    {
        // 查找BuffIconConfig
        if (config == null)
        {
            string configPath = "Assets/_Project/Resources/BuffIconConfig.asset";
            config = AssetDatabase.LoadAssetAtPath<BuffIconConfig>(configPath);
            
            if (config != null)
            {
                Debug.Log($"<color=green>✓ 找到BuffIconConfig: {configPath}</color>");
            }
            else
            {
                Debug.LogWarning("未找到BuffIconConfig.asset，请先创建配置文件");
            }
        }

        // 查找buffs.png
        string[] possiblePaths = new string[]
        {
            "Assets/_Project/Art/UI/buffs.png",
            "Assets/_Project/Art/UI/SPD/buffs.png"
        };

        foreach (string path in possiblePaths)
        {
            if (File.Exists(path))
            {
                buffsPath = path;
                Debug.Log($"<color=green>✓ 找到buffs.png: {path}</color>");
                break;
            }
        }

        if (config != null && File.Exists(buffsPath))
        {
            EditorUtility.DisplayDialog("完成", "已找到配置文件和素材！\n可以点击\"自动映射Buff图标\"了。", "确定");
        }
        else
        {
            string message = "";
            if (config == null)
                message += "- 未找到BuffIconConfig.asset\n";
            if (!File.Exists(buffsPath))
                message += "- 未找到buffs.png\n";
            
            EditorUtility.DisplayDialog("警告", "缺少必要文件：\n" + message, "确定");
        }
    }

    private void AutoMapBuffIcons()
    {
        if (config == null)
        {
            EditorUtility.DisplayDialog("错误", "请先选择BuffIconConfig配置文件！", "确定");
            return;
        }

        if (!File.Exists(buffsPath))
        {
            EditorUtility.DisplayDialog("错误", $"未找到buffs.png文件：\n{buffsPath}", "确定");
            return;
        }

        // 加载所有Sprite
        Object[] allAssets = AssetDatabase.LoadAllAssetsAtPath(buffsPath);
        buffSprites = allAssets.Where(obj => obj is Sprite).Cast<Sprite>().ToArray();

        if (buffSprites.Length == 0)
        {
            EditorUtility.DisplayDialog(
                "错误",
                "buffs.png中没有找到Sprite！\n\n请先使用\"Tools > SPD > Slice Buffs Sprite\"切割图片。",
                "确定"
            );
            return;
        }

        Debug.Log($"<color=cyan>找到 {buffSprites.Length} 个Sprite</color>");

        // 根据SPD标准布局映射
        // SPD的buffs.png布局（8x8切割，128个图标，16列x8行）
        // 这里是根据SPD源码和实际观察的映射关系
        var mapping = new System.Collections.Generic.Dictionary<BuffType, int>
        {
            // 第一行：正面Buff
            { BuffType.Strength, 0 },      // 力量 - 红色拳头
            { BuffType.Shield, 1 },        // 护盾 - 蓝色盾牌
            { BuffType.Regeneration, 2 },  // 再生 - 绿色心形
            { BuffType.Haste, 3 },         // 加速 - 黄色闪电
            { BuffType.Agility, 4 },       // 敏捷 - 绿色羽毛
            { BuffType.Defense, 5 },       // 防御 - 土黄色盾牌
            { BuffType.Invisibility, 6 },  // 隐身 - 半透明幽灵
            
            // 第二行：负面Buff
            { BuffType.Poison, 16 },       // 毒 - 紫色毒液
            { BuffType.Weakness, 17 },     // 虚弱 - 灰色向下箭头
            { BuffType.Slow, 18 },         // 减速 - 蓝色龟壳
            { BuffType.Bleeding, 19 },     // 流血 - 深红色血滴
            { BuffType.Burning, 20 },      // 燃烧 - 橙色火焰
            { BuffType.Frozen, 21 },       // 冰冻 - 冰蓝色雪花
            { BuffType.Paralysis, 22 },    // 麻痹 - 黄色闪电
            
            // 第三行：控制Buff
            { BuffType.Blind, 32 },        // 失明 - 黑色眼睛
            { BuffType.Confusion, 33 },    // 混乱 - 紫色问号
            { BuffType.Sleep, 34 },        // 睡眠 - 深蓝色Z
            { BuffType.Charmed, 35 },      // 魅惑 - 粉色心形
            { BuffType.Terror, 36 },       // 恐惧 - 深紫色骷髅
        };

        // 应用映射
        int successCount = 0;
        int failCount = 0;

        foreach (var kvp in mapping)
        {
            BuffType buffType = kvp.Key;
            int spriteIndex = kvp.Value;

            // 查找对应的Sprite
            Sprite sprite = FindSpriteByIndex(spriteIndex);

            if (sprite != null)
            {
                // 在config中找到对应的映射条目
                var configMapping = config.buffIcons.Find(m => m.buffType == buffType);
                if (configMapping != null)
                {
                    configMapping.icon = sprite;
                    successCount++;
                    Debug.Log($"<color=green>✓ {buffType} → {sprite.name}</color>");
                }
            }
            else
            {
                failCount++;
                Debug.LogWarning($"未找到索引 {spriteIndex} 的Sprite（{buffType}）");
            }
        }

        // 保存配置
        EditorUtility.SetDirty(config);
        AssetDatabase.SaveAssets();

        // 显示结果
        string message = $"自动映射完成！\n\n" +
                        $"成功: {successCount} 个\n" +
                        $"失败: {failCount} 个\n" +
                        $"总共: {mapping.Count} 个Buff类型\n\n" +
                        $"配置已保存到BuffIconConfig.asset";

        EditorUtility.DisplayDialog("完成", message, "确定");

        Debug.Log($"<color=green>✓ Buff图标自动映射完成！成功: {successCount}, 失败: {failCount}</color>");

        // 选中配置文件
        Selection.activeObject = config;
        EditorGUIUtility.PingObject(config);
    }

    private Sprite FindSpriteByIndex(int index)
    {
        // 查找名为 buff_X 的Sprite
        string spriteName = $"buff_{index}";
        
        foreach (Sprite sprite in buffSprites)
        {
            if (sprite.name == spriteName)
            {
                return sprite;
            }
        }

        return null;
    }
}

