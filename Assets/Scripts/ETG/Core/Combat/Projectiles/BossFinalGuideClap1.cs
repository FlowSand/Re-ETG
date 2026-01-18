using System.Collections;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalGuide/Clap1")]
public class BossFinalGuideClap1 : Script
    {
        private const int NumBolts = 25;
        private const int BoltSpeed = 20;
        private Vector2 m_roomMin;
        private Vector2 m_roomMax;
        private int[] m_quarters = new int[4]{ 0, 1, 2, 3 };
        private int m_quarterIndex = 4;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossFinalGuideClap1__Topc__Iterator0()
            {
                _this = this
            };
        }

        [DebuggerHidden]
        private IEnumerator FireBolt()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new BossFinalGuideClap1__FireBoltc__Iterator1()
            {
                _this = this
            };
        }

        private class LightningBullet : Bullet
        {
            public LightningBullet()
                : base("lightning")
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new BossFinalGuideClap1.LightningBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }

            public override void OnBulletDestruction(
                Bullet.DestroyType destroyType,
                SpeculativeRigidbody hitRigidbody,
                bool preventSpawningProjectiles)
            {
                if (!(bool) (Object) this.Projectile || !(bool) (Object) this.Projectile.specRigidbody)
                    return;
                this.Projectile.specRigidbody.RemoveCollisionLayerIgnoreOverride(CollisionMask.LayerToMask(CollisionLayer.HighObstacle));
            }
        }
    }

