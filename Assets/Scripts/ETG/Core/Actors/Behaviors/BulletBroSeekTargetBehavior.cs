// Decompiled with JetBrains decompiler
// Type: BulletBroSeekTargetBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    [InspectorDropdownName("Bosses/BulletBro/SeekTargetBehavior")]
    public class BulletBroSeekTargetBehavior : MovementBehaviorBase
    {
      public bool StopWhenInRange = true;
      public float CustomRange = -1f;
      public float PathInterval = 0.25f;
      private float m_repathTimer;
      private AIActor m_otherBro;

      public override void Start()
      {
        base.Start();
        BroController otherBro = BroController.GetOtherBro(this.m_aiActor.gameObject);
        if (!(bool) (Object) otherBro)
          return;
        this.m_otherBro = otherBro.aiActor;
      }

      public override void Upkeep()
      {
        base.Upkeep();
        this.DecrementTimer(ref this.m_repathTimer);
      }

      public override BehaviorResult Update()
      {
        SpeculativeRigidbody targetRigidbody = this.m_aiActor.TargetRigidbody;
        if (!((Object) targetRigidbody != (Object) null))
          return BehaviorResult.Continue;
        float desiredCombatDistance = this.m_aiActor.DesiredCombatDistance;
        if (this.StopWhenInRange && (double) this.m_aiActor.DistanceToTarget <= (double) desiredCombatDistance)
        {
          this.m_aiActor.ClearPath();
          return BehaviorResult.Continue;
        }
        if ((double) this.m_repathTimer <= 0.0)
        {
          Vector2 targetPosition;
          if (!(bool) (Object) this.m_otherBro)
          {
            targetPosition = targetRigidbody.UnitCenter;
          }
          else
          {
            Vector2 unitCenter1 = this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
            Vector2 unitCenter2 = this.m_aiActor.specRigidbody.UnitCenter;
            Vector2 unitCenter3 = this.m_otherBro.specRigidbody.UnitCenter;
            float angle1 = (unitCenter2 - unitCenter1).ToAngle();
            float angle2 = (unitCenter3 - unitCenter1).ToAngle();
            float num = (float) (((double) angle1 + (double) angle2) / 2.0);
            float angle3 = (double) BraveMathCollege.ClampAngle180(angle1 - num) <= 0.0 ? num - 90f : num + 90f;
            targetPosition = unitCenter1 + BraveMathCollege.DegreesToVector(angle3) * this.DesiredCombatDistance;
          }
          this.m_aiActor.PathfindToPosition(targetPosition);
          this.m_repathTimer = this.PathInterval;
        }
        return BehaviorResult.SkipRemainingClassBehaviors;
      }

      public override float DesiredCombatDistance => this.CustomRange;
    }

}
