// Decompiled with JetBrains decompiler
// Type: AutoDistortionDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class AutoDistortionDoer : BraveBehaviour
    {
      public float Intensity = 0.25f;
      public float Width = 0.125f;
      public float Radius = 5f;
      public float Duration = 1f;
      public float DelayTime = 0.25f;
      private bool m_triggered;

      private void Start() => this.OnSpawned();

      private void OnSpawned()
      {
        if (this.m_triggered)
          return;
        this.StartCoroutine(this.Distort(!(bool) (Object) this.sprite ? this.transform.position.XY() : this.sprite.WorldCenter));
        this.m_triggered = true;
      }

      [DebuggerHidden]
      private IEnumerator Distort(Vector2 centerPoint)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new AutoDistortionDoer.\u003CDistort\u003Ec__Iterator0()
        {
          centerPoint = centerPoint,
          \u0024this = this
        };
      }

      private void OnDespawned() => this.m_triggered = false;
    }

}
