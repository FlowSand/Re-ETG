using FullSerializer;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

[fsObject]
public class GameStatsManager
  {
    [fsProperty]
    public Dictionary<PlayableCharacters, GameStats> m_characterStats = new Dictionary<PlayableCharacters, GameStats>((IEqualityComparer<PlayableCharacters>) new PlayableCharactersComparer());
    [fsProperty]
    private Dictionary<string, EncounteredObjectData> m_encounteredTrackables = new Dictionary<string, EncounteredObjectData>();
    [fsProperty]
    public Dictionary<string, int> m_encounteredFlows = new Dictionary<string, int>();
    [fsProperty]
    public Dictionary<string, EncounteredObjectData> m_encounteredRooms = new Dictionary<string, EncounteredObjectData>();
    [fsProperty]
    public HashSet<GungeonFlags> m_flags = new HashSet<GungeonFlags>((IEqualityComparer<GungeonFlags>) new GungeonFlagsComparer());
    [fsProperty]
    public Dictionary<string, int> m_persistentStringsLastIndices = new Dictionary<string, int>();
    [fsProperty]
    public Dictionary<int, int> m_encounteredSynergiesByID = new Dictionary<int, int>();
    [fsProperty]
    public MonsterHuntProgress huntProgress;
    [fsProperty]
    public int CurrentResRatShopSeed = -1;
    [fsProperty]
    public int CurrentEeveeEquipSeed = -1;
    [fsProperty]
    public int CurrentAccumulatedGunderfuryExperience;
    [fsProperty]
    public int CurrentRobotArmFloor = 5;
    [fsProperty]
    public int NumberRunsValidCellWithoutSpawn;
    [fsProperty]
    public float AccumulatedBeetleMerchantChance;
    [fsProperty]
    public float AccumulatedUsedBeetleMerchantChance;
    [fsProperty]
    public Dictionary<GlobalDungeonData.ValidTilesets, string> LastBossEncounteredMap = new Dictionary<GlobalDungeonData.ValidTilesets, string>();
    [fsProperty]
    private HashSet<string> forcedUnlocks = new HashSet<string>();
    [fsProperty]
    public string midGameSaveGuid;
    [fsProperty]
    public int savedSystemHash = -1;
    [fsProperty]
    public bool isChump;
    [fsProperty]
    public bool isTurboMode;
    [fsProperty]
    public bool rainbowRunToggled;
    private int m_numCharacters = -1;
    private PlayableCharacters m_sessionCharacter;
    private GameStats m_sessionStats;
    private GameStats m_savedSessionStats;
    private HashSet<int> m_sessionSynergies = new HashSet<int>();
    private static GameStatsManager m_instance;
    private static List<GungeonFlags> s_pastFlags;
    private static List<GungeonFlags> s_npcFoyerFlags;
    private static List<GungeonFlags> s_frifleHuntFlags;
    [fsIgnore]
    private List<string> m_singleProcessedEncounterTrackables = new List<string>();

    public bool IsRainbowRun
    {
      get
      {
        GameManager instance = GameManager.Instance;
        return instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.CHARACTER_PAST && instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER && instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.TUTORIAL && this.rainbowRunToggled;
      }
    }

    public static GameStatsManager Instance
    {
      get
      {
        if (GameStatsManager.m_instance == null)
          Debug.LogError((object) "Trying to access GameStatsManager before it has been initialized.");
        return GameStatsManager.m_instance;
      }
    }

    public static bool HasInstance => GameStatsManager.m_instance != null;

    public int PlaytimeMin
    {
      get => Mathf.RoundToInt(this.GetPlayerStatValue(TrackedStats.TIME_PLAYED) / 60f);
    }

    public float NewPlayerEnemyCullFactor
    {
      get
      {
        return this.GetFlag(GungeonFlags.BOSSKILLED_DRAGUN) ? 0.0f : Mathf.Clamp(0.1f - (float) Mathf.RoundToInt(this.GetPlayerStatValue(TrackedStats.TIMES_REACHED_FORGE)) * 0.02f, 0.0f, 0.1f);
      }
    }

    public static void Load()
    {
      SaveManager.Init();
      if (!SaveManager.Load<GameStatsManager>(SaveManager.GameSave, out GameStatsManager.m_instance, true))
        GameStatsManager.m_instance = new GameStatsManager();
      if (GameStatsManager.m_instance.huntProgress != null)
      {
        GameStatsManager.m_instance.huntProgress.OnLoaded();
      }
      else
      {
        GameStatsManager.m_instance.huntProgress = new MonsterHuntProgress();
        GameStatsManager.m_instance.huntProgress.OnLoaded();
      }
      if (GameStatsManager.s_pastFlags == null)
      {
        GameStatsManager.s_pastFlags = new List<GungeonFlags>();
        GameStatsManager.s_pastFlags.Add(GungeonFlags.BOSSKILLED_ROGUE_PAST);
        GameStatsManager.s_pastFlags.Add(GungeonFlags.BOSSKILLED_CONVICT_PAST);
        GameStatsManager.s_pastFlags.Add(GungeonFlags.BOSSKILLED_SOLDIER_PAST);
        GameStatsManager.s_pastFlags.Add(GungeonFlags.BOSSKILLED_GUIDE_PAST);
      }
      if (GameStatsManager.s_npcFoyerFlags == null)
      {
        GameStatsManager.s_npcFoyerFlags = new List<GungeonFlags>();
        GameStatsManager.s_npcFoyerFlags.Add(GungeonFlags.META_SHOP_ACTIVE_IN_FOYER);
        GameStatsManager.s_npcFoyerFlags.Add(GungeonFlags.GUNSLING_KING_ACTIVE_IN_FOYER);
        GameStatsManager.s_npcFoyerFlags.Add(GungeonFlags.SORCERESS_ACTIVE_IN_FOYER);
        GameStatsManager.s_npcFoyerFlags.Add(GungeonFlags.LOST_ADVENTURER_ACTIVE_IN_FOYER);
        GameStatsManager.s_npcFoyerFlags.Add(GungeonFlags.TUTORIAL_TALKED_AFTER_RIVAL_KILLED);
        GameStatsManager.s_npcFoyerFlags.Add(GungeonFlags.SHOP_TRUCK_ACTIVE);
        GameStatsManager.s_npcFoyerFlags.Add(GungeonFlags.SHERPA_ACTIVE_IN_ELEVATOR_ROOM);
        GameStatsManager.s_npcFoyerFlags.Add(GungeonFlags.WINCHESTER_MET_PREVIOUSLY);
        GameStatsManager.s_npcFoyerFlags.Add(GungeonFlags.LEDGEGOBLIN_ACTIVE_IN_FOYER);
        GameStatsManager.s_npcFoyerFlags.Add(GungeonFlags.FRIFLE_ACTIVE_IN_FOYER);
      }
      if (GameStatsManager.s_frifleHuntFlags != null)
        return;
      GameStatsManager.s_frifleHuntFlags = new List<GungeonFlags>();
      GameStatsManager.s_frifleHuntFlags.Add(GungeonFlags.FRIFLE_MONSTERHUNT_01_COMPLETE);
      GameStatsManager.s_frifleHuntFlags.Add(GungeonFlags.FRIFLE_MONSTERHUNT_02_COMPLETE);
      GameStatsManager.s_frifleHuntFlags.Add(GungeonFlags.FRIFLE_MONSTERHUNT_03_COMPLETE);
      GameStatsManager.s_frifleHuntFlags.Add(GungeonFlags.FRIFLE_MONSTERHUNT_04_COMPLETE);
      GameStatsManager.s_frifleHuntFlags.Add(GungeonFlags.FRIFLE_MONSTERHUNT_05_COMPLETE);
      GameStatsManager.s_frifleHuntFlags.Add(GungeonFlags.FRIFLE_MONSTERHUNT_06_COMPLETE);
      GameStatsManager.s_frifleHuntFlags.Add(GungeonFlags.FRIFLE_MONSTERHUNT_07_COMPLETE);
      GameStatsManager.s_frifleHuntFlags.Add(GungeonFlags.FRIFLE_MONSTERHUNT_08_COMPLETE);
      GameStatsManager.s_frifleHuntFlags.Add(GungeonFlags.FRIFLE_MONSTERHUNT_09_COMPLETE);
      GameStatsManager.s_frifleHuntFlags.Add(GungeonFlags.FRIFLE_MONSTERHUNT_10_COMPLETE);
      GameStatsManager.s_frifleHuntFlags.Add(GungeonFlags.FRIFLE_MONSTERHUNT_11_COMPLETE);
      GameStatsManager.s_frifleHuntFlags.Add(GungeonFlags.FRIFLE_MONSTERHUNT_12_COMPLETE);
      GameStatsManager.s_frifleHuntFlags.Add(GungeonFlags.FRIFLE_MONSTERHUNT_13_COMPLETE);
      GameStatsManager.s_frifleHuntFlags.Add(GungeonFlags.FRIFLE_MONSTERHUNT_14_COMPLETE);
      GameStatsManager.s_frifleHuntFlags.Add(GungeonFlags.FRIFLE_CORE_HUNTS_COMPLETE);
    }

    public static bool Save()
    {
      GameManager.Instance.platformInterface.StoreStats();
      bool flag = false;
      try
      {
        flag = SaveManager.Save<GameStatsManager>(GameStatsManager.m_instance, SaveManager.GameSave, GameStatsManager.m_instance.PlaytimeMin);
      }
      catch (Exception ex)
      {
        Debug.LogErrorFormat("SAVE FAILED: {0}", (object) ex);
      }
      return flag;
    }

    public static void DANGEROUS_ResetAllStats()
    {
      GameStatsManager.m_instance = new GameStatsManager();
      GameStatsManager.m_instance.huntProgress = new MonsterHuntProgress();
      GameStatsManager.m_instance.huntProgress.OnLoaded();
      SaveManager.DeleteAllBackups(SaveManager.GameSave);
      SaveManager.ResetSaveSlot = false;
    }

    public void BeginNewSession(PlayerController player)
    {
      if (this.IsInSession)
      {
        BraveUtility.Log("MODIFYING CHARACTER FOR SESSION STATS", Color.red, BraveUtility.LogVerbosity.IMPORTANT);
        this.m_sessionCharacter = player.characterIdentity;
        this.m_sessionSynergies.Clear();
        if (!this.m_characterStats.ContainsKey(player.characterIdentity))
          this.m_characterStats.Add(player.characterIdentity, new GameStats());
        foreach (int startingGunId in player.startingGunIds)
        {
          EncounterTrackable component = (PickupObjectDatabase.GetById(startingGunId) as Gun).GetComponent<EncounterTrackable>();
          if ((bool) (UnityEngine.Object) component && this.QueryEncounterableDifferentiator(component) < 1)
          {
            this.HandleEncounteredObject(component);
            this.SetEncounterableDifferentiator(component, 1);
          }
        }
      }
      else
      {
        BraveUtility.Log("CREATING NEW SESSION STATS", Color.red, BraveUtility.LogVerbosity.IMPORTANT);
        this.m_sessionCharacter = player.characterIdentity;
        this.m_sessionSynergies.Clear();
        this.m_sessionStats = new GameStats();
        this.m_savedSessionStats = new GameStats();
        if (!this.m_characterStats.ContainsKey(player.characterIdentity))
          this.m_characterStats.Add(player.characterIdentity, new GameStats());
        foreach (int startingGunId in player.startingGunIds)
        {
          EncounterTrackable component = (PickupObjectDatabase.GetById(startingGunId) as Gun).GetComponent<EncounterTrackable>();
          if ((bool) (UnityEngine.Object) component && this.QueryEncounterableDifferentiator(component) < 1)
          {
            this.HandleEncounteredObject(component);
            this.SetEncounterableDifferentiator(component, 1);
          }
        }
      }
      if (!this.GetFlag(GungeonFlags.TONIC_ACTIVE_IN_FOYER) && GameManager.IsTurboMode)
        GameStatsManager.Instance.isTurboMode = false;
      if (this.GetFlag(GungeonFlags.BOWLER_ACTIVE_IN_FOYER) || !GameStatsManager.Instance.rainbowRunToggled)
        return;
      GameStatsManager.Instance.rainbowRunToggled = false;
    }

    public void AssignMidGameSavedSessionStats(GameStats source)
    {
      if (!this.IsInSession || this.m_savedSessionStats == null)
        return;
      this.m_savedSessionStats.AddStats(source);
    }

    public GameStats MoveSessionStatsToSavedSessionStats()
    {
      if (!this.IsInSession)
        return (GameStats) null;
      if (this.m_sessionStats != null)
      {
        this.m_sessionStats.SetMax(TrackedMaximums.MOST_ENEMIES_KILLED, this.m_sessionStats.GetStatValue(TrackedStats.ENEMIES_KILLED) + this.m_savedSessionStats.GetStatValue(TrackedStats.ENEMIES_KILLED));
        if (this.m_characterStats.ContainsKey(this.m_sessionCharacter))
          this.m_characterStats[this.m_sessionCharacter].AddStats(this.m_sessionStats);
        this.m_savedSessionStats.AddStats(this.m_sessionStats);
        this.m_sessionStats.ClearAllState();
      }
      return this.m_savedSessionStats;
    }

    public void EndSession(bool recordSessionStats, bool decrementDifferentiator = true)
    {
      if (!this.IsInSession)
        return;
      if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.TUTORIAL)
        decrementDifferentiator = false;
      BraveUtility.Log("ENDING SESSION. RIGHT NOW: " + (object) decrementDifferentiator, Color.red, BraveUtility.LogVerbosity.IMPORTANT);
      if (this.m_sessionStats != null)
      {
        if (recordSessionStats)
        {
          this.m_sessionStats.SetMax(TrackedMaximums.MOST_ENEMIES_KILLED, this.m_sessionStats.GetStatValue(TrackedStats.ENEMIES_KILLED));
          if (this.m_characterStats.ContainsKey(this.m_sessionCharacter))
            this.m_characterStats[this.m_sessionCharacter].AddStats(this.m_sessionStats);
          else
            Debug.LogWarning((object) $"Character stats for {(object) this.m_sessionCharacter} were not found; session stats are being thrown away.");
        }
        this.m_sessionStats = (GameStats) null;
        this.m_savedSessionStats = (GameStats) null;
      }
      if (this.m_singleProcessedEncounterTrackables != null)
        this.m_singleProcessedEncounterTrackables.Clear();
      if (!decrementDifferentiator)
        return;
      foreach (string key in this.m_encounteredTrackables.Keys)
      {
        if (this.m_encounteredTrackables[key].differentiator > 0)
          this.m_encounteredTrackables[key].differentiator = Mathf.Min(this.m_encounteredTrackables[key].differentiator - 1, 3);
      }
      foreach (string key in this.m_encounteredRooms.Keys)
      {
        if (this.m_encounteredRooms[key].differentiator > 0)
          this.m_encounteredRooms[key].differentiator = Mathf.Min(3, this.m_encounteredRooms[key].differentiator - 1);
      }
      List<string> stringList = new List<string>();
      foreach (string key in this.m_encounteredFlows.Keys)
      {
        if (this.m_encounteredFlows[key] > 0)
          stringList.Add(key);
      }
      foreach (string key in stringList)
        --this.m_encounteredFlows[key];
    }

    [fsIgnore]
    public bool IsInSession => this.m_sessionStats != null;

    public void ClearAllStatsGlobal()
    {
      this.m_sessionStats.ClearAllState();
      this.m_savedSessionStats.ClearAllState();
      if (this.m_numCharacters <= 0)
        this.m_numCharacters = Enum.GetValues(typeof (PlayableCharacters)).Length;
      for (int key = 0; key < this.m_numCharacters; ++key)
      {
        GameStats gameStats;
        if (this.m_characterStats.TryGetValue((PlayableCharacters) key, out gameStats))
          gameStats.ClearAllState();
      }
    }

    public void ClearStatValueGlobal(TrackedStats stat)
    {
      this.m_sessionStats.SetStat(stat, 0.0f);
      this.m_savedSessionStats.SetStat(stat, 0.0f);
      if (this.m_numCharacters <= 0)
        this.m_numCharacters = Enum.GetValues(typeof (PlayableCharacters)).Length;
      for (int key = 0; key < this.m_numCharacters; ++key)
      {
        GameStats gameStats;
        if (this.m_characterStats.TryGetValue((PlayableCharacters) key, out gameStats))
          gameStats.SetStat(stat, 0.0f);
      }
    }

    public void UpdateMaximum(TrackedMaximums stat, float val)
    {
      if (float.IsNaN(val) || float.IsInfinity(val) || this.m_sessionStats == null)
        return;
      this.m_sessionStats.SetMax(stat, val);
    }

    public void SetStat(TrackedStats stat, float value)
    {
      if (float.IsNaN(value) || float.IsInfinity(value) || this.m_sessionStats == null)
        return;
      this.m_sessionStats.SetStat(stat, value);
      this.HandleStatAchievements(stat);
      this.HandleSetPlatformStat(stat, this.GetPlayerStatValue(stat));
    }

    public void RegisterStatChange(TrackedStats stat, float value)
    {
      if (this.m_sessionStats == null)
      {
        Debug.LogError((object) "No session stats active and we're registering a stat change!");
      }
      else
      {
        if (float.IsNaN(value) || float.IsInfinity(value) || (double) Mathf.Abs(value) > 10000.0)
          return;
        float playerStatValue = this.GetPlayerStatValue(stat);
        this.m_sessionStats.IncrementStat(stat, value);
        this.HandleStatAchievements(stat);
        this.HandleIncrementPlatformStat(stat, value, playerStatValue);
        GameManager.Instance.platformInterface.SendEvent(PlatformInterface.GetTrackedStatEventString(stat), 1);
      }
    }

    public void SetNextFlag(params GungeonFlags[] flagList)
    {
      for (int index = 0; index < flagList.Length; ++index)
      {
        if (!this.GetFlag(flagList[index]))
        {
          this.SetFlag(flagList[index], true);
          break;
        }
      }
    }

    public void SetFlag(GungeonFlags flag, bool value)
    {
      if (flag == GungeonFlags.NONE)
      {
        Debug.LogError((object) "Something is attempting to set a NONE save flag!");
      }
      else
      {
        if (value)
          this.m_flags.Add(flag);
        else
          this.m_flags.Remove(flag);
        if (value)
          this.HandleFlagAchievements(flag);
        if (!value || flag != GungeonFlags.BOSSKILLED_DRAGUN || !GameManager.Options.m_beastmode)
          return;
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.COMPLETE_GAME_WITH_BEAST_MODE);
      }
    }

    public void SetCharacterSpecificFlag(CharacterSpecificGungeonFlags flag, bool value)
    {
      this.SetCharacterSpecificFlag(this.m_sessionCharacter, flag, value);
    }

    public void SetCharacterSpecificFlag(
      PlayableCharacters character,
      CharacterSpecificGungeonFlags flag,
      bool value)
    {
      if (flag == CharacterSpecificGungeonFlags.NONE)
      {
        Debug.LogError((object) "Something is attempting to set a NONE character-specific save flag!");
      }
      else
      {
        if (!this.m_characterStats.ContainsKey(character))
          this.m_characterStats.Add(character, new GameStats());
        if (this.m_sessionStats != null && this.m_sessionCharacter == character)
          this.m_sessionStats.SetFlag(flag, value);
        else
          this.m_characterStats[character].SetFlag(flag, value);
        if (flag != CharacterSpecificGungeonFlags.KILLED_PAST)
          return;
        PlayerController playerController = GameManager.Instance.PrimaryPlayer;
        if (character == PlayableCharacters.CoopCultist)
          playerController = GameManager.Instance.SecondaryPlayer;
        else if ((bool) (UnityEngine.Object) playerController && playerController.IsTemporaryEeveeForUnlock)
          GameStatsManager.Instance.SetFlag(GungeonFlags.FLAG_EEVEE_UNLOCKED, true);
        if (!(bool) (UnityEngine.Object) playerController || !playerController.IsUsingAlternateCostume)
          return;
        if (this.m_sessionStats != null && this.m_sessionCharacter == character)
          this.m_sessionStats.SetFlag(CharacterSpecificGungeonFlags.KILLED_PAST_ALTERNATE_COSTUME, value);
        else
          this.m_characterStats[character].SetFlag(CharacterSpecificGungeonFlags.KILLED_PAST_ALTERNATE_COSTUME, value);
        if (!value)
          return;
        this.SetFlag(GungeonFlags.ITEMSPECIFIC_ALTERNATE_GUNS_UNLOCKED, true);
      }
    }

    public void SetPersistentStringLastIndex(string key, int value)
    {
      this.m_persistentStringsLastIndices[key] = value;
    }

    public void ForceUnlock(string encounterGuid) => this.forcedUnlocks.Add(encounterGuid);

    public bool IsForceUnlocked(string encounterGuid) => this.forcedUnlocks.Contains(encounterGuid);

    public float GetPlayerMaximum(TrackedMaximums stat)
    {
      if (this.m_numCharacters <= 0)
        this.m_numCharacters = Enum.GetValues(typeof (PlayableCharacters)).Length;
      float a = 0.0f;
      if (this.m_sessionStats != null)
        a = Mathf.Max(a, this.m_sessionStats.GetMaximumValue(stat), this.m_savedSessionStats.GetMaximumValue(stat));
      for (int key = 0; key < this.m_numCharacters; ++key)
      {
        GameStats gameStats;
        if (this.m_characterStats.TryGetValue((PlayableCharacters) key, out gameStats))
          a = Mathf.Max(a, gameStats.GetMaximumValue(stat));
      }
      return a;
    }

    public float GetSessionStatValue(TrackedStats stat)
    {
      return this.m_sessionStats.GetStatValue(stat) + this.m_savedSessionStats.GetStatValue(stat);
    }

    public float GetCharacterStatValue(TrackedStats stat)
    {
      return this.GetCharacterStatValue(this.GetCurrentCharacter(), stat);
    }

    public float GetCharacterStatValue(PlayableCharacters character, TrackedStats stat)
    {
      float characterStatValue = 0.0f;
      if (this.m_sessionCharacter == character)
        characterStatValue += this.m_sessionStats.GetStatValue(stat);
      if (this.m_characterStats.ContainsKey(character))
        characterStatValue += this.m_characterStats[character].GetStatValue(stat);
      return characterStatValue;
    }

    public float GetPlayerStatValue(TrackedStats stat)
    {
      if (this.m_numCharacters <= 0)
        this.m_numCharacters = Enum.GetValues(typeof (PlayableCharacters)).Length;
      float playerStatValue = 0.0f;
      if (this.m_sessionStats != null)
        playerStatValue += this.m_sessionStats.GetStatValue(stat);
      for (int key = 0; key < this.m_numCharacters; ++key)
      {
        GameStats gameStats;
        if (this.m_characterStats.TryGetValue((PlayableCharacters) key, out gameStats))
          playerStatValue += gameStats.GetStatValue(stat);
      }
      return playerStatValue;
    }

    public bool TestPastBeaten(PlayableCharacters character)
    {
      switch (character)
      {
        case PlayableCharacters.Pilot:
          return this.GetCharacterSpecificFlag(character, CharacterSpecificGungeonFlags.KILLED_PAST);
        case PlayableCharacters.Convict:
          return this.GetCharacterSpecificFlag(character, CharacterSpecificGungeonFlags.KILLED_PAST);
        case PlayableCharacters.Robot:
          return this.GetCharacterSpecificFlag(character, CharacterSpecificGungeonFlags.KILLED_PAST);
        case PlayableCharacters.Soldier:
          return this.GetCharacterSpecificFlag(character, CharacterSpecificGungeonFlags.KILLED_PAST);
        case PlayableCharacters.Guide:
          return this.GetCharacterSpecificFlag(character, CharacterSpecificGungeonFlags.KILLED_PAST);
        case PlayableCharacters.CoopCultist:
          return this.GetCharacterSpecificFlag(character, CharacterSpecificGungeonFlags.KILLED_PAST);
        case PlayableCharacters.Bullet:
          return this.GetCharacterSpecificFlag(character, CharacterSpecificGungeonFlags.KILLED_PAST);
        default:
          return false;
      }
    }

    public bool AllCorePastsBeaten()
    {
      return this.GetCharacterSpecificFlag(PlayableCharacters.Pilot, CharacterSpecificGungeonFlags.KILLED_PAST) && this.GetCharacterSpecificFlag(PlayableCharacters.Convict, CharacterSpecificGungeonFlags.KILLED_PAST) && this.GetCharacterSpecificFlag(PlayableCharacters.Soldier, CharacterSpecificGungeonFlags.KILLED_PAST) && this.GetCharacterSpecificFlag(PlayableCharacters.Guide, CharacterSpecificGungeonFlags.KILLED_PAST);
    }

    public bool CheckLameyCostumeUnlocked() => false;

    public bool CheckGunslingerCostumeUnlocked()
    {
      return GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_DRAGUN) & GameStatsManager.Instance.GetFlag(GungeonFlags.BOSSKILLED_LICH) & GameStatsManager.Instance.GetCharacterSpecificFlag(PlayableCharacters.Bullet, CharacterSpecificGungeonFlags.KILLED_PAST) & GameStatsManager.Instance.GetCharacterSpecificFlag(PlayableCharacters.Convict, CharacterSpecificGungeonFlags.KILLED_PAST) & GameStatsManager.Instance.GetCharacterSpecificFlag(PlayableCharacters.Guide, CharacterSpecificGungeonFlags.KILLED_PAST) & GameStatsManager.Instance.GetCharacterSpecificFlag(PlayableCharacters.Pilot, CharacterSpecificGungeonFlags.KILLED_PAST) & GameStatsManager.Instance.GetCharacterSpecificFlag(PlayableCharacters.Robot, CharacterSpecificGungeonFlags.KILLED_PAST) & GameStatsManager.Instance.GetCharacterSpecificFlag(PlayableCharacters.Soldier, CharacterSpecificGungeonFlags.KILLED_PAST);
    }

    public int GetNumberPastsBeaten()
    {
      int numberPastsBeaten = 0;
      if (this.GetCharacterSpecificFlag(PlayableCharacters.Pilot, CharacterSpecificGungeonFlags.KILLED_PAST))
        ++numberPastsBeaten;
      if (this.GetCharacterSpecificFlag(PlayableCharacters.Convict, CharacterSpecificGungeonFlags.KILLED_PAST))
        ++numberPastsBeaten;
      if (this.GetCharacterSpecificFlag(PlayableCharacters.Soldier, CharacterSpecificGungeonFlags.KILLED_PAST))
        ++numberPastsBeaten;
      if (this.GetCharacterSpecificFlag(PlayableCharacters.Guide, CharacterSpecificGungeonFlags.KILLED_PAST))
        ++numberPastsBeaten;
      if (this.GetCharacterSpecificFlag(PlayableCharacters.Robot, CharacterSpecificGungeonFlags.KILLED_PAST))
        ++numberPastsBeaten;
      if (this.GetCharacterSpecificFlag(PlayableCharacters.Bullet, CharacterSpecificGungeonFlags.KILLED_PAST))
        ++numberPastsBeaten;
      return numberPastsBeaten;
    }

    public bool AnyPastBeaten()
    {
      return this.GetCharacterSpecificFlag(PlayableCharacters.Pilot, CharacterSpecificGungeonFlags.KILLED_PAST) || this.GetCharacterSpecificFlag(PlayableCharacters.Convict, CharacterSpecificGungeonFlags.KILLED_PAST) || this.GetCharacterSpecificFlag(PlayableCharacters.Soldier, CharacterSpecificGungeonFlags.KILLED_PAST) || this.GetCharacterSpecificFlag(PlayableCharacters.Guide, CharacterSpecificGungeonFlags.KILLED_PAST) || this.GetCharacterSpecificFlag(PlayableCharacters.Robot, CharacterSpecificGungeonFlags.KILLED_PAST) || this.GetCharacterSpecificFlag(PlayableCharacters.Bullet, CharacterSpecificGungeonFlags.KILLED_PAST);
    }

    public int GetNumberOfCompanionsUnlocked()
    {
      int num = 0;
      int companionsUnlocked = 0;
      for (int index = 0; index < PickupObjectDatabase.Instance.Objects.Count; ++index)
      {
        PickupObject pickupObject = PickupObjectDatabase.Instance.Objects[index];
        if ((bool) (UnityEngine.Object) pickupObject && pickupObject is CompanionItem && pickupObject.quality != PickupObject.ItemQuality.EXCLUDED && pickupObject.contentSource != ContentSource.EXCLUDED)
        {
          ++num;
          if (!(bool) (UnityEngine.Object) pickupObject.encounterTrackable || pickupObject.encounterTrackable.PrerequisitesMet())
            ++companionsUnlocked;
        }
      }
      return companionsUnlocked;
    }

    public bool HasPast(PlayableCharacters id)
    {
      return id != PlayableCharacters.Eevee && id != PlayableCharacters.Gunslinger && id != PlayableCharacters.Cosmonaut;
    }

    public bool GetFlag(GungeonFlags flag)
    {
      if (flag != GungeonFlags.NONE)
        return this.m_flags.Contains(flag);
      Debug.LogError((object) "Something is attempting to get a NONE save flag!");
      return false;
    }

    public bool GetCharacterSpecificFlag(CharacterSpecificGungeonFlags flag)
    {
      return this.GetCharacterSpecificFlag(this.m_sessionCharacter, flag);
    }

    public bool GetCharacterSpecificFlag(
      PlayableCharacters character,
      CharacterSpecificGungeonFlags flag)
    {
      if (flag == CharacterSpecificGungeonFlags.NONE)
      {
        Debug.LogError((object) "Something is attempting to get a NONE character-specific save flag!");
        return false;
      }
      if (this.m_sessionStats != null && this.m_sessionCharacter == character && (this.m_sessionStats.GetFlag(flag) || this.m_savedSessionStats.GetFlag(flag)))
        return true;
      GameStats gameStats;
      return this.m_characterStats.TryGetValue(character, out gameStats) && gameStats.GetFlag(flag);
    }

    public int GetPersistentStringLastIndex(string key)
    {
      int num;
      return this.m_persistentStringsLastIndices.TryGetValue(key, out num) ? num : -1;
    }

    public void EncounterFlow(string flowName)
    {
      Debug.Log((object) ("ENCOUNTERING FLOW: " + flowName));
      if (!this.m_encounteredFlows.ContainsKey(flowName))
        this.m_encounteredFlows.Add(flowName, 2);
      else
        this.m_encounteredFlows[flowName] = this.m_encounteredFlows[flowName] + 2;
    }

    public int QueryFlowDifferentiator(string flowName)
    {
      if (BraveRandom.IgnoreGenerationDifferentiator || !this.m_encounteredFlows.ContainsKey(flowName))
        return 0;
      int num = this.m_encounteredFlows[flowName];
      if (num < 0 || num > 1000000)
      {
        this.m_encounteredFlows[flowName] = 0;
        num = 0;
      }
      return num;
    }

    public int QueryRoomEncountered(PrototypeDungeonRoom prototype)
    {
      return this.m_encounteredRooms.ContainsKey(prototype.GUID) ? this.m_encounteredRooms[prototype.GUID].encounterCount : 0;
    }

    public int QueryRoomEncountered(string GUID)
    {
      return string.IsNullOrEmpty(GUID) || !this.m_encounteredRooms.ContainsKey(GUID) ? 0 : this.m_encounteredRooms[GUID].encounterCount;
    }

    public int QueryRoomDifferentiator(PrototypeDungeonRoom prototype)
    {
      if (BraveRandom.IgnoreGenerationDifferentiator || string.IsNullOrEmpty(prototype.GUID) || !this.m_encounteredRooms.ContainsKey(prototype.GUID))
        return 0;
      int num = this.m_encounteredRooms[prototype.GUID].differentiator;
      if (num < 0 || num > 1000000)
      {
        this.m_encounteredRooms[prototype.GUID].differentiator = 0;
        num = 0;
      }
      return num;
    }

    public void ClearAllDifferentiatorHistory(bool doYouReallyWantToDoThis = false)
    {
      this.ClearAllPickupDifferentiators(doYouReallyWantToDoThis);
      this.ClearAllRoomDifferentiators();
    }

    public void ClearAllPickupDifferentiators(bool doYouReallyWantToDoThis = false)
    {
      if (!doYouReallyWantToDoThis)
        return;
      foreach (string key in this.m_encounteredTrackables.Keys)
        this.m_encounteredTrackables[key].differentiator = 0;
    }

    public void ClearAllEncounterableHistory(bool doYouReallyWantToDoThis = false)
    {
      if (!doYouReallyWantToDoThis)
        return;
      foreach (string key in this.m_encounteredTrackables.Keys)
        this.m_encounteredTrackables[key].differentiator = 0;
      this.m_encounteredTrackables.Clear();
    }

    public int QueryEncounterable(EncounterTrackable et)
    {
      return this.m_encounteredTrackables.ContainsKey(et.EncounterGuid) ? this.m_encounteredTrackables[et.EncounterGuid].encounterCount : 0;
    }

    public int QueryEncounterable(EncounterDatabaseEntry et)
    {
      return this.m_encounteredTrackables.ContainsKey(et.myGuid) ? this.m_encounteredTrackables[et.myGuid].encounterCount : 0;
    }

    public int QueryEncounterable(string encounterGuid)
    {
      return this.m_encounteredTrackables.ContainsKey(encounterGuid) ? this.m_encounteredTrackables[encounterGuid].encounterCount : 0;
    }

    public void SetEncounterableDifferentiator(EncounterTrackable et, int val)
    {
      if (et.IgnoreDifferentiator || !this.m_encounteredTrackables.ContainsKey(et.EncounterGuid))
        return;
      this.m_encounteredTrackables[et.EncounterGuid].differentiator = val;
    }

    public void MarkEncounterableAnnounced(EncounterDatabaseEntry et)
    {
      if (this.m_encounteredTrackables.ContainsKey(et.myGuid))
        this.m_encounteredTrackables[et.myGuid].hasBeenAmmonomiconAnnounced = true;
      else
        this.m_encounteredTrackables.Add(et.myGuid, new EncounteredObjectData()
        {
          hasBeenAmmonomiconAnnounced = true
        });
    }

    public bool QueryEncounterableAnnouncement(string guid)
    {
      return this.m_encounteredTrackables.ContainsKey(guid) && this.m_encounteredTrackables[guid].hasBeenAmmonomiconAnnounced;
    }

    public int QueryEncounterableDifferentiator(EncounterTrackable et)
    {
      return this.QueryEncounterableDifferentiator(et.EncounterGuid, et.IgnoreDifferentiator);
    }

    public int QueryEncounterableDifferentiator(EncounterDatabaseEntry encounterData)
    {
      return this.QueryEncounterableDifferentiator(encounterData.myGuid, encounterData.IgnoreDifferentiator);
    }

    public int QueryEncounterableDifferentiator(string encounterGuid, bool ignoreDifferentiator)
    {
      if (!this.m_encounteredTrackables.ContainsKey(encounterGuid) || ignoreDifferentiator)
        return 0;
      int num = this.m_encounteredTrackables[encounterGuid].differentiator;
      if (num < 0 || num > 1000000)
      {
        this.m_encounteredTrackables[encounterGuid].differentiator = 0;
        num = 0;
      }
      return num;
    }

    public void ClearAllRoomDifferentiators()
    {
      foreach (string key in this.m_encounteredRooms.Keys)
        this.m_encounteredRooms[key].differentiator = 0;
    }

    public void HandleEncounteredRoom(RuntimePrototypeRoomData prototype)
    {
      EncounteredObjectData encounteredObjectData1 = (EncounteredObjectData) null;
      if (prototype == null || string.IsNullOrEmpty(prototype.GUID) || GameManager.Instance.CurrentGameMode == GameManager.GameMode.BOSSRUSH || GameManager.Instance.CurrentGameMode == GameManager.GameMode.SUPERBOSSRUSH)
        return;
      if (this.m_encounteredRooms.ContainsKey(prototype.GUID))
      {
        encounteredObjectData1 = this.m_encounteredRooms[prototype.GUID];
      }
      else
      {
        EncounteredObjectData encounteredObjectData2 = new EncounteredObjectData();
        this.m_encounteredRooms.Add(prototype.GUID, encounteredObjectData2);
      }
      ++this.m_encounteredRooms[prototype.GUID].encounterCount;
      this.m_encounteredRooms[prototype.GUID].differentiator += 2;
    }

    public void HandleEncounteredObjectRaw(string encounterGuid)
    {
      EncounteredObjectData encounteredObjectData;
      if (this.m_encounteredTrackables.ContainsKey(encounterGuid))
      {
        encounteredObjectData = this.m_encounteredTrackables[encounterGuid];
      }
      else
      {
        encounteredObjectData = new EncounteredObjectData();
        this.m_encounteredTrackables.Add(encounterGuid, encounteredObjectData);
      }
      ++encounteredObjectData.encounterCount;
    }

    public void SingleIncrementDifferentiator(EncounterTrackable et)
    {
      EncounteredObjectData encounteredObjectData1 = (EncounteredObjectData) null;
      if (this.m_encounteredTrackables.ContainsKey(et.EncounterGuid))
      {
        encounteredObjectData1 = this.m_encounteredTrackables[et.EncounterGuid];
      }
      else
      {
        EncounteredObjectData encounteredObjectData2 = new EncounteredObjectData();
        this.m_encounteredTrackables.Add(et.EncounterGuid, encounteredObjectData2);
      }
      if ((bool) (UnityEngine.Object) et && !string.IsNullOrEmpty(et.EncounterGuid) && !this.m_singleProcessedEncounterTrackables.Contains(et.EncounterGuid))
        this.m_singleProcessedEncounterTrackables.Add(et.EncounterGuid);
      if (et.IgnoreDifferentiator)
        return;
      ++this.m_encounteredTrackables[et.EncounterGuid].differentiator;
    }

    public int GetNumberOfSynergiesEncounteredThisRun() => this.m_sessionSynergies.Count;

    public void HandleEncounteredSynergy(int index)
    {
      if (index <= 0)
        return;
      if (!this.m_encounteredSynergiesByID.ContainsKey(index))
        this.m_encounteredSynergiesByID.Add(index, 0);
      this.m_sessionSynergies.Add(index);
      this.m_encounteredSynergiesByID[index] = this.m_encounteredSynergiesByID[index] + 1;
    }

    public void HandleEncounteredObject(EncounterTrackable et)
    {
      EncounteredObjectData encounteredObjectData;
      if (this.m_encounteredTrackables.ContainsKey(et.EncounterGuid))
      {
        encounteredObjectData = this.m_encounteredTrackables[et.EncounterGuid];
      }
      else
      {
        encounteredObjectData = new EncounteredObjectData();
        this.m_encounteredTrackables.Add(et.EncounterGuid, encounteredObjectData);
      }
      ++encounteredObjectData.encounterCount;
      if (et.IgnoreDifferentiator)
        return;
      if (this.m_singleProcessedEncounterTrackables != null && this.m_singleProcessedEncounterTrackables.Contains(et.EncounterGuid))
        ++this.m_encounteredTrackables[et.EncounterGuid].differentiator;
      else
        this.m_encounteredTrackables[et.EncounterGuid].differentiator += 2;
    }

    public GlobalDungeonData.ValidTilesets GetCurrentRobotArmTileset()
    {
      switch (this.CurrentRobotArmFloor)
      {
        case 0:
          return (GlobalDungeonData.ValidTilesets) -1;
        case 1:
          return GlobalDungeonData.ValidTilesets.CASTLEGEON;
        case 2:
          return GlobalDungeonData.ValidTilesets.GUNGEON;
        case 3:
          return GlobalDungeonData.ValidTilesets.MINEGEON;
        case 4:
          return GlobalDungeonData.ValidTilesets.CATACOMBGEON;
        case 5:
          return GlobalDungeonData.ValidTilesets.FORGEGEON;
        default:
          return (GlobalDungeonData.ValidTilesets) -1;
      }
    }

    private PlayableCharacters GetCurrentCharacter()
    {
      return GameManager.Instance.PrimaryPlayer.characterIdentity;
    }

    public void HandleStatAchievements(TrackedStats stat)
    {
      if (stat == TrackedStats.GUNBERS_MUNCHED && (double) this.GetPlayerStatValue(stat) >= 20.0)
      {
        this.SetFlag(GungeonFlags.MUNCHER_MUTANT_ARM_UNLOCKED, true);
        this.SetFlag(GungeonFlags.MUNCHER_COLD45_UNLOCKED, true);
      }
      switch (stat)
      {
        case TrackedStats.WINCHESTER_GAMES_ACED:
          if ((double) this.GetPlayerStatValue(stat) < 3.0)
            break;
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.ACE_WINCHESTER_MULTI);
          break;
        case TrackedStats.META_CURRENCY_SPENT_AT_META_SHOP:
          if ((double) this.GetPlayerStatValue(stat) < 100.0)
            break;
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.SPEND_META_CURRENCY);
          break;
        case TrackedStats.GUNSLING_KING_CHALLENGES_COMPLETED:
          if ((double) this.GetPlayerStatValue(stat) < 5.0)
            break;
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.COMPLETE_GUNSLING_MULTI);
          break;
        case TrackedStats.TIMES_REACHED_SEWERS:
          if ((double) this.GetPlayerStatValue(stat) < 1.0)
            break;
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.REACH_SEWERS);
          break;
        case TrackedStats.TIMES_REACHED_CATHEDRAL:
          if ((double) this.GetPlayerStatValue(stat) < 1.0)
            break;
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.REACH_CATHEDRAL);
          break;
        case TrackedStats.MERCHANT_PURCHASES_GOOP:
          this.UpdateAllNpcsAchievement();
          break;
        case TrackedStats.MERCHANT_ITEMS_STOLEN:
          if ((double) this.GetPlayerStatValue(stat) < 10.0)
            break;
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.STEAL_MULTI);
          break;
        case TrackedStats.TABLES_FLIPPED:
          if ((double) this.GetPlayerStatValue(stat) < 500.0)
            break;
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.FLIP_TABLES_MULTI);
          break;
        case TrackedStats.ENEMIES_KILLED_WITH_CHANDELIERS:
          if ((double) this.GetPlayerStatValue(stat) < 100.0)
            break;
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.KILL_WITH_CHANDELIER_MULTI);
          break;
        case TrackedStats.ENEMIES_KILLED_WHILE_IN_CARTS:
          if ((double) this.GetPlayerStatValue(stat) < 100.0)
            break;
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.KILL_IN_MINE_CART_MULTI);
          break;
        case TrackedStats.ENEMIES_KILLED_WITH_PITS:
          if ((double) this.GetPlayerStatValue(stat) < 100.0)
            break;
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.KILL_WITH_PITS_MULTI);
          break;
        case TrackedStats.TIMES_CLEARED_CASTLE:
          if ((double) this.GetPlayerStatValue(stat) < 50.0)
            break;
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_FLOOR_ONE_MULTI);
          break;
        case TrackedStats.TIMES_CLEARED_GUNGEON:
          if ((double) this.GetPlayerStatValue(stat) < 40.0)
            break;
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_FLOOR_TWO_MULTI);
          break;
        case TrackedStats.TIMES_CLEARED_MINES:
          if ((double) this.GetPlayerStatValue(stat) < 30.0)
            break;
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_FLOOR_THREE_MULTI);
          break;
        case TrackedStats.TIMES_CLEARED_CATACOMBS:
          if ((double) this.GetPlayerStatValue(stat) < 20.0)
            break;
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_FLOOR_FOUR_MULTI);
          break;
        case TrackedStats.TIMES_CLEARED_FORGE:
          if ((double) this.GetPlayerStatValue(stat) < 10.0)
            break;
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_FLOOR_FIVE_MULTI);
          break;
      }
    }

    private PlatformStat? ConvertToPlatformStat(TrackedStats stat)
    {
      switch (stat)
      {
        case TrackedStats.WINCHESTER_GAMES_ACED:
          return new PlatformStat?(PlatformStat.WINCHESTER_ACED);
        case TrackedStats.META_CURRENCY_SPENT_AT_META_SHOP:
          return new PlatformStat?(PlatformStat.META_SPENT_AT_STORE);
        case TrackedStats.GUNSLING_KING_CHALLENGES_COMPLETED:
          return new PlatformStat?(PlatformStat.GUNSLING_COMPLETED);
        case TrackedStats.MERCHANT_ITEMS_STOLEN:
          return new PlatformStat?(PlatformStat.ITEMS_STOLEN);
        case TrackedStats.TABLES_FLIPPED:
          return new PlatformStat?(PlatformStat.TABLES_FLIPPED);
        case TrackedStats.ENEMIES_KILLED_WITH_CHANDELIERS:
          return new PlatformStat?(PlatformStat.CHANDELIER_KILLS);
        case TrackedStats.ENEMIES_KILLED_WHILE_IN_CARTS:
          return new PlatformStat?(PlatformStat.MINECART_KILLS);
        case TrackedStats.ENEMIES_KILLED_WITH_PITS:
          return new PlatformStat?(PlatformStat.PIT_KILLS);
        case TrackedStats.TIMES_CLEARED_CASTLE:
          return new PlatformStat?(PlatformStat.FLOOR_ONE_CLEARS);
        case TrackedStats.TIMES_CLEARED_GUNGEON:
          return new PlatformStat?(PlatformStat.FLOOR_TWO_CLEARS);
        case TrackedStats.TIMES_CLEARED_MINES:
          return new PlatformStat?(PlatformStat.FLOOR_THREE_CLEARS);
        case TrackedStats.TIMES_CLEARED_CATACOMBS:
          return new PlatformStat?(PlatformStat.FLOOR_FOUR_CLEARS);
        case TrackedStats.TIMES_CLEARED_FORGE:
          return new PlatformStat?(PlatformStat.FLOOR_FIVE_CLEARS);
        default:
          return new PlatformStat?();
      }
    }

    private void HandleSetPlatformStat(TrackedStats stat, float value)
    {
      PlatformStat? platformStat = this.ConvertToPlatformStat(stat);
      if (!platformStat.HasValue)
        return;
      GameManager.Instance.platformInterface.SetStat(platformStat.Value, Mathf.RoundToInt(value));
    }

    private void HandleIncrementPlatformStat(TrackedStats stat, float delta, float previousValue)
    {
      PlatformStat? platformStat = this.ConvertToPlatformStat(stat);
      if (!platformStat.HasValue)
        return;
      GameManager.Instance.platformInterface.SetStat(platformStat.Value, Mathf.RoundToInt(previousValue));
      GameManager.Instance.platformInterface.IncrementStat(platformStat.Value, Mathf.RoundToInt(delta));
    }

    public void HandleFlagAchievements(GungeonFlags flag)
    {
      if (flag == GungeonFlags.BOSSKILLED_ROGUE_PAST)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_PAST_ROGUE);
      else if (flag == GungeonFlags.BOSSKILLED_CONVICT_PAST)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_PAST_CONVICT);
      else if (flag == GungeonFlags.BOSSKILLED_SOLDIER_PAST)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_PAST_MARINE);
      else if (flag == GungeonFlags.BOSSKILLED_GUIDE_PAST)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_PAST_GUIDE);
      else if (flag == GungeonFlags.BOSSKILLED_ROBOT_PAST)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_PAST_ROBOT);
      else if (flag == GungeonFlags.BOSSKILLED_BULLET_PAST)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_PAST_BULLET);
      else if (flag == GungeonFlags.BOSSKILLED_DRAGUN)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_FLOOR_FIVE);
      else if (flag == GungeonFlags.BOSSKILLED_LICH)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_FLOOR_SIX);
      else if (flag == GungeonFlags.BOSSKILLED_HIGHDRAGUN)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_ADVANCED_DRAGUN);
      else if (flag == GungeonFlags.BOSSKILLED_RESOURCEFULRAT)
      {
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_RESOURCEFUL_RAT);
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_METAL_GEAR_RAT);
      }
      if (flag == GungeonFlags.BLACKSMITH_BULLET_COMPLETE)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BUILD_BULLET);
      else if (flag == GungeonFlags.LOST_ADVENTURER_ACHIEVEMENT_REWARD_GIVEN)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.MAP_MAIN_FLOORS);
      else if (flag == GungeonFlags.META_SHOP_RECEIVED_ROBOT_ARM_REWARD)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.COMPLETE_GOLEM_ARM);
      else if (flag == GungeonFlags.FRIFLE_CORE_HUNTS_COMPLETE)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.COMPLETE_FRIFLE_MULTI);
      else if (flag == GungeonFlags.TUTORIAL_KILLED_MANFREDS_RIVAL)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_MANFREDS_RIVAL);
      else if (flag == GungeonFlags.TUTORIAL_COMPLETED)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_TUTORIAL);
      else if (flag == GungeonFlags.DAISUKE_CHALLENGE_COMPLETE)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.COMPLETE_GAME_WITH_CHALLENGE_MODE);
      else if (flag == GungeonFlags.BOSSKILLED_DRAGUN_TURBO_MODE)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.COMPLETE_GAME_WITH_TURBO_MODE);
      if (flag == GungeonFlags.SECRET_BULLETMAN_SEEN_05)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.UNLOCK_BULLET);
      else if (flag == GungeonFlags.BLACKSMITH_RECEIVED_BUSTED_TELEVISION)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.UNLOCK_ROBOT);
      if (flag == GungeonFlags.SHERPA_UNLOCK1_COMPLETE)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.UNLOCK_FLOOR_TWO_SHORTCUT);
      else if (flag == GungeonFlags.SHERPA_UNLOCK2_COMPLETE)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.UNLOCK_FLOOR_THREE_SHORTCUT);
      else if (flag == GungeonFlags.SHERPA_UNLOCK3_COMPLETE)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.UNLOCK_FLOOR_FOUR_SHORTCUT);
      else if (flag == GungeonFlags.SHERPA_UNLOCK4_COMPLETE)
        GameManager.Instance.platformInterface.AchievementUnlock(Achievement.UNLOCK_FLOOR_FIVE_SHORTCUT);
      if (GameStatsManager.s_pastFlags.Contains(flag))
      {
        bool flag1 = true;
        for (int index = 0; index < GameStatsManager.s_pastFlags.Count; ++index)
        {
          if (!this.GetFlag(GameStatsManager.s_pastFlags[index]))
          {
            flag1 = false;
            break;
          }
        }
        if (flag1)
          GameManager.Instance.platformInterface.AchievementUnlock(Achievement.BEAT_PAST_ALL);
      }
      if (GameStatsManager.s_npcFoyerFlags.Contains(flag))
        this.UpdateAllNpcsAchievement();
      if (GameStatsManager.s_frifleHuntFlags.Contains(flag))
        this.UpdateFrifleHuntAchievement();
      if (flag != GungeonFlags.LOST_ADVENTURER_HELPED_CASTLE && flag != GungeonFlags.LOST_ADVENTURER_HELPED_GUNGEON && flag != GungeonFlags.LOST_ADVENTURER_HELPED_MINES && flag != GungeonFlags.LOST_ADVENTURER_HELPED_CATACOMBS && flag != GungeonFlags.LOST_ADVENTURER_HELPED_FORGE)
        return;
      GameManager.Instance.platformInterface.SetStat(PlatformStat.MAIN_FLOORS_MAPPED, 0 + (!this.GetFlag(GungeonFlags.LOST_ADVENTURER_HELPED_CASTLE) ? 0 : 1) + (!this.GetFlag(GungeonFlags.LOST_ADVENTURER_HELPED_GUNGEON) ? 0 : 1) + (!this.GetFlag(GungeonFlags.LOST_ADVENTURER_HELPED_MINES) ? 0 : 1) + (!this.GetFlag(GungeonFlags.LOST_ADVENTURER_HELPED_CATACOMBS) ? 0 : 1) + (!this.GetFlag(GungeonFlags.LOST_ADVENTURER_HELPED_FORGE) ? 0 : 1));
    }

    private void UpdateAllNpcsAchievement()
    {
      bool flag = true;
      int num = 0;
      for (int index = 0; index < GameStatsManager.s_npcFoyerFlags.Count; ++index)
      {
        if (this.GetFlag(GameStatsManager.s_npcFoyerFlags[index]))
          ++num;
        else
          flag = false;
      }
      if (this.GetFlag(GungeonFlags.TUTORIAL_TALKED_AFTER_RIVAL_KILLED))
        ++num;
      if (this.GetFlag(GungeonFlags.SORCERESS_ACTIVE_IN_FOYER))
        this.SetFlag(GungeonFlags.DAISUKE_IS_UNLOCKABLE, true);
      if ((double) this.GetPlayerStatValue(TrackedStats.MERCHANT_PURCHASES_GOOP) >= 1.0)
        ++num;
      else
        flag = false;
      GameManager.Instance.platformInterface.SetStat(PlatformStat.BREACH_POPULATION, num);
      if (!flag)
        return;
      GameManager.Instance.platformInterface.AchievementUnlock(Achievement.POPULATE_BREACH);
    }

    private void UpdateFrifleHuntAchievement()
    {
      bool flag = true;
      int num = 0;
      for (int index = 0; index < GameStatsManager.s_frifleHuntFlags.Count; ++index)
      {
        if (this.GetFlag(GameStatsManager.s_frifleHuntFlags[index]))
          ++num;
        else
          flag = false;
      }
      GameManager.Instance.platformInterface.SetStat(PlatformStat.FRIFLE_CORE_COMPLETED, num);
      if (!flag)
        return;
      GameManager.Instance.platformInterface.AchievementUnlock(Achievement.COMPLETE_FRIFLE_MULTI);
    }
  }

