// Decompiled with JetBrains decompiler
// Type: ThrownGunPassiveItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class ThrownGunPassiveItem : PassiveItem
    {
      public bool MakeThrownGunsExplode;
      [ShowInInspectorIf("MakeThrownGunsExplode", false)]
      public ExplosionData ThrownGunExplosionData;
      public bool MakeThrownGunsReturnLikeBoomerangs;
      [Header("Momentum")]
      public bool HasFlagContingentMomentum;
      [LongEnum]
      public GungeonFlags RequiredFlag;
      public GameObject OverheadVFX;
      public float TimeInMotion = 5f;
      public int AdditionalRollDamage = 100;
      public float MomentumKnockback = 100f;
      public GameObject MomentumVFX;
      private GameObject m_instanceVFX;
      private int m_destroyVFXSemaphore;
      private StatModifier m_damageStat;
      private bool m_cachedFlag;
      private float m_motionTimer;
      private bool m_hasUsedMomentum;

      private void Awake()
      {
        this.m_damageStat = new StatModifier();
        this.m_damageStat.statToBoost = PlayerStats.StatType.DodgeRollDamage;
        this.m_damageStat.modifyType = StatModifier.ModifyMethod.ADDITIVE;
        this.m_damageStat.amount = (float) this.AdditionalRollDamage;
      }

      public void EnableVFX(PlayerController target)
      {
        if (this.m_destroyVFXSemaphore != 0)
          return;
        Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(target.sprite);
        if ((UnityEngine.Object) outlineMaterial != (UnityEngine.Object) null)
          outlineMaterial.SetColor("_OverrideColor", new Color(99f, 0.0f, 99f));
        if (!(bool) (UnityEngine.Object) this.OverheadVFX || (bool) (UnityEngine.Object) this.m_instanceVFX)
          return;
        this.m_instanceVFX = target.PlayEffectOnActor(this.OverheadVFX, new Vector3(0.0f, 1.375f, 0.0f), alreadyMiddleCenter: true);
      }

      public void DisableVFX(PlayerController target)
      {
        if (this.m_destroyVFXSemaphore != 0)
          return;
        Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(target.sprite);
        if ((UnityEngine.Object) outlineMaterial != (UnityEngine.Object) null)
          outlineMaterial.SetColor("_OverrideColor", new Color(0.0f, 0.0f, 0.0f));
        if (!(bool) (UnityEngine.Object) this.m_instanceVFX)
          return;
        SpawnManager.Despawn(this.m_instanceVFX);
        this.m_instanceVFX = (GameObject) null;
      }

      protected override void Update()
      {
        if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel || Dungeon.IsGenerating || GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
        {
          this.m_motionTimer = 0.0f;
          this.m_hasUsedMomentum = true;
        }
        else
        {
          if (this.m_pickedUp && (bool) (UnityEngine.Object) this.Owner && this.HasFlagContingentMomentum && this.m_cachedFlag)
          {
            this.Owner.ReceivesTouchDamage = false;
            if (this.m_destroyVFXSemaphore <= 0)
            {
              if ((double) this.Owner.Velocity.magnitude > 0.05000000074505806)
              {
                this.m_motionTimer += BraveTime.DeltaTime;
                if ((double) this.m_motionTimer > (double) this.TimeInMotion)
                {
                  this.ForceTrigger(this.Owner);
                  this.m_motionTimer = 0.0f;
                }
              }
              else
              {
                this.m_hasUsedMomentum = true;
                this.m_motionTimer = 0.0f;
              }
            }
            else
            {
              if ((double) this.Owner.Velocity.magnitude < 0.05000000074505806)
                this.m_hasUsedMomentum = true;
              this.m_motionTimer = 0.0f;
            }
          }
          else
          {
            this.m_motionTimer = 0.0f;
            this.m_hasUsedMomentum = true;
          }
          base.Update();
        }
      }

      public void ForceTrigger(PlayerController target)
      {
        target.StartCoroutine(this.HandleDamageBoost(target));
      }

      [DebuggerHidden]
      private IEnumerator HandleDamageBoost(PlayerController target)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ThrownGunPassiveItem.<HandleDamageBoost>c__Iterator0()
        {
          target = target,
          $this = this
        };
      }

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        base.Pickup(player);
        if (this.HasFlagContingentMomentum)
          this.m_cachedFlag = GameStatsManager.Instance.GetFlag(this.RequiredFlag);
        player.PostProcessThrownGun += new Action<Projectile>(this.PostProcessThrownGun);
        player.OnRollStarted += new Action<PlayerController, Vector2>(this.HandleRollStarted);
        player.OnRolledIntoEnemy += new Action<PlayerController, AIActor>(this.HandleRolledIntoEnemy);
        player.OnReceivedDamage += new Action<PlayerController>(this.HandleReceivedDamage);
      }

      public void UpdateCachedFlag()
      {
        if (!this.HasFlagContingentMomentum)
          return;
        this.m_cachedFlag = GameStatsManager.Instance.GetFlag(this.RequiredFlag);
      }

      private void HandleRolledIntoEnemy(PlayerController arg1, AIActor arg2)
      {
        if (!this.m_hasUsedMomentum)
        {
          if ((bool) (UnityEngine.Object) arg2.knockbackDoer)
            arg2.knockbackDoer.ApplyKnockback(arg1.specRigidbody.Velocity.normalized, this.MomentumKnockback);
          if ((bool) (UnityEngine.Object) this.MomentumVFX)
          {
            tk2dBaseSprite component = arg2.PlayEffectOnActor(this.MomentumVFX, Vector3.zero, false, true).GetComponent<tk2dBaseSprite>();
            if ((bool) (UnityEngine.Object) component)
              component.HeightOffGround = 3.5f;
          }
        }
        this.m_hasUsedMomentum = true;
      }

      private void HandleReceivedDamage(PlayerController obj)
      {
        this.m_hasUsedMomentum = true;
        this.m_motionTimer = 0.0f;
        Material outlineMaterial = SpriteOutlineManager.GetOutlineMaterial(obj.sprite);
        obj.healthHaver.UpdateCachedOutlineColor(outlineMaterial, Color.black);
      }

      private void HandleRollStarted(PlayerController arg1, Vector2 arg2) => this.m_motionTimer = 0.0f;

      private void PostProcessThrownGun(Projectile thrownGunProjectile)
      {
        if (this.MakeThrownGunsExplode)
        {
          ExplosiveModifier explosiveModifier = thrownGunProjectile.gameObject.AddComponent<ExplosiveModifier>();
          explosiveModifier.doExplosion = true;
          explosiveModifier.explosionData = this.ThrownGunExplosionData;
          explosiveModifier.explosionData.damageToPlayer = 0.0f;
          if (this.ThrownGunExplosionData.useDefaultExplosion)
          {
            explosiveModifier.explosionData = new ExplosionData();
            explosiveModifier.explosionData.CopyFrom(GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData);
            explosiveModifier.explosionData.damageToPlayer = 0.0f;
          }
        }
        if (!this.MakeThrownGunsReturnLikeBoomerangs)
          return;
        thrownGunProjectile.OnBecameDebrisGrounded += new Action<DebrisObject>(this.HandleReturnLikeBoomerang);
      }

      private void HandleReturnLikeBoomerang(DebrisObject obj)
      {
        obj.OnGrounded -= new Action<DebrisObject>(this.HandleReturnLikeBoomerang);
        PickupMover pickupMover = obj.gameObject.AddComponent<PickupMover>();
        if ((bool) (UnityEngine.Object) pickupMover.specRigidbody)
          pickupMover.specRigidbody.CollideWithTileMap = false;
        pickupMover.minRadius = 1f;
        pickupMover.moveIfRoomUnclear = true;
        pickupMover.stopPathingOnContact = true;
      }

      public override DebrisObject Drop(PlayerController player)
      {
        DebrisObject debrisObject = base.Drop(player);
        debrisObject.GetComponent<ThrownGunPassiveItem>().m_pickedUpThisRun = true;
        if ((bool) (UnityEngine.Object) player)
        {
          player.ReceivesTouchDamage = true;
          player.PostProcessThrownGun -= new Action<Projectile>(this.PostProcessThrownGun);
          player.OnRollStarted -= new Action<PlayerController, Vector2>(this.HandleRollStarted);
          player.OnReceivedDamage -= new Action<PlayerController>(this.HandleReceivedDamage);
          player.OnRolledIntoEnemy -= new Action<PlayerController, AIActor>(this.HandleRolledIntoEnemy);
        }
        return debrisObject;
      }

      protected override void OnDestroy()
      {
        BraveTime.ClearMultiplier(this.gameObject);
        if (this.m_pickedUp && (bool) (UnityEngine.Object) this.Owner)
        {
          this.Owner.ReceivesTouchDamage = true;
          this.Owner.PostProcessThrownGun -= new Action<Projectile>(this.PostProcessThrownGun);
          this.Owner.OnRollStarted -= new Action<PlayerController, Vector2>(this.HandleRollStarted);
          this.Owner.OnReceivedDamage -= new Action<PlayerController>(this.HandleReceivedDamage);
          this.Owner.OnRolledIntoEnemy -= new Action<PlayerController, AIActor>(this.HandleRolledIntoEnemy);
        }
        base.OnDestroy();
      }
    }

}
