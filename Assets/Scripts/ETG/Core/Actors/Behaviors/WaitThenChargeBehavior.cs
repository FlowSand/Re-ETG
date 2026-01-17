// Decompiled with JetBrains decompiler
// Type: WaitThenChargeBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    public class WaitThenChargeBehavior : MovementBehaviorBase
    {
      public float Delay;
      private float m_delayTimer;
      private bool m_isCharging;
      private float m_chargeDirection;
      private Vector2 m_center;

      public override void Start()
      {
        base.Start();
        this.m_delayTimer = this.Delay;
      }

      public override void Upkeep()
      {
        base.Upkeep();
        this.DecrementTimer(ref this.m_delayTimer);
      }

      public override BehaviorResult Update()
      {
        if (this.m_isCharging)
        {
          this.m_aiActor.BehaviorOverridesVelocity = true;
          this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_chargeDirection, this.m_aiActor.MovementSpeed);
        }
        else if ((double) this.m_delayTimer <= 0.0)
        {
          this.m_isCharging = true;
          this.m_chargeDirection = !(bool) (Object) this.m_aiActor.TargetRigidbody ? Random.Range(0.0f, 360f) : (this.m_aiActor.behaviorSpeculator.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox)).ToAngle();
          this.m_aiActor.BehaviorOverridesVelocity = true;
          this.m_aiActor.BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_chargeDirection, this.m_aiActor.MovementSpeed);
        }
        return BehaviorResult.SkipRemainingClassBehaviors;
      }
    }

}
