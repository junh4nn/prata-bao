using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GaiaGacha/Item Registry")]
public class ItemRegistry : ScriptableObject
{
    public List<ItemDefinition> items;

    public ItemDefinition FindById(int id) => items.Find(i => i.id == id);
}
