// AuthUIManager.cs — controls what the login/register screen looks like and responds to.
// Sits alongside AuthManager.cs on the same GameObject.
// AuthManager handles the actual API calls; this script handles the UI reactions.

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AuthUIManager : MonoBehaviour
{
    // ── Inspector references ─────────────────────────────────────────────────
    // These are wired automatically by SceneBuilder.cs — no manual dragging needed.

    [Header("Dependencies")]
    [SerializeField] private AuthManager authManager; // handles the actual login/register API calls

    [Header("UI Pages / Panels")]
    [SerializeField] private GameObject authPanel; // the login screen (shown at start)
    [SerializeField] private GameObject hubPanel;  // the hub screen (shown after login)

    [Header("UI Input Fields")]
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    [Header("UI Interaction Buttons")]
    [SerializeField] private Button actionButton;             // the main "Sign In" / "Create Account" button
    [SerializeField] private TextMeshProUGUI actionButtonText; // the label on that button
    [SerializeField] private Button toggleModeButton;             // the "Don't have an account?" link
    [SerializeField] private TextMeshProUGUI toggleModeText;      // the label on that link

    [Header("Status Feedback")]
    [SerializeField] private TextMeshProUGUI statusText; // shows errors and progress messages

    // Tracks whether we're in login mode (true) or register mode (false).
    private bool isLoginMode = true;

    // ── Unity lifecycle ──────────────────────────────────────────────────────

    void Start()
    {
        // If AuthManager wasn't wired, try to find it automatically in the scene.
        if (authManager == null)
        {
            authManager = FindAnyObjectByType<AuthManager>();
            if (authManager == null) Debug.LogError("[AuthUIManager] AuthManager not found in scene!");
        }

        // Register button click listeners.
        if (actionButton != null) actionButton.onClick.AddListener(OnActionClicked);
        if (toggleModeButton != null) toggleModeButton.onClick.AddListener(OnToggleModeClicked);

        // Start on the auth screen with the hub panel hidden.
        if (authPanel != null) authPanel.SetActive(true);
        if (hubPanel  != null) hubPanel.SetActive(false);

        // Set the initial button/link text to match login mode.
        UpdateModeUI();
    }

    // ── Button handlers ──────────────────────────────────────────────────────

    // Called when the player taps the main action button ("Sign In" or "Create Account").
    private void OnActionClicked()
    {
        string email    = emailInputField    != null ? emailInputField.text.Trim() : "";
        string password = passwordInputField != null ? passwordInputField.text     : "";

        // Don't attempt an API call if the inputs are invalid.
        if (!ValidateInputs(email, password)) return;

        if (isLoginMode)
        {
            if (statusText != null) statusText.text = "<color=#E9C46A>Logging in...</color>";
            authManager.Login(email, password, SwapToHubPage);
        }
        else
        {
            if (statusText != null) statusText.text = "<color=#E9C46A>Creating account...</color>";
            authManager.Register(email, password, OnRegisterSuccess);
        }
    }

    // Called when the player taps the "Don't have an account? Register" link.
    // Flips between login and register mode.
    private void OnToggleModeClicked()
    {
        isLoginMode = !isLoginMode;
        if (statusText != null) statusText.text = ""; // clear any previous error message
        UpdateModeUI();
    }

    // ── UI state ─────────────────────────────────────────────────────────────

    // Updates the button label and toggle link text to match the current mode.
    private void UpdateModeUI()
    {
        if (actionButtonText != null)
            actionButtonText.text = isLoginMode ? "Sign In" : "Create Account";

        if (toggleModeText != null)
            toggleModeText.text = isLoginMode
                ? "Don't have an account?\n<b>Register</b>"
                : "Already have an account?\n<b>Sign In</b>";
    }

    // Called after successful registration — switches to login mode with a confirmation message.
    private void OnRegisterSuccess()
    {
        isLoginMode = true;
        UpdateModeUI();
        if (statusText != null) statusText.text = "<color=#57C278>Account created! Please sign in.</color>";
    }

    // Clears the form fields and status text — called on logout.
    public void ClearForm()
    {
        if (emailInputField    != null) emailInputField.text    = "";
        if (passwordInputField != null) passwordInputField.text = "";
        if (statusText         != null) statusText.text         = "";
    }

    // Hides the auth screen and shows the hub screen.
    // Called by AuthManager after a successful login.
    public void SwapToHubPage()
    {
        if (authPanel != null) authPanel.SetActive(false);
        if (hubPanel  != null) hubPanel.SetActive(true);
    }

    // ── Input validation ─────────────────────────────────────────────────────

    // Returns true if the inputs are acceptable, false + shows an error if not.
    private bool ValidateInputs(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            if (statusText != null) statusText.text = "<color=red>Fields cannot be empty!</color>";
            return false;
        }
        if (password.Length < 6)
        {
            if (statusText != null) statusText.text = "<color=red>Password must be at least 6 characters.</color>";
            return false;
        }
        return true;
    }
}
