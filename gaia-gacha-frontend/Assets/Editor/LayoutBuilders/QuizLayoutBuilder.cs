using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class QuizLayoutBuilder
{
    static TMP_FontAsset s_PoppinsSemiBold;
    static TMP_FontAsset s_PoppinsLight;
    static TMP_FontAsset s_CinzelRegular;
    static TMP_FontAsset s_CinzelSemiBold;
    static Sprite        s_BackIcon;
    static Sprite        s_UISprite;
    static Sprite        s_KnobSprite;

    // ── Quiz Panel ───────────────────────────────────────────────────────────
    // Builds the eco-quiz screen reached from the Hub's Quiz tile.
    // Layout (top to bottom):
    //   HeaderBar    — "GaiaGacha" title + Eco-Coins balance, same as every other panel
    //   ProgressBar  — Track + Fill (width driven by QuizManager) + "N/3" text
    //   QuestionCard — wrapped question text
    //   AnswerRowA/B/C — badge + answer text, colored by QuizAnswerOptionDisplay at runtime
    //   FeedbackText — per-question correct/incorrect explanation
    //   NextButton   — outline style, hidden until an answer is given
    //   FooterBar    — back button
    //   ResultsModal — dim backdrop + centered card, parented last so it draws on top;
    //                  SetActive(false) by default

    [MenuItem("GaiaGacha/LayoutBuilders/Build Quiz Panel", priority = 204)]
    static void Build()
    {
        s_PoppinsSemiBold = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Poppins-SemiBold SDF.asset");
        s_PoppinsLight    = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Poppins-Light SDF.asset");
        s_CinzelRegular   = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Cinzel-Regular SDF.asset");
        s_CinzelSemiBold  = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/Cinzel-SemiBold SDF.asset");
        s_BackIcon        = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/back_button.png");
        s_UISprite        = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        s_KnobSprite      = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

        var temp    = new GameObject("Temp");
        var panelGo = BuildQuizPanel(temp.transform);

        PrefabUtility.SaveAsPrefabAsset(panelGo, "Assets/Prefabs/QuizPanel.prefab");
        Object.DestroyImmediate(temp);
        AssetDatabase.Refresh();
        Debug.Log("<color=green>[QuizLayoutBuilder] QuizPanel prefab saved.</color>");
    }

    static GameObject BuildQuizPanel(Transform parent)
    {
        var panel = UIConstants.MakeRect(parent, "QuizPanel");
        UIConstants.Stretch(panel);

        // ── Header bar (matches every other panel) ──────────────────────────
        var header = UIConstants.MakeImage(panel.transform, "HeaderBar", null, UIConstants.ColSurface);
        UIConstants.SetAnchored(header.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0, 64));
        header.rectTransform.anchoredPosition = new Vector2(0, -32);

        var titleTmp = UIConstants.MakeTMP(header.transform, "TitleText", "GaiaGacha", 24, UIConstants.ColTextPrimary, FontStyles.Bold, s_CinzelRegular);
        UIConstants.SetAnchored(titleTmp.rectTransform, new Vector2(0f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 40));
        titleTmp.rectTransform.anchoredPosition = new Vector2(20, 0);
        titleTmp.alignment = TextAlignmentOptions.MidlineLeft;

        var coinsTmp = UIConstants.MakeTMP(header.transform, "CoinsText", "Eco-Coins: --", 16, UIConstants.ColGold, FontStyles.Bold, s_CinzelRegular);
        UIConstants.SetAnchored(coinsTmp.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(1f, 0.5f), new Vector2(0, 40));
        coinsTmp.rectTransform.anchoredPosition = new Vector2(-20, 0);
        coinsTmp.alignment = TextAlignmentOptions.MidlineRight;

        // ── Progress bar — Track + Fill (width driven by QuizManager) + "N/3" text ──
        var progressRow = UIConstants.MakeRect(panel.transform, "ProgressBar");
        UIConstants.SetAnchored(progressRow, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0, 40));
        progressRow.anchoredPosition = new Vector2(0, -73);

        // Margins below are a percentage of the panel's width (20% per side) rather than a
        // fixed pixel inset, so the gap to the screen edge grows on wide windows instead of
        // staying flush — ProgressBar/QuestionCard/FeedbackText all share this same edge.
        // ProgressText sits at the right end of the bar (not stacked above it), so the two
        // only need to share ~20px of vertical room instead of ~32px.
        var progressText = UIConstants.MakeTMP(progressRow.transform, "ProgressText", "1/3", 12, UIConstants.ColTextMuted, FontStyles.Bold, s_PoppinsSemiBold);
        UIConstants.SetAnchored(progressText.rectTransform, new Vector2(0.8f, 0.5f), new Vector2(0.8f, 0.5f), new Vector2(46, 20));
        progressText.rectTransform.pivot = new Vector2(1f, 0.5f);
        progressText.rectTransform.anchoredPosition = new Vector2(0, -6);
        progressText.alignment = TextAlignmentOptions.MidlineRight;

        var track = UIConstants.MakeImage(progressRow.transform, "Track", s_UISprite, UIConstants.ColSurface);
        track.type = Image.Type.Sliced;
        UIConstants.SetAnchored(track.rectTransform, new Vector2(0.2f, 0.5f), new Vector2(0.8f, 0.5f), new Vector2(-54, 8));
        track.rectTransform.anchoredPosition = new Vector2(-27, -6);

        var fillRt = UIConstants.MakeRect(track.transform, "Fill");
        fillRt.anchorMin = new Vector2(0f, 0f);
        fillRt.anchorMax = new Vector2(0f, 1f);
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;
        var fillImg = fillRt.gameObject.AddComponent<Image>();
        fillImg.sprite = s_UISprite;
        fillImg.type   = Image.Type.Sliced;
        fillImg.color  = QuizAnswerVisuals.ColCorrect;

        // ── Question card — contains the question text and all 3 answer rows ──
        var questionCard = UIConstants.MakeImage(panel.transform, "QuestionCard", s_UISprite, UIConstants.ColSurface);
        questionCard.type = Image.Type.Sliced;
        UIConstants.SetAnchored(questionCard.rectTransform, new Vector2(0.2f, 1f), new Vector2(0.8f, 1f), new Vector2(0, 450));
        questionCard.rectTransform.anchoredPosition = new Vector2(0, -319);

        var questionText = UIConstants.MakeTMP(questionCard.transform, "QuestionText", "", 16, UIConstants.ColTextPrimary, FontStyles.Normal, s_CinzelSemiBold);
        UIConstants.SetAnchored(questionText.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(-32, 120));
        questionText.rectTransform.anchoredPosition = new Vector2(0, -72);
        questionText.alignment = TextAlignmentOptions.Center;
        questionText.textWrappingMode = TextWrappingModes.Normal;

        // ── Answer rows — always exactly 3 static rows, nested inside the card ──
        var rowA = BuildAnswerRow(questionCard.transform, "AnswerRowA", -194);
        var rowB = BuildAnswerRow(questionCard.transform, "AnswerRowB", -292);
        var rowC = BuildAnswerRow(questionCard.transform, "AnswerRowC", -390);

        // ── Feedback text — outside the card ────────────────────────────────────
        var feedbackText = UIConstants.MakeTMP(panel.transform, "FeedbackText", "", 13, UIConstants.ColTextPrimary, FontStyles.Italic, s_PoppinsLight);
        UIConstants.SetAnchored(feedbackText.rectTransform, new Vector2(0.2f, 1f), new Vector2(0.8f, 1f), new Vector2(0, 100));
        feedbackText.rectTransform.anchoredPosition = new Vector2(0, -612);
        feedbackText.alignment = TextAlignmentOptions.Center;
        feedbackText.textWrappingMode = TextWrappingModes.Normal;

        // ── Next button — outline style, flush below the feedback text, hidden until an answer is given ──
        var (nextButtonGo, _) = UIConstants.MakeOutlineButton(panel.transform, "NextButton", "NEXT", 15, s_PoppinsSemiBold);
        UIConstants.SetAnchored(nextButtonGo.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(140, 44));
        nextButtonGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -702);
        nextButtonGo.SetActive(false);

        // ── Footer bar (back button, matches every other panel) ─────────────
        var footer = UIConstants.MakeImage(panel.transform, "FooterBar", null, UIConstants.ColSurface);
        UIConstants.SetAnchored(footer.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0, 56));
        footer.rectTransform.anchoredPosition = new Vector2(0, 28);

        var backTileRt = UIConstants.MakeRect(footer.transform, "BackButton");
        UIConstants.SetAnchored(backTileRt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(48, 40));
        backTileRt.anchoredPosition = new Vector2(0, 1);
        var backTileBg = backTileRt.gameObject.AddComponent<Image>();
        backTileBg.color = UIConstants.ColBg;
        var backTileBtn = backTileRt.gameObject.AddComponent<Button>();
        backTileBtn.targetGraphic = backTileBg;

        var backIconImg = UIConstants.MakeImage(backTileRt, "Icon", s_BackIcon, UIConstants.ColTextMuted);
        backIconImg.preserveAspect = true;
        UIConstants.SetAnchored(backIconImg.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(20, 20));
        backIconImg.rectTransform.anchoredPosition = Vector2.zero;

        // ── Results modal — parented last so it draws on top; hidden by default ──
        BuildResultsModal(panel.transform);

        return panel.gameObject;
    }

    // Builds one static answer row using the layered "Border behind Face" trick from
    // InventoryCardBuilder — AnswerBorder peeks a few px around AnswerFace, so recoloring
    // the border at runtime produces the green/red outline.
    static GameObject BuildAnswerRow(Transform parent, string name, float anchoredY)
    {
        var row = UIConstants.MakeRect(parent, name);
        UIConstants.SetAnchored(row, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(-32, 88));
        row.anchoredPosition = new Vector2(0, anchoredY);
        var rowButton = row.gameObject.AddComponent<Button>();

        var border = UIConstants.MakeImage(row, "AnswerBorder", s_UISprite, QuizAnswerVisuals.GetBorderColor(QuizAnswerState.Unanswered));
        border.type = Image.Type.Sliced;
        UIConstants.SetAnchored(border.rectTransform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-8, 88));

        var face = UIConstants.MakeImage(row, "AnswerFace", s_UISprite, QuizAnswerVisuals.GetFaceColor(QuizAnswerState.Unanswered));
        face.type = Image.Type.Sliced;
        UIConstants.SetAnchored(face.rectTransform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-14, 82));
        rowButton.targetGraphic = face;

        var badgeCircle = UIConstants.MakeImage(face.transform, "BadgeCircle", s_KnobSprite, UIConstants.ColInputBg);
        UIConstants.SetAnchored(badgeCircle.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(32, 32));
        badgeCircle.rectTransform.anchoredPosition = new Vector2(28, 0);

        var badgeText = UIConstants.MakeTMP(badgeCircle.transform, "BadgeText", "", 14, UIConstants.ColTextPrimary, FontStyles.Bold, s_PoppinsSemiBold);
        UIConstants.SetAnchored(badgeText.rectTransform, Vector2.zero, Vector2.one, Vector2.zero);
        badgeText.alignment = TextAlignmentOptions.Center;

        var answerText = UIConstants.MakeTMP(face.transform, "AnswerText", "", 13, QuizAnswerVisuals.GetTextColor(QuizAnswerState.Unanswered), FontStyles.Normal, s_PoppinsSemiBold);
        answerText.rectTransform.anchorMin = Vector2.zero;
        answerText.rectTransform.anchorMax = Vector2.one;
        answerText.rectTransform.offsetMin = new Vector2(54, 4);
        answerText.rectTransform.offsetMax = new Vector2(-14, -4);
        answerText.alignment = TextAlignmentOptions.MidlineLeft;
        answerText.textWrappingMode = TextWrappingModes.Normal;

        var display = row.gameObject.AddComponent<QuizAnswerOptionDisplay>();
        var so = new SerializedObject(display);
        so.FindProperty("answerBorderImage").objectReferenceValue = border;
        so.FindProperty("answerFaceImage").objectReferenceValue   = face;
        so.FindProperty("badgeText").objectReferenceValue         = badgeText;
        so.FindProperty("answerText").objectReferenceValue        = answerText;
        so.FindProperty("rowButton").objectReferenceValue         = rowButton;
        so.ApplyModifiedProperties();

        return row.gameObject;
    }

    static void BuildResultsModal(Transform parent)
    {
        var modalRoot = UIConstants.MakeRect(parent, "ResultsModal");
        UIConstants.Stretch(modalRoot);
        modalRoot.gameObject.SetActive(false);

        var backdrop = UIConstants.MakeImage(modalRoot, "Backdrop", null, new Color(0f, 0f, 0f, 0.6f));
        UIConstants.Stretch(backdrop.rectTransform);

        var card = UIConstants.MakeImage(modalRoot, "ResultsCard", s_UISprite, UIConstants.ColSurface);
        card.type = Image.Type.Sliced;
        UIConstants.SetAnchored(card.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(300, 420));

        var glyphTmp = UIConstants.MakeTMP(card.transform, "ResultGlyphText", "", 48, UIConstants.ColTextPrimary, FontStyles.Normal, s_CinzelRegular);
        UIConstants.SetAnchored(glyphTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(200, 60));
        glyphTmp.rectTransform.anchoredPosition = new Vector2(0, -46);
        glyphTmp.alignment = TextAlignmentOptions.Center;

        var titleTmp = UIConstants.MakeTMP(card.transform, "ResultTitleText", "", 20, UIConstants.ColTextPrimary, FontStyles.Bold, s_CinzelSemiBold);
        UIConstants.SetAnchored(titleTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(260, 30));
        titleTmp.rectTransform.anchoredPosition = new Vector2(0, -98);
        titleTmp.alignment = TextAlignmentOptions.Center;

        var scoreTmp = UIConstants.MakeTMP(card.transform, "ScoreText", "", 32, UIConstants.ColGold, FontStyles.Bold, s_CinzelSemiBold);
        UIConstants.SetAnchored(scoreTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(200, 44));
        scoreTmp.rectTransform.anchoredPosition = new Vector2(0, -148);
        scoreTmp.alignment = TextAlignmentOptions.Center;

        var subtitleTmp = UIConstants.MakeTMP(card.transform, "SubtitleText", "", 13, UIConstants.ColTextMuted, FontStyles.Normal, s_PoppinsLight);
        UIConstants.SetAnchored(subtitleTmp.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(260, 40));
        subtitleTmp.rectTransform.anchoredPosition = new Vector2(0, -196);
        subtitleTmp.alignment = TextAlignmentOptions.Center;
        subtitleTmp.textWrappingMode = TextWrappingModes.Normal;

        var rewardPill = UIConstants.MakeImage(card.transform, "RewardPill", s_UISprite, UIConstants.ColInputBg);
        rewardPill.type = Image.Type.Sliced;
        UIConstants.SetAnchored(rewardPill.rectTransform, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(230, 40));
        rewardPill.rectTransform.anchoredPosition = new Vector2(0, -244);

        var rewardGlyphTmp = UIConstants.MakeTMP(rewardPill.transform, "RewardGlyphText", "", 16, UIConstants.ColGold, FontStyles.Bold, s_PoppinsSemiBold);
        UIConstants.SetAnchored(rewardGlyphTmp.rectTransform, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(24, 24));
        rewardGlyphTmp.rectTransform.anchoredPosition = new Vector2(20, 0);
        rewardGlyphTmp.alignment = TextAlignmentOptions.Center;

        var rewardTmp = UIConstants.MakeTMP(rewardPill.transform, "RewardText", "", 14, UIConstants.ColGold, FontStyles.Bold, s_PoppinsSemiBold);
        UIConstants.SetAnchored(rewardTmp.rectTransform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(-44, 24));
        rewardTmp.rectTransform.anchoredPosition = new Vector2(0, 0);
        rewardTmp.alignment = TextAlignmentOptions.Center;

        var (tryAgainGo, _) = UIConstants.MakeOutlineButton(card.transform, "TryAgainButton", "TRY AGAIN", 15, s_PoppinsSemiBold);
        UIConstants.SetAnchored(tryAgainGo.GetComponent<RectTransform>(), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(180, 46));
        tryAgainGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -354);

        // These ResultsModal fields are wired into QuizManager by SceneBuilder once the
        // panel is instantiated into the scene — same convention as every other panel.
    }
}
