using System.Collections;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalBullet/AgunimLightning1")]
public class BossFinalBulletAgunimLightning1 : Script
    {
        public const float Dist = 0.8f;
        public const int MaxBulletDepth = 30;
        public const float RandomOffset = 0.3f;
        public const float TurnChance = 0.2f;
        public const float TurnAngle = 30f;

        protected override IEnumerator Top()
        {
            float direction = BraveMathCollege.QuantizeFloat(this.AimDirection, 45f);
            this.Fire(new Offset("lightning left shoot point"), (Bullet) new BossFinalBulletAgunimLightning1.LightningBullet(direction, -1f, 30, -4));
            this.Fire(new Offset("lightning left shoot point"), (Bullet) new BossFinalBulletAgunimLightning1.LightningBullet(direction, -1f, 30, 4));
            if (BraveUtility.RandomBool())
                this.Fire(new Offset("lightning left shoot point"), (Bullet) new BossFinalBulletAgunimLightning1.LightningBullet(direction, -1f, 30, 4));
            else
                this.Fire(new Offset("lightning right shoot point"), (Bullet) new BossFinalBulletAgunimLightning1.LightningBullet(direction, 1f, 30, 4));
            this.Fire(new Offset("lightning right shoot point"), (Bullet) new BossFinalBulletAgunimLightning1.LightningBullet(direction, 1f, 30, 4));
            this.Fire(new Offset("lightning right shoot point"), (Bullet) new BossFinalBulletAgunimLightning1.LightningBullet(direction, 1f, 30, -4));
            return (IEnumerator) null;
        }

        private class LightningBullet : Bullet
        {
            private float m_direction;
            private float m_sign;
            private int m_maxRemainingBullets;
            private int m_timeSinceLastTurn;
            private Vector2? m_truePosition;

            public LightningBullet(
                float direction,
                float sign,
                int maxRemainingBullets,
                int timeSinceLastTurn,
                Vector2? truePosition = null)
                : base()
            {
                this.m_direction = direction;
                this.m_sign = sign;
                this.m_maxRemainingBullets = maxRemainingBullets;
                this.m_timeSinceLastTurn = timeSinceLastTurn;
                this.m_truePosition = truePosition;
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new BossFinalBulletAgunimLightning1.LightningBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }
        }
    }

