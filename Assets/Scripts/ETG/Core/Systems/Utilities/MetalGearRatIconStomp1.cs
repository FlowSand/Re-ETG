using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using FullInspector;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/MetalGearRat/IconStomp1")]
public class MetalGearRatIconStomp1 : Script
    {
        public int[] delay = new int[9]
        {
            10,
            15,
            15,
            15,
            10,
            15,
            10,
            10,
            0
        };
        public int[] xOffsets1 = new int[9]
        {
            0,
            -4,
            -9,
            -13,
            -16,
            -19,
            -18,
            -15,
            -11
        };
        public int[] yOffsets1 = new int[9]
        {
            0,
            3,
            8,
            13,
            16,
            21,
            36,
            39,
            44
        };
        public int[] xOffsets2 = new int[9]
        {
            40,
            33,
            26,
            19,
            14,
            8,
            7,
            11,
            16
        };
        public int[] yOffsets2 = new int[9]
        {
            -1,
            2,
            5,
            8,
            11,
            18,
            28,
            38,
            43
        };
        private const int SpreadTime = 60;
        private const int HalfBulletsPerLine = 8;
        private const float LineWidth = 20f;
        private const float GapWidth = 3.5f;
        private const float BulletSpeed = 9f;
        private const float AngleVariance = 8f;
        private const float PositionVariance = 0.4f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MetalGearRatIconStomp1__Topc__Iterator0()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator FireBullets(List<MetalGearRatIconStomp1.LineDummy> lineDummies)
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MetalGearRatIconStomp1__FireBulletsc__Iterator1()
            {
                lineDummies = lineDummies,
                _this = this
            };
        }

        private class LineDummy : Bullet
        {
            public LineDummy()
                : base()
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new MetalGearRatIconStomp1.LineDummy__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }

        private class IconBullet : Bullet
        {
            private MetalGearRatIconStomp1.LineDummy m_lineDummy;
            private float scale;

            public IconBullet(MetalGearRatIconStomp1.LineDummy lineDummy, float scale)
                : base()
            {
                this.m_lineDummy = lineDummy;
                this.scale = scale;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new MetalGearRatIconStomp1.IconBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

