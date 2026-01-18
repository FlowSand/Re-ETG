using System;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

#nullable disable

public class dfLanguageManager : MonoBehaviour
    {
        [SerializeField]
        private dfLanguageCode currentLanguage;
        [SerializeField]
        private TextAsset dataFile;
        [NonSerialized]
        private TextAsset backupDataFile;
        private Dictionary<string, string> strings = new Dictionary<string, string>();

        public dfLanguageCode CurrentLanguage => this.currentLanguage;

        public TextAsset DataFile
        {
            get => this.dataFile;
            set
            {
                if ((UnityEngine.Object) value != (UnityEngine.Object) this.dataFile)
                {
                    this.dataFile = value;
                    this.LoadLanguage(this.currentLanguage);
                }
                if (!((UnityEngine.Object) this.backupDataFile == (UnityEngine.Object) null))
                    return;
                this.backupDataFile = StringTableManager.GetBackupUIDataFile();
            }
        }

        public static void ChangeGungeonLanguage()
        {
            dfLanguageCode languageCodeFromGungeon = dfLanguageManager.GetLanguageCodeFromGungeon();
            foreach (dfLanguageManager dfLanguageManager in UnityEngine.Object.FindObjectsOfType<dfLanguageManager>())
                dfLanguageManager.LoadLanguage(languageCodeFromGungeon, true);
        }

        private static dfLanguageCode GetLanguageCodeFromGungeon()
        {
            switch (StringTableManager.CurrentLanguage)
            {
                case StringTableManager.GungeonSupportedLanguages.ENGLISH:
                    return dfLanguageCode.EN;
                case StringTableManager.GungeonSupportedLanguages.RUBEL_TEST:
                    return dfLanguageCode.QU;
                case StringTableManager.GungeonSupportedLanguages.FRENCH:
                    return dfLanguageCode.FR;
                case StringTableManager.GungeonSupportedLanguages.SPANISH:
                    return dfLanguageCode.ES;
                case StringTableManager.GungeonSupportedLanguages.ITALIAN:
                    return dfLanguageCode.IT;
                case StringTableManager.GungeonSupportedLanguages.GERMAN:
                    return dfLanguageCode.DE;
                case StringTableManager.GungeonSupportedLanguages.BRAZILIANPORTUGUESE:
                    return dfLanguageCode.PT;
                case StringTableManager.GungeonSupportedLanguages.JAPANESE:
                    return dfLanguageCode.JA;
                case StringTableManager.GungeonSupportedLanguages.KOREAN:
                    return dfLanguageCode.KO;
                case StringTableManager.GungeonSupportedLanguages.RUSSIAN:
                    return dfLanguageCode.RU;
                case StringTableManager.GungeonSupportedLanguages.POLISH:
                    return dfLanguageCode.PL;
                case StringTableManager.GungeonSupportedLanguages.CHINESE:
                    return dfLanguageCode.ZH;
                default:
                    return dfLanguageCode.EN;
            }
        }

        public void Start()
        {
            this.currentLanguage = dfLanguageManager.GetLanguageCodeFromGungeon();
            dfLanguageCode language = this.currentLanguage;
            if (this.currentLanguage == dfLanguageCode.None)
                language = this.SystemLanguageToLanguageCode(Application.systemLanguage);
            this.LoadLanguage(language, true);
        }

        private void BraveChangeDataFile(dfLanguageCode language)
        {
            this.dataFile = StringTableManager.GetUIDataFile();
            if (!((UnityEngine.Object) this.backupDataFile == (UnityEngine.Object) null))
                return;
            this.backupDataFile = StringTableManager.GetBackupUIDataFile();
        }

        public void LoadLanguage(dfLanguageCode language, bool forceReload = false)
        {
            this.currentLanguage = language;
            this.strings.Clear();
            this.BraveChangeDataFile(language);
            if ((UnityEngine.Object) this.dataFile != (UnityEngine.Object) null)
                this.parseDataFile();
            if (!forceReload)
                return;
            dfControl[] componentsInChildren = this.GetComponentsInChildren<dfControl>();
            for (int index = 0; index < componentsInChildren.Length; ++index)
                componentsInChildren[index].Localize();
            for (int index = 0; index < componentsInChildren.Length; ++index)
            {
                componentsInChildren[index].PerformLayout();
                if (componentsInChildren[index].LanguageChanged != null)
                    componentsInChildren[index].LanguageChanged(componentsInChildren[index]);
            }
        }

        public string GetValue(string key)
        {
            if (this.strings.Count == 0)
            {
                dfLanguageCode language = this.currentLanguage;
                if (this.currentLanguage == dfLanguageCode.None)
                    language = this.SystemLanguageToLanguageCode(Application.systemLanguage);
                this.LoadLanguage(language);
            }
            string empty = string.Empty;
            return this.strings.TryGetValue(key, out empty) ? empty : key;
        }

        private void parseDataFile()
        {
            string data1 = this.dataFile.text.Replace("\r\n", "\n").Trim();
            List<string> values1 = new List<string>();
            int line1 = this.parseLine(data1, values1, 0);
            int index1 = values1.IndexOf(this.currentLanguage.ToString());
            if (index1 < 0)
                return;
            List<string> values2 = new List<string>();
            while (line1 < data1.Length)
            {
                line1 = this.parseLine(data1, values2, line1);
                if (values2.Count != 0)
                    this.strings[values2[0]] = index1 >= values2.Count ? string.Empty : values2[index1];
            }
            string data2 = this.backupDataFile.text.Replace("\r\n", "\n").Trim();
            List<string> values3 = new List<string>();
            int line2 = this.parseLine(data2, values3, 0);
            int index2 = 1;
            List<string> values4 = new List<string>();
            while (line2 < data2.Length)
            {
                line2 = this.parseLine(data2, values4, line2);
                if (values4.Count != 0)
                {
                    string key = values4[0];
                    string str = index2 >= values4.Count ? string.Empty : values4[index2];
                    if (!this.strings.ContainsKey(key))
                        this.strings[key] = str;
                }
            }
        }

        private int parseLine(string data, List<string> values, int index)
        {
            values.Clear();
            bool flag = false;
            StringBuilder stringBuilder = new StringBuilder(256);
            for (; index < data.Length; ++index)
            {
                char ch = data[index];
                switch (ch)
                {
                    case '\n':
                        if (flag)
                        {
                            stringBuilder.Append(ch);
                            break;
                        }
                        ++index;
                        goto label_16;
                    case '"':
                        if (!flag)
                        {
                            flag = true;
                            break;
                        }
                        if (index + 1 < data.Length && (int) data[index + 1] == (int) ch)
                        {
                            ++index;
                            stringBuilder.Append(ch);
                            break;
                        }
                        flag = false;
                        break;
                    case ',':
                        if (flag)
                        {
                            stringBuilder.Append(ch);
                            break;
                        }
                        values.Add(stringBuilder.ToString());
                        stringBuilder.Length = 0;
                        break;
                    default:
                        stringBuilder.Append(ch);
                        break;
                }
            }
    label_16:
            if (stringBuilder.Length > 0)
                values.Add(stringBuilder.ToString());
            return index;
        }

        private dfLanguageCode SystemLanguageToLanguageCode(SystemLanguage language)
        {
            switch (language)
            {
                case SystemLanguage.Afrikaans:
                    return dfLanguageCode.AF;
                case SystemLanguage.Arabic:
                    return dfLanguageCode.AR;
                case SystemLanguage.Basque:
                    return dfLanguageCode.EU;
                case SystemLanguage.Belarusian:
                    return dfLanguageCode.BE;
                case SystemLanguage.Bulgarian:
                    return dfLanguageCode.BG;
                case SystemLanguage.Catalan:
                    return dfLanguageCode.CA;
                case SystemLanguage.Chinese:
                    return dfLanguageCode.ZH;
                case SystemLanguage.Czech:
                    return dfLanguageCode.CS;
                case SystemLanguage.Danish:
                    return dfLanguageCode.DA;
                case SystemLanguage.Dutch:
                    return dfLanguageCode.NL;
                case SystemLanguage.English:
                    return dfLanguageCode.EN;
                case SystemLanguage.Estonian:
                    return dfLanguageCode.ES;
                case SystemLanguage.Faroese:
                    return dfLanguageCode.FO;
                case SystemLanguage.Finnish:
                    return dfLanguageCode.FI;
                case SystemLanguage.French:
                    return dfLanguageCode.FR;
                case SystemLanguage.German:
                    return dfLanguageCode.DE;
                case SystemLanguage.Greek:
                    return dfLanguageCode.EL;
                case SystemLanguage.Hebrew:
                    return dfLanguageCode.HE;
                case SystemLanguage.Hungarian:
                    return dfLanguageCode.HU;
                case SystemLanguage.Icelandic:
                    return dfLanguageCode.IS;
                case SystemLanguage.Indonesian:
                    return dfLanguageCode.ID;
                case SystemLanguage.Italian:
                    return dfLanguageCode.IT;
                case SystemLanguage.Japanese:
                    return dfLanguageCode.JA;
                case SystemLanguage.Korean:
                    return dfLanguageCode.KO;
                case SystemLanguage.Latvian:
                    return dfLanguageCode.LV;
                case SystemLanguage.Lithuanian:
                    return dfLanguageCode.LT;
                case SystemLanguage.Norwegian:
                    return dfLanguageCode.NO;
                case SystemLanguage.Polish:
                    return dfLanguageCode.PL;
                case SystemLanguage.Portuguese:
                    return dfLanguageCode.PT;
                case SystemLanguage.Romanian:
                    return dfLanguageCode.RO;
                case SystemLanguage.Russian:
                    return dfLanguageCode.RU;
                case SystemLanguage.SerboCroatian:
                    return dfLanguageCode.SH;
                case SystemLanguage.Slovak:
                    return dfLanguageCode.SK;
                case SystemLanguage.Slovenian:
                    return dfLanguageCode.SL;
                case SystemLanguage.Spanish:
                    return dfLanguageCode.ES;
                case SystemLanguage.Swedish:
                    return dfLanguageCode.SV;
                case SystemLanguage.Thai:
                    return dfLanguageCode.TH;
                case SystemLanguage.Turkish:
                    return dfLanguageCode.TR;
                case SystemLanguage.Ukrainian:
                    return dfLanguageCode.UK;
                case SystemLanguage.Vietnamese:
                    return dfLanguageCode.VI;
                case SystemLanguage.Unknown:
                    return dfLanguageCode.EN;
                default:
                    throw new ArgumentException("Unknown system language: " + (object) language);
            }
        }
    }

