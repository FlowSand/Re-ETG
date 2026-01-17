// Decompiled with JetBrains decompiler
// Type: StringTableManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using InControl;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Management
{
    public static class StringTableManager
    {
      private static string m_currentFile = "english";
      private static string m_currentSubDirectory = "english_items";
      private static Stack<List<string>> m_tokenLists;
      private static Stack<StringBuilder> m_postprocessors;
      private static Dictionary<string, StringTableManager.StringCollection> m_coreTable;
      private static Dictionary<string, StringTableManager.StringCollection> m_itemsTable;
      private static Dictionary<string, StringTableManager.StringCollection> m_enemiesTable;
      private static Dictionary<string, StringTableManager.StringCollection> m_uiTable;
      private static Dictionary<string, StringTableManager.StringCollection> m_introTable;
      private static Dictionary<string, StringTableManager.StringCollection> m_synergyTable;
      private static Dictionary<string, StringTableManager.StringCollection> m_backupCoreTable;
      private static Dictionary<string, StringTableManager.StringCollection> m_backupItemsTable;
      private static Dictionary<string, StringTableManager.StringCollection> m_backupEnemiesTable;
      private static Dictionary<string, StringTableManager.StringCollection> m_backupUiTable;
      private static Dictionary<string, StringTableManager.StringCollection> m_backupIntroTable;
      private static Dictionary<string, StringTableManager.StringCollection> m_backupSynergyTable;

      public static void ReloadAllTables()
      {
        StringTableManager.m_coreTable = (Dictionary<string, StringTableManager.StringCollection>) null;
        StringTableManager.m_enemiesTable = (Dictionary<string, StringTableManager.StringCollection>) null;
        StringTableManager.m_itemsTable = (Dictionary<string, StringTableManager.StringCollection>) null;
        StringTableManager.m_uiTable = (Dictionary<string, StringTableManager.StringCollection>) null;
        StringTableManager.m_introTable = (Dictionary<string, StringTableManager.StringCollection>) null;
        StringTableManager.m_synergyTable = (Dictionary<string, StringTableManager.StringCollection>) null;
        StringTableManager.m_backupCoreTable = (Dictionary<string, StringTableManager.StringCollection>) null;
        StringTableManager.m_backupEnemiesTable = (Dictionary<string, StringTableManager.StringCollection>) null;
        StringTableManager.m_backupIntroTable = (Dictionary<string, StringTableManager.StringCollection>) null;
        StringTableManager.m_backupItemsTable = (Dictionary<string, StringTableManager.StringCollection>) null;
        StringTableManager.m_backupSynergyTable = (Dictionary<string, StringTableManager.StringCollection>) null;
        StringTableManager.m_backupUiTable = (Dictionary<string, StringTableManager.StringCollection>) null;
      }

      public static string GetString(string key)
      {
        if (StringTableManager.m_coreTable == null)
          StringTableManager.m_coreTable = StringTableManager.LoadTables(StringTableManager.m_currentFile);
        if (StringTableManager.m_backupCoreTable == null)
          StringTableManager.m_backupCoreTable = StringTableManager.LoadTables("english");
        if (StringTableManager.m_coreTable.ContainsKey(key))
          return StringTableManager.PostprocessString(StringTableManager.m_coreTable[key].GetWeightedString());
        return StringTableManager.m_backupCoreTable.ContainsKey(key) ? StringTableManager.PostprocessString(StringTableManager.m_backupCoreTable[key].GetWeightedString()) : "STRING_NOT_FOUND";
      }

      public static string GetExactString(string key, int index)
      {
        if (StringTableManager.m_coreTable == null)
          StringTableManager.m_coreTable = StringTableManager.LoadTables(StringTableManager.m_currentFile);
        if (StringTableManager.m_backupCoreTable == null)
          StringTableManager.m_backupCoreTable = StringTableManager.LoadTables("english");
        if (StringTableManager.m_coreTable.ContainsKey(key))
          return StringTableManager.PostprocessString(StringTableManager.m_coreTable[key].GetExactString(index));
        return StringTableManager.m_backupCoreTable.ContainsKey(key) ? StringTableManager.PostprocessString(StringTableManager.m_backupCoreTable[key].GetExactString(index)) : "STRING_NOT_FOUND";
      }

      public static string GetEnemiesLongDescription(string key)
      {
        if (StringTableManager.m_enemiesTable == null)
          StringTableManager.m_enemiesTable = StringTableManager.LoadEnemiesTable(StringTableManager.m_currentSubDirectory);
        if (StringTableManager.m_backupEnemiesTable == null)
          StringTableManager.m_backupEnemiesTable = StringTableManager.LoadEnemiesTable("english_items");
        if (StringTableManager.m_enemiesTable.ContainsKey(key))
          return StringTableManager.PostprocessString(StringTableManager.m_enemiesTable[key].GetCombinedString());
        return StringTableManager.m_backupEnemiesTable.ContainsKey(key) ? StringTableManager.PostprocessString(StringTableManager.m_backupEnemiesTable[key].GetCombinedString()) : "ENEMIES_STRING_NOT_FOUND";
      }

      public static string GetItemsLongDescription(string key)
      {
        if (StringTableManager.m_itemsTable == null)
          StringTableManager.m_itemsTable = StringTableManager.LoadItemsTable(StringTableManager.m_currentSubDirectory);
        if (StringTableManager.m_backupItemsTable == null)
          StringTableManager.m_backupItemsTable = StringTableManager.LoadItemsTable("english_items");
        if (StringTableManager.m_itemsTable.ContainsKey(key))
          return StringTableManager.PostprocessString(StringTableManager.m_itemsTable[key].GetCombinedString());
        return StringTableManager.m_backupItemsTable.ContainsKey(key) ? StringTableManager.PostprocessString(StringTableManager.m_backupItemsTable[key].GetCombinedString()) : "ITEMS_STRING_NOT_FOUND";
      }

      public static string GetEnemiesString(string key, int index = -1)
      {
        if (StringTableManager.m_enemiesTable == null)
          StringTableManager.m_enemiesTable = StringTableManager.LoadEnemiesTable(StringTableManager.m_currentSubDirectory);
        if (StringTableManager.m_backupEnemiesTable == null)
          StringTableManager.m_backupEnemiesTable = StringTableManager.LoadEnemiesTable("english_items");
        if (StringTableManager.m_enemiesTable.ContainsKey(key))
          return index == -1 ? StringTableManager.PostprocessString(StringTableManager.m_enemiesTable[key].GetWeightedString()) : StringTableManager.PostprocessString(StringTableManager.m_enemiesTable[key].GetExactString(index));
        if (!StringTableManager.m_backupEnemiesTable.ContainsKey(key))
          return "ENEMIES_STRING_NOT_FOUND";
        return index == -1 ? StringTableManager.PostprocessString(StringTableManager.m_backupEnemiesTable[key].GetWeightedString()) : StringTableManager.PostprocessString(StringTableManager.m_backupEnemiesTable[key].GetExactString(index));
      }

      public static string GetIntroString(string key)
      {
        if (StringTableManager.m_introTable == null)
          StringTableManager.m_introTable = StringTableManager.LoadIntroTable(StringTableManager.m_currentSubDirectory);
        if (StringTableManager.m_backupIntroTable == null)
          StringTableManager.m_backupIntroTable = StringTableManager.LoadIntroTable("english_items");
        if (StringTableManager.m_introTable.ContainsKey(key))
          return StringTableManager.PostprocessString(StringTableManager.m_introTable[key].GetWeightedString());
        return StringTableManager.m_backupIntroTable.ContainsKey(key) ? StringTableManager.PostprocessString(StringTableManager.m_backupIntroTable[key].GetWeightedString()) : "INTRO_STRING_NOT_FOUND";
      }

      public static string GetItemsString(string key, int index = -1)
      {
        if (StringTableManager.m_itemsTable == null)
          StringTableManager.m_itemsTable = StringTableManager.LoadItemsTable(StringTableManager.m_currentSubDirectory);
        if (StringTableManager.m_backupItemsTable == null)
          StringTableManager.m_backupItemsTable = StringTableManager.LoadItemsTable("english_items");
        if (StringTableManager.m_itemsTable.ContainsKey(key))
          return index == -1 ? StringTableManager.PostprocessString(StringTableManager.m_itemsTable[key].GetWeightedString()) : StringTableManager.PostprocessString(StringTableManager.m_itemsTable[key].GetExactString(index));
        if (!StringTableManager.m_backupItemsTable.ContainsKey(key))
          return "ITEMS_STRING_NOT_FOUND";
        return index == -1 ? StringTableManager.PostprocessString(StringTableManager.m_backupItemsTable[key].GetWeightedString()) : StringTableManager.PostprocessString(StringTableManager.m_backupItemsTable[key].GetExactString(index));
      }

      public static string GetUIString(string key, int index = -1)
      {
        if (StringTableManager.m_uiTable == null)
          StringTableManager.m_uiTable = StringTableManager.LoadUITable(StringTableManager.m_currentSubDirectory);
        if (StringTableManager.m_backupUiTable == null)
          StringTableManager.m_backupUiTable = StringTableManager.LoadUITable(StringTableManager.m_currentSubDirectory);
        if (StringTableManager.m_uiTable.ContainsKey(key))
          return index == -1 ? StringTableManager.PostprocessString(StringTableManager.m_uiTable[key].GetWeightedString()) : StringTableManager.PostprocessString(StringTableManager.m_uiTable[key].GetExactString(index));
        if (!StringTableManager.m_backupUiTable.ContainsKey(key))
          return "ITEMS_STRING_NOT_FOUND";
        return index == -1 ? StringTableManager.PostprocessString(StringTableManager.m_backupUiTable[key].GetWeightedString()) : StringTableManager.PostprocessString(StringTableManager.m_backupUiTable[key].GetExactString(index));
      }

      public static string GetSynergyString(string key, int index = -1)
      {
        if (StringTableManager.m_synergyTable == null)
          StringTableManager.m_synergyTable = StringTableManager.LoadSynergyTable(StringTableManager.m_currentSubDirectory);
        if (StringTableManager.m_backupSynergyTable == null)
          StringTableManager.m_backupSynergyTable = StringTableManager.LoadSynergyTable("english_items");
        if (!StringTableManager.m_synergyTable.ContainsKey(key))
          return string.Empty;
        return index == -1 ? StringTableManager.PostprocessString(StringTableManager.m_synergyTable[key].GetWeightedString()) : StringTableManager.PostprocessString(StringTableManager.m_synergyTable[key].GetExactString(index));
      }

      public static string GetStringSequential(string key, ref int lastIndex, bool repeatLast = false)
      {
        return StringTableManager.GetStringSequential(key, ref lastIndex, out bool _, repeatLast);
      }

      public static string GetStringSequential(
        string key,
        ref int lastIndex,
        out bool isLast,
        bool repeatLast = false)
      {
        isLast = false;
        if (StringTableManager.m_coreTable == null)
          StringTableManager.m_coreTable = StringTableManager.LoadTables(StringTableManager.m_currentFile);
        if (StringTableManager.m_backupCoreTable == null)
          StringTableManager.m_backupCoreTable = StringTableManager.LoadTables("english");
        if (StringTableManager.m_coreTable.ContainsKey(key))
          return StringTableManager.PostprocessString(StringTableManager.m_coreTable[key].GetWeightedStringSequential(ref lastIndex, out isLast, repeatLast));
        return StringTableManager.m_backupCoreTable.ContainsKey(key) ? StringTableManager.PostprocessString(StringTableManager.m_backupCoreTable[key].GetWeightedStringSequential(ref lastIndex, out isLast, repeatLast)) : "STRING_NOT_FOUND";
      }

      public static string GetStringPersistentSequential(string key)
      {
        if (StringTableManager.m_coreTable == null)
          StringTableManager.m_coreTable = StringTableManager.LoadTables(StringTableManager.m_currentFile);
        if (StringTableManager.m_backupCoreTable == null)
          StringTableManager.m_backupCoreTable = StringTableManager.LoadTables("english");
        if (StringTableManager.m_coreTable.ContainsKey(key))
        {
          int persistentStringLastIndex = GameStatsManager.Instance.GetPersistentStringLastIndex(key);
          string stringSequential = StringTableManager.m_coreTable[key].GetWeightedStringSequential(ref persistentStringLastIndex, out bool _);
          GameStatsManager.Instance.SetPersistentStringLastIndex(key, persistentStringLastIndex);
          return StringTableManager.PostprocessString(stringSequential);
        }
        if (!StringTableManager.m_backupCoreTable.ContainsKey(key))
          return "STRING_NOT_FOUND";
        int persistentStringLastIndex1 = GameStatsManager.Instance.GetPersistentStringLastIndex(key);
        string stringSequential1 = StringTableManager.m_backupCoreTable[key].GetWeightedStringSequential(ref persistentStringLastIndex1, out bool _);
        GameStatsManager.Instance.SetPersistentStringLastIndex(key, persistentStringLastIndex1);
        return StringTableManager.PostprocessString(stringSequential1);
      }

      public static int GetNumStrings(string key)
      {
        if (StringTableManager.m_coreTable == null)
          StringTableManager.m_coreTable = StringTableManager.LoadTables(StringTableManager.m_currentFile);
        if (StringTableManager.m_backupCoreTable == null)
          StringTableManager.m_backupCoreTable = StringTableManager.LoadTables("english");
        if (StringTableManager.m_coreTable.ContainsKey(key))
          return StringTableManager.m_coreTable[key].Count();
        return StringTableManager.m_backupCoreTable.ContainsKey(key) ? StringTableManager.m_backupCoreTable[key].Count() : 0;
      }

      public static string GetLongString(string key)
      {
        if (StringTableManager.m_coreTable == null)
          StringTableManager.m_coreTable = StringTableManager.LoadTables(StringTableManager.m_currentFile);
        if (StringTableManager.m_backupCoreTable == null)
          StringTableManager.m_backupCoreTable = StringTableManager.LoadTables("english");
        if (StringTableManager.m_coreTable.ContainsKey(key))
          return StringTableManager.PostprocessString(StringTableManager.m_coreTable[key].GetCombinedString());
        return StringTableManager.m_backupCoreTable.ContainsKey(key) ? StringTableManager.PostprocessString(StringTableManager.m_backupCoreTable[key].GetCombinedString()) : "STRING_NOT_FOUND";
      }

      public static void LoadTablesIfNecessary()
      {
        if (StringTableManager.m_coreTable == null)
          StringTableManager.m_coreTable = StringTableManager.LoadTables(StringTableManager.m_currentFile);
        if (StringTableManager.m_backupCoreTable != null)
          return;
        StringTableManager.m_backupCoreTable = StringTableManager.LoadTables("english");
      }

      public static Dictionary<string, StringTableManager.StringCollection> LoadTables(
        string currentFile)
      {
        TextAsset textAsset = (TextAsset) BraveResources.Load("strings/" + currentFile, typeof (TextAsset), ".txt");
        if ((UnityEngine.Object) textAsset == (UnityEngine.Object) null)
        {
          Debug.LogError((object) "Failed to load string table.");
          return (Dictionary<string, StringTableManager.StringCollection>) null;
        }
        StringReader stringReader = new StringReader(textAsset.text);
        Dictionary<string, StringTableManager.StringCollection> dictionary = new Dictionary<string, StringTableManager.StringCollection>();
        StringTableManager.StringCollection stringCollection = (StringTableManager.StringCollection) null;
        string key;
        while ((key = stringReader.ReadLine()) != null)
        {
          if (!key.StartsWith("//"))
          {
            if (key.StartsWith("#"))
            {
              stringCollection = (StringTableManager.StringCollection) new StringTableManager.ComplexStringCollection();
              if (dictionary.ContainsKey(key))
                Debug.LogError((object) $"Attempting to add the key {key} twice to the string table!");
              else
                dictionary.Add(key, stringCollection);
            }
            else
            {
              string[] strArray = key.Split('|');
              if (strArray.Length == 1)
              {
                stringCollection.AddString(strArray[0], 1f);
              }
              else
              {
                CultureInfo invariantCulture = CultureInfo.InvariantCulture;
                stringCollection.AddString(strArray[1], float.Parse(strArray[0], NumberStyles.Any, (IFormatProvider) invariantCulture.NumberFormat));
              }
            }
          }
        }
        return dictionary;
      }

      public static StringTableManager.GungeonSupportedLanguages CurrentLanguage
      {
        get => GameManager.Options.CurrentLanguage;
        set => StringTableManager.SetNewLanguage(value, true);
      }

      public static void SetNewLanguage(
        StringTableManager.GungeonSupportedLanguages language,
        bool force = false)
      {
        if (!force && StringTableManager.CurrentLanguage == language)
          return;
        switch (language)
        {
          case StringTableManager.GungeonSupportedLanguages.ENGLISH:
            StringTableManager.m_currentFile = "english";
            StringTableManager.m_currentSubDirectory = "english_items";
            break;
          case StringTableManager.GungeonSupportedLanguages.FRENCH:
            StringTableManager.m_currentFile = "french";
            StringTableManager.m_currentSubDirectory = "french_items";
            break;
          case StringTableManager.GungeonSupportedLanguages.SPANISH:
            StringTableManager.m_currentFile = "spanish";
            StringTableManager.m_currentSubDirectory = "spanish_items";
            break;
          case StringTableManager.GungeonSupportedLanguages.ITALIAN:
            StringTableManager.m_currentFile = "italian";
            StringTableManager.m_currentSubDirectory = "italian_items";
            break;
          case StringTableManager.GungeonSupportedLanguages.GERMAN:
            StringTableManager.m_currentFile = "german";
            StringTableManager.m_currentSubDirectory = "german_items";
            break;
          case StringTableManager.GungeonSupportedLanguages.BRAZILIANPORTUGUESE:
            StringTableManager.m_currentFile = "portuguese";
            StringTableManager.m_currentSubDirectory = "portuguese_items";
            break;
          case StringTableManager.GungeonSupportedLanguages.JAPANESE:
            StringTableManager.m_currentFile = "japanese";
            StringTableManager.m_currentSubDirectory = "japanese_items";
            break;
          case StringTableManager.GungeonSupportedLanguages.KOREAN:
            StringTableManager.m_currentFile = "korean";
            StringTableManager.m_currentSubDirectory = "korean_items";
            break;
          case StringTableManager.GungeonSupportedLanguages.RUSSIAN:
            StringTableManager.m_currentFile = "russian";
            StringTableManager.m_currentSubDirectory = "russian_items";
            break;
          case StringTableManager.GungeonSupportedLanguages.POLISH:
            StringTableManager.m_currentFile = "polish";
            StringTableManager.m_currentSubDirectory = "polish_items";
            break;
          case StringTableManager.GungeonSupportedLanguages.CHINESE:
            StringTableManager.m_currentFile = "chinese";
            StringTableManager.m_currentSubDirectory = "chinese_items";
            break;
          default:
            StringTableManager.m_currentFile = "english";
            StringTableManager.m_currentSubDirectory = "english_items";
            break;
        }
        StringTableManager.ReloadAllTables();
        dfLanguageManager.ChangeGungeonLanguage();
        ++JournalEntry.ReloadDataSemaphore;
      }

      private static Dictionary<string, StringTableManager.StringCollection> LoadEnemiesTable(
        string subDirectory)
      {
        TextAsset textAsset = (TextAsset) BraveResources.Load($"strings/{subDirectory}/enemies", typeof (TextAsset), ".txt");
        if ((UnityEngine.Object) textAsset == (UnityEngine.Object) null)
        {
          Debug.LogError((object) "Failed to load string table: ENEMIES.");
          return (Dictionary<string, StringTableManager.StringCollection>) null;
        }
        StringReader stringReader = new StringReader(textAsset.text);
        Dictionary<string, StringTableManager.StringCollection> dictionary = new Dictionary<string, StringTableManager.StringCollection>();
        StringTableManager.StringCollection stringCollection = (StringTableManager.StringCollection) null;
        string key;
        while ((key = stringReader.ReadLine()) != null)
        {
          if (!key.StartsWith("//"))
          {
            if (key.StartsWith("#"))
            {
              stringCollection = (StringTableManager.StringCollection) new StringTableManager.ComplexStringCollection();
              if (dictionary.ContainsKey(key))
                Debug.LogError((object) ("Failed to add duplicate key to items table: " + key));
              else
                dictionary.Add(key, stringCollection);
            }
            else
            {
              string[] strArray = key.Split('|');
              if (strArray.Length == 1)
                stringCollection.AddString(strArray[0], 1f);
              else
                stringCollection.AddString(strArray[1], float.Parse(strArray[0]));
            }
          }
        }
        return dictionary;
      }

      public static TextAsset GetUIDataFile()
      {
        return (TextAsset) BraveResources.Load($"strings/{StringTableManager.m_currentSubDirectory}/ui", typeof (TextAsset), ".txt");
      }

      public static TextAsset GetBackupUIDataFile()
      {
        return (TextAsset) BraveResources.Load("strings/english_items/ui", typeof (TextAsset), ".txt");
      }

      private static Dictionary<string, StringTableManager.StringCollection> LoadSynergyTable(
        string subDirectory)
      {
        TextAsset textAsset = (TextAsset) BraveResources.Load($"strings/{subDirectory}/synergies", typeof (TextAsset), ".txt");
        Dictionary<string, StringTableManager.StringCollection> dictionary = new Dictionary<string, StringTableManager.StringCollection>();
        if ((UnityEngine.Object) textAsset == (UnityEngine.Object) null)
        {
          Debug.LogError((object) "Failed to load string table: ITEMS.");
          return dictionary;
        }
        StringReader stringReader = new StringReader(textAsset.text);
        StringTableManager.StringCollection stringCollection = (StringTableManager.StringCollection) null;
        string key;
        while ((key = stringReader.ReadLine()) != null)
        {
          if (!key.StartsWith("//"))
          {
            if (key.StartsWith("#"))
            {
              stringCollection = (StringTableManager.StringCollection) new StringTableManager.ComplexStringCollection();
              if (dictionary.ContainsKey(key))
                Debug.LogError((object) ("Failed to add duplicate key to synergies table: " + key));
              else
                dictionary.Add(key, stringCollection);
            }
            else
            {
              string[] strArray = key.Split('|');
              if (strArray.Length == 1)
                stringCollection.AddString(strArray[0], 1f);
              else
                stringCollection.AddString(strArray[1], float.Parse(strArray[0]));
            }
          }
        }
        return dictionary;
      }

      private static Dictionary<string, StringTableManager.StringCollection> LoadUITable(
        string subDirectory)
      {
        TextAsset textAsset = (TextAsset) BraveResources.Load($"strings/{subDirectory}/ui", typeof (TextAsset), ".txt");
        if ((UnityEngine.Object) textAsset == (UnityEngine.Object) null)
        {
          Debug.LogError((object) "Failed to load string table: ITEMS.");
          return (Dictionary<string, StringTableManager.StringCollection>) null;
        }
        StringReader stringReader = new StringReader(textAsset.text);
        Dictionary<string, StringTableManager.StringCollection> dictionary = new Dictionary<string, StringTableManager.StringCollection>();
        StringTableManager.StringCollection stringCollection = (StringTableManager.StringCollection) null;
        string key;
        while ((key = stringReader.ReadLine()) != null)
        {
          if (!key.StartsWith("//"))
          {
            if (key.StartsWith("#"))
            {
              stringCollection = (StringTableManager.StringCollection) new StringTableManager.ComplexStringCollection();
              if (dictionary.ContainsKey(key))
                Debug.LogError((object) ("Failed to add duplicate key to items table: " + key));
              else
                dictionary.Add(key, stringCollection);
            }
            else
            {
              string[] strArray = key.Split('|');
              if (strArray.Length == 1)
                stringCollection.AddString(strArray[0], 1f);
              else
                stringCollection.AddString(strArray[1], float.Parse(strArray[0]));
            }
          }
        }
        return dictionary;
      }

      private static Dictionary<string, StringTableManager.StringCollection> LoadIntroTable(
        string subDirectory)
      {
        TextAsset textAsset = (TextAsset) BraveResources.Load($"strings/{subDirectory}/intro", typeof (TextAsset), ".txt");
        if ((UnityEngine.Object) textAsset == (UnityEngine.Object) null)
        {
          Debug.LogError((object) "Failed to load string table: INTRO.");
          return (Dictionary<string, StringTableManager.StringCollection>) null;
        }
        StringReader stringReader = new StringReader(textAsset.text);
        Dictionary<string, StringTableManager.StringCollection> dictionary = new Dictionary<string, StringTableManager.StringCollection>();
        StringTableManager.StringCollection stringCollection = (StringTableManager.StringCollection) null;
        string key;
        while ((key = stringReader.ReadLine()) != null)
        {
          if (!key.StartsWith("//"))
          {
            if (key.StartsWith("#"))
            {
              stringCollection = (StringTableManager.StringCollection) new StringTableManager.ComplexStringCollection();
              if (dictionary.ContainsKey(key))
                Debug.LogError((object) ("Failed to add duplicate key to items table: " + key));
              else
                dictionary.Add(key, stringCollection);
            }
            else
            {
              string[] strArray = key.Split('|');
              if (strArray.Length == 1)
                stringCollection.AddString(strArray[0], 1f);
              else
                stringCollection.AddString(strArray[1], float.Parse(strArray[0]));
            }
          }
        }
        return dictionary;
      }

      private static Dictionary<string, StringTableManager.StringCollection> LoadItemsTable(
        string subDirectory)
      {
        TextAsset textAsset = (TextAsset) BraveResources.Load($"strings/{subDirectory}/items", typeof (TextAsset), ".txt");
        if ((UnityEngine.Object) textAsset == (UnityEngine.Object) null)
        {
          Debug.LogError((object) "Failed to load string table: ITEMS.");
          return (Dictionary<string, StringTableManager.StringCollection>) null;
        }
        StringReader stringReader = new StringReader(textAsset.text);
        Dictionary<string, StringTableManager.StringCollection> dictionary = new Dictionary<string, StringTableManager.StringCollection>();
        StringTableManager.StringCollection stringCollection = (StringTableManager.StringCollection) null;
        string key;
        while ((key = stringReader.ReadLine()) != null)
        {
          if (!key.StartsWith("//"))
          {
            if (key.StartsWith("#"))
            {
              stringCollection = (StringTableManager.StringCollection) new StringTableManager.ComplexStringCollection();
              if (dictionary.ContainsKey(key))
                Debug.LogError((object) ("Failed to add duplicate key to items table: " + key));
              else
                dictionary.Add(key, stringCollection);
            }
            else
            {
              string[] strArray = key.Split('|');
              if (strArray.Length == 1)
                stringCollection.AddString(strArray[0], 1f);
              else
                stringCollection.AddString(strArray[1], float.Parse(strArray[0]));
            }
          }
        }
        return dictionary;
      }

      public static string GetBindingText(GungeonActions.GungeonActionType ActionType)
      {
        GungeonActions gungeonActions = GameManager.Instance.IsSelectingCharacter ? BraveInput.PlayerlessInstance.ActiveActions : BraveInput.GetInstanceForPlayer(GameManager.Instance.PrimaryPlayer.PlayerIDX).ActiveActions;
        if (gungeonActions == null)
          return string.Empty;
        PlayerAction actionFromType = gungeonActions.GetActionFromType(ActionType);
        if (actionFromType == null || actionFromType.Bindings == null)
          return string.Empty;
        bool flag = false;
        string str = "-";
        for (int index = 0; index < actionFromType.Bindings.Count; ++index)
        {
          BindingSource binding = actionFromType.Bindings[index];
          if ((binding.BindingSourceType == BindingSourceType.KeyBindingSource || binding.BindingSourceType == BindingSourceType.MouseBindingSource) && !flag)
          {
            str = binding.Name;
            break;
          }
        }
        return str.Trim();
      }

      private static PlayerController GetTalkingPlayer()
      {
        List<TalkDoerLite> allNpcs = StaticReferenceManager.AllNpcs;
        for (int index = 0; index < allNpcs.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) allNpcs[index] && (!allNpcs[index].IsTalking || !(bool) (UnityEngine.Object) allNpcs[index].TalkingPlayer || GameManager.Instance.HasPlayer(allNpcs[index].TalkingPlayer)) && allNpcs[index].IsTalking && (bool) (UnityEngine.Object) allNpcs[index].TalkingPlayer)
            return allNpcs[index].TalkingPlayer;
        }
        return GameManager.Instance.PrimaryPlayer;
      }

      private static string GetTalkingPlayerName()
      {
        PlayerController talkingPlayer = StringTableManager.GetTalkingPlayer();
        if (talkingPlayer.IsThief)
          return "#THIEF_NAME";
        if (talkingPlayer.characterIdentity == PlayableCharacters.Eevee)
          return "#PLAYER_NAME_RANDOM";
        return talkingPlayer.characterIdentity == PlayableCharacters.Gunslinger ? "#PLAYER_NAME_GUNSLINGER" : "#PLAYER_NAME_" + talkingPlayer.characterIdentity.ToString().ToUpperInvariant();
      }

      private static string GetTalkingPlayerNick()
      {
        PlayerController talkingPlayer = StringTableManager.GetTalkingPlayer();
        if (talkingPlayer.IsThief)
          return "#THIEF_NAME";
        if (talkingPlayer.characterIdentity == PlayableCharacters.Eevee)
          return "#PLAYER_NICK_RANDOM";
        return talkingPlayer.characterIdentity == PlayableCharacters.Gunslinger ? "#PLAYER_NICK_GUNSLINGER" : "#PLAYER_NICK_" + talkingPlayer.characterIdentity.ToString().ToUpperInvariant();
      }

      public static string GetPlayerName(PlayableCharacters player)
      {
        return StringTableManager.GetString("#PLAYER_NAME_" + player.ToString().ToUpperInvariant());
      }

      public static string EvaluateReplacementToken(string input)
      {
        BraveInput primaryPlayerInstance = BraveInput.PrimaryPlayerInstance;
        GungeonActions activeActions = !((UnityEngine.Object) primaryPlayerInstance != (UnityEngine.Object) null) ? (GungeonActions) null : primaryPlayerInstance.ActiveActions;
        switch (input)
        {
          case "%META_CURRENCY_SYMBOL":
            return "[sprite \"hbux_text_icon\"]";
          case "%CURRENCY_SYMBOL":
            return "[sprite \"ui_coin\"]";
          case "%KEY_SYMBOL":
            return "[sprite \"ui_key\"]";
          case "%BLANK_SYMBOL":
            return "[sprite \"ui_blank\"]";
          case "%PLAYER_NAME":
            return StringTableManager.GetString(StringTableManager.GetTalkingPlayerName());
          case "%PLAYER_NICK":
            return StringTableManager.GetString(StringTableManager.GetTalkingPlayerNick());
          case "%BRACELETRED_ENCNAME":
            return StringTableManager.GetItemsString("#BRACELETRED_ENCNAME");
          case "%PLAYER_THIEF":
            return StringTableManager.GetString("#THIEF_NAME");
          case "%INSULT":
            return StringTableManager.GetString("#INSULT_NAME");
          case "%CONTROL_INTERACT_MAP":
            return primaryPlayerInstance.IsKeyboardAndMouse() ? StringTableManager.GetBindingText(GungeonActions.GungeonActionType.Interact) : UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.Action1, BraveInput.PlayerOneCurrentSymbology);
          case "%CONTROL_INTERACT":
            if (primaryPlayerInstance.IsKeyboardAndMouse())
              return StringTableManager.GetBindingText(GungeonActions.GungeonActionType.Interact);
            if (activeActions != null && activeActions.InteractAction.Bindings.Count > 0)
            {
              DeviceBindingSource binding = activeActions.InteractAction.Bindings[0] as DeviceBindingSource;
              if ((BindingSource) binding != (BindingSource) null && binding.Control != InputControlType.None)
                return UIControllerButtonHelper.GetUnifiedControllerButtonTag(binding.Control, BraveInput.PlayerOneCurrentSymbology);
            }
            return UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.Action1, BraveInput.PlayerOneCurrentSymbology);
          case "%CONTROL_DODGEROLL":
            if (primaryPlayerInstance.IsKeyboardAndMouse())
              return StringTableManager.GetBindingText(GungeonActions.GungeonActionType.DodgeRoll);
            if (activeActions != null && activeActions.DodgeRollAction.Bindings.Count > 0)
            {
              DeviceBindingSource binding = activeActions.DodgeRollAction.Bindings[0] as DeviceBindingSource;
              if ((BindingSource) binding != (BindingSource) null && binding.Control != InputControlType.None)
                return UIControllerButtonHelper.GetUnifiedControllerButtonTag(binding.Control, BraveInput.PlayerOneCurrentSymbology);
            }
            return GameManager.Options.CurrentControlPreset == GameOptions.ControlPreset.FLIPPED_TRIGGERS ? UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.LeftTrigger, BraveInput.PlayerOneCurrentSymbology) : UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.LeftBumper, BraveInput.PlayerOneCurrentSymbology);
          case "%CONTROL_PAUSE":
            if (primaryPlayerInstance.IsKeyboardAndMouse())
              return StringTableManager.GetBindingText(GungeonActions.GungeonActionType.Pause);
            if (activeActions != null && activeActions.PauseAction.Bindings.Count > 0)
            {
              DeviceBindingSource binding = activeActions.PauseAction.Bindings[0] as DeviceBindingSource;
              if ((BindingSource) binding != (BindingSource) null && binding.Control != InputControlType.None)
                return UIControllerButtonHelper.GetUnifiedControllerButtonTag(binding.Control, BraveInput.PlayerOneCurrentSymbology);
            }
            return UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.Start, BraveInput.PlayerOneCurrentSymbology);
          case "%CONTROL_USEITEM":
            if (primaryPlayerInstance.IsKeyboardAndMouse())
              return StringTableManager.GetBindingText(GungeonActions.GungeonActionType.UseItem);
            if (activeActions != null && activeActions.UseItemAction.Bindings.Count > 0)
            {
              DeviceBindingSource binding = activeActions.UseItemAction.Bindings[0] as DeviceBindingSource;
              if ((BindingSource) binding != (BindingSource) null && binding.Control != InputControlType.None)
                return UIControllerButtonHelper.GetUnifiedControllerButtonTag(binding.Control, BraveInput.PlayerOneCurrentSymbology);
            }
            return GameManager.Options.CurrentControlPreset == GameOptions.ControlPreset.FLIPPED_TRIGGERS ? UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.RightBumper, BraveInput.PlayerOneCurrentSymbology) : UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.RightTrigger, BraveInput.PlayerOneCurrentSymbology);
          case "%CONTROL_USEBLANK":
            if (activeActions != null && activeActions.BlankAction.Bindings.Count > 0)
            {
              DeviceBindingSource binding = activeActions.BlankAction.Bindings[0] as DeviceBindingSource;
              if ((BindingSource) binding != (BindingSource) null && binding.Control != InputControlType.None)
                return UIControllerButtonHelper.GetUnifiedControllerButtonTag(binding.Control, BraveInput.PlayerOneCurrentSymbology);
            }
            return StringTableManager.GetBindingText(GungeonActions.GungeonActionType.Blank);
          case "%CONTROL_R_STICK_DOWN":
            if (BraveInput.PlayerOneCurrentSymbology == GameOptions.ControllerSymbology.Xbox)
              return "[sprite \"xbone_RS\"]";
            return BraveInput.PlayerOneCurrentSymbology == GameOptions.ControllerSymbology.Switch ? "[sprite \"switch_r3\"]" : "[sprite \"ps4_R3\"]";
          case "%CONTROL_L_STICK_DOWN":
            if (BraveInput.PlayerOneCurrentSymbology == GameOptions.ControllerSymbology.Xbox)
              return "[sprite \"xbone_LS\"]";
            return BraveInput.PlayerOneCurrentSymbology == GameOptions.ControllerSymbology.Switch ? "[sprite \"switch_l3\"]" : "[sprite \"ps4_L3\"]";
          case "%CONTROL_ALT_DODGEROLL":
            return primaryPlayerInstance.IsKeyboardAndMouse() ? "Circle" : UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.Action2, BraveInput.PlayerOneCurrentSymbology);
          case "%CONTROL_AIM":
            return primaryPlayerInstance.IsKeyboardAndMouse() ? "Mouse" : UIControllerButtonHelper.GetUnifiedControllerButtonTag("RightStick", BraveInput.PlayerOneCurrentSymbology);
          case "%SYMBOL_TELEPORTER":
            return UIControllerButtonHelper.GetUnifiedControllerButtonTag("Teleporter", BraveInput.PlayerOneCurrentSymbology);
          case "%CONTROL_FIRE":
            if (primaryPlayerInstance.IsKeyboardAndMouse())
              return StringTableManager.GetBindingText(GungeonActions.GungeonActionType.Shoot);
            if (activeActions != null && activeActions.ShootAction.Bindings.Count > 0)
            {
              DeviceBindingSource binding = activeActions.ShootAction.Bindings[0] as DeviceBindingSource;
              if ((BindingSource) binding != (BindingSource) null && binding.Control != InputControlType.None)
                return UIControllerButtonHelper.GetUnifiedControllerButtonTag(binding.Control, BraveInput.PlayerOneCurrentSymbology);
            }
            return GameManager.Options.CurrentControlPreset == GameOptions.ControlPreset.FLIPPED_TRIGGERS ? UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.RightTrigger, BraveInput.PlayerOneCurrentSymbology) : UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.RightBumper, BraveInput.PlayerOneCurrentSymbology);
          case "%CONTROL_MAP":
            if (primaryPlayerInstance.IsKeyboardAndMouse())
              return StringTableManager.GetBindingText(GungeonActions.GungeonActionType.Map);
            if (activeActions != null && activeActions.MapAction.Bindings.Count > 0)
            {
              DeviceBindingSource binding = activeActions.MapAction.Bindings[0] as DeviceBindingSource;
              if ((BindingSource) binding != (BindingSource) null && binding.Control != InputControlType.None)
                return UIControllerButtonHelper.GetUnifiedControllerButtonTag(binding.Control, BraveInput.PlayerOneCurrentSymbology);
            }
            return GameManager.Options.CurrentControlPreset == GameOptions.ControlPreset.FLIPPED_TRIGGERS ? UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.LeftBumper, BraveInput.PlayerOneCurrentSymbology) : UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.LeftTrigger, BraveInput.PlayerOneCurrentSymbology);
          case "%CONTROL_RELOAD":
            if (primaryPlayerInstance.IsKeyboardAndMouse())
              return StringTableManager.GetBindingText(GungeonActions.GungeonActionType.Reload);
            if (activeActions != null && activeActions.ReloadAction.Bindings.Count > 0)
            {
              DeviceBindingSource binding = activeActions.ReloadAction.Bindings[0] as DeviceBindingSource;
              if ((BindingSource) binding != (BindingSource) null && binding.Control != InputControlType.None)
                return UIControllerButtonHelper.GetUnifiedControllerButtonTag(binding.Control, BraveInput.PlayerOneCurrentSymbology);
            }
            return UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.Action3, BraveInput.PlayerOneCurrentSymbology);
          case "%CONTROL_QUICKSWITCHGUN":
            if (primaryPlayerInstance.IsKeyboardAndMouse())
              return StringTableManager.GetBindingText(GungeonActions.GungeonActionType.GunQuickEquip);
            if (activeActions != null && activeActions.GunQuickEquipAction.Bindings.Count > 0)
            {
              DeviceBindingSource binding = activeActions.GunQuickEquipAction.Bindings[0] as DeviceBindingSource;
              if ((BindingSource) binding != (BindingSource) null && binding.Control != InputControlType.None)
                return UIControllerButtonHelper.GetUnifiedControllerButtonTag(binding.Control, BraveInput.PlayerOneCurrentSymbology);
            }
            return UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.Action4, BraveInput.PlayerOneCurrentSymbology);
          case "%CONTROL_SWITCHGUN_ALT":
            return primaryPlayerInstance.IsKeyboardAndMouse() ? StringTableManager.GetBindingText(GungeonActions.GungeonActionType.DropGun) : UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.DPadDown, BraveInput.PlayerOneCurrentSymbology);
          case "%CONTROL_CIRCLE":
            return UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.Action2, BraveInput.PlayerOneCurrentSymbology);
          case "%CONTROL_L1":
            return UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.LeftBumper, BraveInput.PlayerOneCurrentSymbology);
          case "%CONTROL_L2":
            return UIControllerButtonHelper.GetUnifiedControllerButtonTag(InputControlType.LeftTrigger, BraveInput.PlayerOneCurrentSymbology);
          case "%CONTROL_ALL_FACE_BUTTONS":
            return "[sprite \"switch_single_all\"]";
          case "%ESCAPE_ROPE_SYMBOL":
            return "[sprite \"escape_rope_text_icon_001\"]";
          case "%ARMOR_SYMBOL":
            return "[sprite \"armor_money_icon_001\"]";
          case "%CHAMBER1_MASTERY_TOKEN_SYMBOL":
            return "[sprite \"master_token_icon_001\"]";
          case "%CHAMBER2_MASTERY_TOKEN_SYMBOL":
            return "[sprite \"master_token_icon_002\"]";
          case "%CHAMBER3_MASTERY_TOKEN_SYMBOL":
            return "[sprite \"master_token_icon_003\"]";
          case "%CHAMBER4_MASTERY_TOKEN_SYMBOL":
            return "[sprite \"master_token_icon_004\"]";
          case "%CHAMBER5_MASTERY_TOKEN_SYMBOL":
            return "[sprite \"master_token_icon_005\"]";
          case "%HEART_SYMBOL":
            return "[sprite \"heart_big_idle_001\"]";
          case "%JUNK_SYMBOL":
            return "[sprite \"poopsack_001\"]";
          case "%BTCKTP_PRIMER":
            return "[sprite \"forged_bullet_primer_001\"]";
          case "%BTCKTP_POWDER":
            return "[sprite \"forged_bullet_powder_001\"]";
          case "%BTCKTP_SLUG":
            return "[sprite \"forged_bullet_slug_001\"]";
          case "%BTCKTP_CASING":
            return "[sprite \"forged_bullet_case_001\"]";
          case "%PLAYTIMEHOURS":
            return $"{(ValueType) (float) ((double) GameStatsManager.Instance.GetPlayerStatValue(TrackedStats.TIME_PLAYED) / 3600.0):0.0}";
          default:
            return input;
        }
      }

      private static bool CharIsEnglishAlphaNumeric(char c) => char.IsLetterOrDigit(c) && c < '׀';

      public static string PostprocessString(string input)
      {
        if (StringTableManager.m_postprocessors == null)
          StringTableManager.m_postprocessors = new Stack<StringBuilder>();
        if (StringTableManager.m_tokenLists == null)
          StringTableManager.m_tokenLists = new Stack<List<string>>();
        StringBuilder stringBuilder = StringTableManager.m_postprocessors.Count <= 0 ? new StringBuilder() : StringTableManager.m_postprocessors.Pop();
        List<string> stringList = StringTableManager.m_tokenLists.Count <= 0 ? new List<string>() : StringTableManager.m_tokenLists.Pop();
        int startIndex = 0;
        for (int index = 0; index < input.Length; ++index)
        {
          char c = input[index];
          if (!StringTableManager.CharIsEnglishAlphaNumeric(c) && c != '_')
          {
            stringList.Add(input.Substring(startIndex, index - startIndex));
            startIndex = index;
          }
        }
        stringList.Add(input.Substring(startIndex, input.Length - startIndex));
        for (int index = 0; index < stringList.Count; ++index)
        {
          if (stringList[index] != null && stringList[index].Length != 0)
          {
            if (stringList[index][0] == '%')
            {
              bool flag = false;
              if (index < stringList.Count - 1 && stringList[index + 1].Length > 0 && stringList[index] == "%KEY_SYMBOL" && stringList[index + 1][0] == '?')
                flag = true;
              string replacementToken = StringTableManager.EvaluateReplacementToken(stringList[index]);
              stringBuilder.Append(replacementToken);
              if (flag)
                stringBuilder.Append(' ');
            }
            else
              stringBuilder.Append(stringList[index]);
          }
        }
        string str = stringBuilder.ToString();
        stringBuilder.Length = 0;
        stringList.Clear();
        StringTableManager.m_postprocessors.Push(stringBuilder);
        StringTableManager.m_tokenLists.Push(stringList);
        return str;
      }

      public static Dictionary<string, StringTableManager.StringCollection> IntroTable
      {
        get
        {
          if (StringTableManager.m_introTable == null)
            StringTableManager.m_introTable = StringTableManager.LoadIntroTable(StringTableManager.m_currentSubDirectory);
          if (StringTableManager.m_backupIntroTable == null)
            StringTableManager.m_backupIntroTable = StringTableManager.LoadIntroTable("english_items");
          return StringTableManager.m_introTable;
        }
      }

      public static Dictionary<string, StringTableManager.StringCollection> CoreTable
      {
        get
        {
          if (StringTableManager.m_coreTable == null)
            StringTableManager.m_coreTable = StringTableManager.LoadTables(StringTableManager.m_currentFile);
          if (StringTableManager.m_backupCoreTable == null)
            StringTableManager.m_backupCoreTable = StringTableManager.LoadTables("english");
          return StringTableManager.m_coreTable;
        }
      }

      public static Dictionary<string, StringTableManager.StringCollection> ItemTable
      {
        get
        {
          if (StringTableManager.m_itemsTable == null)
            StringTableManager.m_itemsTable = StringTableManager.LoadItemsTable(StringTableManager.m_currentSubDirectory);
          if (StringTableManager.m_backupItemsTable == null)
            StringTableManager.m_backupItemsTable = StringTableManager.LoadItemsTable("english_items");
          return StringTableManager.m_itemsTable;
        }
      }

      public static Dictionary<string, StringTableManager.StringCollection> EnemyTable
      {
        get
        {
          if (StringTableManager.m_enemiesTable == null)
            StringTableManager.m_enemiesTable = StringTableManager.LoadEnemiesTable(StringTableManager.m_currentSubDirectory);
          if (StringTableManager.m_backupEnemiesTable == null)
            StringTableManager.m_backupEnemiesTable = StringTableManager.LoadEnemiesTable("english_items");
          return StringTableManager.m_enemiesTable;
        }
      }

      public enum GungeonSupportedLanguages
      {
        ENGLISH,
        RUBEL_TEST,
        FRENCH,
        SPANISH,
        ITALIAN,
        GERMAN,
        BRAZILIANPORTUGUESE,
        JAPANESE,
        KOREAN,
        RUSSIAN,
        POLISH,
        CHINESE,
      }

      public abstract class StringCollection
      {
        public abstract int Count();

        public abstract void AddString(string value, float weight);

        public abstract string GetCombinedString();

        public abstract string GetExactString(int index);

        public abstract string GetWeightedString();

        public abstract string GetWeightedStringSequential(
          ref int lastIndex,
          out bool isLast,
          bool repeatLast = false);
      }

      public class SimpleStringCollection : StringTableManager.StringCollection
      {
        private string singleString;

        public override int Count() => 1;

        public override void AddString(string value, float weight) => this.singleString = value;

        public override string GetCombinedString() => this.singleString;

        public override string GetExactString(int index) => this.singleString;

        public override string GetWeightedString() => this.singleString;

        public override string GetWeightedStringSequential(
          ref int lastIndex,
          out bool isLast,
          bool repeatLast = false)
        {
          isLast = true;
          return this.singleString;
        }
      }

      public class ComplexStringCollection : StringTableManager.StringCollection
      {
        private List<string> strings;
        private List<float> weights;
        private float maxWeight;

        public ComplexStringCollection()
        {
          this.strings = new List<string>();
          this.weights = new List<float>();
          this.maxWeight = 0.0f;
        }

        public override int Count() => this.strings.Count;

        public override void AddString(string value, float weight)
        {
          this.strings.Add(value);
          this.weights.Add(weight);
          this.maxWeight += weight;
        }

        public override string GetCombinedString()
        {
          StringBuilder stringBuilder = new StringBuilder();
          for (int index = 0; index < this.strings.Count; ++index)
            stringBuilder.AppendLine(this.strings[index]);
          return stringBuilder.ToString();
        }

        public override string GetExactString(int index)
        {
          return this.strings.Count == 0 ? string.Empty : this.strings[index % this.strings.Count];
        }

        public override string GetWeightedString()
        {
          float num1 = UnityEngine.Random.value * this.maxWeight;
          float num2 = 0.0f;
          for (int index = 0; index < this.strings.Count; ++index)
          {
            num2 += this.weights[index];
            if ((double) num2 >= (double) num1)
              return this.strings[index];
          }
          return this.strings.Count == 0 ? (string) null : this.strings[0];
        }

        public override string GetWeightedStringSequential(
          ref int lastIndex,
          out bool isLast,
          bool repeatLast = false)
        {
          int index = lastIndex + 1;
          isLast = index == this.strings.Count - 1;
          if (index >= this.strings.Count)
          {
            if (repeatLast)
            {
              index = lastIndex;
              isLast = true;
            }
            else
              index = 0;
          }
          if (index < 0)
            index = 0;
          if (index >= this.strings.Count)
            index = this.strings.Count - 1;
          if (this.strings.Count == 0)
            return string.Empty;
          lastIndex = index;
          return this.strings[index];
        }
      }
    }

}
