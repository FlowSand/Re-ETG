// Decompiled with JetBrains decompiler
// Type: LeapExplosion
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using UnityEngine;

#nullable disable
public class LeapExplosion : AttackBehaviorBase
{
  public float minLeapDistance = 1f;
  public float leapDistance = 4f;
  public float maxTravelDistance = 5f;
  public float leadAmount;
  public float leapTime = 0.75f;
  public string chargeAnim;
  public string leapAnim;
  private PixelCollider m_enemyCollider;
  private float m_elapsed;
  private LeapExplosion.State m_state;

  public override void Start()
  {
    base.Start();
    for (int index = 0; index < this.m_aiActor.specRigidbody.PixelColliders.Count; ++index)
    {
      PixelCollider pixelCollider = this.m_aiActor.specRigidbody.PixelColliders[index];
      if (pixelCollider.CollisionLayer == CollisionLayer.EnemyCollider)
        this.m_enemyCollider = pixelCollider;
    }
  }

  public override void Upkeep() => base.Upkeep();

  public override BehaviorResult Update()
  {
    int num1 = (int) base.Update();
    BehaviorResult behaviorResult = base.Update();
    if (behaviorResult != BehaviorResult.Continue)
      return behaviorResult;
    if (!(bool) (Object) this.m_aiActor.TargetRigidbody)
      return BehaviorResult.Continue;
    Vector2 vector2 = this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
    if ((double) this.leadAmount > 0.0)
    {
      Vector2 b = vector2 + this.m_aiActor.TargetRigidbody.specRigidbody.Velocity * 0.75f;
      vector2 = Vector2.Lerp(vector2, b, this.leadAmount);
    }
    float num2 = Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, vector2);
    if ((double) num2 <= (double) this.minLeapDistance || (double) num2 >= (double) this.leapDistance)
      return BehaviorResult.Continue;
    this.m_state = LeapExplosion.State.Charging;
    this.m_aiAnimator.PlayUntilFinished(this.chargeAnim, true);
    this.m_aiActor.ClearPath();
    this.m_aiActor.BehaviorOverridesVelocity = true;
    this.m_aiActor.BehaviorVelocity = Vector2.zero;
    this.m_updateEveryFrame = true;
    return BehaviorResult.RunContinuous;
  }

  public override ContinuousBehaviorResult ContinuousUpdate()
  {
    if (this.m_state == LeapExplosion.State.Charging)
    {
      if (!this.m_aiAnimator.IsPlaying(this.chargeAnim))
      {
        this.m_state = LeapExplosion.State.Leaping;
        if (!(bool) (Object) this.m_aiActor.TargetRigidbody)
        {
          this.m_state = LeapExplosion.State.Idle;
          return ContinuousBehaviorResult.Finished;
        }
        Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
        Vector2 vector2 = this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
        if ((double) this.leadAmount > 0.0)
        {
          Vector2 b = vector2 + this.m_aiActor.TargetRigidbody.specRigidbody.Velocity * 0.75f;
          vector2 = Vector2.Lerp(vector2, b, this.leadAmount);
        }
        float num = Vector2.Distance(unitCenter, vector2);
        if ((double) num > (double) this.maxTravelDistance)
        {
          vector2 = unitCenter + (vector2 - unitCenter).normalized * this.maxTravelDistance;
          num = Vector2.Distance(unitCenter, vector2);
        }
        this.m_aiActor.ClearPath();
        this.m_aiActor.BehaviorOverridesVelocity = true;
        this.m_aiActor.BehaviorVelocity = (vector2 - unitCenter).normalized * (num / this.leapTime);
        float angle = this.m_aiActor.BehaviorVelocity.ToAngle();
        this.m_aiAnimator.LockFacingDirection = true;
        this.m_aiAnimator.FacingDirection = angle;
        this.m_aiActor.PathableTiles = CellTypes.FLOOR | CellTypes.PIT;
        this.m_enemyCollider.CollisionLayer = CollisionLayer.TileBlocker;
        this.m_aiActor.specRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.BulletBlocker, CollisionLayer.EnemyBulletBlocker));
        this.m_aiActor.DoDustUps = false;
        this.m_aiAnimator.PlayUntilCancelled(this.leapAnim, true);
      }
    }
    else if (this.m_state == LeapExplosion.State.Leaping)
    {
      this.m_elapsed += this.m_deltaTime;
      if ((double) this.m_elapsed >= (double) this.leapTime)
        return ContinuousBehaviorResult.Finished;
    }
    return ContinuousBehaviorResult.Continue;
  }

  public override void EndContinuousUpdate()
  {
    base.EndContinuousUpdate();
    if (this.m_aiActor.healthHaver.IsAlive)
      this.m_aiActor.healthHaver.ApplyDamage(float.MaxValue, Vector2.zero, "self-immolation", CoreDamageTypes.Fire, DamageCategory.Unstoppable, true);
    this.m_updateEveryFrame = false;
  }

  public override bool IsReady() => true;

  public override float GetMinReadyRange() => this.leapDistance;

  public override float GetMaxRange() => this.leapDistance;

  private enum State
  {
    Idle,
    Charging,
    Leaping,
  }
}
