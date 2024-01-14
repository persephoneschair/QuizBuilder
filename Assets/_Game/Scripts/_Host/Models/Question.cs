using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Question
{
    public string questionText = "";
    public string categoryText = "";
    public string assetPath = "";
    public string hostNotes = "";

    public Question()
    {

    }

    public Question(string questionText, string categoryText, string assetPath, string hostNotes)
    {
        this.questionText = questionText;
        this.categoryText = categoryText;
        this.assetPath = assetPath;
        this.hostNotes = hostNotes;
    }
}
