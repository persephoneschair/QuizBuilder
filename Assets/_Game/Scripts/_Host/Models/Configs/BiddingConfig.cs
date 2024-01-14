using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiddingConfig : Config
{
    public int maxBid = 3;
    public int minBid = 1;

    public BiddingConfig()
    {

    }

    public BiddingConfig(int maxBid, int minBid)
    {
        this.maxBid = maxBid;
        this.minBid = minBid;
    }
}
