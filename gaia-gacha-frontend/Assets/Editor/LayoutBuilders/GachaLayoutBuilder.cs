using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class GachaLayoutBuilder {

    static TMP_FontAsset s_Poppins;
    static TMP_FontAsset s_Cinzel;
    static Sprite        s_BackIcon;

    
     // ── Gacha Panel ──────────────────────────────────────────────────────────
    // Builds the main pull/collection screen shown after login.
    // Layout (top to bottom):
    //   HeaderBar   — "GaiaGacha" title + Eco-Coins balance
    //   BannerLabel — "NATURE'S COLLECTION" label
    //   ItemCard    — shows "?" before a pull, then the item with rarity diamonds after
    //   PullButton  — costs 10 Eco-Coins per pull
    //   StatusText  — feedback during/after a pull


    [MenuItem("GaiaGacha/LayoutBuilders/Build Gacha Panel")]
    static void Build()
    {
        s_Poppins  = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Poppins-SemiBold SDF.asset");
        s_Cinzel   = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Cinzel-Regular SDF.asset");
        s_BackIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/back_button.png");

        var temp   = new GameObject("Temp");
        var panelGo = BuildGachaPanel(temp.transform);

        PrefabUtility.SaveAsPrefabAsset(panelGo, "Assets/Prefabs/GachaPanel.prefab");
        Object.DestroyImmediate(temp);
        AssetDatabase.Refresh();
        Debug.Log("<color=green>[GachaLayoutBuilder] GachaPanel prefab saved.</color>");
    }

    
    static GameObject BuildGachaPanel(Transform parent)
    {
        // Gacha panel also fills the entire screen.
        var panel = UIConstants.MakeRect(parent, "GachaPanel");
        UIConstants.Stretch(panel);

        // Header bar pinned to the top of the screen.
        // anchorMin/Max of (0,1)→(1,1) means it stretches full width but only has a fixed height.
        var header = UIConstants.MakeImage(panel.transform, "HeaderBar", null, UIConstants.ColSurface);
        UIConstants.SetAnchored(header.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0, 64));
        header.rectTransform.anchoredPosition = new Vector2(0, -32);

        // Game title on the left side of the header, using the Cinzel decorative font.
        var titleTmp = UIConstants.MakeTMP(header.transform, "TitleText", "GaiaGacha", 24, UIConstants.ColTextPrimary, FontStyles.Bold, s_Cinzel);
        UIConstants.SetAnchored(titleTmp.rectTransform, new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 40));
        titleTmp.rectTransform.anchoredPosition = new Vector2(20, 0);
        titleTmp.alignment = TextAlignmentOptions.MidlineLeft;

        // Eco-Coins balance on the right side of the header, in gold text.
        // GachaManager.cs updates this after each pull.
        var balanceTmp = UIConstants.MakeTMP(header.transform, "BalanceText", "Eco-Coins: --", 16, UIConstants.ColGold, FontStyles.Bold, s_Cinzel);
        UIConstants.SetAnchored(balanceTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0, 40));
        balanceTmp.rectTransform.anchoredPosition = new Vector2(-20, 0);
        balanceTmp.alignment = TextAlignmentOptions.MidlineRight;

        // Small decorative label above the item card.
        var bannerTmp = UIConstants.MakeTMP(panel.transform, "BannerLabel", "NATURE'S COLLECTION", 14, UIConstants.ColTextMuted, FontStyles.Normal, s_Poppins);
        UIConstants.SetAnchored(bannerTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 30));
        bannerTmp.rectTransform.anchoredPosition = new Vector2(0, -260);
        bannerTmp.alignment = TextAlignmentOptions.Center;
        bannerTmp.characterSpacing = 4;

        // Item card — the main display area. Contains two child states:
        //   DefaultState  — shown before any pull ("?" placeholder)
        //   RevealedState — shown after a pull (item name, rarity, diamonds)
        var itemCard = UIConstants.MakeImage(panel.transform, "ItemCard", null, UIConstants.ColSurface);
        UIConstants.SetAnchored(itemCard.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 320));
        itemCard.rectTransform.anchoredPosition = new Vector2(0, -380);

        // Default state: fills the entire card and shows a "?" until the player pulls.
        var defaultState = UIConstants.MakeRect(itemCard.transform, "DefaultState");
        UIConstants.Stretch(defaultState);

        var questionMark = UIConstants.MakeTMP(defaultState.transform, "QuestionMark", "?", 52, UIConstants.ColTextPrimary, FontStyles.Bold, s_Poppins);
        UIConstants.SetAnchored(questionMark.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(120, 70));
        questionMark.rectTransform.anchoredPosition = new Vector2(0, 10);
        questionMark.alignment = TextAlignmentOptions.Center;

        var readyTmp = UIConstants.MakeTMP(defaultState.transform, "ReadyText", "What will nature reveal?", 12, UIConstants.ColTextMuted, FontStyles.Italic, s_Poppins);
        UIConstants.SetAnchored(readyTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(220, 44));
        readyTmp.rectTransform.anchoredPosition = new Vector2(0, -40);
        readyTmp.alignment = TextAlignmentOptions.Center;
        readyTmp.textWrappingMode = TextWrappingModes.Normal;

        // Revealed state: hidden at start. GachaManager.cs activates it after a successful pull.
        var revealedState = UIConstants.MakeRect(itemCard.transform, "RevealedState");
        UIConstants.Stretch(revealedState);
        revealedState.gameObject.SetActive(false);

        // Rarity indicator: three diamond shapes in a row.
        // Gold = earned rarity tier, dark green = unearned tier.
        // GachaManager.cs sets the colors based on Common/Rare/Legendary.
        // Squares rotated 45° become diamond shapes — avoids font glyph issues with ★.
        var starsRow = UIConstants.MakeRect(revealedState.transform, "StarsRow");
        UIConstants.SetAnchored(starsRow, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(80, 20));
        starsRow.anchoredPosition = new Vector2(0, -24);

        var star1 = UIConstants.MakeImage(starsRow.transform, "Star1", null, UIConstants.ColGold); // always gold (at least 1 rarity)
        UIConstants.SetAnchored(star1.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(16, 16));
        star1.rectTransform.anchoredPosition = new Vector2(-28, 0);
        star1.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        var star2 = UIConstants.MakeImage(starsRow.transform, "Star2", null, new Color(0.2f, 0.32f, 0.24f)); // dim until Rare+
        UIConstants.SetAnchored(star2.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(16, 16));
        star2.rectTransform.anchoredPosition = new Vector2(0, 0);
        star2.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        var star3 = UIConstants.MakeImage(starsRow.transform, "Star3", null, new Color(0.2f, 0.32f, 0.24f)); // dim until Legendary
        UIConstants.SetAnchored(star3.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(16, 16));
        star3.rectTransform.anchoredPosition = new Vector2(28, 0);
        star3.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        // Item sprite — swapped at runtime by GachaManager based on the pulled item name.
        var itemImg = UIConstants.MakeImage(revealedState.transform, "ItemImage", null, Color.white);
        itemImg.preserveAspect = true;
        UIConstants.SetAnchored(itemImg.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(120, 120));
        itemImg.rectTransform.anchoredPosition = new Vector2(0, 16);

        // Item name displayed in bold cream text after a pull.
        var itemNameTmp = UIConstants.MakeTMP(revealedState.transform, "ItemNameText", "", 18, UIConstants.ColTextPrimary, FontStyles.Bold, s_Poppins);
        UIConstants.SetAnchored(itemNameTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(240, 32));
        itemNameTmp.rectTransform.anchoredPosition = new Vector2(0, -80);
        itemNameTmp.alignment = TextAlignmentOptions.Center;

        // Rarity badge — pill-shaped background whose color is set by GachaManager.cs.
        var rarityBadge = UIConstants.MakeImage(revealedState.transform, "RarityBadge", null, UIConstants.Hex("#A8B5A2"));
        UIConstants.SetAnchored(rarityBadge.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(110, 28));
        rarityBadge.rectTransform.anchoredPosition = new Vector2(0, -120);

        // Text inside the rarity badge ("COMMON", "RARE", or "LEGENDARY").
        var rarityTmp = UIConstants.MakeTMP(rarityBadge.transform, "RarityText", "COMMON", 12, UIConstants.ColButtonText, FontStyles.Bold, s_Poppins);
        UIConstants.SetAnchored(rarityTmp.rectTransform, Vector2.zero, Vector2.one, new Vector2(0, 0));
        rarityTmp.alignment = TextAlignmentOptions.Center;

        // Pull button — costs 10 Eco-Coins. GachaManager.cs listens to its onClick event.
        var (pullBtnGo, _) = UIConstants.MakeButton(panel.transform, "PullButton", "Pull  ·  10 Eco-Coins", 20, s_Poppins);
        UIConstants.SetAnchored(pullBtnGo.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 62));
        pullBtnGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -580);

        // Status text — shows "Connecting to nature registry..." during a pull, errors on failure.
        var statusTmp = UIConstants.MakeTMP(panel.transform, "StatusText", "", 13, UIConstants.ColTextMuted, FontStyles.Normal, s_Poppins);
        UIConstants.SetAnchored(statusTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 32));
        statusTmp.rectTransform.anchoredPosition = new Vector2(0, -660);
        statusTmp.alignment = TextAlignmentOptions.Center;

        // Footer bar pinned to the bottom of the screen.
        var footer = UIConstants.MakeImage(panel.transform, "FooterBar", null, UIConstants.ColSurface);
        UIConstants.SetAnchored(footer.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0, 56));
        footer.rectTransform.anchoredPosition = new Vector2(0, 28);

        var backTileRt = UIConstants.MakeRect(footer.transform, "BackButton");
        UIConstants.SetAnchored(backTileRt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(48, 40));
        backTileRt.anchoredPosition = new Vector2(0, 2);
        var backTileBg = backTileRt.gameObject.AddComponent<Image>();
        backTileBg.color = UIConstants.ColBg;
        var backTileBtn = backTileRt.gameObject.AddComponent<Button>();
        backTileBtn.targetGraphic = backTileBg;

        var backIconImg = UIConstants.MakeImage(backTileRt, "Icon", s_BackIcon, UIConstants.ColTextMuted);
        backIconImg.preserveAspect = true;
        UIConstants.SetAnchored(backIconImg.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(20, 20));
        backIconImg.rectTransform.anchoredPosition = Vector2.zero;

        return panel.gameObject;
    }
    }
