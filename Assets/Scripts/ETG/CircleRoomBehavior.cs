// Decompiled with JetBrains decompiler
// Type: CircleRoomBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class CircleRoomBehavior : MovementBehaviorBase
{
  public float PathInterval = 0.25f;
  public float Radius = 3f;
  public float Direction = 1f;
  private float m_repathTimer;
  private Vector2 m_center;

  public override void Start()
  {
    base.Start();
    this.m_center = this.m_aiActor.ParentRoom.area.UnitCenter;
  }

  public override void Upkeep()
  {
    base.Upkeep();
    this.DecrementTimer(ref this.m_repathTimer);
  }

  public override BehaviorResult Update()
  {
    if ((double) this.m_repathTimer <= 0.0)
    {
      Vector2 targetPosition = this.m_center + BraveMathCollege.DegreesToVector((this.m_aiActor.specRigidbody.UnitCenter - this.m_center).ToAngle() + (float) ((double) this.Direction * ((double) (this.PathInterval * 2f * this.m_aiActor.MovementSpeed) / (double) this.Radius) * 57.295780181884766), this.Radius);
      this.m_aiActor.PathfindToPosition(targetPosition, new Vector2?(targetPosition));
      this.m_repathTimer = this.PathInterval;
    }
    return BehaviorResult.SkipRemainingClassBehaviors;
  }
}
