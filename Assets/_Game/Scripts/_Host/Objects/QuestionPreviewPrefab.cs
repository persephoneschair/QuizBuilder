using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionPreviewPrefab : MonoBehaviour
{
    public Guid containedID;
    public BaseQuestion.QuestionType qType;
    public TextMeshProUGUI typeMesh;
    public TextMeshProUGUI previewMesh;

    public TextMeshProUGUI answerPreview;

    public void Initiate(Guid qID, BaseQuestion.QuestionType type, string qText, string answerText = "")
    {
        containedID = qID;
        qType = type;
        if(typeMesh != null)
            typeMesh.text = type == BaseQuestion.QuestionType.Simple ? "Simple Question" : type == BaseQuestion.QuestionType.MultiChoice ? "Multiple Choice Question" : "Sequential Question";
        previewMesh.text = string.IsNullOrEmpty(qText) ? "No question" : qText;

        if (answerPreview != null)
            answerPreview.text = answerText;
    }


    public void OnDeleteQuestion()
    {
        QuestionCreatorManager.Get.OnInitialDelete(this);
    }

    public void OnConfirmDeleteQuestion()
    {
        StorageManager.DeleteQuestion(containedID, qType);
        QuestionCreatorManager.Get.OnFilterChange();
    }

    public void OnEditQuestion()
    {
        QuestionCreatorManager.Get.EditingQID = containedID;
    }

    public void OnRemoveQuestionFromQuiz()
    {
        BuildQuizManager.Get.RemoveQuestion(containedID);
    }

    public void OnAddQuestionToQuiz()
    {
        BuildQuizManager.Get.AddInQuestion(containedID);
    }
}
