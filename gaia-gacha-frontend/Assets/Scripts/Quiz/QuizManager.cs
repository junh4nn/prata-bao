using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [Header("Backend Configuration")]
    [SerializeField] private string questionsUrl = "http://localhost:3000/api/quiz/questions";
    [SerializeField] private string answerUrl    = "http://localhost:3000/api/quiz/answer";

    [Header("UI - Header")]
    [SerializeField] private TextMeshProUGUI coinsText;

    [Header("UI - Progress Bar")]
    [SerializeField] private Image           progressFillImage;
    [SerializeField] private TextMeshProUGUI progressText;

    [Header("UI - Question")]
    [SerializeField] private TextMeshProUGUI questionText;

    [Header("UI - Answer Rows")]
    [SerializeField] private QuizAnswerOptionDisplay answerRowA;
    [SerializeField] private QuizAnswerOptionDisplay answerRowB;
    [SerializeField] private QuizAnswerOptionDisplay answerRowC;

    [Header("UI - Feedback")]
    [SerializeField] private TextMeshProUGUI feedbackText;
    [SerializeField] private Button          nextButton;

    [Header("UI - Results Modal")]
    [SerializeField] private GameObject      resultsModal;
    [SerializeField] private TextMeshProUGUI resultGlyphText;
    [SerializeField] private TextMeshProUGUI resultTitleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button          tryAgainButton;

    [Header("Navigation")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private GameObject hubPanel;
    [SerializeField] private Button     backButton;

    private static readonly string[] letters = { "A", "B", "C" };

    private QuizAnswerOptionDisplay[] answerRows;
    private QuizQuestionDto[]         questions = Array.Empty<QuizQuestionDto>();
    private int currentIndex;
    private int score;
    private int coinsEarnedThisRun;

    void Start()
    {
        answerRows = new[] { answerRowA, answerRowB, answerRowC };
        foreach (QuizAnswerOptionDisplay row in answerRows)
            if (row != null) row.OnClicked += OnAnswerClicked;

        if (nextButton    != null) nextButton.onClick.AddListener(OnNextClicked);
        if (tryAgainButton != null) tryAgainButton.onClick.AddListener(OnTryAgainClicked);
        if (backButton    != null) backButton.onClick.AddListener(OnBackClicked);
    }

    void OnEnable()
    {
        if (coinsText != null) coinsText.text = $"Eco-Coins: {AuthManager.Coins}";
        if (resultsModal != null) resultsModal.SetActive(false);
        StartCoroutine(FetchQuestions());
    }

    private IEnumerator FetchQuestions()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(questionsUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[QuizManager] Failed to load questions: {request.error}");
                yield break;
            }

            questions = JsonUtility.FromJson<QuizQuestionsResponse>(request.downloadHandler.text).questions;
        }

        currentIndex = 0;
        score = 0;
        coinsEarnedThisRun = 0;
        ShowQuestion(currentIndex);
    }

    private void ShowQuestion(int index)
    {
        QuizQuestionDto question = questions[index];

        if (questionText != null) questionText.text = question.questionText;

        for (int i = 0; i < answerRows.Length; i++)
        {
            if (answerRows[i] == null) continue;
            answerRows[i].Setup(question.options[i].key, letters[i], question.options[i].text);
        }

        if (feedbackText != null) feedbackText.text = "";
        if (nextButton   != null) nextButton.gameObject.SetActive(false);

        float pct = (index + 1) / 3f;
        if (progressFillImage != null)
        {
            RectTransform fillRt = progressFillImage.rectTransform;
            fillRt.anchorMax = new Vector2(pct, fillRt.anchorMax.y);
        }
        if (progressText != null) progressText.text = $"{index + 1}/3";
    }

    private void OnAnswerClicked(int key)
    {
        foreach (QuizAnswerOptionDisplay row in answerRows)
            if (row != null) row.SetInteractable(false);

        StartCoroutine(SubmitAnswer(key));
    }

    private IEnumerator SubmitAnswer(int key)
    {
        string jsonPayload = JsonUtility.ToJson(new QuizAnswerRequest
        {
            questionId    = questions[currentIndex].id,
            selectedIndex = key
        });

        using (UnityWebRequest request = new UnityWebRequest(answerUrl, "POST"))
        {
            request.uploadHandler   = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonPayload));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + AuthManager.Token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                string err = request.downloadHandler?.text ?? request.error;
                Debug.LogError($"[QuizManager] Backend error: {err}");
                foreach (QuizAnswerOptionDisplay row in answerRows)
                    if (row != null) row.SetInteractable(true);
            }
            else
            {
                ApplyAnswerResult(key, JsonUtility.FromJson<QuizAnswerResponse>(request.downloadHandler.text));
            }
        }
    }

    private void ApplyAnswerResult(int tappedKey, QuizAnswerResponse response)
    {
        foreach (QuizAnswerOptionDisplay row in answerRows)
        {
            if (row == null) continue;

            QuizAnswerState state = row.Key == response.correctIndex ? QuizAnswerState.Correct
                : row.Key == tappedKey ? QuizAnswerState.WrongSelected
                : QuizAnswerState.WrongUnselected;
            row.SetState(state);
        }

        if (feedbackText != null)
        {
            string prefix = response.isCorrect ? "Correct! " : "Not quite. ";
            feedbackText.text  = prefix + response.explanation;
            feedbackText.color = response.isCorrect ? QuizAnswerVisuals.ColCorrect : QuizAnswerVisuals.ColWrong;
        }

        AuthManager.Coins = response.newBalance;
        if (coinsText != null) coinsText.text = $"Eco-Coins: {response.newBalance}";

        if (response.isCorrect) score++;
        coinsEarnedThisRun += response.coinsEarned;

        if (nextButton != null) nextButton.gameObject.SetActive(true);
    }

    private void OnNextClicked()
    {
        currentIndex++;
        if (currentIndex < questions.Length)
            ShowQuestion(currentIndex);
        else
            ShowResults();
    }

    private void ShowResults()
    {
        string glyph, title, subtitle;
        if (score == questions.Length)
        {
            glyph = ""; title = "Perfect!"; subtitle = "You know your ecosystems inside and out.";
        }
        else if (score > 0)
        {
            glyph = ""; title = "Nice work!"; subtitle = "Solid grasp of the material! Keep it up.";
        }
        else
        {
            glyph = ""; title = "Keep learning!"; subtitle = "Every attempt grows your knowledge.";
        }

        if (resultGlyphText != null) resultGlyphText.text = glyph;
        if (resultTitleText != null) resultTitleText.text = title;
        if (subtitleText    != null) subtitleText.text    = subtitle;
        if (scoreText        != null) scoreText.text        = $"{score}/{questions.Length}";
        if (rewardText        != null) rewardText.text        = $"{coinsEarnedThisRun} Eco-Coins earned";
        if (resultsModal      != null) resultsModal.SetActive(true);
    }

    private void OnTryAgainClicked()
    {
        if (resultsModal != null) resultsModal.SetActive(false);
        StartCoroutine(FetchQuestions());
    }

    private void OnBackClicked()
    {
        if (quizPanel != null) quizPanel.SetActive(false);
        if (hubPanel  != null) hubPanel.SetActive(true);
    }
}
