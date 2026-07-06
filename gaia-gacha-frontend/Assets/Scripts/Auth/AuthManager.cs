// AuthManager.cs: handles registration and login API calls against the Express backend.
// Sits alongside AuthUIManager.cs, which reacts to the results this script produces.

using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    private string registerUrl => $"{BackendConfig.Instance.baseUrl}/api/auth/register";
    private string loginUrl => $"{BackendConfig.Instance.baseUrl}/api/auth/login";

    // Static so any script in the scene can read the current player's token
    // instantly, without needing a wired reference to this component.
    public static string Token { get; private set; }
    public static int    Coins    { get; set; }
    public static bool IsLoggedIn => !string.IsNullOrEmpty(Token);

    public static void ClearSession()
    {
        Token  = null;
        Coins  = 0;
    }

    public void Register(string email, string password, Action onSuccess, Action<string> onError = null)
    {
        StartCoroutine(SendAuthRequest(registerUrl, email, password, isLogin: false, onSuccess, onError));
    }

    public void Login(string email, string password, Action onSuccess, Action<string> onError = null)
    {
        StartCoroutine(SendAuthRequest(loginUrl, email, password, isLogin: true, onSuccess, onError));
    }

    private IEnumerator SendAuthRequest(string url, string email, string password, bool isLogin, Action onSuccess, Action<string> onError)
    {
        // 1. Build the Request Payload
        string jsonPayload = isLogin
            ? JsonUtility.ToJson(new LoginRequest { email = email, password = password })
            : JsonUtility.ToJson(new RegisterRequest { email = email, password = password });

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonPayload));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 10; // fail after 10 seconds instead of hanging forever

            // 2. Send the Request
            Debug.Log($"[AuthManager] Sending request to {url}...");
            yield return request.SendWebRequest();

            Debug.Log($"[AuthManager] Response received. Result: {request.result}, Code: {request.responseCode}");

            // 3. Bail Out Early on a Network-Level Failure
            if (request.result != UnityWebRequest.Result.Success)
            {
                string err = request.error ?? "Unknown network error";
                Debug.LogError($"[AuthManager] Request failed: {err}");
                onError?.Invoke(err);
                yield break;
            }

            // 4. Parse the Response Body
            string jsonResponse = request.downloadHandler.text;
            AuthResponse responseData = JsonUtility.FromJson<AuthResponse>(jsonResponse);

            // 5. Report Success or Failure (capturing the session on login)
            if (request.responseCode == 200 || request.responseCode == 201)
            {
                Debug.Log($"<color=green>[AuthManager] Success: {responseData.message}</color>");

                if (isLogin)
                {
                    Token = responseData.token;
                    Coins = responseData.coins;
                }

                onSuccess?.Invoke();
            }
            else
            {
                string err = responseData?.error ?? $"Server error {request.responseCode}";
                Debug.LogError($"[AuthManager] Server rejected ({request.responseCode}): {err}");
                onError?.Invoke(err);
            }
        }
    }
}