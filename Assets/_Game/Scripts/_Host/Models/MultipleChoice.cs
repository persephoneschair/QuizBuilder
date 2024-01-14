using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleChoice : BaseQuestion
{
    public Question question;
    public List<Answer> answers = new List<Answer>();

    public MultipleChoice()
    {

    }

    public MultipleChoice(Question question, List<Answer> answers)
    {
        questionType = QuestionType.MultiChoice;
        this.question = question;
        this.answers = answers;
    }
}
