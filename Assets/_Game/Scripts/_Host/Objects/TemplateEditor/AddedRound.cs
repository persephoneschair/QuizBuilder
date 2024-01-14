using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AddedRound : MonoBehaviour
{
    public RoundType.Round round;

    public TextMeshProUGUI label;
    public Button removeButton;
    public Config config;
    public Guid roundID;

    public void Init(RoundType.Round rnd, string label, Guid id, Config cnf = null)
    {
        this.roundID = id;
        this.label.text = label;
        this.round = rnd;
        if (cnf != null)
            this.config = cnf;
        switch (rnd)
        {
            case RoundType.Round.SimpleQAndA:
            case RoundType.Round.RedHerrings:
            case RoundType.Round.SimpleMC:
                if(config == null)
                    config = new SimpleConfig();
                break;

            case RoundType.Round.BiddingQAndA:
            case RoundType.Round.BiddingMC:
                if (config == null)
                    config = new BiddingConfig();
                break;

            case RoundType.Round.Escalator:
                if (config == null)
                    config = new EscalatorConfig();
                break;

            case RoundType.Round.Wipeout:
                if (config == null)
                    config = new WipeoutConfig();
                break;

            case RoundType.Round.Pointless:
                if (config == null)
                    config = new PointlessConfig();
                break;

            case RoundType.Round.Dripfeed:
                if (config == null)
                    config = new DripfeedConfig();
                break;
        }
    }

    public void OnEditSettings()
    {
        ConfigEditor.Get.ActivateEditor(config, this, label.text);
    }

    public void OnRemove()
    {
        QuizTemplateManager.Get.OnRemoveRound(this);
    }

    public void OnUpdateConfig(Config updatedConfigFile)
    {
        this.config = updatedConfigFile;
        label.text = this.config.roundDisplayName;
    }
}
