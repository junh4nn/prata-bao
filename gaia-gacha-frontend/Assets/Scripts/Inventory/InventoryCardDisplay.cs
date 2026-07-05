// InventoryCardDisplay.cs: renders one inventory grid cell, showing rarity stars, colours,
// type icon, and a duplicate-count badge. Sibling to GachaCardDisplay.cs.

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryCardDisplay : MonoBehaviour
{
    [SerializeField] private Image             itemImage;
    [SerializeField] private Sprite             placeholderSprite;
    [SerializeField] private Image              starImage1;
    [SerializeField] private Image              starImage2;
    [SerializeField] private Image              starImage3;
    [SerializeField] private TextMeshProUGUI    itemNameText;
    [SerializeField] private TextMeshProUGUI    scientificNameText;
    [SerializeField] private Image              rarityBadgeImage;
    [SerializeField] private Image              rarityBadgeBorderImage;
    [SerializeField] private TextMeshProUGUI    rarityBadgeText;
    [SerializeField] private Image              cardBorderImage;
    [SerializeField] private Image              cardGlowImage;
    [SerializeField] private Image              typeIconImage;
    [SerializeField] private Image              typeIconBgImage;
    [SerializeField] private TypeVisuals        typeVisuals;
    [SerializeField] private GameObject         countBadge;
    [SerializeField] private TextMeshProUGUI    countBadgeText;
    [SerializeField] private Button             cardButton;

    static readonly Color ColTypeFlora = new Color(0.133f, 0.773f, 0.369f);
    static readonly Color ColTypeFauna = new Color(0.545f, 0.353f, 0.169f);

    public Action<InventoryItemRow, ItemDefinition> OnClicked;

    private InventoryItemRow row;
    private ItemDefinition   visuals;

    public void Setup(InventoryItemRow row, ItemDefinition visuals)
    {
        this.row     = row;
        this.visuals = visuals;

        Rarity rarity = Enum.Parse<Rarity>(row.rarity);
        Color rarityColor = RarityVisuals.GetColor(rarity);

        int starCount = RarityVisuals.GetStarCount(rarity);
        if (starImage1 != null) starImage1.color = starCount >= 1 ? RarityVisuals.ColStarActive : RarityVisuals.ColStarInactive;
        if (starImage2 != null) starImage2.color = starCount >= 2 ? RarityVisuals.ColStarActive : RarityVisuals.ColStarInactive;
        if (starImage3 != null) starImage3.color = starCount >= 3 ? RarityVisuals.ColStarActive : RarityVisuals.ColStarInactive;

        if (itemImage != null)
        {
            Sprite sprite = visuals != null ? visuals.itemSprite : placeholderSprite;
            itemImage.sprite  = sprite;
            itemImage.enabled = sprite != null;
        }

        if (typeIconImage != null)
        {
            Sprite icon = typeVisuals.GetIcon(row.type);
            typeIconImage.sprite  = icon;
            typeIconImage.enabled = icon != null;
        }

        if (typeIconBgImage != null)
        {
            typeIconBgImage.color = row.type switch
            {
                "Flora" => ColTypeFlora,
                "Fauna" => ColTypeFauna,
                _       => Color.clear
            };
        }

        if (itemNameText       != null) itemNameText.text       = row.name;
        if (scientificNameText != null) scientificNameText.text = visuals != null ? visuals.scientificName : "Unknown Specimen";

        if (rarityBadgeImage != null)
            rarityBadgeImage.color = new Color(0.063f, 0.133f, 0.082f, 0f); // kept transparent: the rarity-coloured border underneath provides the colour instead

        if (rarityBadgeBorderImage != null)
        {
            rarityBadgeBorderImage.gameObject.SetActive(true);
            rarityBadgeBorderImage.color = rarityColor;
        }

        if (rarityBadgeText != null)
            rarityBadgeText.text = rarity.ToString().ToUpper();

        if (cardBorderImage != null)
        {
            cardBorderImage.gameObject.SetActive(true);
            cardBorderImage.color = rarityColor;
        }

        if (cardGlowImage != null)
        {
            bool showGlow = rarity == Rarity.Legendary;
            cardGlowImage.gameObject.SetActive(showGlow);
            if (showGlow)
                cardGlowImage.color = new Color(RarityVisuals.ColLegendary.r, RarityVisuals.ColLegendary.g, RarityVisuals.ColLegendary.b, 0.30f);
        }

        if (countBadge != null)
            countBadge.SetActive(row.count > 1);

        if (countBadgeText != null)
            countBadgeText.text = $"x{row.count}";

        if (cardButton != null)
            cardButton.onClick.AddListener(() => OnClicked?.Invoke(this.row, this.visuals));
    }
}
