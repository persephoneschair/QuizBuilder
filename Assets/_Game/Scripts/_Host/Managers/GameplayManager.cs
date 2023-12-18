using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using TMPro;
using System.Linq;

public class GameplayManager : SingletonMonoBehaviour<GameplayManager>
{
    [Header("Rounds")]
    public RoundBase[] rounds;

    [Header("Question Data")]
    public static int nextQuestionIndex = 0;

    public enum GameplayState
    {
        DoNothing
    };
    public GameplayState currentStage = GameplayState.DoNothing;

    public enum Round { None };
    public Round currentRound = Round.None;
    public int roundsPlayed = 0;

    [Button]
    public void ProgressGameplay()
    {
        
    }
}
