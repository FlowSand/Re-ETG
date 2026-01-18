using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using FullInspector;
using UnityEngine;

#nullable disable

    public static class SaveManager
    {
        public static SaveManager.SaveType GameSave = new SaveManager.SaveType()
        {
            filePattern = "Slot{0}.save",
            encrypted = true,
            backupCount = 3,
            backupPattern = "Slot{0}.backup.{1}",
            backupMinTimeMin = 45,
            legacyFilePattern = "gameStatsSlot{0}.txt"
        };
        public static SaveManager.SaveType OptionsSave = new SaveManager.SaveType()
        {
            filePattern = "Slot{0}.options",
            legacyFilePattern = "optionsSlot{0}.txt"
        };
        public static SaveManager.SaveType MidGameSave = new SaveManager.SaveType()
        {
            filePattern = "Active{0}.game",
            legacyFilePattern = "activeSlot{0}.txt",
            encrypted = true,
            backupCount = 0,
            backupPattern = "Active{0}.backup.{1}",
            backupMinTimeMin = 60
        };
        public static SaveManager.SaveSlot CurrentSaveSlot;
        public static SaveManager.SaveSlot? TargetSaveSlot;
        public static bool ResetSaveSlot;
        public static bool PreventMidgameSaveDeletionOnExit;
        private static bool s_hasBeenInitialized;
        public static string OldSavePath = Path.Combine(Application.dataPath, "saves");
        public static string SavePath = Application.persistentDataPath;
        private static string secret = PrivateImplementationDetails_DE5600AD_6212_4D84_9A32_9D951E3289D1.Decrypt.DecryptLiteral(new byte[512 /*0x0200*/]
        {
            (byte) 107,
            (byte) 164,
            (byte) 67,
            (byte) 89,
            (byte) 49,
            (byte) 25,
            (byte) 207,
            (byte) 88,
            (byte) 51,
            (byte) 60,
            (byte) 248,
            (byte) 156,
            (byte) 50,
            (byte) 78,
            (byte) 62,
            (byte) 211,
            (byte) 54,
            (byte) 174,
            (byte) 103,
            (byte) 13,
            (byte) 39,
            (byte) 68,
            (byte) 125,
            (byte) 41,
            (byte) 212,
            (byte) 32 /*0x20*/,
            (byte) 206,
            (byte) 226,
            (byte) 34,
            (byte) 63 /*0x3F*/,
            (byte) 197,
            (byte) 19,
            (byte) 19,
            (byte) 117,
            (byte) 113,
            (byte) 209,
            (byte) 103,
            (byte) 3,
            (byte) 1,
            (byte) 163,
            (byte) 61,
            (byte) 192 /*0xC0*/,
            (byte) 126,
            (byte) 0,
            (byte) 244,
            (byte) 203,
            (byte) 3,
            (byte) 4,
            (byte) 11,
            (byte) 108,
            (byte) 159,
            (byte) 196,
            (byte) 108,
            (byte) 214,
            (byte) 227,
            (byte) 208 /*0xD0*/,
            (byte) 152,
            (byte) 145,
            (byte) 17,
            (byte) 137,
            (byte) 89,
            (byte) 180,
            (byte) 195,
            (byte) 87,
            (byte) 96 /*0x60*/,
            (byte) 118,
            (byte) 244,
            (byte) 44,
            (byte) 199,
            (byte) 223,
            (byte) 239,
            (byte) 184,
            (byte) 22,
            (byte) 82,
            (byte) 128 /*0x80*/,
            (byte) 135,
            (byte) 64 /*0x40*/,
            (byte) 240 /*0xF0*/,
            (byte) 94,
            (byte) 185,
            (byte) 88,
            (byte) 205,
            (byte) 243,
            (byte) 96 /*0x60*/,
            (byte) 62,
            (byte) 87,
            (byte) 155,
            (byte) 104,
            (byte) 144 /*0x90*/,
            (byte) 192 /*0xC0*/,
            (byte) 34,
            (byte) 70,
            (byte) 1,
            (byte) 239,
            (byte) 161,
            (byte) 188,
            (byte) 14,
            (byte) 153,
            (byte) 124,
            (byte) 2,
            (byte) 246,
            (byte) 184,
            (byte) 50,
            (byte) 132,
            (byte) 244,
            (byte) 9,
            (byte) 206,
            (byte) 79,
            (byte) 200,
            (byte) 158,
            (byte) 157,
            (byte) 211,
            (byte) 245,
            (byte) 131,
            (byte) 63 /*0x3F*/,
            (byte) 188,
            (byte) 198,
            (byte) 235,
            (byte) 132,
            (byte) 123,
            (byte) 7,
            (byte) 13,
            (byte) 79,
            (byte) 198,
            (byte) 171,
            (byte) 90,
            (byte) 107,
            (byte) 236,
            (byte) 70,
            (byte) 239,
            (byte) 119,
            (byte) 197,
            (byte) 158,
            (byte) 76,
            (byte) 83,
            (byte) 10,
            (byte) 84,
            (byte) 218,
            (byte) 232,
            (byte) 25,
            (byte) 170,
            (byte) 217,
            (byte) 88,
            (byte) 66,
            (byte) 198,
            (byte) 250,
            (byte) 184,
            (byte) 192 /*0xC0*/,
            (byte) 176 /*0xB0*/,
            (byte) 105,
            (byte) 243,
            (byte) 82,
            (byte) 25,
            (byte) 247,
            (byte) 177,
            (byte) 63 /*0x3F*/,
            (byte) 181,
            (byte) 102,
            (byte) 253,
            (byte) 247,
            (byte) 214,
            (byte) 105,
            (byte) 219,
            (byte) 211,
            (byte) 176 /*0xB0*/,
            (byte) 131,
            (byte) 156,
            (byte) 84,
            (byte) 224 /*0xE0*/,
            (byte) 32 /*0x20*/,
            (byte) 229,
            (byte) 183,
            (byte) 82,
            (byte) 186,
            (byte) 243,
            (byte) 41,
            (byte) 165,
            (byte) 59,
            (byte) 238,
            (byte) 55,
            (byte) 229,
            (byte) 239,
            (byte) 53,
            (byte) 57,
            (byte) 253,
            (byte) 139,
            (byte) 100,
            (byte) 135,
            (byte) 34,
            (byte) 235,
            (byte) 11,
            (byte) 133,
            (byte) 93,
            (byte) 172,
            (byte) 63 /*0x3F*/,
            (byte) 83,
            (byte) 0,
            (byte) 152,
            (byte) 227,
            (byte) 53,
            (byte) 44,
            (byte) 3,
            (byte) 123,
            (byte) 81,
            (byte) 39,
            (byte) 204,
            (byte) 1,
            (byte) 22,
            (byte) 52,
            (byte) 97,
            (byte) 222,
            byte.MaxValue,
            (byte) 125,
            (byte) 39,
            (byte) 214,
            (byte) 138,
            (byte) 77,
            (byte) 75,
            (byte) 103,
            (byte) 7,
            (byte) 156,
            (byte) 155,
            (byte) 67,
            (byte) 97,
            (byte) 184,
            (byte) 169,
            (byte) 80 /*0x50*/,
            (byte) 31 /*0x1F*/,
            (byte) 69,
            (byte) 109,
            (byte) 67,
            (byte) 226,
            (byte) 79,
            (byte) 110,
            (byte) 76,
            (byte) 182,
            (byte) 224 /*0xE0*/,
            (byte) 186,
            (byte) 22,
            (byte) 101,
            (byte) 232,
            (byte) 81,
            (byte) 224 /*0xE0*/,
            (byte) 77,
            (byte) 4,
            (byte) 98,
            (byte) 97,
            (byte) 103,
            (byte) 131,
            (byte) 61,
            (byte) 71,
            (byte) 4,
            (byte) 99,
            (byte) 206,
            (byte) 0,
            (byte) 14,
            (byte) 95,
            (byte) 73,
            (byte) 235,
            (byte) 147,
            (byte) 40,
            (byte) 79,
            (byte) 233,
            (byte) 6,
            (byte) 102,
            (byte) 85,
            (byte) 70,
            (byte) 225,
            (byte) 163,
            (byte) 63 /*0x3F*/,
            (byte) 160 /*0xA0*/,
            (byte) 182,
            (byte) 233,
            (byte) 37,
            (byte) 148,
            (byte) 56,
            (byte) 205,
            (byte) 109,
            (byte) 155,
            (byte) 0,
            (byte) 10,
            (byte) 243,
            (byte) 34,
            (byte) 10,
            (byte) 12,
            (byte) 97,
            (byte) 103,
            (byte) 208 /*0xD0*/,
            (byte) 119,
            (byte) 134,
            (byte) 48 /*0x30*/,
            (byte) 61,
            (byte) 52,
            (byte) 69,
            (byte) 172,
            (byte) 234,
            (byte) 68,
            (byte) 57,
            (byte) 166,
            (byte) 56,
            (byte) 200,
            (byte) 156,
            (byte) 208 /*0xD0*/,
            (byte) 23,
            (byte) 44,
            (byte) 65,
            (byte) 247,
            (byte) 229,
            (byte) 41,
            (byte) 254,
            (byte) 213,
            (byte) 44,
            (byte) 138,
            (byte) 242,
            (byte) 224 /*0xE0*/,
            (byte) 126,
            (byte) 192 /*0xC0*/,
            (byte) 90,
            (byte) 108,
            (byte) 194,
            (byte) 124,
            (byte) 130,
            (byte) 123,
            (byte) 166,
            (byte) 114,
            (byte) 136,
            (byte) 36,
            (byte) 173,
            (byte) 235,
            (byte) 13,
            (byte) 82,
            (byte) 108,
            (byte) 19,
            (byte) 120,
            (byte) 168,
            (byte) 62,
            (byte) 61,
            (byte) 122,
            (byte) 111,
            (byte) 176 /*0xB0*/,
            (byte) 173,
            (byte) 186,
            (byte) 40,
            (byte) 90,
            (byte) 80 /*0x50*/,
            (byte) 74,
            (byte) 253,
            (byte) 219,
            (byte) 206,
            (byte) 156,
            (byte) 117,
            (byte) 12,
            (byte) 28,
            (byte) 77,
            (byte) 229,
            (byte) 173,
            (byte) 218,
            (byte) 10,
            (byte) 33,
            (byte) 44,
            (byte) 207,
            (byte) 111,
            (byte) 164,
            (byte) 212,
            (byte) 133,
            (byte) 237,
            (byte) 87,
            (byte) 0,
            (byte) 233,
            (byte) 201,
            (byte) 143,
            (byte) 214,
            (byte) 221,
            (byte) 233,
            (byte) 86,
            (byte) 153,
            (byte) 49,
            (byte) 64 /*0x40*/,
            (byte) 151,
            (byte) 69,
            (byte) 1,
            (byte) 17,
            (byte) 50,
            (byte) 191,
            (byte) 59,
            (byte) 239,
            (byte) 43,
            (byte) 243,
            (byte) 197,
            (byte) 129,
            (byte) 190,
            (byte) 47,
            (byte) 237,
            (byte) 161,
            (byte) 69,
            (byte) 195,
            (byte) 136,
            (byte) 223,
            (byte) 174,
            (byte) 98,
            (byte) 171,
            byte.MaxValue,
            (byte) 75,
            (byte) 174,
            (byte) 101,
            (byte) 177,
            (byte) 69,
            (byte) 71,
            (byte) 115,
            (byte) 63 /*0x3F*/,
            (byte) 228,
            (byte) 67,
            (byte) 89,
            (byte) 114,
            (byte) 66,
            (byte) 42,
            (byte) 160 /*0xA0*/,
            (byte) 226,
            (byte) 61,
            (byte) 213,
            (byte) 254,
            (byte) 151,
            (byte) 66,
            (byte) 222,
            (byte) 47,
            (byte) 247,
            (byte) 59,
            (byte) 130,
            (byte) 47,
            (byte) 53,
            (byte) 101,
            (byte) 12,
            (byte) 140,
            (byte) 207,
            (byte) 11,
            (byte) 150,
            (byte) 172,
            (byte) 9,
            (byte) 147,
            (byte) 162,
            (byte) 240 /*0xF0*/,
            (byte) 61,
            (byte) 29,
            (byte) 156,
            (byte) 223,
            (byte) 49,
            (byte) 162,
            (byte) 105,
            (byte) 19,
            (byte) 232,
            (byte) 212,
            (byte) 177,
            (byte) 184,
            (byte) 91,
            (byte) 49,
            (byte) 106,
            (byte) 8,
            (byte) 130,
            (byte) 151,
            (byte) 213,
            (byte) 81,
            (byte) 23,
            (byte) 154,
            (byte) 45,
            (byte) 8,
            (byte) 252,
            (byte) 212,
            (byte) 186,
            (byte) 70,
            (byte) 94,
            (byte) 51,
            (byte) 148,
            (byte) 7,
            (byte) 99,
            (byte) 155,
            (byte) 117,
            (byte) 74,
            (byte) 51,
            (byte) 30,
            (byte) 170,
            (byte) 203,
            (byte) 200,
            (byte) 46,
            (byte) 51,
            (byte) 146,
            (byte) 214,
            (byte) 94,
            (byte) 14,
            (byte) 84,
            (byte) 30,
            (byte) 89,
            (byte) 23,
            (byte) 193,
            (byte) 141,
            (byte) 63 /*0x3F*/,
            (byte) 13,
            (byte) 162,
            (byte) 19,
            (byte) 27,
            (byte) 199,
            (byte) 80 /*0x50*/,
            (byte) 206,
            (byte) 186,
            (byte) 115,
            (byte) 52,
            (byte) 128 /*0x80*/,
            (byte) 227,
            (byte) 139,
            (byte) 123,
            (byte) 247,
            (byte) 24,
            (byte) 20
        });

        public static void Init()
        {
            if (SaveManager.s_hasBeenInitialized)
                return;
            if (string.IsNullOrEmpty(SaveManager.SavePath))
            {
                Debug.LogError((object) ("Application.persistentDataPath FAILED! " + SaveManager.SavePath));
                SaveManager.SavePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "../LocalLow/Dodge Roll/Enter the Gungeon");
            }
            if (!Directory.Exists(SaveManager.SavePath))
            {
                try
                {
                    Debug.LogWarning((object) "Manually create default save directory!");
                    Directory.CreateDirectory(SaveManager.SavePath);
                }
                catch (Exception ex)
                {
                    Debug.LogError((object) ("Failed to create default save directory: " + ex.Message));
                }
            }
            int num = Brave.PlayerPrefs.GetInt("saveslot", 0);
            Brave.PlayerPrefs.SetInt("saveslot", num);
            if (num < 0 || num > 3)
                num = 0;
            SaveManager.CurrentSaveSlot = (SaveManager.SaveSlot) num;
            for (int index = 0; index < 3; ++index)
            {
                SaveManager.SaveSlot saveSlot = (SaveManager.SaveSlot) index;
                SaveManager.SafeMove(Path.Combine(SaveManager.OldSavePath, string.Format(SaveManager.GameSave.legacyFilePattern, (object) saveSlot)), Path.Combine(SaveManager.OldSavePath, string.Format(SaveManager.GameSave.filePattern, (object) saveSlot)));
                SaveManager.SafeMove(Path.Combine(SaveManager.OldSavePath, string.Format(SaveManager.OptionsSave.legacyFilePattern, (object) saveSlot)), Path.Combine(SaveManager.OldSavePath, string.Format(SaveManager.OptionsSave.filePattern, (object) saveSlot)));
                SaveManager.SafeMove(Path.Combine(SaveManager.OldSavePath, string.Format(SaveManager.GameSave.filePattern, (object) saveSlot)), Path.Combine(SaveManager.SavePath, string.Format(SaveManager.GameSave.filePattern, (object) saveSlot)));
                SaveManager.SafeMove(Path.Combine(SaveManager.OldSavePath, string.Format(SaveManager.OptionsSave.filePattern, (object) saveSlot)), Path.Combine(SaveManager.SavePath, string.Format(SaveManager.OptionsSave.filePattern, (object) saveSlot)));
                SaveManager.SafeMove(SaveManager.PathCombine(SaveManager.SavePath, "01", string.Format(SaveManager.GameSave.filePattern, (object) saveSlot)), Path.Combine(SaveManager.SavePath, string.Format(SaveManager.GameSave.filePattern, (object) saveSlot)), true);
                SaveManager.SafeMove(SaveManager.PathCombine(SaveManager.SavePath, "01", string.Format(SaveManager.OptionsSave.filePattern, (object) saveSlot)), Path.Combine(SaveManager.SavePath, string.Format(SaveManager.OptionsSave.filePattern, (object) saveSlot)), true);
            }
            SaveManager.s_hasBeenInitialized = true;
        }

        private static string PathCombine(string a, string b, string c)
        {
            return Path.Combine(Path.Combine(a, b), c);
        }

        public static void ChangeSlot(SaveManager.SaveSlot newSaveSlot)
        {
            if (!SaveManager.s_hasBeenInitialized)
                Debug.LogErrorFormat("Tried to change save slots before before SaveManager was initialized! {0}", (object) newSaveSlot);
            SaveManager.CurrentSaveSlot = newSaveSlot;
            Brave.PlayerPrefs.SetInt("saveslot", (int) SaveManager.CurrentSaveSlot);
        }

        public static void DeleteCurrentSlotMidGameSave(SaveManager.SaveSlot? overrideSaveSlot = null)
        {
            Debug.LogError((object) "DELETING CURRENT MID GAME SAVE");
            if (GameStatsManager.HasInstance)
                GameStatsManager.Instance.midGameSaveGuid = (string) null;
            string path2 = string.Format(SaveManager.MidGameSave.filePattern, (object) (SaveManager.SaveSlot) (!overrideSaveSlot.HasValue ? (int) SaveManager.CurrentSaveSlot : (int) overrideSaveSlot.Value));
            string path = Path.Combine(SaveManager.SavePath, path2);
            if (!File.Exists(path))
                return;
            File.Delete(path);
        }

        public static bool Save<T>(
            T obj,
            SaveManager.SaveType saveType,
            int playTimeMin,
            uint versionNumber = 0,
            SaveManager.SaveSlot? overrideSaveSlot = null)
        {
            bool encrypted = saveType.encrypted;
            if (!SaveManager.s_hasBeenInitialized)
            {
                Debug.LogErrorFormat("Tried to save data before SaveManager was initialized! {0} {1}", (object) obj.GetType(), (object) saveType.filePattern);
                return false;
            }
            string path2_1 = string.Format(saveType.filePattern, (object) (SaveManager.SaveSlot) (!overrideSaveSlot.HasValue ? (int) SaveManager.CurrentSaveSlot : (int) overrideSaveSlot.Value));
            string str1 = Path.Combine(SaveManager.SavePath, path2_1);
            string plaintext;
            try
            {
                bool printSerializedJson = fiSettings.PrettyPrintSerializedJson;
                fiSettings.PrettyPrintSerializedJson = !encrypted;
                plaintext = SerializationHelpers.SerializeToContent<T, FullSerializerSerializer>(obj);
                fiSettings.PrettyPrintSerializedJson = printSerializedJson;
            }
            catch (Exception ex)
            {
                Debug.LogError((object) ("Failed to serialize save data: " + ex.Message));
                return false;
            }
            if (encrypted)
                plaintext = SaveManager.Encrypt(plaintext);
            string contents = $"version: {versionNumber}\n{plaintext}";
            if (!Directory.Exists(SaveManager.SavePath))
                Directory.CreateDirectory(SaveManager.SavePath);
            bool flag = false;
            if (File.Exists(str1))
            {
                try
                {
                    File.Copy(str1, str1 + ".temp", true);
                    flag = true;
                }
                catch (Exception ex)
                {
                    Debug.LogError((object) ("Failed to create a temporary copy of current save: " + ex.Message));
                    return false;
                }
            }
            try
            {
                SaveManager.WriteAllText(str1, contents);
            }
            catch (Exception ex1)
            {
                Debug.LogError((object) ("Failed to write new save data: " + ex1.Message));
                try
                {
                    File.Delete(str1);
                    File.Move(str1 + ".temp", str1);
                }
                catch (Exception ex2)
                {
                    Debug.LogError((object) ("Failed to restore temp save data: " + ex2.Message));
                }
                return false;
            }
            if (flag)
            {
                try
                {
                    if (File.Exists(str1 + ".temp"))
                        File.Delete(str1 + ".temp");
                }
                catch (Exception ex)
                {
                    Debug.LogError((object) ("Failed to replace temp save file: " + ex.Message));
                }
            }
            if (saveType.backupCount > 0)
            {
                int backupPlaytimeMinutes = SaveManager.GetLatestBackupPlaytimeMinutes(saveType, overrideSaveSlot);
                if (playTimeMin >= backupPlaytimeMinutes + saveType.backupMinTimeMin)
                {
                    string str2 = $"{playTimeMin / 60}h{playTimeMin % 60}m";
                    string path2_2 = string.Format(saveType.backupPattern, (object) (SaveManager.SaveSlot) (!overrideSaveSlot.HasValue ? (int) SaveManager.CurrentSaveSlot : (int) overrideSaveSlot.Value), (object) str2);
                    string path = Path.Combine(SaveManager.SavePath, path2_2);
                    try
                    {
                        SaveManager.WriteAllText(path, contents);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError((object) ("Failed to create new save backup: " + ex.Message));
                    }
                    SaveManager.DeleteOldBackups(saveType, overrideSaveSlot);
                }
            }
            return true;
        }

        public static bool Load<T>(
            SaveManager.SaveType saveType,
            out T obj,
            bool allowDecrypted,
            uint expectedVersion = 0,
            Func<string, uint, string> versionUpdater = null,
            SaveManager.SaveSlot? overrideSaveSlot = null)
        {
            obj = default (T);
            if (!SaveManager.s_hasBeenInitialized)
            {
                Debug.LogErrorFormat("Tried to load data before SaveManager was initialized! {0} {1}", (object) saveType.filePattern, (object) typeof (T));
                return false;
            }
            string path2 = string.Format(saveType.filePattern, (object) (SaveManager.SaveSlot) (!overrideSaveSlot.HasValue ? (int) SaveManager.CurrentSaveSlot : (int) overrideSaveSlot.Value));
            string path = Path.Combine(SaveManager.SavePath, path2);
            if (!File.Exists(path))
            {
                Debug.LogWarning((object) ("Save data doesn't exist: " + path2));
                return false;
            }
            string str1;
            try
            {
                str1 = SaveManager.ReadAllText(path);
            }
            catch (Exception ex)
            {
                Debug.LogError((object) ("Failed to read save data: " + ex.Message));
                return false;
            }
            uint num = 0;
            if (str1.StartsWith("version: "))
            {
                StringReader stringReader = new StringReader(str1);
                string str2 = stringReader.ReadLine();
                uint result;
                if (uint.TryParse(str2.Substring(9), out result))
                {
                    num = result;
                    str1 = stringReader.ReadToEnd();
                }
                else
                {
                    Debug.LogErrorFormat("Failed to read save version number (expected [{0}], got [{1}]", (object) expectedVersion, (object) str2.Substring(9));
                    return false;
                }
            }
            if (SaveManager.IsDataEncrypted(str1))
                str1 = SaveManager.Decrypt(str1);
            else if (!allowDecrypted)
            {
                Debug.LogError((object) "Save file corrupted!  Copying to a new file");
                string contents = $"version: {num}\n{str1}";
                try
                {
                    SaveManager.WriteAllText(path + ".corrupt", contents);
                }
                catch (Exception ex)
                {
                    Debug.LogError((object) ("Failed to save off the corrupted file: " + ex.Message));
                }
                return false;
            }
            if (num < expectedVersion && versionUpdater != null)
                str1 = versionUpdater(str1, num);
            obj = SerializationHelpers.DeserializeFromContent<T, FullSerializerSerializer>(str1);
            if ((object) obj != null)
                return true;
            Debug.LogError((object) "Save file corrupted!  Copying to a new file");
            try
            {
                str1 = SaveManager.ReadAllText(path);
            }
            catch (Exception ex)
            {
                Debug.LogError((object) ("Failed to read corrupted save data: " + ex.Message));
            }
            try
            {
                SaveManager.WriteAllText(path + ".corrupt", str1);
            }
            catch (Exception ex)
            {
                Debug.LogError((object) ("Failed to save off the corrupted file: " + ex.Message));
            }
            return false;
        }

        public static void WriteAllText(string path, string contents)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(contents);
            string str1 = "null";
            try
            {
                str1 = Path.GetTempFileName();
                if (Directory.Exists(Path.GetDirectoryName(str1)))
                {
                    using (FileStream fileStream = File.Create(str1, 4096 /*0x1000*/, FileOptions.WriteThrough))
                        fileStream.Write(bytes, 0, bytes.Length);
                    File.Delete(path);
                    File.Move(str1, path);
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Failed to write to temp file {0}: {1}", (object) str1, (object) ex);
            }
            string str2 = Path.Combine(Path.GetDirectoryName(path), Path.GetFileName(str1));
            using (FileStream fileStream = File.Create(str2, 4096 /*0x1000*/, FileOptions.WriteThrough))
                fileStream.Write(bytes, 0, bytes.Length);
            File.Delete(path);
            File.Move(str2, path);
        }

        public static string ReadAllText(string path) => File.ReadAllText(path, Encoding.UTF8);

        private static int GetLatestBackupPlaytimeMinutes(
            SaveManager.SaveType saveType,
            SaveManager.SaveSlot? overrideSaveSlot = null)
        {
            string pattern = string.Format(saveType.backupPattern, (object) (SaveManager.SaveSlot) (!overrideSaveSlot.HasValue ? (int) SaveManager.CurrentSaveSlot : (int) overrideSaveSlot.Value), (object) string.Empty) + "(?<hour>\\d+)h(?<min>\\d+)m";
            string[] files = Directory.GetFiles(SaveManager.SavePath);
            int backupPlaytimeMinutes = 0;
            for (int index = 0; index < files.Length; ++index)
            {
                Match match = Regex.Match(files[index], pattern, RegexOptions.Multiline);
                if (match.Success)
                {
                    int num = Convert.ToInt32(match.Groups["hour"].Captures[0].Value) * 60 + Convert.ToInt32(match.Groups["min"].Captures[0].Value);
                    if (num > backupPlaytimeMinutes)
                        backupPlaytimeMinutes = num;
                }
            }
            return backupPlaytimeMinutes;
        }

        private static void SafeMove(string oldPath, string newPath, bool allowOverwritting = false)
        {
            if (!File.Exists(oldPath) || !allowOverwritting && File.Exists(newPath))
                return;
            string contents = SaveManager.ReadAllText(oldPath);
            try
            {
                SaveManager.WriteAllText(newPath, contents);
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Failed to move {0} to {1}: {2}", (object) oldPath, (object) newPath, (object) ex);
                return;
            }
            try
            {
                File.Delete(oldPath);
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("Failed to delete old file {0}: {1}", (object) oldPath, (object) newPath, (object) ex);
                return;
            }
            if (!File.Exists(oldPath + ".bak"))
                return;
            File.Delete(oldPath + ".bak");
        }

        public static void DeleteAllBackups(
            SaveManager.SaveType saveType,
            SaveManager.SaveSlot? overrideSaveSlot = null)
        {
            string pattern = string.Format(saveType.backupPattern, (object) (SaveManager.SaveSlot) (!overrideSaveSlot.HasValue ? (int) SaveManager.CurrentSaveSlot : (int) overrideSaveSlot.Value), (object) string.Empty) + "(?<hour>\\d+)h(?<min>\\d+)m";
            string[] files = Directory.GetFiles(SaveManager.SavePath);
            for (int index = 0; index < files.Length; ++index)
            {
                if (Regex.Match(files[index], pattern, RegexOptions.Multiline).Success)
                {
                    try
                    {
                        File.Delete(files[index]);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError((object) ("Failed to remove backup file: " + ex.Message));
                        break;
                    }
                }
            }
        }

        private static void SafeMoveBackups(
            SaveManager.SaveType saveType,
            string oldPath,
            string newPath,
            SaveManager.SaveSlot? overrideSaveSlot = null)
        {
            string pattern = string.Format(saveType.backupPattern, (object) (SaveManager.SaveSlot) (!overrideSaveSlot.HasValue ? (int) SaveManager.CurrentSaveSlot : (int) overrideSaveSlot.Value), (object) string.Empty) + "(?<hour>\\d+)h(?<min>\\d+)m";
            string[] files = Directory.GetFiles(oldPath);
            for (int index = 0; index < files.Length; ++index)
            {
                if (Regex.Match(files[index], pattern, RegexOptions.Multiline).Success)
                    SaveManager.SafeMove(files[index], Path.Combine(newPath, Path.GetFileName(files[index])));
            }
        }

        private static void DeleteOldBackups(
            SaveManager.SaveType saveType,
            SaveManager.SaveSlot? overrideSaveSlot = null)
        {
            string pattern = string.Format(saveType.backupPattern, (object) (SaveManager.SaveSlot) (!overrideSaveSlot.HasValue ? (int) SaveManager.CurrentSaveSlot : (int) overrideSaveSlot.Value), (object) string.Empty) + "(?<hour>\\d+)h(?<min>\\d+)m";
            List<Tuple<string, int>> tupleList = new List<Tuple<string, int>>();
            string[] files = Directory.GetFiles(SaveManager.SavePath);
            for (int index = 0; index < files.Length; ++index)
            {
                Match match = Regex.Match(files[index], pattern, RegexOptions.Multiline);
                if (match.Success)
                {
                    int second = Convert.ToInt32(match.Groups["hour"].Captures[0].Value) * 60 + Convert.ToInt32(match.Groups["min"].Captures[0].Value);
                    tupleList.Add(Tuple.Create<string, int>(files[index], second));
                }
            }
            tupleList.Sort((Comparison<Tuple<string, int>>) ((a, b) => b.Second - a.Second));
            while (tupleList.Count > saveType.backupCount)
            {
                if (tupleList.Count <= 0)
                    break;
                try
                {
                    File.Delete(tupleList[tupleList.Count - 1].First);
                }
                catch (Exception ex)
                {
                    Debug.LogError((object) ("Failed to remove backup file: " + ex.Message));
                    break;
                }
                tupleList.RemoveAt(tupleList.Count - 1);
            }
        }

        private static string Encrypt(string plaintext)
        {
            SaveManager.FixSecret();
            StringBuilder stringBuilder = new StringBuilder(plaintext.Length);
            for (int index = 0; index < plaintext.Length; ++index)
                stringBuilder.Append((char) ((uint) plaintext[index] ^ (uint) SaveManager.secret[index % SaveManager.secret.Length]));
            return stringBuilder.ToString();
        }

        private static string Decrypt(string cypertext)
        {
            SaveManager.FixSecret();
            return SaveManager.Encrypt(cypertext);
        }

        private static bool IsDataEncrypted(string data)
        {
            SaveManager.FixSecret();
            char ch1 = '{';
            if (data.StartsWith(ch1.ToString()))
                return false;
            char ch2 = (char) (123U ^ (uint) SaveManager.secret[0]);
            return data.StartsWith(ch2.ToString());
        }

        private static void FixSecret()
        {
            if (!SaveManager.secret.StartsWith("å") || !SaveManager.secret.EndsWith("å"))
                return;
            SaveManager.secret = SaveManager.secret.Substring(1, SaveManager.secret.Length - 2);
        }

public class SaveType
        {
            public string legacyFilePattern;
            public string filePattern;
            public bool encrypted;
            public int backupCount;
            public int backupMinTimeMin;
            public string backupPattern;
        }

        public enum SaveSlot
        {
            A,
            B,
            C,
            D,
        }
    }

}
