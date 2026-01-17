// Decompiled with JetBrains decompiler
// Type: InfinilichBasicSwingRight1
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
    [InspectorDropdownName("Bosses/Infinilich/BasicSwingRight1")]
    public class InfinilichBasicSwingRight1 : Script
    {
      private const float EnemyBulletSpeedItem = 12f;
      private static int[] ShootPoints = new int[11]
      {
        4,
        9,
        13,
        18,
        20,
        21,
        22,
        23,
        24,
        25,
        26
      };

      protected override IEnumerator Top()
      {
        for (int index = 0; index < InfinilichBasicSwingRight1.ShootPoints.Length; ++index)
        {
          string transform = "bullet limb " + (object) InfinilichBasicSwingRight1.ShootPoints[index];
          float leadAmount = Mathf.Lerp(0.0f, 2f, (float) index / ((float) InfinilichBasicSwingRight1.ShootPoints.Length - 1f));
          float aimDirection = this.GetAimDirection(transform, leadAmount, 12f);
          this.Fire(new Offset(transform), new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(12f));
        }
        return (IEnumerator) null;
      }
    }

}
