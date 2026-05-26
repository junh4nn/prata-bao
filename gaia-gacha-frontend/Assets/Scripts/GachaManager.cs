using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class GachaManager : MonoBehaviour
{
    [Header("Backend Configuration")]
    [SerializeField] private string backendUrl = "http://localhost:3000/api/gacha/pull";
    [Tooltip("Paste a UUID here to bypass login for testing.")]
    [SerializeField] private string testUserId = "";

    [Header("UI - Pull Controls")]
    [SerializeField] private Button pullButton;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI balanceText;

    [Header("UI - Item Card")]
    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private Image rarityBadgeImage;
    [SerializeField] private TextMeshProUGUI rarityBadgeText;
    [SerializeField] private GameObject defaultCardState;
    [SerializeField] private GameObject revealedCardState;

    private bool isPulling = false;

    static readonly Color ColCommon    = new Color(0.659f, 0.710f, 0.635f);
    static readonly Color ColRare      = new Color(0.322f, 0.718f, 0.533f);
    static readonly Color ColLegendary = new Color(0.914f, 0.769f, 0.404f);

    void Start()
    {
        if (statusText != null) statusText.text = "";
        if (balanceText != null) balanceText.text = "Eco-Coins: --";
        if (defaultCardState != null) defaultCardState.SetActive(true);
        if (revealedCardState != null) revealedCardState.SetActive(false);

        if (pullButton != null)
            pullButton.onClick.AddListener(OnPullButtonClicked);
        else
            Debug.LogError("[GachaManager] Pull Button reference is missing.");
    }

    public void OnPullButtonClicked()
    {
        if (isPulling) return;

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

    private IEnumerator SendPullRequest(string userId)
    {
        isPulling = true;
        SetUIInteractivity(false);
        if (statusText != null) statusText.text = "Connecting to nature registry...";

        string jsonPayload = JsonUtility.ToJson(new PullRequest { userId = userId });

        using (UnityWebRequest request = new UnityWebRequest(backendUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonPayload));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                string err = request.downloadHandler?.text ?? request.error;
                Debug.LogError($"[GachaManager] Backend error: {err}");
                if (statusText != null) statusText.text = "<color=red>Pull failed. Check server logs.</color>";
            }
            else
            {
                UpdateGachaUI(JsonUtility.FromJson<PullResponse>(request.downloadHandler.text));
            }
        }

        isPulling = false;
        SetUIInteractivity(true);
    }

    private void UpdateGachaUI(PullResponse response)
    {
        if (response?.item == null) return;

        if (defaultCardState != null) defaultCardState.SetActive(false);
        if (revealedCardState != null) revealedCardState.SetActive(true);

        if (starsText != null)
            starsText.text = response.item.rarity switch
            {
                "Legendary" => "★ ★ ★",
                "Rare"      => "★ ★",
                _           => "★"
            };

        if (itemNameText != null)
            itemNameText.text = response.item.name;

        if (rarityBadgeText != null)
            rarityBadgeText.text = response.item.rarity.ToUpper();

        if (rarityBadgeImage != null)
            rarityBadgeImage.color = response.item.rarity switch
            {
                "Legendary" => ColLegendary,
                "Rare"      => ColRare,
                _           => ColCommon
            };

        if (balanceText != null)
            balanceText.text = $"Eco-Coins: {response.newBalance}";

        if (statusText != null) statusText.text = "";
    }

    private void SetUIInteractivity(bool isInteractable)
    {
        if (pullButton != null) pullButton.interactable = isInteractable;
    }
}
