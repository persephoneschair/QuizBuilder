using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : SingletonMonoBehaviour<MainMenuManager>
{
    public GameObject fileBrowserBlocker;

    public List<Button> allAccessButtons;
    public List<MenuButton> buttons;
    public List<GameObject> menuObjects;

    private MenuButton.MenuOption selectedOption;
    public MenuButton.MenuOption SelectedOption
    { 
        get
        {
            return selectedOption;
        }
        set
        {
            selectedOption = value;
            SetActiveMenu();
        }
    }

    private void SetActiveMenu()
    {
        if(selectedOption == MenuButton.MenuOption.None)
        {
            foreach (GameObject go in menuObjects)
                go.gameObject.SetActive(false);
            foreach (MenuButton mb in buttons)
                mb.gameObject.SetActive(true);
        }
        else
        {
            menuObjects[(int)selectedOption].SetActive(true);
            foreach (MenuButton mb in buttons)
                mb.gameObject.SetActive(false);
            if (SelectedOption == MenuButton.MenuOption.EditQuestions)
                QuestionCreatorManager.Get.OnFilterChange();
            else if (SelectedOption == MenuButton.MenuOption.CreateTemplate)
                QuizTemplateManager.Get.RefreshPreviews();
            else if (SelectedOption == MenuButton.MenuOption.BuildQuiz)
                BuildQuizManager.Get.RefreshPreviews();
        }
    }

    public void LicensedProduct()
    {
        foreach (MenuButton mb in buttons)
            mb.button.interactable = true;
    }

    public void UnlicensedProduct()
    {
        foreach(MenuButton mb in buttons)
            mb.button.interactable = false;

        foreach (Button aab in allAccessButtons)
            aab.interactable = true;
    }
}
