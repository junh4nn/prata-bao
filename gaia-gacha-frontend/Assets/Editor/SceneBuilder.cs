// SceneBuilder.cs — Unity Editor script
// Adds a "GaiaGacha → Build Scene" menu item to the Unity toolbar.
// Running it wipes the existing UI, loads the AuthPanel and GachaPanel prefabs,
// and wires all manager references

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using TMPro;

public static class SceneBuilder
{
    // ── Entry point ──────────────────────────────────────────────────────────
    [MenuItem("GaiaGacha/Build Scene", priority = 300)]
    static void Build()
    {
        // Wipe any existing Canvas, EventSystem, and manager objects so we start fresh.
        foreach (var c in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include))
            Object.DestroyImmediate(c.gameObject);
        foreach (var e in Object.FindObjectsByType<EventSystem>(FindObjectsInactive.Include))
            Object.DestroyImmediate(e.gameObject);
        foreach (var a in Object.FindObjectsByType<AuthManager>(FindObjectsInactive.Include))
            Object.DestroyImmediate(a.gameObject);
        foreach (var g in Object.FindObjectsByType<GachaManager>(FindObjectsInactive.Include))
            Object.DestroyImmediate(g.gameObject);
        foreach (var h in Object.FindObjectsByType<MainHubUIManager>(FindObjectsInactive.Include))
            Object.DestroyImmediate(h.gameObject);
        foreach (var inv in Object.FindObjectsByType<InventoryManager>(FindObjectsInactive.Include))
            Object.DestroyImmediate(inv.gameObject);
        foreach (var q in Object.FindObjectsByType<QuizManager>(FindObjectsInactive.Include))
            Object.DestroyImmediate(q.gameObject);

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
        var bgImg = UIConstants.MakeImage(canvasGo.transform, "Background", null, UIConstants.ColBg);
        UIConstants.Stretch(bgImg.rectTransform); // stretch to fill the entire canvas

        // Load the pre-built panel prefabs. Run "GaiaGacha/LayoutBuilders/Build Auth Panel" and
        // "GaiaGacha/LayoutBuilders/Build Gacha Panel" first if the prefabs don't exist yet.
        var authPrefab  = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AuthPanel.prefab");
        var gachaPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GachaPanel.prefab");
        var hubPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/HubPanel.prefab");
        var inventoryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/InventoryPanel.prefab");
        var quizPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/QuizPanel.prefab");

        var authPanel  = (GameObject)PrefabUtility.InstantiatePrefab(authPrefab,  canvasGo.transform);
        var gachaPanel = (GameObject)PrefabUtility.InstantiatePrefab(gachaPrefab, canvasGo.transform);
        gachaPanel.SetActive(false);
        var hubPanel = (GameObject)PrefabUtility.InstantiatePrefab(hubPrefab, canvasGo.transform);
        hubPanel.SetActive(false);
        var inventoryPanel = (GameObject)PrefabUtility.InstantiatePrefab(inventoryPrefab, canvasGo.transform);
        inventoryPanel.SetActive(false);
        var quizPanel = (GameObject)PrefabUtility.InstantiatePrefab(quizPrefab, canvasGo.transform);
        quizPanel.SetActive(false);

        // Create the manager GameObjects that handle the game logic.
        // AuthManager handles login/register API calls.
        // AuthUIManager handles what the auth panel looks like and reacts to.
        var authMgrGo = new GameObject("_AuthManager");
        var authMgr   = authMgrGo.AddComponent<AuthManager>();
        var authUIMgr = authMgrGo.AddComponent<AuthUIManager>();

        // GachaManager and MainHubUIManager live on their respective panels so that
        // Start/OnEnable fire when the panel activates (post-login), not at scene load.
        var gachaMgr = gachaPanel.AddComponent<GachaManager>();
        var hubUIMgr = hubPanel.AddComponent<MainHubUIManager>();
        var inventoryMgr = inventoryPanel.AddComponent<InventoryManager>();
        var quizMgr = quizPanel.AddComponent<QuizManager>();

