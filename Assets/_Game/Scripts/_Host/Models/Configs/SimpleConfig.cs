using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleConfig : Config
{
    public int pointsForCorrect = 1;
    public int pointsForIncorrect = 0;

    public SimpleConfig()
    {

    }

    public SimpleConfig(int pointsForCorrect, int pointsForIncorrect)
    {
        this.pointsForCorrect = pointsForCorrect;
        this.pointsForIncorrect = pointsForIncorrect;
    }
}
