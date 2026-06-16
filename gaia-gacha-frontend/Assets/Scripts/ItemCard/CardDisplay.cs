using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    [SerializeField] private GameObject        defaultCardState;
    [SerializeField] private GameObject        revealedCardState;
    [SerializeField] private Image             itemImage;
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

    static readonly Color ColCommon    = new Color(0.659f, 0.710f, 0.635f);
    static readonly Color ColRare      = new Color(0.322f, 0.718f, 0.533f);
    static readonly Color ColLegendary = new Color(0.914f, 0.769f, 0.404f);
    static readonly Color ColStarActive   = new Color(0.914f, 0.769f, 0.404f);
    static readonly Color ColStarInactive = new Color(0.2f,   0.32f,  0.24f);

    public void Setup(ItemDefinition item)
    {
        if (defaultCardState  != null) defaultCardState.SetActive(false);
        if (revealedCardState != null) revealedCardState.SetActive(true);

        Color rarityColor = item.rarity switch
        {
            Rarity.Legendary => ColLegendary,
            Rarity.Rare      => ColRare,
            _                => ColCommon
        };

        int starCount = item.rarity switch { Rarity.Legendary => 3, Rarity.Rare => 2, _ => 1 };
        if (starImage1 != null) starImage1.color = starCount >= 1 ? ColStarActive : ColStarInactive;
        if (starImage2 != null) starImage2.color = starCount >= 2 ? ColStarActive : ColStarInactive;
        if (starImage3 != null) starImage3.color = starCount >= 3 ? ColStarActive : ColStarInactive;

        if (itemImage != null)
        {
            itemImage.sprite  = item.sprite;
            itemImage.enabled = item.sprite != null;
        }

        if (itemNameText       != null) itemNameText.text       = item.displayName;
        if (scientificNameText != null) scientificNameText.text = item.scientificName;

        if (rarityBadgeImage != null)
            rarityBadgeImage.color = new Color(0.063f, 0.133f, 0.082f);

        if (rarityBadgeBorderImage != null)
        {
            rarityBadgeBorderImage.gameObject.SetActive(true);
            rarityBadgeBorderImage.color = rarityColor;
        }

        if (rarityBadgeText != null)
            rarityBadgeText.text = item.rarity.ToString().ToUpper();

        if (cardBorderImage != null)
        {
            cardBorderImage.gameObject.SetActive(true);
            cardBorderImage.color = rarityColor;
        }

        if (cardGlowImage != null)
        {
            bool showGlow = item.rarity == Rarity.Legendary;
            cardGlowImage.gameObject.SetActive(showGlow);
            if (showGlow)
                cardGlowImage.color = new Color(ColLegendary.r, ColLegendary.g, ColLegendary.b, 0.30f);
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
