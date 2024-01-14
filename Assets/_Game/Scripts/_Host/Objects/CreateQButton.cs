using SimpleFileBrowser;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateQButton : MonoBehaviour
{
    private Button btn;
    public MultichoiceAnswerPrefab mcP;
    public ClueAnswerPrefab cp;

    public TextMeshProUGUI assetPathMesh;

    private void Start()
    {
        btn = GetComponent<Button>();
        if (GetComponentInParent<MultichoiceAnswerPrefab>() != null)
            mcP = GetComponentInParent<MultichoiceAnswerPrefab>();
        if (GetComponentInParent<ClueAnswerPrefab>() != null)
            cp = GetComponentInParent<ClueAnswerPrefab>();
    }

    public void ToggleButtonInteractivity()
    {
        btn.interactable = !btn.interactable;
    }

    public enum QButton
    {
        Simple,
        MultipleChoice,
        Sequential,
        Edit,
        Save,
        Discard,
        AddNewAnswer,
        DeleteAnswer,
        AddNewClue,
        DeleteClue,
        ImportAsset,
        ClearAsset
    }

    public QButton option;

    public void PressCreateButton()
    {
        switch(option)
        {
            case QButton.ImportAsset:
                SetFiltersToAsset();
                StartCoroutine(ShowLoadDialogCoroutine());
                break;

            case QButton.ClearAsset:
                assetPathMesh.text = "None";
                break;

            case QButton.DeleteAnswer:
                QuestionCreatorManager.Get.DeleteAnswer(mcP);
                break;

            case QButton.AddNewAnswer:
                QuestionCreatorManager.Get.CreateSingleAnswer();
                break;

            case QButton.DeleteClue:
                QuestionCreatorManager.Get.DeleteClue(cp);
                break;

            case QButton.AddNewClue:
                QuestionCreatorManager.Get.CreateSingleClue();
                break;

            default:
                QuestionCreatorManager.Get.SelectedOption = option;
                break;
        }
    }

    public void SetFiltersToAsset()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Assets", ".jpg", ".jpeg", ".png", ".wav", ".ogg"));
        FileBrowser.SetDefaultFilter(".jpg");
        MainMenuManager.Get.fileBrowserBlocker.SetActive(true);
    }

    public IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Set Asset", "Select");
        if (FileBrowser.Success)
            assetPathMesh.text = FileBrowser.Result.FirstOrDefault();
        MainMenuManager.Get.fileBrowserBlocker.SetActive(false);
    }
}
