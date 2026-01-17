// Decompiled with JetBrains decompiler
// Type: BulletKingSuck1
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
    [InspectorDropdownName("Bosses/BulletKing/Suck1")]
    public class BulletKingSuck1 : Script
    {
      private const int NumBulletRings = 20;
      private const int BulletsPerRing = 6;
      private const float AngleDeltaPerRing = 10f;
      private const float StartRadius = 1f;
      private const float RadiusPerRing = 1f;
      public static float SpinningBulletSpinSpeed = 180f;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new BulletKingSuck1.\u003CTop\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      public class SuckBullet : Bullet
      {
        private Vector2 m_centerPoint;
        private float m_startAngle;
        private int m_index;

        public SuckBullet(Vector2 centerPoint, float startAngle, int i)
          : base("suck")
        {
          this.m_centerPoint = centerPoint;
          this.m_startAngle = startAngle;
          this.m_index = i;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new BulletKingSuck1.SuckBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }
    }

}
