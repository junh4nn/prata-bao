using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AuthUIManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private AuthManager authManager;

    [Header("UI Pages / Panels")]
    [SerializeField] private GameObject authPanel;
    [SerializeField] private GameObject gachaPanel;

    [Header("UI Input Fields")]
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    [Header("UI Interaction Buttons")]
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;

    [Header("Status Feedback")]
    [SerializeField] private TextMeshProUGUI statusText;

    void Start()
    {
        // Clear any placeholder error/status text at startup
        if (statusText != null) statusText.text = "Welcome! Please log in or register.";

        // Explicitly wire up our button click events via code
        if (loginButton != null) loginButton.onClick.AddListener(OnLoginClicked);
        if (registerButton != null) registerButton.onClick.AddListener(OnRegisterClicked);
        
        // Safety check to ensure we didn't forget the manager connection
        if (authManager == null)
        {
            authManager = FindAnyObjectByType<AuthManager>();
            if (authManager == null) Debug.LogError("AuthUIManager is missing a reference to AuthManager!");
        }

        if (authPanel != null) authPanel.SetActive(true);
        if (gachaPanel != null) gachaPanel.SetActive(false);
    }

    private void OnLoginClicked()
    {
        string email = emailInputField.text.Trim();
        string password = passwordInputField.text;

        if (ValidateInputs(email, password))
        {
            statusText.text = "<color=yellow>Logging in...</color>";
            authManager.Login(email, password); // We pass a callback method down to the network manager

            // SwapToGachaPage();
        }
    }

    private void OnRegisterClicked()
    {
        string email = emailInputField.text.Trim();
        string password = passwordInputField.text;

        if (ValidateInputs(email, password))
        {
            statusText.text = "<color=yellow>Creating account...</color>";
            authManager.Register(email, password);
        }
    }
    
    public void SwapToGachaPage()
    {
        if (authPanel != null) authPanel.SetActive(false); // Hides login page
        if (gachaPanel != null) gachaPanel.SetActive(true);  // Reveals pulling game interface
    }

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