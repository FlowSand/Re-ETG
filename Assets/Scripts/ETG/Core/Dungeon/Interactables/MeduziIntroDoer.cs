// Decompiled with JetBrains decompiler
// Type: MeduziIntroDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Dungeon.Interactables
{
    [RequireComponent(typeof (GenericIntroDoer))]
    public class MeduziIntroDoer : SpecificIntroDoer
    {
      private bool m_isFinished;

      protected override void OnDestroy() => base.OnDestroy();

      public override void StartIntro(List<tk2dSpriteAnimator> animators)
      {
        this.StartCoroutine(this.DoIntro());
      }

      [DebuggerHidden]
      public IEnumerator DoIntro()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MeduziIntroDoer.\u003CDoIntro\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      public override bool IsIntroFinished => true;

      public override void EndIntro() => this.m_isFinished = true;
    }

}
