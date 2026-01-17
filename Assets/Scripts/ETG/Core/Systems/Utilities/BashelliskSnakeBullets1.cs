// Decompiled with JetBrains decompiler
// Type: BashelliskSnakeBullets1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Brave.BulletScript;
using FullInspector;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    [InspectorDropdownName("Bosses/Bashellisk/SnakeBullets1")]
    public class BashelliskSnakeBullets1 : Script
    {
      private const int NumBullets = 8;
      private const int BulletSpeed = 11;
      private const float SnakeMagnitude = 0.6f;
      private const float SnakePeriod = 3f;

      protected override IEnumerator Top()
      {
        float aimDirection = this.GetAimDirection((double) Random.value >= 0.5 ? 1f : 0.0f, 11f);
        for (int index = 0; index < 8; ++index)
          this.Fire(new Brave.BulletScript.Direction(aimDirection), new Brave.BulletScript.Speed(11f), (Bullet) new BashelliskSnakeBullets1.SnakeBullet(index * 3));
        return (IEnumerator) null;
      }

      public class SnakeBullet : Bullet
      {
        private int delay;

        public SnakeBullet(int delay)
          : base("snakeBullet")
        {
          this.delay = delay;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BashelliskSnakeBullets1.SnakeBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }
    }

}
