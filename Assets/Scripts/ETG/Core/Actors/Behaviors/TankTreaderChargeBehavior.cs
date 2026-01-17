// Decompiled with JetBrains decompiler
// Type: TankTreaderChargeBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using FullInspector;
using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    [InspectorDropdownName("Bosses/TankTreader/ChargeBehavior")]
    public class TankTreaderChargeBehavior : BasicAttackBehavior
    {
      public float minRange;
      public string chargeAnim;
      public string hitAnim;
      public float chargeSpeed;
      public float maxChargeDistance = -1f;
      public float chargeKnockback = 50f;
      public float chargeDamage = 0.5f;
      public float wallRecoilForce = 10f;
      public GameObject launchVfx;
      public GameObject trailVfx;
      public Transform trailVfxParent;
      public GameObject hitVfx;
      public bool chargeDustUps;
      public float chargeDustUpInterval;
      private TankTreaderChargeBehavior.FireState m_state;
      private float m_chargeTime;
      private Vector2 m_chargeDir;
      private float m_cachedKnockback;
      private float m_cachedDamage;
      private VFXPool m_cachedVfx;
      private CellTypes m_cachedPathableTiles;
      private bool m_cachedDoDustUps;
      private float m_cachedDustUpInterval;
      private GameObject m_trailVfx;

      public override void Start()
      {
        base.Start();
        this.m_aiActor.specRigidbody.OnCollision += new Action<CollisionData>(this.OnCollision);
        this.m_cachedKnockback = this.m_aiActor.CollisionKnockbackStrength;
        this.m_cachedDamage = this.m_aiActor.CollisionDamage;
        this.m_cachedVfx = this.m_aiActor.CollisionVFX;
        this.m_cachedPathableTiles = this.m_aiActor.PathableTiles;
        this.m_cachedDoDustUps = this.m_aiActor.DoDustUps;
        this.m_cachedDustUpInterval = this.m_aiActor.DustUpInterval;
      }

      public override void Upkeep() => base.Upkeep();

      public override BehaviorResult Update()
      {
        int num1 = (int) base.Update();
        BehaviorResult behaviorResult = base.Update();
        if (behaviorResult != BehaviorResult.Continue)
          return behaviorResult;
        if (!this.IsReady() || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody || (double) Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox)) <= (double) this.minRange)
          return BehaviorResult.Continue;
        PixelCollider hitboxPixelCollider = this.m_aiActor.TargetRigidbody.specRigidbody.HitboxPixelCollider;
        PixelCollider groundPixelCollider = this.m_aiActor.specRigidbody.GroundPixelCollider;
        bool flag1 = (double) hitboxPixelCollider.UnitRight < (double) groundPixelCollider.UnitLeft;
        bool flag2 = (double) hitboxPixelCollider.UnitLeft > (double) groundPixelCollider.UnitRight;
        bool flag3 = (double) hitboxPixelCollider.UnitBottom > (double) groundPixelCollider.UnitTop;
        bool flag4 = (double) hitboxPixelCollider.UnitTop < (double) groundPixelCollider.UnitBottom;
        Vector2 vector = Vector2.zero;
        if (flag1 && !flag4 && !flag3)
          vector = -Vector2.right;
        else if (flag2 && !flag4 && !flag3)
          vector = Vector2.right;
        else if (flag3 && !flag1 && !flag2)
          vector = Vector2.up;
        else if (flag4 && !flag1 && !flag2)
          vector = -Vector2.up;
        if (vector != Vector2.zero)
        {
          float num2 = BraveMathCollege.AbsAngleBetween(vector.ToAngle(), this.m_aiAnimator.FacingDirection);
          if ((double) num2 > 90.0)
            num2 = Mathf.Abs(num2 - 180f);
          if ((double) num2 < 20.0)
          {
            this.m_chargeDir = vector;
            this.State = TankTreaderChargeBehavior.FireState.Charging;
            this.m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
          }
        }
        return BehaviorResult.Continue;
      }

      public override ContinuousBehaviorResult ContinuousUpdate()
      {
        if (this.State == TankTreaderChargeBehavior.FireState.Charging)
        {
          this.m_aiActor.BehaviorVelocity = this.m_chargeDir.normalized * this.chargeSpeed;
          if ((double) this.maxChargeDistance > 0.0)
          {
            this.m_chargeTime += this.m_deltaTime;
            if ((double) this.m_chargeTime * (double) this.chargeSpeed > (double) this.maxChargeDistance)
              return ContinuousBehaviorResult.Finished;
          }
        }
        else if (this.State == TankTreaderChargeBehavior.FireState.Bouncing && !this.m_aiAnimator.IsPlaying(this.hitAnim))
          return ContinuousBehaviorResult.Finished;
        return ContinuousBehaviorResult.Continue;
      }

      public override void EndContinuousUpdate()
      {
        base.EndContinuousUpdate();
        this.m_updateEveryFrame = false;
        this.State = TankTreaderChargeBehavior.FireState.Idle;
        this.UpdateCooldowns();
      }

      private void OnCollision(CollisionData collisionData)
      {
        if (this.State != TankTreaderChargeBehavior.FireState.Charging || this.m_aiActor.healthHaver.IsDead)
          return;
        if ((bool) (UnityEngine.Object) collisionData.OtherRigidbody)
        {
          Projectile projectile = collisionData.OtherRigidbody.projectile;
          if ((bool) (UnityEngine.Object) projectile && !(projectile.Owner is PlayerController))
            return;
        }
        this.State = TankTreaderChargeBehavior.FireState.Bouncing;
      }

      private TankTreaderChargeBehavior.FireState State
      {
        get => this.m_state;
        set
        {
          if (this.m_state == value)
            return;
          this.EndState(this.m_state);
          this.m_state = value;
          this.BeginState(this.m_state);
        }
      }

      private void BeginState(TankTreaderChargeBehavior.FireState state)
      {
        if (state == TankTreaderChargeBehavior.FireState.Idle || state != TankTreaderChargeBehavior.FireState.Charging)
          return;
        this.m_chargeTime = 0.0f;
        this.m_aiActor.ClearPath();
        this.m_aiActor.BehaviorVelocity = this.m_chargeDir.normalized * this.chargeSpeed;
        float angle = this.m_aiActor.BehaviorVelocity.ToAngle();
        this.m_aiActor.CollisionKnockbackStrength = this.chargeKnockback;
        this.m_aiActor.CollisionDamage = this.chargeDamage;
        if ((bool) (UnityEngine.Object) this.hitVfx)
          this.m_aiActor.CollisionVFX = new VFXPool()
          {
            type = VFXPoolType.Single,
            effects = new VFXComplex[1]
            {
              new VFXComplex()
              {
                effects = new VFXObject[1]
                {
                  new VFXObject() { effect = this.hitVfx }
                }
              }
            }
          };
        this.m_aiActor.PathableTiles = CellTypes.FLOOR | CellTypes.PIT;
        this.m_aiActor.DoDustUps = this.chargeDustUps;
        this.m_aiActor.DustUpInterval = this.chargeDustUpInterval;
        this.m_aiAnimator.PlayUntilFinished(this.chargeAnim, true);
        if ((bool) (UnityEngine.Object) this.launchVfx)
          SpawnManager.SpawnVFX(this.launchVfx, (Vector3) this.m_aiActor.specRigidbody.UnitCenter, Quaternion.identity);
        if ((bool) (UnityEngine.Object) this.trailVfx)
        {
          this.m_trailVfx = SpawnManager.SpawnParticleSystem(this.trailVfx, (Vector3) this.m_aiActor.sprite.WorldCenter, Quaternion.Euler(0.0f, 0.0f, angle));
          this.m_trailVfx.transform.parent = !(bool) (UnityEngine.Object) this.trailVfxParent ? this.m_aiActor.transform : this.trailVfxParent;
          ParticleKiller component = this.m_trailVfx.GetComponent<ParticleKiller>();
          if ((UnityEngine.Object) component != (UnityEngine.Object) null)
            component.Awake();
        }
        this.m_aiActor.specRigidbody.ForceRegenerate();
      }

      private void EndState(TankTreaderChargeBehavior.FireState state)
      {
        if (state != TankTreaderChargeBehavior.FireState.Charging)
          return;
        this.m_aiActor.BehaviorVelocity = Vector2.zero;
        this.m_aiAnimator.PlayUntilFinished(this.hitAnim, true);
        this.m_aiActor.CollisionKnockbackStrength = this.m_cachedKnockback;
        this.m_aiActor.CollisionDamage = this.m_cachedDamage;
        this.m_aiActor.CollisionVFX = this.m_cachedVfx;
        if ((bool) (UnityEngine.Object) this.m_trailVfx)
        {
          ParticleKiller component = this.m_trailVfx.GetComponent<ParticleKiller>();
          if ((bool) (UnityEngine.Object) component)
            component.StopEmitting();
          else
            SpawnManager.Despawn(this.m_trailVfx);
          this.m_trailVfx = (GameObject) null;
        }
        this.m_aiActor.DoDustUps = this.m_cachedDoDustUps;
        this.m_aiActor.DustUpInterval = this.m_cachedDustUpInterval;
        this.m_aiActor.PathableTiles = this.m_cachedPathableTiles;
        this.m_aiAnimator.EndAnimationIf(this.chargeAnim);
      }

      private enum FireState
      {
        Idle,
        Charging,
        Bouncing,
      }
    }

}
