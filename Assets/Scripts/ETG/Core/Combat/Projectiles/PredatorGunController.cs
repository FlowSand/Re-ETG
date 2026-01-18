using System;

using UnityEngine;

#nullable disable

public class PredatorGunController : MonoBehaviour
    {
        public float HomingRadius = 5f;
        public float HomingAngularVelocity = 360f;
        public GameObject LockOnVFX;
        private AIActor m_lastLockOnEnemy;
        private GameObject m_extantLockOnSprite;
        private Gun m_gun;

        private void Awake()
        {
            this.m_gun = this.GetComponent<Gun>();
            this.m_gun.PostProcessProjectile += new Action<Projectile>(this.PostProcessProjectile);
        }

        private void Update()
        {
            if (!(bool) (UnityEngine.Object) this.m_gun.CurrentOwner || !(this.m_gun.CurrentOwner is PlayerController))
                return;
            PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
            if (currentOwner.CurrentRoom == null)
                return;
            float nearestDistance = -1f;
            AIActor nearestEnemy = currentOwner.CurrentRoom.GetNearestEnemy(currentOwner.unadjustedAimPoint.XY(), out nearestDistance, excludeDying: true);
            if ((bool) (UnityEngine.Object) nearestEnemy)
                this.ProcessNearestEnemy(nearestEnemy);
            else
                this.ProcessNearestEnemy((AIActor) null);
        }

        private void ProcessNearestEnemy(AIActor hitEnemy)
        {
            if ((bool) (UnityEngine.Object) hitEnemy)
            {
                if (!((UnityEngine.Object) this.m_lastLockOnEnemy != (UnityEngine.Object) hitEnemy))
                    return;
                if ((bool) (UnityEngine.Object) this.m_extantLockOnSprite)
                    SpawnManager.Despawn(this.m_extantLockOnSprite);
                this.m_extantLockOnSprite = hitEnemy.PlayEffectOnActor((GameObject) BraveResources.Load("Global VFX/VFX_LockOn_Predator"), Vector3.zero, alreadyMiddleCenter: true, useHitbox: true);
                this.m_lastLockOnEnemy = hitEnemy;
            }
            else
            {
                if (!(bool) (UnityEngine.Object) this.m_extantLockOnSprite)
                    return;
                SpawnManager.Despawn(this.m_extantLockOnSprite);
            }
        }

        private void PostProcessProjectile(Projectile p)
        {
            if (!(bool) (UnityEngine.Object) this.m_lastLockOnEnemy)
                return;
            LockOnHomingModifier onHomingModifier = p.GetComponent<LockOnHomingModifier>();
            if (!(bool) (UnityEngine.Object) onHomingModifier)
            {
                onHomingModifier = p.gameObject.AddComponent<LockOnHomingModifier>();
                onHomingModifier.HomingRadius = 0.0f;
                onHomingModifier.AngularVelocity = 0.0f;
            }
            onHomingModifier.HomingRadius += this.HomingRadius;
            onHomingModifier.AngularVelocity += this.HomingAngularVelocity;
            onHomingModifier.LockOnVFX = this.LockOnVFX;
            onHomingModifier.AssignTargetManually(this.m_lastLockOnEnemy);
        }
    }