        // Wire up the AuthUIManager Inspector references in code.
        // SerializedObject lets us set [SerializeField] values from an Editor script,
        // the same as dragging and dropping in the Inspector by hand.
        var authSO = new SerializedObject(authUIMgr);
        authSO.FindProperty("authManager").objectReferenceValue        = authMgr;
        authSO.FindProperty("authPanel").objectReferenceValue          = authPanel;
        authSO.FindProperty("hubPanel").objectReferenceValue           = hubPanel;
        authSO.FindProperty("emailInputField").objectReferenceValue    = UIConstants.Find<TMP_InputField>(authPanel.transform, "FormCard/EmailInput");
        authSO.FindProperty("passwordInputField").objectReferenceValue = UIConstants.Find<TMP_InputField>(authPanel.transform, "FormCard/PasswordInput");
        authSO.FindProperty("actionButton").objectReferenceValue       = UIConstants.Find<Button>(authPanel.transform, "FormCard/ActionButton");
        authSO.FindProperty("actionButtonText").objectReferenceValue   = UIConstants.Find<TextMeshProUGUI>(authPanel.transform, "FormCard/ActionButton/Text");
        authSO.FindProperty("toggleModeButton").objectReferenceValue   = UIConstants.Find<Button>(authPanel.transform, "FormCard/ToggleModeButton");
        authSO.FindProperty("toggleModeText").objectReferenceValue     = UIConstants.Find<TextMeshProUGUI>(authPanel.transform, "FormCard/ToggleModeButton/Text");
        authSO.FindProperty("statusText").objectReferenceValue         = UIConstants.Find<TextMeshProUGUI>(authPanel.transform, "StatusText");
        authSO.ApplyModifiedProperties(); // save all the wired references
        UIConstants.WarnIfUnwired(authSO, "authManager", "authPanel", "hubPanel", "emailInputField",
            "passwordInputField", "actionButton", "actionButtonText",
            "toggleModeButton", "toggleModeText", "statusText");

        // Wire up the GachaManager Inspector references in the same way.
        var gachaSO = new SerializedObject(gachaMgr);
        gachaSO.FindProperty("cardDisplay").objectReferenceValue  = UIConstants.Find<GachaCardDisplay>(gachaPanel.transform, "GachaCard");
        gachaSO.FindProperty("itemRegistry").objectReferenceValue = AssetDatabase.LoadAssetAtPath<ItemRegistry>("Assets/ScriptableObjects/ItemRegistry.asset");
        gachaSO.FindProperty("pullButton").objectReferenceValue   = UIConstants.Find<Button>(gachaPanel.transform, "PullButton");
        gachaSO.FindProperty("statusText").objectReferenceValue   = UIConstants.Find<TextMeshProUGUI>(gachaPanel.transform, "StatusText");
        gachaSO.FindProperty("balanceText").objectReferenceValue  = UIConstants.Find<TextMeshProUGUI>(gachaPanel.transform, "HeaderBar/BalanceText");
        gachaSO.FindProperty("gachaPanel").objectReferenceValue   = gachaPanel;
        gachaSO.FindProperty("hubPanel").objectReferenceValue     = hubPanel;
        gachaSO.FindProperty("backButton").objectReferenceValue   = UIConstants.Find<Button>(gachaPanel.transform, "FooterBar/BackButton");
        gachaSO.ApplyModifiedProperties();
        UIConstants.WarnIfUnwired(gachaSO, "cardDisplay", "itemRegistry",
            "pullButton", "statusText", "balanceText",
            "gachaPanel", "hubPanel", "backButton");

