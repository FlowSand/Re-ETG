using FullInspector;
using UnityEngine;

#nullable disable

    public abstract class RangedMovementBehavior : MovementBehaviorBase
    {
        public bool SpecifyRange;
[InspectorShowIf("SpecifyRange")]
        public float MinActiveRange;
[InspectorShowIf("SpecifyRange")]
        public float MaxActiveRange;

        protected bool InRange()
        {
            if (!this.SpecifyRange)
                return true;
            if (!(bool) (Object) this.m_aiActor.TargetRigidbody)
                return false;
            float distanceToTarget = this.m_aiActor.DistanceToTarget;
            return (double) distanceToTarget >= (double) this.MinActiveRange && (double) distanceToTarget <= (double) this.MaxActiveRange;
        }
    }

