using System.Collections;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Minibosses/PhantomAgunim/Reflect1")]
public class PhantomAgunimNonReflect1 : Script
    {
        private const int NumRings = 6;
        private const int NumBulletsPerRing = 5;
        private const float RingRadius = 0.55f;
        private const float RingSpinSpeed = 450f;
        private const int RingDelay = 20;
        private const float DeltaStartAim = 10f;
        private const float StartSpeed = 10f;
        private const float SpeedIncrease = 2f;
        private const float RotationSpeed = 45f;
        private const float RotationSpeedIncrease = 10f;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new PhantomAgunimNonReflect1__Topc__Iterator0()
            {
                _this = this
            };
        }

        public override void OnForceEnded()
        {
            if (!(bool) (Object) this.BulletBank || !(bool) (Object) this.BulletBank.aiAnimator)
                return;
            this.BulletBank.aiAnimator.StopVfx("hover_charge_loop");
        }

        public class RingBullet : Bullet
        {
            private const int TicksBeforeStrighteningOut = 35;
            private const int TicksToStraightenOut = 30;
            private float m_angle;
            private float m_rotationSpeed;

            public RingBullet(float angle, float rotationSpeed)
                : base("ring")
            {
                this.m_angle = angle;
                this.m_rotationSpeed = rotationSpeed;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new PhantomAgunimNonReflect1.RingBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

