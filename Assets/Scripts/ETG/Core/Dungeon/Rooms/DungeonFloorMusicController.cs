// Decompiled with JetBrains decompiler
// Type: DungeonFloorMusicController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

public class DungeonFloorMusicController : MonoBehaviour
  {
    private DungeonFloorMusicController.DungeonMusicState m_currentState;
    private float m_cooldownTimerRemaining = -1f;
    private float COOLDOWN_TIMER = 22.5f;
    private float MUSIC_CHANGE_TIMER = 40f;
    private float m_lastMusicChangeTime;
    private string m_cachedMusicEventCore = string.Empty;
    private bool m_overrideMusic;
    private bool m_isVictoryState;
    private float m_changedToArcadeTimer = -1f;
    private uint m_coreMusicEventID;

    public DungeonFloorMusicController.DungeonMusicState CurrentState => this.m_currentState;

    public bool MusicOverridden => this.m_overrideMusic;

    public bool ShouldPulseLightFX => this.m_overrideMusic && !this.m_isVictoryState;

    private void LateUpdate()
    {
      if ((double) this.m_cooldownTimerRemaining > 0.0)
      {
        this.m_cooldownTimerRemaining -= BraveTime.DeltaTime;
        if ((double) this.m_cooldownTimerRemaining <= 0.0)
        {
          this.SwitchToState(DungeonFloorMusicController.DungeonMusicState.CALM);
          this.m_cooldownTimerRemaining = -1f;
        }
      }
      if ((double) this.m_changedToArcadeTimer <= 0.0)
        return;
      this.m_changedToArcadeTimer -= BraveTime.DeltaTime;
    }

    public void ClearCoreMusicEventID()
    {
      Debug.Log((object) "Clearing Core Music ID!");
      this.m_lastMusicChangeTime = -1000f;
      this.m_overrideMusic = false;
      this.m_isVictoryState = false;
      this.m_coreMusicEventID = 0U;
    }

    public void OnAkMusicEvent(object cookie, AkCallbackType eventType, object info)
    {
      if (eventType == AkCallbackType.AK_Starvation)
      {
        Debug.Log((object) $"Core music event: {this.m_cachedMusicEventCore} STARVING with playing ID: {(object) this.m_coreMusicEventID}");
      }
      else
      {
        if (eventType != AkCallbackType.AK_EndOfEvent)
          return;
        Debug.Log((object) $"Core music event: {this.m_cachedMusicEventCore} ENDING with playing ID: {(object) this.m_coreMusicEventID}");
      }
    }

    public void ResetForNewFloor(Dungeonator.Dungeon d)
    {
      this.m_overrideMusic = false;
      this.m_isVictoryState = false;
      this.m_lastMusicChangeTime = -1000f;
      GameManager.Instance.FlushMusicAudio();
      this.m_cachedMusicEventCore = string.IsNullOrEmpty(d.musicEventName) ? "Play_MUS_Dungeon_Theme_01" : d.musicEventName;
      this.m_coreMusicEventID = AkSoundEngine.PostEvent(this.m_cachedMusicEventCore, GameManager.Instance.gameObject, 33U, new AkCallbackManager.EventCallback(this.OnAkMusicEvent), (object) null);
      Debug.LogWarning((object) $"Posting core music event: {this.m_cachedMusicEventCore} with playing ID: {(object) this.m_coreMusicEventID}");
      if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST && GameManager.Instance.PrimaryPlayer.characterIdentity != PlayableCharacters.Bullet)
      {
        this.m_overrideMusic = true;
        int num = (int) AkSoundEngine.PostEvent("Play_MUS_Ending_State_01", GameManager.Instance.gameObject);
      }
      else
        this.SwitchToState(DungeonFloorMusicController.DungeonMusicState.FLOOR_INTRO);
      if (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.FOYER || !GameStatsManager.Instance.AnyPastBeaten())
        return;
      int num1 = (int) AkSoundEngine.PostEvent("Play_MUS_State_Winner", GameManager.Instance.gameObject);
    }

    public void UpdateCoreMusicEvent()
    {
      if (this.m_coreMusicEventID != 0U)
        return;
      this.ResetForNewFloor(GameManager.Instance.Dungeon);
    }

    public void SwitchToArcadeMusic()
    {
      this.m_changedToArcadeTimer = 0.1f;
      this.SwitchToState(DungeonFloorMusicController.DungeonMusicState.ARCADE);
    }

    public void StartArcadeGame()
    {
      int num = (int) AkSoundEngine.PostEvent("Play_MUS_Winchester_state_Game", this.gameObject);
    }

    public void SwitchToCustomMusic(
      string customMusicEvent,
      GameObject source,
      bool useSwitch,
      string switchEvent)
    {
      this.m_cooldownTimerRemaining = -1f;
      int num1 = (int) AkSoundEngine.PostEvent(customMusicEvent, source);
      if (useSwitch)
      {
        int num2 = (int) AkSoundEngine.PostEvent(switchEvent, source);
      }
      this.m_currentState = ~DungeonFloorMusicController.DungeonMusicState.FLOOR_INTRO;
    }

    public void SwitchToVictoryMusic()
    {
      this.m_cooldownTimerRemaining = -1f;
      int num1 = (int) AkSoundEngine.PostEvent("Stop_MUS_All", this.gameObject);
      int num2 = (int) AkSoundEngine.PostEvent("Play_MUS_Victory_Theme_01", this.gameObject);
      this.m_overrideMusic = true;
      this.m_isVictoryState = true;
    }

    public void SwitchToEndTimesMusic()
    {
      this.m_cooldownTimerRemaining = -1f;
      int num1 = (int) AkSoundEngine.PostEvent("Stop_MUS_All", this.gameObject);
      int num2 = (int) AkSoundEngine.PostEvent("Play_MUS_Space_Intro_01", this.gameObject);
      this.m_overrideMusic = true;
      this.m_isVictoryState = false;
    }

    public void SwitchToDragunTwo()
    {
      this.m_cooldownTimerRemaining = -1f;
      int num1 = (int) AkSoundEngine.PostEvent("Stop_MUS_All", GameManager.Instance.gameObject);
      int num2 = (int) AkSoundEngine.PostEvent("Play_MUS_Boss_Theme_Dragun_02", GameManager.Instance.gameObject);
      this.m_overrideMusic = true;
    }

    public void SwitchToBossMusic(string bossMusicString, GameObject source)
    {
      bool flag = (((GameManager.Instance.CurrentGameMode == GameManager.GameMode.BOSSRUSH || GameManager.Instance.CurrentGameMode == GameManager.GameMode.SUPERBOSSRUSH) | GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.MINEGEON ? 1 : 0) | (GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.CHARACTER_PAST ? 0 : (GameManager.Instance.PrimaryPlayer.characterIdentity == PlayableCharacters.Bullet ? 1 : 0))) != 0;
      if (this.m_isVictoryState)
        this.EndVictoryMusic();
      if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST && GameManager.Instance.PrimaryPlayer.characterIdentity == PlayableCharacters.Convict)
        return;
      this.m_cooldownTimerRemaining = -1f;
      int num1 = (int) AkSoundEngine.PostEvent("Stop_MUS_All", source);
      int num2 = (int) AkSoundEngine.PostEvent(bossMusicString, source);
      this.m_overrideMusic = true;
    }

    public void EndBossMusic()
    {
      int num1 = (int) AkSoundEngine.PostEvent("Stop_MUS_Boss_Theme", this.gameObject);
      this.m_overrideMusic = false;
      int num2 = (int) AkSoundEngine.PostEvent("Play_MUS_Victory_Theme_01", this.gameObject);
      this.m_isVictoryState = true;
    }

    public void EndBossMusicNoVictory()
    {
      int num1 = (int) AkSoundEngine.PostEvent("Stop_MUS_Boss_Theme", this.gameObject);
      int num2 = (int) AkSoundEngine.PostEvent(this.m_cachedMusicEventCore, this.gameObject);
      this.m_overrideMusic = false;
      this.SwitchToState(DungeonFloorMusicController.DungeonMusicState.CALM);
    }

    public void EndVictoryMusic()
    {
      this.m_overrideMusic = false;
      this.m_isVictoryState = false;
      int num1 = (int) AkSoundEngine.PostEvent("Stop_MUS_All", GameManager.Instance.gameObject);
      int num2 = (int) AkSoundEngine.PostEvent(this.m_cachedMusicEventCore, GameManager.Instance.gameObject);
    }

    private void SwitchToState(
      DungeonFloorMusicController.DungeonMusicState targetState)
    {
      if ((double) this.m_changedToArcadeTimer > 0.0 && targetState == DungeonFloorMusicController.DungeonMusicState.CALM && this.m_currentState == DungeonFloorMusicController.DungeonMusicState.ARCADE)
        return;
      Debug.Log((object) $"Attemping to switch to state: {targetState.ToString()} with core ID: {(object) this.m_coreMusicEventID}");
      if (this.m_overrideMusic)
        return;
      switch (targetState)
      {
        case DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_B:
          int num1 = (int) AkSoundEngine.PostEvent("Play_MUS_Dungeon_State_LoopB", GameManager.Instance.gameObject);
          break;
        case DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_C:
          int num2 = (int) AkSoundEngine.PostEvent("Play_MUS_Dungeon_State_LoopC", GameManager.Instance.gameObject);
          break;
        case DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_D:
          int num3 = (int) AkSoundEngine.PostEvent("Play_MUS_Dungeon_State_LoopD", GameManager.Instance.gameObject);
          break;
        default:
          if (targetState != DungeonFloorMusicController.DungeonMusicState.FLOOR_INTRO)
          {
            if (targetState != DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_A)
            {
              if (targetState != DungeonFloorMusicController.DungeonMusicState.CALM)
              {
                if (targetState != DungeonFloorMusicController.DungeonMusicState.SHOP)
                {
                  if (targetState != DungeonFloorMusicController.DungeonMusicState.SECRET)
                  {
                    if (targetState != DungeonFloorMusicController.DungeonMusicState.ARCADE)
                    {
                      if (targetState != DungeonFloorMusicController.DungeonMusicState.FOYER_ELEVATOR)
                      {
                        if (targetState == DungeonFloorMusicController.DungeonMusicState.FOYER_SORCERESS)
                        {
                          this.m_cooldownTimerRemaining = -1f;
                          int num4 = (int) AkSoundEngine.PostEvent("Play_MUS_State_Sorceress", GameManager.Instance.gameObject);
                          break;
                        }
                        break;
                      }
                      this.m_cooldownTimerRemaining = -1f;
                      int num5 = (int) AkSoundEngine.PostEvent("Play_MUS_State_Elevator", GameManager.Instance.gameObject);
                      break;
                    }
                    this.m_cooldownTimerRemaining = -1f;
                    int num6 = (int) AkSoundEngine.PostEvent("Play_MUS_Dungeon_State_Winchester", GameManager.Instance.gameObject);
                    int num7 = (int) AkSoundEngine.PostEvent("Play_MUS_Winchester_State_Drone", this.gameObject);
                    break;
                  }
                  this.m_cooldownTimerRemaining = -1f;
                  int num8 = (int) AkSoundEngine.PostEvent("Play_MUS_Dungeon_State_Secret", GameManager.Instance.gameObject);
                  break;
                }
                this.m_cooldownTimerRemaining = -1f;
                int num9 = (int) AkSoundEngine.PostEvent("Play_MUS_Dungeon_State_Shop", GameManager.Instance.gameObject);
                break;
              }
              this.m_cooldownTimerRemaining = -1f;
              if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER && GameStatsManager.Instance.AnyPastBeaten())
              {
                int num10 = (int) AkSoundEngine.PostEvent("Play_MUS_Dungeon_State_Winner", GameManager.Instance.gameObject);
                break;
              }
              int num11 = (int) AkSoundEngine.PostEvent("Play_MUS_Dungeon_State_Drone", GameManager.Instance.gameObject);
              break;
            }
            int num12 = (int) AkSoundEngine.PostEvent("Play_MUS_Dungeon_State_LoopA", GameManager.Instance.gameObject);
            break;
          }
          this.m_cooldownTimerRemaining = -1f;
          int num13 = (int) AkSoundEngine.PostEvent("Play_MUS_Dungeon_State_Intro", GameManager.Instance.gameObject);
          break;
      }
      Debug.Log((object) ("Successfully switched to state: " + targetState.ToString()));
      this.m_currentState = targetState;
    }

    public void NotifyRoomSuddenlyHasEnemies(RoomHandler newRoom)
    {
      if (!newRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear))
        return;
      this.m_cooldownTimerRemaining = -1f;
      if (this.m_currentState != DungeonFloorMusicController.DungeonMusicState.FLOOR_INTRO && this.m_currentState != DungeonFloorMusicController.DungeonMusicState.CALM && this.m_currentState != DungeonFloorMusicController.DungeonMusicState.SHOP)
        return;
      this.SwitchToActiveMusic(new DungeonFloorMusicController.DungeonMusicState?());
    }

    public void SwitchToActiveMusic(
      DungeonFloorMusicController.DungeonMusicState? excludedState)
    {
      if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.CASTLEGEON && GameManager.Instance.CurrentLevelOverrideState != GameManager.LevelOverrideState.TUTORIAL)
      {
        if (GameManager.Instance.RandomIntForCurrentRun % 4 == 1)
        {
          if (excludedState.HasValue)
          {
            if (excludedState.Value == DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_C)
              this.SwitchToState(DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_D);
            else if (excludedState.Value == DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_D)
              this.SwitchToState(DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_C);
            else
              this.SwitchToState((double) Random.value >= 0.5 ? DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_D : DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_C);
          }
          else
            this.SwitchToState((double) Random.value >= 0.5 ? DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_D : DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_C);
        }
        else if (excludedState.HasValue)
        {
          if (excludedState.Value == DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_A)
            this.SwitchToState(DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_B);
          else if (excludedState.Value == DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_B)
            this.SwitchToState(DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_A);
          else
            this.SwitchToState((double) Random.value >= 0.5 ? DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_B : DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_A);
        }
        else
          this.SwitchToState((double) Random.value >= 0.5 ? DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_B : DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_A);
      }
      else
      {
        this.m_lastMusicChangeTime = UnityEngine.Time.realtimeSinceStartup;
        if (excludedState.HasValue && excludedState.Value == DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_A)
          this.SwitchToState(DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_B);
        else if (excludedState.HasValue && excludedState.Value == DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_B)
          this.SwitchToState(DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_A);
        else
          this.SwitchToState((double) Random.value <= 0.5 ? DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_B : DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_A);
      }
    }

    public void NotifyEnteredNewRoom(RoomHandler newRoom)
    {
      this.UpdateCoreMusicEvent();
      if (GameManager.Instance.Dungeon.tileIndices.tilesetId == GlobalDungeonData.ValidTilesets.OFFICEGEON)
      {
        if (newRoom != null && (newRoom.RoomVisualSubtype == 7 || newRoom.RoomVisualSubtype == 8))
        {
          if (this.m_cachedMusicEventCore != "Play_MUS_Space_Theme_01")
          {
            int num1 = (int) AkSoundEngine.PostEvent("Stop_MUS_All", this.gameObject);
            this.m_currentState = DungeonFloorMusicController.DungeonMusicState.FLOOR_INTRO;
            this.m_cachedMusicEventCore = "Play_MUS_Space_Theme_01";
            int num2 = (int) AkSoundEngine.PostEvent("Play_MUS_Space_Theme_01", GameManager.Instance.gameObject);
          }
        }
        else if (this.m_cachedMusicEventCore != "Play_MUS_Office_Theme_01")
        {
          int num3 = (int) AkSoundEngine.PostEvent("Stop_MUS_All", this.gameObject);
          this.m_currentState = DungeonFloorMusicController.DungeonMusicState.FLOOR_INTRO;
          this.m_cachedMusicEventCore = "Play_MUS_Office_Theme_01";
          int num4 = (int) AkSoundEngine.PostEvent("Play_MUS_Office_Theme_01", GameManager.Instance.gameObject);
        }
      }
      if (newRoom.area != null && newRoom.area.runtimePrototypeData != null && newRoom.area.runtimePrototypeData.UsesCustomMusic && !string.IsNullOrEmpty(newRoom.area.runtimePrototypeData.CustomMusicEvent))
        this.SwitchToCustomMusic(newRoom.area.runtimePrototypeData.CustomMusicEvent, this.gameObject, newRoom.area.runtimePrototypeData.UsesCustomSwitch, newRoom.area.runtimePrototypeData.CustomMusicSwitch);
      else if (newRoom.area != null && newRoom.area.runtimePrototypeData != null && newRoom.area.runtimePrototypeData.UsesCustomMusicState)
      {
        this.m_cooldownTimerRemaining = -1f;
        this.SwitchToState(newRoom.area.runtimePrototypeData.CustomMusicState);
      }
      else if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.FOYER)
      {
        this.m_cooldownTimerRemaining = -1f;
        this.SwitchToState(DungeonFloorMusicController.DungeonMusicState.CALM);
      }
      else if (newRoom.IsShop && GameManager.Instance.Dungeon.tileIndices.tilesetId != GlobalDungeonData.ValidTilesets.FORGEGEON)
      {
        this.m_lastMusicChangeTime = UnityEngine.Time.realtimeSinceStartup;
        this.SwitchToState(DungeonFloorMusicController.DungeonMusicState.SHOP);
      }
      else if (newRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear))
      {
        this.m_cooldownTimerRemaining = -1f;
        if (this.m_isVictoryState || this.m_currentState == DungeonFloorMusicController.DungeonMusicState.FLOOR_INTRO || this.m_currentState == DungeonFloorMusicController.DungeonMusicState.CALM || this.m_currentState == DungeonFloorMusicController.DungeonMusicState.SHOP || this.m_currentState == DungeonFloorMusicController.DungeonMusicState.SECRET || this.m_currentState == DungeonFloorMusicController.DungeonMusicState.ARCADE || this.m_currentState == ~DungeonFloorMusicController.DungeonMusicState.FLOOR_INTRO)
        {
          if (this.m_isVictoryState)
            this.EndVictoryMusic();
          this.SwitchToActiveMusic(new DungeonFloorMusicController.DungeonMusicState?());
        }
        else if (this.m_currentState == DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_A)
        {
          if ((double) Random.value <= 0.5 || (double) UnityEngine.Time.realtimeSinceStartup - (double) this.m_lastMusicChangeTime <= (double) this.MUSIC_CHANGE_TIMER)
            return;
          this.m_lastMusicChangeTime = UnityEngine.Time.realtimeSinceStartup;
          this.SwitchToActiveMusic(new DungeonFloorMusicController.DungeonMusicState?(DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_A));
        }
        else
        {
          if (this.m_currentState != DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_B || (double) Random.value <= 0.5 || (double) UnityEngine.Time.realtimeSinceStartup - (double) this.m_lastMusicChangeTime <= (double) this.MUSIC_CHANGE_TIMER)
            return;
          this.m_lastMusicChangeTime = UnityEngine.Time.realtimeSinceStartup;
          this.SwitchToActiveMusic(new DungeonFloorMusicController.DungeonMusicState?(DungeonFloorMusicController.DungeonMusicState.ACTIVE_SIDE_B));
        }
      }
      else if (newRoom.WasEverSecretRoom)
      {
        this.SwitchToState(DungeonFloorMusicController.DungeonMusicState.SECRET);
      }
      else
      {
        if (this.m_currentState != DungeonFloorMusicController.DungeonMusicState.SHOP && this.m_currentState != DungeonFloorMusicController.DungeonMusicState.ARCADE && this.m_currentState != DungeonFloorMusicController.DungeonMusicState.SECRET && this.m_currentState != DungeonFloorMusicController.DungeonMusicState.FOYER_ELEVATOR && this.m_currentState != DungeonFloorMusicController.DungeonMusicState.FOYER_SORCERESS)
          return;
        this.SwitchToState(DungeonFloorMusicController.DungeonMusicState.CALM);
      }
    }

    public void NotifyCurrentRoomEnemiesCleared()
    {
      this.m_cooldownTimerRemaining = this.COOLDOWN_TIMER;
    }

    public enum DungeonMusicState
    {
      FLOOR_INTRO = 0,
      ACTIVE_SIDE_A = 10, // 0x0000000A
      ACTIVE_SIDE_B = 20, // 0x00000014
      ACTIVE_SIDE_C = 23, // 0x00000017
      ACTIVE_SIDE_D = 25, // 0x00000019
      CALM = 30, // 0x0000001E
      SHOP = 40, // 0x00000028
      SECRET = 50, // 0x00000032
      ARCADE = 60, // 0x0000003C
      FOYER_ELEVATOR = 70, // 0x00000046
      FOYER_SORCERESS = 75, // 0x0000004B
    }
  }

