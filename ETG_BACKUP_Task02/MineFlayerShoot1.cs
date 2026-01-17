// Decompiled with JetBrains decompiler
// Type: MineFlayerShoot1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable
[InspectorDropdownName("Bosses/MineFlayer/Shoot1")]
public class MineFlayerShoot1 : Script
{
  private const int NumBursts = 5;
  private const int NumBullets = 36;

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new MineFlayerShoot1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public class BurstBullet : Bullet
  {
    private Vector2 m_addtionalVelocity;

    public BurstBullet(Vector2 additionalVelocity)
      : base("burst")
    {
      this.m_addtionalVelocity = additionalVelocity;
    }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new MineFlayerShoot1.BurstBullet.\u003CTop\u003Ec__Iterator0()
      {
        \u0024this = this
      };
    }
  }
}
