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
        public string spritePath;
    }

    static readonly ItemData[] Items = new[]
    {
        new ItemData { id = 1, displayName = "Mangrove Seed",          scientificName = "Rhizophora mangle",    spritePath = "Assets/Sprites/item_mangrove_seed.png"   },
        new ItemData { id = 2, displayName = "Coral Fragment",         scientificName = "Acropora cervicornis", spritePath = "Assets/Sprites/item_coral_fragment.png"  },
        new ItemData { id = 3, displayName = "Giant Sea Turtle Shell", scientificName = "Chelonia mydas",       spritePath = "Assets/Sprites/item_sea_turtle.png"      },
    };

    [MenuItem("GaiaGacha/ItemCard/Create Item Assets", priority = 10)]
    static void Create()
    {
        EnsureFolder("Assets/ScriptableObjects");
        EnsureFolder("Assets/ScriptableObjects/Items");

        var existingById = new Dictionary<int, ItemDefinition>();
        foreach (var guid in AssetDatabase.FindAssets("t:ItemDefinition", new[] { "Assets/ScriptableObjects/Items" }))
        {
            var existing = AssetDatabase.LoadAssetAtPath<ItemDefinition>(AssetDatabase.GUIDToAssetPath(guid));
            if (existing != null) existingById[existing.id] = existing;
        }

        var definitions = new List<ItemDefinition>();

        foreach (var data in Items)
        {
            string desiredPath = $"Assets/ScriptableObjects/Items/{data.displayName}.asset";

            ItemDefinition def;
            if (existingById.TryGetValue(data.id, out def))
            {
                string currentPath = AssetDatabase.GetAssetPath(def);
                if (currentPath != desiredPath)
                    AssetDatabase.RenameAsset(currentPath, data.displayName);
            }
            else
            {
                def = CreateAssetAt<ItemDefinition>(desiredPath);
            }

            def.id             = data.id;
            def.scientificName = data.scientificName;
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
