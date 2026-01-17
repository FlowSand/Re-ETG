// Decompiled with JetBrains decompiler
// Type: BossDoorMimicWaves1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable
[InspectorDropdownName("Bosses/BossDoorMimic/Waves1")]
public class BossDoorMimicWaves1 : Script
{
  private const int NumWaves = 7;
  private const int NumBulletsPerWave = 5;
  private const float Arc = 60f;

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BossDoorMimicWaves1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
