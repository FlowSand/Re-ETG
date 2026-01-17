// Decompiled with JetBrains decompiler
// Type: RangedMovementBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

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
