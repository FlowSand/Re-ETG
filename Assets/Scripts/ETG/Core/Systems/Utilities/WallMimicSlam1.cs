// Decompiled with JetBrains decompiler
// Type: WallMimicSlam1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("MimicWall/Slam1")]
    public class WallMimicSlam1 : Script
    {
      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new WallMimicSlam1.\u003CTop\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      protected void FireLine(
        float centralAngle,
        float numBullets,
        float minAngle,
        float maxAngle,
        bool addBlackBullets = false)
      {
        float num1 = (float) (((double) maxAngle - (double) minAngle) / ((double) numBullets - 1.0));
        for (int index = 0; (double) index < (double) numBullets; ++index)
        {
          float num2 = Mathf.Atan((float) (((double) minAngle + (double) index * (double) num1) / 45.0)) * 57.29578f;
          float f = Mathf.Cos(num2 * ((float) Math.PI / 180f));
          float num3 = (double) Mathf.Abs(f) >= 0.0001 ? 1f / f : 1f;
          Bullet bullet = new Bullet();
          if (addBlackBullets && index % 2 == 1)
            bullet.ForceBlackBullet = true;
          this.Fire(new Brave.BulletScript.Direction(num2 + centralAngle), new Brave.BulletScript.Speed(num3 * 9f), bullet);
        }
      }
    }

}
