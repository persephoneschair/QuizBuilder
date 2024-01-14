using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TemplatePreview : MonoBehaviour
{
    public QuizTemplate templatePreview;
    public TextMeshProUGUI previewMesh;

    public void Initiate(QuizTemplate qt)
    {
        templatePreview = qt;
        previewMesh.text = qt.templateName;
    }

    public void OnDeleteTemplate()
    {
        QuizTemplateManager.Get.OnInitialDelete(this);
    }

    public void OnConfirmDeleteTemplate()
    {
        //Loop over all Quizzes and delete them if they use this template
        List<Quiz> quizzes = StorageManager.Quizzes.Values.Where(x => x.quizTemplate.templateID == templatePreview.templateID).ToList();
        foreach (Quiz qz in quizzes)
            if (qz.quizTemplate.templateID == templatePreview.templateID)
                qz.DeleteQuiz();

        StorageManager.DeleteTemplate(templatePreview);
        QuizTemplateManager.Get.RefreshPreviews();
    }

    public void OnEditTemplate()
    {
        QuizTemplateManager.Get.EditingTemplate = templatePreview.templateID;
    }
}
