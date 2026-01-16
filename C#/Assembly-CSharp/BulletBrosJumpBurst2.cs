// Decompiled with JetBrains decompiler
// Type: BulletBrosJumpBurst2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable
[InspectorDropdownName("Bosses/BulletBros/JumpBurst2")]
public class BulletBrosJumpBurst2 : Script
{
  private const int NumFastBullets = 18;
  private const int NumSlowBullets = 9;

  protected override IEnumerator Top()
  {
    float startAngle1 = this.RandomAngle();
    for (int i = 0; i < 18; ++i)
      this.Fire(new Brave.BulletScript.Direction(this.SubdivideCircle(startAngle1, 18, i)), new Brave.BulletScript.Speed(9f), new Bullet("jump", true));
    float startAngle2 = startAngle1 + 10f;
    for (int i = 0; i < 9; ++i)
      this.Fire(new Brave.BulletScript.Direction(this.SubdivideCircle(startAngle2, 9, i)), new Brave.BulletScript.Speed(), (Bullet) new SpeedChangingBullet("jump", 9f, 75, suppressVfx: true));
    return (IEnumerator) null;
  }
}
