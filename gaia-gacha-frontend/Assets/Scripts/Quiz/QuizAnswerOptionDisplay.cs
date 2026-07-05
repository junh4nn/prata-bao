// QuizAnswerOptionDisplay.cs: a single tappable answer row in the quiz UI. Reflects its
// visual state (unanswered/correct/wrong) via QuizAnswerVisuals.

using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizAnswerOptionDisplay : MonoBehaviour
{
    [SerializeField] private Image           answerBorderImage;
    [SerializeField] private Image           answerFaceImage;
    [SerializeField] private TextMeshProUGUI badgeText;
    [SerializeField] private TextMeshProUGUI answerText;
    [SerializeField] private Button          rowButton;

    public int Key { get; private set; }
    public event Action<int> OnClicked;

    void Awake()
    {
        if (rowButton != null)
            rowButton.onClick.AddListener(() => OnClicked?.Invoke(Key));
    }

    public void Setup(int key, string letter, string text)
    {
        Key = key;

        if (badgeText  != null) badgeText.text  = letter;
        if (answerText != null) answerText.text = text;

        SetState(QuizAnswerState.Unanswered);
        SetInteractable(true);
    }

    public void SetState(QuizAnswerState state)
    {
        if (answerBorderImage != null) answerBorderImage.color = QuizAnswerVisuals.GetBorderColor(state);
        if (answerFaceImage   != null) answerFaceImage.color   = QuizAnswerVisuals.GetFaceColor(state);
        if (answerText        != null) answerText.color        = QuizAnswerVisuals.GetTextColor(state);
    }

    public void SetInteractable(bool isInteractable)
    {
        if (rowButton != null) rowButton.interactable = isInteractable;
    }
}
