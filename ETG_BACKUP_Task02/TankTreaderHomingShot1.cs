// Decompiled with JetBrains decompiler
// Type: TankTreaderHomingShot1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;

#nullable disable
[InspectorDropdownName("Bosses/TankTreader/HomingShot1")]
public class TankTreaderHomingShot1 : Script
{
  private const int AirTime = 75;
  private const int NumDeathBullets = 16 /*0x10*/;

  protected override IEnumerator Top()
  {
    this.Fire(new Brave.BulletScript.Direction(type: Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(7.5f), (Bullet) new TankTreaderHomingShot1.HomingBullet());
    return (IEnumerator) null;
  }

  private class HomingBullet : Bullet
  {
    public HomingBullet()
      : base("homingBullet")
    {
    }

    [DebuggerHidden]
    protected override IEnumerator Top()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new TankTreaderHomingShot1.HomingBullet.\u003CTop\u003Ec__Iterator0()
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
      float num1 = this.RandomAngle();
      float num2 = 22.5f;
      for (int index = 0; index < 16 /*0x10*/; ++index)
        this.Fire(new Brave.BulletScript.Direction(num1 + num2 * (float) index), new Brave.BulletScript.Speed(10f), (Bullet) null);
      int num3 = (int) AkSoundEngine.PostEvent("Play_WPN_golddoublebarrelshotgun_shot_01", this.Projectile.gameObject);
    }
  }
}
