// Decompiled with JetBrains decompiler
// Type: LichSpinFire1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable

[InspectorDropdownName("Bosses/Lich/SpinFire1")]
public class LichSpinFire1 : Script
  {
    private const int NumWaves = 60;
    private const int NumBulletsPerWave = 6;
    private const float AngleDeltaEachWave = 9f;

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new LichSpinFire1__Topc__Iterator0()
      {
        _this = this
      };
    }
  }

