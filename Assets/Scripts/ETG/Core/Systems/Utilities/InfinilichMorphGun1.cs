using System.Collections;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/Infinilich/MorphGun1")]
public class InfinilichMorphGun1 : Script
    {
        private static int[][] LeftBulletOrder = new int[7][]
        {
            new int[2]{ 1, 8 },
            new int[2]{ 2, 9 },
            new int[2]{ 3, 10 },
            new int[3]{ 4, 11, 15 },
            new int[4]{ 5, 12, 16, 19 },
            new int[5]{ 6, 13, 17, 20, 22 },
            new int[5]{ 7, 14, 18, 21, 23 }
        };
        private static int[][] RightBulletOrder = new int[7][]
        {
            new int[2]{ 2, 9 },
            new int[2]{ 1, 8 },
            new int[2]{ 4, 11 },
            new int[3]{ 3, 10, 15 },
            new int[4]{ 7, 14, 18, 21 },
            new int[5]{ 6, 13, 17, 20, 23 },
            new int[5]{ 5, 12, 16, 19, 22 }
        };
        private float m_sign;

        protected override IEnumerator Top()
        {
            float num = BraveMathCollege.ClampAngle180(this.BulletBank.aiAnimator.FacingDirection);
            this.m_sign = (double) num > 90.0 || (double) num < -90.0 ? -1f : 1f;
            Vector2 vector2 = this.Position + new Vector2(this.m_sign * 2.5f, 1f);
            float angle = (this.BulletManager.PlayerPosition() - vector2).ToAngle();
            int[][] numArray = (double) this.m_sign >= 0.0 ? InfinilichMorphGun1.RightBulletOrder : InfinilichMorphGun1.LeftBulletOrder;
            for (int delay = 0; delay < numArray.Length; ++delay)
            {
                for (int index = 0; index < numArray[delay].Length; ++index)
                    this.Fire(new Offset("morph bullet " + (object) numArray[delay][index]), new Brave.BulletScript.Direction(angle), new Brave.BulletScript.Speed(), (Bullet) new InfinilichMorphGun1.GunBullet(delay));
            }
            return (IEnumerator) null;
        }

        public class GunBullet : Bullet
        {
            private int m_delay;

            public GunBullet(int delay)
                : base()
            {
                this.m_delay = delay;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new InfinilichMorphGun1.GunBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

