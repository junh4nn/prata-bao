// GachaManager.cs — handles the gacha pull mechanic.
// When the player taps "Pull", this script sends a POST request to the backend,
// receives the pulled item, and updates the item card UI to show the result.

using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class GachaManager : MonoBehaviour
{
    // ── Inspector references ─────────────────────────────────────────────────
    // Wired automatically by SceneBuilder.cs.

    [Header("Backend Configuration")]
    [SerializeField] private string backendUrl = "http://localhost:3000/api/gacha/pull";
    [Tooltip("Paste a UUID here to bypass login for testing.")]
    [SerializeField] private string testUserId = ""; // leave empty in production

    [Header("UI - Pull Controls")]
    [SerializeField] private Button pullButton;            // the "Pull · 10 Eco-Coins" button
    [SerializeField] private TextMeshProUGUI statusText;   // feedback text below the button
    [SerializeField] private TextMeshProUGUI balanceText;  // Eco-Coins count in the header

    [Header("UI - Item Card")]
    [SerializeField] private Image itemImage;              // swapped per pull based on item name
    [SerializeField] private Image starImage1;             // leftmost rarity diamond (always gold on pull)
    [SerializeField] private Image starImage2;             // middle diamond (gold if Rare or Legendary)
    [SerializeField] private Image starImage3;             // rightmost diamond (gold if Legendary only)
    [SerializeField] private TextMeshProUGUI itemNameText; // the pulled item's name
    [SerializeField] private Image rarityBadgeImage;       // colored pill background (Common/Rare/Legendary)
    [SerializeField] private TextMeshProUGUI rarityBadgeText; // "COMMON", "RARE", or "LEGENDARY" label
    [SerializeField] private GameObject defaultCardState;  // the "?" state shown before any pull
    [SerializeField] private GameObject revealedCardState; // the item reveal state shown after a pull

    [Header("Item Sprites")]
    [SerializeField] private Sprite spriteMangrove;
    [SerializeField] private Sprite spriteCoral;
    [SerializeField] private Sprite spriteTurtle;

    [Header("Navigation")]
    [SerializeField] private GameObject gachaPanel;  // this panel — needed to hide on back
    [SerializeField] private GameObject hubPanel;     // shown when back button tapped
    [SerializeField] private Button     backButton;

    // Prevents the player from spamming the pull button mid-request.
    private bool isPulling = false;

    // ── Rarity colors ────────────────────────────────────────────────────────
    // Colors for the rarity badge background.
    static readonly Color ColCommon    = new Color(0.659f, 0.710f, 0.635f); // muted green
    static readonly Color ColRare      = new Color(0.322f, 0.718f, 0.533f); // bright green
    static readonly Color ColLegendary = new Color(0.914f, 0.769f, 0.404f); // gold

    // Colors for the rarity diamond indicators.
    static readonly Color ColStarActive   = new Color(0.914f, 0.769f, 0.404f); // gold — earned tier
    static readonly Color ColStarInactive = new Color(0.2f,   0.32f,  0.24f);  // dim green — unearned tier

    // ── Unity lifecycle ──────────────────────────────────────────────────────

    void Start()
    {
        // Set initial UI state.
        if (statusText    != null) statusText.text    = "";
        if (balanceText   != null) balanceText.text   = $"Eco-Coins: {AuthManager.Coins}";
        if (defaultCardState  != null) defaultCardState.SetActive(true);   // show the "?" card
        if (revealedCardState != null) revealedCardState.SetActive(false);  // hide the item reveal

        // Register the pull button's click listener.
        if (pullButton != null)
            pullButton.onClick.AddListener(OnPullButtonClicked);
        else
            Debug.LogError("[GachaManager] Pull Button reference is missing.");

        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
        else
            Debug.LogWarning("[GachaManager] Back Button reference is missing.");
    }

    // ── Pull logic ───────────────────────────────────────────────────────────

    // Called when the player taps the pull button.
    public void OnPullButtonClicked()
    {
        // Ignore extra taps while a pull is already in progress.
        if (isPulling) return;

        // Use the test user ID if set (for development), otherwise use the logged-in user.
        string activeUserId = !string.IsNullOrEmpty(testUserId)
            ? testUserId.Trim()
            : AuthManager.UserId;

        if (string.IsNullOrEmpty(activeUserId))
        {
            if (statusText != null) statusText.text = "<color=red>Please log in first.</color>";
            return;
        }

        StartCoroutine(SendPullRequest(activeUserId));
    }

    // Sends a POST request to the backend with the user's ID.
    // The backend picks a random item, deducts 10 Eco-Coins, and returns the result.
    private IEnumerator SendPullRequest(string userId)
    {
        isPulling = true;
        SetUIInteractivity(false); // disable the pull button during the request
        if (statusText != null) statusText.text = "Connecting to nature registry...";

        string jsonPayload = JsonUtility.ToJson(new PullRequest { userId = userId });

        using (UnityWebRequest request = new UnityWebRequest(backendUrl, "POST"))
        {
            request.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonPayload));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest(); // wait for the response

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                // Something went wrong — log the raw error and show a user-friendly message.
                string err = request.downloadHandler?.text ?? request.error;
                Debug.LogError($"[GachaManager] Backend error: {err}");
                if (statusText != null) statusText.text = "<color=red>Pull failed. Check server logs.</color>";
            }
            else
            {
                // Success — parse the JSON response and update the card.
                UpdateGachaUI(JsonUtility.FromJson<PullResponse>(request.downloadHandler.text));
            }
        }

        isPulling = false;
        SetUIInteractivity(true); // re-enable the pull button
    }

    // ── UI update ────────────────────────────────────────────────────────────

    // Updates the item card to display the pulled item's name, rarity, and diamonds.
    private void UpdateGachaUI(PullResponse response)
    {
        if (response?.item == null) return;

        // Switch from the default "?" state to the item reveal state.
        if (defaultCardState  != null) defaultCardState.SetActive(false);
        if (revealedCardState != null) revealedCardState.SetActive(true);

        // Light up 1, 2, or 3 diamonds based on rarity.
        // Common = 1 gold diamond, Rare = 2, Legendary = 3.
        int starCount = response.item.rarity switch { "Legendary" => 3, "Rare" => 2, _ => 1 };
        if (starImage1 != null) starImage1.color = starCount >= 1 ? ColStarActive : ColStarInactive;
        if (starImage2 != null) starImage2.color = starCount >= 2 ? ColStarActive : ColStarInactive;
        if (starImage3 != null) starImage3.color = starCount >= 3 ? ColStarActive : ColStarInactive;

        // Display the item's name.
        if (itemNameText != null)
            itemNameText.text = response.item.name;

        // Update the rarity badge label ("COMMON", "RARE", "LEGENDARY").
        if (rarityBadgeText != null)
            rarityBadgeText.text = response.item.rarity.ToUpper();

        // Update the rarity badge background color to match the rarity tier.
        if (rarityBadgeImage != null)
            rarityBadgeImage.color = response.item.rarity switch
            {
                "Legendary" => ColLegendary,
                "Rare"      => ColRare,
                _           => ColCommon
            };

        // Swap item sprite based on the pulled item's name.
        if (itemImage != null)
        {
            itemImage.sprite = response.item.name switch
            {
                "Mangrove Seed"          => spriteMangrove,
                "Coral Fragment"         => spriteCoral,
                "Giant Sea Turtle Shell" => spriteTurtle,
                _                        => null
            };
            itemImage.enabled = itemImage.sprite != null;
        }

        // Update the coin balance shown in the header.
        AuthManager.Coins = response.newBalance;
        if (balanceText != null)
            balanceText.text = $"Eco-Coins: {response.newBalance}";

        // Clear the "Connecting to nature registry..." status text.
        if (statusText != null) statusText.text = "";
    }

    // Enables or disables the pull button (used to block input during a request).
    private void SetUIInteractivity(bool isInteractable)
    {
        if (pullButton != null) pullButton.interactable = isInteractable;
    }

    // Handles the back button click — returns to the hub panel.
    private void OnBackClicked()
    {
        if (gachaPanel != null) gachaPanel.SetActive(false);
        if (hubPanel   != null) hubPanel.SetActive(true);
    }
}
