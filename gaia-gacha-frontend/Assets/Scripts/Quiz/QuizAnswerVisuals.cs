using UnityEngine;

public enum QuizAnswerState { Unanswered, Correct, WrongSelected, WrongUnselected }

public static class QuizAnswerVisuals
{
    public static readonly Color ColNeutralBorder = new Color(0.106f, 0.263f, 0.196f); // matches UIConstants.ColSurface
    public static readonly Color ColNeutralFace   = new Color(0.106f, 0.263f, 0.196f); // matches UIConstants.ColSurface
    public static readonly Color ColFaceDimmed    = new Color(0.075f, 0.184f, 0.137f); // darker surface, for the unselected wrong row
    public static readonly Color ColCorrect       = RarityVisuals.ColRare; // reuse the existing rarity green
    public static readonly Color ColWrong         = new Color(0.808f, 0.220f, 0.298f); // muted red, no equivalent exists yet
    public static readonly Color ColTextPrimary   = new Color(0.945f, 0.980f, 0.933f); // matches UIConstants.ColTextPrimary
    public static readonly Color ColTextDimmed    = new Color(0.584f, 0.722f, 0.627f); // matches UIConstants.ColTextMuted

    public static Color GetBorderColor(QuizAnswerState s) => s switch
    {
        QuizAnswerState.Correct         => ColCorrect,
        QuizAnswerState.WrongSelected   => ColWrong,
        QuizAnswerState.WrongUnselected => ColTextDimmed,
        _                                => ColNeutralBorder
    };

    public static Color GetFaceColor(QuizAnswerState s) => s switch
    {
        QuizAnswerState.WrongUnselected => ColFaceDimmed,
        _                                => ColNeutralFace
    };

    public static Color GetTextColor(QuizAnswerState s) => s switch
    {
        QuizAnswerState.WrongUnselected => ColTextDimmed,
        _                                => ColTextPrimary
    };
}
