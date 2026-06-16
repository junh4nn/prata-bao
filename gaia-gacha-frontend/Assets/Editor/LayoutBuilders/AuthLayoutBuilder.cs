using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class AuthLayoutBuilder {

    static TMP_FontAsset s_Poppins;
    static TMP_FontAsset s_Cinzel;

    
     // ── Auth Panel ───────────────────────────────────────────────────────────
    // Builds the login/register screen.
    // Layout (top to bottom):
    //   LogoArea  — icon square + "GAIAGACHA" title + subtitle
    //   FormCard  — email input, password input, Sign In button, Register link
    //   StatusText — error/feedback messages shown below the form


    [MenuItem("GaiaGacha/LayoutBuilders/Build Auth Panel", priority = 200)]
    static void Build()
    {
        s_Poppins = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Poppins-SemiBold SDF.asset");
        s_Cinzel  = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Cinzel-Regular SDF.asset");

        var temp   = new GameObject("Temp");
        var panelGo = BuildAuthPanel(temp.transform);

        PrefabUtility.SaveAsPrefabAsset(panelGo, "Assets/Prefabs/AuthPanel.prefab");
        Object.DestroyImmediate(temp);
        AssetDatabase.Refresh();
        Debug.Log("<color=green>[AuthLayoutBuilder] AuthPanel prefab saved.</color>");
    }

    static GameObject BuildAuthPanel(Transform parent)
    {
        // The auth panel fills the whole screen so elements can be placed anywhere.
        var panel = UIConstants.MakeRect(parent, "AuthPanel");
        UIConstants.Stretch(panel);

            // Logo area: centered horizontally, positioned in the upper portion of the screen.
            // anchoredPosition y=210 pushes it above the screen center.
            var logoArea = UIConstants.MakeRect(panel.transform, "LogoArea");
            UIConstants.SetAnchored(logoArea, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(340, 160));
            logoArea.anchoredPosition = new Vector2(0, 210);

            // Logo image loaded from Assets/Sprites/GaiaGacha.jpg.
            var logoIcon = UIConstants.MakeImage(logoArea.transform, "LogoIcon", null, Color.white);
            UIConstants.SetAnchored(logoIcon.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(120, 150));
            logoIcon.rectTransform.anchoredPosition = new Vector2(0, 20);
            var logoSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/logo.png");
            if (logoSprite != null) logoIcon.sprite = logoSprite;
            else Debug.LogWarning("[SceneBuilder] Logo not found at Assets/Sprites/logo.png");

            // Main game title in Cinzel (the decorative font).
            var titleTmp = UIConstants.MakeTMP(logoArea.transform, "TitleText", "GAIAGACHA", 42, UIConstants.ColTextPrimary, FontStyles.Bold, s_Cinzel);
            UIConstants.SetAnchored(titleTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(320, 52));
            titleTmp.rectTransform.anchoredPosition = new Vector2(0, -88);
            titleTmp.alignment = TextAlignmentOptions.Center;

            // Spaced-caps tagline below the title.
            var subtitleTmp = UIConstants.MakeTMP(logoArea.transform, "SubtitleText", "DISCOVER  ·  PULL  ·  COLLECT", 13, UIConstants.ColTextMuted, FontStyles.Normal, s_Poppins);
            UIConstants.SetAnchored(subtitleTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(300, 24));
            subtitleTmp.rectTransform.anchoredPosition = new Vector2(0, -136);
            subtitleTmp.alignment = TextAlignmentOptions.Center;
            subtitleTmp.characterSpacing = 4; // extra spacing for the spaced-caps look

            // Form card: the dark green box containing the inputs and button.
            // Centered on screen, slightly below the midpoint (y=-60).
            var formCard = UIConstants.MakeImage(panel.transform, "FormCard", null, UIConstants.ColSurface);
            UIConstants.SetAnchored(formCard.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(320, 340));
            formCard.rectTransform.anchoredPosition = new Vector2(0, -60);

            // "Email address" label above the email input.
            var emailLabel = UIConstants.MakeTMP(formCard.transform, "EmailLabel", "Email address", 12, UIConstants.ColTextMuted, FontStyles.Normal, s_Poppins);
            UIConstants.SetAnchored(emailLabel.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 20));
            emailLabel.rectTransform.anchoredPosition = new Vector2(0, -20);

            // Email input field — not a password field (isPassword = false).
            var emailInput = UIConstants.MakeInputField(formCard.transform, "EmailInput", "your@email.com", false, s_Poppins);
            UIConstants.SetAnchored(emailInput.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 52));
            emailInput.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60);

            // "Password" label above the password input.
            var passwordLabel = UIConstants.MakeTMP(formCard.transform, "PasswordLabel", "Password", 12, UIConstants.ColTextMuted, FontStyles.Normal, s_Poppins);
            UIConstants.SetAnchored(passwordLabel.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 20));
            passwordLabel.rectTransform.anchoredPosition = new Vector2(0, -106);

            // Password input field — characters are hidden (isPassword = true).
            var passwordInput = UIConstants.MakeInputField(formCard.transform, "PasswordInput", "••••••••", true, s_Poppins);
            UIConstants.SetAnchored(passwordInput.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 52));
            passwordInput.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -146);

            // Primary action button — label is "Sign In" by default.
            // AuthUIManager.cs changes it to "Create Account" when toggled to register mode.
            var (actionBtnGo, _) = UIConstants.MakeButton(formCard.transform, "ActionButton", "Sign In", 20, s_Poppins);
            UIConstants.SetAnchored(actionBtnGo.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 62));
            actionBtnGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -216);

            // Toggle link — tapping this switches between login and register mode.
            // Uses \n to put "Register" on its own line below the question text.
            var (toggleBtnGo, _) = UIConstants.MakeLinkButton(formCard.transform, "ToggleModeButton", "Don't have an account?\n<b>Register</b>", 14, s_Poppins);
            UIConstants.SetAnchored(toggleBtnGo.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(280, 36));
            toggleBtnGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -294);

            // Status text — shows login errors, "Logging in..." etc. Starts empty.
            var statusTmp = UIConstants.MakeTMP(panel.transform, "StatusText", "", 13, UIConstants.ColTextMuted, FontStyles.Normal, s_Poppins);
            UIConstants.SetAnchored(statusTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(320, 40));
            statusTmp.rectTransform.anchoredPosition = new Vector2(0, -300);
            statusTmp.alignment = TextAlignmentOptions.Center;
            statusTmp.textWrappingMode = TextWrappingModes.Normal;

            

            return panel.gameObject;
        }
    }
