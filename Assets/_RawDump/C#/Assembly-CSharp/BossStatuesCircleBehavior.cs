// Decompiled with JetBrains decompiler
// Type: BossStatuesCircleBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using FullInspector;
using UnityEngine;

#nullable disable
[InspectorDropdownName("Bosses/BossStatues/CircleBehavior")]
public class BossStatuesCircleBehavior : BossStatuesPatternBehavior
{
  public float Duration;
  public float CircleRadius;
  public bool UseFixedCircleCenter;
  public float CircleCenterVelocity;
  private float[] m_statueAngles;
  private float m_cachedStatueAngle;
  private float m_rotationSpeed;
  private float m_circularSpeed;
  private Vector2 m_roomLowerLeft;
  private Vector2 m_roomUpperRight;
  private Vector2 m_circleCenter;
  protected float m_durationTimer;

  public override void Start()
  {
    base.Start();
    this.m_cachedStatueAngle = (float) (0.5 * (360.0 / (double) this.m_statuesController.allStatues.Count));
  }

  public override void Upkeep()
  {
    this.DecrementTimer(ref this.m_durationTimer);
    base.Upkeep();
  }

  public override void EndContinuousUpdate()
  {
    base.EndContinuousUpdate();
    this.m_cachedStatueAngle = BraveMathCollege.ClampAngle360(this.m_statueAngles[0]);
  }

  protected override void InitPositions()
  {
    RoomHandler parentRoom = this.m_activeStatues[0].aiActor.ParentRoom;
    this.m_roomLowerLeft = parentRoom.area.basePosition.ToVector2() + new Vector2(1f, 1f);
    this.m_roomUpperRight = (parentRoom.area.basePosition + parentRoom.area.dimensions).ToVector2() + new Vector2(-1f, -5f);
    float num = 6.28318548f * this.CircleRadius;
    this.m_circularSpeed = this.m_statuesController.GetEffectiveMoveSpeed((double) this.OverrideMoveSpeed <= 0.0 ? this.m_statuesController.moveSpeed : this.OverrideMoveSpeed);
    this.m_rotationSpeed = (float) (360.0 / ((double) num / (double) this.m_circularSpeed));
    this.m_circleCenter = Vector2.zero;
    this.m_statueAngles = new float[this.m_activeStatueCount];
    for (int index = 0; index < this.m_activeStatueCount; ++index)
    {
      this.m_statueAngles[index] = this.m_cachedStatueAngle + (float) index * (360f / (float) this.m_activeStatueCount);
      this.m_circleCenter += this.m_activeStatues[index].GroundPosition;
    }
    this.m_circleCenter /= (float) this.m_activeStatueCount;
    this.m_circleCenter = BraveMathCollege.ClampToBounds(this.m_circleCenter, this.m_roomLowerLeft + new Vector2(this.CircleRadius, this.CircleRadius), this.m_roomUpperRight - new Vector2(this.CircleRadius, this.CircleRadius));
    if (this.UseFixedCircleCenter)
      this.m_circleCenter = this.m_statuesController.PatternCenter;
    Vector2[] positions = new Vector2[this.m_activeStatueCount];
    for (int index = 0; index < this.m_activeStatueCount; ++index)
      positions[index] = this.GetTargetPoint(this.m_statueAngles[index]);
    this.ReorderStatues(positions);
    for (int index = 0; index < positions.Length; ++index)
      this.m_activeStatues[index].Target = new Vector2?(this.GetTargetPoint(this.m_statueAngles[index]));
  }

  protected override void UpdatePositions()
  {
    PlayerController playerClosestToPoint = GameManager.Instance.GetPlayerClosestToPoint(this.m_circleCenter);
    if ((bool) (Object) playerClosestToPoint)
    {
      this.m_circleCenter = Vector2.MoveTowards(this.m_circleCenter, playerClosestToPoint.specRigidbody.UnitCenter, this.CircleCenterVelocity * this.m_deltaTime);
      this.m_circleCenter = BraveMathCollege.ClampToBounds(this.m_circleCenter, this.m_roomLowerLeft + new Vector2(this.CircleRadius, this.CircleRadius), this.m_roomUpperRight - new Vector2(this.CircleRadius, this.CircleRadius));
    }
    for (int index = 0; index < this.m_activeStatueCount; ++index)
    {
      this.m_statueAngles[index] += this.m_deltaTime * this.m_rotationSpeed;
      this.m_activeStatues[index].Target = new Vector2?(this.GetTargetPoint(this.m_statueAngles[index]));
    }
    this.m_statuesController.OverrideMoveSpeed = new float?(this.m_circularSpeed + this.CircleCenterVelocity * 2f);
  }

  protected override bool IsFinished() => (double) this.m_durationTimer <= 0.0;

  protected override void BeginState(BossStatuesPatternBehavior.PatternState state)
  {
    if (state == BossStatuesPatternBehavior.PatternState.InProgress)
      this.m_durationTimer = this.Duration;
    base.BeginState(state);
  }

  private Vector2 GetTargetPoint(float angle)
  {
    return this.m_circleCenter + BraveMathCollege.DegreesToVector(angle, this.CircleRadius);
  }
}
