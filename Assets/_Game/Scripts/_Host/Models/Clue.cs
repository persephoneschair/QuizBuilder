using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clue
{
    public string clueText = "";
    public string assetPath = "";

    public Clue()
    {

    }

    public Clue(string clueText, string assetPath)
    {
        this.clueText = clueText;
        this.assetPath = assetPath;
    }
}
