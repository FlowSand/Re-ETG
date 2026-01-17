// Decompiled with JetBrains decompiler
// Type: BabyDragunBurst1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class BabyDragunBurst1 : Script
    {
      private const int NumBullets = 7;
      private const float HalfArc = 15f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BabyDragunBurst1.<Top>c__Iterator0()
        {
          $this = this
        };
      }
    }

}
