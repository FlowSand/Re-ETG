// Decompiled with JetBrains decompiler
// Type: ZeldaChargeBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using FullInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class ZeldaChargeBehavior : BasicAttackBehavior
{
  public string primeAnim;
  public string chargeAnim;
  public bool endWhenChargeAnimFinishes;
  public bool switchCollidersOnCharge;
  public string hitAnim;
  public string hitPlayerAnim;
  public float leadAmount;
  public float chargeRange = 15f;
  public float chargeSpeed;
  public float chargeKnockback = 50f;
  public float chargeDamage = 0.5f;
  public bool delayWallRecoil;
  public float wallRecoilForce = 10f;
  public bool stopAtPits = true;
  public GameObject launchVfx;
  public GameObject trailVfx;
  public Transform trailVfxParent;
  public GameObject hitVfx;
  public string trailVfxString;
  public string hitWallVfxString;
  [InspectorHeader("Impact BulletScript")]
  public GameObject shootPoint;
  public BulletScriptSelector impactBulletScript;
  private ZeldaChargeBehavior.FireState m_state;
  private float m_primeAnimTime;
  private Vector2? m_chargeDir;
  private Vector2? m_storedCollisionNormal;
  private bool m_hitPlayer;
  private bool m_hitWall;
  private float m_cachedKnockback;
  private float m_cachedDamage;
  private VFXPool m_cachedVfx;
  private CellTypes m_cachedPathableTiles;
  private bool m_cachedDoDustUps;
  private PixelCollider m_enemyCollider;
  private PixelCollider m_enemyHitbox;
  private PixelCollider m_projectileCollider;
  private GameObject m_trailVfx;
  private string m_cachedTrailString;
  private BulletScriptSource m_bulletSource;

  public override void Start()
  {
    base.Start();
    this.m_aiActor.specRigidbody.OnCollision += new Action<CollisionData>(this.OnCollision);
    this.m_cachedKnockback = this.m_aiActor.CollisionKnockbackStrength;
    this.m_cachedDamage = this.m_aiActor.CollisionDamage;
    this.m_cachedVfx = this.m_aiActor.CollisionVFX;
    this.m_cachedPathableTiles = this.m_aiActor.PathableTiles;
    this.m_cachedDoDustUps = this.m_aiActor.DoDustUps;
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
    if (this.stopAtPits)
      this.m_aiActor.specRigidbody.MovementRestrictor += new SpeculativeRigidbody.MovementRestrictorDelegate(this.PitMovementRestrictor);
    if (!string.IsNullOrEmpty(this.primeAnim))
      this.m_primeAnimTime = this.m_aiAnimator.GetDirectionalAnimationLength(this.primeAnim);
    this.m_aiActor.OverrideHitEnemies = true;
  }

  public override void Upkeep() => base.Upkeep();

  public override BehaviorResult Update()
  {
    int num = (int) base.Update();
    BehaviorResult behaviorResult = base.Update();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if (!this.IsReady())
      return BehaviorResult.Continue;
    for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
    {
      PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
      if ((bool) (UnityEngine.Object) allPlayer && !allPlayer.healthHaver.IsDead && !allPlayer.IsFalling && this.ShouldChargePlayer(GameManager.Instance.AllPlayers[index]))
      {
        this.State = ZeldaChargeBehavior.FireState.Priming;
        this.m_updateEveryFrame = true;
        return BehaviorResult.RunContinuous;
      }
    }
    return BehaviorResult.Continue;
  }

  public override ContinuousBehaviorResult ContinuousUpdate()
  {
    if (this.State == ZeldaChargeBehavior.FireState.Priming)
    {
      if (!this.m_aiAnimator.IsPlaying(this.primeAnim))
      {
        if (!(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
          return ContinuousBehaviorResult.Finished;
        this.State = ZeldaChargeBehavior.FireState.Charging;
      }
    }
    else if (this.State == ZeldaChargeBehavior.FireState.Charging)
    {
      if (this.endWhenChargeAnimFinishes && !this.m_aiAnimator.IsPlaying(this.chargeAnim))
        return ContinuousBehaviorResult.Finished;
    }
    else if (this.State == ZeldaChargeBehavior.FireState.Bouncing && !this.m_aiAnimator.IsPlaying(this.hitAnim) && !this.m_aiAnimator.IsPlaying(this.hitPlayerAnim))
    {
      if (this.delayWallRecoil && this.m_storedCollisionNormal.HasValue)
      {
        this.m_aiActor.knockbackDoer.ApplyKnockback(this.m_storedCollisionNormal.Value, this.wallRecoilForce);
        this.m_storedCollisionNormal = new Vector2?();
      }
      return ContinuousBehaviorResult.Finished;
    }
    return ContinuousBehaviorResult.Continue;
  }

  public override void EndContinuousUpdate()
  {
    base.EndContinuousUpdate();
    this.m_updateEveryFrame = false;
    this.State = ZeldaChargeBehavior.FireState.Idle;
    this.UpdateCooldowns();
  }

  public override void Destroy()
  {
    base.Destroy();
    if (!this.stopAtPits)
      return;
    this.m_aiActor.specRigidbody.MovementRestrictor -= new SpeculativeRigidbody.MovementRestrictorDelegate(this.PitMovementRestrictor);
  }

  private void OnCollision(CollisionData collisionData)
  {
    if (this.State != ZeldaChargeBehavior.FireState.Charging)
      return;
    if ((bool) (UnityEngine.Object) collisionData.OtherRigidbody)
    {
      SpeculativeRigidbody otherRigidbody = collisionData.OtherRigidbody;
      if ((bool) (UnityEngine.Object) otherRigidbody.projectile)
        return;
      if ((bool) (UnityEngine.Object) otherRigidbody.aiActor)
      {
        if (otherRigidbody.aiActor.OverrideHitEnemies)
        {
          float angle1 = collisionData.MyRigidbody.Velocity.ToAngle();
          float angle2 = collisionData.Normal.ToAngle();
          if ((double) Mathf.Abs(BraveMathCollege.ClampAngle180(angle1 - angle2)) <= 91.0)
            return;
          float magnitude1 = collisionData.MyRigidbody.Velocity.magnitude;
          float magnitude2 = otherRigidbody.Velocity.magnitude;
          float angle3 = otherRigidbody.Velocity.ToAngle();
          if ((double) Mathf.Abs(BraveMathCollege.ClampAngle180(angle1 - angle3)) < 45.0 && (double) magnitude1 < (double) magnitude2 * 1.25)
            return;
        }
        else
        {
          collisionData.OtherRigidbody.RegisterTemporaryCollisionException(collisionData.MyRigidbody, 0.1f);
          collisionData.MyRigidbody.RegisterTemporaryCollisionException(collisionData.OtherRigidbody, 0.1f);
          return;
        }
      }
    }
    this.m_hitPlayer = (bool) (UnityEngine.Object) collisionData.OtherRigidbody && (bool) (UnityEngine.Object) collisionData.OtherRigidbody.GetComponent<PlayerController>();
    this.m_hitWall = collisionData.collisionType == CollisionData.CollisionType.TileMap;
    this.State = ZeldaChargeBehavior.FireState.Bouncing;
    if (!(bool) (UnityEngine.Object) collisionData.OtherRigidbody || !(bool) (UnityEngine.Object) collisionData.OtherRigidbody.knockbackDoer)
    {
      if (this.delayWallRecoil)
      {
        this.m_storedCollisionNormal = new Vector2?(collisionData.Normal);
        if (collisionData.Normal == Vector2.zero)
        {
          Vector2? chargeDir = this.m_chargeDir;
          this.m_storedCollisionNormal = !chargeDir.HasValue ? new Vector2?() : new Vector2?(-chargeDir.Value);
        }
      }
      else
      {
        this.m_storedCollisionNormal = new Vector2?();
        this.m_aiActor.knockbackDoer.ApplyKnockback(collisionData.Normal, this.wallRecoilForce);
      }
    }
    else
      this.m_storedCollisionNormal = new Vector2?();
    if ((bool) (UnityEngine.Object) collisionData.OtherRigidbody || string.IsNullOrEmpty(this.hitWallVfxString))
      return;
    this.m_aiAnimator.PlayVfx(string.Format(this.hitWallVfxString, (double) this.m_storedCollisionNormal.Value.x >= -0.75 ? ((double) this.m_storedCollisionNormal.Value.x <= 0.75 ? ((double) this.m_storedCollisionNormal.Value.y >= -0.75 ? (object) "down" : (object) "up") : (object) "left") : (object) "right"));
  }

  private void PitMovementRestrictor(
    SpeculativeRigidbody specRigidbody,
    IntVector2 prevPixelOffset,
    IntVector2 pixelOffset,
    ref bool validLocation)
  {
    if (!validLocation)
      return;
    Func<IntVector2, bool> func = (Func<IntVector2, bool>) (pixel =>
    {
      Vector2 unitMidpoint = PhysicsEngine.PixelToUnitMidpoint(pixel);
      if (!GameManager.Instance.Dungeon.CellSupportsFalling((Vector3) unitMidpoint))
        return false;
      List<SpeculativeRigidbody> platformsAt = GameManager.Instance.Dungeon.GetPlatformsAt((Vector3) unitMidpoint);
      if (platformsAt != null)
      {
        for (int index = 0; index < platformsAt.Count; ++index)
        {
          if (platformsAt[index].PrimaryPixelCollider.ContainsPixel(pixel))
            return false;
        }
      }
      return true;
    });
    PixelCollider primaryPixelCollider = specRigidbody.PrimaryPixelCollider;
    if (primaryPixelCollider == null)
      return;
    IntVector2 intVector2 = pixelOffset - prevPixelOffset;
    if (intVector2 == IntVector2.Down && func(primaryPixelCollider.LowerLeft + pixelOffset) && func(primaryPixelCollider.LowerRight + pixelOffset) && (!func(primaryPixelCollider.UpperRight + prevPixelOffset) || !func(primaryPixelCollider.UpperLeft + prevPixelOffset)))
      validLocation = false;
    else if (intVector2 == IntVector2.Right && func(primaryPixelCollider.LowerRight + pixelOffset) && func(primaryPixelCollider.UpperRight + pixelOffset) && (!func(primaryPixelCollider.UpperLeft + prevPixelOffset) || !func(primaryPixelCollider.LowerLeft + prevPixelOffset)))
      validLocation = false;
    else if (intVector2 == IntVector2.Up && func(primaryPixelCollider.UpperRight + pixelOffset) && func(primaryPixelCollider.UpperLeft + pixelOffset) && (!func(primaryPixelCollider.LowerLeft + prevPixelOffset) || !func(primaryPixelCollider.LowerRight + prevPixelOffset)))
    {
      validLocation = false;
    }
    else
    {
      if (!(intVector2 == IntVector2.Left) || !func(primaryPixelCollider.UpperLeft + pixelOffset) || !func(primaryPixelCollider.LowerLeft + pixelOffset) || func(primaryPixelCollider.LowerRight + prevPixelOffset) && func(primaryPixelCollider.UpperRight + prevPixelOffset))
        return;
      validLocation = false;
    }
  }

  private ZeldaChargeBehavior.FireState State
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

  private void BeginState(ZeldaChargeBehavior.FireState state)
  {
    switch (state)
    {
      case ZeldaChargeBehavior.FireState.Idle:
        this.m_aiActor.BehaviorOverridesVelocity = false;
        this.m_aiAnimator.LockFacingDirection = false;
        break;
      case ZeldaChargeBehavior.FireState.Priming:
        this.m_aiAnimator.PlayUntilFinished(this.primeAnim, true);
        this.m_aiActor.ClearPath();
        this.m_aiActor.BehaviorOverridesVelocity = true;
        this.m_aiActor.BehaviorVelocity = Vector2.zero;
        break;
      case ZeldaChargeBehavior.FireState.Charging:
        int num = (int) AkSoundEngine.PostEvent("Play_ENM_cube_dash_01", GameManager.Instance.PrimaryPlayer.gameObject);
        this.m_aiActor.ClearPath();
        this.m_aiActor.BehaviorOverridesVelocity = true;
        this.m_aiActor.BehaviorVelocity = this.m_chargeDir.Value.normalized * this.chargeSpeed;
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
        if (this.switchCollidersOnCharge)
        {
          this.m_enemyCollider.CollisionLayer = CollisionLayer.TileBlocker;
          this.m_enemyHitbox.Enabled = false;
          this.m_projectileCollider.Enabled = true;
        }
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
        if (!string.IsNullOrEmpty(this.trailVfxString))
        {
          Vector2 normalized = this.m_aiActor.BehaviorVelocity.normalized;
          this.m_cachedTrailString = string.Format(this.trailVfxString, (double) normalized.x <= 0.75 ? ((double) normalized.x >= -0.75 ? ((double) normalized.y <= 0.75 ? (object) "down" : (object) "up") : (object) "left") : (object) "right");
          AIAnimator aiAnimator = this.m_aiAnimator;
          string cachedTrailString = this.m_cachedTrailString;
          Vector2? nullable = new Vector2?(normalized);
          string name = cachedTrailString;
          Vector2? sourceNormal = new Vector2?();
          Vector2? sourceVelocity = nullable;
          Vector2? position = new Vector2?();
          aiAnimator.PlayVfx(name, sourceNormal, sourceVelocity, position);
        }
        else
          this.m_cachedTrailString = (string) null;
        this.m_aiActor.specRigidbody.ForceRegenerate();
        break;
      case ZeldaChargeBehavior.FireState.Bouncing:
        if (!string.IsNullOrEmpty(this.hitPlayerAnim) && this.m_hitPlayer)
        {
          this.m_aiAnimator.PlayUntilFinished(this.hitPlayerAnim, true);
          if (this.m_aiAnimator.spriteAnimator.CurrentClip.wrapMode == tk2dSpriteAnimationClip.WrapMode.Loop)
            this.m_aiAnimator.PlayForDuration(this.hitPlayerAnim, 1f, true);
        }
        else
          this.m_aiAnimator.PlayUntilFinished(this.hitAnim, true);
        if (this.impactBulletScript == null || this.impactBulletScript.IsNull || !this.m_hitWall)
          break;
        if (!(bool) (UnityEngine.Object) this.m_bulletSource)
          this.m_bulletSource = this.shootPoint.GetOrAddComponent<BulletScriptSource>();
        this.m_bulletSource.BulletManager = this.m_aiActor.bulletBank;
        this.m_bulletSource.BulletScript = this.impactBulletScript;
        this.m_bulletSource.Initialize();
        break;
    }
  }

  private void EndState(ZeldaChargeBehavior.FireState state)
  {
    if (state != ZeldaChargeBehavior.FireState.Charging)
      return;
    this.m_aiActor.BehaviorVelocity = Vector2.zero;
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
    if (!string.IsNullOrEmpty(this.m_cachedTrailString))
    {
      this.m_aiAnimator.StopVfx(this.m_cachedTrailString);
      this.m_cachedTrailString = (string) null;
    }
    this.m_aiActor.DoDustUps = this.m_cachedDoDustUps;
    this.m_aiActor.PathableTiles = this.m_cachedPathableTiles;
    if (!this.switchCollidersOnCharge)
      return;
    this.m_enemyCollider.CollisionLayer = CollisionLayer.EnemyCollider;
    this.m_enemyHitbox.Enabled = true;
    this.m_projectileCollider.Enabled = false;
    PhysicsEngine.Instance.RegisterOverlappingGhostCollisionExceptions(this.m_aiActor.specRigidbody);
  }

  private bool ShouldChargePlayer(PlayerController player)
  {
    Vector2 vector2 = player.specRigidbody.GetUnitCenter(ColliderType.HitBox);
    if ((double) this.leadAmount > 0.0)
    {
      Vector2 b = vector2 + player.specRigidbody.Velocity * this.m_primeAnimTime;
      vector2 = Vector2.Lerp(vector2, b, this.leadAmount);
    }
    Vector2 unitBottomLeft = this.m_aiActor.specRigidbody.UnitBottomLeft;
    Vector2 unitTopRight = this.m_aiActor.specRigidbody.UnitTopRight;
    this.m_chargeDir = new Vector2?();
    if (BraveMathCollege.AABBContains(new Vector2(unitBottomLeft.x - this.chargeRange, unitBottomLeft.y), unitTopRight, vector2))
      this.m_chargeDir = new Vector2?(-Vector2.right);
    else if (BraveMathCollege.AABBContains(unitBottomLeft, new Vector2(unitTopRight.x + this.chargeRange, unitTopRight.y), vector2))
      this.m_chargeDir = new Vector2?(Vector2.right);
    else if (BraveMathCollege.AABBContains(new Vector2(unitBottomLeft.x, unitBottomLeft.y - this.chargeRange), unitTopRight, vector2))
      this.m_chargeDir = new Vector2?(-Vector2.up);
    else if (BraveMathCollege.AABBContains(unitBottomLeft, new Vector2(unitTopRight.x, unitTopRight.y + this.chargeRange), vector2))
      this.m_chargeDir = new Vector2?(Vector2.up);
    return this.m_chargeDir.HasValue;
  }

  private enum FireState
  {
    Idle,
    Priming,
    Charging,
    Bouncing,
  }
}
