// Decompiled with JetBrains decompiler
// Type: DraGunGlockDirected1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable

[InspectorDropdownName("Bosses/DraGun/GlockDirected1")]
public class DraGunGlockDirected1 : Script
  {
    protected virtual string BulletName => "glock";

    protected virtual bool IsHard => false;

    protected override IEnumerator Top()
    {
      float num = BraveMathCollege.ClampAngle180(this.Direction);
      if ((double) num > -91.0 && (double) num < -89.0)
      {
        int numBullets = 8;
        float startAngle = -170f;
        for (int i = 0; i < numBullets; ++i)
          this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(startAngle, 160f, numBullets, i)), new Brave.BulletScript.Speed(9f), new Bullet(this.BulletName));
        if (this.IsHard)
        {
          for (int i = 0; i < numBullets - 1; ++i)
            this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(startAngle, 160f, numBullets, i, true)), new Brave.BulletScript.Speed(1f), (Bullet) new SpeedChangingBullet(this.BulletName, 9f, 60));
        }
        if ((double) BraveMathCollege.AbsAngleBetween(this.AimDirection, -90f) <= 90.0)
          this.Fire(new Brave.BulletScript.Direction(this.AimDirection), new Brave.BulletScript.Speed(9f), new Bullet(this.BulletName));
      }
      else
      {
        int numBullets = 12;
        float startAngle = this.RandomAngle();
        for (int i = 0; i < numBullets; ++i)
          this.Fire(new Brave.BulletScript.Direction(this.SubdivideCircle(startAngle, numBullets, i)), new Brave.BulletScript.Speed(9f), new Bullet(this.BulletName + "_spin"));
        if (this.IsHard)
        {
          for (int i = 0; i < numBullets; ++i)
            this.Fire(new Brave.BulletScript.Direction(this.SubdivideCircle(startAngle, numBullets, i, offset: true)), new Brave.BulletScript.Speed(1f), (Bullet) new SpeedChangingBullet(this.BulletName + "_spin", 9f, 60));
        }
        this.Fire(new Brave.BulletScript.Direction(this.AimDirection), new Brave.BulletScript.Speed(9f), new Bullet(this.BulletName));
      }
      return (IEnumerator) null;
    }
  }

