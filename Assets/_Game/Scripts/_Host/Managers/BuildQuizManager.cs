using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildQuizManager : SingletonMonoBehaviour<BuildQuizManager>
{
    [Header("Builder Overview")]
    public GameObject editorList;

    [Header("Buttons")]
    public Button mainMenuButton;
    public Button newQuizButton;
    public Button saveButton;
    public Button discardButton;

    [Header("Editor Overview")]
    public Transform quizPreviewTransformTarget;
    public GameObject quizPreviewPrefab;
    public List<QuizPreviewPrefab> quizPreviews = new List<QuizPreviewPrefab>();
    public TMP_Dropdown stackFilter;

    #region Confirm Deletion

    [Header("Confirm Deletion")]
    public GameObject confirmationBox;
    public Button confirmDeleteButton;
    public Button cancelDeleteButton;
    private QuizPreviewPrefab quizToDelete;

    public void OnInitialDelete(QuizPreviewPrefab prefab)
    {
        quizToDelete = prefab;
        if (SettingsManager.Get.CurrentSettings.promptBeforeDeleting)
        {
            confirmationBox.SetActive(true);
            mainMenuButton.interactable = false;
        }
        else
            OnConfirmDelete();
    }

    public void OnConfirmDelete()
    {
        quizToDelete.OnConfirmDeleteQuiz();
        OnCancelDelete();
    }

    public void OnCancelDelete()
    {
        mainMenuButton.interactable = true;
        quizToDelete = null;
        confirmationBox.SetActive(false);
    }

    #endregion

    private Guid? editingQuiz;
    public Guid? EditingQuiz
    {
        get
        {
            return editingQuiz;
        }
        set
        {
            editingQuiz = value;
            if (value != null && StorageManager.Quizzes.ContainsKey(value.Value))
                OnLoadQuizFromPreview(StorageManager.Quizzes[value.Value]);

        }
    }

    private Quiz quizUnderConstruction;
    public Quiz QuizUnderConstruction
    {
        get
        {
            return quizUnderConstruction;
        }
        set
        {
            quizUnderConstruction = value;
            //if(value != null && value.quizTemplate != null)
        }
    }


    [Header("Editing Quiz")]
    public GameObject quizCreatorObj;
    public TMP_Dropdown templateDropdown;
    public TMP_Dropdown roundDropdown;
    public TMP_InputField quizNameInput;
    public Transform questionAddedTransformTarget;
    public GameObject questionAddedPreview;
    public List<QuestionPreviewPrefab> addedQuestionPreviewPrefabs = new List<QuestionPreviewPrefab>();

    public Transform potentialQuestionsTransformTarget;
    public GameObject potentialQuestionPreview;
    public List<QuestionPreviewPrefab> potentialQuestionPreviewPrefabs = new List<QuestionPreviewPrefab>();

    public void OnNewQuiz()
    {
        ClearPopulatedUnusedQuestions();
        roundDropdown.ClearOptions();
        ClearCurrentQuiz();
        EditingQuiz = null;
        quizNameInput.text = "";
        OnChangeRound();
        ToggleButtonFunctions(true);
        PopulateTemplateDropdown();

        QuizUnderConstruction = new Quiz();
    }

    public void OnLoadQuizFromPreview(Quiz qz)
    {
        ClearPopulatedUnusedQuestions();
        roundDropdown.ClearOptions();

        ClearCurrentQuiz();
        quizNameInput.text = qz.quizName;
        QuizUnderConstruction = new Quiz(qz.quizTemplate, qz.quizCreationDate, qz.quizName, qz.quizID);

        PopulateTemplateDropdown();
        templateDropdown.value = Array.IndexOf(StorageManager.QuizTemplates.ToArray(), StorageManager.QuizTemplates.FirstOrDefault(x => x.templateID == qz.quizTemplate.templateID)) + 1;

        PopulateRoundDropdown();
        roundDropdown.value = 0;
        OnChangeRound();

        ToggleButtonFunctions(true);
    }

    public void OnSaveQuiz()
    {
        if (EditingQuiz == null)
        {
            Quiz persistQuiz = new Quiz(QuizUnderConstruction.quizTemplate, DateTime.Now, quizNameInput.text, QuizUnderConstruction.quizID);
            StorageManager.SaveQuiz(persistQuiz);
        }
        else
        {
            Quiz persistQuiz = new Quiz(QuizUnderConstruction.quizTemplate, DateTime.Now, quizNameInput.text, EditingQuiz.Value);
            StorageManager.UpdateQuiz(persistQuiz, quizNameInput.text);
        }

        StorageManager.ReSaveAllQuestions();
        OnDiscardQuiz();
        RefreshPreviews();
    }

    public void OnDiscardQuiz()
    {
        ClearCurrentQuiz();
        EditingQuiz = null;
        ToggleButtonFunctions(false);
    }

    public void OnChangeTemplate()
    {
        if (QuizUnderConstruction == null)
            return;

        if (templateDropdown.value != 0)
        {
            //Off by one to account for "None" value
            var tmp = StorageManager.QuizTemplates[templateDropdown.value - 1];
            if (EditingQuiz == null)
                QuizUnderConstruction.quizTemplate = tmp.DeepCopy();

            PopulateRoundDropdown();
        }
        else
        {
            QuizUnderConstruction.quizTemplate = null;
            roundDropdown.ClearOptions();
            ClearPopulatedUnusedQuestions();
        }
    }

    public void OnChangeRound()
    {
        foreach (QuestionPreviewPrefab qp in addedQuestionPreviewPrefabs)
            Destroy(qp.gameObject);

        foreach (QuestionPreviewPrefab qp in potentialQuestionPreviewPrefabs)
            Destroy(qp.gameObject);

        addedQuestionPreviewPrefabs.Clear();
        potentialQuestionPreviewPrefabs.Clear();

        if (roundDropdown.value == 0)
            return;

        if (QuizUnderConstruction.quizTemplate.rounds[roundDropdown.value - 1].questionStack.questionIDs == null)
            QuizUnderConstruction.quizTemplate.rounds[roundDropdown.value - 1].questionStack.questionIDs = new List<Guid>();

        BaseQuestion.QuestionType qt = GetQuestionType(QuizUnderConstruction.quizTemplate.rounds[roundDropdown.value - 1].roundType);

        switch(qt)
        {
            case BaseQuestion.QuestionType.Simple:
                PopulateSimpleQs();
                break;

            case BaseQuestion.QuestionType.MultiChoice:
                PopulateMCQs();
                break;

            case BaseQuestion.QuestionType.Sequential:
                PopulateSeqQs();
                break;
        }

        foreach (Guid id in QuizUnderConstruction.quizTemplate.rounds[roundDropdown.value - 1].questionStack.questionIDs)
        {
            var x = Instantiate(questionAddedPreview, questionAddedTransformTarget);
            var y = x.GetComponent<QuestionPreviewPrefab>();

            if (StorageManager.GetSimpleQuestion(id) != null)
                y.Initiate(id, qt, StorageManager.GetSimpleQuestion(id).question.questionText, StorageManager.GetSimpleQuestion(id).answer.answerText);
            else if (StorageManager.GetMCQuestion(id) != null)
                y.Initiate(id, qt, StorageManager.GetMCQuestion(id).question.questionText, StorageManager.GetMCQuestion(id).answers.FirstOrDefault(x => x.isCorrect).answerText);
            else if (StorageManager.GetSeqQuestion(id) != null)
                y.Initiate(id, qt, StorageManager.GetSeqQuestion(id).question.questionText, StorageManager.GetSeqQuestion(id).answer.answerText);

            addedQuestionPreviewPrefabs.Add(y);
        }
    }

    private BaseQuestion.QuestionType GetQuestionType(RoundType.Round rnd)
    {
        switch (rnd)
        {
            case RoundType.Round.SimpleQAndA:
            case RoundType.Round.BiddingQAndA:
            case RoundType.Round.RedHerrings:
                return BaseQuestion.QuestionType.Simple;

            case RoundType.Round.SimpleMC:
            case RoundType.Round.BiddingMC:
            case RoundType.Round.Escalator:
            case RoundType.Round.Wipeout:
            case RoundType.Round.Pointless:
                return BaseQuestion.QuestionType.MultiChoice;

            case RoundType.Round.Dripfeed:
            case RoundType.Round.ConnectionGrid:
                return BaseQuestion.QuestionType.Sequential;

            //Define a null value in the enum
            default:
                return BaseQuestion.QuestionType.Simple;
        }
    }

    private void PopulateSimpleQs()
    {
        ClearPopulatedUnusedQuestions();

        if (roundDropdown.value == 0)
            return;

        foreach (SimpleQuestion sq in StorageManager.SimpleQuestions.Where(x => !QuizUnderConstruction.quizTemplate.rounds[roundDropdown.value - 1].questionStack.questionIDs.Contains(x.questionID)).ToList())
        {
            switch(stackFilter.value)
            {
                //Show Only Unstacked Questions
                case 0:
                    if (sq.IsStacked)
                        continue;
                    break;

                //Show Only Questions Not In This Quiz
                case 1:
                    if (StorageManager.GetAllQGuidsForQuiz(QuizUnderConstruction).Contains(sq.questionID))
                        continue;

                    break;

                //Show All Questions
                default:
                    break;
            }

            var x = Instantiate(potentialQuestionPreview, potentialQuestionsTransformTarget);
            var y = x.GetComponent<QuestionPreviewPrefab>();
            y.Initiate(sq.questionID, BaseQuestion.QuestionType.Simple, sq.question.questionText, sq.answer.answerText);
            potentialQuestionPreviewPrefabs.Add(y);
        }
    }

    private void PopulateMCQs()
    {
        ClearPopulatedUnusedQuestions();

        if (roundDropdown.value == 0)
            return;

        foreach (MultipleChoice mq in StorageManager.MultiChoiceQuestions.Where(x => !QuizUnderConstruction.quizTemplate.rounds[roundDropdown.value - 1].questionStack.questionIDs.Contains(x.questionID)).ToList())
        {
            switch (stackFilter.value)
            {
                //Show Only Unstacked Questions
                case 0:
                    if (mq.IsStacked)
                        continue;
                    break;

                //Show Only Questions Not In This Quiz
                case 1:
                    if (StorageManager.GetAllQGuidsForQuiz(QuizUnderConstruction).Contains(mq.questionID))
                        continue;

                    break;

                //Show All Questions
                default:
                    break;
            }

            var x = Instantiate(potentialQuestionPreview, potentialQuestionsTransformTarget);
            var y = x.GetComponent<QuestionPreviewPrefab>();
            y.Initiate(mq.questionID, BaseQuestion.QuestionType.MultiChoice, mq.question.questionText, mq.answers.FirstOrDefault().answerText);
            potentialQuestionPreviewPrefabs.Add(y);
        }
    }

    private void PopulateSeqQs()
    {
        ClearPopulatedUnusedQuestions();

        if (roundDropdown.value == 0)
            return;

        foreach (Sequential sq in StorageManager.SequentialQuestions.Where(x => !QuizUnderConstruction.quizTemplate.rounds[roundDropdown.value - 1].questionStack.questionIDs.Contains(x.questionID)).ToList())
        {
            switch (stackFilter.value)
            {
                //Show Only Unstacked Questions
                case 0:
                    if (sq.IsStacked)
                        continue;
                    break;

                //Show Only Questions Not In This Quiz
                case 1:
                    if (StorageManager.GetAllQGuidsForQuiz(QuizUnderConstruction).Contains(sq.questionID))
                        continue;

                    break;

                //Show All Questions
                default:
                    break;
            }
            var x = Instantiate(potentialQuestionPreview, potentialQuestionsTransformTarget);
            var y = x.GetComponent<QuestionPreviewPrefab>();
            y.Initiate(sq.questionID, BaseQuestion.QuestionType.Sequential, sq.question.questionText, sq.answer.answerText);
            potentialQuestionPreviewPrefabs.Add(y);
        }
    }

    private void ClearPopulatedUnusedQuestions()
    {
        foreach (QuestionPreviewPrefab qp in potentialQuestionPreviewPrefabs)
            Destroy(qp.gameObject);

        potentialQuestionPreviewPrefabs.Clear();
    }

    public void AddQuizIDToQuestion(Guid questionID)
    {
        if (StorageManager.GetSimpleQuestion(questionID) != null)
            StorageManager.GetSimpleQuestion(questionID).quizIDs.Add(QuizUnderConstruction.quizID);
        else if (StorageManager.GetMCQuestion(questionID) != null)
            StorageManager.GetMCQuestion(questionID).quizIDs.Add(QuizUnderConstruction.quizID);
        else if (StorageManager.GetSeqQuestion(questionID) != null)
            StorageManager.GetSeqQuestion(questionID).quizIDs.Add(QuizUnderConstruction.quizID);
    }

    public void RemoveQuizIDFromQuestion(Guid questionID)
    {
        List<Guid> questionIdsInThisQuiz = QuizUnderConstruction.quizTemplate.rounds
                    .Where(r => r.questionStack != null && r.questionStack.questionIDs != null)
                    .SelectMany(r => r.questionStack.questionIDs)
                    .ToList();

        if(questionIdsInThisQuiz.Count(x => x == questionID) == 1)
        {
            if (StorageManager.GetSimpleQuestion(questionID) != null)
                StorageManager.GetSimpleQuestion(questionID).quizIDs.Remove(QuizUnderConstruction.quizID);
            else if (StorageManager.GetMCQuestion(questionID) != null)
                StorageManager.GetMCQuestion(questionID).quizIDs.Remove(QuizUnderConstruction.quizID);
            else if (StorageManager.GetSeqQuestion(questionID) != null)
                StorageManager.GetSeqQuestion(questionID).quizIDs.Remove(QuizUnderConstruction.quizID);
        }
    }

    public void AddInQuestion(Guid questionID)
    {
        AddQuizIDToQuestion(questionID);
        QuizUnderConstruction.quizTemplate.rounds[roundDropdown.value - 1].questionStack.questionIDs.Add(questionID);
        OnChangeRound();
    }

    public void RemoveQuestion(Guid questionID)
    {
        RemoveQuizIDFromQuestion(questionID);
        QuizUnderConstruction.quizTemplate.rounds[roundDropdown.value - 1].questionStack.questionIDs.Remove(questionID);
        OnChangeRound();
    }

    private void PopulateTemplateDropdown()
    {
        templateDropdown.ClearOptions();

        List<string> templates = new List<string> { "None" };
        templates.AddRange(StorageManager.QuizTemplates.Select(x => x.templateName).ToList());
        templateDropdown.AddOptions(templates);
        templateDropdown.value = 0;        
    }

    public void PopulateRoundDropdown()
    {
        roundDropdown.ClearOptions();

        List<string> rounds = new List<string> { "None" };
        rounds.AddRange(QuizUnderConstruction.quizTemplate.rounds.Select(x => string.IsNullOrEmpty(x.roundConfig.roundDisplayName) ? x.roundTypeString : x.roundConfig.roundDisplayName).ToList());
        roundDropdown.AddOptions(rounds);
        roundDropdown.value = 0;
    }

    public void ToggleButtonFunctions(bool active)
    {
        mainMenuButton.interactable = !active;
        newQuizButton.interactable = !active;
        editorList.SetActive(!active);
        quizCreatorObj.SetActive(active);
        discardButton.interactable = active;
    }

    public void ClearCurrentQuiz()
    {
        if(QuizUnderConstruction != null)
            QuizUnderConstruction.quizTemplate = null;
        QuizUnderConstruction = null;
    }

    private void Update()
    {
        saveButton.interactable = (discardButton.interactable && quizNameInput.text.Length > 0 && QuizUnderConstruction != null && QuizUnderConstruction.quizTemplate != null);

        if ((QuizUnderConstruction != null && QuizUnderConstruction.quizTemplate != null && QuizUnderConstruction.quizTemplate.rounds.Any(x => x.questionStack?.questionIDs?.Count > 0)) || EditingQuiz != null)
            templateDropdown.interactable = false;
        else
            templateDropdown.interactable = true;
    }

    public void RefreshPreviews()
    {
        foreach (QuizPreviewPrefab qp in quizPreviews)
            Destroy(qp.gameObject);

        quizPreviews.Clear();

        foreach (Quiz qz in StorageManager.Quizzes.Values.OrderBy(x => x.quizName))
        {
            var x = Instantiate(quizPreviewPrefab, quizPreviewTransformTarget);
            var y = x.GetComponent<QuizPreviewPrefab>();
            y.Initiate(qz);
            quizPreviews.Add(y);
        }
    }
}
