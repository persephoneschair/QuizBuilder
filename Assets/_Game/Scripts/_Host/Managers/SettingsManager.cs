using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : SingletonMonoBehaviour<SettingsManager>
{
    private Settings _currentSettings;
    public Settings CurrentSettings
    {
        get
        {
            return _currentSettings; 
        }
        set
        {
            _currentSettings = value;
            OnInitialiseSettings();
        }
    }

    public Toggle lateEntry;
    public TMP_InputField operatorName;
    public Toggle deletePrompt;
    public Slider autoCorrect;
    public TextMeshProUGUI correctLabel;
    public Slider autoReject;
    public TextMeshProUGUI rejectLabel;

    private bool loading = false;

    public void OnInitialiseSettings()
    {
        loading = true;

        lateEntry.isOn = CurrentSettings.allowLateEntry;
        operatorName.text = CurrentSettings.operatorName;
        deletePrompt.isOn = CurrentSettings.promptBeforeDeleting;
        autoCorrect.value = CurrentSettings.spellcheckerAutoCorrectTolerance;
        correctLabel.text = CurrentSettings.spellcheckerAutoCorrectTolerance + "%";

        autoReject.maxValue = CurrentSettings.spellcheckerAutoCorrectTolerance;
        autoReject.value = CurrentSettings.spellcheckerAutoRejectTolerance > autoReject.maxValue ? autoReject.maxValue : CurrentSettings.spellcheckerAutoRejectTolerance;
        CurrentSettings.spellcheckerAutoRejectTolerance = (int)autoReject.value;
        rejectLabel.text = CurrentSettings.spellcheckerAutoRejectTolerance + "%";

        loading = false;

        UpdateSettings();
    }

    public void OnToggle()
    {
        if (loading)
            return;
        UpdateSettings();
    }

    public void OnChangeUpperThreshold()
    {
        if (loading)
            return;
        correctLabel.text = autoCorrect.value + "%";
        autoReject.maxValue = autoCorrect.value;
        OnChangeLowerThreshold();
    }

    public void OnChangeLowerThreshold()
    {
        if (loading)
            return;
        if (autoReject.value > autoReject.maxValue)
            autoReject.value = autoReject.maxValue;

        rejectLabel.text = autoReject.value + "%";
        UpdateSettings();
    }

    private void UpdateSettings()
    {
        CurrentSettings.allowLateEntry = lateEntry.isOn;
        CurrentSettings.operatorName = operatorName.text;
        CurrentSettings.promptBeforeDeleting = deletePrompt.isOn;
        CurrentSettings.spellcheckerAutoCorrectTolerance = (int)autoCorrect.value;
        CurrentSettings.spellcheckerAutoRejectTolerance = (int)autoReject.value;
    }

    public void OnPersistSettings()
    {
        UpdateSettings();
        StorageManager.SaveSettings(CurrentSettings);
    }
}
