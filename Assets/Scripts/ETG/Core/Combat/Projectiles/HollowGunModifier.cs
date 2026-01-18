using System;

using UnityEngine;

#nullable disable

public class HollowGunModifier : MonoBehaviour
    {
        public float DamageMultiplier = 1.5f;
        private Gun m_gun;

        public void Awake()
        {
            this.m_gun = this.GetComponent<Gun>();
            this.m_gun.PostProcessProjectile += new Action<Projectile>(this.HandleProcessProjectile);
        }

        private void HandleProcessProjectile(Projectile obj)
        {
            if (!(this.m_gun.CurrentOwner is PlayerController) || !(this.m_gun.CurrentOwner as PlayerController).IsDarkSoulsHollow)
                return;
            obj.baseData.damage *= this.DamageMultiplier;
        }
    }

