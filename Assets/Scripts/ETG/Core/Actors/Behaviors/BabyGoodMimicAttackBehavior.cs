// Decompiled with JetBrains decompiler
// Type: BabyGoodMimicAttackBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    public class BabyGoodMimicAttackBehavior : BasicAttackBehavior
    {
      public string AttackAnimationName = "attack";
      public ProjectileVolleyData Volley;
      public float TimeBetweenAttacks = 0.25f;
      public int NumberOfAttacks = 10;
      public VFXPool ShootVFX;
      private bool m_wasDamaged;
      private float m_continuousShotTimer;
      private float m_continuousElapsed;

      public override void Start()
      {
        base.Start();
        this.m_aiActor.healthHaver.ModifyDamage += new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage);
      }

      private void ModifyIncomingDamage(
        HealthHaver health,
        HealthHaver.ModifyDamageEventArgs damageArgs)
      {
        this.m_wasDamaged = true;
        damageArgs.ModifiedDamage = 0.0f;
      }

      public override void Upkeep() => base.Upkeep();

      public override BehaviorResult Update()
      {
        int num = (int) base.Update();
        if ((bool) (UnityEngine.Object) this.m_aiActor && (bool) (UnityEngine.Object) this.m_aiAnimator && (bool) (UnityEngine.Object) this.m_aiActor.CompanionOwner && this.m_aiActor.CompanionOwner.IsInCombat)
          this.m_aiAnimator.OverrideIdleAnimation = "mimic";
        else if ((bool) (UnityEngine.Object) this.m_aiAnimator)
          this.m_aiAnimator.OverrideIdleAnimation = string.Empty;
        BehaviorResult behaviorResult = base.Update();
        if (behaviorResult != BehaviorResult.Continue)
          return behaviorResult;
        if (!this.IsReady() || !this.m_wasDamaged)
          return BehaviorResult.Continue;
        this.m_wasDamaged = false;
        this.UpdateCooldowns();
        this.m_continuousElapsed = 0.0f;
        this.m_continuousShotTimer = 0.0f;
        this.m_updateEveryFrame = true;
        return BehaviorResult.RunContinuous;
      }

      public override ContinuousBehaviorResult ContinuousUpdate()
      {
        if ((double) this.m_continuousElapsed > (double) this.NumberOfAttacks * (double) this.TimeBetweenAttacks)
          return ContinuousBehaviorResult.Finished;
        this.m_continuousShotTimer -= BraveTime.DeltaTime;
        if ((double) this.m_continuousShotTimer <= 0.0)
        {
          Vector2 normalized = UnityEngine.Random.insideUnitCircle.normalized;
          this.m_aiAnimator.FacingDirection = normalized.ToAngle();
          if ((UnityEngine.Object) this.m_aiAnimator != (UnityEngine.Object) null)
            this.m_aiAnimator.PlayUntilFinished(this.AttackAnimationName, true);
          this.ShootVFX.SpawnAtPosition((Vector3) this.m_aiActor.CenterPosition, normalized.ToAngle());
          VolleyUtility.FireVolley(this.Volley, this.m_aiActor.CenterPosition, normalized, (GameActor) this.m_aiActor.CompanionOwner, true);
          this.m_continuousShotTimer += this.TimeBetweenAttacks;
        }
        this.m_continuousElapsed += BraveTime.DeltaTime;
        return base.ContinuousUpdate();
      }

      public override void EndContinuousUpdate()
      {
        this.m_updateEveryFrame = false;
        this.m_continuousShotTimer = 0.0f;
        this.m_continuousElapsed = 0.0f;
        if ((bool) (UnityEngine.Object) this.m_aiAnimator)
          this.m_aiAnimator.EndAnimationIf(this.AttackAnimationName);
        base.EndContinuousUpdate();
      }
    }

}
