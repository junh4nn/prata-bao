using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using TMPro;

public static class SceneBuilder
{
    static readonly Color ColBg          = Hex("#081C15");
    static readonly Color ColSurface     = Hex("#1B4332");
    static readonly Color ColButton      = Hex("#D4A373");
    static readonly Color ColButtonText  = Hex("#1B4332");
    static readonly Color ColTextPrimary = Hex("#F1FAEE");
    static readonly Color ColTextMuted   = Hex("#95B8A0");
    static readonly Color ColGold        = Hex("#E9C46A");

    static TMP_FontAsset s_Poppins;
    static TMP_FontAsset s_Cinzel;

    [MenuItem("GaiaGacha/Build Scene")]
    static void Build()
    {
        s_Poppins = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Poppins-SemiBold SDF.asset");
        s_Cinzel  = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Cinzel-Regular SDF.asset");
        if (s_Poppins == null) Debug.LogWarning("[SceneBuilder] Poppins font not found at Assets/Fonts/Poppins-SemiBold SDF.asset");
        if (s_Cinzel  == null) Debug.LogWarning("[SceneBuilder] Cinzel font not found at Assets/Fonts/Cinzel-Regular SDF.asset");

        foreach (var c in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            Object.DestroyImmediate(c.gameObject);
        foreach (var e in Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None))
            Object.DestroyImmediate(e.gameObject);
        foreach (var a in Object.FindObjectsByType<AuthManager>(FindObjectsSortMode.None))
            Object.DestroyImmediate(a.gameObject);
        foreach (var g in Object.FindObjectsByType<GachaManager>(FindObjectsSortMode.None))
            Object.DestroyImmediate(g.gameObject);

        var esGo = new GameObject("EventSystem");
        esGo.AddComponent<EventSystem>();
        esGo.AddComponent<InputSystemUIInputModule>();

        var canvasGo = new GameObject("Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(390, 844);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        var bgImg = MakeImage(canvasGo.transform, "Background", ColBg);
        Stretch(bgImg.rectTransform);

        var authPanel  = BuildAuthPanel(canvasGo.transform);
        var gachaPanel = BuildGachaPanel(canvasGo.transform);
        gachaPanel.SetActive(false);

        var authMgrGo = new GameObject("_AuthManager");
        var authMgr   = authMgrGo.AddComponent<AuthManager>();
        var authUIMgr = authMgrGo.AddComponent<AuthUIManager>();

        var gachaMgrGo = new GameObject("_GachaManager");
        var gachaMgr   = gachaMgrGo.AddComponent<GachaManager>();

        var authSO = new SerializedObject(authUIMgr);
        authSO.FindProperty("authManager").objectReferenceValue        = authMgr;
        authSO.FindProperty("authPanel").objectReferenceValue          = authPanel;
        authSO.FindProperty("gachaPanel").objectReferenceValue         = gachaPanel;
        authSO.FindProperty("emailInputField").objectReferenceValue    = Find<TMP_InputField>(authPanel.transform, "FormCard/EmailInput");
        authSO.FindProperty("passwordInputField").objectReferenceValue = Find<TMP_InputField>(authPanel.transform, "FormCard/PasswordInput");
        authSO.FindProperty("actionButton").objectReferenceValue       = Find<Button>(authPanel.transform, "FormCard/ActionButton");
        authSO.FindProperty("actionButtonText").objectReferenceValue   = Find<TextMeshProUGUI>(authPanel.transform, "FormCard/ActionButton/Text");
        authSO.FindProperty("toggleModeButton").objectReferenceValue   = Find<Button>(authPanel.transform, "FormCard/ToggleModeButton");
        authSO.FindProperty("toggleModeText").objectReferenceValue     = Find<TextMeshProUGUI>(authPanel.transform, "FormCard/ToggleModeButton/Text");
        authSO.FindProperty("statusText").objectReferenceValue         = Find<TextMeshProUGUI>(authPanel.transform, "StatusText");
        authSO.ApplyModifiedProperties();
        WarnIfUnwired(authSO, "authManager", "authPanel", "gachaPanel", "emailInputField",
            "passwordInputField", "actionButton", "actionButtonText",
            "toggleModeButton", "toggleModeText", "statusText");

        var gachaSO = new SerializedObject(gachaMgr);
        gachaSO.FindProperty("pullButton").objectReferenceValue        = Find<Button>(gachaPanel.transform, "PullButton");
        gachaSO.FindProperty("statusText").objectReferenceValue        = Find<TextMeshProUGUI>(gachaPanel.transform, "StatusText");
        gachaSO.FindProperty("balanceText").objectReferenceValue       = Find<TextMeshProUGUI>(gachaPanel.transform, "HeaderBar/BalanceText");
        gachaSO.FindProperty("starImage1").objectReferenceValue         = Find<Image>(gachaPanel.transform, "ItemCard/RevealedState/StarsRow/Star1");
        gachaSO.FindProperty("starImage2").objectReferenceValue         = Find<Image>(gachaPanel.transform, "ItemCard/RevealedState/StarsRow/Star2");
        gachaSO.FindProperty("starImage3").objectReferenceValue         = Find<Image>(gachaPanel.transform, "ItemCard/RevealedState/StarsRow/Star3");
        gachaSO.FindProperty("itemNameText").objectReferenceValue      = Find<TextMeshProUGUI>(gachaPanel.transform, "ItemCard/RevealedState/ItemNameText");
        gachaSO.FindProperty("rarityBadgeImage").objectReferenceValue  = Find<Image>(gachaPanel.transform, "ItemCard/RevealedState/RarityBadge");
        gachaSO.FindProperty("rarityBadgeText").objectReferenceValue   = Find<TextMeshProUGUI>(gachaPanel.transform, "ItemCard/RevealedState/RarityBadge/RarityText");
        gachaSO.FindProperty("defaultCardState").objectReferenceValue  = Find<Transform>(gachaPanel.transform, "ItemCard/DefaultState")?.gameObject;
        gachaSO.FindProperty("revealedCardState").objectReferenceValue = Find<Transform>(gachaPanel.transform, "ItemCard/RevealedState")?.gameObject;
        gachaSO.ApplyModifiedProperties();
        WarnIfUnwired(gachaSO, "pullButton", "statusText", "balanceText",
            "starImage1", "starImage2", "starImage3",
            "itemNameText", "rarityBadgeImage", "rarityBadgeText",
            "defaultCardState", "revealedCardState");

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("<color=green>[SceneBuilder] Scene rebuilt successfully! Press Play to test.</color>");
    }

    // ── Auth Panel ───────────────────────────────────────────────────────────

    static GameObject BuildAuthPanel(Transform parent)
    {
        var panel = MakeRect(parent, "AuthPanel");
        Stretch(panel);

        var logoArea = MakeRect(panel.transform, "LogoArea");
        SetAnchored(logoArea, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(340, 160));
        logoArea.anchoredPosition = new Vector2(0, 210);

        var logoIcon = MakeImage(logoArea.transform, "LogoIcon", ColSurface);
        SetAnchored(logoIcon.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(72, 72));
        logoIcon.rectTransform.anchoredPosition = new Vector2(0, -20);

        var titleTmp = MakeTMP(logoArea.transform, "TitleText", "GAIAGACHA", 42, ColTextPrimary, FontStyles.Bold, s_Cinzel);
        SetAnchored(titleTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(320, 52));
        titleTmp.rectTransform.anchoredPosition = new Vector2(0, -88);
        titleTmp.alignment = TextAlignmentOptions.Center;

        var subtitleTmp = MakeTMP(logoArea.transform, "SubtitleText", "DISCOVER  ·  PULL  ·  COLLECT", 13, ColTextMuted, FontStyles.Normal);
        SetAnchored(subtitleTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 24));
        subtitleTmp.rectTransform.anchoredPosition = new Vector2(0, -136);
        subtitleTmp.alignment = TextAlignmentOptions.Center;
        subtitleTmp.characterSpacing = 4;

        var formCard = MakeImage(panel.transform, "FormCard", ColSurface);
        SetAnchored(formCard.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(320, 310));
        formCard.rectTransform.anchoredPosition = new Vector2(0, -30);

        var emailInput = MakeInputField(formCard.transform, "EmailInput", "Email address", false);
        SetAnchored(emailInput.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 52));
        emailInput.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -40);

        var passwordInput = MakeInputField(formCard.transform, "PasswordInput", "Password", true);
        SetAnchored(passwordInput.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 52));
        passwordInput.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -108);

        var (actionBtnGo, _) = MakeButton(formCard.transform, "ActionButton", "Sign In", 20);
        SetAnchored(actionBtnGo.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 62));
        actionBtnGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -184);

        var (toggleBtnGo, _) = MakeLinkButton(formCard.transform, "ToggleModeButton", "Don't have an account? <b>Register</b>", 14);
        SetAnchored(toggleBtnGo.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 36));
        toggleBtnGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -262);

        var statusTmp = MakeTMP(panel.transform, "StatusText", "", 13, ColTextMuted, FontStyles.Normal);
        SetAnchored(statusTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(320, 40));
        statusTmp.rectTransform.anchoredPosition = new Vector2(0, -210);
        statusTmp.alignment = TextAlignmentOptions.Center;
        statusTmp.textWrappingMode = TextWrappingModes.Normal;

        return panel.gameObject;
    }

    // ── Gacha Panel ──────────────────────────────────────────────────────────

    static GameObject BuildGachaPanel(Transform parent)
    {
        var panel = MakeRect(parent, "GachaPanel");
        Stretch(panel);

        var header = MakeImage(panel.transform, "HeaderBar", ColSurface);
        SetAnchored(header.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0, 64));
        header.rectTransform.anchoredPosition = new Vector2(0, -32);

        var titleTmp = MakeTMP(header.transform, "TitleText", "GaiaGacha", 24, ColTextPrimary, FontStyles.Bold, s_Cinzel);
        SetAnchored(titleTmp.rectTransform, new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 40));
        titleTmp.rectTransform.anchoredPosition = new Vector2(20, 0);
        titleTmp.alignment = TextAlignmentOptions.MidlineLeft;

        var balanceTmp = MakeTMP(header.transform, "BalanceText", "Eco-Coins: --", 16, ColGold, FontStyles.Bold);
        SetAnchored(balanceTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0, 40));
        balanceTmp.rectTransform.anchoredPosition = new Vector2(-20, 0);
        balanceTmp.alignment = TextAlignmentOptions.MidlineRight;

        var bannerTmp = MakeTMP(panel.transform, "BannerLabel", "NATURE'S COLLECTION", 14, ColTextMuted, FontStyles.Normal);
        SetAnchored(bannerTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 30));
        bannerTmp.rectTransform.anchoredPosition = new Vector2(0, -260);
        bannerTmp.alignment = TextAlignmentOptions.Center;
        bannerTmp.characterSpacing = 4;

        var itemCard = MakeImage(panel.transform, "ItemCard", ColSurface);
        SetAnchored(itemCard.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 320));
        itemCard.rectTransform.anchoredPosition = new Vector2(0, -380);

        var defaultState = MakeRect(itemCard.transform, "DefaultState");
        Stretch(defaultState);

        var questionMark = MakeTMP(defaultState.transform, "QuestionMark", "?", 52, ColTextPrimary, FontStyles.Bold);
        SetAnchored(questionMark.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(120, 70));
        questionMark.rectTransform.anchoredPosition = new Vector2(0, 10);
        questionMark.alignment = TextAlignmentOptions.Center;

        var readyTmp = MakeTMP(defaultState.transform, "ReadyText", "Ready to discover your ecosystem", 15, ColTextPrimary, FontStyles.Normal);
        SetAnchored(readyTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(220, 44));
        readyTmp.rectTransform.anchoredPosition = new Vector2(0, -40);
        readyTmp.alignment = TextAlignmentOptions.Center;
        readyTmp.textWrappingMode = TextWrappingModes.Normal;

        var revealedState = MakeRect(itemCard.transform, "RevealedState");
        Stretch(revealedState);
        revealedState.gameObject.SetActive(false);

        var starsRow = MakeRect(revealedState.transform, "StarsRow");
        SetAnchored(starsRow, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(80, 20));
        starsRow.anchoredPosition = new Vector2(0, -24);

        var star1 = MakeImage(starsRow.transform, "Star1", ColGold);
        SetAnchored(star1.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(16, 16));
        star1.rectTransform.anchoredPosition = new Vector2(-28, 0);
        star1.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        var star2 = MakeImage(starsRow.transform, "Star2", new Color(0.2f, 0.32f, 0.24f));
        SetAnchored(star2.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(16, 16));
        star2.rectTransform.anchoredPosition = new Vector2(0, 0);
        star2.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        var star3 = MakeImage(starsRow.transform, "Star3", new Color(0.2f, 0.32f, 0.24f));
        SetAnchored(star3.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(16, 16));
        star3.rectTransform.anchoredPosition = new Vector2(28, 0);
        star3.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        var circle = MakeImage(revealedState.transform, "PlaceholderCircle", ColTextMuted);
        SetAnchored(circle.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(80, 80));
        circle.rectTransform.anchoredPosition = new Vector2(0, 16);

        var itemNameTmp = MakeTMP(revealedState.transform, "ItemNameText", "", 18, ColTextPrimary, FontStyles.Bold);
        SetAnchored(itemNameTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(240, 32));
        itemNameTmp.rectTransform.anchoredPosition = new Vector2(0, -48);
        itemNameTmp.alignment = TextAlignmentOptions.Center;

        var rarityBadge = MakeImage(revealedState.transform, "RarityBadge", Hex("#A8B5A2"));
        SetAnchored(rarityBadge.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(110, 28));
        rarityBadge.rectTransform.anchoredPosition = new Vector2(0, -84);

        var rarityTmp = MakeTMP(rarityBadge.transform, "RarityText", "COMMON", 12, ColButtonText, FontStyles.Bold);
        SetAnchored(rarityTmp.rectTransform, Vector2.zero, Vector2.one, new Vector2(0, 0));
        rarityTmp.alignment = TextAlignmentOptions.Center;

        var (pullBtnGo, _) = MakeButton(panel.transform, "PullButton", "Pull  ·  10 Eco-Coins", 20);
        SetAnchored(pullBtnGo.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 62));
        pullBtnGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -580);

        var statusTmp = MakeTMP(panel.transform, "StatusText", "", 13, ColTextMuted, FontStyles.Normal);
        SetAnchored(statusTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 32));
        statusTmp.rectTransform.anchoredPosition = new Vector2(0, -660);
        statusTmp.alignment = TextAlignmentOptions.Center;

        return panel.gameObject;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    static RectTransform MakeRect(Transform parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go.AddComponent<RectTransform>();
    }

    static Image MakeImage(Transform parent, string name, Color color)
    {
        var rt = MakeRect(parent, name);
        var img = rt.gameObject.AddComponent<Image>();
        img.color = color;
        return img;
    }

    static TextMeshProUGUI MakeTMP(Transform parent, string name, string text, float size, Color color, FontStyles style, TMP_FontAsset font = null)
    {
        var rt = MakeRect(parent, name);
        var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.fontStyle = style;
        var f = font ?? s_Poppins;
        if (f != null) tmp.font = f;
        return tmp;
    }

    static TMP_InputField MakeInputField(Transform parent, string name, string placeholder, bool isPassword)
    {
        var bg = MakeImage(parent, name, new Color(0.063f, 0.133f, 0.082f));
        var input = bg.gameObject.AddComponent<TMP_InputField>();

        var textArea = new GameObject("Text Area");
        textArea.transform.SetParent(bg.transform, false);
        var textAreaRt = textArea.AddComponent<RectTransform>();
        textAreaRt.anchorMin = Vector2.zero;
        textAreaRt.anchorMax = Vector2.one;
        textAreaRt.offsetMin = new Vector2(12, 6);
        textAreaRt.offsetMax = new Vector2(-12, -6);
        textArea.AddComponent<RectMask2D>();

        var phGo = new GameObject("Placeholder");
        phGo.transform.SetParent(textArea.transform, false);
        var phRt = phGo.AddComponent<RectTransform>();
        phRt.anchorMin = Vector2.zero;
        phRt.anchorMax = Vector2.one;
        phRt.offsetMin = phRt.offsetMax = Vector2.zero;
        var phTmp = phGo.AddComponent<TextMeshProUGUI>();
        phTmp.text = placeholder;
        phTmp.color = ColTextMuted;
        phTmp.fontSize = 15;
        if (s_Poppins != null) phTmp.font = s_Poppins;

        var txtGo = new GameObject("Text");
        txtGo.transform.SetParent(textArea.transform, false);
        var txtRt = txtGo.AddComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.offsetMin = txtRt.offsetMax = Vector2.zero;
        var txtTmp = txtGo.AddComponent<TextMeshProUGUI>();
        txtTmp.color = ColTextPrimary;
        txtTmp.fontSize = 15;
        if (s_Poppins != null) txtTmp.font = s_Poppins;

        input.textComponent = txtTmp;
        input.placeholder = phTmp;
        input.textViewport = textAreaRt;
        if (isPassword) input.contentType = TMP_InputField.ContentType.Password;

        return input;
    }

    static (GameObject go, TextMeshProUGUI label) MakeButton(Transform parent, string name, string text, float fontSize = 16)
    {
        var bg  = MakeImage(parent, name, ColButton);
        var btn = bg.gameObject.AddComponent<Button>();
        btn.targetGraphic = bg;
        var tmp = MakeTMP(bg.transform, "Text", text, fontSize, ColButtonText, FontStyles.Bold);
        SetAnchored(tmp.rectTransform, Vector2.zero, Vector2.one, Vector2.zero);
        tmp.alignment = TextAlignmentOptions.Center;
        return (bg.gameObject, tmp);
    }

    static (GameObject go, TextMeshProUGUI label) MakeLinkButton(Transform parent, string name, string text, float fontSize = 13)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = Color.clear;
        go.AddComponent<Button>();
        var tmp = MakeTMP(go.transform, "Text", text, fontSize, ColTextMuted, FontStyles.Normal);
        SetAnchored(tmp.rectTransform, Vector2.zero, Vector2.one, Vector2.zero);
        tmp.alignment = TextAlignmentOptions.Center;
        return (go, tmp);
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    static void SetAnchored(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta)
    {
        rt.anchorMin  = anchorMin;
        rt.anchorMax  = anchorMax;
        rt.sizeDelta  = sizeDelta;
    }

    static T Find<T>(Transform root, string path) where T : Component
    {
        var t = root.Find(path);
        return t != null ? t.GetComponent<T>() : null;
    }

    static Color Hex(string hex)
    {
        if (!ColorUtility.TryParseHtmlString(hex, out var c))
            Debug.LogError($"[SceneBuilder] Invalid hex color: '{hex}'");
        return c;
    }

    static void WarnIfUnwired(SerializedObject so, params string[] props)
    {
        foreach (var prop in props)
        {
            var p = so.FindProperty(prop);
            if (p == null || p.objectReferenceValue == null)
                Debug.LogWarning($"[SceneBuilder] '{prop}' on {so.targetObject.name} was not wired.");
        }
    }
}
