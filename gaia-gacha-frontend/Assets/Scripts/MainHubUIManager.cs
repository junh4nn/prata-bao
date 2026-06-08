using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainHubUIManager : MonoBehaviour
{
    [Header("UI - Header")]
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private Button          logoutButton;

    [Header("UI - Hero Card")]
    [SerializeField] private TextMeshProUGUI quoteText;

    [Header("UI - Navigation Tiles")]
    [SerializeField] private Button gachaButton;
    [SerializeField] private Button quizButton;
    [SerializeField] private Button inventoryButton;

    [Header("Panels")]
    [SerializeField] private GameObject hubPanel;
    [SerializeField] private GameObject authPanel;
    [SerializeField] private GameObject gachaPanel;
    // quizPanel and inventoryPanel intentionally absent — panels not yet built

    static readonly string[] Quotes = {
        "\"In every walk with nature, one receives\nfar more than he seeks.\"",
        "\"The earth does not belong to us.\nWe belong to the earth.\"",
        "\"What we are doing to the forests of the world\nis but a mirror of what we do to ourselves.\"",
        "\"The environment is where we all meet;\nwhere we all have a mutual interest.\"",
        "\"We do not inherit the earth from our ancestors;\nwe borrow it from our children.\"",
        "\"Look deep into nature, and then\nyou will understand everything better.\""
    };

    static readonly string[] Authors = {
        "— John Muir",
        "— Chief Seattle",
        "— Mahatma Gandhi",
        "— Lady Bird Johnson",
        "— Antoine de Saint-Exupéry",
        "— Albert Einstein"
    };

    void Start()
    {
        // Wire button handlers (runs once).
        if (gachaButton     != null) gachaButton.onClick.AddListener(OnGachaClicked);
        if (quizButton      != null) quizButton.onClick.AddListener(OnQuizClicked);
        if (inventoryButton != null) inventoryButton.onClick.AddListener(OnInventoryClicked);
        if (logoutButton    != null) logoutButton.onClick.AddListener(OnLogoutClicked);

        // Pick a random quote for this session.
        int idx = Random.Range(0, Quotes.Length);
        if (quoteText != null) quoteText.text = Quotes[idx] + "\n" + Authors[idx];
    }

    void OnEnable()
    {
        // Refresh coin balance every time the hub becomes visible (e.g. returning from gacha).
        if (coinsText != null)
            coinsText.text = $"✦ {AuthManager.Coins} ECO-COINS";
    }

    private void OnGachaClicked()
    {
        if (hubPanel   != null) hubPanel.SetActive(false);
        if (gachaPanel != null) gachaPanel.SetActive(true);
    }

    private void OnQuizClicked()
    {
        // Quiz panel not yet built — no-op until wired by SceneBuilder.
        Debug.Log("[MainHubUIManager] Quiz panel not yet available.");
    }

    private void OnInventoryClicked()
    {
        // Inventory panel not yet built — no-op until wired by SceneBuilder.
        Debug.Log("[MainHubUIManager] Inventory panel not yet available.");
    }

    private void OnLogoutClicked()
    {
        AuthManager.ClearSession();
        if (hubPanel  != null) hubPanel.SetActive(false);
        if (authPanel != null) authPanel.SetActive(true);
    }
}
