using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DripfeedConfig : Config
{
    public int startingPoints = 5;
    public int pointDecreasePerClue = 1;

    public DripfeedConfig()
    {

    }

    public DripfeedConfig(int startingPoints, int pointDecreasePerClue)
    {
        this.startingPoints = startingPoints;
        this.pointDecreasePerClue = pointDecreasePerClue;
    }
}
