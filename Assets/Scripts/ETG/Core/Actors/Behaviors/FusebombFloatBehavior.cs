// Decompiled with JetBrains decompiler
// Type: FusebombFloatBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Minibosses/Fusebomb/SeekTargetBehavior")]
public class FusebombFloatBehavior : MovementBehaviorBase
  {
    public Vector2 minPoint;
    public Vector2 maxPoint;
    public Vector2 period;
    public float MaxSpeed = 6f;
    private float m_timer;
    private bool m_isMoving;

    public override void Start()
    {
      base.Start();
      this.m_updateEveryFrame = true;
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.m_aiActor.OverridePathVelocity = new Vector2?();
    }

    public override BehaviorResult Update()
    {
      this.m_timer += this.m_deltaTime;
      Vector2 vector2_1 = this.m_aiActor.ParentRoom.area.UnitBottomLeft + new Vector2(Mathf.SmoothStep(this.minPoint.x, this.maxPoint.x, Mathf.PingPong(this.m_timer, this.period.x) / this.period.x), Mathf.SmoothStep(this.minPoint.y, this.maxPoint.y, Mathf.PingPong(this.m_timer, this.period.y) / this.period.y)) - this.m_aiActor.specRigidbody.UnitCenter;
      Vector2 vector2_2;
      if ((double) this.m_deltaTime > 0.0 && (double) vector2_1.magnitude > 0.0)
      {
        vector2_2 = vector2_1 / BraveTime.DeltaTime;
        if ((double) this.MaxSpeed >= 0.0 && (double) vector2_2.magnitude > (double) this.MaxSpeed)
          vector2_2 = this.MaxSpeed * vector2_2.normalized;
      }
      else
        vector2_2 = Vector2.zero;
      this.m_isMoving = true;
      this.m_aiActor.OverridePathVelocity = new Vector2?(vector2_2);
      return BehaviorResult.Continue;
    }
  }

