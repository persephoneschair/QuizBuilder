using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddRoundButton : MonoBehaviour
{
    private string[] displayNames = new string[]
    {
        "Simple Q&A",
        "Bidding Q&A",
        "Red Herrings",

        "Simple Multiple Choice",
        "Bidding Multiple Choice",
        "Escalator",
        "Wipeout",
        "Pointless",

        "Dripfeed",
        "Connection Grid"
    };

    public RoundType.Round round;
    public Button button;
    public TextMeshProUGUI label;
    public string displayName;

    public void Initialise(int index)
    {
        round = (RoundType.Round)index;
        displayName = displayNames[index];
        label.text = "Add " + displayName + " Round";
    }

    public void OnClick()
    {
        QuizTemplateManager.Get.OnAddRound(round, displayName, Guid.NewGuid());
    }
}
