using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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

    [MenuItem("GaiaGacha/Build Scene")]
    static void Build()
    {
        // Clear existing UI objects
        foreach (var c in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            Object.DestroyImmediate(c.gameObject);
        foreach (var e in Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None))
            Object.DestroyImmediate(e.gameObject);
        foreach (var a in Object.FindObjectsByType<AuthManager>(FindObjectsSortMode.None))
            Object.DestroyImmediate(a.gameObject);
        foreach (var g in Object.FindObjectsByType<GachaManager>(FindObjectsSortMode.None))
            Object.DestroyImmediate(g.gameObject);

        // Event System
        var esGo = new GameObject("EventSystem");
        esGo.AddComponent<EventSystem>();
        esGo.AddComponent<StandaloneInputModule>();

        // Canvas
        var canvasGo = new GameObject("Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(390, 844);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGo.AddComponent<GraphicRaycaster>();

        // Background
        var bgImg = MakeImage(canvasGo.transform, "Background", ColBg);
        Stretch(bgImg.rectTransform);

        // Panels
        var authPanel  = BuildAuthPanel(canvasGo.transform);
        var gachaPanel = BuildGachaPanel(canvasGo.transform);
        gachaPanel.SetActive(false);

        // Manager GameObjects
        var authMgrGo = new GameObject("_AuthManager");
        var authMgr   = authMgrGo.AddComponent<AuthManager>();
        var authUIMgr = authMgrGo.AddComponent<AuthUIManager>();

        var gachaMgrGo = new GameObject("_GachaManager");
        var gachaMgr   = gachaMgrGo.AddComponent<GachaManager>();

        // Wire AuthUIManager
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

        // Wire GachaManager
        var gachaSO = new SerializedObject(gachaMgr);
        gachaSO.FindProperty("pullButton").objectReferenceValue        = Find<Button>(gachaPanel.transform, "PullButton");
        gachaSO.FindProperty("statusText").objectReferenceValue        = Find<TextMeshProUGUI>(gachaPanel.transform, "StatusText");
        gachaSO.FindProperty("balanceText").objectReferenceValue       = Find<TextMeshProUGUI>(gachaPanel.transform, "HeaderBar/BalanceText");
        gachaSO.FindProperty("starsText").objectReferenceValue         = Find<TextMeshProUGUI>(gachaPanel.transform, "ItemCard/RevealedState/StarsText");
        gachaSO.FindProperty("itemNameText").objectReferenceValue      = Find<TextMeshProUGUI>(gachaPanel.transform, "ItemCard/RevealedState/ItemNameText");
        gachaSO.FindProperty("rarityBadgeImage").objectReferenceValue  = Find<Image>(gachaPanel.transform, "ItemCard/RevealedState/RarityBadge");
        gachaSO.FindProperty("rarityBadgeText").objectReferenceValue   = Find<TextMeshProUGUI>(gachaPanel.transform, "ItemCard/RevealedState/RarityBadge/RarityText");
        gachaSO.FindProperty("defaultCardState").objectReferenceValue  = Find<Transform>(gachaPanel.transform, "ItemCard/DefaultState")?.gameObject;
        gachaSO.FindProperty("revealedCardState").objectReferenceValue = Find<Transform>(gachaPanel.transform, "ItemCard/RevealedState")?.gameObject;
        gachaSO.ApplyModifiedProperties();
        WarnIfUnwired(gachaSO, "pullButton", "statusText", "balanceText", "starsText",
            "itemNameText", "rarityBadgeImage", "rarityBadgeText",
            "defaultCardState", "revealedCardState");

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("<color=green>[SceneBuilder] Scene rebuilt successfully! Press Play to test.</color>");
    }

    // ── Auth Panel ───────────────────────────────────────────────────────────

    static GameObject BuildAuthPanel(Transform parent)
    {
        var panel = MakeRect(parent, "AuthPanel");
        SetAnchored(panel, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(340, 580));

        // Logo area
        var logoArea = MakeRect(panel.transform, "LogoArea");
        SetAnchored(logoArea, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(340, 160));
        logoArea.anchoredPosition = new Vector2(0, -80);

        var logoIcon = MakeImage(logoArea.transform, "LogoIcon", ColSurface);
        SetAnchored(logoIcon.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(72, 72));
        logoIcon.rectTransform.anchoredPosition = new Vector2(0, -20);

        var titleTmp = MakeTMP(logoArea.transform, "TitleText", "GAIAGACHA", 32, ColTextPrimary, FontStyles.Bold);
        SetAnchored(titleTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 44));
        titleTmp.rectTransform.anchoredPosition = new Vector2(0, -108);
        titleTmp.alignment = TextAlignmentOptions.Center;

        var subtitleTmp = MakeTMP(logoArea.transform, "SubtitleText", "DISCOVER  ·  PULL  ·  COLLECT", 11, ColTextMuted, FontStyles.Normal);
        SetAnchored(subtitleTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 24));
        subtitleTmp.rectTransform.anchoredPosition = new Vector2(0, -148);
        subtitleTmp.alignment = TextAlignmentOptions.Center;
        subtitleTmp.characterSpacing = 4;

        // Form card
        var formCard = MakeImage(panel.transform, "FormCard", ColSurface);
        SetAnchored(formCard.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(320, 300));
        formCard.rectTransform.anchoredPosition = new Vector2(0, 80);

        var emailInput = MakeInputField(formCard.transform, "EmailInput", "Email address", false);
        SetAnchored(emailInput.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 52));
        emailInput.rectTransform.anchoredPosition = new Vector2(0, -40);

        var passwordInput = MakeInputField(formCard.transform, "PasswordInput", "Password", true);
        SetAnchored(passwordInput.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 52));
        passwordInput.rectTransform.anchoredPosition = new Vector2(0, -108);

        var (actionBtnGo, _) = MakeButton(formCard.transform, "ActionButton", "Sign In");
        SetAnchored(actionBtnGo.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 52));
        actionBtnGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -184);

        var (toggleBtnGo, _) = MakeLinkButton(formCard.transform, "ToggleModeButton", "Don't have an account? <b>Register</b>");
        SetAnchored(toggleBtnGo.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 36));
        toggleBtnGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -252);

        // Status text
        var statusTmp = MakeTMP(panel.transform, "StatusText", "", 13, ColTextMuted, FontStyles.Normal);
        SetAnchored(statusTmp.rectTransform, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(320, 40));
        statusTmp.rectTransform.anchoredPosition = new Vector2(0, 30);
        statusTmp.alignment = TextAlignmentOptions.Center;
        statusTmp.enableWordWrapping = true;

        return panel.gameObject;
    }

    // ── Gacha Panel ──────────────────────────────────────────────────────────

    static GameObject BuildGachaPanel(Transform parent)
    {
        var panel = MakeRect(parent, "GachaPanel");
        Stretch(panel);

        // Header bar
        var header = MakeImage(panel.transform, "HeaderBar", ColSurface);
        SetAnchored(header.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0, 64));
        header.rectTransform.anchoredPosition = new Vector2(0, -32);

        var titleTmp = MakeTMP(header.transform, "TitleText", "GaiaGacha", 20, ColTextPrimary, FontStyles.Bold);
        SetAnchored(titleTmp.rectTransform, new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 40));
        titleTmp.rectTransform.anchoredPosition = new Vector2(20, 0);
        titleTmp.alignment = TextAlignmentOptions.MidlineLeft;

        var balanceTmp = MakeTMP(header.transform, "BalanceText", "Eco-Coins: --", 16, ColGold, FontStyles.Bold);
        SetAnchored(balanceTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0, 40));
        balanceTmp.rectTransform.anchoredPosition = new Vector2(-20, 0);
        balanceTmp.alignment = TextAlignmentOptions.MidlineRight;

        // Banner label
        var bannerTmp = MakeTMP(panel.transform, "BannerLabel", "NATURE'S COLLECTION", 11, ColTextMuted, FontStyles.Normal);
        SetAnchored(bannerTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 30));
        bannerTmp.rectTransform.anchoredPosition = new Vector2(0, -90);
        bannerTmp.alignment = TextAlignmentOptions.Center;
        bannerTmp.characterSpacing = 4;

        // Item card
        var itemCard = MakeImage(panel.transform, "ItemCard", ColSurface);
        SetAnchored(itemCard.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 320));
        itemCard.rectTransform.anchoredPosition = new Vector2(0, -280);

        // Default card state
        var defaultState = MakeRect(itemCard.transform, "DefaultState");
        Stretch(defaultState);

        var questionMark = MakeTMP(defaultState.transform, "QuestionMark", "?", 64, ColTextMuted, FontStyles.Bold);
        SetAnchored(questionMark.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(120, 80));
        questionMark.rectTransform.anchoredPosition = new Vector2(0, 20);
        questionMark.alignment = TextAlignmentOptions.Center;

        var readyTmp = MakeTMP(defaultState.transform, "ReadyText", "Ready to discover your ecosystem", 13, ColTextMuted, FontStyles.Normal);
        SetAnchored(readyTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(220, 40));
        readyTmp.rectTransform.anchoredPosition = new Vector2(0, -40);
        readyTmp.alignment = TextAlignmentOptions.Center;
        readyTmp.enableWordWrapping = true;

        // Revealed card state
        var revealedState = MakeRect(itemCard.transform, "RevealedState");
        Stretch(revealedState);
        revealedState.gameObject.SetActive(false);

        var starsTmp = MakeTMP(revealedState.transform, "StarsText", "★", 24, ColGold, FontStyles.Normal);
        SetAnchored(starsTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(200, 40));
        starsTmp.rectTransform.anchoredPosition = new Vector2(0, -30);
        starsTmp.alignment = TextAlignmentOptions.Center;

        var circle = MakeImage(revealedState.transform, "PlaceholderCircle", ColTextMuted);
        SetAnchored(circle.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(100, 100));
        circle.rectTransform.anchoredPosition = new Vector2(0, 20);

        var itemNameTmp = MakeTMP(revealedState.transform, "ItemNameText", "", 18, ColTextPrimary, FontStyles.Bold);
        SetAnchored(itemNameTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(240, 30));
        itemNameTmp.rectTransform.anchoredPosition = new Vector2(0, -60);
        itemNameTmp.alignment = TextAlignmentOptions.Center;

        var rarityBadge = MakeImage(revealedState.transform, "RarityBadge", Hex("#A8B5A2"));
        SetAnchored(rarityBadge.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(110, 26));
        rarityBadge.rectTransform.anchoredPosition = new Vector2(0, -100);

        var rarityTmp = MakeTMP(rarityBadge.transform, "RarityText", "COMMON", 11, ColButtonText, FontStyles.Bold);
        SetAnchored(rarityTmp.rectTransform, Vector2.zero, Vector2.one, new Vector2(0, 0));
        rarityTmp.alignment = TextAlignmentOptions.Center;

        // Pull button
        var (pullBtnGo, _) = MakeButton(panel.transform, "PullButton", "Pull  ·  10 Eco-Coins");
        SetAnchored(pullBtnGo.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 56));
        pullBtnGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -660);

        // Status text
        var statusTmp = MakeTMP(panel.transform, "StatusText", "", 13, ColTextMuted, FontStyles.Normal);
        SetAnchored(statusTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 36));
        statusTmp.rectTransform.anchoredPosition = new Vector2(0, -724);
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

    static TextMeshProUGUI MakeTMP(Transform parent, string name, string text, float size, Color color, FontStyles style)
    {
        var rt = MakeRect(parent, name);
        var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.fontStyle = style;
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

        var txtGo = new GameObject("Text");
        txtGo.transform.SetParent(textArea.transform, false);
        var txtRt = txtGo.AddComponent<RectTransform>();
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.offsetMin = txtRt.offsetMax = Vector2.zero;
        var txtTmp = txtGo.AddComponent<TextMeshProUGUI>();
        txtTmp.color = ColTextPrimary;
        txtTmp.fontSize = 15;

        input.textComponent = txtTmp;
        input.placeholder = phTmp;
        input.textViewport = textAreaRt;
        if (isPassword) input.contentType = TMP_InputField.ContentType.Password;

        return input;
    }

    static (GameObject go, TextMeshProUGUI label) MakeButton(Transform parent, string name, string text)
    {
        var bg  = MakeImage(parent, name, ColButton);
        var btn = bg.gameObject.AddComponent<Button>();
        btn.targetGraphic = bg;
        var tmp = MakeTMP(bg.transform, "Text", text, 16, ColButtonText, FontStyles.Bold);
        SetAnchored(tmp.rectTransform, Vector2.zero, Vector2.one, Vector2.zero);
        tmp.alignment = TextAlignmentOptions.Center;
        return (bg.gameObject, tmp);
    }

    static (GameObject go, TextMeshProUGUI label) MakeLinkButton(Transform parent, string name, string text)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = Color.clear;
        go.AddComponent<Button>();
        var tmp = MakeTMP(go.transform, "Text", text, 13, ColTextMuted, FontStyles.Normal);
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
