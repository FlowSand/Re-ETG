// Decompiled with JetBrains decompiler
// Type: ShrinkEnemiesInRoomItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
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
        return (IEnumerator) new ShrinkEnemiesInRoomItem.<HandleShrink>c__Iterator0()
        {
          target = target,
          $this = this
        };
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
