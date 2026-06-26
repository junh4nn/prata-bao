using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailModal : MonoBehaviour
{
    [SerializeField] private Image              cardGlowImage;
    [SerializeField] private Image              cardBorderImage;
    [SerializeField] private Image              itemImage;
    [SerializeField] private Sprite             placeholderSprite;
    [SerializeField] private Image              starImage1;
    [SerializeField] private Image              starImage2;
    [SerializeField] private Image              starImage3;
    [SerializeField] private TextMeshProUGUI    itemNameText;
    [SerializeField] private TextMeshProUGUI    scientificNameText;
    [SerializeField] private Image              rarityBadgeImage;
    [SerializeField] private Image              rarityBadgeBorderImage;
    [SerializeField] private TextMeshProUGUI    rarityBadgeText;
    [SerializeField] private TextMeshProUGUI    typeValueText;
    [SerializeField] private TextMeshProUGUI    firstObtainedValueText;
    [SerializeField] private TextMeshProUGUI    abilityText;
    [SerializeField] private Button             closeButton;

    static readonly Color ColTypeFlora = new Color(0.133f, 0.773f, 0.369f);
    static readonly Color ColTypeFauna = new Color(0.545f, 0.353f, 0.169f);

    const string PlaceholderAbilityText = "Placeholder ability text for this card.";

    void Start()
    {
        if (closeButton != null) closeButton.onClick.AddListener(Hide);
    }

    public void Show(InventoryItemRow row, ItemDefinition visuals)
    {
        gameObject.SetActive(true);

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

        if (itemNameText       != null) itemNameText.text       = row.name;
        if (scientificNameText != null) scientificNameText.text = visuals != null ? visuals.scientificName : "Unknown Specimen";

        if (rarityBadgeImage != null)
            rarityBadgeImage.color = new Color(0.063f, 0.133f, 0.082f, 0f); // dark fill hidden for now — bigger colored border shows through solid

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

        if (typeValueText != null)
        {
            typeValueText.text  = row.type;
            typeValueText.color = row.type switch
            {
                "Flora" => ColTypeFlora,
                "Fauna" => ColTypeFauna,
                _       => Color.white
            };
        }

        if (firstObtainedValueText != null)
        {
            DateTime obtainedDate = DateTime.Parse(row.firstObtainedAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            firstObtainedValueText.text = obtainedDate.ToString("d MMM yyyy", CultureInfo.InvariantCulture);
        }

        if (abilityText != null)
            abilityText.text = visuals != null && !string.IsNullOrEmpty(visuals.abilityText) ? visuals.abilityText : PlaceholderAbilityText;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
