// Decompiled with JetBrains decompiler
// Type: BossDoorMimicBurst1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable
[InspectorDropdownName("Bosses/BossDoorMimic/Burst1")]
public class BossDoorMimicBurst1 : Script
{
  private const int NumBullets = 36;

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BossDoorMimicBurst1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public class SpinBullet : Bullet
  {
    public SpinBullet()
      : base("teleport_burst")
    {
    }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BossDoorMimicBurst1.SpinBullet.\u003CTop\u003Ec__Iterator0()
      {
        \u0024this = this
      };
    }
  }
}
