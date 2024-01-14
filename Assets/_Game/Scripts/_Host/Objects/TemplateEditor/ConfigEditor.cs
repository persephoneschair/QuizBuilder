using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfigEditor : SingletonMonoBehaviour<ConfigEditor>
{
    public GameObject configObject;

    public TMP_InputField[] inputFields;
    public GameObject[] inputObjects;
    public Toggle[] toggles;
    public TextMeshProUGUI header;

    public AddedRound addedRoundToAlter;
    public Config config;

    public void ActivateEditor(Config conf, AddedRound ar, string header)
    {
        addedRoundToAlter = ar;
        this.config = conf;
        this.header.text = header + " Settings";

        ClearAllFields();

        //Round display name which needs to be on ALL rounds
        inputFields[8].gameObject.SetActive(true);
        inputObjects[8].SetActive(true);
        inputFields[8].text = conf.roundDisplayName;

        //Always give an option for "is question timed" and show the question time input field
        toggles[6].gameObject.SetActive(true);
        toggles[6].isOn = false;

        inputFields[9].gameObject.SetActive(true);
        inputObjects[9].SetActive(true);
        inputFields[9].text = conf.questionTime.ToString();

        if (config is SimpleConfig sc)
        {
            for(int i = 0; i < 2; i++)
            {
                inputFields[i].gameObject.SetActive(true);
                inputObjects[i].SetActive(true);
            }

            inputFields[0].text = sc.pointsForCorrect.ToString();
            inputFields[1].text = sc.pointsForIncorrect.ToString();
        }
        else if(config is BiddingConfig bc)
        {
            for (int i = 2; i < 4; i++)
            {
                inputFields[i].gameObject.SetActive(true);
                inputObjects[i].SetActive(true);
            }

            inputFields[2].text = bc.maxBid.ToString();
            inputFields[3].text = bc.minBid.ToString();
        }
        else if(config is EscalatorConfig ec)
        {
            for (int i = 4; i < 6; i++)
            {
                inputFields[i].gameObject.SetActive(true);
                inputObjects[i].SetActive(true);
            }

            for (int i = 0; i < 5; i++)
                toggles[i].gameObject.SetActive(true);

            inputFields[4].text = ec.startingPoints.ToString();
            inputFields[5].text = ec.pointIncreasePerQuestion.ToString();

            toggles[0].isOn = ec.fixPointValues;
            toggles[1].isOn = ec.wipeoutOnIncorrect;
            toggles[2].isOn = ec.wipeoutOnAbstention;
            toggles[3].isOn = ec.lockoutOnIncorrect;
            toggles[4].isOn = ec.lockoutOnAbstention;
        }
        else if(config is WipeoutConfig wc)
        {
            for (int i = 4; i < 6; i++)
            {
                inputFields[i].gameObject.SetActive(true);
                inputObjects[i].SetActive(true);
            }

            inputFields[4].text = wc.startingPoints.ToString();
            inputFields[5].text = wc.pointIncreasePerQuestion.ToString();
        }
        else if (config is PointlessConfig pc)
        {
            inputFields[6].gameObject.SetActive(true);
            inputObjects[6].SetActive(true);
            
            toggles[5].gameObject.SetActive(true);

            inputFields[6].text = pc.pointsForUnique.ToString();
            toggles[5].isOn = pc.floorAtOne;
        }
        else if(config is DripfeedConfig dc)
        {
            inputFields[4].gameObject.SetActive(true);
            inputFields[7].gameObject.SetActive(true);
            inputObjects[4].SetActive(true);
            inputObjects[7].SetActive(true);

            inputFields[4].text = dc.startingPoints.ToString();
            inputFields[7].text = dc.pointDecreasePerClue.ToString();
        }

        configObject.SetActive(true);
    }

    private void ClearAllFields()
    {
        foreach (TMP_InputField field in inputFields)
            field.gameObject.SetActive(false);
        foreach (GameObject lbl in inputObjects)
            lbl.SetActive(false);
        foreach (Toggle toggle in toggles)
            toggle.gameObject.SetActive(false);
    }

    public void OnDiscardConfig()
    {
        ClearAllFields();
        addedRoundToAlter = null;
        configObject.SetActive(false);
    }

    public void OnSaveConfig()
    {
        if (string.IsNullOrEmpty(inputFields[8].text))
            config.roundDisplayName = addedRoundToAlter.label.text;
        else
            config.roundDisplayName = inputFields[8].text;

        if (int.TryParse(inputFields[9].text, out int tm))
            config.questionTime = tm > 5 ? tm: 5;

        config.isTimed = toggles[6].isOn;

        if (config is SimpleConfig sc)
        {
            if (int.TryParse(inputFields[0].text, out int val))
                sc.pointsForCorrect = val;

            if (int.TryParse(inputFields[1].text, out val))
                sc.pointsForIncorrect = val;
        }
        else if (config is BiddingConfig bc)
        {
            if (int.TryParse(inputFields[2].text, out int val))
                bc.maxBid = val;

            if (int.TryParse(inputFields[3].text, out val))
                bc.minBid = val;
        }
        else if (config is EscalatorConfig ec)
        {
            if (int.TryParse(inputFields[4].text, out int val))
                ec.startingPoints = val;

            if (int.TryParse(inputFields[5].text, out val))
                ec.pointIncreasePerQuestion = val;

            ec.fixPointValues = toggles[0].isOn;
            ec.wipeoutOnIncorrect = toggles[1].isOn;
            ec.wipeoutOnAbstention = toggles[2].isOn;
            ec.lockoutOnIncorrect = toggles[3].isOn;
            ec.lockoutOnAbstention = toggles[4].isOn;
        }
        else if (config is WipeoutConfig wc)
        {
            if (int.TryParse(inputFields[4].text, out int val))
                wc.startingPoints = val;

            if (int.TryParse(inputFields[5].text, out val))
                wc.pointIncreasePerQuestion = val;
        }
        else if (config is PointlessConfig pc)
        {
            if (int.TryParse(inputFields[6].text, out int val))
                pc.pointsForUnique = val;

            pc.floorAtOne = toggles[5].isOn;
        }
        else if (config is DripfeedConfig dc)
        {
            if (int.TryParse(inputFields[4].text, out int val))
                dc.startingPoints = val;

            if (int.TryParse(inputFields[7].text, out val))
                dc.pointDecreasePerClue = val;
        }
        addedRoundToAlter.OnUpdateConfig(config);
        DebugLog.Print("Round config updated...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Orange);
        OnDiscardConfig();
    }
}
