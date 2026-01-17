// Decompiled with JetBrains decompiler
// Type: FusebombRandomSimple1
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
    [InspectorDropdownName("Bosses/Fusebomb/RandomBurstsSimple1")]
    public class FusebombRandomSimple1 : Script
    {
      protected override IEnumerator Top()
      {
        if ((double) Random.value < 0.5)
        {
          int numBullets = 10;
          float startAngle = this.RandomAngle();
          for (int i = 0; i < numBullets; ++i)
            this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(startAngle, 360f, numBullets, i)), new Brave.BulletScript.Speed(9f), (Bullet) null);
        }
        else
        {
          int numBullets = 5;
          float aimDirection = this.AimDirection;
          float num = 35f;
          bool offset = BraveUtility.RandomBool();
          for (int i = 0; i < numBullets + (!offset ? 0 : -1); ++i)
            this.Fire(new Brave.BulletScript.Direction(this.SubdivideArc(aimDirection - num, num * 2f, numBullets, i, offset)), new Brave.BulletScript.Speed(9f), (Bullet) null);
        }
        return (IEnumerator) null;
      }
    }

}
