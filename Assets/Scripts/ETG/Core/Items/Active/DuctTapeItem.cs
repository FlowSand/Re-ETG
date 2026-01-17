// Decompiled with JetBrains decompiler
// Type: DuctTapeItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Active
{
    public class DuctTapeItem : PlayerItem
    {
      private Gun m_validSourceGun;
      private Gun m_validTargetGun;

      public override bool CanBeUsed(PlayerController user)
      {
        if (!(bool) (UnityEngine.Object) user || user.inventory == null || user.inventory.AllGuns.Count < 2)
          return false;
        int num = 0;
        for (int index = 0; index < user.inventory.AllGuns.Count; ++index)
        {
          if ((bool) (UnityEngine.Object) user.inventory.AllGuns[index] && !user.inventory.AllGuns[index].InfiniteAmmo && user.inventory.AllGuns[index].CanActuallyBeDropped(user))
            ++num;
        }
        return num >= 2 && this.IsGunValid(user.CurrentGun, this.m_validSourceGun) && base.CanBeUsed(user);
      }

      public static ProjectileVolleyData TransferDuctTapeModules(
        ProjectileVolleyData source,
        ProjectileVolleyData target,
        Gun targetGun)
      {
        ProjectileVolleyData instance = ScriptableObject.CreateInstance<ProjectileVolleyData>();
        if ((UnityEngine.Object) target != (UnityEngine.Object) null)
        {
          instance.InitializeFrom(target);
        }
        else
        {
          instance.projectiles = new List<ProjectileModule>();
          instance.projectiles.Add(ProjectileModule.CreateClone(targetGun.singleModule));
          instance.BeamRotationDegreesPerSecond = float.MaxValue;
        }
        for (int index = 0; index < source.projectiles.Count; ++index)
        {
          ProjectileModule projectile = source.projectiles[index];
          if (projectile.IsDuctTapeModule)
          {
            ProjectileModule clone = ProjectileModule.CreateClone(projectile);
            instance.projectiles.Add(clone);
          }
        }
        DuctTapeItem.ReconfigureVolley(instance);
        return instance;
      }

      protected static ProjectileVolleyData CombineVolleys(Gun sourceGun, Gun mergeGun)
      {
        ProjectileVolleyData instance = ScriptableObject.CreateInstance<ProjectileVolleyData>();
        if ((UnityEngine.Object) sourceGun.RawSourceVolley != (UnityEngine.Object) null)
        {
          instance.InitializeFrom(sourceGun.RawSourceVolley);
        }
        else
        {
          instance.projectiles = new List<ProjectileModule>();
          instance.projectiles.Add(ProjectileModule.CreateClone(sourceGun.singleModule));
          instance.BeamRotationDegreesPerSecond = float.MaxValue;
        }
        if ((UnityEngine.Object) mergeGun.RawSourceVolley != (UnityEngine.Object) null)
        {
          for (int index = 0; index < mergeGun.RawSourceVolley.projectiles.Count; ++index)
          {
            ProjectileModule clone = ProjectileModule.CreateClone(mergeGun.RawSourceVolley.projectiles[index]);
            clone.IsDuctTapeModule = true;
            clone.ignoredForReloadPurposes = clone.ammoCost <= 0 || clone.numberOfShotsInClip <= 0;
            instance.projectiles.Add(clone);
            if (!string.IsNullOrEmpty(mergeGun.gunSwitchGroup) && index == 0)
            {
              clone.runtimeGuid = clone.runtimeGuid == null ? Guid.NewGuid().ToString() : clone.runtimeGuid;
              sourceGun.AdditionalShootSoundsByModule.Add(clone.runtimeGuid, mergeGun.gunSwitchGroup);
            }
            if (mergeGun.RawSourceVolley.projectiles[index].runtimeGuid != null && mergeGun.AdditionalShootSoundsByModule.ContainsKey(mergeGun.RawSourceVolley.projectiles[index].runtimeGuid))
              sourceGun.AdditionalShootSoundsByModule.Add(mergeGun.RawSourceVolley.projectiles[index].runtimeGuid, mergeGun.AdditionalShootSoundsByModule[mergeGun.RawSourceVolley.projectiles[index].runtimeGuid]);
          }
        }
        else
        {
          ProjectileModule clone = ProjectileModule.CreateClone(mergeGun.singleModule);
          clone.IsDuctTapeModule = true;
          clone.ignoredForReloadPurposes = clone.ammoCost <= 0 || clone.numberOfShotsInClip <= 0;
          instance.projectiles.Add(clone);
          if (!string.IsNullOrEmpty(mergeGun.gunSwitchGroup))
          {
            clone.runtimeGuid = clone.runtimeGuid == null ? Guid.NewGuid().ToString() : clone.runtimeGuid;
            sourceGun.AdditionalShootSoundsByModule.Add(clone.runtimeGuid, mergeGun.gunSwitchGroup);
          }
        }
        return instance;
      }

      protected static void ReconfigureVolley(ProjectileVolleyData newVolley)
      {
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        bool flag4 = false;
        int num1 = 0;
        for (int index = 0; index < newVolley.projectiles.Count; ++index)
        {
          if (newVolley.projectiles[index].shootStyle == ProjectileModule.ShootStyle.Automatic)
            flag1 = true;
          if (newVolley.projectiles[index].shootStyle == ProjectileModule.ShootStyle.Beam)
            flag1 = true;
          if (newVolley.projectiles[index].shootStyle == ProjectileModule.ShootStyle.Burst)
            flag4 = true;
          if (newVolley.projectiles[index].shootStyle == ProjectileModule.ShootStyle.Charged)
          {
            flag3 = true;
            ++num1;
          }
        }
        if (!flag1 && !flag2 && !flag3 && !flag4 || !flag1 && !flag2 && !flag3 && flag4)
          return;
        int num2 = 0;
        for (int index = 0; index < newVolley.projectiles.Count; ++index)
        {
          if (newVolley.projectiles[index].shootStyle == ProjectileModule.ShootStyle.SemiAutomatic)
            newVolley.projectiles[index].shootStyle = ProjectileModule.ShootStyle.Automatic;
          if (newVolley.projectiles[index].shootStyle == ProjectileModule.ShootStyle.Charged && num1 > 1)
          {
            ++num2;
            if (num1 <= 1)
              ;
          }
        }
      }

      protected Gun GetValidGun(PlayerController user, Gun excluded = null)
      {
        int num = user.inventory.AllGuns.IndexOf(user.CurrentGun);
        if (num < 0)
          num = 0;
        for (int index1 = num; index1 < num + user.inventory.AllGuns.Count; ++index1)
        {
          int index2 = index1 % user.inventory.AllGuns.Count;
          Gun allGun = user.inventory.AllGuns[index2];
          if (!allGun.InfiniteAmmo && allGun.CanActuallyBeDropped(user) && !((UnityEngine.Object) allGun == (UnityEngine.Object) excluded))
            return allGun;
        }
        return (Gun) null;
      }

      protected bool IsGunValid(Gun g, Gun excluded)
      {
        return !g.InfiniteAmmo && g.CanActuallyBeDropped(g.CurrentOwner as PlayerController) && !((UnityEngine.Object) g == (UnityEngine.Object) excluded);
      }

      public static void DuctTapeGuns(Gun merged, Gun target)
      {
        ProjectileVolleyData newVolley = DuctTapeItem.CombineVolleys(target, merged);
        DuctTapeItem.ReconfigureVolley(newVolley);
        target.RawSourceVolley = newVolley;
        target.SetBaseMaxAmmo(target.GetBaseMaxAmmo() + merged.GetBaseMaxAmmo());
        target.GainAmmo(merged.CurrentAmmo);
        if (target.DuctTapeMergedGunIDs == null)
          target.DuctTapeMergedGunIDs = new List<int>();
        if (merged.DuctTapeMergedGunIDs != null)
          target.DuctTapeMergedGunIDs.AddRange((IEnumerable<int>) merged.DuctTapeMergedGunIDs);
        target.DuctTapeMergedGunIDs.Add(merged.PickupObjectId);
      }

      protected override void DoActiveEffect(PlayerController user)
      {
        if ((bool) (UnityEngine.Object) user && (bool) (UnityEngine.Object) user.CurrentGun && this.IsGunValid(user.CurrentGun, this.m_validSourceGun))
        {
          this.m_validTargetGun = user.CurrentGun;
          if (!(bool) (UnityEngine.Object) this.m_validSourceGun || !(bool) (UnityEngine.Object) this.m_validTargetGun)
            return;
          DuctTapeItem.DuctTapeGuns(this.m_validSourceGun, this.m_validTargetGun);
          user.inventory.RemoveGunFromInventory(this.m_validSourceGun);
          UnityEngine.Object.Destroy((UnityEngine.Object) this.m_validSourceGun.gameObject);
          user.stats.RecalculateStats(user);
        }
        this.m_isCurrentlyActive = false;
      }

      protected override void DoEffect(PlayerController user)
      {
        if (user.inventory.AllGuns.Count < 2)
          return;
        this.m_validSourceGun = (Gun) null;
        this.m_validTargetGun = (Gun) null;
        if (!(bool) (UnityEngine.Object) user || !(bool) (UnityEngine.Object) user.CurrentGun || !this.IsGunValid(user.CurrentGun, this.m_validSourceGun))
          return;
        this.m_validSourceGun = user.CurrentGun;
        this.m_isCurrentlyActive = true;
      }

      protected override void OnDestroy() => base.OnDestroy();
    }

}
