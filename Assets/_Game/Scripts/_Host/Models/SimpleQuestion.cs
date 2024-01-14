using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleQuestion : BaseQuestion
{
    public Question question;
    public Answer answer;

    public SimpleQuestion()
    {

    }

    public SimpleQuestion(Question question, Answer answer)
    {
        questionType = QuestionType.Simple;
        this.question = question;
        this.answer = answer;
    }
}
