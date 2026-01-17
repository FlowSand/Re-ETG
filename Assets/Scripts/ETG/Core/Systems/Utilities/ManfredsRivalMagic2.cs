// Decompiled with JetBrains decompiler
// Type: ManfredsRivalMagic2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("ManfredsRival/Magic2")]
    public class ManfredsRivalMagic2 : ManfredsRivalMagic1
    {
      private const int NumTimes = 3;
      private const int NumBulletsMainWave = 16 /*0x10*/;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ManfredsRivalMagic2.<Top>c__Iterator0()
        {
          $this = this
        };
      }
    }

}
