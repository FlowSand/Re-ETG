using Dungeonator;
using FullInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class ChargeBehavior : BasicAttackBehavior
  {
    [InspectorCategory("Conditions")]
    public float minRange;
    [InspectorHeader("Prime")]
    public float primeTime = -1f;
    public bool stopDuringPrime = true;
    [InspectorHeader("Charge")]
    public float leadAmount;
    public float chargeSpeed;
    public float chargeAcceleration = -1f;
    public float maxChargeDistance = -1f;
    public float chargeKnockback = 50f;
    public float chargeDamage = 0.5f;
    public float wallRecoilForce = 10f;
    public bool stoppedByProjectiles = true;
    public bool endWhenChargeAnimFinishes;
    public bool switchCollidersOnCharge;
    public bool collidesWithDodgeRollingPlayers = true;
    [InspectorCategory("Attack")]
    public GameObject ShootPoint;
    [InspectorCategory("Attack")]
    public BulletScriptSelector bulletScript;
    [InspectorCategory("Visuals")]
    public string primeAnim;
    [InspectorCategory("Visuals")]
    public string chargeAnim;
    [InspectorCategory("Visuals")]
    public string hitAnim;
    [InspectorCategory("Visuals")]
    public bool HideGun;
    [InspectorCategory("Visuals")]
    public GameObject launchVfx;
    [InspectorCategory("Visuals")]
    public GameObject trailVfx;
    [InspectorCategory("Visuals")]
    public Transform trailVfxParent;
    [InspectorCategory("Visuals")]
    public GameObject hitVfx;
    [InspectorCategory("Visuals")]
    public GameObject nonActorHitVfx;
    [InspectorCategory("Visuals")]
    public bool chargeDustUps;
    [InspectorShowIf("chargeDustUps")]
    [InspectorCategory("Visuals")]
    [InspectorIndent]
    public float chargeDustUpInterval;
    private BulletScriptSource m_bulletSource;
    private bool m_initialized;
    private float m_timer;
    private float m_chargeTime;
    private float m_cachedKnockback;
    private float m_cachedDamage;
    private VFXPool m_cachedVfx;
    private VFXPool m_cachedNonActorWallVfx;
    private float m_currentSpeed;
    private float m_chargeDirection;
    private CellTypes m_cachedPathableTiles;
    private bool m_cachedDoDustUps;
    private float m_cachedDustUpInterval;
    private PixelCollider m_enemyCollider;
    private PixelCollider m_enemyHitbox;
    private PixelCollider m_projectileCollider;
    private GameObject m_trailVfx;
    private Vector2 m_collisionNormal;
    private ChargeBehavior.FireState m_state;

    public override void Start()
    {
      base.Start();
      this.m_cachedKnockback = this.m_aiActor.CollisionKnockbackStrength;
      this.m_cachedDamage = this.m_aiActor.CollisionDamage;
      this.m_cachedVfx = this.m_aiActor.CollisionVFX;
      this.m_cachedNonActorWallVfx = this.m_aiActor.NonActorCollisionVFX;
      this.m_cachedPathableTiles = this.m_aiActor.PathableTiles;
      this.m_cachedDoDustUps = this.m_aiActor.DoDustUps;
      this.m_cachedDustUpInterval = this.m_aiActor.DustUpInterval;
      if (this.switchCollidersOnCharge)
      {
        for (int index = 0; index < this.m_aiActor.specRigidbody.PixelColliders.Count; ++index)
        {
          PixelCollider pixelCollider = this.m_aiActor.specRigidbody.PixelColliders[index];
          if (pixelCollider.CollisionLayer == CollisionLayer.EnemyCollider)
            this.m_enemyCollider = pixelCollider;
          if (pixelCollider.CollisionLayer == CollisionLayer.EnemyHitBox)
            this.m_enemyHitbox = pixelCollider;
          if (!pixelCollider.Enabled && pixelCollider.CollisionLayer == CollisionLayer.Projectile)
          {
            this.m_projectileCollider = pixelCollider;
            this.m_projectileCollider.CollisionLayerCollidableOverride |= CollisionMask.LayerToMask(CollisionLayer.Projectile);
          }
        }
      }
      if (this.collidesWithDodgeRollingPlayers)
        return;
      this.m_aiActor.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.OnPreRigidbodyCollision);
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_timer);
    }

    public override BehaviorResult Update()
    {
      int num = (int) base.Update();
      if (!this.m_initialized)
      {
        this.m_aiActor.specRigidbody.OnCollision += new Action<CollisionData>(this.OnCollision);
        this.m_initialized = true;
      }
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady() || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
        return BehaviorResult.Continue;
      Vector2 vector2_1 = this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
      if ((double) this.leadAmount > 0.0)
      {
        Vector2 vector2_2 = vector2_1 + this.m_aiActor.TargetRigidbody.specRigidbody.Velocity * 0.75f;
        Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(vector2_1, this.m_aiActor.TargetVelocity, this.m_aiActor.specRigidbody.UnitCenter, this.chargeSpeed);
        vector2_1 = Vector2.Lerp(vector2_1, predictedPosition, this.leadAmount);
      }
      if ((double) Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, vector2_1) <= (double) this.minRange)
        return BehaviorResult.Continue;
      this.State = !string.IsNullOrEmpty(this.primeAnim) || (double) this.primeTime > 0.0 ? ChargeBehavior.FireState.Priming : ChargeBehavior.FireState.Charging;
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      if (this.State == ChargeBehavior.FireState.Priming)
      {
        if (!(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
          return ContinuousBehaviorResult.Finished;
        if ((double) this.m_timer > 0.0)
        {
          float facingDirection = this.m_aiAnimator.FacingDirection;
          float b = BraveMathCollege.ClampAngle180((this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_aiActor.specRigidbody.UnitCenter).ToAngle() - facingDirection);
          this.m_aiAnimator.FacingDirection = facingDirection + Mathf.Lerp(0.0f, b, this.m_deltaTime / (this.m_timer + this.m_deltaTime));
        }
        if (!this.stopDuringPrime)
          this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_aiAnimator.FacingDirection, Mathf.Lerp(this.m_aiActor.BehaviorVelocity.magnitude, 0.0f, this.m_deltaTime / (this.m_timer + this.m_deltaTime)));
        if (((double) this.primeTime <= 0.0 ? (!this.m_aiAnimator.IsPlaying(this.primeAnim) ? 1 : 0) : ((double) this.m_timer <= 0.0 ? 1 : 0)) != 0)
          this.State = ChargeBehavior.FireState.Charging;
      }
      else if (this.State == ChargeBehavior.FireState.Charging)
      {
        if ((double) this.chargeAcceleration > 0.0)
        {
          this.m_currentSpeed = Mathf.Min(this.chargeSpeed, this.m_currentSpeed + this.chargeAcceleration * this.m_deltaTime);
          this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_chargeDirection, this.m_currentSpeed);
        }
        if (this.endWhenChargeAnimFinishes && !this.m_aiAnimator.IsPlaying(this.chargeAnim))
          return ContinuousBehaviorResult.Finished;
        if ((double) this.maxChargeDistance > 0.0)
        {
          this.m_chargeTime += this.m_deltaTime;
          if ((double) this.m_chargeTime * (double) this.chargeSpeed > (double) this.maxChargeDistance)
            return ContinuousBehaviorResult.Finished;
        }
      }
      else if (this.State == ChargeBehavior.FireState.Bouncing)
      {
        if (!this.m_aiAnimator.IsPlaying(this.hitAnim))
          return ContinuousBehaviorResult.Finished;
      }
      else if (this.State == ChargeBehavior.FireState.Idle)
        return ContinuousBehaviorResult.Finished;
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      this.m_updateEveryFrame = false;
      this.State = ChargeBehavior.FireState.Idle;
      this.UpdateCooldowns();
    }

    public override void Destroy()
    {
      if ((bool) (UnityEngine.Object) this.m_aiActor)
        this.m_aiActor.specRigidbody.OnPostRigidbodyMovement -= new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.OnPostRigidbodyMovement);
      base.Destroy();
    }

    private void Fire()
    {
      if (!(bool) (UnityEngine.Object) this.m_bulletSource)
        this.m_bulletSource = this.ShootPoint.GetOrAddComponent<BulletScriptSource>();
      this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
      this.m_bulletSource.BulletScript = this.bulletScript;
      this.m_bulletSource.Initialize();
    }

    private void OnPreRigidbodyCollision(
      SpeculativeRigidbody myRigidbody,
      PixelCollider myPixelCollider,
      SpeculativeRigidbody otherRigidbody,
      PixelCollider otherPixelCollider)
    {
      if (this.m_state != ChargeBehavior.FireState.Charging)
        return;
      PlayerController gameActor = otherRigidbody.gameActor as PlayerController;
      if (!(bool) (UnityEngine.Object) gameActor || !gameActor.spriteAnimator.QueryInvulnerabilityFrame())
        return;
      PhysicsEngine.SkipCollision = true;
    }

    private void OnCollision(CollisionData collisionData)
    {
      if (this.State != ChargeBehavior.FireState.Charging || this.m_aiActor.healthHaver.IsDead)
        return;
      if ((bool) (UnityEngine.Object) collisionData.OtherRigidbody)
      {
        Projectile projectile = collisionData.OtherRigidbody.projectile;
        if ((bool) (UnityEngine.Object) projectile && (!(projectile.Owner is PlayerController) || !this.stoppedByProjectiles))
          return;
      }
      this.State = string.IsNullOrEmpty(this.hitAnim) ? ChargeBehavior.FireState.Idle : ChargeBehavior.FireState.Bouncing;
      if (this.switchCollidersOnCharge)
      {
        PhysicsEngine.CollisionHaltsVelocity = new bool?(true);
        PhysicsEngine.HaltRemainingMovement = true;
        PhysicsEngine.PostSliceVelocity = new Vector2?(Vector2.zero);
        this.m_collisionNormal = collisionData.Normal;
        this.m_aiActor.specRigidbody.OnPostRigidbodyMovement += new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.OnPostRigidbodyMovement);
      }
      if ((bool) (UnityEngine.Object) collisionData.OtherRigidbody && (bool) (UnityEngine.Object) collisionData.OtherRigidbody.knockbackDoer)
        return;
      this.m_aiActor.knockbackDoer.ApplyKnockback(collisionData.Normal, this.wallRecoilForce);
    }

    private void OnPostRigidbodyMovement(
      SpeculativeRigidbody specRigidbody,
      Vector2 unitDelta,
      IntVector2 pixelDelta)
    {
      if (!(bool) (UnityEngine.Object) this.m_behaviorSpeculator)
        return;
      List<CollisionData> overlappingCollisions = new List<CollisionData>();
      bool flag = false;
      if (PhysicsEngine.Instance.OverlapCast(this.m_aiActor.specRigidbody, overlappingCollisions))
      {
        for (int index = 0; index < overlappingCollisions.Count; ++index)
        {
          SpeculativeRigidbody otherRigidbody = overlappingCollisions[index].OtherRigidbody;
          if ((bool) (UnityEngine.Object) otherRigidbody && (bool) (UnityEngine.Object) otherRigidbody.transform.parent && ((bool) (UnityEngine.Object) otherRigidbody.transform.parent.GetComponent<DungeonDoorSubsidiaryBlocker>() || (bool) (UnityEngine.Object) otherRigidbody.transform.parent.GetComponent<DungeonDoorController>()))
          {
            flag = true;
            break;
          }
        }
      }
      if (flag)
      {
        if ((double) this.m_collisionNormal.y >= 0.5)
          this.m_aiActor.transform.position += new Vector3(0.0f, 0.5f);
        if ((double) this.m_collisionNormal.x <= -0.5)
          this.m_aiActor.transform.position += new Vector3(-5f / 16f, 0.0f);
        if ((double) this.m_collisionNormal.x >= 0.5)
          this.m_aiActor.transform.position += new Vector3(5f / 16f, 0.0f);
        this.m_aiActor.specRigidbody.Reinitialize();
      }
      else
        PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.m_aiActor.specRigidbody);
      this.m_aiActor.specRigidbody.OnPostRigidbodyMovement -= new Action<SpeculativeRigidbody, Vector2, IntVector2>(this.OnPostRigidbodyMovement);
    }

    private ChargeBehavior.FireState State
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

    private void BeginState(ChargeBehavior.FireState state)
    {
      switch (state)
      {
        case ChargeBehavior.FireState.Idle:
          if (this.HideGun)
            this.m_aiShooter.ToggleGunAndHandRenderers(true, nameof (ChargeBehavior));
          this.m_aiActor.BehaviorOverridesVelocity = false;
          this.m_aiAnimator.LockFacingDirection = false;
          break;
        case ChargeBehavior.FireState.Priming:
          if (this.HideGun)
            this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (ChargeBehavior));
          this.m_aiAnimator.PlayUntilFinished(this.primeAnim, true);
          this.m_timer = (double) this.primeTime <= 0.0 ? this.m_aiAnimator.CurrentClipLength : this.primeTime;
          if (this.stopDuringPrime)
          {
            this.m_aiActor.ClearPath();
            this.m_aiActor.BehaviorOverridesVelocity = true;
            this.m_aiActor.BehaviorVelocity = Vector2.zero;
            break;
          }
          this.m_aiActor.BehaviorOverridesVelocity = true;
          this.m_aiActor.BehaviorVelocity = this.m_aiActor.specRigidbody.Velocity;
          break;
        case ChargeBehavior.FireState.Charging:
          if (this.HideGun)
            this.m_aiShooter.ToggleGunAndHandRenderers(false, nameof (ChargeBehavior));
          this.m_chargeTime = 0.0f;
          Vector2 vector2_1 = this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
          if ((double) this.leadAmount > 0.0)
          {
            Vector2 vector2_2 = vector2_1 + this.m_aiActor.TargetRigidbody.specRigidbody.Velocity * 0.75f;
            Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(vector2_1, this.m_aiActor.TargetVelocity, this.m_aiActor.specRigidbody.UnitCenter, this.chargeSpeed);
            vector2_1 = Vector2.Lerp(vector2_1, predictedPosition, this.leadAmount);
          }
          this.m_aiActor.ClearPath();
          this.m_aiActor.BehaviorOverridesVelocity = true;
          this.m_currentSpeed = (double) this.chargeAcceleration <= 0.0 ? this.chargeSpeed : 0.0f;
          this.m_chargeDirection = (vector2_1 - this.m_aiActor.specRigidbody.UnitCenter).ToAngle();
          this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_chargeDirection, this.m_currentSpeed);
          this.m_aiAnimator.LockFacingDirection = true;
          this.m_aiAnimator.FacingDirection = this.m_chargeDirection;
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
          if ((bool) (UnityEngine.Object) this.nonActorHitVfx)
            this.m_aiActor.NonActorCollisionVFX = new VFXPool()
            {
              type = VFXPoolType.Single,
              effects = new VFXComplex[1]
              {
                new VFXComplex()
                {
                  effects = new VFXObject[1]
                  {
                    new VFXObject() { effect = this.nonActorHitVfx }
                  }
                }
              }
            };
          this.m_aiActor.PathableTiles = CellTypes.FLOOR | CellTypes.PIT;
          if (this.switchCollidersOnCharge)
          {
            this.m_enemyCollider.CollisionLayer = CollisionLayer.TileBlocker;
            this.m_enemyHitbox.Enabled = false;
            this.m_projectileCollider.Enabled = true;
          }
          this.m_aiActor.DoDustUps = this.chargeDustUps;
          this.m_aiActor.DustUpInterval = this.chargeDustUpInterval;
          this.m_aiAnimator.PlayUntilFinished(this.chargeAnim, true);
          if ((bool) (UnityEngine.Object) this.launchVfx)
            SpawnManager.SpawnVFX(this.launchVfx, (Vector3) this.m_aiActor.specRigidbody.UnitCenter, Quaternion.identity);
          if ((bool) (UnityEngine.Object) this.trailVfx)
          {
            this.m_trailVfx = SpawnManager.SpawnParticleSystem(this.trailVfx, (Vector3) this.m_aiActor.sprite.WorldCenter, Quaternion.Euler(0.0f, 0.0f, this.m_chargeDirection));
            this.m_trailVfx.transform.parent = !(bool) (UnityEngine.Object) this.trailVfxParent ? this.m_aiActor.transform : this.trailVfxParent;
            ParticleKiller component = this.m_trailVfx.GetComponent<ParticleKiller>();
            if ((UnityEngine.Object) component != (UnityEngine.Object) null)
              component.Awake();
          }
          if (this.bulletScript != null && !this.bulletScript.IsNull)
            this.Fire();
          this.m_aiActor.specRigidbody.ForceRegenerate();
          break;
        case ChargeBehavior.FireState.Bouncing:
          this.m_aiAnimator.PlayUntilFinished(this.hitAnim, true);
          break;
      }
    }

    private void EndState(ChargeBehavior.FireState state)
    {
      if (state != ChargeBehavior.FireState.Charging)
        return;
      this.m_aiActor.BehaviorVelocity = Vector2.zero;
      this.m_aiActor.CollisionKnockbackStrength = this.m_cachedKnockback;
      this.m_aiActor.CollisionDamage = this.m_cachedDamage;
      this.m_aiActor.CollisionVFX = this.m_cachedVfx;
      this.m_aiActor.NonActorCollisionVFX = this.m_cachedNonActorWallVfx;
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
      if (this.switchCollidersOnCharge)
      {
        this.m_enemyCollider.CollisionLayer = CollisionLayer.EnemyCollider;
        this.m_enemyHitbox.Enabled = true;
        this.m_projectileCollider.Enabled = false;
      }
      if ((UnityEngine.Object) this.m_bulletSource != (UnityEngine.Object) null)
        this.m_bulletSource.ForceStop();
      this.m_aiAnimator.EndAnimationIf(this.chargeAnim);
    }

    private enum FireState
    {
      Idle,
      Priming,
      Charging,
      Bouncing,
    }
  }

