using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    [Header("Backend Endpoints")]
    [SerializeField] private string registerUrl = "http://localhost:3000/api/auth/register";
    [SerializeField] private string loginUrl = "http://localhost:3000/api/auth/login";

    // Static properties allows any other script in your game 
    // to read the current player's token instantly without manual linking.
    public static string Token { get; private set; }
    public static string UserId { get; private set; }
    public static bool IsLoggedIn => !string.IsNullOrEmpty(Token);

    // Registers a new user account on the Express backend.
    public void Register(string email, string password, Action onSuccess, Action<string> onError = null)
    {
        StartCoroutine(SendAuthRequest(registerUrl, email, password, isLogin: false, onSuccess, onError));
    }

    // Logs in an existing user and captures their JWT session key.
    public void Login(string email, string password, Action onSuccess, Action<string> onError = null)
    {
        StartCoroutine(SendAuthRequest(loginUrl, email, password, isLogin: true, onSuccess, onError));
    }

    private IEnumerator SendAuthRequest(string url, string email, string password, bool isLogin, Action onSuccess, Action<string> onError)
    {
        string jsonPayload = isLogin
            ? JsonUtility.ToJson(new LoginRequest { email = email, password = password })
            : JsonUtility.ToJson(new RegisterRequest { email = email, password = password });

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonPayload));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.timeout = 10; // fail after 10 seconds instead of hanging forever

            Debug.Log($"[AuthManager] Sending request to {url}...");
            yield return request.SendWebRequest();

            Debug.Log($"[AuthManager] Response received. Result: {request.result}, Code: {request.responseCode}");

            if (request.result != UnityWebRequest.Result.Success)
            {
                string err = request.error ?? "Unknown network error";
                Debug.LogError($"[AuthManager] Request failed: {err}");
                onError?.Invoke(err);
                yield break;
            }

            string jsonResponse = request.downloadHandler.text;
            Debug.Log($"[AuthManager] Response body: {jsonResponse}");
            AuthResponse responseData = JsonUtility.FromJson<AuthResponse>(jsonResponse);

            if (request.responseCode == 200 || request.responseCode == 201)
            {
                Debug.Log($"<color=green>[AuthManager] Success: {responseData.message}</color>");

                if (isLogin)
                {
                    Token  = responseData.token;
                    UserId = responseData.userId;
                    Debug.Log($"[AuthManager] JWT captured. UserId: {UserId}");
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