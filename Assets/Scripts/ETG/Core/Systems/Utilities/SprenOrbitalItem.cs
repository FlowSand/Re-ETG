// Decompiled with JetBrains decompiler
// Type: SprenOrbitalItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class SprenOrbitalItem : PlayerOrbitalItem
    {
      [PickupIdentifier]
      public int LimitGunId = -1;
      public float LimitDuration = 15f;
      public string IdleAnimation;
      public string GunChangeAnimation;
      public string GunChangeMoreAnimation;
      public string BackchangeAnimation;
      private SprenOrbitalItem.SprenTrigger m_trigger;
      private SprenOrbitalItem.SprenTrigger m_secondaryTrigger;
      private PlayerController m_player;
      private Gun m_extantGun;
      private SprenOrbitalItem.SprenTransformationState m_transformation;
      private int m_lastEquippedGunID = -1;
      private int m_lastEquippedGunAmmo = -1;

      private void Start() => this.AssignTrigger();

      private void AssignTrigger()
      {
        if (this.m_trigger == SprenOrbitalItem.SprenTrigger.UNASSIGNED)
          this.m_trigger = (SprenOrbitalItem.SprenTrigger) UnityEngine.Random.Range(1, 11);
        if (this.m_secondaryTrigger != SprenOrbitalItem.SprenTrigger.UNASSIGNED)
          return;
        while (this.m_secondaryTrigger == SprenOrbitalItem.SprenTrigger.UNASSIGNED || this.m_secondaryTrigger == this.m_trigger)
          this.m_secondaryTrigger = (SprenOrbitalItem.SprenTrigger) UnityEngine.Random.Range(1, 11);
      }

      private bool CheckTrigger(SprenOrbitalItem.SprenTrigger target, bool force = false)
      {
        return force || (bool) (UnityEngine.Object) this.m_owner && this.m_owner.HasActiveBonusSynergy(CustomSynergyType.SHARDBLADE) && this.m_secondaryTrigger == target || this.m_trigger == target;
      }

      public override void Pickup(PlayerController player)
      {
        base.Pickup(player);
        this.m_player = player;
        this.AssignTrigger();
        if (this.CheckTrigger(SprenOrbitalItem.SprenTrigger.USED_LAST_BLANK, true))
          player.OnUsedBlank += new Action<PlayerController, int>(this.HandleBlank);
        if (this.CheckTrigger(SprenOrbitalItem.SprenTrigger.LOST_LAST_ARMOR, true))
          player.LostArmor += new System.Action(this.HandleLostArmor);
        if (this.CheckTrigger(SprenOrbitalItem.SprenTrigger.ELECTROCUTED_OR_POISONED, true) || this.CheckTrigger(SprenOrbitalItem.SprenTrigger.TOOK_ANY_HEART_DAMAGE, true) || this.CheckTrigger(SprenOrbitalItem.SprenTrigger.REDUCED_TO_ONE_HEALTH, true))
          player.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.HandleDamaged);
        if (this.CheckTrigger(SprenOrbitalItem.SprenTrigger.ACTIVE_ITEM_USED, true))
          player.OnUsedPlayerItem += new Action<PlayerController, PlayerItem>(this.HandleActiveItemUsed);
        if (this.CheckTrigger(SprenOrbitalItem.SprenTrigger.FLIPPED_TABLE, true))
          player.OnTableFlipped += new Action<FlippableCover>(this.HandleTableFlipped);
        if (this.CheckTrigger(SprenOrbitalItem.SprenTrigger.FELL_IN_PIT, true))
          player.OnPitfall += new System.Action(this.HandlePitfall);
        if (!this.CheckTrigger(SprenOrbitalItem.SprenTrigger.SET_ON_FIRE, true))
          return;
        player.OnIgnited += new Action<PlayerController>(this.HandleIgnited);
      }

      protected override void Update()
      {
        if (this.m_transformation == SprenOrbitalItem.SprenTransformationState.TRANSFORMED && (GameManager.Instance.IsLoadingLevel || Dungeon.IsGenerating || (bool) (UnityEngine.Object) this.m_player && this.m_player.CurrentRoom != null && this.m_player.CurrentRoom.IsWinchesterArcadeRoom))
          this.DetransformSpren();
        if (this.m_transformation == SprenOrbitalItem.SprenTransformationState.NORMAL && this.CheckTrigger(SprenOrbitalItem.SprenTrigger.GUN_OUT_OF_AMMO) && (bool) (UnityEngine.Object) this.m_player && (bool) (UnityEngine.Object) this.m_player.CurrentGun)
        {
          if (!this.m_player.CurrentGun.InfiniteAmmo && this.m_player.CurrentGun.ammo <= 0 && this.m_player.CurrentGun.PickupObjectId == this.m_lastEquippedGunID && this.m_lastEquippedGunAmmo > 0)
            this.TransformSpren();
          this.m_lastEquippedGunID = this.m_player.CurrentGun.PickupObjectId;
          this.m_lastEquippedGunAmmo = this.m_player.CurrentGun.ammo;
        }
        base.Update();
      }

      private void HandleIgnited(PlayerController obj)
      {
        if (this.m_transformation != SprenOrbitalItem.SprenTransformationState.NORMAL || !this.CheckTrigger(SprenOrbitalItem.SprenTrigger.SET_ON_FIRE))
          return;
        this.TransformSpren();
      }

      private void HandlePitfall()
      {
        if (this.m_transformation != SprenOrbitalItem.SprenTransformationState.NORMAL || !this.CheckTrigger(SprenOrbitalItem.SprenTrigger.FELL_IN_PIT))
          return;
        this.TransformSpren();
      }

      private void HandleTableFlipped(FlippableCover obj)
      {
        if (this.m_transformation != SprenOrbitalItem.SprenTransformationState.NORMAL || !this.CheckTrigger(SprenOrbitalItem.SprenTrigger.FLIPPED_TABLE))
          return;
        this.TransformSpren();
      }

      private void HandleActiveItemUsed(PlayerController arg1, PlayerItem arg2)
      {
        if (this.m_transformation != SprenOrbitalItem.SprenTransformationState.NORMAL || !this.CheckTrigger(SprenOrbitalItem.SprenTrigger.ACTIVE_ITEM_USED))
          return;
        this.TransformSpren();
      }

      private void HandleLostArmor()
      {
        if (this.m_transformation != SprenOrbitalItem.SprenTransformationState.NORMAL || !this.CheckTrigger(SprenOrbitalItem.SprenTrigger.LOST_LAST_ARMOR) || (this.m_player.ForceZeroHealthState || (double) this.m_player.healthHaver.Armor != 0.0) && (!this.m_player.ForceZeroHealthState || (double) this.m_player.healthHaver.Armor != 1.0))
          return;
        this.TransformSpren();
      }

      private void HandleDamaged(
        float resultValue,
        float maxValue,
        CoreDamageTypes damageTypes,
        DamageCategory damageCategory,
        Vector2 damageDirection)
      {
        if (this.m_transformation != SprenOrbitalItem.SprenTransformationState.NORMAL)
          return;
        if (this.CheckTrigger(SprenOrbitalItem.SprenTrigger.ELECTROCUTED_OR_POISONED) && ((damageTypes | CoreDamageTypes.Electric) == damageTypes || (damageTypes | CoreDamageTypes.Poison) == damageTypes))
          this.TransformSpren();
        else if (this.CheckTrigger(SprenOrbitalItem.SprenTrigger.TOOK_ANY_HEART_DAMAGE) && (double) this.m_player.healthHaver.Armor == 0.0)
        {
          this.TransformSpren();
        }
        else
        {
          if (!this.CheckTrigger(SprenOrbitalItem.SprenTrigger.REDUCED_TO_ONE_HEALTH) || (double) this.m_player.healthHaver.GetCurrentHealth() > 0.5)
            return;
          this.TransformSpren();
        }
      }

      private void HandleBlank(PlayerController arg1, int BlanksRemaining)
      {
        if (this.m_transformation != SprenOrbitalItem.SprenTransformationState.NORMAL || !this.CheckTrigger(SprenOrbitalItem.SprenTrigger.USED_LAST_BLANK) || BlanksRemaining != 0)
          return;
        this.TransformSpren();
      }

      private void Disconnect(PlayerController player)
      {
        player.OnUsedBlank -= new Action<PlayerController, int>(this.HandleBlank);
        player.LostArmor -= new System.Action(this.HandleLostArmor);
        player.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.HandleDamaged);
        player.OnUsedPlayerItem -= new Action<PlayerController, PlayerItem>(this.HandleActiveItemUsed);
        player.OnTableFlipped -= new Action<FlippableCover>(this.HandleTableFlipped);
        player.OnPitfall -= new System.Action(this.HandlePitfall);
        player.OnIgnited -= new Action<PlayerController>(this.HandleIgnited);
      }

      public override DebrisObject Drop(PlayerController player)
      {
        this.Disconnect(player);
        return base.Drop(player);
      }

      protected void TransformSpren()
      {
        if (this.m_transformation != SprenOrbitalItem.SprenTransformationState.NORMAL || (bool) (UnityEngine.Object) this.m_player && this.m_player.CurrentRoom != null && this.m_player.CurrentRoom.IsWinchesterArcadeRoom)
          return;
        this.m_transformation = SprenOrbitalItem.SprenTransformationState.PRE_TRANSFORM;
        if (!(bool) (UnityEngine.Object) this.m_player || this.m_player.IsGhost)
          return;
        this.m_player.StartCoroutine(this.HandleTransformationDuration());
      }

      [DebuggerHidden]
      private IEnumerator HandleTransformationDuration()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SprenOrbitalItem.<HandleTransformationDuration>c__Iterator0()
        {
          _this = this
        };
      }

      protected void DetransformSpren()
      {
        if (this.m_transformation != SprenOrbitalItem.SprenTransformationState.TRANSFORMED || !(bool) (UnityEngine.Object) this || !(bool) (UnityEngine.Object) this.m_player || !(bool) (UnityEngine.Object) this.m_extantGun)
          return;
        this.m_transformation = SprenOrbitalItem.SprenTransformationState.NORMAL;
        if ((bool) (UnityEngine.Object) this.m_player)
        {
          if (!GameManager.Instance.IsLoadingLevel && !Dungeon.IsGenerating)
            Minimap.Instance.ToggleMinimap(false);
          this.m_player.inventory.GunLocked.RemoveOverride("spren gun");
          this.m_player.inventory.DestroyGun(this.m_extantGun);
          this.m_extantGun = (Gun) null;
        }
        this.m_player.inventory.GunChangeForgiveness = false;
      }

      protected override void OnDestroy()
      {
        if ((bool) (UnityEngine.Object) this.m_player)
          this.Disconnect(this.m_player);
        this.m_player = (PlayerController) null;
        base.OnDestroy();
      }

      public enum SprenTrigger
      {
        UNASSIGNED,
        USED_LAST_BLANK,
        LOST_LAST_ARMOR,
        REDUCED_TO_ONE_HEALTH,
        GUN_OUT_OF_AMMO,
        SET_ON_FIRE,
        ELECTROCUTED_OR_POISONED,
        FELL_IN_PIT,
        TOOK_ANY_HEART_DAMAGE,
        FLIPPED_TABLE,
        ACTIVE_ITEM_USED,
      }

      private enum SprenTransformationState
      {
        NORMAL,
        PRE_TRANSFORM,
        TRANSFORMED,
      }
    }

}
