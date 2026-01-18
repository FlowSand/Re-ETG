using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class ShrinkEnemiesInRoomItem : AffectEnemiesInRoomItem
  {
    public Vector2 TargetScale;
    public float ShrinkTime = 0.1f;
    public float HoldTime = 3f;
    public float RegrowTime = 3f;
    public float DamageMultiplier = 2f;
    public bool DepixelatesTargets;

    protected override void AffectEnemy(AIActor target)
    {
      target.StartCoroutine(this.HandleShrink(target));
    }

    [DebuggerHidden]
    private IEnumerator HandleShrink(AIActor target)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new ShrinkEnemiesInRoomItem__HandleShrinkc__Iterator0()
      {
        target = target,
        _this = this
      };
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

