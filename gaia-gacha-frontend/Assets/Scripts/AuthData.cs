using System;

// ========================================================
// REQUEST PAYLOADS (Data going from Unity -> Express)
// ========================================================

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

// ========================================================
// RESPONSE PAYLOADS (Data coming from Express -> Unity)
// ========================================================

[Serializable]
public class AuthResponse
{
    public string message;
    public string token;   // Captures JWT passport string
    public string userId;  // Captures the player's unique UUID
    public int coins;      // Captures the player's coin balance
    public string error;   // Captures error messages if status is 400/500
}