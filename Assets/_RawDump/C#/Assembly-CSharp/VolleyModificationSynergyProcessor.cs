// Decompiled with JetBrains decompiler
// Type: VolleyModificationSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class VolleyModificationSynergyProcessor : MonoBehaviour
{
  public VolleyModificationSynergyData[] synergies;
  private Gun m_gun;
  private PassiveItem m_item;

  public void Awake()
  {
    this.m_gun = this.GetComponent<Gun>();
    if ((bool) (UnityEngine.Object) this.m_gun)
    {
      this.m_gun.PostProcessVolley += new Action<ProjectileVolleyData>(this.HandleVolleyRebuild);
      bool flag = false;
      if (this.synergies != null)
      {
        for (int index = 0; index < this.synergies.Length; ++index)
        {
          if (this.synergies[index].ReplacesSourceProjectile)
          {
            flag = true;
            break;
          }
        }
      }
      if (!flag)
        return;
      this.m_gun.OnPreFireProjectileModifier += new Func<Gun, Projectile, ProjectileModule, Projectile>(this.HandlePreFireProjectileReplacement);
    }
    else
    {
      this.m_item = this.GetComponent<PassiveItem>();
      if (!(bool) (UnityEngine.Object) this.m_item)
        return;
      this.m_item.OnPickedUp += new Action<PlayerController>(this.LinkPassiveItem);
      this.m_item.OnDisabled += new Action<PlayerController>(this.DelinkPassiveItem);
    }
  }

  private Projectile HandlePreFireProjectileReplacementPlayer(
    Gun sourceGun,
    Projectile sourceProjectile)
  {
    Projectile projectile = sourceProjectile;
    PlayerController currentOwner = sourceGun.CurrentOwner as PlayerController;
    if (this.synergies != null)
    {
      for (int index = 0; index < this.synergies.Length; ++index)
      {
        VolleyModificationSynergyData synergy = this.synergies[index];
        if (synergy.ReplacesSourceProjectile && (bool) (UnityEngine.Object) currentOwner && currentOwner.HasActiveBonusSynergy(synergy.RequiredSynergy) && (double) UnityEngine.Random.value < (double) synergy.ReplacementChance)
        {
          if (synergy.UsesMultipleReplacementProjectiles)
          {
            if (synergy.MultipleReplacementsSequential)
            {
              projectile = synergy.MultipleReplacementProjectiles[synergy.multipleSequentialReplacementIndex];
              synergy.multipleSequentialReplacementIndex = (synergy.multipleSequentialReplacementIndex + 1) % synergy.MultipleReplacementProjectiles.Length;
            }
            else
              projectile = synergy.MultipleReplacementProjectiles[UnityEngine.Random.Range(0, synergy.MultipleReplacementProjectiles.Length)];
          }
          else
            projectile = synergy.ReplacementProjectile;
        }
      }
    }
    return projectile;
  }

  private Projectile HandlePreFireProjectileReplacement(
    Gun sourceGun,
    Projectile sourceProjectile,
    ProjectileModule sourceModule)
  {
    Projectile projectile = sourceProjectile;
    PlayerController currentOwner = sourceGun.CurrentOwner as PlayerController;
    if (this.synergies != null)
    {
      for (int index1 = 0; index1 < this.synergies.Length; ++index1)
      {
        VolleyModificationSynergyData synergy = this.synergies[index1];
        if (synergy.ReplacesSourceProjectile && (bool) (UnityEngine.Object) currentOwner && currentOwner.HasActiveBonusSynergy(synergy.RequiredSynergy) && (!synergy.OnlyReplacesAdditionalProjectiles || sourceModule.ignoredForReloadPurposes) && (!(bool) (UnityEngine.Object) sourceGun || !sourceGun.IsCharging || synergy.RequiredSynergy == CustomSynergyType.ANTIMATTER_BODY))
        {
          if (synergy.ReplacementSkipsChargedShots && sourceModule.shootStyle == ProjectileModule.ShootStyle.Charged)
          {
            bool flag = false;
            for (int index2 = 0; index2 < sourceModule.chargeProjectiles.Count; ++index2)
            {
              if ((UnityEngine.Object) sourceModule.chargeProjectiles[index2].Projectile == (UnityEngine.Object) sourceProjectile && (double) sourceModule.chargeProjectiles[index2].ChargeTime > 0.0)
              {
                flag = true;
                break;
              }
            }
            if (flag)
              continue;
          }
          if ((double) UnityEngine.Random.value < (double) synergy.ReplacementChance)
          {
            if (synergy.UsesMultipleReplacementProjectiles)
            {
              if (synergy.MultipleReplacementsSequential)
              {
                projectile = synergy.MultipleReplacementProjectiles[synergy.multipleSequentialReplacementIndex];
                synergy.multipleSequentialReplacementIndex = (synergy.multipleSequentialReplacementIndex + 1) % synergy.MultipleReplacementProjectiles.Length;
              }
              else
                projectile = synergy.MultipleReplacementProjectiles[UnityEngine.Random.Range(0, synergy.MultipleReplacementProjectiles.Length)];
            }
            else
              projectile = synergy.ReplacementProjectile;
          }
        }
      }
    }
    return projectile;
  }

  private void LinkPassiveItem(PlayerController p)
  {
    p.stats.AdditionalVolleyModifiers -= new Action<ProjectileVolleyData>(this.HandleVolleyRebuild);
    p.stats.AdditionalVolleyModifiers += new Action<ProjectileVolleyData>(this.HandleVolleyRebuild);
    bool flag = false;
    if (this.synergies != null)
    {
      for (int index = 0; index < this.synergies.Length; ++index)
      {
        if (this.synergies[index].ReplacesSourceProjectile)
        {
          flag = true;
          break;
        }
      }
    }
    if (!flag)
      return;
    p.OnPreFireProjectileModifier += new Func<Gun, Projectile, Projectile>(this.HandlePreFireProjectileReplacementPlayer);
  }

  private void DelinkPassiveItem(PlayerController p)
  {
    if (!(bool) (UnityEngine.Object) p || !((UnityEngine.Object) p.stats != (UnityEngine.Object) null))
      return;
    p.stats.AdditionalVolleyModifiers -= new Action<ProjectileVolleyData>(this.HandleVolleyRebuild);
    p.OnPreFireProjectileModifier -= new Func<Gun, Projectile, Projectile>(this.HandlePreFireProjectileReplacementPlayer);
  }

  private void HandleVolleyRebuild(ProjectileVolleyData targetVolley)
  {
    PlayerController owner = (PlayerController) null;
    if ((bool) (UnityEngine.Object) this.m_gun)
      owner = this.m_gun.CurrentOwner as PlayerController;
    else if ((bool) (UnityEngine.Object) this.m_item)
      owner = this.m_item.Owner;
    if (!(bool) (UnityEngine.Object) owner || this.synergies == null)
      return;
    for (int index = 0; index < this.synergies.Length; ++index)
    {
      if (owner.HasActiveBonusSynergy(this.synergies[index].RequiredSynergy))
        this.ApplySynergy(targetVolley, this.synergies[index], owner);
    }
  }

  private void ApplySynergy(
    ProjectileVolleyData volley,
    VolleyModificationSynergyData synergy,
    PlayerController owner)
  {
    if (synergy.AddsChargeProjectile)
      volley.projectiles[0].chargeProjectiles.Add(synergy.ChargeProjectileToAdd);
    if (synergy.AddsModules)
    {
      bool flag = true;
      if ((UnityEngine.Object) volley != (UnityEngine.Object) null && volley.projectiles.Count > 0 && volley.projectiles[0].projectiles != null && volley.projectiles[0].projectiles.Count > 0)
      {
        Projectile projectile = volley.projectiles[0].projectiles[0];
        if ((bool) (UnityEngine.Object) projectile && (bool) (UnityEngine.Object) projectile.GetComponent<ArtfulDodgerProjectileController>())
          flag = false;
      }
      if (flag)
      {
        for (int index = 0; index < synergy.ModulesToAdd.Length; ++index)
        {
          synergy.ModulesToAdd[index].isExternalAddedModule = true;
          volley.projectiles.Add(synergy.ModulesToAdd[index]);
        }
      }
    }
    if (synergy.AddsDuplicatesOfBaseModule)
      GunVolleyModificationItem.AddDuplicateOfBaseModule(volley, this.m_gun.CurrentOwner as PlayerController, synergy.DuplicatesOfBaseModule, synergy.BaseModuleDuplicateAngle, 0.0f);
    if (synergy.SetsNumberFinalProjectiles)
    {
      bool flag = false;
      for (int index = 0; index < volley.projectiles.Count; ++index)
      {
        if (!flag && synergy.AddsNewFinalProjectile && !volley.projectiles[index].usesOptionalFinalProjectile)
        {
          flag = true;
          this.m_gun.OverrideFinaleAudio = true;
          volley.projectiles[index].usesOptionalFinalProjectile = true;
          volley.projectiles[index].numberOfFinalProjectiles = 1;
          volley.projectiles[index].finalProjectile = synergy.NewFinalProjectile;
          volley.projectiles[index].finalAmmoType = GameUIAmmoType.AmmoType.CUSTOM;
          volley.projectiles[index].finalCustomAmmoType = synergy.NewFinalProjectileAmmoType;
          if (string.IsNullOrEmpty(this.m_gun.finalShootAnimation))
            this.m_gun.finalShootAnimation = this.m_gun.shootAnimation;
        }
        if (volley.projectiles[index].usesOptionalFinalProjectile)
          volley.projectiles[index].numberOfFinalProjectiles = synergy.NumberFinalProjectiles;
      }
    }
    if (synergy.SetsBurstCount)
    {
      if (synergy.MakesDefaultModuleBurst && volley.projectiles.Count > 0 && volley.projectiles[0].shootStyle != ProjectileModule.ShootStyle.Burst)
        volley.projectiles[0].shootStyle = ProjectileModule.ShootStyle.Burst;
      for (int index = 0; index < volley.projectiles.Count; ++index)
      {
        if (volley.projectiles[index].shootStyle == ProjectileModule.ShootStyle.Burst)
        {
          int burstShotCount = volley.projectiles[index].burstShotCount;
          int max = volley.projectiles[index].GetModNumberOfShotsInClip((GameActor) owner);
          if (max < 0)
            max = int.MaxValue;
          int num = Mathf.Clamp(Mathf.RoundToInt((float) burstShotCount * synergy.BurstMultiplier) + synergy.BurstShift, 1, max);
          volley.projectiles[index].burstShotCount = num;
        }
      }
    }
    if (!synergy.AddsPossibleProjectileToPrimaryModule)
      return;
    volley.projectiles[0].projectiles.Add(synergy.AdditionalModuleProjectile);
  }
}
