using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using System;

public class QuizPreviewPrefab : MonoBehaviour
{
    public Quiz quiz;
    public TextMeshProUGUI previewMesh;

    public void Initiate(Quiz qz)
    {
        quiz = qz;
        previewMesh.text = qz.quizName;
    }

    public void OnDeleteQuiz()
    {
        BuildQuizManager.Get.OnInitialDelete(this);
    }

    public void OnConfirmDeleteQuiz()
    {
        quiz.DeleteQuiz();
        BuildQuizManager.Get.RefreshPreviews();
    }

    public void OnEditQuiz()
    {
        BuildQuizManager.Get.EditingQuiz = quiz.quizID;
    }
}
