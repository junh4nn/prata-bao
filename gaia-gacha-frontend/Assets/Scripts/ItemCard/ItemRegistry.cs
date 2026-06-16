using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GaiaGacha/Item Registry")]
public class ItemRegistry : ScriptableObject
{
    public List<ItemDefinition> items;

    public ItemDefinition FindByName(string name) => items.Find(i => i.displayName == name);
    public ItemDefinition FindById(int id)         => items.Find(i => i.id == id);
}
