// Decompiled with JetBrains decompiler
// Type: DraGunGlockRicochet1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable
[InspectorDropdownName("Bosses/DraGun/GlockRicochet1")]
public class DraGunGlockRicochet1 : Script
{
  protected override IEnumerator Top()
  {
    float num1 = BraveMathCollege.ClampAngle180(this.Direction);
    if ((double) num1 > -91.0 && (double) num1 < -89.0)
    {
      int num2 = 8;
      float num3 = -170f;
      float num4 = 160f / (float) (num2 - 1);
      for (int index = 0; index < num2; ++index)
        this.Fire(new Brave.BulletScript.Direction(num3 + (float) index * num4), new Brave.BulletScript.Speed(9f), new Bullet("ricochet"));
      if ((double) BraveMathCollege.AbsAngleBetween(this.AimDirection, -90f) <= 90.0)
        this.Fire(new Brave.BulletScript.Direction(this.AimDirection), new Brave.BulletScript.Speed(9f), new Bullet("ricochet"));
    }
    else
    {
      int num5 = 8;
      float num6 = -45f;
      float num7 = 90f / (float) (num5 - 1);
      for (int index = 0; index < num5; ++index)
        this.Fire(new Brave.BulletScript.Direction(num6 + (float) index * num7, Brave.BulletScript.DirectionType.Relative), new Brave.BulletScript.Speed(9f), new Bullet("ricochet"));
    }
    return (IEnumerator) null;
  }
}
