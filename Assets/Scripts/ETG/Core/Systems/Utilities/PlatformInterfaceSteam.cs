using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

#nullable disable

public class PlatformInterfaceSteam : PlatformInterface
  {
    private CGameID m_GameID;
    private bool m_bRequestedStats;
    private bool m_bStatsValid;
    private bool m_bStoreStats;
    protected Callback<UserStatsReceived_t> m_UserStatsReceived;
    protected Callback<UserStatsStored_t> m_UserStatsStored;
    protected Callback<UserAchievementStored_t> m_UserAchievementStored;
    protected Callback<DlcInstalled_t> m_DLCInstalled;
    private PlatformInterfaceSteam.AchievementData[] m_achievements = new PlatformInterfaceSteam.AchievementData[54]
    {
      new PlatformInterfaceSteam.AchievementData(Achievement.COLLECT_FIVE_MASTERY_TOKENS, "Lead God", "Collect five Master Rounds in one run"),
      new PlatformInterfaceSteam.AchievementData(Achievement.SPEND_META_CURRENCY, "Patron", "Spend big at the Acquisitions Department", PlatformStat.META_SPENT_AT_STORE, 100, new int[3]
      {
        25,
        50,
        75
      }),
      new PlatformInterfaceSteam.AchievementData(Achievement.COMPLETE_GAME_WITH_ENCHANTED_GUN, "Gun Game", "Complete the game with the Sorceress's Enchanted Gun"),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_FLOOR_SIX, "Gungeon Master", "Clear the Sixth Chamber"),
      new PlatformInterfaceSteam.AchievementData(Achievement.BUILD_BULLET, "Gunsmith", "Construct the Bullet that can kill the Past"),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_PAST_ALL, "Historian", "Complete all 4 main character Pasts"),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_PAST_ROGUE, "Wingman", "Kill the Pilot's Past"),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_PAST_CONVICT, "Double Jeopardy", "Kill the Convict's Past"),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_PAST_MARINE, "Squad Captain", "Kill the Marine's Past"),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_PAST_GUIDE, "Deadliest Game", "Kill the Hunter's Past"),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_FLOOR_FIVE, "Slayer", "Defeat the Boss of the Fifth Chamber"),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_FLOOR_ONE_MULTI, "Castle Crasher", "Clear the First Chamber 50 Times", PlatformStat.FLOOR_ONE_CLEARS, 50, new int[1]
      {
        25
      }),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_FLOOR_TWO_MULTI, "Dungeon Diver", "Clear the Second Chamber 40 Times", PlatformStat.FLOOR_TWO_CLEARS, 40, new int[1]
      {
        20
      }),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_FLOOR_THREE_MULTI, "Mine Master", "Clear the Third Chamber 30 Times", PlatformStat.FLOOR_THREE_CLEARS, 30, new int[1]
      {
        15
      }),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_FLOOR_FOUR_MULTI, "Hollowed Out", "Clear the Fourth Chamber 20 Times", PlatformStat.FLOOR_FOUR_CLEARS, 20, new int[1]
      {
        10
      }),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_FLOOR_FIVE_MULTI, "Forger", "Clear the Fifth Chamber 10 Times", PlatformStat.FLOOR_FIVE_CLEARS, 10, new int[1]
      {
        5
      }),
      new PlatformInterfaceSteam.AchievementData(Achievement.HAVE_MANY_COINS, "Biggest Wallet", "Carry a large amount of money at once"),
      new PlatformInterfaceSteam.AchievementData(Achievement.MAP_MAIN_FLOORS, "Cartographer's Assistant", "Map the first Five Chambers for the lost adventurer", PlatformStat.MAIN_FLOORS_MAPPED, 5, new int[4]
      {
        1,
        2,
        3,
        4
      }),
      new PlatformInterfaceSteam.AchievementData(Achievement.REACH_SEWERS, "Grate Hall", "Access the Oubliette"),
      new PlatformInterfaceSteam.AchievementData(Achievement.REACH_CATHEDRAL, "Reverence for the Dead", "Access the Temple"),
      new PlatformInterfaceSteam.AchievementData(Achievement.COMPLETE_GOLEM_ARM, "Re-Armed", "Deliver the Golem's replacement arm"),
      new PlatformInterfaceSteam.AchievementData(Achievement.COMPLETE_FRIFLE_MULTI, "Weird Tale", "Complete Frifle's Challenges", PlatformStat.FRIFLE_CORE_COMPLETED, 15, new int[14]
      {
        1,
        2,
        3,
        4,
        5,
        6,
        7,
        8,
        9,
        10,
        11,
        12,
        13,
        14
      }),
      new PlatformInterfaceSteam.AchievementData(Achievement.ACE_WINCHESTER_MULTI, "Trickshot", "Ace Winchester's game 3 times", PlatformStat.WINCHESTER_ACED, 3, new int[2]
      {
        1,
        2
      }),
      new PlatformInterfaceSteam.AchievementData(Achievement.COMPLETE_GUNSLING_MULTI, "Hedge Slinger", "Win a wager against the Gunsling King 5 times", PlatformStat.GUNSLING_COMPLETED, 5, new int[4]
      {
        1,
        2,
        3,
        4
      }),
      new PlatformInterfaceSteam.AchievementData(Achievement.UNLOCK_BULLET, "Case Closed", "Unlock the Bullet"),
      new PlatformInterfaceSteam.AchievementData(Achievement.UNLOCK_ROBOT, "Beep", "Unlock the Robot"),
      new PlatformInterfaceSteam.AchievementData(Achievement.UNLOCK_FLOOR_TWO_SHORTCUT, "Going Down", "Open the shortcut to the Second Chamber"),
      new PlatformInterfaceSteam.AchievementData(Achievement.UNLOCK_FLOOR_THREE_SHORTCUT, "Going Downer", "Open the shortcut to the Third Chamber"),
      new PlatformInterfaceSteam.AchievementData(Achievement.UNLOCK_FLOOR_FOUR_SHORTCUT, "Going Downest", "Open the shortcut to the Fourth Chamber"),
      new PlatformInterfaceSteam.AchievementData(Achievement.UNLOCK_FLOOR_FIVE_SHORTCUT, "Last Stop", "Open the shortcut to the Fifth Chamber"),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_MANFREDS_RIVAL, "Sworn Gun", "Avenge Manuel"),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_TUTORIAL, "Gungeon Acolyte", "Complete the Tutorial"),
      new PlatformInterfaceSteam.AchievementData(Achievement.POPULATE_BREACH, "Great Hall", "Populate the Breach", PlatformStat.BREACH_POPULATION, 12, new int[3]
      {
        3,
        6,
        9
      }),
      new PlatformInterfaceSteam.AchievementData(Achievement.PREFIRE_ON_MIMIC, "Not Just A Box", "Get the jump on a Mimic"),
      new PlatformInterfaceSteam.AchievementData(Achievement.KILL_FROZEN_ENEMY_WITH_ROLL, "Demolition Man", "Kill a frozen enemy by rolling into it"),
      new PlatformInterfaceSteam.AchievementData(Achievement.PUSH_TABLE_INTO_PIT, "I Knew Someone Would Do It", "Why"),
      new PlatformInterfaceSteam.AchievementData(Achievement.STEAL_MULTI, "Woodsie Lord", "Steal 10 things", PlatformStat.ITEMS_STOLEN, 10, new int[1]
      {
        5
      }),
      new PlatformInterfaceSteam.AchievementData(Achievement.KILL_BOSS_WITH_GLITTER, "Day Ruiner", "Kill a boss after covering it with glitter"),
      new PlatformInterfaceSteam.AchievementData(Achievement.FALL_IN_END_TIMES, "Lion Leap", "Fall at the last second"),
      new PlatformInterfaceSteam.AchievementData(Achievement.KILL_WITH_CHANDELIER_MULTI, "Money Pit", "Kill 100 enemies by dropping chandeliers", PlatformStat.CHANDELIER_KILLS, 100, new int[3]
      {
        25,
        50,
        75
      }),
      new PlatformInterfaceSteam.AchievementData(Achievement.KILL_IN_MINE_CART_MULTI, "Rider", "Kill 100 enemies while riding in a mine cart", PlatformStat.MINECART_KILLS, 100, new int[3]
      {
        25,
        50,
        75
      }),
      new PlatformInterfaceSteam.AchievementData(Achievement.KILL_WITH_PITS_MULTI, "Pit Lord", "Kill 100 enemies by knocking them into pits", PlatformStat.PIT_KILLS, 100, new int[3]
      {
        25,
        50,
        75
      }),
      new PlatformInterfaceSteam.AchievementData(Achievement.DIE_IN_PAST, "Time Paradox", "Die in the Past"),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_A_JAMMED_BOSS, "Exorcist", "Kill a Jammed Boss"),
      new PlatformInterfaceSteam.AchievementData(Achievement.REACH_BLACK_MARKET, "The Password", "Accessed the Hidden Market"),
      new PlatformInterfaceSteam.AchievementData(Achievement.HAVE_MAX_CURSE, "Jammed", "You've met with a terrible fate, haven't you"),
      new PlatformInterfaceSteam.AchievementData(Achievement.FLIP_TABLES_MULTI, "Rage Mode", "Always be flipping. Guns are for flippers", PlatformStat.TABLES_FLIPPED, 500, new int[4]
      {
        100,
        200,
        300,
        400
      }),
      new PlatformInterfaceSteam.AchievementData(Achievement.COMPLETE_GAME_WITH_BEAST_MODE, "Beast Master", "Complete the game with Beast Mode on"),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_PAST_ROBOT, "Terminated", "Kill the Robot's Past"),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_PAST_BULLET, "Hero of Gun", "Kill the Bullet's Past"),
      new PlatformInterfaceSteam.AchievementData(Achievement.COMPLETE_GAME_WITH_CHALLENGE_MODE, "Challenger", "Complete Daisuke's trial"),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_ADVANCED_DRAGUN, "Advanced Slayer", "Defeat an Advanced Boss"),
      new PlatformInterfaceSteam.AchievementData(Achievement.BEAT_METAL_GEAR_RAT, "Resourceful", "Take Revenge"),
      new PlatformInterfaceSteam.AchievementData(Achievement.COMPLETE_GAME_WITH_TURBO_MODE, "Sledge-Dog", "Complete Tonic's Challenge")
    };
    private PlatformInterfaceSteam.StatData[] m_stats = new PlatformInterfaceSteam.StatData[16 /*0x10*/]
    {
      new PlatformInterfaceSteam.StatData(PlatformStat.META_SPENT_AT_STORE),
      new PlatformInterfaceSteam.StatData(PlatformStat.FLOOR_ONE_CLEARS),
      new PlatformInterfaceSteam.StatData(PlatformStat.FLOOR_TWO_CLEARS),
      new PlatformInterfaceSteam.StatData(PlatformStat.FLOOR_THREE_CLEARS),
      new PlatformInterfaceSteam.StatData(PlatformStat.FLOOR_FOUR_CLEARS),
      new PlatformInterfaceSteam.StatData(PlatformStat.FLOOR_FIVE_CLEARS),
      new PlatformInterfaceSteam.StatData(PlatformStat.MAIN_FLOORS_MAPPED),
      new PlatformInterfaceSteam.StatData(PlatformStat.FRIFLE_CORE_COMPLETED),
      new PlatformInterfaceSteam.StatData(PlatformStat.WINCHESTER_ACED),
      new PlatformInterfaceSteam.StatData(PlatformStat.GUNSLING_COMPLETED),
      new PlatformInterfaceSteam.StatData(PlatformStat.BREACH_POPULATION),
      new PlatformInterfaceSteam.StatData(PlatformStat.ITEMS_STOLEN),
      new PlatformInterfaceSteam.StatData(PlatformStat.CHANDELIER_KILLS),
      new PlatformInterfaceSteam.StatData(PlatformStat.MINECART_KILLS),
      new PlatformInterfaceSteam.StatData(PlatformStat.PIT_KILLS),
      new PlatformInterfaceSteam.StatData(PlatformStat.TABLES_FLIPPED)
    };
    private PlatformInterfaceSteam.DlcData[] m_dlc = new PlatformInterfaceSteam.DlcData[2]
    {
      new PlatformInterfaceSteam.DlcData(PlatformDlc.EARLY_MTX_GUN, (AppId_t) 457842U),
      new PlatformInterfaceSteam.DlcData(PlatformDlc.EARLY_COBALT_HAMMER, (AppId_t) 457843U)
    };
    private readonly Dictionary<string, StringTableManager.GungeonSupportedLanguages> SteamDefaultLanguageToGungeonLanguage = new Dictionary<string, StringTableManager.GungeonSupportedLanguages>()
    {
      {
        "english",
        StringTableManager.GungeonSupportedLanguages.ENGLISH
      },
      {
        "french",
        StringTableManager.GungeonSupportedLanguages.FRENCH
      },
      {
        "german",
        StringTableManager.GungeonSupportedLanguages.GERMAN
      },
      {
        "italian",
        StringTableManager.GungeonSupportedLanguages.ITALIAN
      },
      {
        "japanese",
        StringTableManager.GungeonSupportedLanguages.JAPANESE
      },
      {
        "korean",
        StringTableManager.GungeonSupportedLanguages.KOREAN
      },
      {
        "portuguese",
        StringTableManager.GungeonSupportedLanguages.BRAZILIANPORTUGUESE
      },
      {
        "brazilian",
        StringTableManager.GungeonSupportedLanguages.BRAZILIANPORTUGUESE
      },
      {
        "spanish",
        StringTableManager.GungeonSupportedLanguages.SPANISH
      },
      {
        "russian",
        StringTableManager.GungeonSupportedLanguages.RUSSIAN
      },
      {
        "polish",
        StringTableManager.GungeonSupportedLanguages.POLISH
      },
      {
        "chinese",
        StringTableManager.GungeonSupportedLanguages.CHINESE
      }
    };

    public static bool IsSteamBuild()
    {
      return File.Exists(Path.Combine(Application.dataPath, "../steam_api64.dll")) || File.Exists(Path.Combine(Application.dataPath, "../steam_api.dll"));
    }

    protected override void OnStart()
    {
      Debug.Log((object) "Starting Steam platform interface.");
      if (!SteamManager.Initialized)
        return;
      this.m_GameID = new CGameID(SteamUtils.GetAppID());
      this.UnlockedDlc.Clear();
      this.m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(new Callback<UserStatsReceived_t>.DispatchDelegate(this.OnUserStatsReceived));
      this.m_UserStatsStored = Callback<UserStatsStored_t>.Create(new Callback<UserStatsStored_t>.DispatchDelegate(this.OnUserStatsStored));
      this.m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(new Callback<UserAchievementStored_t>.DispatchDelegate(this.OnAchievementStored));
      this.m_DLCInstalled = Callback<DlcInstalled_t>.Create(new Callback<DlcInstalled_t>.DispatchDelegate(this.OnDlcInstalled));
      for (int index = 0; index < this.m_dlc.Length; ++index)
      {
        if (SteamApps.BIsDlcInstalled(this.m_dlc[index].appId))
          this.UnlockedDlc.Add(this.m_dlc[index].dlc);
      }
      this.m_bRequestedStats = false;
      this.m_bStatsValid = false;
    }

    protected override void OnAchievementUnlock(Achievement achievement, int playerIndex)
    {
      if (!SteamManager.Initialized)
        return;
      switch (achievement)
      {
        case Achievement.SPEND_META_CURRENCY:
          this.SetStat(PlatformStat.META_SPENT_AT_STORE, 100);
          break;
        case Achievement.BEAT_FLOOR_ONE_MULTI:
          break;
        case Achievement.BEAT_FLOOR_TWO_MULTI:
          break;
        case Achievement.BEAT_FLOOR_THREE_MULTI:
          break;
        case Achievement.BEAT_FLOOR_FOUR_MULTI:
          break;
        case Achievement.BEAT_FLOOR_FIVE_MULTI:
          break;
        case Achievement.MAP_MAIN_FLOORS:
          break;
        case Achievement.COMPLETE_FRIFLE_MULTI:
          this.SetStat(PlatformStat.FRIFLE_CORE_COMPLETED, 15);
          break;
        case Achievement.ACE_WINCHESTER_MULTI:
          break;
        case Achievement.COMPLETE_GUNSLING_MULTI:
          break;
        case Achievement.POPULATE_BREACH:
          break;
        case Achievement.STEAL_MULTI:
          break;
        case Achievement.KILL_WITH_CHANDELIER_MULTI:
          break;
        case Achievement.KILL_IN_MINE_CART_MULTI:
          break;
        case Achievement.KILL_WITH_PITS_MULTI:
          break;
        case Achievement.FLIP_TABLES_MULTI:
          break;
        default:
          PlatformInterfaceSteam.AchievementData achievementData = (PlatformInterfaceSteam.AchievementData) null;
          for (int index = 0; index < this.m_achievements.Length; ++index)
          {
            if (this.m_achievements[index].achievement == achievement)
              achievementData = this.m_achievements[index];
          }
          if (achievementData == null)
            break;
          achievementData.isUnlocked = true;
          SteamUserStats.SetAchievement(achievementData.ApiKey);
          this.m_bStoreStats = true;
          break;
      }
    }

    public override bool IsAchievementUnlocked(Achievement achievement)
    {
      PlatformInterfaceSteam.AchievementData achievementData = (PlatformInterfaceSteam.AchievementData) null;
      for (int index = 0; index < this.m_achievements.Length; ++index)
      {
        if (this.m_achievements[index].achievement == achievement)
          achievementData = this.m_achievements[index];
      }
      return achievementData != null && achievementData.isUnlocked;
    }

    public override void SetStat(PlatformStat stat, int value)
    {
      if (!SteamManager.Initialized)
        return;
      PlatformInterfaceSteam.StatData statData = (PlatformInterfaceSteam.StatData) null;
      for (int index = 0; index < this.m_stats.Length; ++index)
      {
        if (this.m_stats[index].stat == stat)
          statData = this.m_stats[index];
      }
      if (statData == null)
        return;
      int prevValue = statData.value;
      statData.value = value;
      SteamUserStats.SetStat(statData.ApiKey, value);
      this.MaybeShowProgress(stat, prevValue, value);
      this.MaybeStoreStats(stat, prevValue, value);
    }

    public override void IncrementStat(PlatformStat stat, int delta)
    {
      if (!SteamManager.Initialized)
        return;
      PlatformInterfaceSteam.StatData statData = (PlatformInterfaceSteam.StatData) null;
      for (int index = 0; index < this.m_stats.Length; ++index)
      {
        if (this.m_stats[index].stat == stat)
          statData = this.m_stats[index];
      }
      if (statData == null)
        return;
      int prevValue = statData.value;
      statData.value = prevValue + delta;
      SteamUserStats.SetStat(statData.ApiKey, statData.value);
      this.MaybeShowProgress(stat, prevValue, statData.value);
      this.MaybeStoreStats(stat, prevValue, statData.value);
    }

    private void MaybeShowProgress(PlatformStat stat, int prevValue, int newValue)
    {
      for (int index1 = 0; index1 < this.m_achievements.Length; ++index1)
      {
        PlatformInterfaceSteam.AchievementData achievement = this.m_achievements[index1];
        if (achievement.hasProgressStat && achievement.progressStat == stat && achievement.subgoals != null && newValue < achievement.goalValue)
        {
          if (achievement.progressStat == PlatformStat.BREACH_POPULATION && prevValue == 0 && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
            break;
          for (int index2 = achievement.subgoals.Length - 1; index2 >= 0; --index2)
          {
            int subgoal = achievement.subgoals[index2];
            if (prevValue < subgoal && newValue >= subgoal)
            {
              SteamUserStats.IndicateAchievementProgress(achievement.ApiKey, (uint) newValue, (uint) achievement.goalValue);
              return;
            }
          }
        }
      }
    }

    private void MaybeStoreStats(PlatformStat stat, int prevValue, int newValue)
    {
      if ((stat != PlatformStat.META_SPENT_AT_STORE || prevValue >= 100 || newValue < 100) && (stat != PlatformStat.FLOOR_ONE_CLEARS || prevValue >= 50 || newValue < 50) && (stat != PlatformStat.FLOOR_TWO_CLEARS || prevValue >= 40 || newValue < 40) && (stat != PlatformStat.FLOOR_THREE_CLEARS || prevValue >= 30 || newValue < 30) && (stat != PlatformStat.FLOOR_FOUR_CLEARS || prevValue >= 20 || newValue < 20) && (stat != PlatformStat.FLOOR_FIVE_CLEARS || prevValue >= 10 || newValue < 10) && stat != PlatformStat.MAIN_FLOORS_MAPPED && stat != PlatformStat.FRIFLE_CORE_COMPLETED && stat != PlatformStat.WINCHESTER_ACED && stat != PlatformStat.GUNSLING_COMPLETED && stat != PlatformStat.BREACH_POPULATION && stat != PlatformStat.ITEMS_STOLEN && (stat != PlatformStat.CHANDELIER_KILLS || prevValue >= 100 || newValue < 100) && (stat != PlatformStat.MINECART_KILLS || prevValue >= 100 || newValue < 100) && (stat != PlatformStat.PIT_KILLS || prevValue >= 100 || newValue < 100) && (stat != PlatformStat.TABLES_FLIPPED || prevValue >= 500 || newValue < 500))
        return;
      this.m_bStoreStats = true;
    }

    public override void StoreStats() => this.m_bStoreStats = true;

    public override void ResetStats(bool achievementsToo)
    {
      SteamUserStats.ResetAllStats(achievementsToo);
    }

    protected override void OnLateUpdate()
    {
      if (!SteamManager.Initialized)
        return;
      if (!this.m_bRequestedStats)
      {
        if (!SteamManager.Initialized)
        {
          this.m_bRequestedStats = true;
          return;
        }
        this.m_bRequestedStats = SteamUserStats.RequestCurrentStats();
      }
      if (!this.m_bStatsValid || !this.m_bStoreStats)
        return;
      this.m_bStoreStats = !SteamUserStats.StoreStats();
    }

    protected override StringTableManager.GungeonSupportedLanguages OnGetPreferredLanguage()
    {
      string steamUiLanguage = SteamUtils.GetSteamUILanguage();
      return this.SteamDefaultLanguageToGungeonLanguage.ContainsKey(steamUiLanguage) ? this.SteamDefaultLanguageToGungeonLanguage[steamUiLanguage] : StringTableManager.GungeonSupportedLanguages.ENGLISH;
    }

    private void OnUserStatsReceived(UserStatsReceived_t pCallback)
    {
      if (!SteamManager.Initialized || (long) (ulong) this.m_GameID != (long) pCallback.m_nGameID)
        return;
      if (pCallback.m_eResult == EResult.k_EResultOK)
      {
        Debug.Log((object) "Received stats and achievements from Steam\n");
        this.m_bStatsValid = true;
        foreach (PlatformInterfaceSteam.AchievementData achievement in this.m_achievements)
        {
          if (SteamUserStats.GetAchievement(achievement.ApiKey, out achievement.isUnlocked))
          {
            achievement.name = SteamUserStats.GetAchievementDisplayAttribute(achievement.ApiKey, "name");
            achievement.description = SteamUserStats.GetAchievementDisplayAttribute(achievement.ApiKey, "desc");
          }
          else
            Debug.LogWarning((object) $"SteamUserStats.GetAchievement failed for Achievement {(object) achievement.achievement}\nIs it registered in the Steam Partner site?");
        }
        foreach (PlatformInterfaceSteam.StatData stat in this.m_stats)
        {
          if (!SteamUserStats.GetStat(stat.ApiKey, out stat.value))
            Debug.LogWarning((object) $"SteamUserStats.GetStat failed for Stat {(object) stat.stat}\nIs it registered in the Steam Partner site?");
        }
        this.CatchupAchievements();
      }
      else
        Debug.Log((object) ("RequestStats - failed, " + (object) pCallback.m_eResult));
    }

    private void OnUserStatsStored(UserStatsStored_t pCallback)
    {
      if ((long) (ulong) this.m_GameID != (long) pCallback.m_nGameID)
        return;
      if (pCallback.m_eResult == EResult.k_EResultOK)
        Debug.Log((object) "StoreStats - success");
      else if (pCallback.m_eResult == EResult.k_EResultInvalidParam)
      {
        Debug.Log((object) "StoreStats - some failed to validate");
        this.OnUserStatsReceived(new UserStatsReceived_t()
        {
          m_eResult = EResult.k_EResultOK,
          m_nGameID = (ulong) this.m_GameID
        });
      }
      else
        Debug.Log((object) ("StoreStats - failed, " + (object) pCallback.m_eResult));
    }

    private void OnAchievementStored(UserAchievementStored_t pCallback)
    {
      if ((long) (ulong) this.m_GameID != (long) pCallback.m_nGameID)
        return;
      if (pCallback.m_nMaxProgress == 0U)
        Debug.Log((object) $"Achievement '{pCallback.m_rgchAchievementName}' unlocked!");
      else
        Debug.Log((object) $"Achievement '{pCallback.m_rgchAchievementName}' progress callback, ({(object) pCallback.m_nCurProgress},{(object) pCallback.m_nMaxProgress})");
    }

    private void OnDlcInstalled(DlcInstalled_t pCallback)
    {
      for (int index = 0; index < this.m_dlc.Length; ++index)
      {
        if (this.m_dlc[index].appId == pCallback.m_nAppID)
          this.UnlockedDlc.Add(this.m_dlc[index].dlc);
      }
    }

    public void DebugPrintAchievements()
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < this.m_achievements.Length; ++index)
      {
        PlatformInterfaceSteam.AchievementData achievementData = this.m_achievements[index];
        stringBuilder.AppendFormat("[{0}] {1}\n", !achievementData.isUnlocked ? (object) " " : (object) "X", (object) achievementData.name);
        stringBuilder.AppendFormat("{0}\n", (object) achievementData.description);
        if (achievementData.hasProgressStat)
        {
          PlatformInterfaceSteam.StatData statData = Array.Find<PlatformInterfaceSteam.StatData>(this.m_stats, (Predicate<PlatformInterfaceSteam.StatData>) (s => s.stat == achievementData.progressStat));
          stringBuilder.AppendFormat("{0} of {1}\n", (object) statData.value, (object) achievementData.goalValue);
        }
        stringBuilder.AppendLine();
      }
      Debug.Log((object) stringBuilder.ToString());
    }

    private class AchievementData
    {
      public Achievement achievement;
      public string name;
      public string description;
      public bool isUnlocked;
      public bool hasProgressStat;
      public PlatformStat progressStat;
      public int goalValue;
      public int[] subgoals;
      private string m_cachedApiKey;

      public AchievementData(Achievement achievement, string name, string desc)
      {
        this.achievement = achievement;
        this.name = name;
        this.description = desc;
        this.isUnlocked = false;
      }

      public AchievementData(
        Achievement achievement,
        string name,
        string desc,
        PlatformStat progressStat,
        int goalValue,
        params int[] subgoals)
      {
        this.achievement = achievement;
        this.name = name;
        this.description = desc;
        this.isUnlocked = false;
        this.hasProgressStat = true;
        this.progressStat = progressStat;
        this.goalValue = goalValue;
        this.subgoals = subgoals;
      }

      public string ApiKey
      {
        get
        {
          if (this.m_cachedApiKey == null)
            this.m_cachedApiKey = this.achievement.ToString();
          return this.m_cachedApiKey;
        }
      }
    }

    private class StatData
    {
      public PlatformStat stat;
      public int value;
      private string m_cachedApiKey;

      public StatData(PlatformStat stat) => this.stat = stat;

      public string ApiKey
      {
        get
        {
          if (this.m_cachedApiKey == null)
            this.m_cachedApiKey = this.stat.ToString();
          return this.m_cachedApiKey;
        }
      }
    }

    private class DlcData
    {
      public PlatformDlc dlc;
      public AppId_t appId;

      public DlcData(PlatformDlc dlc, AppId_t appId)
      {
        this.dlc = dlc;
        this.appId = appId;
      }
    }
  }

