using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuizTemplate
{
    public Guid templateID;
    public string templateName = "";
    public List<RoundType> rounds = new List<RoundType>();

    public QuizTemplate()
    {

    }

    public QuizTemplate(string templateName, List<AddedRound> rounds, Guid id)
    {
        this.templateName = templateName;
        foreach (AddedRound ar in rounds)
            this.rounds.Add(new RoundType(ar.round, ar.config, ar.roundID));
        this.templateID = id;
    }

    public QuizTemplate(string templateName, List<RoundType> rounds, Guid id)
    {
        this.templateName = templateName;
        this.rounds = rounds;
        this.templateID = id;
    }

    public QuizTemplate DeepCopy()
    {
        QuizTemplate qzt = new QuizTemplate();
        qzt.templateName = this.templateName;
        foreach(RoundType round in this.rounds)
            qzt.rounds.Add(new RoundType(round.roundType, round.roundConfig, round.roundID));
        qzt.templateID = this.templateID;
        return qzt;
    }
}
