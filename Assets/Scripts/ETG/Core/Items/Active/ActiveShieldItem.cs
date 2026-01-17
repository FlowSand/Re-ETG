// Decompiled with JetBrains decompiler
// Type: ActiveShieldItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Active
{
    public class ActiveShieldItem : PlayerItem
    {
      public GameObject prefabToAttachToPlayer;
      public float MaxShieldTime = 7f;
      public float DurationPortionToFlicker = 2f;
      private GameObject instanceShield;
      private tk2dSprite instanceShieldSprite;

      protected override void DoEffect(PlayerController user)
      {
        if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
          return;
        int num = (int) AkSoundEngine.PostEvent("Play_OBJ_item_throw_01", this.gameObject);
        this.IsCurrentlyActive = true;
        this.instanceShield = user.RegisterAttachedObject(this.prefabToAttachToPlayer, "jetpack");
        this.instanceShieldSprite = this.instanceShield.GetComponentInChildren<tk2dSprite>();
        if (user.HasActiveBonusSynergy(CustomSynergyType.MIRROR_SHIELD))
          this.instanceShieldSprite.spriteAnimator.Play("shield2_on");
        user.ChangeAttachedSpriteDepth((tk2dBaseSprite) this.instanceShieldSprite, -1f);
        user.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.PreventBulletCollisions);
        user.specRigidbody.BlockBeams = true;
        user.MovementModifiers += new GameActor.MovementModifier(this.NoMotionModifier);
        user.IsStationary = true;
        user.IsGunLocked = true;
        user.OnPreDodgeRoll += new Action<PlayerController>(this.HandleDodgeRollStarted);
        user.OnTriedToInitiateAttack += new Action<PlayerController>(this.HandleTriedAttack);
        user.StartCoroutine(this.HandleDuration(user));
      }

      [DebuggerHidden]
      private IEnumerator HandleDuration(PlayerController user)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ActiveShieldItem__HandleDurationc__Iterator0()
        {
          user = user,
          _this = this
        };
      }

      private void HandleTriedAttack(PlayerController obj) => this.DoActiveEffect(obj);

      private void HandleDodgeRollStarted(PlayerController obj) => this.DoActiveEffect(obj);

      private void PreventBulletCollisions(
        SpeculativeRigidbody myRigidbody,
        PixelCollider myPixelCollider,
        SpeculativeRigidbody otherRigidbody,
        PixelCollider otherPixelCollider)
      {
        if ((bool) (UnityEngine.Object) otherRigidbody.projectile)
        {
          if ((bool) (UnityEngine.Object) this.LastOwner && this.LastOwner.HasActiveBonusSynergy(CustomSynergyType.MIRROR_SHIELD))
            PassiveReflectItem.ReflectBullet(otherRigidbody.projectile, true, (GameActor) this.LastOwner, 10f);
          else
            otherRigidbody.projectile.DieInAir();
          PhysicsEngine.SkipCollision = true;
        }
        if (!(bool) (UnityEngine.Object) otherRigidbody.aiActor)
          return;
        if ((bool) (UnityEngine.Object) otherRigidbody.knockbackDoer)
          otherRigidbody.knockbackDoer.ApplyKnockback(otherRigidbody.UnitCenter - myRigidbody.UnitCenter, 25f);
        PhysicsEngine.SkipCollision = true;
      }

      private void LateUpdate()
      {
        if (!this.IsCurrentlyActive)
          return;
        this.instanceShieldSprite.HeightOffGround = 0.5f;
        this.instanceShieldSprite.UpdateZDepth();
      }

      private void NoMotionModifier(ref Vector2 voluntaryVel, ref Vector2 involuntaryVel)
      {
        voluntaryVel = Vector2.zero;
      }

      protected override void DoActiveEffect(PlayerController user)
      {
        this.IsCurrentlyActive = false;
        user.MovementModifiers -= new GameActor.MovementModifier(this.NoMotionModifier);
        user.specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.PreventBulletCollisions);
        user.specRigidbody.BlockBeams = false;
        Transform parent = this.instanceShield.transform.parent;
        user.DeregisterAttachedObject(this.instanceShield, false);
        user.IsStationary = false;
        user.IsGunLocked = false;
        user.OnPreDodgeRoll -= new Action<PlayerController>(this.HandleDodgeRollStarted);
        user.OnTriedToInitiateAttack -= new Action<PlayerController>(this.HandleTriedAttack);
        this.instanceShield.transform.parent = parent;
        this.instanceShieldSprite.spriteAnimator.Play(!(bool) (UnityEngine.Object) user || !user.HasActiveBonusSynergy(CustomSynergyType.MIRROR_SHIELD) ? "shield_off" : "shield2_off");
        this.instanceShieldSprite.spriteAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.DestroyParentObject);
        this.instanceShieldSprite = (tk2dSprite) null;
      }

      private void DestroyParentObject(tk2dSpriteAnimator source, tk2dSpriteAnimationClip clip)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) source.transform.parent.gameObject);
      }

      protected override void OnPreDrop(PlayerController user)
      {
        if (!this.IsCurrentlyActive)
          return;
        this.DoActiveEffect(user);
      }

      public override void OnItemSwitched(PlayerController user)
      {
        if (!this.IsCurrentlyActive)
          return;
        this.DoActiveEffect(user);
      }

      protected override void OnDestroy()
      {
        if ((UnityEngine.Object) this.LastOwner != (UnityEngine.Object) null)
        {
          this.LastOwner.OnPreDodgeRoll -= new Action<PlayerController>(this.HandleDodgeRollStarted);
          this.LastOwner.OnTriedToInitiateAttack -= new Action<PlayerController>(this.HandleTriedAttack);
        }
        if (this.IsCurrentlyActive)
          this.DoActiveEffect(this.LastOwner);
        base.OnDestroy();
      }
    }

}
