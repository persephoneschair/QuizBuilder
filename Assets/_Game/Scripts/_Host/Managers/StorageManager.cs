using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;
using System.Linq;

public static class StorageManager
{
    private static string dataPath;

    public static List<SimpleQuestion> SimpleQuestions = new List<SimpleQuestion>();
    public static List<MultipleChoice> MultiChoiceQuestions = new List<MultipleChoice>();
    public static List<Sequential> SequentialQuestions = new List<Sequential>();

    public static List<QuizTemplate> QuizTemplates = new List<QuizTemplate>();
    public static Dictionary<Guid, Quiz> Quizzes = new Dictionary<Guid, Quiz>();

    #region Storage Init

    public static void SetUpStorage()
    {
        dataPath = Application.persistentDataPath;

        //Questions
        if(!File.Exists(dataPath + @"/SimpleQuestions.json"))
        {
            SaveData(dataPath + @"/SimpleQuestions.json", JsonConvert.SerializeObject(SimpleQuestions, Formatting.Indented));
            DebugLog.Print("Simple question structure created", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
        }
        else
        {
            SimpleQuestions = JsonConvert.DeserializeObject<List<SimpleQuestion>>(File.ReadAllText(dataPath + @"/SimpleQuestions.json"));
            DebugLog.Print("Simple questions loaded", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Yellow);
        }

        if (!File.Exists(dataPath + @"/MultiChoiceQuestions.json"))
        {
            SaveData(dataPath + @"/MultiChoiceQuestions.json", JsonConvert.SerializeObject(MultiChoiceQuestions, Formatting.Indented));
            DebugLog.Print("Multi-choice question structure created", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
        }
        else
        {
            MultiChoiceQuestions = JsonConvert.DeserializeObject<List<MultipleChoice>>(File.ReadAllText(dataPath + @"/MultiChoiceQuestions.json"));
            DebugLog.Print("Multi-choice questions loaded", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Yellow);
        }

        if (!File.Exists(dataPath + @"/SequentialQuestions.json"))
        {
            SaveData(dataPath + @"/SequentialQuestions.json", JsonConvert.SerializeObject(SequentialQuestions, Formatting.Indented));
            DebugLog.Print("Sequential question structure created", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
        }
        else
        {
            SequentialQuestions = JsonConvert.DeserializeObject<List<Sequential>>(File.ReadAllText(dataPath + @"/SequentialQuestions.json"));
            DebugLog.Print("Sequential questions loaded", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Yellow);
        }

        //Templates
        if (!File.Exists(dataPath + @"/QuizTemplates.json"))
        {
            SaveData(dataPath + @"/QuizTemplates.json", JsonConvert.SerializeObject(QuizTemplates, Formatting.Indented));
            DebugLog.Print("Quiz templates structure created", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
        }
        else
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            QuizTemplates = JsonConvert.DeserializeObject<List<QuizTemplate>>(File.ReadAllText(dataPath + @"/QuizTemplates.json"), settings: settings);
            DebugLog.Print("Quiz templates loaded", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Yellow);
        }

        //Quizzes
        if (!File.Exists(dataPath + @"/Quizzes.json"))
        {
            SaveData(dataPath + @"/Quizzes.json", JsonConvert.SerializeObject(Quizzes, Formatting.Indented));
            DebugLog.Print("Quizzes structure created", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
        }
        else
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            Quizzes = JsonConvert.DeserializeObject<Dictionary<Guid, Quiz>>(File.ReadAllText(dataPath + @"/Quizzes.json"), settings: settings);
            DebugLog.Print("Quizzes loaded", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Yellow);
        }

        //Settings
        if(!File.Exists(dataPath + @"/Settings.json"))
        {
            //Create defaults if file does not exist
            SaveData(dataPath + @"/Settings.json", JsonConvert.SerializeObject(new Settings(), Formatting.Indented));
            DebugLog.Print("Default settings structure created", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
        }
        //Otherwise, load in what's there
        SettingsManager.Get.CurrentSettings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(dataPath + @"/Settings.json"));
        DebugLog.Print("Settings loaded", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Yellow);
    }

    #endregion

    #region Get Questions

    public static SimpleQuestion GetSimpleQuestion(Guid? id)
    {
        if (SimpleQuestions.FirstOrDefault(x => x.questionID == id) != null)
            return SimpleQuestions.FirstOrDefault(x => x.questionID == id);
        else return null;
    }

    public static MultipleChoice GetMCQuestion(Guid? id)
    {
        if (MultiChoiceQuestions.FirstOrDefault(x => x.questionID == id) != null)
            return MultiChoiceQuestions.FirstOrDefault(x => x.questionID == id);
        else return null;
    }

    public static Sequential GetSeqQuestion(Guid? id)
    {
        if (SequentialQuestions.FirstOrDefault(x => x.questionID == id) != null)
            return SequentialQuestions.FirstOrDefault(x => x.questionID == id);
        else return null;
    }

    #endregion

    #region Read/Write Question Data

    public static void SaveQuestion(SimpleQuestion q)
    {
        SimpleQuestions.Add(q);
        SaveData(dataPath + @"/SimpleQuestions.json", JsonConvert.SerializeObject(SimpleQuestions, Formatting.Indented));
        DebugLog.Print("Simple question saved...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
    }

    public static void SaveQuestion(MultipleChoice q)
    {
        MultiChoiceQuestions.Add(q);
        SaveData(dataPath + @"/MultiChoiceQuestions.json", JsonConvert.SerializeObject(MultiChoiceQuestions, Formatting.Indented));
        DebugLog.Print("Multi-choice question saved...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
    }

    public static void SaveQuestion(Sequential q)
    {
        SequentialQuestions.Add(q);
        SaveData(dataPath + @"/SequentialQuestions.json", JsonConvert.SerializeObject(SequentialQuestions, Formatting.Indented));
        DebugLog.Print("Sequential question saved...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
    }

    public static void UpdateQuestion(Guid? qID, Question newQ, Answer newA)
    {
        SimpleQuestion q = SimpleQuestions.FirstOrDefault(x => x.questionID == qID);
        if(q != null)
        {
            q.question = newQ;
            q.answer = newA;
            SaveData(dataPath + @"/SimpleQuestions.json", JsonConvert.SerializeObject(SimpleQuestions, Formatting.Indented));
            DebugLog.Print("Simple question updated...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Orange);
        }
    }

    public static void UpdateQuestion(Guid? qID, Question newQ, List<Answer> newAs)
    {
        MultipleChoice q = MultiChoiceQuestions.FirstOrDefault(x => x.questionID == qID);
        if (q != null)
        {
            q.question = newQ;
            q.answers = newAs;
            SaveData(dataPath + @"/MultiChoiceQuestions.json", JsonConvert.SerializeObject(MultiChoiceQuestions, Formatting.Indented));
            DebugLog.Print("Multi-choice question updated...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Orange);
        }
    }

    public static void UpdateQuestion(Guid? qID, Question newQ, List<Clue> newCs, Answer newA)
    {
        Sequential q = SequentialQuestions.FirstOrDefault(x => x.questionID == qID);
        if (q != null)
        {
            q.question = newQ;
            q.clues = newCs;
            q.answer = newA;
            SaveData(dataPath + @"/SequentialQuestions.json", JsonConvert.SerializeObject(SequentialQuestions, Formatting.Indented));
            DebugLog.Print("Sequential question updated...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Orange);
        }
    }

    public static void ReSaveAllQuestions()
    {
        SaveData(dataPath + @"/SimpleQuestions.json", JsonConvert.SerializeObject(SimpleQuestions, Formatting.Indented));
        DebugLog.Print("Simple question updated...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Orange);
        SaveData(dataPath + @"/MultiChoiceQuestions.json", JsonConvert.SerializeObject(MultiChoiceQuestions, Formatting.Indented));
        DebugLog.Print("Multi-choice question updated...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Orange);
        SaveData(dataPath + @"/SequentialQuestions.json", JsonConvert.SerializeObject(SequentialQuestions, Formatting.Indented));
        DebugLog.Print("Sequential question updated...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Orange);
    }

    public static void DeleteQuestion(Guid qID, BaseQuestion.QuestionType type)
    {
        RemoveDeletedQuestionFromQuizzes(qID);
        switch (type)
        {
            case BaseQuestion.QuestionType.Simple:
                SimpleQuestion s = SimpleQuestions.FirstOrDefault(x => x.questionID == qID);
                if (s == null)
                    return;
                SimpleQuestions.Remove(s);
                SaveData(dataPath + @"/SimpleQuestions.json", JsonConvert.SerializeObject(SimpleQuestions, Formatting.Indented));
                DebugLog.Print("Simple question deleted...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Red);
                break;

            case BaseQuestion.QuestionType.MultiChoice:
                MultipleChoice mc = MultiChoiceQuestions.FirstOrDefault(x => x.questionID == qID);
                if (mc == null)
                    return;
                MultiChoiceQuestions.Remove(mc);
                SaveData(dataPath + @"/MultiChoiceQuestions.json", JsonConvert.SerializeObject(MultiChoiceQuestions, Formatting.Indented));
                DebugLog.Print("Multi-choice question deleted...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Red);
                break;

            case BaseQuestion.QuestionType.Sequential:
                Sequential sq = SequentialQuestions.FirstOrDefault(x => x.questionID == qID);
                if (sq == null)
                    return;
                SequentialQuestions.Remove(sq);
                SaveData(dataPath + @"/SequentialQuestions.json", JsonConvert.SerializeObject(SequentialQuestions, Formatting.Indented));
                DebugLog.Print("Sequential question deleted...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Red);
                break;
        }
    }

    public static void RemoveDeletedQuestionFromQuizzes(Guid questionID)
    {
        foreach (Quiz q in Quizzes.Values)
        {
            foreach (RoundType r in q.quizTemplate.rounds)
                if (r.questionStack.questionIDs != null)
                    r.questionStack.questionIDs.RemoveAll(g => g == questionID);

            UpdateQuiz(q, q.quizName);
        }
    }

    #endregion

    #region Read/Write Template Data

    public static void SaveTemplate(QuizTemplate qt)
    {
        QuizTemplates.Add(qt);

        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        SaveData(dataPath + @"/QuizTemplates.json", JsonConvert.SerializeObject(QuizTemplates, Formatting.Indented, settings: settings));
        DebugLog.Print("Quiz template saved...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
    }

    public static void UpdateTemplate(Guid? templateId, string newTemplateName, List<AddedRound> newTemplateRounds)
    {
        QuizTemplate qt = QuizTemplates.FirstOrDefault(x => x.templateID == templateId);
        if(qt != null)
        {
            qt.templateName = newTemplateName;
            qt.rounds.Clear();
            foreach (AddedRound ar in newTemplateRounds)
                qt.rounds.Add(new RoundType(ar.round, ar.config, ar.roundID));

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            SaveData(dataPath + @"/QuizTemplates.json", JsonConvert.SerializeObject(QuizTemplates, Formatting.Indented, settings: settings));
            DebugLog.Print("Quiz template updated...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Orange);
        }

        //When we update the template, we need to pass all of that new template data into any quizzes that are already built with it
        //However, we must RETAIN the question Guids stored inside those existing quizzes and delete any where the rounds have been removed
        List<Quiz> quizzesBuiltOnModifiedTemplate = Quizzes.Values.Where(x => x.quizTemplate.templateID == templateId).ToList();
        foreach(Quiz q in quizzesBuiltOnModifiedTemplate)
        {
            //Remove quiz references from all questions in this quiz
            var oldStack = GetAllQGuidsForQuiz(q);
            foreach (Guid g in oldStack)
            {
                if (GetSimpleQuestion(g) != null)
                    GetSimpleQuestion(g).quizIDs.Remove(q.quizID);
                else if (GetMCQuestion(g) != null)
                    GetMCQuestion(g).quizIDs.Remove(q.quizID);
                else if (GetSeqQuestion(g) != null)
                    GetSeqQuestion(g).quizIDs.Remove(q.quizID);
            }

            //Make a temporary new quiz with the same constructor details EXCEPT the template
            Quiz newQuiz = new Quiz(qt.DeepCopy(), q.quizCreationDate, q.quizName, q.quizID);
            foreach(RoundType r in newQuiz.quizTemplate.rounds)
            {
                if (q.quizTemplate.rounds.Any(x => x.roundID == r.roundID))
                {
                    //Bring over all the old questions and re-add references
                    foreach (Guid g in q.quizTemplate.rounds.FirstOrDefault(x => x.roundID == r.roundID).questionStack.questionIDs)
                    {
                        r.questionStack.questionIDs.Add(g);

                        if (GetSimpleQuestion(g) != null)
                            GetSimpleQuestion(g).quizIDs.Add(newQuiz.quizID);
                        else if (GetMCQuestion(g) != null)
                            GetMCQuestion(g).quizIDs.Add(newQuiz.quizID);
                        else if (GetSeqQuestion(g) != null)
                            GetSeqQuestion(g).quizIDs.Add(newQuiz.quizID);
                    }
                }
            }

            //Apply the newly created template to the old quiz and save it
            q.quizTemplate = newQuiz.quizTemplate;
            UpdateQuiz(q, q.quizName);
            ReSaveAllQuestions();
        }
    }

    public static void DeleteTemplate(QuizTemplate qt)
    {
        QuizTemplates.Remove(qt);

        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        SaveData(dataPath + @"/QuizTemplates.json", JsonConvert.SerializeObject(QuizTemplates, Formatting.Indented, settings:settings));
        DebugLog.Print("Quiz template deleted...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Red);
    }

    #endregion

    #region Read/Write Quiz Data

    public static void SaveQuiz(Quiz qz)
    {
        Quizzes.Add(qz.quizID, qz);

        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        SaveData(dataPath + @"/Quizzes.json", JsonConvert.SerializeObject(Quizzes, Formatting.Indented, settings: settings));
        DebugLog.Print("Quiz saved...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
    }

    public static void UpdateQuiz(Quiz qz, string newQuizName)
    {
        if (qz != null)
        {
            qz.quizName = newQuizName;
            if(!ReferenceEquals(qz, Quizzes[qz.quizID]))
                Quizzes[qz.quizID] = qz;
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All
            };
            SaveData(dataPath + @"/Quizzes.json", JsonConvert.SerializeObject(Quizzes, Formatting.Indented, settings: settings));
            DebugLog.Print("Quiz updated...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Orange);
        }
    }

    public static void DeleteQuiz(Quiz qz)
    {
        Quizzes.Remove(qz.quizID);

        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        SaveData(dataPath + @"/Quizzes.json", JsonConvert.SerializeObject(Quizzes, Formatting.Indented, settings: settings));
        DebugLog.Print("Quiz deleted...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Red);
    }

    #endregion

    public static void SaveSettings(Settings sts)
    {
        SaveData(dataPath + @"/Settings.json", JsonConvert.SerializeObject(sts, Formatting.Indented));
        DebugLog.Print("Settings saved...", DebugLog.StyleOption.Italic, DebugLog.ColorOption.Green);
    }

    private static void SaveData(string path, string data)
    {
        File.WriteAllText(path, data);
    }

    public static string GetDataPath()
    {
        return dataPath;
    }

    public static List<Guid> GetAllQGuidsForQuiz(Quiz quiz)
    {
        return quiz.quizTemplate.rounds
                .Where(r => r.questionStack != null && r.questionStack.questionIDs != null)
                .SelectMany(r => r.questionStack.questionIDs)
                .ToList();
    }
}
