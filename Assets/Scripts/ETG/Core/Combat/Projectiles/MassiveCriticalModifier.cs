// Decompiled with JetBrains decompiler
// Type: MassiveCriticalModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
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

}
