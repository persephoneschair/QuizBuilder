using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateQButton : MonoBehaviour
{
    private Button btn;

    private void Start()
    {
        btn = GetComponent<Button>();
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
        Discard
    }

    public QButton option;

    public void PressCreateButton()
    {
        QuestionCreatorManager.Get.SelectedOption = option;
    }
}
