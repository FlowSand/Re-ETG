// Decompiled with JetBrains decompiler
// Type: GungeonEggItem
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

public class GungeonEggItem : PlayerItem
  {
    public int m_numberElapsedFloors;
    public GameObject HealVFX;
    [EnemyIdentifier]
    public string GudetamaGuid;
    [PickupIdentifier]
    public int BabyDragunItemId;
    public DungeonPlaceableBehaviour BabyDragunPlaceable;
    public float TimeInFireToHatch = 4f;
    public bool DoShards;
    public ShardsModule Shards;
    private bool m_isBroken;
    private float m_elapsedInFire;
    private bool m_coroutineActive;

    protected override void Start()
    {
      base.Start();
      this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.HandleTriggerCollision);
    }

    private bool IsPointOnFire(Vector2 testPos)
    {
      IntVector2 intVector2 = (testPos / DeadlyDeadlyGoopManager.GOOP_GRID_SIZE).ToIntVector2(VectorConversions.Floor);
      return DeadlyDeadlyGoopManager.allGoopPositionMap.ContainsKey(intVector2) && DeadlyDeadlyGoopManager.allGoopPositionMap[intVector2].IsPositionOnFire(testPos);
    }

    private void HatchToDragun()
    {
      this.m_isBroken = true;
      this.spriteAnimator.PlayAndDestroyObject("gungeon_egg_hatch");
      this.StartCoroutine(this.HandleDelayedShards());
      this.m_pickedUp = true;
      RoomHandler absoluteRoom = this.transform.position.GetAbsoluteRoom();
      GameObject gameObject = this.BabyDragunPlaceable.InstantiateObject(absoluteRoom, this.transform.position.IntXY() - absoluteRoom.area.basePosition + IntVector2.NegOne);
      gameObject.transform.position = this.transform.position + new Vector3(-0.25f, -0.5f, 0.0f);
      gameObject.GetComponentInChildren<tk2dBaseSprite>().UpdateZDepth();
      gameObject.GetComponentInChildren<SpeculativeRigidbody>().Reinitialize();
      DeadlyDeadlyGoopManager.DelayedClearGoopsInRadius(this.transform.position.XY() + new Vector2(0.25f, 0.5f), 3f);
    }

    private void HandleTriggerCollision(
      SpeculativeRigidbody specRigidbody,
      SpeculativeRigidbody sourceSpecRigidbody,
      CollisionData collisionData)
    {
      if (this.m_isBroken || !this.enabled || this.m_pickedUp)
        return;
      if (this.IsPointOnFire(specRigidbody.UnitCenter))
      {
        this.CanBeSold = false;
        this.HatchToDragun();
      }
      else if (this.m_numberElapsedFloors > 0 && !this.m_isBroken)
      {
        if (!(bool) (UnityEngine.Object) specRigidbody || !(bool) (UnityEngine.Object) specRigidbody.projectile || !(specRigidbody.projectile.Owner is PlayerController))
          return;
        this.m_isBroken = true;
        this.CreateRewardItem();
        this.m_pickedUp = true;
        this.CanBeSold = false;
        this.spriteAnimator.PlayAndDestroyObject("gungeon_egg_hatch");
        this.StartCoroutine(this.HandleDelayedShards());
      }
      else
      {
        if (this.m_numberElapsedFloors != 0 || this.m_isBroken || !(bool) (UnityEngine.Object) specRigidbody || !(bool) (UnityEngine.Object) specRigidbody.projectile || !(specRigidbody.projectile.Owner is PlayerController))
          return;
        this.m_isBroken = true;
        AIActor aiActor = AIActor.Spawn(EnemyDatabase.GetOrLoadByGuid(this.GudetamaGuid), this.transform.position.XY().ToIntVector2(), this.transform.position.GetAbsoluteRoom());
        if ((bool) (UnityEngine.Object) aiActor)
        {
          aiActor.healthHaver.TriggerInvulnerabilityPeriod(0.5f);
          aiActor.PreventAutoKillOnBossDeath = true;
        }
        this.m_pickedUp = true;
        this.spriteAnimator.PlayAndDestroyObject("gungeon_egg_hatch");
        this.StartCoroutine(this.HandleDelayedShards());
      }
    }

    [DebuggerHidden]
    private IEnumerator HandleDelayedShards()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GungeonEggItem__HandleDelayedShardsc__Iterator0()
      {
        _this = this
      };
    }

    public override bool CanBeUsed(PlayerController user)
    {
      return (double) user.healthHaver.GetCurrentHealthPercentage() < 1.0 && base.CanBeUsed(user);
    }

    protected override void DoEffect(PlayerController user)
    {
      base.DoEffect(user);
      user.healthHaver.FullHeal();
      user.PlayEffectOnActor(this.HealVFX, Vector3.zero);
      int num = (int) AkSoundEngine.PostEvent("Play_OBJ_med_kit_01", this.gameObject);
    }

    protected void CreateRewardItem()
    {
      PickupObject.ItemQuality itemQuality = PickupObject.ItemQuality.D;
      if (this.m_numberElapsedFloors >= 4)
      {
        itemQuality = PickupObject.ItemQuality.S;
        if (this.m_numberElapsedFloors < 9)
          ;
      }
      else
      {
        switch (this.m_numberElapsedFloors)
        {
          case 1:
            itemQuality = PickupObject.ItemQuality.C;
            break;
          case 2:
            itemQuality = PickupObject.ItemQuality.B;
            break;
          case 3:
            itemQuality = PickupObject.ItemQuality.A;
            break;
        }
      }
      PickupObject ofTypeAndQuality = LootEngine.GetItemOfTypeAndQuality<PickupObject>(itemQuality, (double) UnityEngine.Random.value >= 0.5 ? GameManager.Instance.RewardManager.GunsLootTable : GameManager.Instance.RewardManager.ItemsLootTable);
      if (!(bool) (UnityEngine.Object) ofTypeAndQuality)
        return;
      LootEngine.SpawnItem(ofTypeAndQuality.gameObject, this.transform.position, Vector2.up, 0.1f);
    }

    public override void Update()
    {
      base.Update();
      if (!this.m_pickedUp && !this.m_isBroken)
      {
        if (this.IsPointOnFire(this.specRigidbody.UnitCenter))
        {
          this.m_elapsedInFire += BraveTime.DeltaTime;
          if ((double) this.m_elapsedInFire > (double) this.TimeInFireToHatch)
            this.HatchToDragun();
        }
        else
          this.m_elapsedInFire = 0.0f;
      }
      if (this.spriteAnimator.IsPlaying("gungeon_egg_hatch"))
        return;
      if (this.m_numberElapsedFloors >= 2 && this.m_numberElapsedFloors < 4 && !this.spriteAnimator.IsPlaying("gungeon_egg_stir_2"))
      {
        this.spriteAnimator.Play("gungeon_egg_stir_2");
      }
      else
      {
        if (this.m_numberElapsedFloors < 4 || this.spriteAnimator.IsPlaying("gungeon_egg_stir_3"))
          return;
        this.spriteAnimator.Play("gungeon_egg_stir_3");
      }
    }

    public override void Pickup(PlayerController player)
    {
      base.Pickup(player);
      player.OnNewFloorLoaded += new Action<PlayerController>(this.HandleLevelLoaded);
    }

    private void HandleLevelLoaded(PlayerController source)
    {
      if (this.m_coroutineActive)
        return;
      this.m_coroutineActive = true;
      this.StartCoroutine(this.DelayedProcessing());
    }

    [DebuggerHidden]
    private IEnumerator DelayedProcessing()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new GungeonEggItem__DelayedProcessingc__Iterator1()
      {
        _this = this
      };
    }

    protected override void OnPreDrop(PlayerController user)
    {
      base.OnPreDrop(user);
      user.OnNewFloorLoaded -= new Action<PlayerController>(this.HandleLevelLoaded);
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      if (!(bool) (UnityEngine.Object) this.LastOwner)
        return;
      this.LastOwner.OnNewFloorLoaded -= new Action<PlayerController>(this.HandleLevelLoaded);
    }

    public override void MidGameSerialize(List<object> data)
    {
      base.MidGameSerialize(data);
      data.Add((object) this.m_numberElapsedFloors);
    }

    public override void MidGameDeserialize(List<object> data)
    {
      base.MidGameDeserialize(data);
      if (data.Count != 1)
        return;
      this.m_numberElapsedFloors = (int) data[0];
    }
  }

