using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundType
{
    public enum Round
    {
        SimpleQAndA,
        BiddingQAndA,
        RedHerrings,

        SimpleMC,
        BiddingMC,
        Escalator,
        Wipeout,
        Pointless,

        Dripfeed,
        ConnectionGrid
    }

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

    public Round roundType;
    public string roundTypeString;
    public Guid roundID;

    public Config roundConfig;
    public QuestionStack questionStack;

    public RoundType()
    {

    }

    public RoundType(Round type, Config config)
    {
        roundID = Guid.NewGuid();
        roundType = type;
        roundTypeString = displayNames[(int)type];
        questionStack = new QuestionStack();

        roundConfig = config;
    }

    public RoundType(Round type, Config config, Guid roundID)
    {
        this.roundID = roundID;
        roundType = type;
        roundTypeString = displayNames[(int)type];
        questionStack = new QuestionStack();

        roundConfig = config;
    }
}
