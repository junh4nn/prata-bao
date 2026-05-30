// SceneBuilder.cs — Unity Editor script
// Adds a "GaiaGacha → Build Scene" menu item to the Unity toolbar.
// Running it wipes the existing UI and rebuilds the entire scene from scratch
// in code, so you never have to drag-and-drop references manually in the Inspector.

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using TMPro;

public static class SceneBuilder
{
    // ── Color palette ────────────────────────────────────────────────────────
    // All UI colors defined in one place. Change a hex here and it updates everywhere.
    static readonly Color ColBg          = Hex("#081C15"); // dark forest background
    static readonly Color ColSurface     = Hex("#1B4332"); // panels, cards, input fields
    static readonly Color ColButton      = Hex("#D4A373"); // earth-tone primary button
    static readonly Color ColButtonText  = Hex("#1B4332"); // dark text on buttons
    static readonly Color ColTextPrimary = Hex("#F1FAEE"); // headings and body text
    static readonly Color ColTextMuted   = Hex("#95B8A0"); // placeholder and subtitle text
    static readonly Color ColGold        = Hex("#E9C46A"); // gold for coins and rarity diamonds

    // ── Fonts ────────────────────────────────────────────────────────────────
    // Loaded from Assets/Fonts/ at build time.
    // Cinzel is used for titles only; Poppins is used for everything else.
    static TMP_FontAsset s_Poppins;
    static TMP_FontAsset s_Cinzel;

