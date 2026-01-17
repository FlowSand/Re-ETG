// Decompiled with JetBrains decompiler
// Type: LichSpinFire2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Bosses/Lich/SpinFire2")]
    public class LichSpinFire2 : Script
    {
      private const int NumWaves = 6;
      private const int NumBulletsPerWave = 48 /*0x30*/;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new LichSpinFire2.\u003CTop\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }
    }

}
