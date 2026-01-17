// Decompiled with JetBrains decompiler
// Type: BossFinalMarineSpinFire1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable
[InspectorDropdownName("Bosses/BossFinalMarine/SpinFire1")]
public class BossFinalMarineSpinFire1 : Script
{
  private const int NumWaves = 25;
  private const int NumBulletsPerWave = 6;
  private const float AngleDeltaEachWave = 37f;
  private const int NumBulletsFinalWave = 64 /*0x40*/;

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BossFinalMarineSpinFire1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
