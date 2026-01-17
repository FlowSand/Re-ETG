// Decompiled with JetBrains decompiler
// Type: NebulaRegisterer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class NebulaRegisterer : MonoBehaviour
    {
      private Renderer m_renderer;

      [DebuggerHidden]
      private IEnumerator Start()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new NebulaRegisterer.\u003CStart\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      private void Update()
      {
        if (!(bool) (Object) this.m_renderer)
          return;
        if (GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.VERY_LOW || GameManager.Options.ShaderQuality == GameOptions.GenericHighMedLowOption.LOW)
        {
          this.m_renderer.enabled = false;
        }
        else
        {
          if (this.m_renderer.enabled)
            return;
          this.m_renderer.enabled = true;
        }
      }
    }

}
