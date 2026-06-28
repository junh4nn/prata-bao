using System;

[Serializable]
public class QuizOptionDto
{
    public int    key;
    public string text;
}

[Serializable]
public class QuizQuestionDto
{
    public int             id;
    public string          questionText;
    public QuizOptionDto[] options;
}

[Serializable]
public class QuizQuestionsResponse
{
    public QuizQuestionDto[] questions;
}

[Serializable]
public class QuizAnswerRequest
{
    public int questionId;
    public int selectedIndex; // the tapped option's key, not its on-screen row position
}

[Serializable]
public class QuizAnswerResponse
{
    public bool   isCorrect;
    public int    correctIndex; // also a key, not a row position
    public string explanation;
    public int    coinsEarned;
    public int    newBalance;
}
