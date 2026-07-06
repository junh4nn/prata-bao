using UnityEngine;

public enum Rarity { Common, Rare, Legendary }

[CreateAssetMenu(menuName = "GaiaGacha/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    public int    id;
    public string scientificName;
    public Sprite itemSprite;
    public string abilityText; // shown in the inventory detail view; falls back to placeholder text if empty
}
