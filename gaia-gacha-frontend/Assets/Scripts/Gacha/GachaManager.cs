using System;
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

    [Header("UI - Card")]
    [SerializeField] private CardDisplay cardDisplay;

    [Header("Data")]
    [SerializeField] private ItemRegistry itemRegistry;

    [Header("Navigation")]
    [SerializeField] private GameObject gachaPanel;
    [SerializeField] private GameObject hubPanel;
    [SerializeField] private Button     backButton;

    private bool isPulling = false;

    void Start()
    {
        if (statusText  != null) statusText.text  = "";
        if (balanceText != null) balanceText.text = $"Eco-Coins: {AuthManager.Coins}";
        if (cardDisplay != null) cardDisplay.ResetToDefault();

        if (pullButton != null)
            pullButton.onClick.AddListener(OnPullButtonClicked);
        else
            Debug.LogError("[GachaManager] Pull Button reference is missing.");

        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
        else
            Debug.LogWarning("[GachaManager] Back Button reference is missing.");
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
            request.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonPayload));
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
        var visuals = itemRegistry.FindById(response.item.id);
        var rarity  = Enum.Parse<Rarity>(response.item.rarity);
        cardDisplay.Setup(response.item.name, rarity, visuals);
        AuthManager.Coins = response.newBalance;
        if (balanceText != null) balanceText.text = $"Eco-Coins: {response.newBalance}";
        if (statusText  != null) statusText.text  = "";
    }

    private void SetUIInteractivity(bool isInteractable)
    {
        if (pullButton != null) pullButton.interactable = isInteractable;
    }

    private void OnBackClicked()
    {
        if (gachaPanel != null) gachaPanel.SetActive(false);
        if (hubPanel   != null) hubPanel.SetActive(true);
    }
}
