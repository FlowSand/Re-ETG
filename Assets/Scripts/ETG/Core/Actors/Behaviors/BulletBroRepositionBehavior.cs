using Dungeonator;
using FullInspector;
using System;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/BulletBro/RepositionBehavior")]
public class BulletBroRepositionBehavior : BasicAttackBehavior
  {
    public float triggerAngle = 30f;
    public string primeAnim;
    public string chargeAnim;
    public string hitAnim;
    public float chargeSpeed;
    public float chargeKnockback = 50f;
    public float chargeDamage = 0.5f;
    public bool HideGun;
    public GameObject launchVfx;
    public GameObject trailVfx;
    public Transform trailVfxParent;
    public GameObject hitVfx;
    [InspectorCategory("Conditions")]
    public float StaticCooldown;
    private BulletBroRepositionBehavior.FireState m_state;
    private AIActor m_otherBro;
    private Vector2 m_targetCenter;
    private float m_lastAngleToTarget;
    private float m_cachedKnockback;
    private float m_cachedDamage;
    private VFXPool m_cachedVfx;
    private CellTypes m_cachedPathableTiles;
    private bool m_cachedDoDustUps;
    private GameObject m_trailVfx;
    private Vector2 m_cachedTargetCenter;
    private static float s_staticCooldown;
    private static int s_lastStaticUpdateFrameNum = -1;

    public override void Start()
    {
      base.Start();
      this.m_aiActor.specRigidbody.OnCollision += new Action<CollisionData>(this.OnCollision);
      this.m_cachedKnockback = this.m_aiActor.CollisionKnockbackStrength;
      this.m_cachedDamage = this.m_aiActor.CollisionDamage;
      this.m_cachedVfx = this.m_aiActor.CollisionVFX;
      this.m_cachedPathableTiles = this.m_aiActor.PathableTiles;
      this.m_cachedDoDustUps = this.m_aiActor.DoDustUps;
      this.m_otherBro = BroController.GetOtherBro(this.m_aiActor.gameObject).aiActor;
    }

    public override void Upkeep()
    {
      base.Upkeep();
      if ((double) BulletBroRepositionBehavior.s_staticCooldown <= 0.0 || BulletBroRepositionBehavior.s_lastStaticUpdateFrameNum == UnityEngine.Time.frameCount)
        return;
      BulletBroRepositionBehavior.s_staticCooldown = Mathf.Max(0.0f, BulletBroRepositionBehavior.s_staticCooldown - this.m_deltaTime);
      BulletBroRepositionBehavior.s_lastStaticUpdateFrameNum = UnityEngine.Time.frameCount;
    }

    public override BehaviorResult Update()
    {
      int num = (int) base.Update();
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!(bool) (UnityEngine.Object) this.m_otherBro || !this.IsReady() || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
        return BehaviorResult.Continue;
      Vector2 unitCenter1 = this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
      Vector2 unitCenter2 = this.m_aiActor.specRigidbody.UnitCenter;
      Vector2 unitCenter3 = this.m_otherBro.specRigidbody.UnitCenter;
      if ((double) BraveMathCollege.AbsAngleBetween((unitCenter2 - unitCenter1).ToAngle(), (unitCenter3 - unitCenter1).ToAngle()) < (double) this.triggerAngle)
      {
        Vector2 vector2 = unitCenter1 - unitCenter3;
        this.m_targetCenter = unitCenter3 + vector2 + vector2.normalized * 7f;
        this.m_lastAngleToTarget = (this.m_targetCenter - unitCenter2).ToAngle();
        this.State = BulletBroRepositionBehavior.FireState.Priming;
        BulletBroRepositionBehavior.s_staticCooldown += this.StaticCooldown;
        this.m_updateEveryFrame = true;
        return BehaviorResult.RunContinuous;
      }
      this.m_cachedTargetCenter = this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
      return BehaviorResult.Continue;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      if (this.State == BulletBroRepositionBehavior.FireState.Priming)
      {
        if (!this.m_aiAnimator.IsPlaying(this.primeAnim))
        {
          if (!(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
            return ContinuousBehaviorResult.Finished;
          this.State = BulletBroRepositionBehavior.FireState.Charging;
        }
      }
      else if (this.State == BulletBroRepositionBehavior.FireState.Charging)
      {
        Vector2 cachedTargetCenter = this.m_cachedTargetCenter;
        if ((bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
          this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
        Vector2 unitCenter1 = this.m_aiActor.specRigidbody.UnitCenter;
        if ((bool) (UnityEngine.Object) this.m_otherBro)
        {
          Vector2 unitCenter2 = this.m_otherBro.specRigidbody.UnitCenter;
          Vector2 vector2 = cachedTargetCenter - unitCenter2;
          this.m_targetCenter = unitCenter2 + vector2 + vector2.normalized * 7f;
        }
        float angle = (this.m_targetCenter - unitCenter1).ToAngle();
        if ((double) BraveMathCollege.AbsAngleBetween(angle, this.m_lastAngleToTarget) > 135.0)
          return ContinuousBehaviorResult.Finished;
        this.m_aiActor.BehaviorVelocity = (this.m_targetCenter - unitCenter1).normalized * this.chargeSpeed;
        this.m_aiAnimator.FacingDirection = this.m_aiActor.BehaviorVelocity.ToAngle();
        this.m_lastAngleToTarget = angle;
      }
      else if (this.State == BulletBroRepositionBehavior.FireState.Bouncing && !this.m_aiAnimator.IsPlaying(this.hitAnim))
        return ContinuousBehaviorResult.Finished;
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      this.m_updateEveryFrame = false;
      this.State = BulletBroRepositionBehavior.FireState.Idle;
      this.UpdateCooldowns();
    }

    public override bool IsReady()
    {
      return base.IsReady() && (double) BulletBroRepositionBehavior.s_staticCooldown <= 0.0;
    }

    private void OnCollision(CollisionData collisionData)
    {
      if (this.State != BulletBroRepositionBehavior.FireState.Charging || this.m_aiActor.healthHaver.IsDead)
        return;
      this.State = BulletBroRepositionBehavior.FireState.Bouncing;
    }

    private BulletBroRepositionBehavior.FireState State
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

    private void BeginState(BulletBroRepositionBehavior.FireState state)
    {
      switch (state)
      {
        case BulletBroRepositionBehavior.FireState.Idle:
          if (this.HideGun)
            this.m_aiShooter.ToggleGunAndHandRenderers(true, nameof (BulletBroRepositionBehavior));
          this.m_aiActor.BehaviorOverridesVelocity = false;
          this.m_aiAnimator.LockFacingDirection = false;
          break;
        case BulletBroRepositionBehavior.FireState.Priming:
          if (this.HideGun)
            this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (BulletBroRepositionBehavior));
          this.m_aiAnimator.PlayUntilFinished(this.primeAnim, true);
          this.m_aiActor.ClearPath();
          this.m_aiActor.BehaviorOverridesVelocity = true;
          this.m_aiActor.BehaviorVelocity = Vector2.zero;
          break;
        case BulletBroRepositionBehavior.FireState.Charging:
          if (this.HideGun)
            this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (BulletBroRepositionBehavior));
          this.m_aiActor.ClearPath();
          this.m_aiActor.BehaviorOverridesVelocity = true;
          this.m_aiActor.BehaviorVelocity = (this.m_targetCenter - this.m_aiActor.specRigidbody.UnitCenter).normalized * this.chargeSpeed;
          float angle = this.m_aiActor.BehaviorVelocity.ToAngle();
          this.m_aiAnimator.LockFacingDirection = true;
          this.m_aiAnimator.FacingDirection = angle;
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
          this.m_aiActor.DoDustUps = false;
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
          break;
      }
    }

    private void EndState(BulletBroRepositionBehavior.FireState state)
    {
      if (state != BulletBroRepositionBehavior.FireState.Charging)
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
      this.m_aiActor.PathableTiles = this.m_cachedPathableTiles;
      this.m_aiAnimator.EndAnimationIf(this.chargeAnim);
    }

    private void TestTargetPosition(
      IntVector2 testPos,
      float broAngleToTarget,
      Vector2 targetCenter,
      ref IntVector2? targetPos,
      ref float targetAngleFromBro)
    {
      float num = BraveMathCollege.AbsAngleBetween(broAngleToTarget, (testPos.ToCenterVector2() - targetCenter).ToAngle());
      if ((double) num <= (double) targetAngleFromBro)
        return;
      targetPos = new IntVector2?(testPos);
      targetAngleFromBro = num;
    }

    private enum FireState
    {
      Idle,
      Priming,
      Charging,
      Bouncing,
    }
  }

