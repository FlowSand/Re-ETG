// Decompiled with JetBrains decompiler
// Type: BossDoorMimicPuke1
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
    [InspectorDropdownName("Bosses/BossDoorMimic/Puke1")]
    public class BossDoorMimicPuke1 : Script
    {
      private const int NumPulseBullets = 32 /*0x20*/;
      private const float PulseBulletSpeed = 4.5f;
      private const int NumInitialSnakes = 8;
      private const int NumLateSnakes = 6;
      private const int NumBulletsInSnake = 5;
      private const int SnakeBulletSpeed = 8;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BossDoorMimicPuke1.\u003CTop\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      public class PulseBullet : Bullet
      {
        private const float SinPeriod = 0.75f;
        private const float SinMagnitude = 0.75f;
        private float m_initialOffest;

        public PulseBullet(float initialOffest)
          : base("puke_burst")
        {
          this.m_initialOffest = initialOffest;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BossDoorMimicPuke1.PulseBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }

      public class SnakeBullet : Bullet
      {
        private const float SnakeMagnitude = 0.75f;
        private const float SnakePeriod = 3f;
        private int m_delay;
        private Vector2 m_target;
        private bool m_shouldHome;

        public SnakeBullet(int delay, Vector2 target, bool shouldHome)
          : base("puke_snake")
        {
          this.m_delay = delay;
          this.m_target = target;
          this.m_shouldHome = shouldHome;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BossDoorMimicPuke1.SnakeBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }
    }

}
