using System;

// --- Request Payloads (Unity to Express) ---

[Serializable]
public class RegisterRequest
{
    public string email;
    public string password;
}

[Serializable]
public class LoginRequest
{
    public string email;
    public string password;
}

// --- Response Payloads (Express to Unity) ---

[Serializable]
public class AuthResponse
{
    public string message;
    public string token; // the session JWT string returned on a successful login
    public string userId;
    public int coins;
    public string error; // populated only when the response status is 400 or 500
}