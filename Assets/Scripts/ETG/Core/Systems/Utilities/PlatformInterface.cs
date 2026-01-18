using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;

#nullable disable

    public abstract class PlatformInterface
    {
        public List<PlatformDlc> UnlockedDlc = new List<PlatformDlc>();
        public static float LastManyCoinsUnlockTime = 0.0f;
        private float m_lastDlcCheckTime;
        private bool m_hasCaughtUpAchievements;
        private static bool m_useLightFX = false;
        private static List<PlatformInterface.LightFXUnit> m_AlienFXExtantEffects = new List<PlatformInterface.LightFXUnit>();
        private static PlatformInterface.LightFXUnit m_AlienFXAmbientEffect;
        private PlatformInterface.DlcUnlockedItem[] m_dlcUnlockedItems = new PlatformInterface.DlcUnlockedItem[2]
        {
            new PlatformInterface.DlcUnlockedItem(PlatformDlc.EARLY_MTX_GUN, "5c2241fc117740d59ad8e29f5324b773", GungeonFlags.BLUEPRINTMETA_MTXGUN),
            new PlatformInterface.DlcUnlockedItem(PlatformDlc.EARLY_COBALT_HAMMER, "2d91904ba70a4c0d861dac03a6417591")
        };
        private bool m_hasCheckedForGalaxyMtx;

        public void Start()
        {
            this.OnStart();
            PlatformInterface.InitializeAlienFXController();
        }

        public virtual void SignIn()
        {
        }

        public void AchievementUnlock(Achievement achievement, int playerIndex = 0)
        {
            this.SetGungeonFlagForAchievement(achievement);
            this.OnAchievementUnlock(achievement, playerIndex);
        }

        public virtual bool IsAchievementUnlocked(Achievement achievement) => false;

        public virtual void SetStat(PlatformStat stat, int value)
        {
        }

        public virtual void IncrementStat(PlatformStat stat, int delta)
        {
        }

        public virtual void SendEvent(string eventName, int value)
        {
        }

        public virtual void SetPresence(string presence)
        {
        }

        public virtual void StoreStats()
        {
        }

        public virtual void ResetStats(bool achievementsToo)
        {
        }

        public void ProcessDlcUnlocks()
        {
            if ((double) UnityEngine.Time.realtimeSinceStartup < (double) this.m_lastDlcCheckTime + 1.0)
                return;
            this.GalaxyMtxGunHack();
            for (int index1 = 0; index1 < this.UnlockedDlc.Count; ++index1)
            {
                PlatformDlc platformDlc = this.UnlockedDlc[index1];
                for (int index2 = 0; index2 < this.m_dlcUnlockedItems.Length; ++index2)
                {
                    if (this.m_dlcUnlockedItems[index2].PlatformDlc == platformDlc)
                    {
                        PlatformInterface.DlcUnlockedItem dlcUnlockedItem = this.m_dlcUnlockedItems[index2];
                        EncounterDatabaseEntry entry = EncounterDatabase.GetEntry(dlcUnlockedItem.encounterGuid);
                        if (entry != null && !entry.PrerequisitesMet())
                            GameStatsManager.Instance.ForceUnlock(dlcUnlockedItem.encounterGuid);
                        if (dlcUnlockedItem.flag != GungeonFlags.NONE && !GameStatsManager.Instance.GetFlag(dlcUnlockedItem.flag))
                            GameStatsManager.Instance.SetFlag(dlcUnlockedItem.flag, true);
                    }
                }
            }
            this.m_lastDlcCheckTime = UnityEngine.Time.realtimeSinceStartup;
        }

        public void LateUpdate()
        {
            this.OnLateUpdate();
            PlatformInterface.UpdateAlienFXController();
        }

        public void CatchupAchievements()
        {
            if (this.m_hasCaughtUpAchievements)
                return;
            if (GameManager.Options.wipeAllAchievements)
            {
                this.ResetStats(true);
                GameManager.Options.wipeAllAchievements = false;
            }
            if (GameManager.Options.scanAchievementsForUnlocks)
            {
                if (this.IsAchievementUnlocked(Achievement.COLLECT_FIVE_MASTERY_TOKENS))
                    GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_LEAD_GOD, true);
                GameManager.Options.scanAchievementsForUnlocks = false;
            }
            foreach (TrackedStats stat in Enum.GetValues(typeof (TrackedStats)))
                GameStatsManager.Instance.HandleStatAchievements(stat);
            foreach (GungeonFlags flag in GameStatsManager.Instance.m_flags)
                GameStatsManager.Instance.HandleFlagAchievements(flag);
            if (GameStatsManager.Instance.GetFlag(GungeonFlags.ACHIEVEMENT_TABLE_PIT))
                this.AchievementUnlock(Achievement.PUSH_TABLE_INTO_PIT);
            if (GameStatsManager.Instance.GetFlag(GungeonFlags.ACHIEVEMENT_BEASTMODE))
                this.AchievementUnlock(Achievement.COMPLETE_GAME_WITH_BEAST_MODE);
            if (GameStatsManager.Instance.GetFlag(GungeonFlags.ACHIEVEMENT_CONSTRUCT_BULLET))
                this.AchievementUnlock(Achievement.BUILD_BULLET);
            if (GameStatsManager.Instance.GetFlag(GungeonFlags.ACHIEVEMENT_ACCESS_OUBLIETTE))
                this.AchievementUnlock(Achievement.REACH_SEWERS);
            if (GameStatsManager.Instance.GetFlag(GungeonFlags.ACHIEVEMENT_ACCESS_ABBEY))
                this.AchievementUnlock(Achievement.REACH_CATHEDRAL);
            if (GameStatsManager.Instance.GetFlag(GungeonFlags.ACHIEVEMENT_SURPRISE_MIMIC))
                this.AchievementUnlock(Achievement.PREFIRE_ON_MIMIC);
            if (GameStatsManager.Instance.GetFlag(GungeonFlags.ACHIEVEMENT_KILL_JAMMED_BOSS))
                this.AchievementUnlock(Achievement.BEAT_A_JAMMED_BOSS);
            if (GameStatsManager.Instance.GetFlag(GungeonFlags.ACHIEVEMENT_BIGGEST_WALLET))
                this.AchievementUnlock(Achievement.HAVE_MANY_COINS);
            if (GameStatsManager.Instance.GetFlag(GungeonFlags.ACHIEVEMENT_LEAD_GOD))
                this.AchievementUnlock(Achievement.COLLECT_FIVE_MASTERY_TOKENS);
            if (GameStatsManager.Instance.GetFlag(GungeonFlags.BLACKSMITH_RECEIVED_BUSTED_TELEVISION))
                this.AchievementUnlock(Achievement.UNLOCK_ROBOT);
            if (GameStatsManager.Instance.GetFlag(GungeonFlags.SECRET_BULLETMAN_SEEN_05))
                this.AchievementUnlock(Achievement.UNLOCK_BULLET);
            this.m_hasCaughtUpAchievements = true;
        }

        public StringTableManager.GungeonSupportedLanguages GetPreferredLanguage()
        {
            return this.OnGetPreferredLanguage();
        }

        public static void InitializeAlienFXController()
        {
        }

        public static void SetAlienFXAmbientColor(Color32 color)
        {
            if (!PlatformInterface.m_useLightFX)
                return;
            PlatformInterface.m_AlienFXAmbientEffect = new PlatformInterface.LightFXUnit(color, 1f);
        }

        public static void SetAlienFXColor(Color32 color, float duration)
        {
            if (!PlatformInterface.m_useLightFX)
                return;
            PlatformInterface.LightFXUnit lightFxUnit = new PlatformInterface.LightFXUnit(color, duration);
            PlatformInterface.m_AlienFXExtantEffects.Add(lightFxUnit);
        }

        public static void UpdateAlienFXController()
        {
            if (!PlatformInterface.m_useLightFX)
                return;
            if (PlatformInterface.m_AlienFXExtantEffects.Count > 0)
            {
                Color32 color32 = new Color32((byte) 0, (byte) 0, (byte) 0, (byte) 0);
                for (int index = 0; index < PlatformInterface.m_AlienFXExtantEffects.Count; ++index)
                {
                    PlatformInterface.LightFXUnit alienFxExtantEffect = PlatformInterface.m_AlienFXExtantEffects[index];
                    alienFxExtantEffect.remainingDuration -= BraveTime.DeltaTime;
                    if ((double) alienFxExtantEffect.remainingDuration <= 0.0)
                    {
                        PlatformInterface.m_AlienFXExtantEffects.RemoveAt(index);
                        --index;
                    }
                    else
                    {
                        byte num = (byte) Mathf.Lerp(0.0f, (float) alienFxExtantEffect.TargetLightColor.a, alienFxExtantEffect.remainingDuration / alienFxExtantEffect.startDuration);
                        color32.a = (byte) Mathf.Min((int) byte.MaxValue, (int) color32.a + (int) num);
                        color32.r = (byte) Mathf.Min((int) byte.MaxValue, (int) color32.r + (int) alienFxExtantEffect.TargetLightColor.r);
                        color32.g = (byte) Mathf.Min((int) byte.MaxValue, (int) color32.g + (int) alienFxExtantEffect.TargetLightColor.g);
                        color32.b = (byte) Mathf.Min((int) byte.MaxValue, (int) color32.b + (int) alienFxExtantEffect.TargetLightColor.b);
                        PlatformInterface.m_AlienFXExtantEffects[index] = alienFxExtantEffect;
                    }
                }
                float t = (float) color32.a / (float) byte.MaxValue;
                Color targetLightColor = (Color) PlatformInterface.m_AlienFXAmbientEffect.TargetLightColor;
                if ((bool) (UnityEngine.Object) GameManager.Instance.DungeonMusicController && GameManager.Instance.DungeonMusicController.ShouldPulseLightFX)
                {
                    float num = Mathf.Clamp01(Mathf.Lerp(targetLightColor.a, targetLightColor.a - 0.25f, Mathf.PingPong(UnityEngine.Time.realtimeSinceStartup, 5f) / 5f));
                    targetLightColor.a = num;
                }
                color32 = Color32.Lerp((Color32) targetLightColor, color32, t);
                AlienFXInterface._LFX_COLOR c = new AlienFXInterface._LFX_COLOR(color32);
                uint numDevices = 0;
                if (AlienFXInterface.LFX_GetNumDevices(ref numDevices) == 0U)
                {
                    for (uint index = 0; index < numDevices; ++index)
                    {
                        uint numLights = 0;
                        if (AlienFXInterface.LFX_GetNumLights(index, ref numLights) == 0U)
                        {
                            for (uint p2 = 0; p2 < numLights; ++p2)
                            {
                                int num = (int) AlienFXInterface.LFX_SetLightColor(index, p2, ref c);
                            }
                        }
                    }
                }
            }
            else if (PlatformInterface.m_AlienFXAmbientEffect.TargetLightColor.a > (byte) 0)
            {
                Color targetLightColor = (Color) PlatformInterface.m_AlienFXAmbientEffect.TargetLightColor;
                if ((bool) (UnityEngine.Object) GameManager.Instance.DungeonMusicController && GameManager.Instance.DungeonMusicController.ShouldPulseLightFX)
                {
                    float num = Mathf.Clamp01(Mathf.Lerp(targetLightColor.a, 0.0f, Mathf.PingPong(UnityEngine.Time.realtimeSinceStartup, 2f) / 2f));
                    targetLightColor.a = num;
                }
                AlienFXInterface._LFX_COLOR c = new AlienFXInterface._LFX_COLOR((Color32) targetLightColor);
                uint numDevices = 0;
                if (AlienFXInterface.LFX_GetNumDevices(ref numDevices) == 0U)
                {
                    for (uint index = 0; index < numDevices; ++index)
                    {
                        uint numLights = 0;
                        if (AlienFXInterface.LFX_GetNumLights(index, ref numLights) == 0U)
                        {
                            for (uint p2 = 0; p2 < numLights; ++p2)
                            {
                                int num = (int) AlienFXInterface.LFX_SetLightColor(index, p2, ref c);
                            }
                        }
                    }
                }
            }
            else
            {
                int num1 = (int) AlienFXInterface.LFX_Reset();
            }
            int num2 = (int) AlienFXInterface.LFX_Update();
        }

        public static void CleanupAlienFXController()
        {
            if (!PlatformInterface.m_useLightFX)
                return;
            int num = (int) AlienFXInterface.LFX_Release();
        }

        protected void SetGungeonFlagForAchievement(Achievement achievement)
        {
            switch (achievement)
            {
                case Achievement.POPULATE_BREACH:
                    GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_BREACH_POPULATED, true);
                    break;
                case Achievement.PREFIRE_ON_MIMIC:
                    GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_SURPRISE_MIMIC, true);
                    break;
                case Achievement.PUSH_TABLE_INTO_PIT:
                    GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_TABLE_PIT, true);
                    break;
                case Achievement.HAVE_MANY_COINS:
                    GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_BIGGEST_WALLET, true);
                    break;
                case Achievement.STEAL_MULTI:
                    GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_STEAL_THINGS, true);
                    break;
                case Achievement.KILL_WITH_PITS_MULTI:
                    GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_PIT_LORD, true);
                    break;
                case Achievement.BEAT_A_JAMMED_BOSS:
                    GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_KILL_JAMMED_BOSS, true);
                    break;
                case Achievement.FLIP_TABLES_MULTI:
                    GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_TABLES_FLIPPED, true);
                    break;
                case Achievement.COMPLETE_GAME_WITH_BEAST_MODE:
                    GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_BEASTMODE, true);
                    break;
                case Achievement.COMPLETE_GAME_WITH_CHALLENGE_MODE:
                    GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_CHALLENGE_MODE_COMPLETE, true);
                    break;
                default:
                    switch (achievement - 19)
                    {
                        case Achievement.DAT_PLAT:
                            GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_CASTLE_MANYTIMES, true);
                            return;
                        case Achievement.COLLECT_FIVE_MASTERY_TOKENS:
                            GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_GUNGEON_MANYTIMES, true);
                            return;
                        case Achievement.UNLOCK_EVERYTHING:
                            GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_MINES_MANYTIMES, true);
                            return;
                        case Achievement.COMPLETE_AMMONOMICON:
                            GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_HOLLOW_MANYTIMES, true);
                            return;
                        case Achievement.NO_DAMAGE_FLOOR_ONE:
                            GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_FORGE_MANYTIMES, true);
                            return;
                        case Achievement.NO_DAMAGE_FLOOR_TWO:
                            GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_STARTING_GUN, true);
                            return;
                        case Achievement.NO_DAMAGE_FLOOR_FIVE:
                            GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_ACCESS_OUBLIETTE, true);
                            return;
                        case Achievement.SPEND_META_CURRENCY:
                            GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_ACCESS_ABBEY, true);
                            return;
                        default:
                            switch (achievement - 10)
                            {
                                case Achievement.DAT_PLAT:
                                    GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_GUNGAME_COMPLETE, true);
                                    return;
                                case Achievement.UNLOCK_EVERYTHING:
                                    GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_CONSTRUCT_BULLET, true);
                                    return;
                                case Achievement.COMPLETE_AMMONOMICON:
                                    GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_COMPLETE_FOUR_PASTS, true);
                                    return;
                                default:
                                    if (achievement != Achievement.COLLECT_FIVE_MASTERY_TOKENS)
                                        return;
                                    GameStatsManager.Instance.SetFlag(GungeonFlags.ACHIEVEMENT_LEAD_GOD, true);
                                    return;
                            }
                    }
            }
        }

        protected abstract void OnStart();

        protected abstract void OnAchievementUnlock(Achievement achievement, int playerIndex);

        protected abstract void OnLateUpdate();

        protected abstract StringTableManager.GungeonSupportedLanguages OnGetPreferredLanguage();

        private void GalaxyMtxGunHack()
        {
            if (this.m_hasCheckedForGalaxyMtx)
                return;
            this.m_hasCheckedForGalaxyMtx = true;
            if (PlatformInterfaceSteam.IsSteamBuild())
                return;
            string path = Path.Combine(Application.dataPath, "../Unlock MTX Gun.dat");
            if (path == null || !File.Exists(path))
                return;
            byte[] numArray = File.ReadAllBytes(path);
            byte[] byteArray = PlatformInterface.StringToByteArray("e226 87d5 f590 279d 38f5 fe7b 07fe cdf5 41c8 1c7d 257f 6ad5 d293 985e 994e 3032 c91d 8d6e 5697 5abb 8ee6 15ab 9afc 12e2 f8cf d5dd 8339 f987 6bcb ba0e 6280 1386 2881 c560 5980 457f c52f 1378 18ad f5da c8ec a283 f32e 8e78 0970 ea11 213a ed71 66d2 6ab2 7124 2c4e 6778 0e61 ada5 f225 e921 6326 2126 cd37 183b db48 3110 c14b 3358 c772 fbce a89b bde0 5ba9 6458 3acf 9307 2496 3be6 825d 1d75 84db 379e c360 7da9 0342 1042 7f5f 89ba 77e3 e74c 1195 f896 ff9a b1db 1350 2dce b368 7884 d5ad 5e6e 5957 fe74 1980 fabe 0e90 bf57 e29d 0239 0355 8ca7 d212 450b c426 10c2 7098 63a6 769b e827 d5e0 0d65 d6d7 fb3c e531 d0e8 bf83 5d2a bc83 388d 4b8f 8a22 b424");
            if (numArray.Length != byteArray.Length)
                return;
            for (int index = 0; index < numArray.Length; ++index)
            {
                if ((int) numArray[index] != (int) byteArray[index])
                    return;
            }
            if (this.UnlockedDlc.Contains(PlatformDlc.EARLY_MTX_GUN))
                return;
            this.UnlockedDlc.Add(PlatformDlc.EARLY_MTX_GUN);
        }

        public static byte[] StringToByteArray(string hex)
        {
            hex = hex.Replace(" ", string.Empty);
            return Enumerable.Range(0, hex.Length).Where<int>((Func<int, bool>) (x => x % 2 == 0)).Select<int, byte>((Func<int, byte>) (x => Convert.ToByte(hex.Substring(x, 2), 16))).ToArray<byte>();
        }

        public static string GetTrackedStatEventString(TrackedStats stat)
        {
            string trackedStatEventString = string.Empty;
            switch (stat)
            {
                case TrackedStats.NUMBER_DEATHS:
                    trackedStatEventString = "Deaths";
                    break;
                case TrackedStats.ENEMIES_KILLED:
                    trackedStatEventString = "EnemiesKilled";
                    break;
                case TrackedStats.TIMES_KILLED_PAST:
                    trackedStatEventString = "PastsKilled";
                    break;
                case TrackedStats.TABLES_FLIPPED:
                    trackedStatEventString = "TablesFlipped";
                    break;
            }
            return trackedStatEventString;
        }

private struct LightFXUnit
        {
            public Color32 TargetLightColor;
            public float remainingDuration;
            public float startDuration;

            public LightFXUnit(Color32 sourceColor, float sourceDuration)
            {
                TargetLightColor = sourceColor;
                remainingDuration = sourceDuration;
                startDuration = sourceDuration;
            }
        }

        private class DlcUnlockedItem
        {
            public PlatformDlc PlatformDlc;
            public string encounterGuid;
            public GungeonFlags flag;

            public DlcUnlockedItem(PlatformDlc PlatformDlc, string encounterGuid, GungeonFlags flag = GungeonFlags.NONE)
            {
                this.PlatformDlc = PlatformDlc;
                this.encounterGuid = encounterGuid;
                this.flag = flag;
            }
        }
    }

