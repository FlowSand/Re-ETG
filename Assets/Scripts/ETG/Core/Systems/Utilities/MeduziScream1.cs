// Decompiled with JetBrains decompiler
// Type: MeduziScream1
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
    [InspectorDropdownName("Bosses/Meduzi/Scream1")]
    public class MeduziScream1 : Script
    {
      private const int NumWaves = 16 /*0x10*/;
      private const int NumBulletsPerWave = 64 /*0x40*/;
      private const int NumGaps = 3;
      private const int StepOpenTime = 14;
      private const int GapHalfWidth = 3;
      private const int GapHoldWaves = 6;
      private const float TurnDegPerWave = 12f;
      private static float[] s_gapAngles;
      private GoopDefinition m_goopDefinition;

      [DebuggerHidden]
      protected override IEnumerator Top()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MeduziScream1.\u003CTop\u003Ec__Iterator0()
        {
          \u0024this = this
        };
      }

      private void SafeUpdateAngle(ref float idealGapAngle, SpeculativeRigidbody target)
      {
        bool flag1 = this.IsSafeAngle(idealGapAngle + 12f, target);
        bool flag2 = this.IsSafeAngle(idealGapAngle - 12f, target);
        if (flag1 && flag2 || !flag1 && !flag2)
          idealGapAngle += BraveUtility.RandomSign() * 12f;
        else
          idealGapAngle += (float) ((!flag1 ? -1.0 : 1.0) * 12.0);
      }

      private bool IsSafeAngle(float angle, SpeculativeRigidbody target)
      {
        float magnitude = Vector2.Distance(target.GetUnitCenter(ColliderType.HitBox), this.Position);
        Vector2 position = this.Position + BraveMathCollege.DegreesToVector(angle, magnitude);
        return !GameManager.Instance.Dungeon.data.isWall((int) position.x, (int) position.y) && !DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.m_goopDefinition).IsPositionInGoop(position);
      }

      private class TimedBullet : Bullet
      {
        private int m_bulletsFromSafeDir;
        private float m_direction;

        public TimedBullet(int bulletsFromSafeDir, float direction)
          : base("scream")
        {
          this.m_bulletsFromSafeDir = bulletsFromSafeDir;
          this.m_direction = direction;
        }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
          // ISSUE: object of a compiler-generated type is created
          return (IEnumerator) new MeduziScream1.TimedBullet.\u003CTop\u003Ec__Iterator0()
          {
            \u0024this = this
          };
        }
      }
    }

}
