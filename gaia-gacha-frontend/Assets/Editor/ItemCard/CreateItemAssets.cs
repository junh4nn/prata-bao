using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public static class CreateItemAssets
{
    struct ItemData
    {
        public int    id;
        public string displayName;
        public string scientificName;
        public Rarity rarity;
        public string spritePath;
    }

    static readonly ItemData[] Items = new[]
    {
        new ItemData { id = 1, displayName = "Mangrove Seed",          scientificName = "Rhizophora mangle",    rarity = Rarity.Common,    spritePath = "Assets/Sprites/item_mangrove_seed.png"   },
        new ItemData { id = 2, displayName = "Coral Fragment",         scientificName = "Acropora cervicornis", rarity = Rarity.Rare,      spritePath = "Assets/Sprites/item_coral_fragment.png"  },
        new ItemData { id = 3, displayName = "Giant Sea Turtle Shell", scientificName = "Chelonia mydas",       rarity = Rarity.Legendary, spritePath = "Assets/Sprites/item_sea_turtle.png"      },
    };

    [MenuItem("GaiaGacha/ItemCard/Create Item Assets", priority = 100)]
    static void Create()
    {
        EnsureFolder("Assets/ScriptableObjects");
        EnsureFolder("Assets/ScriptableObjects/Items");

        var definitions = new List<ItemDefinition>();

        foreach (var data in Items)
        {
            string path = $"Assets/ScriptableObjects/Items/{data.displayName}.asset";

            var def = AssetDatabase.LoadAssetAtPath<ItemDefinition>(path)
                   ?? CreateAssetAt<ItemDefinition>(path);

            def.id             = data.id;
            def.displayName    = data.displayName;
            def.scientificName = data.scientificName;
            def.rarity         = data.rarity;
            def.sprite         = AssetDatabase.LoadAssetAtPath<Sprite>(data.spritePath);

            EditorUtility.SetDirty(def);
            definitions.Add(def);
        }

        const string registryPath = "Assets/ScriptableObjects/ItemRegistry.asset";
        var registry = AssetDatabase.LoadAssetAtPath<ItemRegistry>(registryPath)
                    ?? CreateAssetAt<ItemRegistry>(registryPath);

        registry.items = definitions;
        EditorUtility.SetDirty(registry);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"<color=green>[CreateItemAssets] {definitions.Count} items written to ItemRegistry.</color>");
    }

    static void EnsureFolder(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            int slash = path.LastIndexOf('/');
            AssetDatabase.CreateFolder(path.Substring(0, slash), path.Substring(slash + 1));
        }
    }

    static T CreateAssetAt<T>(string path) where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        return asset;
    }
}
