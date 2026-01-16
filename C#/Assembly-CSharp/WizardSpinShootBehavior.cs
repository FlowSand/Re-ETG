// Decompiled with JetBrains decompiler
// Type: WizardSpinShootBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable disable
public class WizardSpinShootBehavior : BasicAttackBehavior
{
  public bool LineOfSight = true;
  public string OverrideBulletName;
  public bool CanHitEnemies;
  public Transform ShootPoint;
  public int NumBullets;
  public int BulletCircleSpeed;
  public float BulletCircleRadius;
  public float FirstSpawnDelay;
  public float SpawnDelay;
  public bool PrefireUseAnimTime;
  public float PrefireDelay;
  public float FirstFireDelay;
  public float FireDelay;
  public float LeadAmount;
  public string CastVfx;
  public List<Light> CastLights;
  private WizardSpinShootBehavior.SpinShootState m_state;
  private float m_stateTimer;
  private bool m_isCharmed;
  private List<Tuple<Projectile, float>> m_bulletPositions = new List<Tuple<Projectile, float>>();
  private PixelCollider m_bulletCatcher;

  public override void Start()
  {
    base.Start();
    IntVector2 pixel1 = PhysicsEngine.UnitToPixel((Vector2) (this.ShootPoint.position - this.m_aiActor.transform.position));
    int pixel2 = PhysicsEngine.UnitToPixel(this.BulletCircleRadius);
    this.m_bulletCatcher = new PixelCollider();
    this.m_bulletCatcher.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Circle;
    this.m_bulletCatcher.CollisionLayer = CollisionLayer.BulletBlocker;
    this.m_bulletCatcher.IsTrigger = true;
    this.m_bulletCatcher.ManualOffsetX = pixel1.x - pixel2;
    this.m_bulletCatcher.ManualOffsetY = pixel1.y - pixel2;
    this.m_bulletCatcher.ManualDiameter = pixel2 * 2;
    this.m_bulletCatcher.Regenerate(this.m_aiActor.transform);
    this.m_aiActor.specRigidbody.PixelColliders.Add(this.m_bulletCatcher);
    this.m_aiActor.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnTriggerCollision);
    if (this.CastLights == null)
      return;
    for (int index = 0; index < this.CastLights.Count; ++index)
      this.CastLights[index].enabled = false;
  }

  private void OnTriggerCollision(
    SpeculativeRigidbody specRigidbody,
    SpeculativeRigidbody sourceSpecRigidbody,
    CollisionData collisionData)
  {
    if (this.State != WizardSpinShootBehavior.SpinShootState.Spawn && this.State != WizardSpinShootBehavior.SpinShootState.Prefire || collisionData.MyPixelCollider != this.m_bulletCatcher || !((UnityEngine.Object) collisionData.OtherRigidbody != (UnityEngine.Object) null) || !((UnityEngine.Object) collisionData.OtherRigidbody.projectile != (UnityEngine.Object) null))
      return;
    Projectile projectile = collisionData.OtherRigidbody.projectile;
    if (!(!this.m_isCharmed ? projectile.Owner is PlayerController : projectile.Owner is AIActor) || !projectile.CanBeCaught)
      return;
    projectile.specRigidbody.DeregisterSpecificCollisionException(projectile.Owner.specRigidbody);
    projectile.Shooter = this.m_aiActor.specRigidbody;
    projectile.Owner = (GameActor) this.m_aiActor;
    projectile.specRigidbody.Velocity = Vector2.zero;
    projectile.ManualControl = true;
    projectile.baseData.SetAll(this.m_aiActor.bulletBank.GetBullet().ProjectileData);
    projectile.UpdateSpeed();
    projectile.specRigidbody.CollideWithTileMap = false;
    projectile.ResetDistance();
    projectile.collidesWithEnemies = this.m_isCharmed;
    projectile.collidesWithPlayer = true;
    projectile.UpdateCollisionMask();
    projectile.sprite.color = new Color(1f, 0.1f, 0.1f);
    projectile.MakeLookLikeEnemyBullet();
    projectile.RemovePlayerOnlyModifiers();
    float second = BraveMathCollege.ClampAngle360((collisionData.Contact - this.ShootPoint.position.XY()).ToAngle());
    this.m_bulletPositions.Insert(Mathf.Max(0, this.m_bulletPositions.Count - 1), Tuple.Create<Projectile, float>(projectile, second));
  }

  public override void Upkeep()
  {
    base.Upkeep();
    this.m_stateTimer -= this.m_deltaTime;
    if (this.m_isCharmed == this.m_aiActor.CanTargetEnemies)
      return;
    this.m_isCharmed = this.m_aiActor.CanTargetEnemies;
    for (int index = 0; index < this.m_bulletPositions.Count; ++index)
    {
      Projectile first = this.m_bulletPositions[index].First;
      if (!((UnityEngine.Object) first == (UnityEngine.Object) null))
      {
        first.collidesWithEnemies = this.m_isCharmed;
        first.UpdateCollisionMask();
      }
    }
  }

  public override BehaviorResult Update()
  {
    int num = (int) base.Update();
    BehaviorResult behaviorResult = base.Update();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if (!this.IsReady() || !this.m_aiActor.HasLineOfSightToTarget)
      return BehaviorResult.Continue;
    this.State = WizardSpinShootBehavior.SpinShootState.Spawn;
    this.m_updateEveryFrame = true;
    return BehaviorResult.RunContinuous;
  }

  public override ContinuousBehaviorResult ContinuousUpdate()
  {
    for (int index = this.m_bulletPositions.Count - 1; index >= 0; --index)
    {
      float angle = this.m_bulletPositions[index].Second + this.m_deltaTime * (float) this.BulletCircleSpeed;
      this.m_bulletPositions[index].Second = angle;
      Projectile first = this.m_bulletPositions[index].First;
      if (!((UnityEngine.Object) first == (UnityEngine.Object) null))
      {
        if (!(bool) (UnityEngine.Object) first)
        {
          this.m_bulletPositions[index] = (Tuple<Projectile, float>) null;
        }
        else
        {
          Vector2 bulletPosition = this.GetBulletPosition(angle);
          first.specRigidbody.Velocity = (bulletPosition - (Vector2) first.transform.position) / BraveTime.DeltaTime;
          if (first.shouldRotate)
            first.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180f + (Quaternion.Euler(0.0f, 0.0f, 90f) * (Vector3) (this.ShootPoint.position.XY() - bulletPosition)).XY().ToAngle());
          first.ResetDistance();
        }
      }
    }
    if (this.State == WizardSpinShootBehavior.SpinShootState.Spawn)
    {
      while ((double) this.m_stateTimer <= 0.0 && this.State == WizardSpinShootBehavior.SpinShootState.Spawn)
      {
        AIBulletBank.Entry bullet = this.m_aiActor.bulletBank.GetBullet(this.OverrideBulletName);
        GameObject bulletObject = bullet.BulletObject;
        float num = 0.0f;
        if (this.m_bulletPositions.Count > 0)
          num = BraveMathCollege.ClampAngle360(this.m_bulletPositions[this.m_bulletPositions.Count - 1].Second - this.BulletAngleDelta);
        Projectile component = SpawnManager.SpawnProjectile(bulletObject, (Vector3) this.GetBulletPosition(num), Quaternion.Euler(0.0f, 0.0f, 0.0f)).GetComponent<Projectile>();
        if (bullet != null && bullet.OverrideProjectile)
          component.baseData.SetAll(bullet.ProjectileData);
        component.Shooter = this.m_aiActor.specRigidbody;
        component.specRigidbody.Velocity = Vector2.zero;
        component.ManualControl = true;
        component.specRigidbody.CollideWithTileMap = false;
        component.collidesWithEnemies = this.m_isCharmed;
        component.UpdateCollisionMask();
        this.m_bulletPositions.Add(Tuple.Create<Projectile, float>(component, num));
        this.m_stateTimer += this.SpawnDelay;
        if (this.m_bulletPositions.Count >= this.NumBullets)
          this.State = WizardSpinShootBehavior.SpinShootState.Prefire;
      }
    }
    else if (this.State == WizardSpinShootBehavior.SpinShootState.Prefire)
    {
      if ((double) this.m_stateTimer <= 0.0)
        this.State = WizardSpinShootBehavior.SpinShootState.Fire;
    }
    else if (this.State == WizardSpinShootBehavior.SpinShootState.Fire)
    {
      if (this.m_behaviorSpeculator.TargetBehaviors != null && this.m_behaviorSpeculator.TargetBehaviors.Count > 0)
      {
        int num1 = (int) this.m_behaviorSpeculator.TargetBehaviors[0].Update();
      }
      if (this.m_bulletPositions.All<Tuple<Projectile, float>>((Func<Tuple<Projectile, float>, bool>) (t => (UnityEngine.Object) t.First == (UnityEngine.Object) null)))
        return ContinuousBehaviorResult.Finished;
      while ((double) this.m_stateTimer <= 0.0)
      {
        Vector2 vector2_1 = this.ShootPoint.position.XY();
        Vector2 vector2_2 = !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody ? Vector2.zero : this.m_aiActor.TargetRigidbody.UnitCenter - vector2_1;
        Vector2 vector2_3 = vector2_1 + vector2_2.normalized * this.BulletCircleRadius;
        int index1 = -1;
        float num2 = float.MaxValue;
        for (int index2 = 0; index2 < this.m_bulletPositions.Count; ++index2)
        {
          Projectile first = this.m_bulletPositions[index2].First;
          if (!((UnityEngine.Object) first == (UnityEngine.Object) null))
          {
            float sqrMagnitude = (first.specRigidbody.UnitCenter - vector2_3).sqrMagnitude;
            if ((double) sqrMagnitude < (double) num2)
            {
              num2 = sqrMagnitude;
              index1 = index2;
            }
          }
        }
        if (index1 >= 0)
        {
          Projectile first = this.m_bulletPositions[index1].First;
          first.ManualControl = false;
          first.specRigidbody.CollideWithTileMap = true;
          if ((bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
          {
            Vector2 unitCenter = this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
            float speed = first.Speed;
            float num3 = Vector2.Distance(first.specRigidbody.UnitCenter, unitCenter) / speed;
            Vector2 b = unitCenter + this.m_aiActor.TargetRigidbody.specRigidbody.Velocity * num3;
            Vector2 vector2_4 = Vector2.Lerp(unitCenter, b, this.LeadAmount);
            first.SendInDirection(vector2_4 - first.specRigidbody.UnitCenter, true);
          }
          first.transform.rotation = Quaternion.Euler(0.0f, 0.0f, first.specRigidbody.Velocity.ToAngle());
          this.m_bulletPositions[index1].First = (Projectile) null;
        }
        else
          Debug.LogError((object) "WizardSpinShootBehaviour.ContinuousUpdate(): This shouldn't happen!");
        this.m_stateTimer += this.FireDelay;
        if (this.m_bulletPositions.All<Tuple<Projectile, float>>((Func<Tuple<Projectile, float>, bool>) (t => (UnityEngine.Object) t.First == (UnityEngine.Object) null)))
          return ContinuousBehaviorResult.Finished;
      }
    }
    return ContinuousBehaviorResult.Continue;
  }

  public override void EndContinuousUpdate()
  {
    base.EndContinuousUpdate();
    this.FreeRemainingProjectiles();
    this.State = WizardSpinShootBehavior.SpinShootState.None;
    this.m_aiAnimator.EndAnimationIf("attack");
    this.UpdateCooldowns();
    this.m_updateEveryFrame = false;
  }

  public override void OnActorPreDeath()
  {
    base.OnActorPreDeath();
    this.m_aiActor.specRigidbody.OnTriggerCollision -= new SpeculativeRigidbody.OnTriggerDelegate(this.OnTriggerCollision);
    this.FreeRemainingProjectiles();
  }

  public override void Destroy()
  {
    base.Destroy();
    this.State = WizardSpinShootBehavior.SpinShootState.None;
  }

  private float BulletAngleDelta => 360f / (float) this.NumBullets;

  private Vector2 GetBulletPosition(float angle)
  {
    return this.ShootPoint.position.XY() + new Vector2(Mathf.Cos(angle * ((float) Math.PI / 180f)), Mathf.Sin(angle * ((float) Math.PI / 180f))) * this.BulletCircleRadius;
  }

  private WizardSpinShootBehavior.SpinShootState State
  {
    get => this.m_state;
    set
    {
      this.EndState(this.m_state);
      this.m_state = value;
      this.BeginState(this.m_state);
    }
  }

  private void BeginState(WizardSpinShootBehavior.SpinShootState state)
  {
    if (state == WizardSpinShootBehavior.SpinShootState.None)
      this.m_bulletPositions.Clear();
    if (state == WizardSpinShootBehavior.SpinShootState.Spawn)
    {
      this.m_aiAnimator.PlayUntilCancelled("cast", true);
      this.m_stateTimer = this.FirstSpawnDelay;
      if (!string.IsNullOrEmpty(this.CastVfx))
        this.m_aiAnimator.PlayVfx(this.CastVfx);
      if (this.CastLights != null)
      {
        for (int index = 0; index < this.CastLights.Count; ++index)
          this.CastLights[index].enabled = true;
      }
      if ((bool) (UnityEngine.Object) this.m_aiActor && (bool) (UnityEngine.Object) this.m_aiActor.knockbackDoer)
        this.m_aiActor.knockbackDoer.SetImmobile(true, nameof (WizardSpinShootBehavior));
      this.m_aiActor.ClearPath();
    }
    else if (state == WizardSpinShootBehavior.SpinShootState.Prefire)
    {
      this.m_aiAnimator.PlayUntilFinished("attack", true);
      this.m_stateTimer = this.PrefireDelay;
      if (!this.PrefireUseAnimTime)
        return;
      this.m_stateTimer += (float) this.m_aiAnimator.spriteAnimator.CurrentClip.frames.Length / this.m_aiAnimator.spriteAnimator.CurrentClip.fps;
    }
    else
    {
      if (state != WizardSpinShootBehavior.SpinShootState.Fire)
        return;
      this.m_stateTimer = this.FirstFireDelay;
    }
  }

  private void EndState(WizardSpinShootBehavior.SpinShootState state)
  {
    if (state == WizardSpinShootBehavior.SpinShootState.Spawn)
    {
      this.m_aiAnimator.EndAnimationIf("cast");
      if (!string.IsNullOrEmpty(this.CastVfx))
        this.m_aiAnimator.StopVfx(this.CastVfx);
      if (this.CastLights != null)
      {
        for (int index = 0; index < this.CastLights.Count; ++index)
          this.CastLights[index].enabled = false;
      }
    }
    if (!(bool) (UnityEngine.Object) this.m_aiActor || !(bool) (UnityEngine.Object) this.m_aiActor.knockbackDoer)
      return;
    this.m_aiActor.knockbackDoer.SetImmobile(false, nameof (WizardSpinShootBehavior));
  }

  private void FreeRemainingProjectiles()
  {
    for (int index = 0; index < this.m_bulletPositions.Count; ++index)
    {
      Projectile first = this.m_bulletPositions[index].First;
      if (!((UnityEngine.Object) first == (UnityEngine.Object) null))
      {
        first.ManualControl = false;
        first.specRigidbody.CollideWithTileMap = true;
        first.SendInDirection((Quaternion.Euler(0.0f, 0.0f, 90f) * (Vector3) (first.specRigidbody.UnitCenter - this.ShootPoint.position.XY())).XY(), true);
        first.transform.rotation = Quaternion.Euler(0.0f, 0.0f, first.specRigidbody.Velocity.ToAngle());
        this.m_bulletPositions[index].First = (Projectile) null;
      }
    }
  }

  private enum SpinShootState
  {
    None,
    Spawn,
    Prefire,
    Fire,
  }
}
