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
    [MenuItem("GaiaGacha/Build Scene")]
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

        var authPanel  = (GameObject)PrefabUtility.InstantiatePrefab(authPrefab,  canvasGo.transform);
        var gachaPanel = (GameObject)PrefabUtility.InstantiatePrefab(gachaPrefab, canvasGo.transform);
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
        authSO.FindProperty("emailInputField").objectReferenceValue    = UIConstants.Find<TMP_InputField>(authPanel.transform, "FormCard/EmailInput");
        authSO.FindProperty("passwordInputField").objectReferenceValue = UIConstants.Find<TMP_InputField>(authPanel.transform, "FormCard/PasswordInput");
        authSO.FindProperty("actionButton").objectReferenceValue       = UIConstants.Find<Button>(authPanel.transform, "FormCard/ActionButton");
        authSO.FindProperty("actionButtonText").objectReferenceValue   = UIConstants.Find<TextMeshProUGUI>(authPanel.transform, "FormCard/ActionButton/Text");
        authSO.FindProperty("toggleModeButton").objectReferenceValue   = UIConstants.Find<Button>(authPanel.transform, "FormCard/ToggleModeButton");
        authSO.FindProperty("toggleModeText").objectReferenceValue     = UIConstants.Find<TextMeshProUGUI>(authPanel.transform, "FormCard/ToggleModeButton/Text");
        authSO.FindProperty("statusText").objectReferenceValue         = UIConstants.Find<TextMeshProUGUI>(authPanel.transform, "StatusText");
        authSO.ApplyModifiedProperties(); // save all the wired references
        UIConstants.WarnIfUnwired(authSO, "authManager", "authPanel", "gachaPanel", "emailInputField",
            "passwordInputField", "actionButton", "actionButtonText",
            "toggleModeButton", "toggleModeText", "statusText");

        // Wire up the GachaManager Inspector references in the same way.
        var gachaSO = new SerializedObject(gachaMgr);
        gachaSO.FindProperty("pullButton").objectReferenceValue        = UIConstants.Find<Button>(gachaPanel.transform, "PullButton");
        gachaSO.FindProperty("statusText").objectReferenceValue        = UIConstants.Find<TextMeshProUGUI>(gachaPanel.transform, "StatusText");
        gachaSO.FindProperty("balanceText").objectReferenceValue       = UIConstants.Find<TextMeshProUGUI>(gachaPanel.transform, "HeaderBar/BalanceText");
        gachaSO.FindProperty("starImage1").objectReferenceValue        = UIConstants.Find<Image>(gachaPanel.transform, "ItemCard/RevealedState/StarsRow/Star1");
        gachaSO.FindProperty("starImage2").objectReferenceValue        = UIConstants.Find<Image>(gachaPanel.transform, "ItemCard/RevealedState/StarsRow/Star2");
        gachaSO.FindProperty("starImage3").objectReferenceValue        = UIConstants.Find<Image>(gachaPanel.transform, "ItemCard/RevealedState/StarsRow/Star3");
        gachaSO.FindProperty("itemNameText").objectReferenceValue      = UIConstants.Find<TextMeshProUGUI>(gachaPanel.transform, "ItemCard/RevealedState/ItemNameText");
        gachaSO.FindProperty("rarityBadgeImage").objectReferenceValue  = UIConstants.Find<Image>(gachaPanel.transform, "ItemCard/RevealedState/RarityBadge");
        gachaSO.FindProperty("rarityBadgeText").objectReferenceValue   = UIConstants.Find<TextMeshProUGUI>(gachaPanel.transform, "ItemCard/RevealedState/RarityBadge/RarityText");
        gachaSO.FindProperty("defaultCardState").objectReferenceValue  = UIConstants.Find<Transform>(gachaPanel.transform, "ItemCard/DefaultState")?.gameObject;
        gachaSO.FindProperty("revealedCardState").objectReferenceValue = UIConstants.Find<Transform>(gachaPanel.transform, "ItemCard/RevealedState")?.gameObject;
        gachaSO.ApplyModifiedProperties();
        UIConstants.WarnIfUnwired(gachaSO, "pullButton", "statusText", "balanceText",
            "starImage1", "starImage2", "starImage3",
            "itemNameText", "rarityBadgeImage", "rarityBadgeText",
            "defaultCardState", "revealedCardState");

        // Mark the scene as changed so Unity knows to save it.
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("<color=green>[SceneBuilder] Scene rebuilt successfully! Press Play to test.</color>");
    }
}
