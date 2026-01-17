// Decompiled with JetBrains decompiler
// Type: ChancebulonCubeSlam1
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

namespace ETG.Core.Combat.Projectiles
{
    [InspectorDropdownName("Chancebulon/CubeSlam1")]
    public class ChancebulonCubeSlam1 : Script
    {
      private const int NumBullets = 11;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new ChancebulonCubeSlam1.<Top>c__Iterator0()
        {
          _this = this
        };
      }

      private void FireLine(float startingAngle)
      {
        float num1 = 9f;
        for (int index = 0; index < 11; ++index)
        {
          float num2 = Mathf.Atan((float) (((double) index * (double) num1 - 45.0) / 45.0)) * 57.29578f;
          float f = Mathf.Cos(num2 * ((float) Math.PI / 180f));
          float num3 = (double) Mathf.Abs(f) >= 0.0001 ? 1f / f : 1f;
          this.Fire(new Brave.BulletScript.Direction(num2 + startingAngle), new Brave.BulletScript.Speed(num3 * 9f), (Bullet) new ChancebulonCubeSlam1.ReversingBullet());
        }
      }

      public class ReversingBullet : Bullet
      {
        public ReversingBullet()
          : base("reversible")
        {
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new ChancebulonCubeSlam1.ReversingBullet.<Top>c__Iterator0()
          {
            _this = this
          };
        }

        private void OnPreDeath(Vector2 deathDir) => this.Vanish();

        public override void OnBulletDestruction(
          Bullet.DestroyType destroyType,
          SpeculativeRigidbody hitRigidbody,
          bool preventSpawningProjectiles)
        {
          if (!(bool) (UnityEngine.Object) this.BulletBank || !(bool) (UnityEngine.Object) this.BulletBank.healthHaver)
            return;
          this.BulletBank.healthHaver.OnPreDeath -= new Action<Vector2>(this.OnPreDeath);
        }
      }
    }

}
