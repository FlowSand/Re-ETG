// Decompiled with JetBrains decompiler
// Type: TankTreaderSeekTargetBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[InspectorDropdownName("Bosses/TankTreader/SeekTargetBehavior")]
public class TankTreaderSeekTargetBehavior : RangedMovementBehavior
{
  public bool StopWhenInRange = true;
  public float CustomRange = -1f;
  [InspectorShowIf("StopWhenInRange")]
  public bool LineOfSight = true;
  public float PathInterval = 0.25f;
  public float turnSpeed = 120f;
  private float m_repathTimer;
  private IntVector2 m_startStep;
  private float m_desiredFacingDirection = -90f;
  private TankTreaderSeekTargetBehavior.State m_state;

  public override void Start()
  {
    base.Start();
    this.m_updateEveryFrame = true;
  }

  public override void Upkeep()
  {
    base.Upkeep();
    this.DecrementTimer(ref this.m_repathTimer);
    this.m_aiActor.BehaviorOverridesVelocity = true;
    this.m_aiActor.BehaviorVelocity = Vector2.zero;
    this.m_aiAnimator.LockFacingDirection = true;
  }

  public override BehaviorResult Update()
  {
    SpeculativeRigidbody targetRigidbody = this.m_aiActor.TargetRigidbody;
    this.m_aiAnimator.FacingDirection = Mathf.MoveTowardsAngle(this.m_aiAnimator.FacingDirection, this.m_desiredFacingDirection, this.turnSpeed * this.m_deltaTime);
    if (this.InRange() && (bool) (Object) targetRigidbody)
    {
      bool lineOfSightToTarget = this.m_aiActor.HasLineOfSightToTarget;
      float num = (double) this.CustomRange < 0.0 ? this.m_aiActor.DesiredCombatDistance : this.CustomRange;
      this.m_state = TankTreaderSeekTargetBehavior.State.PathingToTarget;
      if (this.StopWhenInRange && (double) this.m_aiActor.DistanceToTarget <= (double) num && (!this.LineOfSight || lineOfSightToTarget))
      {
        this.m_aiActor.ClearPath();
        this.m_aiActor.BehaviorVelocity = Vector2.zero;
        return BehaviorResult.Continue;
      }
      if ((double) this.m_repathTimer <= 0.0)
      {
        this.m_startStep = (double) this.m_aiActor.specRigidbody.Velocity.magnitude <= 0.0099999997764825821 ? IntVector2.Zero : BraveUtility.GetIntMajorAxis(this.m_aiActor.specRigidbody.Velocity);
        this.m_aiActor.PathfindToPosition(targetRigidbody.UnitCenter, smooth: false, extraWeightingFunction: new ExtraWeightingFunction(this.WeightDoer));
        this.m_repathTimer = this.PathInterval;
        this.SimplifyPath();
      }
      this.UpdateVelocity();
      return BehaviorResult.SkipRemainingClassBehaviors;
    }
    if (this.m_state == TankTreaderSeekTargetBehavior.State.PathingToTarget)
    {
      this.m_aiActor.ClearPath();
      this.m_state = TankTreaderSeekTargetBehavior.State.Idle;
    }
    return BehaviorResult.Continue;
  }

  private void SimplifyPath()
  {
    Path path = this.m_aiActor.Path;
    if (path == null || path.Count < 2)
      return;
    Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
    Vector2 firstCenterVector2 = this.m_aiActor.Path.GetFirstCenterVector2();
    Vector2 secondCenterVector2 = this.m_aiActor.Path.GetSecondCenterVector2();
    float num = BraveMathCollege.ClampAngle360((firstCenterVector2 - unitCenter).ToAngle() - (secondCenterVector2 - unitCenter).ToAngle());
    if ((double) num > 179.0 && (double) num < 181.0)
      path.Positions.RemoveFirst();
    if (path.Count < 2)
      return;
    LinkedListNode<IntVector2> next = path.Positions.First.Next;
    IntVector2 intVector2_1 = next.Value - next.Previous.Value;
    while (next != null && next.Next != null)
    {
      IntVector2 intVector2_2 = next.Next.Value - next.Value;
      if (intVector2_1 == intVector2_2)
      {
        next = next.Next;
        path.Positions.Remove(next.Previous);
      }
      else
      {
        intVector2_1 = intVector2_2;
        next = next.Next;
      }
    }
  }

  private int WeightDoer(IntVector2 prevStep, IntVector2 nextStep)
  {
    if (prevStep == IntVector2.Zero)
    {
      if (this.m_startStep == IntVector2.Zero)
        return 0;
      prevStep = this.m_startStep;
    }
    return prevStep != nextStep ? 10 : 0;
  }

  private void UpdateVelocity()
  {
    bool willReachGoal;
    Vector2 totalDistToMove;
    Vector2 vector = this.GetPathVelocityContribution(out willReachGoal, out totalDistToMove);
    Vector2 vector2 = vector;
    if ((double) Mathf.Abs(totalDistToMove.x) < (double) PhysicsEngine.PixelToUnit(2))
      vector2.x = 0.0f;
    if ((double) Mathf.Abs(totalDistToMove.y) < (double) PhysicsEngine.PixelToUnit(2))
      vector2.y = 0.0f;
    if ((double) vector2.magnitude > 0.0099999997764825821)
    {
      float angle = vector.ToAngle();
      float f = BraveMathCollege.ClampAngle180(this.m_aiAnimator.FacingDirection - angle);
      if ((double) Mathf.Abs(f) > 0.5 && (double) Mathf.Abs(f) < 179.5)
      {
        vector = Vector2.zero;
        this.m_desiredFacingDirection = (double) BraveMathCollege.AbsAngleBetween(this.m_aiAnimator.FacingDirection, angle) > 100.0 ? BraveMathCollege.ClampAngle360(angle + 180f) : angle;
      }
    }
    this.m_aiActor.BehaviorVelocity = vector;
    if (!willReachGoal)
      return;
    this.m_aiActor.Path.RemoveFirst();
  }

  private Vector2 GetPathVelocityContribution(out bool willReachGoal, out Vector2 totalDistToMove)
  {
    willReachGoal = false;
    totalDistToMove = Vector2.zero;
    if (this.m_aiActor.Path == null || this.m_aiActor.Path.Count == 0)
      return Vector2.zero;
    Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
    Vector2 firstCenterVector2 = this.m_aiActor.Path.GetFirstCenterVector2();
    totalDistToMove = firstCenterVector2 - unitCenter;
    if ((double) (this.m_aiActor.MovementSpeed * this.m_aiActor.LocalDeltaTime) <= (double) totalDistToMove.magnitude)
      return this.m_aiActor.MovementSpeed * totalDistToMove.normalized;
    willReachGoal = true;
    return totalDistToMove / this.m_aiActor.LocalDeltaTime;
  }

  private enum State
  {
    Idle,
    PathingToTarget,
  }
}
