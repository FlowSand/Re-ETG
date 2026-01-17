// Decompiled with JetBrains decompiler
// Type: ChargeTargetBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    public class ChargeTargetBehavior : MovementBehaviorBase
    {
      public float ChargeCooldownTime = 1f;
      public float OvershootFactor = 3f;
      public float ChargeSpeed = 8f;
      public float ChargeAcceleration = 4f;
      public float ChargeKnockback = 50f;
      public float ChargeDamage;
      public bool ChargeDoDustUps;
      public float ChargeDustUpInterval;
      public GameObject ChargeHitVFX;
      public float BumpTime = 1f;
      public float PlayMeleeAnimDistance = 2f;
      protected bool m_playedMelee;
      protected bool m_playedBump;
      private ChargeTargetBehavior.ChargeState m_state = ChargeTargetBehavior.ChargeState.Waiting;
      private float m_chargeTargetLength;
      private Vector2 m_chargeDirection;
      private float m_chargeElapsedDistance;
      private float m_deceleration;
      private float m_currentMovementSpeed;
      private float m_cachedKnockback;
      private float m_cachedDamage;
      private bool m_cachedDoDustUps;
      private float m_cachedDustUpInterval;
      private VFXPool m_cachedVfx;
      private float m_repathTimer;
      private float m_phaseTimer;

      protected ChargeTargetBehavior.ChargeState State
      {
        get => this.m_state;
        set
        {
          this.EndState(this.m_state);
          this.m_state = value;
          this.BeginState(this.m_state);
        }
      }

      public override void Start()
      {
        base.Start();
        this.m_cachedKnockback = this.m_aiActor.CollisionKnockbackStrength;
        this.m_cachedDamage = this.m_aiActor.CollisionDamage;
        this.m_cachedDoDustUps = this.m_aiActor.DoDustUps;
        this.m_cachedDustUpInterval = this.m_aiActor.DustUpInterval;
        this.m_cachedVfx = this.m_aiActor.CollisionVFX;
        this.m_deceleration = (float) ((double) this.ChargeSpeed * (double) this.ChargeSpeed / (-2.0 * (double) this.OvershootFactor));
        this.m_aiAnimator.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.OnRigidbodyCollision);
      }

      public override void Upkeep()
      {
        base.Upkeep();
        this.DecrementTimer(ref this.m_repathTimer);
        this.DecrementTimer(ref this.m_phaseTimer);
      }

      public override BehaviorResult Update()
      {
        if ((Object) this.m_aiActor.TargetRigidbody != (Object) null)
        {
          switch (this.State)
          {
            case ChargeTargetBehavior.ChargeState.Charging:
              return this.HandleChargeState();
            case ChargeTargetBehavior.ChargeState.Waiting:
              return this.HandleWaitState();
            case ChargeTargetBehavior.ChargeState.Bumped:
              return this.HandleBumpedState();
          }
        }
        return BehaviorResult.Continue;
      }

      protected void BeginState(ChargeTargetBehavior.ChargeState state)
      {
        switch (state)
        {
          case ChargeTargetBehavior.ChargeState.Charging:
            this.m_playedMelee = false;
            this.m_playedBump = false;
            this.m_chargeElapsedDistance = 0.0f;
            this.m_aiActor.CollisionKnockbackStrength = this.ChargeKnockback;
            this.m_aiActor.CollisionDamage = this.ChargeDamage;
            this.m_aiActor.DoDustUps = this.ChargeDoDustUps;
            this.m_aiActor.DustUpInterval = this.ChargeDustUpInterval;
            if ((bool) (Object) this.ChargeHitVFX)
              this.m_aiActor.CollisionVFX = new VFXPool()
              {
                type = VFXPoolType.Single,
                effects = new VFXComplex[1]
                {
                  new VFXComplex()
                  {
                    effects = new VFXObject[1]
                    {
                      new VFXObject() { effect = this.ChargeHitVFX }
                    }
                  }
                }
              };
            this.m_aiActor.ClearPath();
            Vector2 vector2 = this.m_aiActor.TargetRigidbody.UnitCenter - this.m_aiActor.specRigidbody.UnitCenter;
            this.m_chargeTargetLength = vector2.magnitude;
            this.m_chargeDirection = vector2.normalized;
            this.m_aiActor.BehaviorOverridesVelocity = true;
            break;
          case ChargeTargetBehavior.ChargeState.Waiting:
            this.m_aiActor.CollisionKnockbackStrength = this.m_cachedKnockback;
            this.m_aiActor.CollisionDamage = this.m_cachedDamage;
            this.m_aiActor.DoDustUps = this.m_cachedDoDustUps;
            this.m_aiActor.DustUpInterval = this.m_cachedDustUpInterval;
            this.m_aiActor.CollisionVFX = this.m_cachedVfx;
            this.m_aiActor.BehaviorOverridesVelocity = false;
            this.m_aiActor.BehaviorVelocity = Vector2.zero;
            this.m_currentMovementSpeed = this.m_aiActor.MovementSpeed;
            this.m_phaseTimer = this.ChargeCooldownTime;
            break;
          case ChargeTargetBehavior.ChargeState.Bumped:
            this.m_aiActor.BehaviorOverridesVelocity = true;
            this.m_aiActor.BehaviorVelocity = Vector2.zero;
            this.m_currentMovementSpeed = 0.0f;
            this.m_phaseTimer = this.BumpTime;
            break;
        }
      }

      protected void EndState(ChargeTargetBehavior.ChargeState state)
      {
        if (state == ChargeTargetBehavior.ChargeState.Charging)
        {
          this.m_aiAnimator.EndAnimationIf("prebump");
          this.m_aiAnimator.LockFacingDirection = false;
        }
        else
        {
          if (state == ChargeTargetBehavior.ChargeState.Waiting || state != ChargeTargetBehavior.ChargeState.Bumped)
            return;
          this.m_aiAnimator.EndAnimationIf("bump");
          this.m_aiAnimator.LockFacingDirection = false;
        }
      }

      protected BehaviorResult HandleChargeState()
      {
        this.m_aiActor.BehaviorVelocity = this.m_chargeDirection * this.m_currentMovementSpeed;
        this.m_chargeElapsedDistance += this.m_currentMovementSpeed * this.m_deltaTime;
        this.m_aiActor.ClearPath();
        if ((double) this.m_chargeElapsedDistance >= (double) this.m_chargeTargetLength + (double) this.OvershootFactor || (double) this.m_currentMovementSpeed == 0.0)
          this.State = ChargeTargetBehavior.ChargeState.Waiting;
        else if ((double) this.m_chargeElapsedDistance > (double) this.m_chargeTargetLength)
        {
          this.m_currentMovementSpeed = Mathf.Max(this.m_currentMovementSpeed + this.m_deceleration * this.m_deltaTime, 0.0f);
          if (this.m_playedMelee && !this.m_playedBump && (double) Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, this.m_aiActor.TargetRigidbody.UnitCenter) > (double) this.PlayMeleeAnimDistance)
            this.m_aiAnimator.EndAnimationIf("prebump");
        }
        else
        {
          this.m_currentMovementSpeed = Mathf.Min(this.m_currentMovementSpeed + this.ChargeAcceleration * this.m_deltaTime, this.ChargeSpeed);
          if (!this.m_playedMelee && (double) Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, this.m_aiActor.TargetRigidbody.UnitCenter) < (double) this.PlayMeleeAnimDistance)
          {
            this.m_aiAnimator.LockFacingDirection = true;
            this.m_aiAnimator.FacingDirection = BraveMathCollege.Atan2Degrees(this.m_chargeDirection);
            this.m_aiAnimator.PlayUntilCancelled("prebump", true);
            this.m_playedMelee = true;
          }
        }
        return BehaviorResult.SkipRemainingClassBehaviors;
      }

      protected BehaviorResult HandleWaitState()
      {
        bool lineOfSightToTarget = this.m_aiActor.HasLineOfSightToTarget;
        bool flag = false;
        if (lineOfSightToTarget)
          flag = GameManager.Instance.Dungeon.data.CheckLineForCellType(this.m_aiActor.PathTile, this.m_aiActor.TargetRigidbody.UnitCenter.ToIntVector2(VectorConversions.Floor), CellType.PIT);
        if (lineOfSightToTarget && !flag && (double) this.m_phaseTimer == 0.0)
          this.State = ChargeTargetBehavior.ChargeState.Charging;
        return BehaviorResult.Continue;
      }

      protected BehaviorResult HandleBumpedState()
      {
        this.m_aiActor.CollisionKnockbackStrength = this.m_cachedKnockback;
        this.m_aiActor.CollisionDamage = this.m_cachedDamage;
        this.m_aiActor.DoDustUps = this.m_cachedDoDustUps;
        this.m_aiActor.DustUpInterval = this.m_cachedDustUpInterval;
        this.m_aiActor.CollisionVFX = this.m_cachedVfx;
        this.m_aiActor.ClearPath();
        if ((double) this.m_phaseTimer == 0.0)
          this.State = ChargeTargetBehavior.ChargeState.Waiting;
        return BehaviorResult.Continue;
      }

      private void OnRigidbodyCollision(CollisionData rigidbodyCollision)
      {
        if (this.State != ChargeTargetBehavior.ChargeState.Charging || !this.m_playedMelee || this.m_playedBump || !(bool) (Object) rigidbodyCollision.OtherRigidbody.GetComponent<PlayerController>())
          return;
        this.m_aiAnimator.LockFacingDirection = true;
        this.m_aiAnimator.FacingDirection = BraveMathCollege.Atan2Degrees(rigidbodyCollision.OtherRigidbody.UnitCenter - this.m_aiAnimator.specRigidbody.UnitCenter);
        this.m_aiAnimator.PlayUntilCancelled("bump", true);
        this.State = ChargeTargetBehavior.ChargeState.Bumped;
      }

      protected enum ChargeState
      {
        Charging,
        Waiting,
        Bumped,
      }
    }

}
