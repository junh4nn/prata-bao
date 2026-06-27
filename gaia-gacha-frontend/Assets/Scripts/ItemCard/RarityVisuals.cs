using UnityEngine;

public static class RarityVisuals
{
    public static readonly Color ColCommon    = new Color(0.659f, 0.710f, 0.635f);
    public static readonly Color ColRare      = new Color(0.322f, 0.718f, 0.533f);
    public static readonly Color ColLegendary = new Color(0.914f, 0.769f, 0.404f);
    public static readonly Color ColStarActive   = new Color(0.914f, 0.769f, 0.404f);
    public static readonly Color ColStarInactive = new Color(0.2f,   0.32f,  0.24f);

    public static Color GetColor(Rarity r) => r switch
    {
        Rarity.Legendary => ColLegendary,
        Rarity.Rare      => ColRare,
        _                => ColCommon
    };

    public static int GetStarCount(Rarity r) => r switch
    {
        Rarity.Legendary => 3,
        Rarity.Rare      => 2,
        _                => 1
    };
}
