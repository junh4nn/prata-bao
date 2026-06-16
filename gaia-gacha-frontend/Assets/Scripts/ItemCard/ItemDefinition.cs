using UnityEngine;

public enum Rarity { Common, Rare, Legendary }

[CreateAssetMenu(menuName = "GaiaGacha/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    public int    id;
    public string displayName;
    public string scientificName;
    public Rarity rarity;
    public Sprite sprite;
}
