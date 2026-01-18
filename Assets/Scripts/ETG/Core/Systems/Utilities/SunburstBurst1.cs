// Decompiled with JetBrains decompiler
// Type: SunburstBurst1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;

#nullable disable

[InspectorDropdownName("Sunburst/Burst1")]
public class SunburstBurst1 : Script
  {
    private const int NumBullets = 24;

    protected override IEnumerator Top()
    {
      float num1 = this.RandomAngle();
      float num2 = 15f;
      for (int index = 0; index < 24; ++index)
        this.Fire(new Brave.BulletScript.Direction(num1 + (float) index * num2), new Brave.BulletScript.Speed(9f), (Bullet) new SunburstBurst1.BurstBullet());
      return (IEnumerator) null;
    }

    public class BurstBullet : Bullet
    {
      public BurstBullet()
        : base()
      {
      }

      protected override IEnumerator Top()
      {
        this.ChangeSpeed(new Brave.BulletScript.Speed(5f), 40);
        return (IEnumerator) null;
      }
    }
  }

