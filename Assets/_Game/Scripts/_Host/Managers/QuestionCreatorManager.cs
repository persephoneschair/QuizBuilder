using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestionCreatorManager : SingletonMonoBehaviour<QuestionCreatorManager>
{
    public List<CreateQButton> buttons;
    public Button mainMenuButton;
    public List<GameObject> creatorObjects;

    [Header("Simple Q Data Fields")]
    public TMP_InputField simpleQInput;
    public TMP_InputField simpleCatInput;
    public TextMeshProUGUI simpleQAssetPath;
    public TMP_InputField simpleQHostNotesInput;

    public TMP_InputField simpleAnsInput;
    public TMP_InputField simpleAccInput;
    public TextMeshProUGUI simpleAnsAssetPath;
    public TMP_InputField simpleAnsHostNotesInput;

    [Header("Multi Choice Data Fields")]
    public TMP_InputField multiQInput;
    public TMP_InputField multiCatInput;
    public TextMeshProUGUI multiQAssetPath;
    public TMP_InputField multiQHostNotesInput;

    public GameObject labellessAnswerObject;
    public Transform multiChoiceTargetTransform;
    public List<MultichoiceAnswerPrefab> instancedMCPrefabs = new List<MultichoiceAnswerPrefab>();

    [Header("Sequential Data Fields")]
    public TMP_InputField sequentialQInput;
    public TMP_InputField sequentialCatInput;
    public TextMeshProUGUI sequentialQAssetPath;
    public TMP_InputField sequentialQHostNotesInput;

    public GameObject labellessSeqObject;
    public Transform seqTargetTransform;
    public List<ClueAnswerPrefab> instancedSQPrefabs = new List<ClueAnswerPrefab>();

    public TMP_InputField sequentialAnsInput;
    public TMP_InputField sequentialAccInput;
    public TextMeshProUGUI sequentialAnsAssetPath;
    public TMP_InputField sequentialAnsHostNotesInput;

    [Header("Editor Fields")]
    public GameObject previewPrefab;
    public Transform previewTargetTransform;
    public List<QuestionPreviewPrefab> instancedPreviews = new List<QuestionPreviewPrefab>();
    public TMP_Dropdown dropdownFilter;
    public Toggle showStackedToggle;

    #region Confirm Deletion

    [Header("Confirm Deletion")]
    public GameObject confirmationBox;
    public Button confirmDeleteButton;
    public Button cancelDeleteButton;
    private QuestionPreviewPrefab questionToDelete;

    public void OnInitialDelete(QuestionPreviewPrefab prefab)
    {
        questionToDelete = prefab;
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
        questionToDelete.OnConfirmDeleteQuestion();
        OnCancelDelete();
    }

    public void OnCancelDelete()
    {
        mainMenuButton.interactable = true;
        questionToDelete = null;
        confirmationBox.SetActive(false);
    }

    #endregion

    private CreateQButton.QButton selectedOption;
    public CreateQButton.QButton SelectedOption
    {
        get
        {
            return selectedOption;
        }
        set
        {
            //x is the previous state - used to determine which list to save
            var x = selectedOption;
            selectedOption = value;
            SetActiveCreator(x);
        }
    }

    private Guid? editingQID;
    public Guid? EditingQID
    {
        get
        {
            return editingQID;
        }
        set
        {
            editingQID = value;
            if(value != null)
            {
                if (StorageManager.GetSimpleQuestion(value) != null)
                    RecoverSimpleQuestion(value);
                else if (StorageManager.GetMCQuestion(value) != null)
                    RecoverMCQuestion(value);
                else if (StorageManager.GetSeqQuestion(value) != null)
                    RecoverSequentialQuestion(value);
            }
        }
    }

    private void SetActiveCreator(CreateQButton.QButton x)
    {
        switch(selectedOption)
        {
            case CreateQButton.QButton.Save:
                foreach (GameObject go in creatorObjects)
                    go.gameObject.SetActive(false);
                foreach (CreateQButton qb in buttons)
                    qb.ToggleButtonInteractivity();

                switch(x)
                {
                    case CreateQButton.QButton.Simple:
                        CreateSimpleQuestion();
                        ClearSimpleDataFields();
                        break;

                    case CreateQButton.QButton.MultipleChoice:
                        CreateMultiChoiceQuestion();
                        ClearMultiDataField();
                        break;

                    case CreateQButton.QButton.Sequential:
                        CreateSequentialQuestion();
                        ClearSequentialDataFields();
                        break;
                }
                EditingQID = null;
                mainMenuButton.interactable = true;
                SelectedOption = CreateQButton.QButton.Edit;
                OnFilterChange();
                break;

            case CreateQButton.QButton.Discard:
                foreach (GameObject go in creatorObjects)
                    go.gameObject.SetActive(false);
                foreach (CreateQButton qb in buttons)
                    qb.ToggleButtonInteractivity();
                ClearSimpleDataFields();
                ClearMultiDataField();
                ClearSequentialDataFields();

                EditingQID = null;
                mainMenuButton.interactable = true;
                SelectedOption = CreateQButton.QButton.Edit;
                OnFilterChange();
                break;

            case CreateQButton.QButton.Edit:
                foreach (GameObject go in creatorObjects)
                    go.gameObject.SetActive(false);
                creatorObjects[(int)selectedOption].SetActive(true);
                InitiateEditor();
                mainMenuButton.interactable = true;
                break;

            default:
                mainMenuButton.interactable = false;
                foreach (GameObject go in creatorObjects)
                    go.gameObject.SetActive(false);

                creatorObjects[(int)selectedOption].SetActive(true);
                foreach (CreateQButton qb in buttons)
                    qb.ToggleButtonInteractivity();

                if (selectedOption == CreateQButton.QButton.MultipleChoice)
                    InitiateMultiChoice();
                if (selectedOption == CreateQButton.QButton.Sequential)
                    InitiateSequential();
                break;
        }
    }

    #region Recover Qs For Editing

    private void RecoverSimpleQuestion(Guid? value)
    {
        SimpleQuestion q = StorageManager.GetSimpleQuestion(value);
        SelectedOption = CreateQButton.QButton.Simple;

        simpleQInput.text = q.question.questionText;
        simpleCatInput.text = q.question.categoryText;
        simpleQAssetPath.text = string.IsNullOrEmpty(q.question.assetPath) ? "None" : q.question.assetPath;
        simpleQHostNotesInput.text = q.question.hostNotes;

        simpleAnsInput.text = q.answer.answerText;
        simpleAccInput.text = string.Join("|", q.answer.validAnswers);
        simpleAnsAssetPath.text = string.IsNullOrEmpty(q.answer.assetPath) ? "None" : q.answer.assetPath;
        simpleAnsHostNotesInput.text = q.answer.hostNotes;
    }

    private void RecoverMCQuestion(Guid? value)
    {
        MultipleChoice q = StorageManager.GetMCQuestion(value);
        SelectedOption = CreateQButton.QButton.MultipleChoice;

        foreach (MultichoiceAnswerPrefab mp in instancedMCPrefabs)
            Destroy(mp.gameObject);
        instancedMCPrefabs.Clear();

        multiQInput.text = q.question.questionText;
        multiCatInput.text = q.question.categoryText;
        multiQAssetPath.text = string.IsNullOrEmpty(q.question.assetPath) ? "None" : q.question.assetPath;
        multiQHostNotesInput.text = q.question.hostNotes;

        foreach (Answer a in q.answers)
        {
            CreateSingleAnswer();
            var pr = instancedMCPrefabs.LastOrDefault();
            if (pr != null)
            {
                pr.ansInput.text = a.answerText;
                pr.accInput.text = string.Join("|", a.validAnswers);
                pr.isCorrectToggle.isOn = a.isCorrect;
                pr.ansAssetPath.text = string.IsNullOrEmpty(a.assetPath) ? "None" : a.assetPath;
                pr.ansHostNotes.text = a.hostNotes;
            }
        }
    }

    private void RecoverSequentialQuestion(Guid? value)
    {
        Sequential q = StorageManager.GetSeqQuestion(value);
        SelectedOption = CreateQButton.QButton.Sequential;

        foreach (ClueAnswerPrefab cp in instancedSQPrefabs)
            Destroy(cp.gameObject);
        instancedSQPrefabs.Clear();

        sequentialQInput.text = q.question.questionText;
        sequentialCatInput.text = q.question.categoryText;
        sequentialQAssetPath.text = string.IsNullOrEmpty(q.question.assetPath) ? "None" : q.question.assetPath;
        sequentialQHostNotesInput.text = q.question.hostNotes;

        sequentialAnsInput.text = q.answer.answerText;
        sequentialAccInput.text = string.Join("|", q.answer.validAnswers);
        sequentialAnsAssetPath.text = string.IsNullOrEmpty(q.answer.assetPath) ? "None" : q.answer.assetPath;
        sequentialAnsHostNotesInput.text = q.answer.hostNotes;

        foreach (Clue c in q.clues)
        {
            CreateSingleClue();
            var pr = instancedSQPrefabs.LastOrDefault();
            if (pr != null)
            {
                pr.ansInput.text = c.clueText;
                pr.ansAssetPath.text = string.IsNullOrEmpty(c.assetPath) ? "None" : c.assetPath;
            }
        }
    }


    #endregion

    #region Prefab Creation/Deletion

    private void InitiateMultiChoice()
    {
        foreach (MultichoiceAnswerPrefab mp in instancedMCPrefabs)
            Destroy(mp.gameObject);
        instancedMCPrefabs.Clear();
        for (int i = 0; i < 2; i++)
            CreateSingleAnswer();
    }

    private void InitiateSequential()
    {
        foreach (ClueAnswerPrefab cp in instancedSQPrefabs)
            Destroy(cp.gameObject);
        instancedSQPrefabs.Clear();
        for (int i = 0; i < 2; i++)
            CreateSingleClue();
    }

    private void InitiateEditor(int filter = 0, bool showStacked = false)
    {
        foreach (QuestionPreviewPrefab qp in instancedPreviews)
            Destroy(qp.gameObject);
        instancedPreviews.Clear();

        if(filter == 0 || filter == 1)
        {
            foreach (SimpleQuestion q in StorageManager.SimpleQuestions)
            {
                if (!showStacked && q.IsStacked)
                    continue;

                var x = Instantiate(previewPrefab, previewTargetTransform);
                var y = x.GetComponent<QuestionPreviewPrefab>();
                instancedPreviews.Add(y);
                y.Initiate(q.questionID, q.questionType, q.question.questionText);
            }
        }

        if(filter == 0 || filter == 2)
        {
            foreach (MultipleChoice q in StorageManager.MultiChoiceQuestions)
            {
                if (!showStacked && q.IsStacked)
                    continue;

                var x = Instantiate(previewPrefab, previewTargetTransform);
                var y = x.GetComponent<QuestionPreviewPrefab>();
                instancedPreviews.Add(y);
                y.Initiate(q.questionID, q.questionType, q.question.questionText);
            }
        }

        if(filter == 0 || filter == 3)
        {
            foreach (Sequential q in StorageManager.SequentialQuestions)
            {
                if (!showStacked && q.IsStacked)
                    continue;

                var x = Instantiate(previewPrefab, previewTargetTransform);
                var y = x.GetComponent<QuestionPreviewPrefab>();
                instancedPreviews.Add(y);
                y.Initiate(q.questionID, q.questionType, q.question.questionText);
            }
        }
    }

    public void OnFilterChange()
    {
        InitiateEditor(dropdownFilter.value, showStackedToggle.isOn);
    }

    public void CreateSingleAnswer()
    {
        var x = Instantiate(labellessAnswerObject, multiChoiceTargetTransform);
        var y = x.GetComponent<MultichoiceAnswerPrefab>();
        instancedMCPrefabs.Add(y);
    }

    public void CreateSingleClue()
    {
        var x = Instantiate(labellessSeqObject, seqTargetTransform);
        var y = x.GetComponent<ClueAnswerPrefab>();
        instancedSQPrefabs.Add(y);
    }

    public void DeleteAnswer(MultichoiceAnswerPrefab mcP)
    {
        Destroy(instancedMCPrefabs.FirstOrDefault(x => x == mcP).gameObject);
        instancedMCPrefabs.Remove(mcP);
    }

    public void DeleteClue(ClueAnswerPrefab cp)
    {
        Destroy(instancedSQPrefabs.FirstOrDefault(x => x == cp).gameObject);
        instancedSQPrefabs.Remove(cp);
    }

    #endregion

    #region Question Creator

    public void CreateSimpleQuestion()
    {
        Question q = new Question(simpleQInput.text, simpleCatInput.text, simpleQAssetPath.text == "None" ? "" : simpleQAssetPath.text, simpleQHostNotesInput.text);
        Answer a = new Answer(simpleAnsInput.text, simpleAccInput.text.Split('|').ToList(), true, simpleAnsAssetPath.text == "None" ? "" : simpleAnsAssetPath.text, simpleAnsHostNotesInput.text);

        if(editingQID != null)
            StorageManager.UpdateQuestion(editingQID, q, a);
        else
            StorageManager.SaveQuestion(new SimpleQuestion(q, a));
    }

    public void CreateMultiChoiceQuestion()
    {
        Question q = new Question(multiQInput.text, multiCatInput.text, multiQAssetPath.text == "None" ? "" : multiQAssetPath.text, multiQHostNotesInput.text);
        List<Answer> ans = new List<Answer>();
        foreach (MultichoiceAnswerPrefab mc in instancedMCPrefabs)
            ans.Add(new Answer(mc.ansInput.text, mc.accInput.text.Split('|').ToList(), mc.isCorrectToggle.isOn, mc.ansAssetPath.text == "None" ? "" : mc.ansAssetPath.text, mc.ansHostNotes.text));

        if (editingQID != null)
            StorageManager.UpdateQuestion(editingQID, q, ans);
        else
            StorageManager.SaveQuestion(new MultipleChoice(q, ans));
    }

    public void CreateSequentialQuestion()
    {
        Question q = new Question(sequentialQInput.text, sequentialCatInput.text, sequentialQAssetPath.text == "None" ? "" : sequentialQAssetPath.text, sequentialQHostNotesInput.text);
        Answer a = new Answer(sequentialAnsInput.text, sequentialAccInput.text.Split('|').ToList(), true, sequentialAnsAssetPath.text == "None" ? "" : sequentialAnsAssetPath.text, sequentialAnsHostNotesInput.text);
        List<Clue> clues = new List<Clue>();

        foreach (ClueAnswerPrefab sq in instancedSQPrefabs)
            clues.Add(new Clue(sq.ansInput.text, sq.ansAssetPath.text == "None" ? "" : sq.ansAssetPath.text));

        if (editingQID != null)
            StorageManager.UpdateQuestion(editingQID, q, clues, a);
        else
            StorageManager.SaveQuestion(new Sequential(q, clues, a));
    }

    #endregion

    #region Clear Data Fields

    public void ClearSimpleDataFields()
    {
        simpleQInput.text = string.Empty;
        simpleCatInput.text = string.Empty;
        simpleQAssetPath.text = "None";
        simpleQHostNotesInput.text = string.Empty;

        simpleAnsInput.text = string.Empty;
        simpleAccInput.text = string.Empty;
        simpleAnsAssetPath.text = "None";
        simpleAnsHostNotesInput.text = string.Empty;
    }

    public void ClearMultiDataField()
    {
        multiQInput.text = string.Empty;
        multiCatInput.text = string.Empty;
        multiQAssetPath.text = "None";
        multiQHostNotesInput.text = string.Empty;
    }

    public void ClearSequentialDataFields()
    {
        sequentialQInput.text = string.Empty;
        sequentialCatInput.text = string.Empty;
        sequentialQAssetPath.text = "None";
        sequentialQHostNotesInput.text = string.Empty;

        sequentialAnsInput.text = string.Empty;
        sequentialAccInput.text = string.Empty;
        sequentialAnsAssetPath.text = "None";
        sequentialAnsHostNotesInput.text = string.Empty;
    }

    #endregion
}
