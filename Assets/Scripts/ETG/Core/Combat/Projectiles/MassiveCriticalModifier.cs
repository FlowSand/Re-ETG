using System;
using UnityEngine;

#nullable disable

public class MassiveCriticalModifier : MonoBehaviour
  {
    public float ActivationChance = 0.01f;
    public Projectile ReplacementProjectile;
    private Gun m_gun;

    private void Awake()
    {
      this.m_gun = this.GetComponent<Gun>();
      this.m_gun.OnPreFireProjectileModifier += new Func<Gun, Projectile, ProjectileModule, Projectile>(this.HandleProjectileReplacement);
    }

    private Projectile HandleProjectileReplacement(
      Gun sourceGun,
      Projectile sourceProjectile,
      ProjectileModule sourceModule)
    {
      if ((double) UnityEngine.Random.value > (double) this.ActivationChance)
        return sourceProjectile;
      this.ReplacementProjectile.IsCritical = true;
      return this.ReplacementProjectile;
    }
  }

