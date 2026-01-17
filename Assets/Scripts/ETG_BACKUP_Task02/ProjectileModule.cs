// Decompiled with JetBrains decompiler
// Type: ProjectileModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
[Serializable]
public class ProjectileModule
{
  public ProjectileModule.ShootStyle shootStyle;
  public GameUIAmmoType.AmmoType ammoType;
  public string customAmmoType;
  public List<Projectile> projectiles = new List<Projectile>();
  public ProjectileModule.ProjectileSequenceStyle sequenceStyle;
  public List<int> orderedGroupCounts;
  public List<ProjectileModule.ChargeProjectile> chargeProjectiles = new List<ProjectileModule.ChargeProjectile>();
  public float maxChargeTime;
  public bool triggerCooldownForAnyChargeAmount;
  public bool isFinalVolley;
  public bool usesOptionalFinalProjectile;
  public Projectile finalProjectile;
  public ProjectileVolleyData finalVolley;
  public int numberOfFinalProjectiles = 1;
  public GameUIAmmoType.AmmoType finalAmmoType;
  public string finalCustomAmmoType;
  public float angleFromAim;
  public bool alternateAngle;
  public float angleVariance;
  public Vector3 positionOffset;
  public bool mirror;
  public bool inverted;
  public int ammoCost = 1;
  public int burstShotCount = 3;
  public float burstCooldownTime = 0.2f;
  public float cooldownTime = 1f;
  public int numberOfShotsInClip = -1;
  public bool ignoredForReloadPurposes;
  public bool preventFiringDuringCharge;
  [NonSerialized]
  public bool isExternalAddedModule;
  private int m_cloneSourceIndex = -1;
  [NonSerialized]
  public string runtimeGuid;
  [NonSerialized]
  public bool IsDuctTapeModule;
  private int currentOrderedProjNumber;
  private int currentOrderedGroupNumber;
  private static int m_angleVarianceIterator;

  public int GetModifiedNumberOfFinalProjectiles(GameActor owner)
  {
    return (bool) (UnityEngine.Object) owner && owner is PlayerController && this.numberOfFinalProjectiles > 0 && (owner as PlayerController).OnlyFinalProjectiles.Value ? this.GetModNumberOfShotsInClip(owner) : this.numberOfFinalProjectiles;
  }

  public int CloneSourceIndex
  {
    get => this.m_cloneSourceIndex;
    set => this.m_cloneSourceIndex = value;
  }

  public int GetModNumberOfShotsInClip(GameActor owner)
  {
    if (this.numberOfShotsInClip == 1 || !((UnityEngine.Object) owner != (UnityEngine.Object) null) || !(owner is PlayerController))
      return this.numberOfShotsInClip;
    PlayerController playerController = owner as PlayerController;
    int a = Mathf.FloorToInt((float) this.numberOfShotsInClip * playerController.stats.GetStatValue(PlayerStats.StatType.AdditionalClipCapacityMultiplier) * playerController.stats.GetStatValue(PlayerStats.StatType.TarnisherClipCapacityMultiplier));
    return a < 0 ? a : Mathf.Max(a, 1);
  }

