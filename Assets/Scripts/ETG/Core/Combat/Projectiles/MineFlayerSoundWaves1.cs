using System.Collections;
using System.Diagnostics;

using FullInspector;
using UnityEngine;

using Brave.BulletScript;

#nullable disable

[InspectorDropdownName("Bosses/MineFlayer/SoundWaves1")]
public class MineFlayerSoundWaves1 : Script
    {
        private const int NumWaves = 5;
        private const int NumBullets = 18;

        [DebuggerHidden]
        protected override IEnumerator Top()
        {
            // ISSUE: object of a compiler-generated type is created
            return (IEnumerator) new MineFlayerSoundWaves1__Topc__Iterator0()
            {
                _this = this
            };
        }

        private class ReflectBullet : Bullet
        {
            private int m_ticksLeft = -1;

            public ReflectBullet()
                : base("bounce")
            {
            }

            [DebuggerHidden]
            protected override IEnumerator Top()
            {
                // ISSUE: object of a compiler-generated type is created
                return (IEnumerator) new MineFlayerSoundWaves1.ReflectBullet__Topc__Iterator0()
                {
                    _this = this
                };
            }

            private void OnTileCollision(CollisionData tilecollision) => this.Reflect();

            private void Reflect()
            {
                this.Speed = 8f;
                this.Direction += 180f + Random.Range(-10f, 10f);
                this.Velocity = BraveMathCollege.DegreesToVector(this.Direction, this.Speed);
                PhysicsEngine.PostSliceVelocity = new Vector2?(this.Velocity);
                this.m_ticksLeft = (int) ((double) this.Tick * 1.5);
                if ((bool) (Object) this.Projectile.TrailRendererController)
                    this.Projectile.TrailRendererController.Stop();
                this.Projectile.BulletScriptSettings.surviveTileCollisions = false;
                this.Projectile.specRigidbody.OnTileCollision -= new SpeculativeRigidbody.OnTileCollisionDelegate(this.OnTileCollision);
            }

            public override void OnBulletDestruction(
                Bullet.DestroyType destroyType,
                SpeculativeRigidbody hitRigidbody,
                bool preventSpawningProjectiles)
            {
                if (!(bool) (Object) this.Projectile)
                    return;
                this.Projectile.specRigidbody.OnTileCollision -= new SpeculativeRigidbody.OnTileCollisionDelegate(this.OnTileCollision);
            }
        }
    }

