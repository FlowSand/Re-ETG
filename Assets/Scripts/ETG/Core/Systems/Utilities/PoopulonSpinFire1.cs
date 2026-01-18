using System.Collections;
using System.Diagnostics;

using UnityEngine;

using Brave.BulletScript;

#nullable disable

public class PoopulonSpinFire1 : Script
    {
        private const int NumBullets = 100;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PoopulonSpinFire1__Topc__Iterator0()
            {
                _this = this
            };
        }

        public class RotatingBullet : Bullet
        {
            private Vector2 m_origin;

            public RotatingBullet(Vector2 origin)
                : base()
            {
                this.m_origin = origin;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new PoopulonSpinFire1.RotatingBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

