using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscalatorConfig : Config
{
    public int startingPoints = 1;
    public int pointIncreasePerQuestion = 1;
    public bool fixPointValues = false;
    public bool wipeoutOnIncorrect = true;
    public bool wipeoutOnAbstention = false;
    public bool lockoutOnIncorrect = false;
    public bool lockoutOnAbstention = false;

    public EscalatorConfig()
    {

    }

    public EscalatorConfig(int startingPoints, int pointIncreasePerQuestion, bool fixPointValues, bool wipeoutOnIncorrect, bool wipeoutOnAbstention, bool lockoutOnIncorrect, bool lockoutOnAbstention)
    {
        this.startingPoints = startingPoints;
        this.pointIncreasePerQuestion = pointIncreasePerQuestion;
        this.fixPointValues = fixPointValues;
        this.wipeoutOnIncorrect = wipeoutOnIncorrect;
        this.wipeoutOnAbstention = wipeoutOnAbstention;
        this.lockoutOnIncorrect = lockoutOnIncorrect;
        this.lockoutOnAbstention = lockoutOnAbstention;
    }
}
