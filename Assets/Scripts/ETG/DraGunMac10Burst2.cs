// Decompiled with JetBrains decompiler
// Type: DraGunMac10Burst2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable
[InspectorDropdownName("Bosses/DraGun/Mac10Burst2")]
public class DraGunMac10Burst2 : Script
{
  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new DraGunMac10Burst2.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public class UziBullet : Bullet
  {
    public UziBullet()
      : base(nameof (UziBullet))
    {
    }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new DraGunMac10Burst2.UziBullet.\u003CTop\u003Ec__Iterator0()
      {
        \u0024this = this
      };
    }
  }
}
