// Decompiled with JetBrains decompiler
// Type: UnearthController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class UnearthController : BraveBehaviour
    {
      public string triggerAnim;
      public List<GameObject> dirtVfx;
      public int dirtCount;
      public List<GameObject> dustVfx;
      public float dustMidDelay = 0.05f;
      public Vector2 dustOffset;
      public Vector2 dustDimensions;
      private UnearthController.UnearthState m_state;

      private void Update()
      {
        if (this.m_state == UnearthController.UnearthState.Idle)
        {
          if (!this.aiAnimator.IsPlaying(this.triggerAnim))
            return;
          this.m_state = UnearthController.UnearthState.Unearth;
          this.StartCoroutine(this.DirtCR());
          this.StartCoroutine(this.PuffCR());
        }
        else
        {
          if (this.m_state != UnearthController.UnearthState.Unearth || this.aiAnimator.IsPlaying(this.triggerAnim))
            return;
          this.m_state = UnearthController.UnearthState.Finished;
        }
      }

      protected override void OnDestroy() => base.OnDestroy();

      [DebuggerHidden]
      private IEnumerator DirtCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new UnearthController.\u003CDirtCR\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      [DebuggerHidden]
      private IEnumerator PuffCR()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new UnearthController.\u003CPuffCR\u003Ec__Iterator1()
        {
          \u0024this = this
        };
      }

      private enum UnearthState
      {
        Idle,
        Unearth,
        Finished,
      }
    }

}
