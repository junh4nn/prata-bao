// InventoryCardBuilder.cs: builds the InventoryCard prefab used by InventoryLayoutBuilder.cs.
// The InventoryCardDisplay component wired here does the actual runtime rendering.

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class InventoryCardBuilder
{
    static TMP_FontAsset s_PoppinsSemiBold;
    static TMP_FontAsset s_PoppinsLight;
    static TMP_FontAsset s_CinzelSemiBold;
    static Sprite        s_UISprite;

    [MenuItem("GaiaGacha/ItemCard/Build Inventory Card", priority = 102)]
    static void Build()
    {
        s_PoppinsSemiBold = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Poppins-SemiBold SDF.asset");
        s_PoppinsLight    = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Poppins-Light SDF.asset");
        s_CinzelSemiBold  = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Cinzel-SemiBold SDF.asset");
        s_UISprite        = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        var temp   = new GameObject("Temp");
        var cardGo = BuildInventoryCard(temp.transform);

        PrefabUtility.SaveAsPrefabAsset(cardGo, "Assets/Prefabs/InventoryCard.prefab");
        Object.DestroyImmediate(temp);
        AssetDatabase.Refresh();
        Debug.Log("<color=green>[InventoryCardBuilder] InventoryCard prefab saved.</color>");
    }

    public static GameObject BuildInventoryCard(Transform parent)
    {
        // Root: InventoryCardDisplay lives here, sized to match one grid cell. The whole tile
        // is the click target, so the Button sits on the root too.
        var root = UIConstants.MakeRect(parent, "InventoryCard");
        UIConstants.SetAnchored(root, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(160, 220));
        var display = root.gameObject.AddComponent<InventoryCardDisplay>();
        var cardButton = root.gameObject.AddComponent<Button>();

        // CardGlow: bleeds beyond CardBorder, behind everything; shown for Legendary only
        var cardGlow = UIConstants.MakeImage(root.transform, "CardGlow", s_UISprite, UIConstants.ColGold);
        cardGlow.color = new Color(UIConstants.ColGold.r, UIConstants.ColGold.g, UIConstants.ColGold.b, 0.30f);
        cardGlow.type  = Image.Type.Sliced;
        UIConstants.Stretch(cardGlow.rectTransform);
        cardGlow.gameObject.SetActive(false);

        // CardBorder: coloured by rarity at runtime, sits behind CardFace as a peeking outline
        var cardBorder = UIConstants.MakeImage(root.transform, "CardBorder", s_UISprite, UIConstants.ColSurface);
        cardBorder.type = Image.Type.Sliced;
        UIConstants.SetAnchored(cardBorder.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(156, 216));
        cardBorder.rectTransform.anchoredPosition = Vector2.zero;
        cardBorder.gameObject.SetActive(false);

        // CardFace: the surface the player sees
        var cardFace = UIConstants.MakeImage(root.transform, "CardFace", s_UISprite, UIConstants.ColSurface);
        cardFace.type = Image.Type.Sliced;
        UIConstants.SetAnchored(cardFace.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(150, 210));
        cardFace.rectTransform.anchoredPosition = Vector2.zero;
        cardButton.targetGraphic = cardFace;

        // Type icon background: small rounded-square chip, coloured by type
        var typeIconBg = UIConstants.MakeImage(cardFace.transform, "TypeIconBg", s_UISprite, Color.white);
        typeIconBg.type = Image.Type.Sliced;
        UIConstants.SetAnchored(typeIconBg.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(23, 23));
        typeIconBg.rectTransform.anchoredPosition = new Vector2(16, -15);

        // Type icon: top-left corner of the card face
        var typeIcon = UIConstants.MakeImage(cardFace.transform, "TypeIcon", null, Color.white);
        typeIcon.preserveAspect = true;
        UIConstants.SetAnchored(typeIcon.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(15, 15));
        typeIcon.rectTransform.anchoredPosition = new Vector2(16, -15);

        // Count badge: top-right corner, hidden by InventoryCardDisplay.Setup() when count == 1
        var countBadgeBg = UIConstants.MakeImage(cardFace.transform, "CountBadge", s_UISprite, UIConstants.ColBg);
        countBadgeBg.type = Image.Type.Sliced;
        UIConstants.SetAnchored(countBadgeBg.rectTransform, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(34, 20));
        countBadgeBg.rectTransform.anchoredPosition = new Vector2(-12, -11);

        var countBadgeTmp = UIConstants.MakeTMP(countBadgeBg.transform, "CountText", "x1", 11.5f, UIConstants.ColTextPrimary, FontStyles.Bold, s_PoppinsSemiBold);
        UIConstants.SetAnchored(countBadgeTmp.rectTransform, Vector2.zero, Vector2.one, new Vector2(0, 0));
        countBadgeTmp.alignment = TextAlignmentOptions.Center;

        // Item sprite
        var itemImg = UIConstants.MakeImage(cardFace.transform, "ItemImage", null, Color.white);
        itemImg.preserveAspect = true;
        UIConstants.SetAnchored(itemImg.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(72, 72));
        itemImg.rectTransform.anchoredPosition = new Vector2(0, 44);

        // Item name
        var itemNameTmp = UIConstants.MakeTMP(cardFace.transform, "ItemNameText", "", 14, UIConstants.ColTextPrimary, FontStyles.Normal, s_CinzelSemiBold);
        UIConstants.SetAnchored(itemNameTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(122, 33));
        itemNameTmp.rectTransform.anchoredPosition = new Vector2(0, -10);
        itemNameTmp.alignment = TextAlignmentOptions.Bottom;
        itemNameTmp.textWrappingMode = TextWrappingModes.Normal;

        // Scientific name
        var sciNameTmp = UIConstants.MakeTMP(cardFace.transform, "ScientificNameText", "", 9.5f, UIConstants.ColTextMuted, FontStyles.Italic, s_PoppinsLight);
        UIConstants.SetAnchored(sciNameTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(122, 20));
        sciNameTmp.rectTransform.anchoredPosition = new Vector2(0, -32);
        sciNameTmp.alignment = TextAlignmentOptions.Center;

        // Stars row
        var starsRow = UIConstants.MakeRect(cardFace.transform, "StarsRow");
        UIConstants.SetAnchored(starsRow, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(57, 13));
        starsRow.anchoredPosition = new Vector2(0, -50);

        var star1 = UIConstants.MakeImage(starsRow.transform, "Star1", null, UIConstants.ColGold);
        UIConstants.SetAnchored(star1.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(10, 10));
        star1.rectTransform.anchoredPosition = new Vector2(-18, 0);
        star1.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        var star2 = UIConstants.MakeImage(starsRow.transform, "Star2", null, new Color(0.2f, 0.32f, 0.24f));
        UIConstants.SetAnchored(star2.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(10, 10));
        star2.rectTransform.anchoredPosition = new Vector2(0, 0);
        star2.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        var star3 = UIConstants.MakeImage(starsRow.transform, "Star3", null, new Color(0.2f, 0.32f, 0.24f));
        UIConstants.SetAnchored(star3.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(10, 10));
        star3.rectTransform.anchoredPosition = new Vector2(18, 0);
        star3.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        // Rarity badge border
        var rarityBadgeBorder = UIConstants.MakeImage(cardFace.transform, "RarityBadgeBorder", s_UISprite, UIConstants.ColTextMuted);
        rarityBadgeBorder.type = Image.Type.Sliced;
        UIConstants.SetAnchored(rarityBadgeBorder.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(96, 23));
        rarityBadgeBorder.rectTransform.anchoredPosition = new Vector2(0, -74);
        rarityBadgeBorder.gameObject.SetActive(false);

        // Rarity badge
        var rarityBadge = UIConstants.MakeImage(cardFace.transform, "RarityBadge", s_UISprite, UIConstants.ColInputBg);
        rarityBadge.type = Image.Type.Sliced;
        UIConstants.SetAnchored(rarityBadge.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(88, 19));
        rarityBadge.rectTransform.anchoredPosition = new Vector2(0, -74);

        var rarityTmp = UIConstants.MakeTMP(rarityBadge.transform, "RarityText", "COMMON", 9.5f, UIConstants.ColTextSecondary, FontStyles.Bold, s_PoppinsSemiBold);
        UIConstants.SetAnchored(rarityTmp.rectTransform, Vector2.zero, Vector2.one, new Vector2(0, 0));
        rarityTmp.alignment = TextAlignmentOptions.Center;

        // Wire InventoryCardDisplay SerializedFields from within the prefab
        var so = new SerializedObject(display);
        so.FindProperty("itemImage").objectReferenceValue              = itemImg;
        so.FindProperty("starImage1").objectReferenceValue             = star1;
        so.FindProperty("starImage2").objectReferenceValue             = star2;
        so.FindProperty("starImage3").objectReferenceValue             = star3;
        so.FindProperty("itemNameText").objectReferenceValue           = itemNameTmp;
        so.FindProperty("scientificNameText").objectReferenceValue     = sciNameTmp;
        so.FindProperty("rarityBadgeImage").objectReferenceValue       = rarityBadge;
        so.FindProperty("rarityBadgeBorderImage").objectReferenceValue = rarityBadgeBorder;
        so.FindProperty("rarityBadgeText").objectReferenceValue        = rarityTmp;
        so.FindProperty("cardBorderImage").objectReferenceValue        = cardBorder;
        so.FindProperty("cardGlowImage").objectReferenceValue          = cardGlow;
        so.FindProperty("typeIconImage").objectReferenceValue          = typeIcon;
        so.FindProperty("typeIconBgImage").objectReferenceValue        = typeIconBg;
        so.FindProperty("typeVisuals").objectReferenceValue            = AssetDatabase.LoadAssetAtPath<TypeVisuals>("Assets/ScriptableObjects/TypeVisuals.asset");
        so.FindProperty("countBadge").objectReferenceValue             = countBadgeBg.gameObject;
        so.FindProperty("countBadgeText").objectReferenceValue         = countBadgeTmp;
        so.FindProperty("cardButton").objectReferenceValue             = cardButton;
        so.ApplyModifiedProperties();

        return root.gameObject;
    }
}
