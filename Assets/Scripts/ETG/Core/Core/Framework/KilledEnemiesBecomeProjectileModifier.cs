using System;

using UnityEngine;

#nullable disable

public class KilledEnemiesBecomeProjectileModifier : BraveBehaviour
    {
        public bool CompletelyBecomeProjectile;
        public Projectile BaseProjectile;
        private Projectile m_projectile;

        public void Start()
        {
            this.m_projectile = this.projectile;
            if (!(bool) (UnityEngine.Object) this.m_projectile)
                return;
            this.m_projectile.OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy);
        }

        private void HandleHitEnemy(
            Projectile sourceProjectile,
            SpeculativeRigidbody hitRigidbody,
            bool killedEnemy)
        {
            if (!killedEnemy || !(bool) (UnityEngine.Object) hitRigidbody)
                return;
            AIActor aiActor = hitRigidbody.aiActor;
            if (!(bool) (UnityEngine.Object) aiActor || !aiActor.IsNormalEnemy || !(bool) (UnityEngine.Object) aiActor.healthHaver || aiActor.healthHaver.IsBoss)
                return;
            if ((bool) (UnityEngine.Object) aiActor.GetComponent<ExplodeOnDeath>())
                UnityEngine.Object.Destroy((UnityEngine.Object) aiActor.GetComponent<ExplodeOnDeath>());
            if (this.CompletelyBecomeProjectile && (bool) (UnityEngine.Object) hitRigidbody.sprite)
            {
                aiActor.specRigidbody.enabled = false;
                aiActor.EraseFromExistence();
                Projectile component = UnityEngine.Object.Instantiate<GameObject>(this.BaseProjectile.gameObject, aiActor.transform.position, Quaternion.Euler(0.0f, 0.0f, sourceProjectile.LastVelocity.ToAngle())).GetComponent<Projectile>();
                component.sprite.SetSprite(hitRigidbody.sprite.Collection, hitRigidbody.sprite.spriteId);
                component.shouldRotate = true;
            }
            else
            {
                hitRigidbody.AddCollisionLayerOverride(CollisionMask.LayerToMask(CollisionLayer.EnemyHitBox));
                hitRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandleHitEnemyHitEnemy);
            }
        }

        private void HandleHitEnemyHitEnemy(
            SpeculativeRigidbody myRigidbody,
            PixelCollider myPixelCollider,
            SpeculativeRigidbody otherRigidbody,
            PixelCollider otherPixelCollider)
        {
            if (!(bool) (UnityEngine.Object) otherRigidbody || !(bool) (UnityEngine.Object) otherRigidbody.aiActor || !(bool) (UnityEngine.Object) myRigidbody || !(bool) (UnityEngine.Object) myRigidbody.healthHaver)
                return;
            AIActor aiActor = otherRigidbody.aiActor;
            myRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.HandleHitEnemyHitEnemy);
            if (!aiActor.IsNormalEnemy || !(bool) (UnityEngine.Object) aiActor.healthHaver)
                return;
            aiActor.healthHaver.ApplyDamage(myRigidbody.healthHaver.GetMaxHealth() * 2f, myRigidbody.Velocity, "Pinball");
        }
    }

