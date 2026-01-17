// Decompiled with JetBrains decompiler
// Type: BulletShotgunManWestDeathBurst1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("BulletShotgunMan/WestDeathBurst1")]
    public class BulletShotgunManWestDeathBurst1 : Script
    {
      protected override IEnumerator Top()
      {
        int numBullets1 = 5;
        for (int i = 0; i < numBullets1; ++i)
          this.Fire(new Brave.BulletScript.Direction(this.SubdivideCircle(0.0f, numBullets1, i)), new Brave.BulletScript.Speed(6.5f), new Bullet("flashybullet"));
        int numBullets2 = 5;
        for (int i = 0; i < numBullets2; ++i)
          this.Fire(new Brave.BulletScript.Direction(this.SubdivideCircle(0.0f, numBullets2, i, offset: true)), new Brave.BulletScript.Speed(10f), new Bullet("flashybullet"));
        int num = 3;
        for (int index = 0; index < num; ++index)
          this.Fire(new Brave.BulletScript.Direction(this.RandomAngle()), new Brave.BulletScript.Speed(12f), new Bullet("flashybullet"));
        this.Fire(new Brave.BulletScript.Direction(type: Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(9f), new Bullet("bigBullet"));
        this.Fire(new Brave.BulletScript.Direction(BraveUtility.RandomSign() * Random.Range(20f, 40f), Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(9f), new Bullet("bigBullet"));
        return (IEnumerator) null;
      }
    }

}
