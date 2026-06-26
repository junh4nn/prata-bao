using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GachaCardDisplay : MonoBehaviour
{
    [SerializeField] private GameObject        defaultCardState;
    [SerializeField] private GameObject        revealedCardState;
    [SerializeField] private Image             itemImage;
    [SerializeField] private Sprite            placeholderSprite;
    [SerializeField] private Image             starImage1;
    [SerializeField] private Image             starImage2;
    [SerializeField] private Image             starImage3;
    [SerializeField] private TextMeshProUGUI   itemNameText;
    [SerializeField] private TextMeshProUGUI   scientificNameText;
    [SerializeField] private Image             rarityBadgeImage;
    [SerializeField] private Image             rarityBadgeBorderImage;
    [SerializeField] private TextMeshProUGUI   rarityBadgeText;
    [SerializeField] private Image             cardBorderImage;
    [SerializeField] private Image             cardGlowImage;
    [SerializeField] private Image             typeIconImage;
    [SerializeField] private Image             typeIconBgImage;
    [SerializeField] private TypeVisuals       typeVisuals;

    static readonly Color ColTypeFlora    = new Color(0.133f, 0.773f, 0.369f);
    static readonly Color ColTypeFauna    = new Color(0.545f, 0.353f, 0.169f);

    public void Setup(string displayName, Rarity rarity, string typeName, ItemDefinition visuals)
    {
        if (defaultCardState  != null) defaultCardState.SetActive(false);
        if (revealedCardState != null) revealedCardState.SetActive(true);

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
            Sprite icon = typeVisuals.GetIcon(typeName);
            typeIconImage.sprite  = icon;
            typeIconImage.enabled = icon != null;
        }

        if (typeIconBgImage != null)
        {
            typeIconBgImage.color = typeName switch
            {
                "Flora" => ColTypeFlora,
                "Fauna" => ColTypeFauna,
                _       => Color.clear
            };
        }

        if (itemNameText       != null) itemNameText.text       = displayName;
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
    }

    public void ResetToDefault()
    {
        if (defaultCardState       != null) defaultCardState.SetActive(true);
        if (revealedCardState      != null) revealedCardState.SetActive(false);
        if (cardBorderImage        != null) cardBorderImage.gameObject.SetActive(false);
        if (rarityBadgeBorderImage != null) rarityBadgeBorderImage.gameObject.SetActive(false);
        if (cardGlowImage          != null) cardGlowImage.gameObject.SetActive(false);
    }
}
