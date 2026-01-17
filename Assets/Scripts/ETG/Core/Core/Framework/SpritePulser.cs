// Decompiled with JetBrains decompiler
// Type: SpritePulser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class SpritePulser : BraveBehaviour
    {
      public float duration = 1f;
      public float minDuration = 0.3f;
      public float maxDuration = 2.9f;
      public float metaDuration = 6f;
      public float minAlpha = 0.3f;
      public float minScale = 0.9f;
      public float maxScale = 1.1f;
      private bool m_active;

      private void Start()
      {
        if (!((Object) this.sprite == (Object) null))
          return;
        UnityEngine.Debug.LogError((object) "No sprite on SpritePulser!", (Object) this);
      }

      private void Update()
      {
        if (!this.m_active)
          return;
        float t = Mathf.SmoothStep(0.0f, 1f, Mathf.PingPong(UnityEngine.Time.realtimeSinceStartup, this.duration) / this.duration);
        this.sprite.color = this.sprite.color with
        {
          a = Mathf.Lerp(this.minAlpha, 1f, t)
        };
      }

      private void OnBecameVisible() => this.m_active = true;

      private void OnBecameInvisible() => this.m_active = false;

      [DebuggerHidden]
      private IEnumerator Pulse()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new SpritePulser.<Pulse>c__Iterator0()
        {
          $this = this
        };
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
