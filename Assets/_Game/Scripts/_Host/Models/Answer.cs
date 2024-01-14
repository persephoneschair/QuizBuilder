using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Answer
{
    public string answerText = "";
    public List<string> validAnswers = new List<string>();
    public bool isCorrect = false;
    public string assetPath = "";
    public string hostNotes = "";

    public Answer()
    {
        
    }

    public Answer(string answerText, List<string> validAnswers, bool isCorrect, string assetPath, string hostNotes)
    {
        this.answerText = answerText;
        this.validAnswers = validAnswers;
        this.isCorrect = isCorrect;
        this.assetPath = assetPath;
        this.hostNotes = hostNotes;
    }
}