        var hubSO = new SerializedObject(hubUIMgr);
        hubSO.FindProperty("coinsText").objectReferenceValue       = UIConstants.Find<TextMeshProUGUI>(hubPanel.transform, "HeaderBar/CoinsText");
        hubSO.FindProperty("logoutButton").objectReferenceValue    = UIConstants.Find<Button>(hubPanel.transform, "FooterBar/LogoutButton");
        hubSO.FindProperty("quoteText").objectReferenceValue       = UIConstants.Find<TextMeshProUGUI>(hubPanel.transform, "HeroCard/QuoteText");
        hubSO.FindProperty("gachaButton").objectReferenceValue     = UIConstants.Find<Button>(hubPanel.transform, "GachaTile");
        hubSO.FindProperty("quizButton").objectReferenceValue      = UIConstants.Find<Button>(hubPanel.transform, "BottomRow/QuizTile");
        hubSO.FindProperty("inventoryButton").objectReferenceValue = UIConstants.Find<Button>(hubPanel.transform, "BottomRow/InventoryTile");
        hubSO.FindProperty("hubPanel").objectReferenceValue        = hubPanel;
        hubSO.FindProperty("authPanel").objectReferenceValue       = authPanel;
        hubSO.FindProperty("gachaPanel").objectReferenceValue      = gachaPanel;
        hubSO.FindProperty("inventoryPanel").objectReferenceValue  = inventoryPanel;
        hubSO.FindProperty("quizPanel").objectReferenceValue       = quizPanel;
        hubSO.ApplyModifiedProperties();
        UIConstants.WarnIfUnwired(hubSO, "coinsText", "logoutButton", "quoteText",
            "gachaButton", "quizButton", "inventoryButton", "hubPanel", "authPanel", "gachaPanel", "inventoryPanel", "quizPanel");

        // Wire up the InventoryManager Inspector references in the same way.
        var inventorySO = new SerializedObject(inventoryMgr);
        inventorySO.FindProperty("itemRegistry").objectReferenceValue        = AssetDatabase.LoadAssetAtPath<ItemRegistry>("Assets/ScriptableObjects/ItemRegistry.asset");
        inventorySO.FindProperty("inventoryCardPrefab").objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/InventoryCard.prefab").GetComponent<InventoryCardDisplay>();
        inventorySO.FindProperty("coinsText").objectReferenceValue           = UIConstants.Find<TextMeshProUGUI>(inventoryPanel.transform, "HeaderBar/CoinsText");
        inventorySO.FindProperty("categoryDropdown").objectReferenceValue    = UIConstants.Find<TMP_Dropdown>(inventoryPanel.transform, "FilterRow/CategoryDropdown");
        inventorySO.FindProperty("sortDropdown").objectReferenceValue       = UIConstants.Find<TMP_Dropdown>(inventoryPanel.transform, "FilterRow/SortDropdown");
        inventorySO.FindProperty("sortDirectionButton").objectReferenceValue = UIConstants.Find<Button>(inventoryPanel.transform, "FilterRow/SortDirectionButton");
        inventorySO.FindProperty("sortDirectionIcon").objectReferenceValue   = UIConstants.Find<TextMeshProUGUI>(inventoryPanel.transform, "FilterRow/SortDirectionButton/Text");
        inventorySO.FindProperty("gridContent").objectReferenceValue        = UIConstants.Find<Transform>(inventoryPanel.transform, "InventoryScrollRect/Viewport/Content");
        inventorySO.FindProperty("collectedCountText").objectReferenceValue = UIConstants.Find<TextMeshProUGUI>(inventoryPanel.transform, "CollectedCountText");
        inventorySO.FindProperty("detailModal").objectReferenceValue        = UIConstants.Find<ItemDetailModal>(inventoryPanel.transform, "DetailModal");
        inventorySO.FindProperty("inventoryPanel").objectReferenceValue     = inventoryPanel;
        inventorySO.FindProperty("hubPanel").objectReferenceValue           = hubPanel;
        inventorySO.FindProperty("backButton").objectReferenceValue         = UIConstants.Find<Button>(inventoryPanel.transform, "FooterBar/BackButton");
        inventorySO.ApplyModifiedProperties();
        UIConstants.WarnIfUnwired(inventorySO, "itemRegistry", "inventoryCardPrefab", "coinsText",
            "categoryDropdown", "sortDropdown", "sortDirectionButton", "sortDirectionIcon",
            "gridContent", "collectedCountText", "detailModal",
            "inventoryPanel", "hubPanel", "backButton");

