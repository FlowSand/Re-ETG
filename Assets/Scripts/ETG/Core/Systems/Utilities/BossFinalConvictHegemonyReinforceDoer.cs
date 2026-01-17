// Decompiled with JetBrains decompiler
// Type: BossFinalConvictHegemonyReinforceDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class BossFinalConvictHegemonyReinforceDoer : CustomReinforceDoer
    {
      public GameObject ropeVfx;
      private bool m_isFinished;

      public override void StartIntro() => this.StartCoroutine(this.DoIntro());

      [DebuggerHidden]
      private IEnumerator DoIntro()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossFinalConvictHegemonyReinforceDoer.\u003CDoIntro\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      public override bool IsFinished => this.m_isFinished;
    }

}
