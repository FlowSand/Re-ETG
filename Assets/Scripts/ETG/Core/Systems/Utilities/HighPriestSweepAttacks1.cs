// Decompiled with JetBrains decompiler
// Type: HighPriestSweepAttacks1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;

#nullable disable

  public abstract class HighPriestSweepAttacks1 : Script
  {
    private const int NumBullets = 15;
    private bool m_shootLeft;
    private bool m_shootRight;

    public HighPriestSweepAttacks1(bool shootLeft, bool shootRight)
    {
      this.m_shootLeft = shootLeft;
      this.m_shootRight = shootRight;
    }

[DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new HighPriestSweepAttacks1__Topc__Iterator0()
      {
        _this = this
      };
    }

public class SweepBullet : Bullet
    {
      private int m_delay;

      public SweepBullet(int delay)
        : base("sweep")
      {
        this.m_delay = delay;
      }

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new HighPriestSweepAttacks1.SweepBullet__Topc__Iterator0()
        {
          _this = this
        };
      }
    }
  }

