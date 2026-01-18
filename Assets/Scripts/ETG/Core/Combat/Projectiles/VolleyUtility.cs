// Decompiled with JetBrains decompiler
// Type: VolleyUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable

  public static class VolleyUtility
  {
    public static void FireVolley(
      ProjectileVolleyData sourceVolley,
      Vector2 shootPoint,
      Vector2 aimDirection,
      GameActor possibleOwner = null,
      bool treatedAsNonProjectileForChallenge = false)
    {
      for (int index = 0; index < sourceVolley.projectiles.Count; ++index)
        VolleyUtility.ShootSingleProjectile(sourceVolley.projectiles[index], sourceVolley, shootPoint, BraveMathCollege.Atan2Degrees(aimDirection), 0.0f, possibleOwner, treatedAsNonProjectileForChallenge);
    }

    public static void ShootSingleProjectile(
      ProjectileModule mod,
      ProjectileVolleyData volley,
      Vector2 shootPoint,
      float fireAngle,
      float chargeTime,
      GameActor possibleOwner = null,
      bool treatedAsNonProjectileForChallenge = false)
    {
      Projectile projectile = (Projectile) null;
      if (mod.shootStyle == ProjectileModule.ShootStyle.Charged)
      {
        ProjectileModule.ChargeProjectile chargeProjectile = mod.GetChargeProjectile(chargeTime);
        if (chargeProjectile != null)
        {
          projectile = chargeProjectile.Projectile;
          projectile.pierceMinorBreakables = true;
        }
      }
      else
        projectile = mod.GetCurrentProjectile();
      if (!(bool) (Object) projectile)
      {
        if (mod.shootStyle == ProjectileModule.ShootStyle.Charged)
          return;
        mod.IncrementShootCount();
      }
      else
      {
        float angleForShot = mod.GetAngleForShot();
        Projectile component1 = SpawnManager.SpawnProjectile(projectile.gameObject, shootPoint.ToVector3ZisY() + Quaternion.Euler(0.0f, 0.0f, fireAngle) * mod.positionOffset, Quaternion.Euler(0.0f, 0.0f, fireAngle + angleForShot)).GetComponent<Projectile>();
        if ((bool) (Object) possibleOwner)
        {
          component1.Owner = possibleOwner;
          component1.Shooter = possibleOwner.specRigidbody;
        }
        if (treatedAsNonProjectileForChallenge)
          component1.TreatedAsNonProjectileForChallenge = true;
        component1.Inverted = mod.inverted;
        if ((Object) volley != (Object) null && volley.UsesShotgunStyleVelocityRandomizer)
          component1.baseData.speed *= volley.GetVolleySpeedMod();
        component1.PlayerProjectileSourceGameTimeslice = UnityEngine.Time.time;
        if (possibleOwner is PlayerController)
          (possibleOwner as PlayerController).DoPostProcessProjectile(component1);
        if (!mod.mirror)
          return;
        Projectile component2 = SpawnManager.SpawnProjectile(projectile.gameObject, shootPoint.ToVector3ZisY() + Quaternion.Euler(0.0f, 0.0f, fireAngle) * mod.InversePositionOffset, Quaternion.Euler(0.0f, 0.0f, fireAngle - angleForShot)).GetComponent<Projectile>();
        component2.Inverted = true;
        if ((bool) (Object) possibleOwner)
        {
          component2.Owner = possibleOwner;
          component2.Shooter = possibleOwner.specRigidbody;
          if ((bool) (Object) possibleOwner.aiShooter)
            component2.collidesWithEnemies = possibleOwner.aiShooter.CanShootOtherEnemies;
        }
        if (treatedAsNonProjectileForChallenge)
          component2.TreatedAsNonProjectileForChallenge = true;
        component2.PlayerProjectileSourceGameTimeslice = UnityEngine.Time.time;
        if (possibleOwner is PlayerController)
          (possibleOwner as PlayerController).DoPostProcessProjectile(component2);
        component2.baseData.SetAll(component1.baseData);
        component2.IsCritical = component1.IsCritical;
      }
    }

    public static Projectile ShootSingleProjectile(
      Projectile currentProjectile,
      Vector2 shootPoint,
      float fireAngle,
      bool inverted,
      GameActor possibleOwner = null)
    {
      float num = 0.0f;
      Projectile component = SpawnManager.SpawnProjectile(currentProjectile.gameObject, shootPoint.ToVector3ZisY(), Quaternion.Euler(0.0f, 0.0f, fireAngle + num)).GetComponent<Projectile>();
      if ((bool) (Object) possibleOwner)
      {
        component.Owner = possibleOwner;
        component.Shooter = possibleOwner.specRigidbody;
      }
      component.Inverted = inverted;
      component.PlayerProjectileSourceGameTimeslice = UnityEngine.Time.time;
      if (possibleOwner is PlayerController)
        (possibleOwner as PlayerController).DoPostProcessProjectile(component);
      return component;
    }
  }

