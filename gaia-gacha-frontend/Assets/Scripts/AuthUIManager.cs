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
    [SerializeField] private Button actionButton;
    [SerializeField] private TextMeshProUGUI actionButtonText;
    [SerializeField] private Button toggleModeButton;
    [SerializeField] private TextMeshProUGUI toggleModeText;

    [Header("Status Feedback")]
    [SerializeField] private TextMeshProUGUI statusText;

    private bool isLoginMode = true;

    void Start()
    {
        if (authManager == null)
        {
            authManager = FindAnyObjectByType<AuthManager>();
            if (authManager == null) Debug.LogError("[AuthUIManager] AuthManager not found in scene!");
        }

        if (actionButton != null) actionButton.onClick.AddListener(OnActionClicked);
        if (toggleModeButton != null) toggleModeButton.onClick.AddListener(OnToggleModeClicked);

        if (authPanel != null) authPanel.SetActive(true);
        if (gachaPanel != null) gachaPanel.SetActive(false);

        UpdateModeUI();
    }

    private void OnActionClicked()
    {
        string email = emailInputField != null ? emailInputField.text.Trim() : "";
        string password = passwordInputField != null ? passwordInputField.text : "";

        if (!ValidateInputs(email, password)) return;

        if (isLoginMode)
        {
            if (statusText != null) statusText.text = "<color=#E9C46A>Logging in...</color>";
            authManager.Login(email, password, SwapToGachaPage);
        }
        else
        {
            if (statusText != null) statusText.text = "<color=#E9C46A>Creating account...</color>";
            authManager.Register(email, password);
        }
    }

    private void OnToggleModeClicked()
    {
        isLoginMode = !isLoginMode;
        if (statusText != null) statusText.text = "";
        UpdateModeUI();
    }

    private void UpdateModeUI()
    {
        if (actionButtonText != null)
            actionButtonText.text = isLoginMode ? "Sign In" : "Create Account";

        if (toggleModeText != null)
            toggleModeText.text = isLoginMode
                ? "Don't have an account? <b>Register</b>"
                : "Already have an account? <b>Sign In</b>";
    }

    public void SwapToGachaPage()
    {
        if (authPanel != null) authPanel.SetActive(false);
        if (gachaPanel != null) gachaPanel.SetActive(true);
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
