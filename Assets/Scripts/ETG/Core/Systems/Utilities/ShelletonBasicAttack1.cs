// Decompiled with JetBrains decompiler
// Type: ShelletonBasicAttack1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using System.Collections;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class ShelletonBasicAttack1 : Script
    {
      private const int NumBullets = 21;
      private const int NumPlugs = 2;

      protected override IEnumerator Top()
      {
        for (int index = 0; index < 21; ++index)
          this.Fire(new Brave.BulletScript.Direction(Mathf.Lerp(-80f, 80f, (float) index / 20f), Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(index % 2 != 0 ? 10f : 4f), (Bullet) new SpeedChangingBullet(10f, 60, 180));
        for (int index = 0; index < 2; ++index)
        {
          int num = Random.Range(0, 21);
          this.Fire(new Brave.BulletScript.Direction(Mathf.Lerp(-80f, 80f, (float) num / 20f), Brave.BulletScript.DirectionType.Aim), new Brave.BulletScript.Speed(num % 2 != 1 ? 10f : 4f), (Bullet) new SpeedChangingBullet(10f, 60, 180));
        }
        return (IEnumerator) null;
      }
    }

}
