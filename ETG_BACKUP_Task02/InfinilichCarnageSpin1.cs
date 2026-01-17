// Decompiled with JetBrains decompiler
// Type: InfinilichCarnageSpin1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable
[InspectorDropdownName("Bosses/Infinilich/CarnageSpin1")]
public class InfinilichCarnageSpin1 : Script
{
  private const int SpinTime = 420;
  private static float SpinDirection;
  public bool Spin;

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new InfinilichCarnageSpin1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public class TipBullet : Bullet
  {
    private InfinilichCarnageSpin1 m_parentScript;

    public TipBullet(InfinilichCarnageSpin1 parentScript)
      : base("carnageTip")
    {
      this.m_parentScript = parentScript;
    }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new InfinilichCarnageSpin1.TipBullet.\u003CTop\u003Ec__Iterator0()
      {
        \u0024this = this
      };
    }
  }

  public class ChainBullet : Bullet
  {
    private const float WiggleMagnitude = 0.75f;
    private const float WigglePeriodMultiplier = 0.333f;
    private InfinilichCarnageSpin1 m_parentScript;
    private int m_spawnDelay;
    private float m_tipSpeed;
    private float m_spinSpeed;

    public ChainBullet(
      InfinilichCarnageSpin1 parentScript,
      int spawnDelay,
      float tipSpeed,
      float spinSpeed)
      : base()
    {
      this.m_parentScript = parentScript;
      this.m_spawnDelay = spawnDelay;
      this.m_tipSpeed = tipSpeed;
      this.m_spinSpeed = spinSpeed;
    }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new InfinilichCarnageSpin1.ChainBullet.\u003CTop\u003Ec__Iterator0()
      {
        \u0024this = this
      };
    }
  }
}
