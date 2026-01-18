using Galaxy.Api;
using System;
using System.IO;
using System.Text;
using UnityEngine;

#nullable disable

public class PlatformInterfaceGalaxy : PlatformInterface
  {
    private bool m_isInitialized;
    private bool m_bRequestedStats;
    private bool m_bStatsValid;
    private bool m_bStoreStats;
    private PlatformInterfaceGalaxy.AchievementsReceivedListener m_achievementsReceivedListener;
    private PlatformInterfaceGalaxy.StatsStoredListener m_storeStatsCallback;
    private PlatformInterfaceGalaxy.AchievementData[] m_achievements = new PlatformInterfaceGalaxy.AchievementData[54]
    {
      new PlatformInterfaceGalaxy.AchievementData(Achievement.COLLECT_FIVE_MASTERY_TOKENS, "Lead God", "Collect five Master Rounds in one run"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.SPEND_META_CURRENCY, "Patron", "Spend big at the Acquisitions Department", PlatformStat.META_SPENT_AT_STORE, 100),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.COMPLETE_GAME_WITH_ENCHANTED_GUN, "Gun Game", "Complete the game with the Sorceress's Enchanted Gun"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_FLOOR_SIX, "Gungeon Master", "Clear the Sixth Chamber"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BUILD_BULLET, "Gunsmith", "Construct the Bullet that can kill the Past"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_PAST_ALL, "Historian", "Complete all 4 main character Pasts"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_PAST_ROGUE, "Wingman", "Kill the Pilot's Past"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_PAST_CONVICT, "Double Jeopardy", "Kill the Convict's Past"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_PAST_MARINE, "Squad Captain", "Kill the Marine's Past"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_PAST_GUIDE, "Deadliest Game", "Kill the Hunter's Past"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_FLOOR_FIVE, "Slayer", "Defeat the Boss of the Fifth Chamber"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_FLOOR_ONE_MULTI, "Castle Crasher", "Clear the First Chamber 50 Times", PlatformStat.FLOOR_ONE_CLEARS, 50),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_FLOOR_TWO_MULTI, "Dungeon Diver", "Clear the Second Chamber 40 Times", PlatformStat.FLOOR_TWO_CLEARS, 40),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_FLOOR_THREE_MULTI, "Mine Master", "Clear the Third Chamber 30 Times", PlatformStat.FLOOR_THREE_CLEARS, 30),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_FLOOR_FOUR_MULTI, "Hollowed Out", "Clear the Fourth Chamber 20 Times", PlatformStat.FLOOR_FOUR_CLEARS, 20),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_FLOOR_FIVE_MULTI, "Forger", "Clear the Fifth Chamber 10 Times", PlatformStat.FLOOR_FIVE_CLEARS, 10),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.HAVE_MANY_COINS, "Biggest Wallet", "Carry a large amount of money at once"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.MAP_MAIN_FLOORS, "Cartographer's Assistant", "Map the first Five Chambers for the lost adventurer", PlatformStat.MAIN_FLOORS_MAPPED, 5),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.REACH_SEWERS, "Grate Hall", "Access the Oubliette"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.REACH_CATHEDRAL, "Reverence for the Dead", "Access the Temple"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.COMPLETE_GOLEM_ARM, "Re-Armed", "Deliver the Golem's replacement arm"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.COMPLETE_FRIFLE_MULTI, "Weird Tale", "Complete Frifle's Challenges", PlatformStat.FRIFLE_CORE_COMPLETED, 15),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.ACE_WINCHESTER_MULTI, "Trickshot", "Ace Winchester's game 3 times", PlatformStat.WINCHESTER_ACED, 3),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.COMPLETE_GUNSLING_MULTI, "Hedge Slinger", "Win a wager against the Gunsling King 5 times", PlatformStat.GUNSLING_COMPLETED, 5),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.UNLOCK_BULLET, "Case Closed", "Unlock the Bullet"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.UNLOCK_ROBOT, "Beep", "Unlock the Robot"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.UNLOCK_FLOOR_TWO_SHORTCUT, "Going Down", "Open the shortcut to the Second Chamber"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.UNLOCK_FLOOR_THREE_SHORTCUT, "Going Downer", "Open the shortcut to the Third Chamber"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.UNLOCK_FLOOR_FOUR_SHORTCUT, "Going Downest", "Open the shortcut to the Fourth Chamber"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.UNLOCK_FLOOR_FIVE_SHORTCUT, "Last Stop", "Open the shortcut to the Fifth Chamber"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_MANFREDS_RIVAL, "Sworn Gun", "Avenge Manuel"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_TUTORIAL, "Gungeon Acolyte", "Complete the Tutorial"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.POPULATE_BREACH, "Great Hall", "Populate the Breach", PlatformStat.BREACH_POPULATION, 12),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.PREFIRE_ON_MIMIC, "Not Just A Box", "Get the jump on a Mimic"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.KILL_FROZEN_ENEMY_WITH_ROLL, "Demolition Man", "Kill a frozen enemy by rolling into it"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.PUSH_TABLE_INTO_PIT, "I Knew Someone Would Do It", "Why"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.STEAL_MULTI, "Woodsie Lord", "Steal 10 things", PlatformStat.ITEMS_STOLEN, 10),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.KILL_BOSS_WITH_GLITTER, "Day Ruiner", "Kill a boss after covering it with glitter"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.FALL_IN_END_TIMES, "Lion Leap", "Fall at the last second"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.KILL_WITH_CHANDELIER_MULTI, "Money Pit", "Kill 100 enemies by dropping chandeliers", PlatformStat.CHANDELIER_KILLS, 100),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.KILL_IN_MINE_CART_MULTI, "Rider", "Kill 100 enemies while riding in a mine cart", PlatformStat.MINECART_KILLS, 100),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.KILL_WITH_PITS_MULTI, "Pit Lord", "Kill 100 enemies by knocking them into pits", PlatformStat.PIT_KILLS, 100),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.DIE_IN_PAST, "Time Paradox", "Die in the Past"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_A_JAMMED_BOSS, "Exorcist", "Kill a Jammed Boss"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.REACH_BLACK_MARKET, "The Password", "Accessed the Hidden Market"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.HAVE_MAX_CURSE, "Jammed", "You've met with a terrible fate, haven't you"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.FLIP_TABLES_MULTI, "Rage Mode", "Always be flipping. Guns are for flippers", PlatformStat.TABLES_FLIPPED, 500),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.COMPLETE_GAME_WITH_BEAST_MODE, "Beast Master", "Complete the game with Beast Mode on"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_PAST_ROBOT, "Terminated", "Kill the Robot's Past"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_PAST_BULLET, "Hero of Gun", "Kill the Bullet's Past"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.COMPLETE_GAME_WITH_CHALLENGE_MODE, "Challenger", "Complete Daisuke's trial"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_ADVANCED_DRAGUN, "Advanced Slayer", "Defeat an Advanced Boss"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.BEAT_METAL_GEAR_RAT, "Resourceful", "Take Revenge"),
      new PlatformInterfaceGalaxy.AchievementData(Achievement.COMPLETE_GAME_WITH_TURBO_MODE, "Sledge-Dog", "Complete Tonic's Challenge")
    };
    private PlatformInterfaceGalaxy.StatData[] m_stats = new PlatformInterfaceGalaxy.StatData[16 /*0x10*/]
    {
      new PlatformInterfaceGalaxy.StatData(PlatformStat.META_SPENT_AT_STORE),
      new PlatformInterfaceGalaxy.StatData(PlatformStat.FLOOR_ONE_CLEARS),
      new PlatformInterfaceGalaxy.StatData(PlatformStat.FLOOR_TWO_CLEARS),
      new PlatformInterfaceGalaxy.StatData(PlatformStat.FLOOR_THREE_CLEARS),
      new PlatformInterfaceGalaxy.StatData(PlatformStat.FLOOR_FOUR_CLEARS),
      new PlatformInterfaceGalaxy.StatData(PlatformStat.FLOOR_FIVE_CLEARS),
      new PlatformInterfaceGalaxy.StatData(PlatformStat.MAIN_FLOORS_MAPPED),
      new PlatformInterfaceGalaxy.StatData(PlatformStat.FRIFLE_CORE_COMPLETED),
      new PlatformInterfaceGalaxy.StatData(PlatformStat.WINCHESTER_ACED),
      new PlatformInterfaceGalaxy.StatData(PlatformStat.GUNSLING_COMPLETED),
      new PlatformInterfaceGalaxy.StatData(PlatformStat.BREACH_POPULATION),
      new PlatformInterfaceGalaxy.StatData(PlatformStat.ITEMS_STOLEN),
      new PlatformInterfaceGalaxy.StatData(PlatformStat.CHANDELIER_KILLS),
      new PlatformInterfaceGalaxy.StatData(PlatformStat.MINECART_KILLS),
      new PlatformInterfaceGalaxy.StatData(PlatformStat.PIT_KILLS),
      new PlatformInterfaceGalaxy.StatData(PlatformStat.TABLES_FLIPPED)
    };

    public static bool IsGalaxyBuild()
    {
      return File.Exists(Path.Combine(Application.dataPath, "../Galaxy.dll"));
    }

    protected override void OnStart()
    {
      Debug.Log((object) "Starting GOG Galaxy platform interface.");
    }

    protected override void OnAchievementUnlock(Achievement achievement, int playerIndex)
    {
      if (!GalaxyManager.Initialized || !this.m_isInitialized)
        return;
      PlatformInterfaceGalaxy.AchievementData achievementData = (PlatformInterfaceGalaxy.AchievementData) null;
      for (int index = 0; index < this.m_achievements.Length; ++index)
      {
        if (this.m_achievements[index].achievement == achievement)
          achievementData = this.m_achievements[index];
      }
      if (achievementData == null)
        return;
      achievementData.isUnlocked = true;
      try
      {
        GalaxyInstance.Stats().SetAchievement(achievementData.ApiKey);
      }
      catch (Exception ex)
      {
        Debug.LogError((object) ex);
      }
      this.m_bStoreStats = true;
    }

    public override bool IsAchievementUnlocked(Achievement achievement)
    {
      PlatformInterfaceGalaxy.AchievementData achievementData = (PlatformInterfaceGalaxy.AchievementData) null;
      for (int index = 0; index < this.m_achievements.Length; ++index)
      {
        if (this.m_achievements[index].achievement == achievement)
          achievementData = this.m_achievements[index];
      }
      return achievementData != null && achievementData.isUnlocked;
    }

    public override void SetStat(PlatformStat stat, int value)
    {
      if (!GalaxyManager.Initialized || !this.m_isInitialized)
        return;
      PlatformInterfaceGalaxy.StatData statData = (PlatformInterfaceGalaxy.StatData) null;
      for (int index = 0; index < this.m_stats.Length; ++index)
      {
        if (this.m_stats[index].stat == stat)
          statData = this.m_stats[index];
      }
      if (statData == null || value <= statData.value)
        return;
      statData.value = value;
      try
      {
        GalaxyInstance.Stats().SetStatInt(statData.ApiKey, value);
      }
      catch (Exception ex)
      {
        Debug.LogError((object) ex);
      }
    }

    public override void IncrementStat(PlatformStat stat, int delta)
    {
      if (!GalaxyManager.Initialized || !this.m_isInitialized)
        return;
      PlatformInterfaceGalaxy.StatData statData = (PlatformInterfaceGalaxy.StatData) null;
      for (int index = 0; index < this.m_stats.Length; ++index)
      {
        if (this.m_stats[index].stat == stat)
          statData = this.m_stats[index];
      }
      if (statData == null)
        return;
      int num = statData.value;
      statData.value = num + delta;
      try
      {
        GalaxyInstance.Stats().SetStatInt(statData.ApiKey, statData.value);
      }
      catch (Exception ex)
      {
        Debug.LogError((object) ex);
      }
    }

    public override void StoreStats() => this.m_bStoreStats = true;

    public override void ResetStats(bool achievementsToo)
    {
      try
      {
        GalaxyInstance.Stats().ResetStatsAndAchievements();
      }
      catch (Exception ex)
      {
        Debug.LogError((object) ex);
      }
      this.m_bRequestedStats = false;
      this.m_bStatsValid = false;
    }

    protected override void OnLateUpdate()
    {
      if (!GalaxyManager.Initialized)
        return;
      if (GalaxyManager.Initialized && !this.m_isInitialized)
      {
        this.m_achievementsReceivedListener = new PlatformInterfaceGalaxy.AchievementsReceivedListener(this);
        this.m_storeStatsCallback = new PlatformInterfaceGalaxy.StatsStoredListener();
        this.m_isInitialized = true;
      }
      else
      {
        if (!this.m_bRequestedStats)
        {
          if (!GalaxyManager.Initialized)
          {
            this.m_bRequestedStats = true;
            return;
          }
          try
          {
            GalaxyInstance.Stats().RequestUserStatsAndAchievements();
            this.m_bRequestedStats = true;
          }
          catch (Exception ex)
          {
            Debug.LogError((object) ex);
          }
        }
        if (!this.m_bStatsValid)
          return;
        if (!this.m_bStoreStats)
          return;
        try
        {
          GalaxyInstance.Stats().StoreStatsAndAchievements();
          this.m_bStoreStats = false;
        }
        catch (Exception ex)
        {
          Debug.LogError((object) ex);
        }
      }
    }

    protected override StringTableManager.GungeonSupportedLanguages OnGetPreferredLanguage()
    {
      return StringTableManager.GungeonSupportedLanguages.ENGLISH;
    }

    public void DebugPrintAchievements()
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < this.m_achievements.Length; ++index)
      {
        PlatformInterfaceGalaxy.AchievementData achievementData = this.m_achievements[index];
        stringBuilder.AppendFormat("[{0}] {1}\n", !achievementData.isUnlocked ? (object) " " : (object) "X", (object) achievementData.name);
        stringBuilder.AppendFormat("{0}\n", (object) achievementData.description);
        if (achievementData.hasProgressStat)
        {
          PlatformInterfaceGalaxy.StatData statData = Array.Find<PlatformInterfaceGalaxy.StatData>(this.m_stats, (Predicate<PlatformInterfaceGalaxy.StatData>) (s => s.stat == achievementData.progressStat));
          stringBuilder.AppendFormat("{0} of {1}\n", (object) statData.value, (object) achievementData.goalValue);
        }
        stringBuilder.AppendLine();
      }
      Debug.Log((object) stringBuilder.ToString());
    }

    public void OnUserStatsReceived()
    {
      Debug.Log((object) "Received stats and achievements from Galaxy\n");
      this.m_bStatsValid = true;
      foreach (PlatformInterfaceGalaxy.AchievementData achievement in this.m_achievements)
      {
        try
        {
          GalaxyInstance.Stats().GetAchievement(achievement.ApiKey, ref achievement.isUnlocked, ref achievement.unlockTime);
          achievement.name = GalaxyInstance.Stats().GetAchievementDisplayName(achievement.ApiKey);
          achievement.description = GalaxyInstance.Stats().GetAchievementDescription(achievement.ApiKey);
        }
        catch (Exception ex)
        {
          Debug.LogError((object) ex);
        }
      }
      foreach (PlatformInterfaceGalaxy.StatData stat in this.m_stats)
      {
        try
        {
          stat.value = GalaxyInstance.Stats().GetStatInt(stat.ApiKey);
        }
        catch (Exception ex)
        {
          Debug.LogError((object) ex);
        }
      }
    }

    public class AchievementsReceivedListener : GlobalUserStatsAndAchievementsRetrieveListener
    {
      private PlatformInterfaceGalaxy m_platformInterface;

      public AchievementsReceivedListener(PlatformInterfaceGalaxy platformInterface)
      {
        this.m_platformInterface = platformInterface;
      }

      public override void OnUserStatsAndAchievementsRetrieveSuccess(GalaxyID userID)
      {
        Debug.Log((object) "Received achievement data!");
        this.m_platformInterface.OnUserStatsReceived();
        this.m_platformInterface.CatchupAchievements();
      }

      public override void OnUserStatsAndAchievementsRetrieveFailure(
        GalaxyID userID,
        IUserStatsAndAchievementsRetrieveListener.FailureReason failureReason)
      {
        Debug.LogErrorFormat("OnUserStatsAndAchievementsRetrieveFailure() Error: {0} ", (object) failureReason);
      }
    }

    public class StatsStoredListener : GlobalStatsAndAchievementsStoreListener
    {
      public override void OnUserStatsAndAchievementsStoreSuccess()
      {
        Debug.Log((object) "Stats and achievements stored!");
      }

      public override void OnUserStatsAndAchievementsStoreFailure(
        IStatsAndAchievementsStoreListener.FailureReason failureReason)
      {
        Debug.LogErrorFormat("OnUserStatsAndAchievementsStoreFailure() Error: {0} ", (object) failureReason);
      }
    }

    private class AchievementData
    {
      public Achievement achievement;
      public string name;
      public string description;
      public bool isUnlocked;
      public uint unlockTime;
      public bool hasProgressStat;
      public PlatformStat progressStat;
      public int goalValue;
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
        int goalValue)
      {
        this.achievement = achievement;
        this.name = name;
        this.description = desc;
        this.isUnlocked = false;
        this.hasProgressStat = true;
        this.progressStat = progressStat;
        this.goalValue = goalValue;
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
  }

