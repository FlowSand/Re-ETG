// Decompiled with JetBrains decompiler
// Type: BossFinalMarineBelch1
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
    [InspectorDropdownName("Bosses/BossFinalMarine/Belch1")]
    public class BossFinalMarineBelch1 : Script
    {
      private const int NumSnakes = 10;
      private const int NumBullets = 5;
      private const int BulletSpeed = 12;
      private const float SnakeMagnitude = 0.75f;
      private const float SnakePeriod = 3f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossFinalMarineBelch1.<Top>c__Iterator0()
        {
          _this = this
        };
      }

      public class SnakeBullet : Bullet
      {
        private int delay;
        private Vector2 target;

        public SnakeBullet(int delay, Vector2 target)
          : base()
        {
          this.delay = delay;
          this.target = target;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BossFinalMarineBelch1.SnakeBullet.<Top>c__Iterator0()
          {
            _this = this
          };
        }
      }
    }

}
