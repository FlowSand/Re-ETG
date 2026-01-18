// Decompiled with JetBrains decompiler
// Type: PuzzleBoxItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class PuzzleBoxItem : PlayerItem
  {
    public int NumberOfUsesToOpen = 3;
    public int NumUsesIncreasePerUsage = 1;
    public GameObject UseVFX;
    public GameObject OpenVFX;
    public bool ShouldUseRitualEveryUse;
    public float DemonicRitualChance = 0.05f;
    public float HurtPlayerChance = 0.2f;
    public float AmountToDamagePlayer = 0.5f;
    public float RitualChanceIncreasePerUsage = 0.05f;
    public float MaxEnemiesToSpawn = 5f;
    private float NumEnemiesToSpawn = 3f;
    public float ChanceToIncreaseCursePerAttempt = 0.5f;
    public int CurseIncreasePerAttempt;
    public float ChanceToDamagePlayerOnSuccess = 0.2f;
    public int CurseIncreasePerItem = 1;
    public float ChanceToEyeball = 1f / 1000f;
    [EnemyIdentifier]
    public string DevilEnemyGuid;
    [EnemyIdentifier]
    public string[] AdditionalEnemyGuids;
    private int m_numberOfUses;

    public override bool CanBeUsed(PlayerController user)
    {
      return (!(bool) (UnityEngine.Object) user || !user.InExitCell) && (!(bool) (UnityEngine.Object) user || user.CurrentRoom == null || !user.CurrentRoom.IsShop) && base.CanBeUsed(user);
    }

    protected override void OnPreDrop(PlayerController user) => base.OnPreDrop(user);

    public override void Pickup(PlayerController player) => base.Pickup(player);

    public override void MidGameSerialize(List<object> data)
    {
      base.MidGameSerialize(data);
      data.Add((object) this.m_numberOfUses);
    }

    public override void MidGameDeserialize(List<object> data)
    {
      base.MidGameDeserialize(data);
      if (data.Count != 1)
        return;
      this.m_numberOfUses = (int) data[0];
    }

    private void PlayTeleporterEffect(PlayerController p)
    {
      for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
      {
        if (!GameManager.Instance.AllPlayers[index].IsGhost)
        {
          GameManager.Instance.AllPlayers[index].healthHaver.TriggerInvulnerabilityPeriod(1f);
          GameManager.Instance.AllPlayers[index].knockbackDoer.TriggerTemporaryKnockbackInvulnerability(1f);
        }
      }
      GameObject original = (GameObject) ResourceCache.Acquire("Global VFX/VFX_Tentacleport");
      if (!((UnityEngine.Object) original != (UnityEngine.Object) null))
        return;
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(original);
      gameObject.GetComponent<tk2dBaseSprite>().PlaceAtLocalPositionByAnchor((Vector3) (p.specRigidbody.UnitBottomCenter + new Vector2(0.0f, -1f)), tk2dBaseSprite.Anchor.LowerCenter);
      gameObject.transform.position = gameObject.transform.position.Quantize(1f / 16f);
      gameObject.GetComponent<tk2dBaseSprite>().UpdateZDepth();
    }

    protected override void DoEffect(PlayerController user)
    {
      ++this.m_numberOfUses;
      this.CheckRitual(user, this.m_numberOfUses >= this.NumberOfUsesToOpen);
      GameStatsManager.Instance.RegisterStatChange(TrackedStats.LAMENT_CONFIGURUM_USES, 1f);
      if (this.m_numberOfUses >= this.NumberOfUsesToOpen)
      {
        user.PlayEffectOnActor(this.OpenVFX, new Vector3(1f / 32f, 1.5f, 0.0f));
        PickupObject pickupObject = this.Open(user);
        this.NumberOfUsesToOpen += this.NumUsesIncreasePerUsage;
        this.m_numberOfUses = 0;
        if (this.CurseIncreasePerItem > 0)
        {
          StatModifier statModifier = new StatModifier();
          statModifier.statToBoost = PlayerStats.StatType.Curse;
          statModifier.amount = (float) this.CurseIncreasePerItem;
          statModifier.modifyType = StatModifier.ModifyMethod.ADDITIVE;
          if ((bool) (UnityEngine.Object) pickupObject)
          {
            switch (pickupObject)
            {
              case Gun _:
                Gun gun = pickupObject as Gun;
                Array.Resize<StatModifier>(ref gun.passiveStatModifiers, gun.passiveStatModifiers.Length + 1);
                gun.passiveStatModifiers[gun.passiveStatModifiers.Length - 1] = statModifier;
                break;
              case PassiveItem _:
                PassiveItem passiveItem = pickupObject as PassiveItem;
                Array.Resize<StatModifier>(ref passiveItem.passiveStatModifiers, passiveItem.passiveStatModifiers.Length + 1);
                passiveItem.passiveStatModifiers[passiveItem.passiveStatModifiers.Length - 1] = statModifier;
                break;
              case PlayerItem _:
                PlayerItem playerItem = pickupObject as PlayerItem;
                Array.Resize<StatModifier>(ref playerItem.passiveStatModifiers, playerItem.passiveStatModifiers.Length + 1);
                playerItem.passiveStatModifiers[playerItem.passiveStatModifiers.Length - 1] = statModifier;
                break;
            }
          }
          else
          {
            user.ownerlessStatModifiers.Add(statModifier);
            user.stats.RecalculateStats(user);
          }
        }
        this.DemonicRitualChance += this.RitualChanceIncreasePerUsage;
      }
      else
      {
        if (this.CurseIncreasePerAttempt > 0 && (double) UnityEngine.Random.value < (double) this.ChanceToIncreaseCursePerAttempt)
        {
          user.ownerlessStatModifiers.Add(new StatModifier()
          {
            statToBoost = PlayerStats.StatType.Curse,
            amount = (float) this.CurseIncreasePerAttempt,
            modifyType = StatModifier.ModifyMethod.ADDITIVE
          });
          user.stats.RecalculateStats(user);
        }
        user.PlayEffectOnActor(this.UseVFX, new Vector3(1f / 32f, 49f / 32f, 0.0f));
      }
    }

    [DebuggerHidden]
    private IEnumerator TimedKill(AIActor targetActor)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new PuzzleBoxItem__TimedKillc__Iterator0()
      {
        targetActor = targetActor
      };
    }

    private void DoDamageIfIShould(PlayerController user)
    {
      if (user.HasActiveBonusSynergy(CustomSynergyType.HEART_SHAPED_BOX))
      {
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_heart_heal_01", this.gameObject);
        user.healthHaver.ApplyHealing(0.5f);
      }
      else
      {
        if ((double) UnityEngine.Random.value >= (double) this.ChanceToDamagePlayerOnSuccess)
          return;
        this.AmountToDamagePlayer += 0.5f;
        user.healthHaver.ApplyDamage(this.AmountToDamagePlayer, Vector2.zero, StringTableManager.GetItemsString("#LAMENTBOX_ENCNAME"), ignoreInvulnerabilityFrames: true);
      }
    }

    private void CheckRitual(PlayerController user, bool shouldOpen)
    {
      if (!shouldOpen && !this.ShouldUseRitualEveryUse || (double) UnityEngine.Random.value >= (double) this.DemonicRitualChance)
        return;
      bool flag1 = !user.CurrentRoom.IsSealed;
      FloodFillUtility.PreprocessContiguousCells(this.LastOwner.CurrentRoom, this.LastOwner.specRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor));
      IntVector2? targetCenter = new IntVector2?(user.CenterPosition.ToIntVector2(VectorConversions.Floor));
      int num = 0;
      this.NumEnemiesToSpawn = UnityEngine.Random.Range(2f, this.MaxEnemiesToSpawn);
      for (int index1 = 0; (double) index1 < (double) this.NumEnemiesToSpawn; ++index1)
      {
        string guid = this.DevilEnemyGuid;
        if (this.AdditionalEnemyGuids.Length > 0)
        {
          int index2 = UnityEngine.Random.Range(-1, this.AdditionalEnemyGuids.Length);
          if (index2 >= 0)
            guid = this.AdditionalEnemyGuids[index2];
        }
        AIActor enemyPrefab = EnemyDatabase.GetOrLoadByGuid(guid);
        bool checkContiguous = true;
        CellValidator cellValidator = (CellValidator) (c =>
        {
          if (checkContiguous && !FloodFillUtility.WasFilled(c))
            return false;
          for (int index3 = 0; index3 < enemyPrefab.Clearance.x; ++index3)
          {
            for (int index4 = 0; index4 < enemyPrefab.Clearance.y; ++index4)
            {
              if (GameManager.Instance.Dungeon.data.isTopWall(c.x + index3, c.y + index4) || targetCenter.HasValue && ((double) IntVector2.Distance(targetCenter.Value, c.x + index3, c.y + index4) < 4.0 || (double) IntVector2.Distance(targetCenter.Value, c.x + index3, c.y + index4) > 20.0))
                return false;
            }
          }
          return true;
        });
        checkContiguous = true;
        IntVector2? randomAvailableCell = user.CurrentRoom.GetRandomAvailableCell(new IntVector2?(enemyPrefab.Clearance), new CellTypes?(enemyPrefab.PathableTiles), cellValidator: cellValidator);
        if (!randomAvailableCell.HasValue)
        {
          checkContiguous = false;
          randomAvailableCell = user.CurrentRoom.GetRandomAvailableCell(new IntVector2?(enemyPrefab.Clearance), new CellTypes?(enemyPrefab.PathableTiles), cellValidator: cellValidator);
        }
        if (randomAvailableCell.HasValue)
        {
          AIActor targetActor = AIActor.Spawn(enemyPrefab, randomAvailableCell.Value, user.CurrentRoom, true);
          targetActor.StartCoroutine(this.TimedKill(targetActor));
          ++num;
          targetActor.HandleReinforcementFallIntoRoom();
        }
      }
      if (num <= 0)
        return;
      if (user.CurrentRoom.area.runtimePrototypeData != null)
      {
        bool flag2 = false;
        for (int index = 0; index < user.CurrentRoom.area.runtimePrototypeData.roomEvents.Count; ++index)
        {
          RoomEventDefinition roomEvent = user.CurrentRoom.area.runtimePrototypeData.roomEvents[index];
          if (roomEvent.condition == RoomEventTriggerCondition.ON_ENEMIES_CLEARED && roomEvent.action == RoomEventTriggerAction.UNSEAL_ROOM)
            flag2 = true;
        }
        if (!flag2)
          user.CurrentRoom.area.runtimePrototypeData.roomEvents.Add(new RoomEventDefinition(RoomEventTriggerCondition.ON_ENEMIES_CLEARED, RoomEventTriggerAction.UNSEAL_ROOM));
      }
      if (flag1)
        user.CurrentRoom.PreventStandardRoomReward = true;
      user.CurrentRoom.SealRoom();
    }

    private PickupObject Open(PlayerController user)
    {
      DebrisObject debrisObject = GameManager.Instance.RewardManager.SpawnTotallyRandomItem(user.CenterPosition, PickupObject.ItemQuality.B);
      this.DoDamageIfIShould(user);
      if (!(bool) (UnityEngine.Object) debrisObject)
        return (PickupObject) null;
      Vector2 position = !(bool) (UnityEngine.Object) debrisObject.sprite ? debrisObject.transform.position.XY() + new Vector2(0.5f, 0.5f) : debrisObject.sprite.WorldCenter;
      GameObject gameObject = SpawnManager.SpawnVFX((GameObject) BraveResources.Load("Global VFX/VFX_BlackPhantomDeath"), (Vector3) position, Quaternion.identity, false);
      if ((bool) (UnityEngine.Object) gameObject && (bool) (UnityEngine.Object) gameObject.GetComponent<tk2dSprite>())
      {
        tk2dSprite component = gameObject.GetComponent<tk2dSprite>();
        component.HeightOffGround = 5f;
        component.UpdateZDepth();
      }
      return debrisObject.GetComponentInChildren<PickupObject>();
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