  public static ProjectileModule CreateClone(
    ProjectileModule source,
    bool inheritGuid = true,
    int sourceIndex = -1)
  {
    ProjectileModule clone = new ProjectileModule();
    clone.shootStyle = source.shootStyle;
    clone.ammoType = source.ammoType;
    clone.customAmmoType = source.customAmmoType;
    clone.sequenceStyle = source.sequenceStyle;
    clone.maxChargeTime = source.maxChargeTime;
    clone.triggerCooldownForAnyChargeAmount = source.triggerCooldownForAnyChargeAmount;
    clone.angleFromAim = source.angleFromAim;
    clone.alternateAngle = source.alternateAngle;
    clone.angleVariance = source.angleVariance;
    clone.mirror = source.mirror;
    clone.inverted = source.inverted;
    clone.positionOffset = source.positionOffset;
    clone.ammoCost = source.ammoCost;
    clone.cooldownTime = source.cooldownTime;
    clone.numberOfShotsInClip = source.numberOfShotsInClip;
    clone.usesOptionalFinalProjectile = source.usesOptionalFinalProjectile;
    clone.finalAmmoType = source.finalAmmoType;
    clone.finalCustomAmmoType = source.finalCustomAmmoType;
    clone.numberOfFinalProjectiles = source.numberOfFinalProjectiles;
    clone.isFinalVolley = source.isFinalVolley;
    clone.burstCooldownTime = source.burstCooldownTime;
    clone.burstShotCount = source.burstShotCount;
    clone.ignoredForReloadPurposes = source.ignoredForReloadPurposes;
    clone.preventFiringDuringCharge = source.preventFiringDuringCharge;
    clone.isExternalAddedModule = source.isExternalAddedModule;
    clone.IsDuctTapeModule = source.IsDuctTapeModule;
    clone.projectiles = new List<Projectile>();
    for (int index = 0; index < source.projectiles.Count; ++index)
      clone.projectiles.Add(source.projectiles[index]);
    clone.chargeProjectiles = source.chargeProjectiles;
    clone.finalProjectile = source.finalProjectile;
    clone.finalVolley = source.finalVolley;
    clone.orderedGroupCounts = source.orderedGroupCounts;
    if (sourceIndex >= 0)
      clone.CloneSourceIndex = sourceIndex;
    clone.runtimeGuid = !inheritGuid || source.runtimeGuid == null ? Guid.NewGuid().ToString() : source.runtimeGuid;
    return clone;
  }

  public void ClearOrderedProjectileData()
  {
    this.currentOrderedGroupNumber = 0;
    this.currentOrderedProjNumber = 0;
  }

  public void ResetRuntimeData()
  {
    this.currentOrderedProjNumber = 0;
    this.currentOrderedGroupNumber = 0;
    if (!string.IsNullOrEmpty(this.runtimeGuid))
      return;
    this.runtimeGuid = Guid.NewGuid().ToString();
  }

  public bool IsFinalShot(ModuleShootData runtimeData, GameActor owner)
  {
    if (runtimeData.needsReload)
      return false;
    if (this.isFinalVolley)
      return true;
    return this.usesOptionalFinalProjectile && this.GetModNumberOfShotsInClip(owner) - this.GetModifiedNumberOfFinalProjectiles(owner) <= runtimeData.numberShotsFired;
  }

  public bool HasFinalVolleyOverride()
  {
    return this.usesOptionalFinalProjectile && (UnityEngine.Object) this.finalVolley != (UnityEngine.Object) null;
  }

  public Projectile GetCurrentProjectile(ModuleShootData runtimeData, GameActor owner)
  {
    if (this.usesOptionalFinalProjectile && this.GetModNumberOfShotsInClip(owner) - this.GetModifiedNumberOfFinalProjectiles(owner) <= runtimeData.numberShotsFired)
      return this.finalProjectile;
    if (this.sequenceStyle == ProjectileModule.ProjectileSequenceStyle.Ordered)
      return this.projectiles[this.currentOrderedProjNumber];
    if (this.sequenceStyle != ProjectileModule.ProjectileSequenceStyle.OrderedGroups)
      return this.projectiles[UnityEngine.Random.Range(0, this.projectiles.Count)];
    int min = 0;
    for (int index = 0; index < this.currentOrderedGroupNumber; ++index)
      min += this.orderedGroupCounts[index];
    int index1 = UnityEngine.Random.Range(min, min + this.orderedGroupCounts[this.currentOrderedGroupNumber]);
    this.currentOrderedGroupNumber = (this.currentOrderedGroupNumber + 1) % this.orderedGroupCounts.Count;
    return this.projectiles[index1];
  }

  public Projectile GetCurrentProjectile()
  {
    if (this.shootStyle == ProjectileModule.ShootStyle.Charged)
    {
      for (int index = 0; index < this.chargeProjectiles.Count; ++index)
      {
        if ((bool) (UnityEngine.Object) this.chargeProjectiles[index].Projectile)
        {
          Projectile projectile = this.chargeProjectiles[index].Projectile;
          projectile.pierceMinorBreakables = true;
          return projectile;
        }
      }
      return (Projectile) null;
    }
    if (this.sequenceStyle == ProjectileModule.ProjectileSequenceStyle.Ordered)
      return this.projectiles[this.currentOrderedProjNumber];
    if (this.sequenceStyle != ProjectileModule.ProjectileSequenceStyle.OrderedGroups)
      return this.projectiles[UnityEngine.Random.Range(0, this.projectiles.Count)];
    int min = 0;
    for (int index = 0; index < this.currentOrderedGroupNumber; ++index)
      min += this.orderedGroupCounts[index];
    int index1 = UnityEngine.Random.Range(min, this.orderedGroupCounts[this.currentOrderedGroupNumber]);
    this.currentOrderedGroupNumber = (this.currentOrderedGroupNumber + 1) % this.orderedGroupCounts.Count;
    return this.projectiles[index1];
  }

