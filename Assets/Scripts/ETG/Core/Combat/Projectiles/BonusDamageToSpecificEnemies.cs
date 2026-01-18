using System;

using UnityEngine;

#nullable disable

public class BonusDamageToSpecificEnemies : MonoBehaviour
    {
        [EnemyIdentifier]
        public string[] enemyGuids;
        public float damageFraction = 0.5f;
        private Projectile m_projectile;

        public void Start()
        {
            this.m_projectile = this.GetComponent<Projectile>();
            this.m_projectile.OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy);
        }

        private void HandleHitEnemy(
            Projectile sourceProjectile,
            SpeculativeRigidbody hitEnemyRigidbody,
            bool killedEnemy)
        {
            if (killedEnemy || !(bool) (UnityEngine.Object) hitEnemyRigidbody.aiActor || Array.IndexOf<string>(this.enemyGuids, hitEnemyRigidbody.aiActor.EnemyGuid) == -1)
                return;
            hitEnemyRigidbody.aiActor.healthHaver.ApplyDamage(sourceProjectile.ModifiedDamage * this.damageFraction, Vector2.zero, "bonus damage");
        }
    }

