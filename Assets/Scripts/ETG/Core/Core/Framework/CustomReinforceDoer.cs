// Decompiled with JetBrains decompiler
// Type: CustomReinforceDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public abstract class CustomReinforceDoer : BraveBehaviour
    {
      public virtual void StartIntro()
      {
      }

      public virtual bool IsFinished => true;

      public virtual void OnCleanup()
      {
      }

      [DebuggerHidden]
      public IEnumerator TimeInvariantWait(float duration)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CustomReinforceDoer__TimeInvariantWaitc__Iterator0()
        {
          duration = duration
        };
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
