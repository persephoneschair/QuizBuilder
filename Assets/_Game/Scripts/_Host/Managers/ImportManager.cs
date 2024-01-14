using SimpleFileBrowser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using yutokun;

public class ImportManager : SingletonMonoBehaviour<ImportManager>
{
    public void RunImport()
    {
        SetFiltersToAsset();
        StartCoroutine(ShowLoadDialogCoroutine());
        MainMenuManager.Get.fileBrowserBlocker.SetActive(true);
    }


    public void SetFiltersToAsset()
    {
        FileBrowser.SetFilters(false, new FileBrowser.Filter("Question Sheet", ".csv"));
        FileBrowser.SetDefaultFilter(".csv");
    }

    public IEnumerator ShowLoadDialogCoroutine()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.Files, false, null, null, "Import Question Data", "Import");
        if (FileBrowser.Success)
            ImportCSV(File.ReadAllText(FileBrowser.Result.FirstOrDefault()));
        MainMenuManager.Get.fileBrowserBlocker.SetActive(false);
    }

    public void ImportCSV(string data)
    {
        try
        {
            //THIS REGION SHOULDN'T NEED TO BE TOUCHED
            //It handles the .csv conversion accounting for quotes and commas in data fields and spits out a string[] called 'splitData'
            #region Magical bullshit code that deals with csv nonsense

            string trimmedCSV = data.Replace("\n", ",").Replace("\r", "");
            var sheet = CSVParser.LoadFromString(trimmedCSV);

            var styled = new StringBuilder();
            foreach (var row in sheet)
            {
                styled.Append("¬");

                foreach (var cell in row)
                {
                    styled.Append(cell);
                    styled.Append("¬");
                }

                styled.AppendLine();
            }

            if (styled.Length > 0 && styled[0].ToString() == "¬")
                styled.Remove(0, 1);

            string[] splitData = styled.ToString().Split('¬');

            #endregion

            switch(splitData.FirstOrDefault())
            {
                case "SimpleQuestion":
                    for (int i = 16; i < splitData.Length; i += 8)
                    {
                        if (splitData.Length < (i + 8))
                            continue;
                        Question qn = new Question(splitData[i], splitData[i + 1], splitData[i + 2], splitData[i + 3]);
                        Answer a = new Answer(splitData[i + 4], splitData[i + 5].Split('|').ToList(), true, splitData[i + 6], splitData[i + 7]);
                        StorageManager.SaveQuestion(new SimpleQuestion(qn, a));
                    }
                    break;

                case "MultipleChoiceQuestion":
                    Question q = null;
                    List<Answer> ans = new List<Answer>();

                    for(int i = 18; i < splitData.Length; i += 9)
                    {
                        if (splitData.Length < (i + 9))
                            continue;

                        //Create a new question if questionText string is not empty
                        if (!string.IsNullOrEmpty(splitData[i]))
                        {
                            //If we don't have an empty question, we're about to start a new one, so save it and clear temp variables
                            if(q != null)
                            {
                                StorageManager.SaveQuestion(new MultipleChoice(q, ans));
                                q = null;
                                ans = new List<Answer>();
                            }
                            q = new Question(splitData[i], splitData[i + 1], splitData[i + 2], splitData[i + 3]);
                        }

                        ans.Add(new Answer(splitData[i + 4], splitData[i + 5].Split('|').ToList(), splitData[i + 6].ToUpperInvariant() == "TRUE" ? true : false, splitData[i + 7], splitData[i + 8]));                        
                    }

                    //Save final question of loop
                    if(q != null)
                        StorageManager.SaveQuestion(new MultipleChoice(q, ans));
                    break;

                case "SequentialQuestion":
                    q = null;
                    List<Clue> clues = new List<Clue>();
                    Answer an = new Answer();
                    //First valid field is 20

                    for(int i = 20; i < splitData.Length; i += 10)
                    {
                        if (splitData.Length < (i + 10))
                            continue;

                        //Create a new question if questionText string is not empty
                        if (!string.IsNullOrEmpty(splitData[i]))
                        {
                            //If we don't have an empty question, we're about to start a new one, so save it and clear temp variables
                            if (q != null)
                            {
                                StorageManager.SaveQuestion(new Sequential(q, clues, an));
                                q = null;
                                clues = new List<Clue>();
                                an = new Answer();
                            }

                            q = new Question(splitData[i], splitData[i + 1], splitData[i + 2], splitData[i + 3]);
                            an = new Answer(splitData[i + 6], splitData[i + 7].Split('|').ToList(), true, splitData[i + 8], splitData[i + 9]);
                        }
                        clues.Add(new Clue(splitData[i + 4], splitData[i + 5]));
                    }

                    //Save final question of loop
                    if (q != null)
                        StorageManager.SaveQuestion(new Sequential(q, clues, an));
                    break;
            }
        }
        catch (Exception ex)
        {
            DebugLog.Print($"Error on question import: {ex.Message}", DebugLog.StyleOption.Bold, DebugLog.ColorOption.Red);
            //Sys.Get.Alert.FireAlert($"<color=red>Error on question import: {ex.Message}");
        }
    }
}
