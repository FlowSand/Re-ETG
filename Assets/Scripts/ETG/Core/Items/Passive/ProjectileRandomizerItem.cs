using System;
using UnityEngine;

#nullable disable

public class ProjectileRandomizerItem : PassiveItem
  {
    public float OverallChanceToTakeEffect = 0.5f;
    public float BeamShootDuration = 3f;
    private PlayerController m_player;

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      this.m_player = player;
      base.Pickup(player);
      player.OnPreFireProjectileModifier += new Func<Gun, Projectile, Projectile>(this.HandlePreFireProjectileModification);
    }

    private static float GetQualityModifiedAmmo(Gun cg)
    {
      switch (cg.quality)
      {
        case PickupObject.ItemQuality.A:
          return Mathf.Min((float) cg.AdjustedMaxAmmo * 0.8f, 250f);
        case PickupObject.ItemQuality.S:
          return Mathf.Min((float) cg.AdjustedMaxAmmo * 0.7f, 100f);
        default:
          return (float) cg.AdjustedMaxAmmo;
      }
    }

    public static Projectile GetRandomizerProjectileFromPlayer(
      PlayerController sourcePlayer,
      Projectile fallbackProjectile,
      int fallbackAmmo)
    {
      int num1 = fallbackAmmo;
      for (int index = 0; index < sourcePlayer.inventory.AllGuns.Count; ++index)
      {
        if ((bool) (UnityEngine.Object) sourcePlayer.inventory.AllGuns[index] && !sourcePlayer.inventory.AllGuns[index].InfiniteAmmo)
        {
          Gun allGun = sourcePlayer.inventory.AllGuns[index];
          num1 += Mathf.CeilToInt(ProjectileRandomizerItem.GetQualityModifiedAmmo(allGun));
        }
      }
      int num2 = fallbackAmmo;
      float num3 = (float) num1 * UnityEngine.Random.value;
      if ((double) num2 > (double) num3)
        return fallbackProjectile;
      for (int index1 = 0; index1 < sourcePlayer.inventory.AllGuns.Count; ++index1)
      {
        if ((bool) (UnityEngine.Object) sourcePlayer.inventory.AllGuns[index1] && !sourcePlayer.inventory.AllGuns[index1].InfiniteAmmo)
        {
          Gun allGun = sourcePlayer.inventory.AllGuns[index1];
          num2 += Mathf.CeilToInt(ProjectileRandomizerItem.GetQualityModifiedAmmo(allGun));
          if ((double) num2 > (double) num3)
          {
            ProjectileModule defaultModule = sourcePlayer.inventory.AllGuns[index1].DefaultModule;
            if (defaultModule.shootStyle == ProjectileModule.ShootStyle.Beam)
              return fallbackProjectile;
            if (defaultModule.shootStyle != ProjectileModule.ShootStyle.Charged)
              return defaultModule.GetCurrentProjectile() ?? fallbackProjectile;
            Projectile projectile = (Projectile) null;
            for (int index2 = 0; index2 < 15; ++index2)
            {
              ProjectileModule.ChargeProjectile chargeProjectile = defaultModule.chargeProjectiles[UnityEngine.Random.Range(0, defaultModule.chargeProjectiles.Count)];
              if (chargeProjectile != null)
                projectile = chargeProjectile.Projectile;
              if ((bool) (UnityEngine.Object) projectile)
                break;
            }
            return projectile ?? fallbackProjectile;
          }
        }
      }
      return fallbackProjectile;
    }

    private Projectile HandlePreFireProjectileModification(Gun sourceGun, Projectile sourceProjectile)
    {
      float chanceToTakeEffect = this.OverallChanceToTakeEffect;
      if ((bool) (UnityEngine.Object) sourceGun && (UnityEngine.Object) sourceGun.Volley != (UnityEngine.Object) null)
        chanceToTakeEffect /= (float) sourceGun.Volley.projectiles.Count;
      if ((double) UnityEngine.Random.value > (double) chanceToTakeEffect || (bool) (UnityEngine.Object) sourceGun && sourceGun.InfiniteAmmo)
        return sourceProjectile;
      int num1 = 0;
      if ((bool) (UnityEngine.Object) this.m_player && this.m_player.inventory != null)
      {
        for (int index = 0; index < this.m_player.inventory.AllGuns.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) this.m_player.inventory.AllGuns[index] && !this.m_player.inventory.AllGuns[index].InfiniteAmmo)
          {
            Gun allGun = this.m_player.inventory.AllGuns[index];
            num1 += Mathf.CeilToInt(ProjectileRandomizerItem.GetQualityModifiedAmmo(allGun));
          }
        }
        int num2 = 0;
        float num3 = (float) num1 * UnityEngine.Random.value;
        for (int index1 = 0; index1 < this.m_player.inventory.AllGuns.Count; ++index1)
        {
          if ((bool) (UnityEngine.Object) this.m_player.inventory.AllGuns[index1] && !this.m_player.inventory.AllGuns[index1].InfiniteAmmo)
          {
            Gun allGun = this.m_player.inventory.AllGuns[index1];
            num2 += Mathf.CeilToInt(ProjectileRandomizerItem.GetQualityModifiedAmmo(allGun));
            if ((double) num2 > (double) num3)
            {
              ProjectileModule defaultModule = this.m_player.inventory.AllGuns[index1].DefaultModule;
              if (defaultModule.shootStyle == ProjectileModule.ShootStyle.Beam)
              {
                BeamController.FreeFireBeam(defaultModule.GetCurrentProjectile(), this.m_player, this.m_player.CurrentGun.CurrentAngle, this.BeamShootDuration, true);
                return sourceProjectile;
              }
              if (defaultModule.shootStyle != ProjectileModule.ShootStyle.Charged)
                return defaultModule.GetCurrentProjectile() ?? sourceProjectile;
              Projectile projectile = (Projectile) null;
              for (int index2 = 0; index2 < 15; ++index2)
              {
                ProjectileModule.ChargeProjectile chargeProjectile = defaultModule.chargeProjectiles[UnityEngine.Random.Range(0, defaultModule.chargeProjectiles.Count)];
                if (chargeProjectile != null)
                  projectile = chargeProjectile.Projectile;
                if ((bool) (UnityEngine.Object) projectile)
                  break;
              }
              return projectile ?? sourceProjectile;
            }
          }
        }
      }
      return sourceProjectile;
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      this.m_player = (PlayerController) null;
      debrisObject.GetComponent<ProjectileRandomizerItem>().m_pickedUpThisRun = true;
      player.OnPreFireProjectileModifier -= new Func<Gun, Projectile, Projectile>(this.HandlePreFireProjectileModification);
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      if (!(bool) (UnityEngine.Object) this.m_player)
        return;
      this.m_player.OnPreFireProjectileModifier -= new Func<Gun, Projectile, Projectile>(this.HandlePreFireProjectileModification);
    }
  }

