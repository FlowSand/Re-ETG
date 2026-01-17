// Decompiled with JetBrains decompiler
// Type: HighPriestAmuletPulses1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable
[InspectorDropdownName("Bosses/HighPriest/AmuletPulses1")]
public class HighPriestAmuletPulses1 : Script
{
  private const int NumBullets = 25;

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new HighPriestAmuletPulses1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public class VibratingBullet : Bullet
  {
    public VibratingBullet()
      : base("amuletRing")
    {
    }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new HighPriestAmuletPulses1.VibratingBullet.\u003CTop\u003Ec__Iterator0()
      {
        \u0024this = this
      };
    }
  }
}
