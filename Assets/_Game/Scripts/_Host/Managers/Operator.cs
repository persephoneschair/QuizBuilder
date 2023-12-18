using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using NaughtyAttributes;
using System.Linq;

public class Operator : SingletonMonoBehaviour<Operator>
{
    [Header("Game Settings")]
    [Tooltip("Players must join the room with valid Twitch username as their name; this will skip the process of validation")]
    public bool fastValidation;
    [Tooltip("Start the game in recovery mode to restore any saved data from a previous game crash")]
    public bool recoveryMode;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        
    }

    [Button]
    public void ProgressGameplay()
    {
        GameplayManager.Get.ProgressGameplay();
    }
}
