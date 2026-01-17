// Decompiled with JetBrains decompiler
// Type: BulletKingHomingRing1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable
[InspectorDropdownName("Bosses/BulletKing/HomingRing1")]
public class BulletKingHomingRing1 : Script
{
  private const int NumBullets = 24;

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BulletKingHomingRing1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public class SmokeBullet : Bullet
  {
    private const float ExpandSpeed = 2f;
    private const float SpinSpeed = 120f;
    private const float Lifetime = 600f;
    private float m_angle;

    public SmokeBullet(float angle)
      : base("homingRing", forceBlackBullet: true)
    {
      this.m_angle = angle;
    }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BulletKingHomingRing1.SmokeBullet.\u003CTop\u003Ec__Iterator0()
      {
        \u0024this = this
      };
    }
  }
}
