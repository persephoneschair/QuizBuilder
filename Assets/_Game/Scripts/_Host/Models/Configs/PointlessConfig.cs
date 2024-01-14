using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointlessConfig : Config
{
    public int pointsForUnique = 5;
    public bool floorAtOne = true;

    public PointlessConfig()
    {

    }

    public PointlessConfig(int pointsForUnique, bool floorAtOne)
    {
        this.pointsForUnique = pointsForUnique;
        this.floorAtOne = floorAtOne;
    }
}
