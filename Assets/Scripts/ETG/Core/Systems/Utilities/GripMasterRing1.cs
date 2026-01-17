// Decompiled with JetBrains decompiler
// Type: GripMasterRing1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class GripMasterRing1 : Script
    {
      protected override IEnumerator Top()
      {
        float aimDirection = this.AimDirection;
        int numBullets1 = 16 /*0x10*/;
        for (int i = 0; i < numBullets1; ++i)
          this.Fire(new Brave.BulletScript.Direction(this.SubdivideCircle(aimDirection, numBullets1, i)), new Brave.BulletScript.Speed(9f), (Bullet) null);
        float sweepAngle = 135f;
        float startAngle = aimDirection - sweepAngle / 2f;
        int numBullets2 = 7;
        for (int i = 0; i < numBullets2 - 1; ++i)
          this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(startAngle, sweepAngle, numBullets2, i, true)), new Brave.BulletScript.Speed(17f), (Bullet) new SpeedChangingBullet(9f, 30));
        for (int i = 0; i < numBullets2 - 1; ++i)
          this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(startAngle, sweepAngle, numBullets2, i, true)), new Brave.BulletScript.Speed(1f), (Bullet) new SpeedChangingBullet(9f, 30));
        return (IEnumerator) null;
      }
    }

}
