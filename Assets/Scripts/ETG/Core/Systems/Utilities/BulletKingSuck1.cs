using System.Collections;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

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
            return (IEnumerator) new BulletKingSuck1__Topc__Iterator0()
            {
                _this = this
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
                return (IEnumerator) new BulletKingSuck1.SuckBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

