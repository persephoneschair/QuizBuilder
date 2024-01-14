using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    public Settings()
    {

    }

    public bool allowLateEntry = true;
    public string operatorName = "Operator";
    public bool promptBeforeDeleting = true;
    public int spellcheckerAutoCorrectTolerance = 80;
    public int spellcheckerAutoRejectTolerance = 20;
}
