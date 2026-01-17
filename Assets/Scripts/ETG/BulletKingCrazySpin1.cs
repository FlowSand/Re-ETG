// Decompiled with JetBrains decompiler
// Type: BulletKingCrazySpin1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable
[InspectorDropdownName("Bosses/BulletKing/CrazySpin1")]
public class BulletKingCrazySpin1 : Script
{
  private const int NumWaves = 29;
  private const int NumBulletsPerWave = 6;
  private const float AngleDeltaEachWave = 37f;
  private const int NumBulletsFinalWave = 64 /*0x40*/;

  protected bool IsHard => this is BulletKingCrazySpinHard1;

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BulletKingCrazySpin1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
