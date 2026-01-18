using System;

#nullable disable

[Serializable]
public class PlayerItemProjectileInterface
  {
    public bool UseCurrentGunProjectile = true;
    public Projectile SpecifiedProjectile;

    public Projectile GetProjectile(PlayerController targetPlayer)
    {
      if (this.UseCurrentGunProjectile && (bool) (UnityEngine.Object) targetPlayer.CurrentGun)
      {
        Projectile currentProjectile = targetPlayer.CurrentGun.DefaultModule.GetCurrentProjectile();
        if ((bool) (UnityEngine.Object) currentProjectile)
          return currentProjectile;
      }
      return this.SpecifiedProjectile;
    }
  }

