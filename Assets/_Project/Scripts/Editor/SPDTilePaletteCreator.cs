using UnityEngine;
using UnityEditor;
using UnityEngine.Tilemaps;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// SPD Tile Palette自动创建工具
/// 自动从切割好的Sprite创建Tile Assets和Tile Palette
/// </summary>
public class SPDTilePaletteCreator : EditorWindow
{
    [MenuItem("DungeonRoguelike/SPD Tools/Create Tile Palettes")]
    public static void ShowWindow()
    {
        GetWindow<SPDTilePaletteCreator>("Tile Palette Creator");
    }
    
    private Vector2 scrollPos;
    
    private void OnGUI()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        
        GUILayout.Label("SPD Tile Palette自动创建工具", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        EditorGUILayout.HelpBox(
            "此工具会自动完成以下操作：\n" +
            "1. 从切割好的Sprite创建Tile Assets\n" +
            "2. 创建Tile Palette资源\n" +
            "3. 将Tile添加到Palette中\n\n" +
            "确保已运行 'Auto Slice Sprites' 工具！", 
            MessageType.Info);
        
        GUILayout.Space(10);
        
        if (GUILayout.Button("🎨 创建所有Tile Palette", GUILayout.Height(50)))
        {
            CreateAllPalettes();
        }
        
        GUILayout.Space(20);
        GUILayout.Label("单独创建:", EditorStyles.boldLabel);
        
        if (GUILayout.Button("创建 Sewers Palette", GUILayout.Height(30)))
        {
            CreatePalette("tiles_sewers", "SewersTilePalette");
        }
        
        if (GUILayout.Button("创建 Prison Palette", GUILayout.Height(30)))
        {
            CreatePalette("tiles_prison", "PrisonTilePalette");
        }
        
        if (GUILayout.Button("创建 Caves Palette", GUILayout.Height(30)))
        {
            CreatePalette("tiles_caves", "CavesTilePalette");
        }
        
        if (GUILayout.Button("创建 City Palette", GUILayout.Height(30)))
        {
            CreatePalette("tiles_city", "CityTilePalette");
        }
        
        if (GUILayout.Button("创建 Halls Palette", GUILayout.Height(30)))
        {
            CreatePalette("tiles_halls", "HallsTilePalette");
        }
        
        if (GUILayout.Button("创建 Features Palette", GUILayout.Height(30)))
        {
            CreatePalette("terrain_features", "FeaturesTilePalette");
        }
        
        EditorGUILayout.EndScrollView();
    }
    
    private void CreateAllPalettes()
    {
        Debug.Log("=== 开始创建所有Tile Palette ===");
        
        string[] configs = new string[]
        {
            "tiles_sewers|SewersTilePalette",
            "tiles_prison|PrisonTilePalette",
            "tiles_caves|CavesTilePalette",
            "tiles_city|CityTilePalette",
            "tiles_halls|HallsTilePalette",
            "terrain_features|FeaturesTilePalette"
        };
        
        int successCount = 0;
        foreach (string config in configs)
        {
            string[] parts = config.Split('|');
            if (CreatePalette(parts[0], parts[1]))
            {
                successCount++;
            }
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("完成", 
            $"成功创建 {successCount}/{configs.Length} 个Tile Palette！\n\n" +
            "查看位置: Assets/_Project/Art/Tiles/Palettes/", 
            "确定");
        
        Debug.Log($"=== 完成！成功创建 {successCount} 个Palette ===");
    }
    
    private bool CreatePalette(string textureName, string paletteName)
    {
        try
        {
            Debug.Log($"\n--- 创建 {paletteName} ---");
            
            // 1. 查找贴图
            string texturePath = $"Assets/_Project/Art/Tiles/Environment/{textureName}.png";
            Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            
            if (texture == null)
            {
                Debug.LogError($"✗ 找不到贴图: {texturePath}");
                return false;
            }
            
            // 2. 获取所有Sprite
            Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(texturePath);
            List<Sprite> spriteList = new List<Sprite>();
            
            foreach (Object obj in sprites)
            {
                if (obj is Sprite sprite)
                {
                    spriteList.Add(sprite);
                }
            }
            
            if (spriteList.Count == 0)
            {
                Debug.LogError($"✗ 贴图未切割！请先运行 'Auto Slice Sprites'");
                return false;
            }
            
            Debug.Log($"找到 {spriteList.Count} 个Sprite");
            
            // 3. 创建输出目录
            string tilesFolder = "Assets/_Project/Art/Tiles/TileAssets";
            string paletteFolder = "Assets/_Project/Art/Tiles/Palettes";
            
            if (!AssetDatabase.IsValidFolder(tilesFolder))
            {
                string parent = "Assets/_Project/Art/Tiles";
                AssetDatabase.CreateFolder(parent, "TileAssets");
            }
            
            if (!AssetDatabase.IsValidFolder(paletteFolder))
            {
                string parent = "Assets/_Project/Art/Tiles";
                AssetDatabase.CreateFolder(parent, "Palettes");
            }
            
            string tileSetFolder = $"{tilesFolder}/{paletteName}";
            if (!AssetDatabase.IsValidFolder(tileSetFolder))
            {
                AssetDatabase.CreateFolder(tilesFolder, paletteName);
            }
            
            // 4. 创建Tile Assets
            List<TileBase> tiles = new List<TileBase>();
            int tileCount = 0;
            
            foreach (Sprite sprite in spriteList)
            {
                string tilePath = $"{tileSetFolder}/{sprite.name}.asset";
                
                // 检查是否已存在
                Tile existingTile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
                if (existingTile != null)
                {
                    tiles.Add(existingTile);
                    continue;
                }
                
                // 创建新Tile
                Tile tile = ScriptableObject.CreateInstance<Tile>();
                tile.sprite = sprite;
                tile.colliderType = Tile.ColliderType.None;
                
                AssetDatabase.CreateAsset(tile, tilePath);
                tiles.Add(tile);
                tileCount++;
            }
            
            Debug.Log($"✓ 创建了 {tileCount} 个新Tile Assets");
            
            // 5. 创建Tile Palette (GameObject)
            string palettePath = $"{paletteFolder}/{paletteName}.prefab";
            
            // 删除旧的
            if (File.Exists(palettePath))
            {
                AssetDatabase.DeleteAsset(palettePath);
            }
            
            // 创建Palette GameObject
            GameObject paletteGO = new GameObject(paletteName);
            Grid grid = paletteGO.AddComponent<Grid>();
            grid.cellSize = new Vector3(1, 1, 0);
            
            GameObject tilemapGO = new GameObject("Layer1");
            tilemapGO.transform.SetParent(paletteGO.transform);
            
            Tilemap tilemap = tilemapGO.AddComponent<Tilemap>();
            TilemapRenderer renderer = tilemapGO.AddComponent<TilemapRenderer>();
            
            // 填充Tile到Tilemap（排列成网格）
            int tilesPerRow = 16; // 每行16个Tile
            int x = 0, y = 0;
            
            foreach (TileBase tile in tiles)
            {
                tilemap.SetTile(new Vector3Int(x, -y, 0), tile);
                
                x++;
                if (x >= tilesPerRow)
                {
                    x = 0;
                    y++;
                }
            }
            
            // 保存为Prefab
            PrefabUtility.SaveAsPrefabAsset(paletteGO, palettePath);
            GameObject.DestroyImmediate(paletteGO);
            
            Debug.Log($"✓ Palette已创建: {palettePath}");
            Debug.Log($"✓ {paletteName} 创建完成！");
            
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"✗ 创建 {paletteName} 失败: {e.Message}\n{e.StackTrace}");
            return false;
        }
    }
}

