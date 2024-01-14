using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequential : BaseQuestion
{
    public Question question;
    public List<Clue> clues;
    public Answer answer;

    public Sequential()
    {

    }

    public Sequential(Question question, List<Clue> clues, Answer answer)
    {
        questionType = QuestionType.Sequential;
        this.question = question;
        this.clues = clues;
        this.answer = answer;
    }
}
