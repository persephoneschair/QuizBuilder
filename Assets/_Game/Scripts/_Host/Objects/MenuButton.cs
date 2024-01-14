using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    public Button button;

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

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void PressMenuButton()
    {
        switch(option)
        {
            case MenuOption.ImportQuestions:
                DebugLog.Print("Running importer...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Blue);
                ImportManager.Get.RunImport();
                break;

            case MenuOption.Documentation:
                DebugLog.Print("Opening documentation website...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Blue);
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
