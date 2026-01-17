// Decompiled with JetBrains decompiler
// Type: SpriteAnimatorStopper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.VFX.Animation
{
    public class SpriteAnimatorStopper : MonoBehaviour
    {
      public float duration = 10f;
      private tk2dSpriteAnimator animator;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SpriteAnimatorStopper__Startc__Iterator0()
        {
          _this = this
        };
      }
    }

}
