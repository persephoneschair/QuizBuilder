using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuButton : MonoBehaviour
{
    public enum MenuOption
    {
        RunQuiz,
        CreateTemplate,
        ImportQuestions,
        EditQuestions,
        BuildQuiz,
        Settings,
        Documentation,
        License,
        Quit,
        None
    }

    public MenuOption option;

    public void PressMenuButton()
    {
        switch(option)
        {
            case MenuOption.Documentation:
                DebugLog.Print("Open documentation website", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Blue);
                break;

            case MenuOption.Quit:
                DebugLog.Print("QUITTING TO DESKTOP", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Red);
                Application.Quit();
                break;

            default:
                MainMenuManager.Get.SelectedOption = option;
                break;
        }
    }
}
