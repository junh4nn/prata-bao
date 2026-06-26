using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class GachaCardBuilder
{
    static TMP_FontAsset s_PoppinsSemiBold;
    static TMP_FontAsset s_PoppinsLight;
    static TMP_FontAsset s_CinzelSemiBold;
    static Sprite        s_UISprite;

    [MenuItem("GaiaGacha/Gacha/Build Gacha Card", priority = 100)]
    static void Build()
    {
        s_PoppinsSemiBold = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Poppins-SemiBold SDF.asset");
        s_PoppinsLight    = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Poppins-Light SDF.asset");
        s_CinzelSemiBold  = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Cinzel-SemiBold SDF.asset");
        s_UISprite        = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        var temp   = new GameObject("Temp");
        var cardGo = BuildGachaCard(temp.transform);

        PrefabUtility.SaveAsPrefabAsset(cardGo, "Assets/Prefabs/GachaCard.prefab");
        Object.DestroyImmediate(temp);
        AssetDatabase.Refresh();
        Debug.Log("<color=green>[GachaCardBuilder] GachaCard prefab saved.</color>");
    }

    public static GameObject BuildGachaCard(Transform parent)
    {
        // Root — GachaCardDisplay lives here, sized to match the full card visual
        var root = UIConstants.MakeRect(parent, "GachaCard");
        UIConstants.SetAnchored(root, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(302, 402));
        root.anchoredPosition = new Vector2(0, -340);
        var display = root.gameObject.AddComponent<GachaCardDisplay>();

        // CardGlow — 8px bleed on each side, behind everything
        var cardGlow = UIConstants.MakeImage(root.transform, "CardGlow", s_UISprite, UIConstants.ColGold);
        cardGlow.color = new Color(UIConstants.ColGold.r, UIConstants.ColGold.g, UIConstants.ColGold.b, 0.30f);
        cardGlow.type  = Image.Type.Sliced;
        UIConstants.Stretch(cardGlow.rectTransform);
        cardGlow.gameObject.SetActive(false);

        // CardBorder — 9px border, sits between glow and face
        var cardBorder = UIConstants.MakeImage(root.transform, "CardBorder", s_UISprite, UIConstants.ColSurface);
        cardBorder.type = Image.Type.Sliced;
        UIConstants.SetAnchored(cardBorder.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(292, 392));
        cardBorder.rectTransform.anchoredPosition = Vector2.zero;
        cardBorder.gameObject.SetActive(false);

        // CardFace — the white rounded rectangle the player sees
        var cardFace = UIConstants.MakeImage(root.transform, "CardFace", s_UISprite, UIConstants.ColSurface);
        cardFace.type = Image.Type.Sliced;
        UIConstants.SetAnchored(cardFace.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(280, 380));
        cardFace.rectTransform.anchoredPosition = Vector2.zero;

        // DefaultState — "?" placeholder shown before any pull
        var defaultState = UIConstants.MakeRect(cardFace.transform, "DefaultState");
        UIConstants.Stretch(defaultState);

        var questionMark = UIConstants.MakeTMP(defaultState.transform, "QuestionMark", "?", 52, UIConstants.ColTextPrimary, FontStyles.Bold, s_PoppinsSemiBold);
        UIConstants.SetAnchored(questionMark.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(120, 70));
        questionMark.rectTransform.anchoredPosition = new Vector2(0, 10);
        questionMark.alignment = TextAlignmentOptions.Center;

        var readyTmp = UIConstants.MakeTMP(defaultState.transform, "ReadyText", "What will nature reveal?", 12, UIConstants.ColTextMuted, FontStyles.Italic, s_PoppinsSemiBold);
        UIConstants.SetAnchored(readyTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(220, 44));
        readyTmp.rectTransform.anchoredPosition = new Vector2(0, -40);
        readyTmp.alignment = TextAlignmentOptions.Center;
        readyTmp.textWrappingMode = TextWrappingModes.Normal;

        // RevealedState — populated by GachaCardDisplay.Setup()
        var revealedState = UIConstants.MakeRect(cardFace.transform, "RevealedState");
        UIConstants.Stretch(revealedState);
        revealedState.gameObject.SetActive(false);

        // Stars row
        var starsRow = UIConstants.MakeRect(revealedState.transform, "StarsRow");
        UIConstants.SetAnchored(starsRow, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(80, 20));
        starsRow.anchoredPosition = new Vector2(0, -90);

        var star1 = UIConstants.MakeImage(starsRow.transform, "Star1", null, UIConstants.ColGold);
        UIConstants.SetAnchored(star1.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(16, 16));
        star1.rectTransform.anchoredPosition = new Vector2(-28, 0);
        star1.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        var star2 = UIConstants.MakeImage(starsRow.transform, "Star2", null, new Color(0.2f, 0.32f, 0.24f));
        UIConstants.SetAnchored(star2.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(16, 16));
        star2.rectTransform.anchoredPosition = new Vector2(0, 0);
        star2.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        var star3 = UIConstants.MakeImage(starsRow.transform, "Star3", null, new Color(0.2f, 0.32f, 0.24f));
        UIConstants.SetAnchored(star3.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(16, 16));
        star3.rectTransform.anchoredPosition = new Vector2(28, 0);
        star3.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        // Item sprite
        var itemImg = UIConstants.MakeImage(revealedState.transform, "ItemImage", null, Color.white);
        itemImg.preserveAspect = true;
        UIConstants.SetAnchored(itemImg.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(140, 140));
        itemImg.rectTransform.anchoredPosition = new Vector2(0, 75);

        // Item name
        var itemNameTmp = UIConstants.MakeTMP(revealedState.transform, "ItemNameText", "", 19, UIConstants.ColTextPrimary, FontStyles.Normal, s_CinzelSemiBold);
        UIConstants.SetAnchored(itemNameTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(240, 36));
        itemNameTmp.rectTransform.anchoredPosition = new Vector2(0, -30);
        itemNameTmp.alignment = TextAlignmentOptions.Center;

        // Scientific name
        var sciNameTmp = UIConstants.MakeTMP(revealedState.transform, "ScientificNameText", "", 13, UIConstants.ColTextMuted, FontStyles.Italic, s_PoppinsLight);
        UIConstants.SetAnchored(sciNameTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(240, 24));
        sciNameTmp.rectTransform.anchoredPosition = new Vector2(0, -55);
        sciNameTmp.alignment = TextAlignmentOptions.Center;

        // Rarity badge border
        var rarityBadgeBorder = UIConstants.MakeImage(revealedState.transform, "RarityBadgeBorder", s_UISprite, UIConstants.ColTextMuted);
        rarityBadgeBorder.type = Image.Type.Sliced;
        UIConstants.SetAnchored(rarityBadgeBorder.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(128, 38));
        rarityBadgeBorder.rectTransform.anchoredPosition = new Vector2(0, -148);
        rarityBadgeBorder.gameObject.SetActive(false);

        // Rarity badge
        var rarityBadge = UIConstants.MakeImage(revealedState.transform, "RarityBadge", s_UISprite, UIConstants.ColInputBg);
        rarityBadge.type = Image.Type.Sliced;
        UIConstants.SetAnchored(rarityBadge.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(120, 32));
        rarityBadge.rectTransform.anchoredPosition = new Vector2(0, -148);

        var rarityTmp = UIConstants.MakeTMP(rarityBadge.transform, "RarityText", "COMMON", 12, UIConstants.ColTextPrimary, FontStyles.Bold, s_PoppinsSemiBold);
        UIConstants.SetAnchored(rarityTmp.rectTransform, Vector2.zero, Vector2.one, new Vector2(0, 0));
        rarityTmp.alignment = TextAlignmentOptions.Center;

        // Wire GachaCardDisplay SerializedFields from within the prefab
        var so = new UnityEditor.SerializedObject(display);
        so.FindProperty("defaultCardState").objectReferenceValue   = defaultState.gameObject;
        so.FindProperty("revealedCardState").objectReferenceValue  = revealedState.gameObject;
        so.FindProperty("itemImage").objectReferenceValue          = itemImg;
        so.FindProperty("starImage1").objectReferenceValue         = star1;
        so.FindProperty("starImage2").objectReferenceValue         = star2;
        so.FindProperty("starImage3").objectReferenceValue         = star3;
        so.FindProperty("itemNameText").objectReferenceValue       = itemNameTmp;
        so.FindProperty("scientificNameText").objectReferenceValue = sciNameTmp;
        so.FindProperty("rarityBadgeImage").objectReferenceValue       = rarityBadge;
        so.FindProperty("rarityBadgeBorderImage").objectReferenceValue = rarityBadgeBorder;
        so.FindProperty("rarityBadgeText").objectReferenceValue        = rarityTmp;
        so.FindProperty("cardBorderImage").objectReferenceValue  = cardBorder;
        so.FindProperty("cardGlowImage").objectReferenceValue    = cardGlow;
        so.ApplyModifiedProperties();

        return root.gameObject;
    }
}
