using System.Collections;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/Meduzi/Scream2 (optimized)")]
public class MeduziScream2 : Script
    {
        private const int NumWaves = 16;
        private const int NumBulletsPerWave = 64;
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
            return (IEnumerator) new MeduziScream2__Topc__Iterator0()
            {
                _this = this
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
            private bool m_hasArc;
            private Vector2 m_arcCenter;
            private float m_startAngle;
            private float m_endAngle;

            public TimedBullet(int bulletsFromSafeDir, float direction)
                : base("scream")
            {
                this.m_bulletsFromSafeDir = bulletsFromSafeDir;
                this.m_direction = direction;
            }

            public void SetArc(Vector2 center, float startAngle, float endAngle)
            {
                this.m_hasArc = true;
                this.m_arcCenter = center;
                this.m_startAngle = startAngle;
                this.m_endAngle = endAngle;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new MeduziScream2.TimedBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