        // Wire up the QuizManager Inspector references in the same way.
        var quizSO = new SerializedObject(quizMgr);
        quizSO.FindProperty("coinsText").objectReferenceValue         = UIConstants.Find<TextMeshProUGUI>(quizPanel.transform, "HeaderBar/CoinsText");
        quizSO.FindProperty("progressFillImage").objectReferenceValue = UIConstants.Find<Image>(quizPanel.transform, "ProgressBar/Track/Fill");
        quizSO.FindProperty("progressText").objectReferenceValue      = UIConstants.Find<TextMeshProUGUI>(quizPanel.transform, "ProgressBar/ProgressText");
        quizSO.FindProperty("questionText").objectReferenceValue      = UIConstants.Find<TextMeshProUGUI>(quizPanel.transform, "QuestionCard/QuestionText");
        quizSO.FindProperty("answerRowA").objectReferenceValue        = UIConstants.Find<QuizAnswerOptionDisplay>(quizPanel.transform, "QuestionCard/AnswerRowA");
        quizSO.FindProperty("answerRowB").objectReferenceValue        = UIConstants.Find<QuizAnswerOptionDisplay>(quizPanel.transform, "QuestionCard/AnswerRowB");
        quizSO.FindProperty("answerRowC").objectReferenceValue        = UIConstants.Find<QuizAnswerOptionDisplay>(quizPanel.transform, "QuestionCard/AnswerRowC");
        quizSO.FindProperty("feedbackText").objectReferenceValue      = UIConstants.Find<TextMeshProUGUI>(quizPanel.transform, "FeedbackText");
        quizSO.FindProperty("nextButton").objectReferenceValue        = UIConstants.Find<Button>(quizPanel.transform, "NextButton");
        quizSO.FindProperty("resultsModal").objectReferenceValue      = UIConstants.Find<Transform>(quizPanel.transform, "ResultsModal")?.gameObject;
        quizSO.FindProperty("resultGlyphText").objectReferenceValue   = UIConstants.Find<TextMeshProUGUI>(quizPanel.transform, "ResultsModal/ResultsCard/ResultGlyphText");
        quizSO.FindProperty("resultTitleText").objectReferenceValue   = UIConstants.Find<TextMeshProUGUI>(quizPanel.transform, "ResultsModal/ResultsCard/ResultTitleText");
        quizSO.FindProperty("scoreText").objectReferenceValue         = UIConstants.Find<TextMeshProUGUI>(quizPanel.transform, "ResultsModal/ResultsCard/ScoreText");
        quizSO.FindProperty("subtitleText").objectReferenceValue      = UIConstants.Find<TextMeshProUGUI>(quizPanel.transform, "ResultsModal/ResultsCard/SubtitleText");
        quizSO.FindProperty("rewardText").objectReferenceValue        = UIConstants.Find<TextMeshProUGUI>(quizPanel.transform, "ResultsModal/ResultsCard/RewardPill/RewardText");
        quizSO.FindProperty("tryAgainButton").objectReferenceValue    = UIConstants.Find<Button>(quizPanel.transform, "ResultsModal/ResultsCard/TryAgainButton");
        quizSO.FindProperty("quizPanel").objectReferenceValue         = quizPanel;
        quizSO.FindProperty("hubPanel").objectReferenceValue          = hubPanel;
        quizSO.FindProperty("backButton").objectReferenceValue        = UIConstants.Find<Button>(quizPanel.transform, "FooterBar/BackButton");
        quizSO.ApplyModifiedProperties();
        UIConstants.WarnIfUnwired(quizSO, "coinsText", "progressFillImage", "progressText", "questionText",
            "answerRowA", "answerRowB", "answerRowC", "feedbackText", "nextButton",
            "resultsModal", "resultGlyphText", "resultTitleText", "scoreText", "subtitleText", "rewardText", "tryAgainButton",
            "quizPanel", "hubPanel", "backButton");

        // Mark the scene as changed so Unity knows to save it.
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("<color=green>[SceneBuilder] Scene rebuilt successfully! Press Play to test.</color>");
    }
}
