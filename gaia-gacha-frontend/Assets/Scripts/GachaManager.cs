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

    // DEVELOPMENT TEST FIELD
    // Leave this completely blank to use normal login details. 
    // Paste an explicit Supabase UUID here to bypass authentication entirely for local testing!
    [Tooltip("Bypass login by pasting a specific UUID here for database lookup testing.")]
    [SerializeField] private string testUserId = "";

    [Header("UI References")]
    [SerializeField] private Button pullButton;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TextMeshProUGUI balanceText;

    private bool isPulling = false;

    void Start()
    {
        if (resultText != null) resultText.text = "Ready to discover your ecosystem!";
        if (balanceText != null) balanceText.text = "Eco-Coins:";
        
        if (pullButton != null)
        {
            pullButton.onClick.AddListener(OnPullButtonClicked);
        }
        else
        {
            Debug.LogError("GachaManager is missing a reference to the Pull Button!");
        }
    }

    public void OnPullButtonClicked()
    {
        if (isPulling) return;

        // 1. Either use the true logged-in user UUID or the test user UUID
        string activeUserId = "";

        if (!string.IsNullOrEmpty(testUserId))
        {
            activeUserId = testUserId.Trim();
            Debug.Log($"[GachaManager] Inspector Test ID detected. Bypassing Auth. Target UUID: {activeUserId}");
        }
        else
        {
            activeUserId = AuthManager.UserId;
            Debug.Log($"[GachaManager] No Test ID. Fetching regular login credentials. Active UUID: {activeUserId}");
        }

        // 2. Clear Guard Clause: If they aren't logged in, block the request completely
        if (string.IsNullOrEmpty(activeUserId))
        {
            Debug.LogError("[GachaManager] Pull blocked! No player is currently logged in.");
            if (resultText != null) 
            {
                resultText.text = "<color=red>Error: Please log in first!</color>";
            }
            return;
        }

        // 3. Fire the request with the true account ID
        StartCoroutine(SendPullRequest(activeUserId));
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
                // Read exact error message body returned by your Express server
                string serverError = request.downloadHandler != null ? request.downloadHandler.text : request.error;
                Debug.LogError($"Backend Error: {serverError}");
                
                if (resultText != null) 
                {
                    resultText.text = "<color=red>Pull failed. Check server logs.</color>";
                }
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                PullResponse responseData = JsonUtility.FromJson<PullResponse>(jsonResponse);

                UpdateGachaUI(responseData);
            }
        }

        isPulling = false;
        SetUIInteractivity(true);
    }

    private void UpdateGachaUI(PullResponse response)
    {
        if (response == null || response.item == null) return;

        if (resultText != null)
        {
            resultText.text = $"<b>Discovered:</b> {response.item.name}\n" +
                              $"<size=80%>Rarity: {response.item.rarity}</size>";
        }

        if (balanceText != null)
        {
            balanceText.text = $"Eco-Coins: {response.newBalance}";
        }
    }

    private void SetUIInteractivity(bool isInteractable)
    {
        if (pullButton != null)
        {
            pullButton.interactable = isInteractable;
        }
    }
}