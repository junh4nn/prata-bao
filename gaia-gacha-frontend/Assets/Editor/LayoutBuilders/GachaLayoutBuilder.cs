using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class GachaLayoutBuilder {

    static TMP_FontAsset s_PoppinsSemiBold;
    static TMP_FontAsset s_CinzelRegular;
    static Sprite        s_BackIcon;

    
     // ── Gacha Panel ──────────────────────────────────────────────────────────
    // Builds the main pull/collection screen shown after login.
    // Layout (top to bottom):
    //   HeaderBar   — "GaiaGacha" title + Eco-Coins balance
    //   BannerLabel — "NATURE'S COLLECTION" label
    //   GachaCard   — shows "?" before a pull, then the item with rarity diamonds after
    //   PullButton  — costs 10 Eco-Coins per pull
    //   StatusText  — feedback during/after a pull


    [MenuItem("GaiaGacha/LayoutBuilders/Build Gacha Panel", priority = 202)]
    static void Build()
    {
        s_PoppinsSemiBold = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Poppins-SemiBold SDF.asset");
        s_CinzelRegular   = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Cinzel-Regular SDF.asset");
        s_BackIcon        = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/back_button.png");

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
        var titleTmp = UIConstants.MakeTMP(header.transform, "TitleText", "GaiaGacha", 24, UIConstants.ColTextPrimary, FontStyles.Bold, s_CinzelRegular);
        UIConstants.SetAnchored(titleTmp.rectTransform, new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 40));
        titleTmp.rectTransform.anchoredPosition = new Vector2(20, 0);
        titleTmp.alignment = TextAlignmentOptions.MidlineLeft;

        // Eco-Coins balance on the right side of the header, in gold text.
        // GachaManager.cs updates this after each pull.
        var balanceTmp = UIConstants.MakeTMP(header.transform, "BalanceText", "Eco-Coins: --", 16, UIConstants.ColGold, FontStyles.Bold, s_CinzelRegular);
        UIConstants.SetAnchored(balanceTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0, 40));
        balanceTmp.rectTransform.anchoredPosition = new Vector2(-20, 0);
        balanceTmp.alignment = TextAlignmentOptions.MidlineRight;

        // Small decorative label above the item card.
        var bannerTmp = UIConstants.MakeTMP(panel.transform, "BannerLabel", "NATURE'S COLLECTION", 14, UIConstants.ColTextMuted, FontStyles.Normal, s_PoppinsSemiBold);
        UIConstants.SetAnchored(bannerTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 30));
        bannerTmp.rectTransform.anchoredPosition = new Vector2(0, -120);
        bannerTmp.alignment = TextAlignmentOptions.Center;
        bannerTmp.characterSpacing = 4;

        // Load the pre-built GachaCard prefab. Run "GaiaGacha/Gacha/Build Gacha Card" first.
        var gachaCardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GachaCard.prefab");
        if (gachaCardPrefab == null) { Debug.LogError("[GachaLayoutBuilder] GachaCard.prefab not found — run Build Gacha Card first."); return panel.gameObject; }
        PrefabUtility.InstantiatePrefab(gachaCardPrefab, panel.transform);

        // Pull button — costs 10 Eco-Coins. GachaManager.cs listens to its onClick event.
        var (pullBtnGo, _) = UIConstants.MakeButton(panel.transform, "PullButton", "Pull  ·  10 Eco-Coins", 20, s_PoppinsSemiBold);
        UIConstants.SetAnchored(pullBtnGo.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 62));
        pullBtnGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -590);

        // Status text — shows "Connecting to nature registry..." during a pull, errors on failure.
        var statusTmp = UIConstants.MakeTMP(panel.transform, "StatusText", "", 13, UIConstants.ColTextMuted, FontStyles.Normal, s_PoppinsSemiBold);
        UIConstants.SetAnchored(statusTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 32));
        statusTmp.rectTransform.anchoredPosition = new Vector2(0, -660);
        statusTmp.alignment = TextAlignmentOptions.Center;

        // Footer bar pinned to the bottom of the screen.
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

        return panel.gameObject;
    }
    }
