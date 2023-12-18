using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionCreatorManager : SingletonMonoBehaviour<QuestionCreatorManager>
{
    public List<CreateQButton> buttons;
    public List<GameObject> creatorObjects;

    private CreateQButton.QButton selectedOption;
    public CreateQButton.QButton SelectedOption
    {
        get
        {
            return selectedOption;
        }
        set
        {
            selectedOption = value;
            SetActiveCreator();
        }
    }

    private void SetActiveCreator()
    {
        switch(selectedOption)
        {
            case CreateQButton.QButton.Save:
                foreach (GameObject go in creatorObjects)
                    go.gameObject.SetActive(false);
                foreach (CreateQButton qb in buttons)
                    qb.ToggleButtonInteractivity();
                break;

            case CreateQButton.QButton.Discard:
                foreach (GameObject go in creatorObjects)
                    go.gameObject.SetActive(false);
                foreach (CreateQButton qb in buttons)
                    qb.ToggleButtonInteractivity();
                break;

            case CreateQButton.QButton.Edit:
                break;

            default:
                creatorObjects[(int)selectedOption].SetActive(true);
                foreach (CreateQButton qb in buttons)
                    qb.ToggleButtonInteractivity();
                break;
        }
    }
}
