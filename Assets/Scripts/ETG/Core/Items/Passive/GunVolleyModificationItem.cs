// Decompiled with JetBrains decompiler
// Type: GunVolleyModificationItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class GunVolleyModificationItem : PassiveItem
  {
    public bool AddsModule;
    [ShowInInspectorIf("AddsModule", false)]
    public ProjectileModule ModuleToAdd;
    public int DuplicatesOfBaseModule;
    public float DuplicateAngleOffset = 10f;
    public float DuplicateAngleBaseOffset;
    public int DuplicatesOfEachModule;
    public float EachModuleOffsetAngle;
    public float EachSingleModuleMinAngleVariance = 15f;
    [Header("For Helix Bullets")]
    public bool AddsHelixModifier;
    [Header("For Orbit Bullets")]
    public bool AddsOrbitModifier;
    private bool UpOrDown;
    private const int MAX_ORBIT_PROJECTILES = 20;
    private const float ORBIT_LIFESPAN = 15f;

    public void ModifyVolley(ProjectileVolleyData volleyToModify)
    {
      if (this.AddsModule)
      {
        bool flag = true;
        if ((UnityEngine.Object) volleyToModify != (UnityEngine.Object) null && volleyToModify.projectiles.Count > 0 && volleyToModify.projectiles[0].projectiles != null && volleyToModify.projectiles[0].projectiles.Count > 0)
        {
          Projectile projectile = volleyToModify.projectiles[0].projectiles[0];
          if ((bool) (UnityEngine.Object) projectile && (bool) (UnityEngine.Object) projectile.GetComponent<ArtfulDodgerProjectileController>())
            flag = false;
        }
        if (flag)
        {
          this.ModuleToAdd.isExternalAddedModule = true;
          volleyToModify.projectiles.Add(this.ModuleToAdd);
        }
      }
      if (this.DuplicatesOfEachModule > 0)
      {
        int count = volleyToModify.projectiles.Count;
        for (int index1 = 0; index1 < count; ++index1)
        {
          ProjectileModule projectile = volleyToModify.projectiles[index1];
          for (int index2 = 0; index2 < this.DuplicatesOfEachModule; ++index2)
          {
            int sourceIndex = index1;
            if (projectile.CloneSourceIndex >= 0)
              sourceIndex = projectile.CloneSourceIndex;
            ProjectileModule clone = ProjectileModule.CreateClone(projectile, false, sourceIndex);
            if (clone.projectiles != null && clone.projectiles.Count > 0 && clone.projectiles[0] is InputGuidedProjectile)
              clone.positionOffset = (Vector3) UnityEngine.Random.insideUnitCircle.normalized;
            clone.angleVariance = Mathf.Max(clone.angleVariance * 2f, this.EachSingleModuleMinAngleVariance);
            clone.ignoredForReloadPurposes = true;
            clone.angleFromAim = projectile.angleFromAim + this.EachModuleOffsetAngle;
            clone.ammoCost = 0;
            volleyToModify.projectiles.Add(clone);
          }
          projectile.angleVariance = Mathf.Max(projectile.angleVariance, 5f);
        }
        if (!volleyToModify.UsesShotgunStyleVelocityRandomizer)
        {
          volleyToModify.UsesShotgunStyleVelocityRandomizer = true;
          volleyToModify.DecreaseFinalSpeedPercentMin = -10f;
          volleyToModify.IncreaseFinalSpeedPercentMax = 10f;
        }
      }
      if (this.DuplicatesOfBaseModule <= 0)
        return;
      GunVolleyModificationItem.AddDuplicateOfBaseModule(volleyToModify, this.Owner, this.DuplicatesOfBaseModule, this.DuplicateAngleOffset, this.DuplicateAngleBaseOffset);
    }

    public static void AddDuplicateOfBaseModule(
      ProjectileVolleyData volleyToModify,
      PlayerController Owner,
      int DuplicatesOfBaseModule,
      float DuplicateAngleOffset,
      float DuplicateAngleBaseOffset)
    {
      ProjectileModule projectile = volleyToModify.projectiles[0];
      int num1 = 0;
      if (volleyToModify.ModulesAreTiers && (bool) (UnityEngine.Object) Owner && (bool) (UnityEngine.Object) Owner.CurrentGun && Owner.CurrentGun.CurrentStrengthTier >= 0 && Owner.CurrentGun.CurrentStrengthTier < volleyToModify.projectiles.Count)
      {
        projectile = volleyToModify.projectiles[Owner.CurrentGun.CurrentStrengthTier];
        num1 = Owner.CurrentGun.CurrentStrengthTier;
      }
      if (projectile.mirror)
        return;
      float num2 = (float) ((double) DuplicatesOfBaseModule * (double) DuplicateAngleOffset * -1.0 / 2.0) + DuplicateAngleBaseOffset;
      projectile.angleFromAim = num2;
      for (int index = 0; index < DuplicatesOfBaseModule; ++index)
      {
        int sourceIndex = num1;
        if (projectile.CloneSourceIndex >= 0)
          sourceIndex = projectile.CloneSourceIndex;
        ProjectileModule clone = ProjectileModule.CreateClone(projectile, false, sourceIndex);
        float num3 = num2 + DuplicateAngleOffset * (float) (index + 1);
        clone.angleFromAim = num3;
        clone.ignoredForReloadPurposes = true;
        clone.ammoCost = 0;
        volleyToModify.projectiles.Add(clone);
      }
    }

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      base.Pickup(player);
      if (this.AddsHelixModifier)
      {
        player.PostProcessProjectile += new Action<Projectile, float>(this.PostProcessProjectileHelix);
        player.PostProcessBeam += new Action<BeamController>(this.PostProcessProjectileHelixBeam);
      }
      if (!this.AddsOrbitModifier)
        return;
      player.PostProcessProjectile += new Action<Projectile, float>(this.PostProcessProjectileOrbit);
      player.PostProcessBeam += new Action<BeamController>(this.PostProcessProjectileOrbitBeam);
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      debrisObject.GetComponent<GunVolleyModificationItem>().m_pickedUpThisRun = true;
      if (this.AddsHelixModifier)
      {
        player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectileHelix);
        player.PostProcessBeam -= new Action<BeamController>(this.PostProcessProjectileHelixBeam);
      }
      if (this.AddsOrbitModifier)
      {
        player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectileOrbit);
        player.PostProcessBeam -= new Action<BeamController>(this.PostProcessProjectileOrbitBeam);
      }
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      if (!(bool) (UnityEngine.Object) this.m_owner)
        return;
      if (this.AddsHelixModifier)
      {
        this.m_owner.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectileHelix);
        this.m_owner.PostProcessBeam -= new Action<BeamController>(this.PostProcessProjectileHelixBeam);
      }
      if (!this.AddsOrbitModifier)
        return;
      this.m_owner.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectileOrbit);
      this.m_owner.PostProcessBeam -= new Action<BeamController>(this.PostProcessProjectileOrbitBeam);
    }

    private void PostProcessProjectileHelix(Projectile obj, float effectChanceScalar)
    {
      switch (obj)
      {
        case InstantDamageOneEnemyProjectile _:
          break;
        case InstantlyDamageAllProjectile _:
          break;
        case HelixProjectile _:
          if (!(bool) (UnityEngine.Object) this.Owner || !this.Owner.HasActiveBonusSynergy(CustomSynergyType.DOUBLE_DOUBLE_HELIX))
            break;
          HelixProjectile helixProjectile = obj as HelixProjectile;
          helixProjectile.helixAmplitude *= 0.5f;
          helixProjectile.helixWavelength *= 0.75f;
          break;
        default:
          if (obj.OverrideMotionModule != null && obj.OverrideMotionModule is OrbitProjectileMotionModule)
          {
            OrbitProjectileMotionModule overrideMotionModule = obj.OverrideMotionModule as OrbitProjectileMotionModule;
            overrideMotionModule.StackHelix = true;
            overrideMotionModule.ForceInvert = !this.UpOrDown;
          }
          else if (this.UpOrDown)
            obj.OverrideMotionModule = (ProjectileMotionModule) new HelixProjectileMotionModule();
          else
            obj.OverrideMotionModule = (ProjectileMotionModule) new HelixProjectileMotionModule()
            {
              ForceInvert = true
            };
          this.UpOrDown = !this.UpOrDown;
          break;
      }
    }

    private void PostProcessProjectileHelixBeam(BeamController beam)
    {
      if (beam.Owner is AIActor)
        return;
      if (beam.projectile.OverrideMotionModule != null && beam.projectile.OverrideMotionModule is OrbitProjectileMotionModule)
      {
        OrbitProjectileMotionModule overrideMotionModule = beam.projectile.OverrideMotionModule as OrbitProjectileMotionModule;
        overrideMotionModule.StackHelix = true;
        overrideMotionModule.ForceInvert = !this.UpOrDown;
      }
      else if (this.UpOrDown)
        beam.projectile.OverrideMotionModule = (ProjectileMotionModule) new HelixProjectileMotionModule();
      else
        beam.projectile.OverrideMotionModule = (ProjectileMotionModule) new HelixProjectileMotionModule()
        {
          ForceInvert = true
        };
      this.UpOrDown = !this.UpOrDown;
    }

    private void PostProcessProjectileOrbit(Projectile obj, float effectChanceScalar)
    {
      if (obj is InstantDamageOneEnemyProjectile || obj is InstantlyDamageAllProjectile || (bool) (UnityEngine.Object) obj.GetComponent<ArtfulDodgerProjectileController>())
        return;
      BounceProjModifier orAddComponent = obj.gameObject.GetOrAddComponent<BounceProjModifier>();
      orAddComponent.numberOfBounces = Mathf.Max(orAddComponent.numberOfBounces, 1);
      orAddComponent.onlyBounceOffTiles = true;
      orAddComponent.OnBounceContext += new Action<BounceProjModifier, SpeculativeRigidbody>(this.HandleStartOrbit);
    }

    private void PostProcessProjectileOrbitBeam(BeamController beam)
    {
      if (beam.Owner is AIActor)
        return;
      float num = 2.75f + (float) OrbitProjectileMotionModule.ActiveBeams;
      if ((double) beam.projectile.baseData.range > 0.0)
        beam.projectile.baseData.range += 6.28318548f * num;
      if ((double) beam.projectile.baseData.speed > 0.0)
        beam.projectile.baseData.speed = Mathf.Max(beam.projectile.baseData.speed, 75f);
      if (beam is BasicBeamController)
      {
        (beam as BasicBeamController).PenetratesCover = true;
        (beam as BasicBeamController).penetration += 10;
      }
      OrbitProjectileMotionModule projectileMotionModule = new OrbitProjectileMotionModule();
      projectileMotionModule.BeamOrbitRadius = num;
      projectileMotionModule.RegisterAsBeam(beam);
      if (beam.projectile.OverrideMotionModule != null && beam.projectile.OverrideMotionModule is HelixProjectileMotionModule)
      {
        projectileMotionModule.StackHelix = true;
        projectileMotionModule.ForceInvert = (beam.projectile.OverrideMotionModule as HelixProjectileMotionModule).ForceInvert;
      }
      beam.projectile.OverrideMotionModule = (ProjectileMotionModule) projectileMotionModule;
    }

    private void HandleStartOrbit(BounceProjModifier bouncer, SpeculativeRigidbody srb)
    {
      if (OrbitProjectileMotionModule.GetOrbitersInGroup(-1) >= 20)
        return;
      bouncer.projectile.specRigidbody.CollideWithTileMap = false;
      bouncer.projectile.ResetDistance();
      bouncer.projectile.baseData.range = Mathf.Max(bouncer.projectile.baseData.range, 500f);
      if ((double) bouncer.projectile.baseData.speed > 50.0)
      {
        bouncer.projectile.baseData.speed = 20f;
        bouncer.projectile.UpdateSpeed();
      }
      OrbitProjectileMotionModule projectileMotionModule = new OrbitProjectileMotionModule();
      projectileMotionModule.lifespan = 15f;
      if (bouncer.projectile.OverrideMotionModule != null && bouncer.projectile.OverrideMotionModule is HelixProjectileMotionModule)
      {
        projectileMotionModule.StackHelix = true;
        projectileMotionModule.ForceInvert = (bouncer.projectile.OverrideMotionModule as HelixProjectileMotionModule).ForceInvert;
      }
      bouncer.projectile.OverrideMotionModule = (ProjectileMotionModule) projectileMotionModule;
    }
  }

