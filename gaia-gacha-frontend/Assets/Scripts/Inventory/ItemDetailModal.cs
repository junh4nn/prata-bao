using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemDetailModal : MonoBehaviour
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
    [SerializeField] private Image              typeIconImage;
    [SerializeField] private TextMeshProUGUI    typeText;
    [SerializeField] private TypeVisuals        typeVisuals;
    [SerializeField] private TextMeshProUGUI    firstObtainedText;
    [SerializeField] private TextMeshProUGUI    abilityText;
    [SerializeField] private Button             closeButton;

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

        if (typeIconImage != null)
        {
            Sprite icon = typeVisuals.GetIcon(row.type);
            typeIconImage.sprite  = icon;
            typeIconImage.enabled = icon != null;
        }

        if (typeText != null)
            typeText.text = $"Type: {row.type}";

        if (firstObtainedText != null)
        {
            DateTime obtainedDate = DateTime.Parse(row.firstObtainedAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            firstObtainedText.text = $"First obtained: {obtainedDate.ToString("d MMM yyyy", CultureInfo.InvariantCulture)}";
        }

        if (abilityText != null)
            abilityText.text = visuals != null && !string.IsNullOrEmpty(visuals.abilityText) ? visuals.abilityText : PlaceholderAbilityText;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
