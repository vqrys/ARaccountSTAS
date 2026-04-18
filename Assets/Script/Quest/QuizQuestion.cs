using UnityEngine;

[System.Serializable]
public class QuizQuestion
{
    [TextArea(2, 4)]
    public string question;

    public string[] answers;

    [Min(0)]
    public int correctIndex;
}