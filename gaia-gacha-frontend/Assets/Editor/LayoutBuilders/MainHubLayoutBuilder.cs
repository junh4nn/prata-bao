using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class MainHubLayoutBuilder
{
    static TMP_FontAsset s_Poppins;
    static TMP_FontAsset s_Cinzel;
    static Sprite s_PanelFlat;
    static Sprite s_GachaIcon;
    static Sprite s_QuizIcon;
    static Sprite s_InventoryIcon;
    static Sprite s_LogoutIcon;

    [MenuItem("GaiaGacha/LayoutBuilders/Build Hub Panel")]
    static void Build()
    {
        s_Poppins       = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Poppins-SemiBold SDF.asset");
        s_Cinzel        = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Cinzel-Regular SDF.asset");
        s_PanelFlat     = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/button_rectangle_depth_flat.png");
        s_GachaIcon     = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/gachatile.png");
        s_QuizIcon      = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/quiztile.png");
        s_InventoryIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/inventorytile.png");
        s_LogoutIcon    = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/log-out.png");

        var temp    = new GameObject("Temp");
        var panelGo = BuildHubPanel(temp.transform);

        PrefabUtility.SaveAsPrefabAsset(panelGo, "Assets/Prefabs/HubPanel.prefab");
        Object.DestroyImmediate(temp);
        AssetDatabase.Refresh();
        Debug.Log("<color=green>[MainHubLayoutBuilder] HubPanel prefab saved.</color>");
    }

    static GameObject BuildHubPanel(Transform parent)
    {
        var panel = UIConstants.MakeRect(parent, "HubPanel");
        UIConstants.Stretch(panel);

        // ── Header bar (matches gacha page) ──────────────────────────────────
        var header = UIConstants.MakeImage(panel.transform, "HeaderBar", null, UIConstants.ColSurface);
        UIConstants.SetAnchored(header.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0, 64));
        header.rectTransform.anchoredPosition = new Vector2(0, -32);

        var titleTmp = UIConstants.MakeTMP(header.transform, "TitleText", "GaiaGacha", 24, UIConstants.ColTextPrimary, FontStyles.Bold, s_Cinzel);
        UIConstants.SetAnchored(titleTmp.rectTransform, new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 40));
        titleTmp.rectTransform.anchoredPosition = new Vector2(20, 0);
        titleTmp.alignment = TextAlignmentOptions.MidlineLeft;

        var coinsTmp = UIConstants.MakeTMP(header.transform, "CoinsText", "Eco-Coins: 0", 16, UIConstants.ColGold, FontStyles.Bold, s_Cinzel);
        UIConstants.SetAnchored(coinsTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0, 40));
        coinsTmp.rectTransform.anchoredPosition = new Vector2(-20, 0);
        coinsTmp.alignment = TextAlignmentOptions.MidlineRight;

        // ── Hero card ─────────────────────────────────────────────────────────
        var heroCard = UIConstants.MakeImage(panel.transform, "HeroCard", s_PanelFlat, UIConstants.Hex("#0d2a1e"));
        heroCard.type = Image.Type.Sliced;
        UIConstants.SetAnchored(heroCard.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(350, 140));
        heroCard.rectTransform.anchoredPosition = new Vector2(0, -168);

        var welcomeTmp = UIConstants.MakeTMP(heroCard.transform, "WelcomeText", "Welcome back, Explorer", 15, UIConstants.ColTextPrimary, FontStyles.Normal, s_Cinzel);
        UIConstants.SetAnchored(welcomeTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(310, 28));
        welcomeTmp.rectTransform.anchoredPosition = new Vector2(0, -24);
        welcomeTmp.alignment = TextAlignmentOptions.Center;
        welcomeTmp.characterSpacing = 6;

        var quoteTmp = UIConstants.MakeTMP(heroCard.transform, "QuoteText", "", 10, UIConstants.ColTextMuted, FontStyles.Italic, s_Poppins);
        UIConstants.SetAnchored(quoteTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(310, 88));
        quoteTmp.rectTransform.anchoredPosition = new Vector2(0, -72);
        quoteTmp.alignment = TextAlignmentOptions.Center;
        quoteTmp.textWrappingMode = TextWrappingModes.Normal;
        quoteTmp.characterSpacing = 2;

        // ── Gacha tile (featured, full-width) ────────────────────────────────
        var gachaTile = BuildNavTile(panel.transform, "GachaTile", "Gacha Pull",
            "Spend 10 Eco-Coins · Discover a specimen", new Vector2(350, 180), UIConstants.ColGold, 20, s_GachaIcon);
        var gachaTileRt = gachaTile.GetComponent<RectTransform>();
        gachaTileRt.anchorMin = new Vector2(0.5f, 1f);
        gachaTileRt.anchorMax = new Vector2(0.5f, 1f);
        gachaTileRt.anchoredPosition = new Vector2(0, -342);

        // ── Bottom row ───────────────────────────────────────────────────────
        var bottomRow = UIConstants.MakeRect(panel.transform, "BottomRow");
        UIConstants.SetAnchored(bottomRow, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(350, 160));
        bottomRow.anchoredPosition = new Vector2(0, -542);

        var quizTile = BuildNavTile(bottomRow, "QuizTile", "Quiz",
            "Earn Eco-Coins", new Vector2(170, 160), UIConstants.Hex("#52b788"), 16, s_QuizIcon);
        quizTile.GetComponent<RectTransform>().anchoredPosition = new Vector2(-90, 0);

        var inventoryTile = BuildNavTile(bottomRow, "InventoryTile", "Inventory",
            "Your collection", new Vector2(170, 160), UIConstants.ColTextMuted, 16, s_InventoryIcon);
        inventoryTile.GetComponent<RectTransform>().anchoredPosition = new Vector2(90, 0);

        // ── Footer bar (logout) ───────────────────────────────────────────────
        var footer = UIConstants.MakeImage(panel.transform, "FooterBar", null, UIConstants.ColSurface);
        UIConstants.SetAnchored(footer.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0, 56));
        footer.rectTransform.anchoredPosition = new Vector2(0, 28);

        var logoutImg = UIConstants.MakeImage(footer.transform, "LogoutButton", s_LogoutIcon, UIConstants.ColTextMuted);
        logoutImg.preserveAspect = true;
        UIConstants.SetAnchored(logoutImg.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(24, 24));
        logoutImg.rectTransform.anchoredPosition = Vector2.zero;
        var logoutBtn = logoutImg.gameObject.AddComponent<Button>();
        logoutBtn.targetGraphic = logoutImg;

        return panel.gameObject;
    }

    // Builds a nav tile: surface background + Button + optional icon + title + subtitle.
    static GameObject BuildNavTile(Transform parent, string name,
        string title, string subtitle, Vector2 size, Color titleColor, float titleSize, Sprite icon = null)
    {
        var tileImg = UIConstants.MakeImage(parent, name, s_PanelFlat, UIConstants.ColSurface);
        tileImg.type = Image.Type.Sliced;
        UIConstants.SetAnchored(tileImg.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), size);
        var tileBtn = tileImg.gameObject.AddComponent<Button>();
        tileBtn.targetGraphic = tileImg;

        float iconSize  = size.y * 0.22f;
        float titleY    = icon != null ?  size.y * 0.02f : size.y * 0.06f;
        float subtitleY = icon != null ? -size.y * 0.24f : -size.y * 0.18f;

        // Icon (optional)
        if (icon != null)
        {
            var iconImg = UIConstants.MakeImage(tileImg.transform, "TileIcon", icon, titleColor);
            iconImg.preserveAspect = true;
            UIConstants.SetAnchored(iconImg.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(iconSize, iconSize));
            iconImg.rectTransform.anchoredPosition = new Vector2(0, size.y * 0.26f);
        }

        // Title
        var tileTitleTmp = UIConstants.MakeTMP(tileImg.transform, "TileTitle", title,
            titleSize, titleColor, FontStyles.Bold, s_Cinzel);
        UIConstants.SetAnchored(tileTitleTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(size.x - 16, 28));
        tileTitleTmp.rectTransform.anchoredPosition = new Vector2(0, titleY);
        tileTitleTmp.alignment = TextAlignmentOptions.Center;
        tileTitleTmp.characterSpacing = 10;

        // Subtitle
        var tileSubTmp = UIConstants.MakeTMP(tileImg.transform, "TileSubtitle", subtitle,
            size.x > 200 ? 11f : 10f, UIConstants.ColTextMuted, FontStyles.Normal, s_Poppins);
        UIConstants.SetAnchored(tileSubTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(size.x - 16, 20));
        tileSubTmp.rectTransform.anchoredPosition = new Vector2(0, subtitleY);
        tileSubTmp.alignment = TextAlignmentOptions.Center;
        tileSubTmp.textWrappingMode = TextWrappingModes.NoWrap;

        return tileImg.gameObject;
    }
}
