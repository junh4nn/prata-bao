using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class InventoryLayoutBuilder
{
    static TMP_FontAsset s_PoppinsSemiBold;
    static TMP_FontAsset s_PoppinsLight;
    static TMP_FontAsset s_CinzelRegular;
    static TMP_FontAsset s_CinzelSemiBold;
    static Sprite        s_BackIcon;
    static Sprite        s_UISprite;

    // ── Inventory Panel ──────────────────────────────────────────────────────
    // Builds the collection-browsing screen reached from the Hub's Inventory tile.
    // Layout (top to bottom):
    //   HeaderBar         — "GaiaGacha" title + Eco-Coins balance, same as every other panel
    //   FilterRow         — category dropdown (left), sort dropdown + direction button (right)
    //   CollectedCountText— "COLLECTED · N CARDS"
    //   InventoryScrollRect — vertical-only scroll, GridLayoutGroup content, empty until
    //                         InventoryManager populates it at runtime
    //   FooterBar         — back button
    //   DetailModal       — dim backdrop + centered card, parented last so it draws on top;
    //                       SetActive(false) by default

    [MenuItem("GaiaGacha/LayoutBuilders/Build Inventory Panel", priority = 203)]
    static void Build()
    {
        s_PoppinsSemiBold = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Poppins-SemiBold SDF.asset");
        s_PoppinsLight    = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Poppins-Light SDF.asset");
        s_CinzelRegular   = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Cinzel-Regular SDF.asset");
        s_CinzelSemiBold  = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Cinzel-SemiBold SDF.asset");
        s_BackIcon        = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/back_button.png");
        s_UISprite        = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        var temp    = new GameObject("Temp");
        var panelGo = BuildInventoryPanel(temp.transform);

        PrefabUtility.SaveAsPrefabAsset(panelGo, "Assets/Prefabs/InventoryPanel.prefab");
        Object.DestroyImmediate(temp);
        AssetDatabase.Refresh();
        Debug.Log("<color=green>[InventoryLayoutBuilder] InventoryPanel prefab saved.</color>");
    }

    static GameObject BuildInventoryPanel(Transform parent)
    {
        var panel = UIConstants.MakeRect(parent, "InventoryPanel");
        UIConstants.Stretch(panel);

        // ── Header bar (matches every other panel) ──────────────────────────
        var header = UIConstants.MakeImage(panel.transform, "HeaderBar", null, UIConstants.ColSurface);
        UIConstants.SetAnchored(header.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0, 64));
        header.rectTransform.anchoredPosition = new Vector2(0, -32);

        var titleTmp = UIConstants.MakeTMP(header.transform, "TitleText", "GaiaGacha", 24, UIConstants.ColTextPrimary, FontStyles.Bold, s_CinzelRegular);
        UIConstants.SetAnchored(titleTmp.rectTransform, new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 40));
        titleTmp.rectTransform.anchoredPosition = new Vector2(20, 0);
        titleTmp.alignment = TextAlignmentOptions.MidlineLeft;

        var coinsTmp = UIConstants.MakeTMP(header.transform, "CoinsText", "Eco-Coins: --", 16, UIConstants.ColGold, FontStyles.Bold, s_CinzelRegular);
        UIConstants.SetAnchored(coinsTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0, 40));
        coinsTmp.rectTransform.anchoredPosition = new Vector2(-20, 0);
        coinsTmp.alignment = TextAlignmentOptions.MidlineRight;

        // ── Filter row — category dropdown (left), sort dropdown + direction (right) ──
        var filterRow = UIConstants.MakeRect(panel.transform, "FilterRow");
        UIConstants.SetAnchored(filterRow, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0, 44));
        filterRow.anchoredPosition = new Vector2(0, -90);

        var categoryDropdown = UIConstants.MakeDropdown(filterRow, "CategoryDropdown", new[] { "All categories", "Fauna", "Flora" }, s_PoppinsSemiBold);
        UIConstants.SetAnchored(categoryDropdown.GetComponent<RectTransform>(), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(150, 36));
        categoryDropdown.GetComponent<RectTransform>().anchoredPosition = new Vector2(91, 0);

        var sortDropdown = UIConstants.MakeDropdown(filterRow, "SortDropdown", new[] { "Rarity", "Alphabetical", "Date obtained" }, s_PoppinsSemiBold);
        UIConstants.SetAnchored(sortDropdown.GetComponent<RectTransform>(), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(160, 36));
        sortDropdown.GetComponent<RectTransform>().anchoredPosition = new Vector2(255, 0);

        var (sortDirGo, _) = UIConstants.MakeButton(filterRow, "SortDirectionButton", "▲", 16, s_PoppinsSemiBold, UIConstants.ColInputBg, UIConstants.ColTextPrimary);
        UIConstants.SetAnchored(sortDirGo.GetComponent<RectTransform>(), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(36, 36));
        sortDirGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(361, 0);

        // ── Collected count label ────────────────────────────────────────────
        var collectedTmp = UIConstants.MakeTMP(panel.transform, "CollectedCountText", "COLLECTED · 0 CARDS", 12, UIConstants.ColTextMuted, FontStyles.Bold, s_PoppinsSemiBold);
        collectedTmp.rectTransform.anchorMin = new Vector2(0f, 1f);
        collectedTmp.rectTransform.anchorMax = new Vector2(1f, 1f);
        collectedTmp.rectTransform.pivot     = new Vector2(0.5f, 1f);
        collectedTmp.rectTransform.sizeDelta = new Vector2(-32, 22);
        collectedTmp.rectTransform.anchoredPosition = new Vector2(0, -118);
        collectedTmp.alignment = TextAlignmentOptions.MidlineLeft;
        collectedTmp.characterSpacing = 2;

        // ── Scrollable grid — vertical-only, GridLayoutGroup auto-fills columns ──
        var scrollRt = UIConstants.MakeRect(panel.transform, "InventoryScrollRect");
        scrollRt.anchorMin = new Vector2(0f, 0f);
        scrollRt.anchorMax = new Vector2(1f, 1f);
        scrollRt.pivot     = new Vector2(0.5f, 0.5f);
        scrollRt.offsetMin = new Vector2(16, 64);
        scrollRt.offsetMax = new Vector2(-16, -148);
        var scrollRect = scrollRt.gameObject.AddComponent<ScrollRect>();
        scrollRect.horizontal   = false;
        scrollRect.vertical     = true;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;

        var viewport = UIConstants.MakeRect(scrollRt, "Viewport");
        UIConstants.Stretch(viewport);
        viewport.gameObject.AddComponent<RectMask2D>();

        var content = UIConstants.MakeRect(viewport, "Content");
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot     = new Vector2(0.5f, 1f);
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = new Vector2(0, 0);

        var grid = content.gameObject.AddComponent<GridLayoutGroup>();
        grid.cellSize        = new Vector2(160, 220);
        grid.spacing         = new Vector2(12, 16);
        grid.constraint       = GridLayoutGroup.Constraint.Flexible;
        grid.childAlignment   = TextAnchor.UpperCenter;
        grid.padding          = new RectOffset(0, 0, 6, 16);

        var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewport;
        scrollRect.content  = content;

        // ── Footer bar (back button, matches every other panel) ─────────────
        var footer = UIConstants.MakeImage(panel.transform, "FooterBar", null, UIConstants.ColSurface);
        UIConstants.SetAnchored(footer.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0, 56));
        footer.rectTransform.anchoredPosition = new Vector2(0, 28);

        var backTileRt = UIConstants.MakeRect(footer.transform, "BackButton");
        UIConstants.SetAnchored(backTileRt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(48, 40));
        backTileRt.anchoredPosition = new Vector2(0, 1);
        var backTileBg = backTileRt.gameObject.AddComponent<Image>();
        backTileBg.color = UIConstants.ColBg;
        var backTileBtn = backTileRt.gameObject.AddComponent<Button>();
        backTileBtn.targetGraphic = backTileBg;

        var backIconImg = UIConstants.MakeImage(backTileRt, "Icon", s_BackIcon, UIConstants.ColTextMuted);
        backIconImg.preserveAspect = true;
        UIConstants.SetAnchored(backIconImg.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(20, 20));
        backIconImg.rectTransform.anchoredPosition = Vector2.zero;

        // ── Detail modal — parented last so it draws on top; hidden by default ──
        BuildDetailModal(panel.transform);

        return panel.gameObject;
    }

    static void BuildDetailModal(Transform parent)
    {
        var modalRoot = UIConstants.MakeRect(parent, "DetailModal");
        UIConstants.Stretch(modalRoot);
        var detailModal = modalRoot.gameObject.AddComponent<ItemDetailModal>();
        modalRoot.gameObject.SetActive(false);

        var backdrop = UIConstants.MakeImage(modalRoot, "Backdrop", null, new Color(0f, 0f, 0f, 0.6f));
        UIConstants.Stretch(backdrop.rectTransform);

        // DetailCard — outer root for the layered card chrome, sized to fit all 3 info sections
        var detailCard = UIConstants.MakeRect(modalRoot, "DetailCard");
        UIConstants.SetAnchored(detailCard, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(322, 642));

        // CardGlow — bleeds beyond CardBorder, behind everything; shown for Legendary only
        var cardGlow = UIConstants.MakeImage(detailCard, "CardGlow", s_UISprite, UIConstants.ColGold);
        cardGlow.color = new Color(UIConstants.ColGold.r, UIConstants.ColGold.g, UIConstants.ColGold.b, 0.30f);
        cardGlow.type  = Image.Type.Sliced;
        UIConstants.Stretch(cardGlow.rectTransform);
        cardGlow.gameObject.SetActive(false);

        // CardBorder — colored by rarity at runtime, sits behind CardFace as a peeking outline
        var cardBorder = UIConstants.MakeImage(detailCard, "CardBorder", s_UISprite, UIConstants.ColSurface);
        cardBorder.type = Image.Type.Sliced;
        UIConstants.SetAnchored(cardBorder.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(312, 632));
        cardBorder.gameObject.SetActive(false);

        // CardFace — the surface the player sees, same look as the mini/pull card
        var cardFace = UIConstants.MakeImage(detailCard, "CardFace", s_UISprite, UIConstants.ColSurface);
        cardFace.type = Image.Type.Sliced;
        UIConstants.SetAnchored(cardFace.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(300, 620));

        var (closeBtnGo, _) = UIConstants.MakeButton(cardFace.transform, "CloseButton", "×", 18, s_PoppinsSemiBold, UIConstants.ColBg, UIConstants.ColTextPrimary);
        UIConstants.SetAnchored(closeBtnGo.GetComponent<RectTransform>(), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(32, 32));
        closeBtnGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(-18, -18);

        // ── Section 1 — normal card stuff: item sprite, name, stars, rarity badge ──────
        var itemImg = UIConstants.MakeImage(cardFace.transform, "ItemImage", null, Color.white);
        itemImg.preserveAspect = true;
        UIConstants.SetAnchored(itemImg.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(140, 140));
        itemImg.rectTransform.anchoredPosition = new Vector2(0, -92);

        var starsRow = UIConstants.MakeRect(cardFace.transform, "StarsRow");
        UIConstants.SetAnchored(starsRow, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(90, 18));
        starsRow.anchoredPosition = new Vector2(0, -180);

        var star1 = UIConstants.MakeImage(starsRow.transform, "Star1", null, UIConstants.ColGold);
        UIConstants.SetAnchored(star1.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(18, 18));
        star1.rectTransform.anchoredPosition = new Vector2(-26, 0);
        star1.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        var star2 = UIConstants.MakeImage(starsRow.transform, "Star2", null, new Color(0.2f, 0.32f, 0.24f));
        UIConstants.SetAnchored(star2.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(18, 18));
        star2.rectTransform.anchoredPosition = new Vector2(0, 0);
        star2.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        var star3 = UIConstants.MakeImage(starsRow.transform, "Star3", null, new Color(0.2f, 0.32f, 0.24f));
        UIConstants.SetAnchored(star3.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(18, 18));
        star3.rectTransform.anchoredPosition = new Vector2(26, 0);
        star3.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        var itemNameTmp = UIConstants.MakeTMP(cardFace.transform, "ItemNameText", "", 20, UIConstants.ColTextPrimary, FontStyles.Normal, s_CinzelSemiBold);
        UIConstants.SetAnchored(itemNameTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(240, 30));
        itemNameTmp.rectTransform.anchoredPosition = new Vector2(0, -216);
        itemNameTmp.alignment = TextAlignmentOptions.Center;

        var sciNameTmp = UIConstants.MakeTMP(cardFace.transform, "ScientificNameText", "", 13, UIConstants.ColTextMuted, FontStyles.Italic, s_PoppinsLight);
        UIConstants.SetAnchored(sciNameTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(240, 20));
        sciNameTmp.rectTransform.anchoredPosition = new Vector2(0, -244);
        sciNameTmp.alignment = TextAlignmentOptions.Center;

        var rarityBadgeBorder = UIConstants.MakeImage(cardFace.transform, "RarityBadgeBorder", s_UISprite, UIConstants.ColTextMuted);
        rarityBadgeBorder.type = Image.Type.Sliced;
        UIConstants.SetAnchored(rarityBadgeBorder.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(130, 36));
        rarityBadgeBorder.rectTransform.anchoredPosition = new Vector2(0, -282);
        rarityBadgeBorder.gameObject.SetActive(false);

        var rarityBadge = UIConstants.MakeImage(cardFace.transform, "RarityBadge", s_UISprite, UIConstants.ColInputBg);
        rarityBadge.type = Image.Type.Sliced;
        UIConstants.SetAnchored(rarityBadge.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(122, 30));
        rarityBadge.rectTransform.anchoredPosition = new Vector2(0, -282);

        var rarityTmp = UIConstants.MakeTMP(rarityBadge.transform, "RarityText", "COMMON", 14, UIConstants.ColTextSecondary, FontStyles.Bold, s_PoppinsSemiBold);
        UIConstants.SetAnchored(rarityTmp.rectTransform, Vector2.zero, Vector2.one, new Vector2(0, 0));
        rarityTmp.alignment = TextAlignmentOptions.Center;

        var divider1 = UIConstants.MakeImage(cardFace.transform, "Divider1", null, new Color(UIConstants.ColTextMuted.r, UIConstants.ColTextMuted.g, UIConstants.ColTextMuted.b, 0.25f));
        UIConstants.SetAnchored(divider1.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(252, 1.5f));
        divider1.rectTransform.anchoredPosition = new Vector2(0, -323);

        // ── Section 2 — type (colored text) and first-obtained date, label left / value right ──
        var typeLabelTmp = UIConstants.MakeTMP(cardFace.transform, "TypeLabelText", "Type", 13, UIConstants.ColTextMuted, FontStyles.Normal, s_PoppinsSemiBold);
        UIConstants.SetAnchored(typeLabelTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(120, 22));
        typeLabelTmp.rectTransform.anchoredPosition = new Vector2(-66, -350);
        typeLabelTmp.alignment = TextAlignmentOptions.MidlineLeft;

        var typeValueTmp = UIConstants.MakeTMP(cardFace.transform, "TypeValueText", "--", 14, UIConstants.ColTextPrimary, FontStyles.Bold, s_PoppinsSemiBold);
        UIConstants.SetAnchored(typeValueTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(120, 22));
        typeValueTmp.rectTransform.anchoredPosition = new Vector2(66, -350);
        typeValueTmp.alignment = TextAlignmentOptions.MidlineRight;

        var firstObtainedLabelTmp = UIConstants.MakeTMP(cardFace.transform, "FirstObtainedLabelText", "First obtained", 13, UIConstants.ColTextMuted, FontStyles.Normal, s_PoppinsSemiBold);
        UIConstants.SetAnchored(firstObtainedLabelTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(120, 22));
        firstObtainedLabelTmp.rectTransform.anchoredPosition = new Vector2(-66, -384);
        firstObtainedLabelTmp.alignment = TextAlignmentOptions.MidlineLeft;

        var firstObtainedValueTmp = UIConstants.MakeTMP(cardFace.transform, "FirstObtainedValueText", "--", 13, UIConstants.ColTextPrimary, FontStyles.Normal, s_PoppinsSemiBold);
        UIConstants.SetAnchored(firstObtainedValueTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(120, 22));
        firstObtainedValueTmp.rectTransform.anchoredPosition = new Vector2(66, -384);
        firstObtainedValueTmp.alignment = TextAlignmentOptions.MidlineRight;

        var divider2 = UIConstants.MakeImage(cardFace.transform, "Divider2", null, new Color(UIConstants.ColTextMuted.r, UIConstants.ColTextMuted.g, UIConstants.ColTextMuted.b, 0.25f));
        UIConstants.SetAnchored(divider2.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(252, 1.5f));
        divider2.rectTransform.anchoredPosition = new Vector2(0, -418);

        // ── Section 3 — ability caption + body text ────────────────────────────────────
        var abilityLabelTmp = UIConstants.MakeTMP(cardFace.transform, "AbilityLabelText", "ABILITY", 11, UIConstants.ColTextMuted, FontStyles.Bold, s_PoppinsSemiBold);
        UIConstants.SetAnchored(abilityLabelTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(252, 18));
        abilityLabelTmp.rectTransform.anchoredPosition = new Vector2(0, -440);
        abilityLabelTmp.alignment = TextAlignmentOptions.MidlineLeft;
        abilityLabelTmp.characterSpacing = 2;

        var abilityTmp = UIConstants.MakeTMP(cardFace.transform, "AbilityText", "", 12, UIConstants.ColTextPrimary, FontStyles.Normal, s_PoppinsLight);
        UIConstants.SetAnchored(abilityTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(252, 160));
        abilityTmp.rectTransform.anchoredPosition = new Vector2(0, -539);
        abilityTmp.alignment = TextAlignmentOptions.TopLeft;
        abilityTmp.textWrappingMode = TextWrappingModes.Normal;

        // Wire ItemDetailModal SerializedFields
        var so = new SerializedObject(detailModal);
        so.FindProperty("cardGlowImage").objectReferenceValue          = cardGlow;
        so.FindProperty("cardBorderImage").objectReferenceValue        = cardBorder;
        so.FindProperty("itemImage").objectReferenceValue              = itemImg;
        so.FindProperty("starImage1").objectReferenceValue             = star1;
        so.FindProperty("starImage2").objectReferenceValue             = star2;
        so.FindProperty("starImage3").objectReferenceValue             = star3;
        so.FindProperty("itemNameText").objectReferenceValue           = itemNameTmp;
        so.FindProperty("scientificNameText").objectReferenceValue     = sciNameTmp;
        so.FindProperty("rarityBadgeImage").objectReferenceValue       = rarityBadge;
        so.FindProperty("rarityBadgeBorderImage").objectReferenceValue = rarityBadgeBorder;
        so.FindProperty("rarityBadgeText").objectReferenceValue        = rarityTmp;
        so.FindProperty("typeValueText").objectReferenceValue          = typeValueTmp;
        so.FindProperty("firstObtainedValueText").objectReferenceValue = firstObtainedValueTmp;
        so.FindProperty("abilityText").objectReferenceValue            = abilityTmp;
        so.FindProperty("closeButton").objectReferenceValue            = closeBtnGo.GetComponent<Button>();
        so.ApplyModifiedProperties();
    }
}
