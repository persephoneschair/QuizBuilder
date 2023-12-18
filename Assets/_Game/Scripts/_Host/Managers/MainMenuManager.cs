using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : SingletonMonoBehaviour<MainMenuManager>
{
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
        }
    }
}
