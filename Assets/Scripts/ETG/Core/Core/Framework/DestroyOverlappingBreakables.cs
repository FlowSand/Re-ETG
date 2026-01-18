using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class DestroyOverlappingBreakables : BraveBehaviour
  {
    public bool everyFrame;

    public void Update()
    {
      List<SpeculativeRigidbody> overlappingRigidbodies = PhysicsEngine.Instance.GetOverlappingRigidbodies(this.specRigidbody);
      for (int index = 0; index < overlappingRigidbodies.Count; ++index)
      {
        SpeculativeRigidbody speculativeRigidbody = overlappingRigidbodies[index];
        if ((bool) (Object) speculativeRigidbody && (bool) (Object) speculativeRigidbody.minorBreakable)
          speculativeRigidbody.minorBreakable.Break(speculativeRigidbody.UnitCenter - this.specRigidbody.UnitCenter);
      }
      if (this.everyFrame)
        return;
      this.enabled = false;
    }
  }

