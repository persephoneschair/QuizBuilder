using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quiz
{
    public QuizTemplate quizTemplate;
    public DateTime quizCreationDate;
    public string quizName;
    public Guid quizID;

    public Quiz()
    {
        quizID = Guid.NewGuid();
    }

    public Quiz(QuizTemplate quizTemplate, DateTime quizCreationDate, string quizName, Guid quizID)
    {
        this.quizTemplate = quizTemplate;
        this.quizCreationDate = quizCreationDate;
        this.quizName = quizName;
        this.quizID = quizID;
    }

    public void DeleteQuiz()
    {
        //Loop over all Qs and remove this QuizRef from their StackRefs
        var questionIdsInThisQuiz = StorageManager.GetAllQGuidsForQuiz(this);

        foreach (Guid g in questionIdsInThisQuiz)
        {
            if (StorageManager.GetSimpleQuestion(g) != null)
                StorageManager.GetSimpleQuestion(g).quizIDs.Remove(quizID);
            else if (StorageManager.GetMCQuestion(g) != null)
                StorageManager.GetMCQuestion(g).quizIDs.Remove(quizID);
            else if (StorageManager.GetSeqQuestion(g) != null)
                StorageManager.GetSeqQuestion(g).quizIDs.Remove(quizID);
        }

        StorageManager.DeleteQuiz(this);
        StorageManager.ReSaveAllQuestions();
    }
}
