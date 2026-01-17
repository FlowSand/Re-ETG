// Decompiled with JetBrains decompiler
// Type: DraGunFlameBreath2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable
[InspectorDropdownName("Bosses/DraGun/FlameBreath2")]
public class DraGunFlameBreath2 : Script
{
  private const int NumBullets = 120;
  private const int NumWaveBullets = 12;
  private const float Spread = 30f;
  private const int PocketResetTime = 30;
  private const float PocketWidth = 5f;
  protected static float StopYHeight;

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new DraGunFlameBreath2.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public class FlameBullet : Bullet
  {
    public FlameBullet()
      : base("Breath")
    {
    }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DraGunFlameBreath2.FlameBullet.\u003CTop\u003Ec__Iterator0()
      {
        \u0024this = this
      };
    }
  }
}
