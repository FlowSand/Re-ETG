// Decompiled with JetBrains decompiler
// Type: MushroomGuySmallWaft1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    [InspectorDropdownName("MushroomGuy/SmallWaft1")]
    public class MushroomGuySmallWaft1 : Script
    {
      private const int NumWaftBullets = 30;
      private const int NumFastBullets = 10;
      private const float VerticalDriftVelocity = -0.5f;
      private const float WaftXPeriod = 3f;
      private const float WaftXMagnitude = 0.5f;
      private const float WaftYPeriod = 1f;
      private const float WaftYMagnitude = 0.125f;
      private const int WaftLifeTime = 300;

      protected override IEnumerator Top()
      {
        for (int index = 0; index < 30; ++index)
        {
          string bankName = (double) Random.value > 0.33000001311302185 ? "spore2" : "spore1";
          this.Fire(new Brave.BulletScript.Direction(this.RandomAngle()), new Brave.BulletScript.Speed(Random.Range(1.2f, 6f)), (Bullet) new MushroomGuySmallWaft1.WaftBullet(bankName));
        }
        for (int index = 0; index < 10; ++index)
        {
          Bullet bullet = (Bullet) new SpeedChangingBullet((double) Random.value > 0.33000001311302185 ? "spore2" : "spore1", 9f, 75, 300);
          this.Fire(new Brave.BulletScript.Direction(this.RandomAngle()), new Brave.BulletScript.Speed((float) Random.Range(2, 16 /*0x10*/)), bullet);
          bullet.Projectile.spriteAnimator.Play();
        }
        return (IEnumerator) null;
      }

      public class WaftBullet(string bankName) : Bullet(bankName)
      {
        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new MushroomGuySmallWaft1.WaftBullet__Topc__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
