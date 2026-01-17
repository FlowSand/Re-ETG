// Decompiled with JetBrains decompiler
// Type: ChallengeManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Management
{
    public class ChallengeManager : MonoBehaviour
    {
      public static bool CHALLENGE_MODE_ACTIVE;
      private static ChallengeManager m_instance;
      public ChallengeModeType ChallengeMode = ChallengeModeType.ChallengeMode;
      [NonSerialized]
      public RoomHandler GunslingTargetRoom;
      public string ChallengeInSFX = "Play_UI_menu_pause_01";
      public dfAnimationClip ChallengeBurstClip;
      public List<ChallengeFloorData> FloorData = new List<ChallengeFloorData>();
      [Header("Remember the other _ChallengeManager too!")]
      public List<ChallengeDataEntry> PossibleChallenges = new List<ChallengeDataEntry>();
      public List<BossChallengeData> BossChallenges = new List<BossChallengeData>();
      private List<ChallengeModifier> m_activeChallenges = new List<ChallengeModifier>();
      private PlayerController m_primaryPlayer;
      private bool m_init;

      public static ChallengeManager Instance
      {
        get
        {
          if (!(bool) (UnityEngine.Object) ChallengeManager.m_instance)
            ChallengeManager.m_instance = UnityEngine.Object.FindObjectOfType<ChallengeManager>();
          return ChallengeManager.m_instance;
        }
      }

      public static ChallengeModeType ChallengeModeType
      {
        get
        {
          ChallengeManager instance = ChallengeManager.Instance;
          return (bool) (UnityEngine.Object) instance ? instance.ChallengeMode : ChallengeModeType.None;
        }
        set
        {
          ChallengeManager instance = ChallengeManager.Instance;
          if ((bool) (UnityEngine.Object) instance)
          {
            if (instance.ChallengeMode == value)
              return;
            UnityEngine.Object.Destroy((UnityEngine.Object) instance.gameObject);
          }
          switch (value)
          {
            case ChallengeModeType.ChallengeMode:
              UnityEngine.Object.Instantiate<GameObject>((GameObject) BraveResources.Load("Global Prefabs/_ChallengeManager"));
              break;
            case ChallengeModeType.ChallengeMegaMode:
              UnityEngine.Object.Instantiate<GameObject>((GameObject) BraveResources.Load("Global Prefabs/_ChallengeMegaManager"));
              break;
            case ChallengeModeType.GunslingKingTemporary:
              UnityEngine.Object.Instantiate<GameObject>((GameObject) BraveResources.Load("Global Prefabs/_ChallengeManager")).GetComponent<ChallengeManager>().ChallengeMode = ChallengeModeType.GunslingKingTemporary;
              break;
          }
        }
      }

      public List<ChallengeModifier> ActiveChallenges => this.m_activeChallenges;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ChallengeManager.<Start>c__Iterator0()
        {
          $this = this
        };
      }

      private void Update()
      {
        if (this.m_activeChallenges.Count > 0 && !this.m_primaryPlayer.IsInCombat)
          this.CleanupChallenges();
        if (!this.m_init || !GameManager.Instance.IsFoyer || !((UnityEngine.Object) this.m_primaryPlayer != (UnityEngine.Object) GameManager.Instance.PrimaryPlayer))
          return;
        if ((bool) (UnityEngine.Object) this.m_primaryPlayer)
          this.m_primaryPlayer.OnEnteredCombat -= new System.Action(this.EnteredCombat);
        this.m_primaryPlayer = GameManager.Instance.PrimaryPlayer;
        this.m_primaryPlayer.OnEnteredCombat += new System.Action(this.EnteredCombat);
      }

      private void OnDestroy()
      {
        this.CleanupChallenges();
        if ((bool) (UnityEngine.Object) this.m_primaryPlayer)
          this.m_primaryPlayer.OnEnteredCombat -= new System.Action(this.EnteredCombat);
        if (!((UnityEngine.Object) ChallengeManager.m_instance == (UnityEngine.Object) this))
          return;
        ChallengeManager.m_instance = (ChallengeManager) null;
        ChallengeManager.CHALLENGE_MODE_ACTIVE = false;
      }

      private ChallengeFloorData GetFloorData(GlobalDungeonData.ValidTilesets tilesetID)
      {
        for (int index = 0; index < this.FloorData.Count; ++index)
        {
          if (this.FloorData[index].floorID == tilesetID)
            return this.FloorData[index];
        }
        return (ChallengeFloorData) null;
      }

      public bool SuppressChallengeStart { get; set; }

      public void EnteredCombat()
      {
        if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.CHARACTER_PAST || GameManager.Instance.InTutorial || this.SuppressChallengeStart || this.ChallengeMode == ChallengeModeType.GunslingKingTemporary && this.GunslingTargetRoom != null && GameManager.Instance.PrimaryPlayer.CurrentRoom != this.GunslingTargetRoom)
          return;
        this.CleanupChallenges();
        ChallengeFloorData floorData = this.GetFloorData(GameManager.Instance.Dungeon.tileIndices.tilesetId);
        int numToAdd;
        if (floorData != null)
        {
          numToAdd = UnityEngine.Random.Range(floorData.minChallenges, floorData.maxChallenges + 1);
        }
        else
        {
          switch (GameManager.Instance.Dungeon.tileIndices.tilesetId)
          {
            case GlobalDungeonData.ValidTilesets.GUNGEON:
              numToAdd = 2;
              break;
            case GlobalDungeonData.ValidTilesets.CASTLEGEON:
              numToAdd = 1;
              break;
            case GlobalDungeonData.ValidTilesets.MINEGEON:
              numToAdd = 3;
              break;
            case GlobalDungeonData.ValidTilesets.CATACOMBGEON:
              numToAdd = 4;
              break;
            case GlobalDungeonData.ValidTilesets.FORGEGEON:
              numToAdd = 5;
              break;
            default:
              numToAdd = 1;
              break;
          }
        }
        this.TriggerNewChallenges(numToAdd);
        this.StartCoroutine(this.HandleNewChallengeAnnouncements());
      }

      [DebuggerHidden]
      private IEnumerator HandleNewChallengeAnnouncements()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ChallengeManager.<HandleNewChallengeAnnouncements>c__Iterator1()
        {
          $this = this
        };
      }

      private void TriggerNewChallenges(int numToAdd)
      {
        if (GameManager.Instance.InTutorial)
          return;
        RoomHandler currentRoom = GameManager.Instance.PrimaryPlayer.CurrentRoom;
        if (currentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS)
        {
          BossChallengeData bossChallengeData = (BossChallengeData) null;
          List<AIActor> activeEnemies = currentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
          for (int index1 = 0; index1 < activeEnemies.Count; ++index1)
          {
            if ((bool) (UnityEngine.Object) activeEnemies[index1] && (bool) (UnityEngine.Object) activeEnemies[index1].healthHaver && activeEnemies[index1].healthHaver.IsBoss)
            {
              for (int index2 = 0; index2 < this.BossChallenges.Count; ++index2)
              {
                BossChallengeData bossChallenge = this.BossChallenges[index2];
                for (int index3 = 0; index3 < bossChallenge.BossGuids.Length; ++index3)
                {
                  if (bossChallenge.BossGuids[index3] == activeEnemies[index1].EnemyGuid)
                  {
                    bossChallengeData = bossChallenge;
                    break;
                  }
                }
              }
            }
          }
          if (bossChallengeData != null)
          {
            numToAdd = bossChallengeData.NumToSelect;
            int num = 0;
            while (this.m_activeChallenges.Count < numToAdd && num < 10000)
            {
              ++num;
              ChallengeModifier modifier = bossChallengeData.Modifiers[UnityEngine.Random.Range(0, bossChallengeData.Modifiers.Length)];
              bool flag = modifier.IsValid(currentRoom);
              for (int index = 0; index < this.m_activeChallenges.Count; ++index)
              {
                if (flag && this.m_activeChallenges[index].DisplayName == modifier.DisplayName)
                  flag = false;
                if (flag && this.m_activeChallenges[index].MutuallyExclusive.Contains(modifier))
                  flag = false;
              }
              if (flag)
                this.m_activeChallenges.Add(UnityEngine.Object.Instantiate<GameObject>(modifier.gameObject).GetComponent<ChallengeModifier>());
            }
          }
        }
        if (this.m_activeChallenges.Count != 0)
          return;
        int num1 = numToAdd;
        int num2 = 0;
        GlobalDungeonData.ValidTilesets tilesetId = GameManager.Instance.Dungeon.tileIndices.tilesetId;
        while (num1 > 0 && num2 < 10000)
        {
          ++num2;
          ChallengeDataEntry possibleChallenge = this.PossibleChallenges[UnityEngine.Random.Range(0, this.PossibleChallenges.Count)];
          ChallengeModifier challenge = possibleChallenge.challenge;
          bool flag = (UnityEngine.Object) challenge != (UnityEngine.Object) null && challenge.IsValid(currentRoom) && possibleChallenge.GetWeightForFloor(tilesetId) <= num1;
          if (flag && (possibleChallenge.excludedTilesets | tilesetId) == possibleChallenge.excludedTilesets)
            flag = false;
          if (flag && currentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.BOSS && !challenge.ValidInBossChambers)
            flag = false;
          for (int index = 0; index < this.m_activeChallenges.Count; ++index)
          {
            if (flag && this.m_activeChallenges[index].DisplayName == challenge.DisplayName)
              flag = false;
            if (flag && this.m_activeChallenges[index].MutuallyExclusive.Contains(challenge))
              flag = false;
          }
          if (flag)
          {
            this.m_activeChallenges.Add(UnityEngine.Object.Instantiate<GameObject>(challenge.gameObject).GetComponent<ChallengeModifier>());
            num1 -= possibleChallenge.GetWeightForFloor(tilesetId);
          }
        }
      }

      public void ForceStop() => this.CleanupChallenges();

      private void CleanupChallenges()
      {
        bool flag = false;
        if (this.m_activeChallenges.Count > 0 && (bool) (UnityEngine.Object) GameManager.Instance.PrimaryPlayer)
        {
          flag = true;
          int num = (int) AkSoundEngine.PostEvent("Play_UI_challenge_clear_01", GameManager.Instance.PrimaryPlayer.gameObject);
        }
        for (int index = 0; index < this.m_activeChallenges.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) this.m_activeChallenges[index])
          {
            this.m_activeChallenges[index].ShatterIcon(this.ChallengeBurstClip);
            UnityEngine.Object.Destroy((UnityEngine.Object) this.m_activeChallenges[index].gameObject);
          }
        }
        this.m_activeChallenges.Clear();
        if (!flag || this.ChallengeMode != ChallengeModeType.GunslingKingTemporary)
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }
    }

}
