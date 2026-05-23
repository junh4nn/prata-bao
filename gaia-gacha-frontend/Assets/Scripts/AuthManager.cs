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
    public void Register(string email, string password)
    {
        StartCoroutine(SendAuthRequest(registerUrl, email, password, isLogin: false, null));
    }

    // Logs in an existing user and captures their JWT session key.
    public void Login(string email, string password, Action onSuccess)
    {
        StartCoroutine(SendAuthRequest(loginUrl, email, password, isLogin: true, onSuccess));
    }

    private IEnumerator SendAuthRequest(string url, string email, string password, bool isLogin, Action onSuccess)
    {
        // 1. Pack data into JSON string
        string jsonPayload = isLogin 
            ? JsonUtility.ToJson(new LoginRequest { email = email, password = password })
            : JsonUtility.ToJson(new RegisterRequest { email = email, password = password });

        // 2. Set up raw network payload over HTTP POST
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            
            // Critical header tells Express we are transmitting a JSON payload object
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log($"[AuthManager] Sending request to {url}...");
            yield return request.SendWebRequest();

            // 3. Process Network Response
            if (request.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError($"[AuthManager] Network Connection Error: {request.error}");
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                AuthResponse responseData = JsonUtility.FromJson<AuthResponse>(jsonResponse);

                if (request.responseCode == 200 || request.responseCode == 201)
                {
                    Debug.Log($"<color=green>[AuthManager] Success: {responseData.message}</color>");

                    if (isLogin)
                    {
                        // KEY SAVER: Safely stash passport details directly inside static memory layout
                        Token = responseData.token;
                        UserId = responseData.userId;

                        Debug.Log($"[AuthManager] JWT Passport Captured! User UUID: {UserId}");

                        onSuccess?.Invoke();
                    }
                }
                else
                {
                    // Handles controlled server rejections like invalid credentials or duplicate emails
                    Debug.LogError($"[AuthManager] Server Rejected Request ({request.responseCode}): {responseData.error}");
                }
            }
        }
    }
}