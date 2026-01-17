// Decompiled with JetBrains decompiler
// Type: ShotgunCreecherUglyCircle1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using UnityEngine;

#nullable disable
public class ShotgunCreecherUglyCircle1 : Script
{
  private const int NumBulletNodes = 7;
  private const int NumBulletsPerNode = 2;

  protected override IEnumerator Top()
  {
    for (int index1 = 1; index1 <= 7; ++index1)
    {
      string transform = $"shoot point {index1}";
      for (int index2 = 0; index2 < 2; ++index2)
        this.Fire(new Offset(transform), new Brave.BulletScript.Direction(this.RandomAngle()), new Brave.BulletScript.Speed((float) Random.Range(8, 12)), (Bullet) new ShotgunCreecherUglyCircle1.CreecherBullet());
    }
    return (IEnumerator) null;
  }

  public class CreecherBullet : Bullet
  {
    public CreecherBullet()
      : base()
    {
    }

    protected override IEnumerator Top()
    {
      this.ChangeSpeed(new Brave.BulletScript.Speed(12f), 60);
      for (int index = 0; index < 60; ++index)
      {
        if ((bool) (Object) this.Projectile)
        {
          this.Projectile.Speed = this.Speed;
          this.Projectile.UpdateSpeed();
        }
      }
      return (IEnumerator) null;
    }
  }
}
