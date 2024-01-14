using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using NaughtyAttributes;
using System.Linq;
using TMPro;

public class Operator : SingletonMonoBehaviour<Operator>
{
    [Header("Game Settings")]
    [Tooltip("Players must join the room with valid Twitch username as their name; this will skip the process of validation")]
    public bool fastValidation;
    [Tooltip("Start the game in recovery mode to restore any saved data from a previous game crash")]
    public bool recoveryMode;

    public TextMeshProUGUI copyrightMesh;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        copyrightMesh.text = copyrightMesh.text.Replace("[###]", Application.version);
        StorageManager.SetUpStorage();
        LicenseManager.Get.InitialiseLicense();
    }

    [Button]
    public void ProgressGameplay()
    {
        GameplayManager.Get.ProgressGameplay();
    }
}