    // ── Entry point ──────────────────────────────────────────────────────────
    // This creates the "GaiaGacha → Build Scene" menu item in the Unity toolbar.
    [MenuItem("GaiaGacha/Build Scene")]
    static void Build()
    {
        // Load fonts from the Assets/Fonts folder.
        // If these files don't exist you'll see a yellow warning in the Console.
        s_Poppins = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Poppins-SemiBold SDF.asset");
        s_Cinzel  = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Cinzel-Regular SDF.asset");
        if (s_Poppins == null) Debug.LogWarning("[SceneBuilder] Poppins font not found at Assets/Fonts/Poppins-SemiBold SDF.asset");
        if (s_Cinzel  == null) Debug.LogWarning("[SceneBuilder] Cinzel font not found at Assets/Fonts/Cinzel-Regular SDF.asset");

        // Wipe any existing Canvas, EventSystem, and manager objects so we start fresh.
        foreach (var c in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            Object.DestroyImmediate(c.gameObject);
        foreach (var e in Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None))
            Object.DestroyImmediate(e.gameObject);
        foreach (var a in Object.FindObjectsByType<AuthManager>(FindObjectsSortMode.None))
            Object.DestroyImmediate(a.gameObject);
        foreach (var g in Object.FindObjectsByType<GachaManager>(FindObjectsSortMode.None))
            Object.DestroyImmediate(g.gameObject);

        // EventSystem is required for Unity UI to detect mouse clicks and keyboard input.
        // InputSystemUIInputModule works with Unity's new Input System package.
        var esGo = new GameObject("EventSystem");
        esGo.AddComponent<EventSystem>();
        esGo.AddComponent<InputSystemUIInputModule>();

        // Canvas is the root container for all UI elements.
        // ScreenSpaceOverlay means it renders on top of the game world.
        // ScaleWithScreenSize makes the UI resize proportionally on different screens.
        // referenceResolution is the "design size" — 390x844 is an iPhone-sized portrait screen.
        // matchWidthOrHeight = 1f means we scale based on screen HEIGHT (good for portrait UIs on PC).
        var canvasGo = new GameObject("Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(390, 844);
        scaler.matchWidthOrHeight = 1f;
        canvasGo.AddComponent<GraphicRaycaster>(); // allows UI elements to receive click events

        // Full-screen background image using the darkest color in the palette.
        var bgImg = MakeImage(canvasGo.transform, "Background", ColBg);
        Stretch(bgImg.rectTransform); // stretch to fill the entire canvas

        // Build both panels. The gacha panel is hidden at the start —
        // it only becomes visible after the user logs in successfully.
        var authPanel  = BuildAuthPanel(canvasGo.transform);
        var gachaPanel = BuildGachaPanel(canvasGo.transform);
        gachaPanel.SetActive(false);

        // Create the manager GameObjects that handle the game logic.
        // AuthManager handles login/register API calls.
        // AuthUIManager handles what the auth panel looks like and reacts to.
        var authMgrGo = new GameObject("_AuthManager");
        var authMgr   = authMgrGo.AddComponent<AuthManager>();
        var authUIMgr = authMgrGo.AddComponent<AuthUIManager>();

        // GachaManager handles pull requests and updating the item card.
        var gachaMgrGo = new GameObject("_GachaManager");
        var gachaMgr   = gachaMgrGo.AddComponent<GachaManager>();

        // Wire up the AuthUIManager Inspector references in code.
        // SerializedObject lets us set [SerializeField] values from an Editor script,
        // the same as dragging and dropping in the Inspector by hand.
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
        authSO.ApplyModifiedProperties(); // save all the wired references
        WarnIfUnwired(authSO, "authManager", "authPanel", "gachaPanel", "emailInputField",
            "passwordInputField", "actionButton", "actionButtonText",
            "toggleModeButton", "toggleModeText", "statusText");

        // Wire up the GachaManager Inspector references in the same way.
        var gachaSO = new SerializedObject(gachaMgr);
        gachaSO.FindProperty("pullButton").objectReferenceValue        = Find<Button>(gachaPanel.transform, "PullButton");
        gachaSO.FindProperty("statusText").objectReferenceValue        = Find<TextMeshProUGUI>(gachaPanel.transform, "StatusText");
        gachaSO.FindProperty("balanceText").objectReferenceValue       = Find<TextMeshProUGUI>(gachaPanel.transform, "HeaderBar/BalanceText");
        gachaSO.FindProperty("starImage1").objectReferenceValue        = Find<Image>(gachaPanel.transform, "ItemCard/RevealedState/StarsRow/Star1");
        gachaSO.FindProperty("starImage2").objectReferenceValue        = Find<Image>(gachaPanel.transform, "ItemCard/RevealedState/StarsRow/Star2");
        gachaSO.FindProperty("starImage3").objectReferenceValue        = Find<Image>(gachaPanel.transform, "ItemCard/RevealedState/StarsRow/Star3");
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

        // Mark the scene as changed so Unity knows to save it.
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("<color=green>[SceneBuilder] Scene rebuilt successfully! Press Play to test.</color>");
    }

    // ── Auth Panel ───────────────────────────────────────────────────────────
    // Builds the login/register screen.
    // Layout (top to bottom):
    //   LogoArea  — icon square + "GAIAGACHA" title + subtitle
    //   FormCard  — email input, password input, Sign In button, Register link
    //   StatusText — error/feedback messages shown below the form

    static GameObject BuildAuthPanel(Transform parent)
    {
        // The auth panel fills the whole screen so elements can be placed anywhere.
        var panel = MakeRect(parent, "AuthPanel");
        Stretch(panel);

        // Logo area: centered horizontally, positioned in the upper portion of the screen.
        // anchoredPosition y=210 pushes it above the screen center.
        var logoArea = MakeRect(panel.transform, "LogoArea");
        SetAnchored(logoArea, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(340, 160));
        logoArea.anchoredPosition = new Vector2(0, 210);

        // Logo image loaded from Assets/Sprites/GaiaGacha.jpg.
        var logoIcon = MakeImage(logoArea.transform, "LogoIcon", ColBg);
        SetAnchored(logoIcon.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(120, 120));
        logoIcon.rectTransform.anchoredPosition = new Vector2(0, -20);
        var logoSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/GaiaGacha.jpg");
        if (logoSprite != null) logoIcon.sprite = logoSprite;
        else Debug.LogWarning("[SceneBuilder] Logo not found at Assets/Sprites/GaiaGacha.jpg");

        // Main game title in Cinzel (the decorative font).
        var titleTmp = MakeTMP(logoArea.transform, "TitleText", "GAIAGACHA", 42, ColTextPrimary, FontStyles.Bold, s_Cinzel);
        SetAnchored(titleTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(320, 52));
        titleTmp.rectTransform.anchoredPosition = new Vector2(0, -88);
        titleTmp.alignment = TextAlignmentOptions.Center;

        // Spaced-caps tagline below the title.
        var subtitleTmp = MakeTMP(logoArea.transform, "SubtitleText", "DISCOVER  ·  PULL  ·  COLLECT", 13, ColTextMuted, FontStyles.Normal);
        SetAnchored(subtitleTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 24));
        subtitleTmp.rectTransform.anchoredPosition = new Vector2(0, -136);
        subtitleTmp.alignment = TextAlignmentOptions.Center;
        subtitleTmp.characterSpacing = 4; // extra spacing for the spaced-caps look

        // Form card: the dark green box containing the inputs and button.
        // Centered on screen, slightly below the midpoint (y=-60).
        var formCard = MakeImage(panel.transform, "FormCard", ColSurface);
        SetAnchored(formCard.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(320, 310));
        formCard.rectTransform.anchoredPosition = new Vector2(0, -60);

        // Email input field — not a password field (isPassword = false).
        var emailInput = MakeInputField(formCard.transform, "EmailInput", "Email address", false);
        SetAnchored(emailInput.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 52));
        emailInput.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -40);

        // Password input field — characters are hidden (isPassword = true).
        var passwordInput = MakeInputField(formCard.transform, "PasswordInput", "Password", true);
        SetAnchored(passwordInput.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 52));
        passwordInput.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -108);

        // Primary action button — label is "Sign In" by default.
        // AuthUIManager.cs changes it to "Create Account" when toggled to register mode.
        var (actionBtnGo, _) = MakeButton(formCard.transform, "ActionButton", "Sign In", 20);
        SetAnchored(actionBtnGo.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 62));
        actionBtnGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -184);

        // Toggle link — tapping this switches between login and register mode.
        // Uses \n to put "Register" on its own line below the question text.
        var (toggleBtnGo, _) = MakeLinkButton(formCard.transform, "ToggleModeButton", "Don't have an account?\n<b>Register</b>", 14);
        SetAnchored(toggleBtnGo.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 36));
        toggleBtnGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -262);

        // Status text — shows login errors, "Logging in..." etc. Starts empty.
        var statusTmp = MakeTMP(panel.transform, "StatusText", "", 13, ColTextMuted, FontStyles.Normal);
        SetAnchored(statusTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(320, 40));
        statusTmp.rectTransform.anchoredPosition = new Vector2(0, -300);
        statusTmp.alignment = TextAlignmentOptions.Center;
        statusTmp.textWrappingMode = TextWrappingModes.Normal;

        return panel.gameObject;
    }

    // ── Gacha Panel ──────────────────────────────────────────────────────────
    // Builds the main pull/collection screen shown after login.
    // Layout (top to bottom):
    //   HeaderBar   — "GaiaGacha" title + Eco-Coins balance
    //   BannerLabel — "NATURE'S COLLECTION" label
    //   ItemCard    — shows "?" before a pull, then the item with rarity diamonds after
    //   PullButton  — costs 10 Eco-Coins per pull
    //   StatusText  — feedback during/after a pull

    static GameObject BuildGachaPanel(Transform parent)
    {
        // Gacha panel also fills the entire screen.
        var panel = MakeRect(parent, "GachaPanel");
        Stretch(panel);

        // Header bar pinned to the top of the screen.
        // anchorMin/Max of (0,1)→(1,1) means it stretches full width but only has a fixed height.
        var header = MakeImage(panel.transform, "HeaderBar", ColSurface);
        SetAnchored(header.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0, 64));
        header.rectTransform.anchoredPosition = new Vector2(0, -32);

        // Game title on the left side of the header, using the Cinzel decorative font.
        var titleTmp = MakeTMP(header.transform, "TitleText", "GaiaGacha", 24, ColTextPrimary, FontStyles.Bold, s_Cinzel);
        SetAnchored(titleTmp.rectTransform, new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 40));
        titleTmp.rectTransform.anchoredPosition = new Vector2(20, 0);
        titleTmp.alignment = TextAlignmentOptions.MidlineLeft;

        // Eco-Coins balance on the right side of the header, in gold text.
        // GachaManager.cs updates this after each pull.
        var balanceTmp = MakeTMP(header.transform, "BalanceText", "Eco-Coins: --", 16, ColGold, FontStyles.Bold);
        SetAnchored(balanceTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0, 40));
        balanceTmp.rectTransform.anchoredPosition = new Vector2(-20, 0);
        balanceTmp.alignment = TextAlignmentOptions.MidlineRight;

        // Small decorative label above the item card.
        var bannerTmp = MakeTMP(panel.transform, "BannerLabel", "NATURE'S COLLECTION", 14, ColTextMuted, FontStyles.Normal);
        SetAnchored(bannerTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 30));
        bannerTmp.rectTransform.anchoredPosition = new Vector2(0, -260);
        bannerTmp.alignment = TextAlignmentOptions.Center;
        bannerTmp.characterSpacing = 4;

        // Item card — the main display area. Contains two child states:
        //   DefaultState  — shown before any pull ("?" placeholder)
        //   RevealedState — shown after a pull (item name, rarity, diamonds)
        var itemCard = MakeImage(panel.transform, "ItemCard", ColSurface);
        SetAnchored(itemCard.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 320));
        itemCard.rectTransform.anchoredPosition = new Vector2(0, -380);

        // Default state: fills the entire card and shows a "?" until the player pulls.
        var defaultState = MakeRect(itemCard.transform, "DefaultState");
        Stretch(defaultState);

        var questionMark = MakeTMP(defaultState.transform, "QuestionMark", "?", 52, ColTextPrimary, FontStyles.Bold);
        SetAnchored(questionMark.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(120, 70));
        questionMark.rectTransform.anchoredPosition = new Vector2(0, 10);
        questionMark.alignment = TextAlignmentOptions.Center;

        var readyTmp = MakeTMP(defaultState.transform, "ReadyText", "What will nature reveal?", 12, ColTextMuted, FontStyles.Italic);
        SetAnchored(readyTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(220, 44));
        readyTmp.rectTransform.anchoredPosition = new Vector2(0, -40);
        readyTmp.alignment = TextAlignmentOptions.Center;
        readyTmp.textWrappingMode = TextWrappingModes.Normal;

        // Revealed state: hidden at start. GachaManager.cs activates it after a successful pull.
        var revealedState = MakeRect(itemCard.transform, "RevealedState");
        Stretch(revealedState);
        revealedState.gameObject.SetActive(false);

        // Rarity indicator: three diamond shapes in a row.
        // Gold = earned rarity tier, dark green = unearned tier.
        // GachaManager.cs sets the colors based on Common/Rare/Legendary.
        // Squares rotated 45° become diamond shapes — avoids font glyph issues with ★.
        var starsRow = MakeRect(revealedState.transform, "StarsRow");
        SetAnchored(starsRow, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(80, 20));
        starsRow.anchoredPosition = new Vector2(0, -24);

        var star1 = MakeImage(starsRow.transform, "Star1", ColGold); // always gold (at least 1 rarity)
        SetAnchored(star1.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(16, 16));
        star1.rectTransform.anchoredPosition = new Vector2(-28, 0);
        star1.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        var star2 = MakeImage(starsRow.transform, "Star2", new Color(0.2f, 0.32f, 0.24f)); // dim until Rare+
        SetAnchored(star2.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(16, 16));
        star2.rectTransform.anchoredPosition = new Vector2(0, 0);
        star2.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        var star3 = MakeImage(starsRow.transform, "Star3", new Color(0.2f, 0.32f, 0.24f)); // dim until Legendary
        SetAnchored(star3.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(16, 16));
        star3.rectTransform.anchoredPosition = new Vector2(28, 0);
        star3.rectTransform.localRotation = Quaternion.Euler(0, 0, 45);

        // Circular placeholder for a future item sprite/image.
        var circle = MakeImage(revealedState.transform, "PlaceholderCircle", ColTextMuted);
        SetAnchored(circle.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(120, 120));
        circle.rectTransform.anchoredPosition = new Vector2(0, 16);

        // Item name displayed in bold cream text after a pull.
        var itemNameTmp = MakeTMP(revealedState.transform, "ItemNameText", "", 18, ColTextPrimary, FontStyles.Bold);
        SetAnchored(itemNameTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(240, 32));
        itemNameTmp.rectTransform.anchoredPosition = new Vector2(0, -80);
        itemNameTmp.alignment = TextAlignmentOptions.Center;

        // Rarity badge — pill-shaped background whose color is set by GachaManager.cs.
        var rarityBadge = MakeImage(revealedState.transform, "RarityBadge", Hex("#A8B5A2"));
        SetAnchored(rarityBadge.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(110, 28));
        rarityBadge.rectTransform.anchoredPosition = new Vector2(0, -120);

        // Text inside the rarity badge ("COMMON", "RARE", or "LEGENDARY").
        var rarityTmp = MakeTMP(rarityBadge.transform, "RarityText", "COMMON", 12, ColButtonText, FontStyles.Bold);
        SetAnchored(rarityTmp.rectTransform, Vector2.zero, Vector2.one, new Vector2(0, 0));
        rarityTmp.alignment = TextAlignmentOptions.Center;

        // Pull button — costs 10 Eco-Coins. GachaManager.cs listens to its onClick event.
        var (pullBtnGo, _) = MakeButton(panel.transform, "PullButton", "Pull  ·  10 Eco-Coins", 20);
        SetAnchored(pullBtnGo.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 62));
        pullBtnGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -580);

        // Status text — shows "Connecting to nature registry..." during a pull, errors on failure.
        var statusTmp = MakeTMP(panel.transform, "StatusText", "", 13, ColTextMuted, FontStyles.Normal);
        SetAnchored(statusTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 32));
        statusTmp.rectTransform.anchoredPosition = new Vector2(0, -660);
        statusTmp.alignment = TextAlignmentOptions.Center;

        return panel.gameObject;
    }

    // ── Helper functions ─────────────────────────────────────────────────────
    // Small reusable utilities for creating UI elements.
    // These keep the build functions above short and readable.

    // Creates an empty GameObject with a RectTransform, parented to 'parent'.
    static RectTransform MakeRect(Transform parent, string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        return go.AddComponent<RectTransform>();
    }

    // Creates a colored Image (a flat rectangle with a color fill).
    static Image MakeImage(Transform parent, string name, Color color)
    {
        var rt = MakeRect(parent, name);
        var img = rt.gameObject.AddComponent<Image>();
        img.color = color;
        return img;
    }

    // Creates a TextMeshPro text element.
    // 'font' defaults to Poppins if not specified — pass s_Cinzel for titles.
    static TextMeshProUGUI MakeTMP(Transform parent, string name, string text, float size, Color color, FontStyles style, TMP_FontAsset font = null)
    {
        var rt = MakeRect(parent, name);
        var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = color;
        tmp.fontStyle = style;
        var f = font ?? s_Poppins; // fall back to Poppins if no specific font given
        if (f != null) tmp.font = f;
        return tmp;
    }

    // Creates a TMP_InputField (a text box the player can type into).
    // Handles all the required child objects Unity needs: Text Area, Placeholder, and Text.
    static TMP_InputField MakeInputField(Transform parent, string name, string placeholder, bool isPassword)
    {
        var bg = MakeImage(parent, name, new Color(0.063f, 0.133f, 0.082f)); // slightly darker than surface
        var input = bg.gameObject.AddComponent<TMP_InputField>();

        // Text Area clips the text so it doesn't overflow outside the input box.
        var textArea = new GameObject("Text Area");
        textArea.transform.SetParent(bg.transform, false);
        var textAreaRt = textArea.AddComponent<RectTransform>();
        textAreaRt.anchorMin = Vector2.zero;
        textAreaRt.anchorMax = Vector2.one;
        textAreaRt.offsetMin = new Vector2(12, 6);  // 12px left/right padding, 6px top/bottom
        textAreaRt.offsetMax = new Vector2(-12, -6);
        textArea.AddComponent<RectMask2D>(); // clips children to this rect

        // Placeholder text shown when the field is empty.
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

        // The actual typed text.
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

        // Wire up the TMP_InputField so it knows which child is the text and which is the placeholder.
        input.textComponent = txtTmp;
        input.placeholder = phTmp;
        input.textViewport = textAreaRt;
        if (isPassword) input.contentType = TMP_InputField.ContentType.Password; // hides characters with dots

        return input;
    }

    // Creates a solid earth-tone button with a text label.
    // fontSize defaults to 16 but can be overridden (e.g. 20 for the main action buttons).
    static (GameObject go, TextMeshProUGUI label) MakeButton(Transform parent, string name, string text, float fontSize = 16)
    {
        var bg  = MakeImage(parent, name, ColButton);
        var btn = bg.gameObject.AddComponent<Button>();
        btn.targetGraphic = bg; // tells Unity which image to tint on hover/press
        var tmp = MakeTMP(bg.transform, "Text", text, fontSize, ColButtonText, FontStyles.Bold);
        SetAnchored(tmp.rectTransform, Vector2.zero, Vector2.one, Vector2.zero); // fill the button
        tmp.alignment = TextAlignmentOptions.Center;
        return (bg.gameObject, tmp);
    }

    // Creates an invisible button that looks like a text link.
    // Used for the "Don't have an account? Register" toggle.
    static (GameObject go, TextMeshProUGUI label) MakeLinkButton(Transform parent, string name, string text, float fontSize = 13)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        go.AddComponent<Image>().color = Color.clear; // invisible background needed to receive clicks
        go.AddComponent<Button>();
        var tmp = MakeTMP(go.transform, "Text", text, fontSize, ColTextMuted, FontStyles.Normal);
        SetAnchored(tmp.rectTransform, Vector2.zero, Vector2.one, Vector2.zero);
        tmp.alignment = TextAlignmentOptions.Center;
        return (go, tmp);
    }

    // Makes a RectTransform fill its parent completely (anchors all four corners to parent corners).
    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    // Sets anchor points and size for a RectTransform.
    // anchorMin/anchorMax control which point on the parent this element is relative to.
    // sizeDelta is the width and height in pixels.
    static void SetAnchored(RectTransform rt, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta)
    {
        rt.anchorMin  = anchorMin;
        rt.anchorMax  = anchorMax;
        rt.sizeDelta  = sizeDelta;
    }

    // Finds a child GameObject by path and returns the requested component on it.
    // e.g. Find<Button>(panel, "FormCard/ActionButton") walks the hierarchy to get the Button.
    static T Find<T>(Transform root, string path) where T : Component
    {
        var t = root.Find(path);
        return t != null ? t.GetComponent<T>() : null;
    }

    // Converts a CSS-style hex color string (e.g. "#1B4332") to a Unity Color.
    static Color Hex(string hex)
    {
        if (!ColorUtility.TryParseHtmlString(hex, out var c))
            Debug.LogError($"[SceneBuilder] Invalid hex color: '{hex}'");
        return c;
    }

    // After wiring all references, this checks that none were missed.
    // A yellow warning in the Console means a field wasn't found and the game may not work correctly.
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
