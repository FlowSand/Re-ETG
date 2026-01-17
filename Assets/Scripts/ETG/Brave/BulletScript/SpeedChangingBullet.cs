// Decompiled with JetBrains decompiler
// Type: Brave.BulletScript.SpeedChangingBullet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;

#nullable disable
namespace Brave.BulletScript;

public class SpeedChangingBullet : Bullet
{
  private float m_newSpeed;
  private int m_term;
  private int m_destroyTimer;

  public SpeedChangingBullet(float newSpeed, int term, int destroyTimer = -1)
    : base()
  {
    this.m_newSpeed = newSpeed;
    this.m_term = term;
    this.m_destroyTimer = destroyTimer;
  }

  public SpeedChangingBullet(
    string name,
    float newSpeed,
    int term,
    int destroyTimer = -1,
    bool suppressVfx = false)
    : base(name, suppressVfx)
  {
    this.m_newSpeed = newSpeed;
    this.m_term = term;
    this.m_destroyTimer = destroyTimer;
  }

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new SpeedChangingBullet.<Top>c__Iterator0()
    {
      $this = this
    };
  }
}
