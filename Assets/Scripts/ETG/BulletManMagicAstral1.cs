// Decompiled with JetBrains decompiler
// Type: BulletManMagicAstral1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable
[InspectorDropdownName("BulletManMagic/Astral1")]
public class BulletManMagicAstral1 : Script
{
  private const int AirTime = 180;

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new BulletManMagicAstral1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public class AstralBullet : Bullet
  {
    private Script m_parentScript;

    public AstralBullet(Script parentScript)
      : base("astral")
    {
      this.m_parentScript = parentScript;
    }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new BulletManMagicAstral1.AstralBullet.\u003CTop\u003Ec__Iterator0()
      {
        \u0024this = this
      };
    }

    public override void OnBulletDestruction(
      Bullet.DestroyType destroyType,
      SpeculativeRigidbody hitRigidbody,
      bool preventSpawningProjectiles)
    {
      if (this.m_parentScript == null)
        return;
      this.m_parentScript.ForceEnd();
    }
  }
}
