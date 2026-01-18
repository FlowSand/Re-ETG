using System.Collections;
using System.Diagnostics;

using UnityEngine;

using Brave.BulletScript;

#nullable disable

public class WizardYellowSlam1 : Script
    {
        public const float Radius = 2f;
        public const int GrowTime = 15;
        public const float RotationSpeed = 180f;
        public const float BulletSpeed = 10f;

        public float aimDirection { get; private set; }

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new WizardYellowSlam1__Topc__Iterator0()
            {
                _this = this
            };
        }

        private void FireX()
        {
            Vector2 start1 = new Vector2(2f, 0.0f).Rotate(45f);
            Vector2 start2 = new Vector2(2f, 0.0f).Rotate(135f);
            Vector2 end1 = new Vector2(2f, 0.0f).Rotate(225f);
            Vector2 end2 = new Vector2(2f, 0.0f).Rotate(-45f);
            this.FireExpandingLine(start1, end1, 11);
            this.FireExpandingLine(start2, end2, 11);
        }

        private void FireSquare()
        {
            Vector2 vector2_1 = new Vector2(2f, 0.0f).Rotate(45f);
            Vector2 vector2_2 = new Vector2(2f, 0.0f).Rotate(135f);
            Vector2 vector2_3 = new Vector2(2f, 0.0f).Rotate(225f);
            Vector2 vector2_4 = new Vector2(2f, 0.0f).Rotate(-45f);
            this.FireExpandingLine(vector2_1, vector2_2, 9);
            this.FireExpandingLine(vector2_2, vector2_3, 9);
            this.FireExpandingLine(vector2_3, vector2_4, 9);
            this.FireExpandingLine(vector2_4, vector2_1, 9);
        }

        private void FireTriangle()
        {
            Vector2 vector2_1 = new Vector2(2f, 0.0f).Rotate(90f);
            Vector2 vector2_2 = new Vector2(2f, 0.0f).Rotate(210f);
            Vector2 vector2_3 = new Vector2(2f, 0.0f).Rotate(330f);
            this.FireExpandingLine(vector2_1, vector2_2, 10);
            this.FireExpandingLine(vector2_2, vector2_3, 10);
            this.FireExpandingLine(vector2_3, vector2_1, 10);
        }

        private void FireCircle()
        {
            for (int index = 0; index < 36; ++index)
                this.Fire((Bullet) new WizardYellowSlam1.ExpandingBullet(this, new Vector2(2f, 0.0f).Rotate((float) ((double) index / 35.0 * 360.0))));
        }

        private void FireExpandingLine(Vector2 start, Vector2 end, int numBullets)
        {
            for (int index = 0; index < numBullets; ++index)
                this.Fire((Bullet) new WizardYellowSlam1.ExpandingBullet(this, Vector2.Lerp(start, end, (float) index / ((float) numBullets - 1f))));
        }

        public class ExpandingBullet : Bullet
        {
            private WizardYellowSlam1 m_parent;
            private Vector2 m_offset;

            public ExpandingBullet(WizardYellowSlam1 parent, Vector2 offset)
                : base()
            {
                this.m_parent = parent;
                this.m_offset = offset;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new WizardYellowSlam1.ExpandingBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

