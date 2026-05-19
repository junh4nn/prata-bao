using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GachaManager : MonoBehaviour
{
    private string backendUrl = "http://localhost:3000/pull"; 

    void Start()
    {
        // For testing purposes, a pull is automatically triggered when the game starts.
        string testUserId = "5102101b-7c13-4c06-b9f3-940afbdb46d1"; 
        
        StartCoroutine(SendPullRequest(testUserId));
    }

    IEnumerator SendPullRequest(string userId)
    {
        // 1. Create the request object and convert it to a JSON string
        PullRequest requestData = new PullRequest { userId = userId };
        string jsonPayload = JsonUtility.ToJson(requestData);

        // 2. Set up the UnityWebRequest for a POST method
        using (UnityWebRequest request = new UnityWebRequest(backendUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            // Crucial: Tell Express server that JSON data is being sent
            request.SetRequestHeader("Content-Type", "application/json");

            // 3. Send the request to Express and wait for it to finish
            yield return request.SendWebRequest();

            // 4. Handle the response
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error pulling from backend: {request.error}");
                Debug.LogError($"Server response: {request.downloadHandler.text}");
            }
            else
            {
                // 5. Parse the successful JSON response back into a C# object
                string jsonResponse = request.downloadHandler.text;
                
                PullResponse responseData = JsonUtility.FromJson<PullResponse>(jsonResponse);

                Debug.Log($"Successfully pulled! You won: {responseData.item.name} ({responseData.item.rarity})");
                Debug.Log($"Remaining Coins: {responseData.newBalance}");
                
                // TODO soon: Trigger Unity UI animations here
            }
        }
    }
}