using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WipeoutConfig : Config
{
    public int startingPoints = 1;
    public int pointIncreasePerQuestion = 0;

    public WipeoutConfig()
    {

    }

    public WipeoutConfig(int startingPoints, int pointIncreasePerQuestion)
    {
        this.startingPoints = startingPoints;
        this.pointIncreasePerQuestion = pointIncreasePerQuestion;
    }
}
