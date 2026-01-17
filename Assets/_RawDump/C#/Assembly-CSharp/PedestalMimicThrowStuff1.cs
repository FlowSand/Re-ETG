// Decompiled with JetBrains decompiler
// Type: PedestalMimicThrowStuff1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using System.Diagnostics;

#nullable disable
public class PedestalMimicThrowStuff1 : Script
{
  private static readonly string[] BulletNames = new string[3]
  {
    "boot",
    "gun",
    "sponge"
  };
  private const float HomingSpeed = 12f;

  [DebuggerHidden]
  protected override IEnumerator Top()
  {
    // ISSUE: object of a compiler-generated type is created
    return (IEnumerator) new PedestalMimicThrowStuff1.\u003CTop\u003Ec__Iterator0()
    {
      \u0024this = this
    };
  }

  public class AcceleratingBullet : Bullet
  {
    public AcceleratingBullet()
      : base("default")
    {
    }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new PedestalMimicThrowStuff1.AcceleratingBullet.\u003CTop\u003Ec__Iterator0()
      {
        \u0024this = this
      };
    }
  }

  public class HomingShot(string bulletName) : Bullet(bulletName)
  {
    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new PedestalMimicThrowStuff1.HomingShot.\u003CTop\u003Ec__Iterator0()
      {
        \u0024this = this
      };
    }

    public override void OnBulletDestruction(
      Bullet.DestroyType destroyType,
      SpeculativeRigidbody hitRigidbody,
      bool preventSpawningProjectiles)
    {
      if (preventSpawningProjectiles)
        return;
      for (int index = 0; index < 8; ++index)
        this.Fire(new Brave.BulletScript.Direction((float) (index * 45)), new Brave.BulletScript.Speed(8f), (Bullet) new SpeedChangingBullet(10f, 120, 600));
    }
  }
}
