// Decompiled with JetBrains decompiler
// Type: SimpleHealthBarController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class SimpleHealthBarController : MonoBehaviour
    {
      public Transform fg;
      public Transform bg;
      private HealthHaver m_healthHaver;
      private float m_baseScale = 1f;

      public void Initialize(SpeculativeRigidbody srb, HealthHaver h)
      {
        this.m_healthHaver = h;
        this.m_baseScale = 1f;
        if ((double) srb.UnitWidth > 1.0)
          this.m_baseScale = srb.UnitWidth;
        this.transform.parent = this.m_healthHaver.transform;
        this.transform.position = srb.UnitBottomCenter.Quantize(1f / 16f).ToVector3ZisY() + new Vector3(0.0f, -0.25f, 0.0f);
        this.fg.localScale = this.fg.localScale.WithX(this.m_baseScale);
        this.bg.localScale = this.bg.localScale.WithX(this.m_baseScale);
        this.fg.localPosition = new Vector3((float) (-1.0 * ((double) this.m_baseScale * 0.5)), 0.0f, 0.0f);
      }

      private void Update()
      {
        if (!(bool) (Object) this.m_healthHaver)
          return;
        this.fg.localScale = this.fg.localScale.WithX(this.m_healthHaver.GetCurrentHealthPercentage() * this.m_baseScale);
      }
    }

}
