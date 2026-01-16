// Decompiled with JetBrains decompiler
// Type: BulletKingQuadShot1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable
[InspectorDropdownName("Bosses/BulletKing/QuadShot1")]
public class BulletKingQuadShot1 : Script
{
  public bool IsHard => this is BulletKingQuadShotHard1;

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BulletKingQuadShot1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  private void QuadShot(float x, float y, float direction)
  {
    for (int index = 0; index < 4; ++index)
      this.Fire(new Offset(x, y, transform: string.Empty), new Brave.BulletScript.Direction(direction - 90f), new Brave.BulletScript.Speed((float) (9.0 - (double) index * 1.5)), (Bullet) new BulletKingQuadShot1.QuadBullet(this.IsHard, index));
  }

  public class QuadBullet : Bullet
  {
    private bool m_isHard;
    private int m_index;

    public QuadBullet(bool isHard, int index)
      : base("quad")
    {
      this.m_isHard = isHard;
      this.m_index = index;
    }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BulletKingQuadShot1.QuadBullet.\u003CTop\u003Ec__Iterator0()
      {
        \u0024this = this
      };
    }
  }
}
