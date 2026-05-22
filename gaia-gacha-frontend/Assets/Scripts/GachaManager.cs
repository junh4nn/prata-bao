using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI; 
using TMPro;           

public class GachaManager : MonoBehaviour
{
    [Header("Backend Configuration")]
    [SerializeField] private string backendUrl = "http://localhost:3000/pull";
    [SerializeField] private string testUserId = "5102101b-7c13-4c06-b9f3-940afbdb46d1";

    [Header("UI References")]
    [SerializeField] private Button pullButton;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI balanceText;

    private bool isPulling = false;

    void Start()
    {
        // Clear any placeholder text at startup
        if (resultText != null) resultText.text = "Ready to discover your ecosystem!";
        if (balanceText != null) balanceText.text = "Eco-Coins:";
        
        // Explicitly hook up the button listener via code
        if (pullButton != null)
        {
            pullButton.onClick.AddListener(OnPullButtonClicked);
        }
        else
        {
            Debug.LogError("GachaManager is missing a reference to the Pull Button!");
        }
    }

    // Public method triggered by the UI Button component.
    public void OnPullButtonClicked()
    {
        // Guard clause: Prevent spamming while waiting for the server authoritative response
        if (isPulling) return;

        StartCoroutine(SendPullRequest(testUserId));
    }

    private IEnumerator SendPullRequest(string userId)
    {
        isPulling = true;
        SetUIInteractivity(false);
        
        if (resultText != null) resultText.text = "Connecting to nature registry...";

        PullRequest requestData = new PullRequest { userId = userId };
        string jsonPayload = JsonUtility.ToJson(requestData);

        using (UnityWebRequest request = new UnityWebRequest(backendUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error pulling from backend: {request.error}");
                if (resultText != null) resultText.text = "<color=red>Connection failed. Try again.</color>";
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                PullResponse responseData = JsonUtility.FromJson<PullResponse>(jsonResponse);

                // Update UI elements with the backend's authoritative response data
                UpdateGachaUI(responseData);
            }
        }

        isPulling = false;
        SetUIInteractivity(true);
    }

    private void UpdateGachaUI(PullResponse response)
    {
        if (response == null || response.item == null) return;

        // Display reward with custom rich text formatting based on environmental theme rarities
        if (resultText != null)
        {
            resultText.text = $"<b>Discovered:</b> {response.item.name}\n" +
                              $"<size=80%>Rarity: {response.item.rarity}</size>";
        }

        // Update the global wallet balance
        if (balanceText != null)
        {
            balanceText.text = $"Eco-Coins: {response.newBalance}";
        }
    }

    private void SetUIInteractivity(bool isInteractable)
    {
        // Disables the button visually and functionally while the coroutine runs
        if (pullButton != null)
        {
            pullButton.interactable = isInteractable;
        }
    }
}