  public Vector3 InversePositionOffset
  {
    get => new Vector3(this.positionOffset.x, -1f * this.positionOffset.y, this.positionOffset.z);
  }

  public float GetEstimatedShotsPerSecond(float reloadTime)
  {
    if ((double) this.cooldownTime <= 0.0)
      return 0.0f;
    float num = this.cooldownTime;
    if (this.shootStyle == ProjectileModule.ShootStyle.Burst && this.burstShotCount > 1 && (double) this.burstCooldownTime > 0.0)
      num = ((float) (this.burstShotCount - 1) * this.burstCooldownTime + this.cooldownTime) / (float) this.burstShotCount;
    if (this.numberOfShotsInClip > 0)
      num += reloadTime / (float) this.numberOfShotsInClip;
    return 1f / num;
  }

  public void IncrementShootCount()
  {
    this.currentOrderedProjNumber = (this.currentOrderedProjNumber + 1) % this.projectiles.Count;
  }

  public float GetAngleVariance(float varianceMultiplier = 1f)
  {
    float num = BraveMathCollege.GetLowDiscrepancyRandom(ProjectileModule.m_angleVarianceIterator) * (2f * this.angleVariance) - this.angleVariance;
    ++ProjectileModule.m_angleVarianceIterator;
    return num * varianceMultiplier;
  }

  public float GetAngleVariance(float customVariance, float varianceMultiplier)
  {
    float num = BraveMathCollege.GetLowDiscrepancyRandom(ProjectileModule.m_angleVarianceIterator) * (2f * customVariance) - customVariance;
    ++ProjectileModule.m_angleVarianceIterator;
    return num * varianceMultiplier;
  }

  public float GetAngleForShot(
    float alternateAngleSign = 1f,
    float varianceMultiplier = 1f,
    float? overrideAngleVariance = null)
  {
    return alternateAngleSign * this.angleFromAim + (!overrideAngleVariance.HasValue ? this.GetAngleVariance(varianceMultiplier) : overrideAngleVariance.Value);
  }

  public int ContainsFinalProjectile(Projectile testProj)
  {
    if (this.usesOptionalFinalProjectile)
    {
      if ((UnityEngine.Object) this.finalVolley != (UnityEngine.Object) null)
      {
        for (int index = 0; index < this.finalVolley.projectiles.Count; ++index)
        {
          if (this.finalVolley.projectiles[index].projectiles.Contains(testProj))
            return this.numberOfFinalProjectiles;
        }
      }
      else if ((UnityEngine.Object) this.finalProjectile == (UnityEngine.Object) testProj)
        return this.numberOfFinalProjectiles;
    }
    return 0;
  }

  public float LongestChargeTime
  {
    get
    {
      float a = 0.0f;
      for (int index = 0; index < this.chargeProjectiles.Count; ++index)
      {
        ProjectileModule.ChargeProjectile chargeProjectile = this.chargeProjectiles[index];
        a = Mathf.Max(a, chargeProjectile.ChargeTime);
      }
      return a;
    }
  }

  public ProjectileModule.ChargeProjectile GetChargeProjectile(float chargeTime)
  {
    ProjectileModule.ChargeProjectile chargeProjectile1 = (ProjectileModule.ChargeProjectile) null;
    for (int index = 0; index < this.chargeProjectiles.Count; ++index)
    {
      ProjectileModule.ChargeProjectile chargeProjectile2 = this.chargeProjectiles[index];
      if ((double) chargeProjectile2.ChargeTime <= (double) chargeTime && (chargeProjectile1 == null || (double) chargeTime - (double) chargeProjectile2.ChargeTime < (double) chargeTime - (double) chargeProjectile1.ChargeTime))
        chargeProjectile1 = chargeProjectile2;
    }
    return chargeProjectile1;
  }

