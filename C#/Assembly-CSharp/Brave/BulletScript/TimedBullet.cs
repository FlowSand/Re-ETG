// Decompiled with JetBrains decompiler
// Type: Brave.BulletScript.TimedBullet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;

#nullable disable
namespace Brave.BulletScript;

public class TimedBullet : Bullet
{
  private int m_destroyTimer;

  public TimedBullet(int destroyTimer)
    : base()
  {
    this.m_destroyTimer = destroyTimer;
  }

  public TimedBullet(string name, int destroyTimer)
    : base(name)
  {
    this.m_destroyTimer = destroyTimer;
  }

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new TimedBullet.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }
}
