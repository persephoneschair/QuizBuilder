using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuizTemplateManager : SingletonMonoBehaviour<QuizTemplateManager>
{
    [Header("Template Overview")]
    public Transform addRoundTransformTarget;
    public GameObject addRoundButton;
    public GameObject editorList;

    [Header("Buttons")]
    public Button mainMenuButton;
    public Button newTemplateButton;
    public Button saveButton;
    public Button discardButton;

    [Header("Editor Overview")]
    public Transform editorPreviewTransformTarget;
    public GameObject editorPreviewPrefab;
    public List<TemplatePreview> templatePreviews = new List<TemplatePreview>();

    private Guid? editingTemplate;
    public Guid? EditingTemplate
    {
        get
        {
            return editingTemplate;
        }
        set
        {
            editingTemplate = value;
            if(value != null && StorageManager.QuizTemplates.FirstOrDefault(x => x.templateID == value) != null)
                OnLoadTemplateFromPreview(StorageManager.QuizTemplates.FirstOrDefault(x => x.templateID == value));
        }
    }

    [Header("Editing Round")]
    public TMP_InputField templateNameInput;
    public GameObject templateCreatorObj;
    public Transform roundAddedTransformTarget;
    public GameObject addedRoundObject;
    public List<AddedRound> addedRounds = new List<AddedRound>();

    public void Start()
    {
        InitialiseAddRoundButtons();
    }

    private void InitialiseAddRoundButtons()
    {
        for (int i = 0; i < 10; i++)
        {
            var x = Instantiate(addRoundButton, addRoundTransformTarget);
            x.GetComponent<AddRoundButton>().Initialise(i);
        }
    }

    public void RefreshPreviews()
    {
        foreach (TemplatePreview tp in templatePreviews)
            Destroy(tp.gameObject);

        templatePreviews.Clear();

        foreach(QuizTemplate qt in StorageManager.QuizTemplates)
        {
            var x = Instantiate(editorPreviewPrefab, editorPreviewTransformTarget);
            var y = x.GetComponent<TemplatePreview>();
            y.Initiate(qt);
            templatePreviews.Add(y);
        }
    }

    #region Confirm Deletion

    [Header("Confirm Deletion")]
    public GameObject confirmationBox;
    public Button confirmDeleteButton;
    public Button cancelDeleteButton;
    private TemplatePreview templateToDelete;

    public void OnInitialDelete(TemplatePreview prefab)
    {
        templateToDelete = prefab;
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
        templateToDelete.OnConfirmDeleteTemplate();
        OnCancelDelete();
    }

    public void OnCancelDelete()
    {
        mainMenuButton.interactable = true;
        templateToDelete = null;
        confirmationBox.SetActive(false);
    }

    #endregion

    #region Button Functions

    private void Update()
    {
        saveButton.interactable = (discardButton.interactable && templateNameInput.text.Length > 0 && addedRounds.Count > 0);
    }

    public void ClearCurrentTemplate()
    {
        foreach (AddedRound ar in addedRounds)
            Destroy(ar.gameObject);

        templateNameInput.text = "";
        addedRounds.Clear();
    }

    public void ToggleButtonFunctions(bool active)
    {
        mainMenuButton.interactable = !active;
        newTemplateButton.interactable = !active;
        editorList.SetActive(!active);
        templateCreatorObj.SetActive(active);
        discardButton.interactable = active;
    }

    public void OnNewQuizTemplate()
    {
        ClearCurrentTemplate();
        EditingTemplate = null;
        ToggleButtonFunctions(true);
    }

    public void OnSaveTemplate()
    {
        if(EditingTemplate == null)
        {
            QuizTemplate qt = new QuizTemplate(templateNameInput.text, addedRounds, Guid.NewGuid());
            StorageManager.SaveTemplate(qt);
        }
        else
            StorageManager.UpdateTemplate(EditingTemplate, templateNameInput.text, addedRounds);

        RefreshPreviews();
        OnDiscardTemplate();
    }

    public void OnDiscardTemplate()
    {
        ClearCurrentTemplate();
        EditingTemplate = null;
        ToggleButtonFunctions(false);
    }

    public void OnAddRound(RoundType.Round rnd, string label, Guid id)
    {
        var x = Instantiate(addedRoundObject, roundAddedTransformTarget);
        var y = x.GetComponent<AddedRound>();
        y.Init(rnd, label, id);
        addedRounds.Add(y);
    }

    public void OnAddRound(RoundType.Round rnd, string label, Config config, Guid id)
    {
        var x = Instantiate(addedRoundObject, roundAddedTransformTarget);
        var y = x.GetComponent<AddedRound>();
        y.Init(rnd, string.IsNullOrEmpty(config.roundDisplayName) ? label : config.roundDisplayName, id, config);
        addedRounds.Add(y);
    }

    public void OnRemoveRound(AddedRound ar)
    {
        addedRounds.Remove(ar);
        Destroy(ar.gameObject);
    }

    public void OnLoadTemplateFromPreview(QuizTemplate qt)
    {
        ClearCurrentTemplate();
        templateNameInput.text = qt.templateName;
        foreach (RoundType r in qt.rounds)
            OnAddRound(r.roundType, r.roundTypeString, r.roundConfig, r.roundID);
        ToggleButtonFunctions(true);
    }


    #endregion
}