  public enum ShootStyle
  {
    SemiAutomatic,
    Automatic,
    Beam,
    Charged,
    Burst,
  }

  public enum ProjectileSequenceStyle
  {
    Random,
    Ordered,
    OrderedGroups,
  }

  [Serializable]
  public class ChargeProjectile
  {
    public float ChargeTime;
    public Projectile Projectile;
    public ProjectileModule.ChargeProjectileProperties UsedProperties;
    public int AmmoCost;
    public VFXPool VfxPool;
    public float LightIntensity;
    public ScreenShakeSettings ScreenShake;
    public string OverrideShootAnimation;
    public VFXPool OverrideMuzzleFlashVfxPool;
    public bool MegaReflection;
    public string AdditionalWwiseEvent;
    [NonSerialized]
    public ProjectileModule.ChargeProjectile previousChargeProjectile;

    public bool UsesOverrideShootAnimation
    {
      get
      {
        return (this.UsedProperties & ProjectileModule.ChargeProjectileProperties.shootAnim) == ProjectileModule.ChargeProjectileProperties.shootAnim;
      }
    }

    public bool UsesOverrideMuzzleFlashVfxPool
    {
      get
      {
        return (this.UsedProperties & ProjectileModule.ChargeProjectileProperties.muzzleFlash) == ProjectileModule.ChargeProjectileProperties.muzzleFlash;
      }
    }

    public bool DepleteAmmo
    {
      get
      {
        return (this.UsedProperties & ProjectileModule.ChargeProjectileProperties.depleteAmmo) == ProjectileModule.ChargeProjectileProperties.depleteAmmo;
      }
    }

    public bool UsesAmmo
    {
      get
      {
        return (this.UsedProperties & ProjectileModule.ChargeProjectileProperties.ammo) == ProjectileModule.ChargeProjectileProperties.ammo;
      }
    }

    public bool UsesVfx
    {
      get
      {
        return (this.UsedProperties & ProjectileModule.ChargeProjectileProperties.vfx) == ProjectileModule.ChargeProjectileProperties.vfx;
      }
    }

    public bool UsesLightIntensity
    {
      get
      {
        return (this.UsedProperties & ProjectileModule.ChargeProjectileProperties.lightIntensity) == ProjectileModule.ChargeProjectileProperties.lightIntensity;
      }
    }

    public bool UsesScreenShake
    {
      get
      {
        return (this.UsedProperties & ProjectileModule.ChargeProjectileProperties.screenShake) == ProjectileModule.ChargeProjectileProperties.screenShake;
      }
    }

    public bool ReflectsIncomingBullets
    {
      get
      {
        return (this.UsedProperties & ProjectileModule.ChargeProjectileProperties.reflectBullets) == ProjectileModule.ChargeProjectileProperties.reflectBullets;
      }
    }

    public bool DelayedVFXDestruction
    {
      get
      {
        return (this.UsedProperties & ProjectileModule.ChargeProjectileProperties.delayedVFXClear) == ProjectileModule.ChargeProjectileProperties.delayedVFXClear;
      }
    }

    public bool ShouldDoChargePoof
    {
      get
      {
        return (bool) (UnityEngine.Object) this.Projectile && (double) this.ChargeTime > 0.0 && (this.UsedProperties & ProjectileModule.ChargeProjectileProperties.disableChargePoof) != ProjectileModule.ChargeProjectileProperties.disableChargePoof;
      }
    }

    public bool UsesAdditionalWwiseEvent
    {
      get
      {
        return (this.UsedProperties & ProjectileModule.ChargeProjectileProperties.additionalWwiseEvent) == ProjectileModule.ChargeProjectileProperties.additionalWwiseEvent;
      }
    }
  }

  [Flags]
  public enum ChargeProjectileProperties
  {
    ammo = 1,
    vfx = 2,
    lightIntensity = 4,
    screenShake = 8,
    shootAnim = 16, // 0x00000010
    muzzleFlash = 32, // 0x00000020
    depleteAmmo = 64, // 0x00000040
    delayedVFXClear = 128, // 0x00000080
    disableChargePoof = 256, // 0x00000100
    reflectBullets = 512, // 0x00000200
    additionalWwiseEvent = 1024, // 0x00000400
  }
}
