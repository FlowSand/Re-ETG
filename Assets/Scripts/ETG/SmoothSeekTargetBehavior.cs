// Decompiled with JetBrains decompiler
// Type: SmoothSeekTargetBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using Pathfinding;
using UnityEngine;

#nullable disable
public class SmoothSeekTargetBehavior : RangedMovementBehavior
{
  public float turnTime = 1f;
  public float stoppedTurnMultiplier = 1f;
  public float targetTolerance;
  public bool slither;
  [InspectorShowIf("slither")]
  [InspectorIndent]
  public float slitherPeriod;
  [InspectorShowIf("slither")]
  [InspectorIndent]
  public float slitherMagnitude;
  public bool bob;
  [InspectorIndent]
  [InspectorShowIf("bob")]
  public float bobPeriod;
  [InspectorIndent]
  [InspectorShowIf("bob")]
  public float bobPeriodVariance;
  [InspectorIndent]
  [InspectorShowIf("bob")]
  public float bobMagnitude;
  public bool pathfind;
  [InspectorIndent]
  [InspectorShowIf("pathfind")]
  public float pathInterval = 0.25f;
  private Vector2 m_targetCenter;
  private float m_timer;
  private float m_pathTimer;
  private float m_direction = -90f;
  private float m_angularVelocity;
  private float m_slitherDirection;
  private float m_bobPeriod;
  private float m_lastBobOffset;
  private float m_timeSinceLastUpdate;

  public override void Start()
  {
    base.Start();
    this.m_updateEveryFrame = true;
    this.m_bobPeriod = this.bobPeriod + Random.Range(-this.bobPeriodVariance, this.bobPeriodVariance);
    this.m_direction = -90f;
    if ((bool) (Object) this.m_aiAnimator)
      this.m_aiAnimator.FacingDirection = -90f;
    this.m_targetCenter = this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.Ground);
  }

  public override void Upkeep()
  {
    base.Upkeep();
    this.m_timer += this.m_deltaTime;
    this.m_timeSinceLastUpdate += this.m_deltaTime;
    this.DecrementTimer(ref this.m_pathTimer);
  }

  public override BehaviorResult Update()
  {
    if ((double) this.m_timeSinceLastUpdate > 0.40000000596046448)
      this.m_direction = this.m_aiAnimator.FacingDirection;
    this.m_timeSinceLastUpdate = 0.0f;
    if (this.pathfind && !this.m_aiActor.HasLineOfSightToTarget)
    {
      if ((double) this.m_pathTimer <= 0.0)
      {
        this.UpdateTargetCenter();
        Path path = (Path) null;
        if (Pathfinder.Instance.GetPath(this.m_aiActor.PathTile, this.m_targetCenter.ToIntVector2(VectorConversions.Floor), out path, new IntVector2?(this.m_aiActor.Clearance), this.m_aiActor.PathableTiles) && path.Count > 0)
        {
          path.Smooth(this.m_aiActor.specRigidbody.UnitCenter, this.m_aiActor.specRigidbody.UnitDimensions / 2f, this.m_aiActor.PathableTiles, false, this.m_aiActor.Clearance);
          this.m_targetCenter = path.GetFirstCenterVector2();
        }
        this.m_pathTimer += this.pathInterval;
      }
    }
    else
      this.UpdateTargetCenter();
    float turnTime = this.turnTime;
    if ((double) this.stoppedTurnMultiplier != 0.0 && (double) this.m_aiActor.specRigidbody.Velocity.magnitude < (double) this.m_aiActor.MovementSpeed / 2.0)
      turnTime *= this.stoppedTurnMultiplier;
    float num1 = (this.m_targetCenter - this.m_aiActor.specRigidbody.UnitCenter).ToAngle();
    if ((double) this.targetTolerance > 0.0)
    {
      float f = Mathf.DeltaAngle(num1, this.m_direction);
      if ((double) Mathf.Abs(f) < (double) this.targetTolerance)
        num1 = this.m_direction;
      else
        num1 += Mathf.Sign(f) * this.targetTolerance;
    }
    this.m_direction = Mathf.SmoothDampAngle(this.m_direction, num1, ref this.m_angularVelocity, turnTime);
    if (this.slither)
      this.m_slitherDirection = Mathf.Sin(this.m_timer * 3.14159274f / this.slitherPeriod) * this.slitherMagnitude;
    this.m_aiActor.BehaviorOverridesVelocity = true;
    this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_direction + this.m_slitherDirection, this.m_aiActor.MovementSpeed);
    if (this.bob)
    {
      float num2 = Mathf.Sin(this.m_timer * 3.14159274f / this.m_bobPeriod) * this.bobMagnitude;
      if ((double) this.m_deltaTime > 0.0)
        this.m_aiActor.BehaviorVelocity += new Vector2(0.0f, num2 - this.m_lastBobOffset) / this.m_deltaTime;
      this.m_lastBobOffset = num2;
    }
    return BehaviorResult.Continue;
  }

  private void UpdateTargetCenter()
  {
    if (!(bool) (Object) this.m_aiActor.TargetRigidbody)
      return;
    this.m_targetCenter = this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
  }
}
