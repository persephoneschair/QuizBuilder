using System;
using System.Collections.Generic;

public class BaseQuestion
{
    public enum QuestionType
    { 
        Simple,
        MultiChoice,
        Sequential
    }

    public Guid questionID;
    public QuestionType questionType;

    public HashSet<Guid> quizIDs = new HashSet<Guid>();
    public bool IsStacked
    {
        get
        {
            return quizIDs.Count > 0;
        }
    }

    public BaseQuestion()
    {
        questionID = Guid.NewGuid();
    }
}
