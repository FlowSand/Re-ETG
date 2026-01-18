using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class PlatinumBulletsItem : PassiveItem, ILevelLoadedListener
  {
    public float ShootSecondsPerDamageDouble = 500f;
    public float ShootSecondsPerRateOfFireDouble = 250f;
    public float MaximumDamageMultiplier = 3f;
    public float MaximumRateOfFireMultiplier = 3f;
    [Header("Per-Floor Starting Values")]
    public float CastleStartingValue;
    public float SewersStartingValue;
    public float GungeonStartingValue;
    public float AbbeyStartingValue;
    public float MinesStartingValue;
    public float RatStartingValue;
    public float HollowStartingValue;
    public float ForgeStartingValue;
    public float HellStartingValue;
    private StatModifier DamageStat;
    private StatModifier RateOfFireStat;
    private float m_totalBulletsFiredNormalizedByFireRate;
    private float m_lastProjectileTimeslice = -1f;
    private Shader m_glintShader;

    public override void Pickup(PlayerController player)
    {
      if (!this.m_pickedUpThisRun)
      {
        GlobalDungeonData.ValidTilesets tilesetId = GameManager.Instance.Dungeon.tileIndices.tilesetId;
        switch (tilesetId)
        {
          case GlobalDungeonData.ValidTilesets.GUNGEON:
            this.m_totalBulletsFiredNormalizedByFireRate = this.GungeonStartingValue;
            break;
          case GlobalDungeonData.ValidTilesets.CASTLEGEON:
            this.m_totalBulletsFiredNormalizedByFireRate = this.CastleStartingValue;
            break;
          case GlobalDungeonData.ValidTilesets.SEWERGEON:
            this.m_totalBulletsFiredNormalizedByFireRate = this.SewersStartingValue;
            break;
          case GlobalDungeonData.ValidTilesets.CATHEDRALGEON:
            this.m_totalBulletsFiredNormalizedByFireRate = this.AbbeyStartingValue;
            break;
          default:
            if (tilesetId != GlobalDungeonData.ValidTilesets.MINEGEON)
            {
              if (tilesetId != GlobalDungeonData.ValidTilesets.CATACOMBGEON)
              {
                if (tilesetId != GlobalDungeonData.ValidTilesets.FORGEGEON)
                {
                  if (tilesetId != GlobalDungeonData.ValidTilesets.HELLGEON)
                  {
                    if (tilesetId == GlobalDungeonData.ValidTilesets.RATGEON)
                    {
                      this.m_totalBulletsFiredNormalizedByFireRate = this.RatStartingValue;
                      break;
                    }
                    break;
                  }
                  this.m_totalBulletsFiredNormalizedByFireRate = this.HellStartingValue;
                  break;
                }
                this.m_totalBulletsFiredNormalizedByFireRate = this.ForgeStartingValue;
                break;
              }
              this.m_totalBulletsFiredNormalizedByFireRate = this.HollowStartingValue;
              break;
            }
            this.m_totalBulletsFiredNormalizedByFireRate = this.MinesStartingValue;
            break;
        }
      }
      base.Pickup(player);
      player.PostProcessProjectile += new Action<Projectile, float>(this.HandlePostProcessProjectile);
      player.PostProcessBeam += new Action<BeamController>(this.HandlePostProcessBeam);
      player.PostProcessBeamTick += new Action<BeamController, SpeculativeRigidbody, float>(this.HandlePostProcessBeamTick);
      player.OnKilledEnemyContext += new Action<PlayerController, HealthHaver>(this.HandleEnemyKilled);
      player.GunChanged += new Action<Gun, Gun, bool>(this.HandleGunChanged);
      this.m_glintShader = Shader.Find("Brave/ItemSpecific/LootGlintAdditivePass");
      if (!(bool) (UnityEngine.Object) player.CurrentGun)
        return;
      this.ProcessGunShader(player.CurrentGun);
    }

    private void HandleGunChanged(Gun oldGun, Gun newGun, bool arg3)
    {
      this.RemoveGunShader(oldGun);
      this.ProcessGunShader(newGun);
    }

    private void RemoveGunShader(Gun g)
    {
      if (!(bool) (UnityEngine.Object) g)
        return;
      MeshRenderer component = g.GetComponent<MeshRenderer>();
      if (!(bool) (UnityEngine.Object) component)
        return;
      Material[] sharedMaterials = component.sharedMaterials;
      List<Material> materialList = new List<Material>();
      for (int index = 0; index < sharedMaterials.Length; ++index)
      {
        if ((UnityEngine.Object) sharedMaterials[index].shader != (UnityEngine.Object) this.m_glintShader)
          materialList.Add(sharedMaterials[index]);
      }
      component.sharedMaterials = materialList.ToArray();
    }

    private void ProcessGunShader(Gun g)
    {
      MeshRenderer component = g.GetComponent<MeshRenderer>();
      if (!(bool) (UnityEngine.Object) component)
        return;
      Material[] sharedMaterials = component.sharedMaterials;
      for (int index = 0; index < sharedMaterials.Length; ++index)
      {
        if ((UnityEngine.Object) sharedMaterials[index].shader == (UnityEngine.Object) this.m_glintShader)
          return;
      }
      Array.Resize<Material>(ref sharedMaterials, sharedMaterials.Length + 1);
      Material material = new Material(this.m_glintShader);
      material.SetTexture("_MainTex", sharedMaterials[0].GetTexture("_MainTex"));
      sharedMaterials[sharedMaterials.Length - 1] = material;
      component.sharedMaterials = sharedMaterials;
    }

    private void HandleEnemyKilled(PlayerController sourcePlayer, HealthHaver enemy)
    {
      if (!sourcePlayer.HasActiveBonusSynergy(CustomSynergyType.PLATINUM_AND_GOLD) || !(bool) (UnityEngine.Object) enemy || !(bool) (UnityEngine.Object) enemy.aiActor)
        return;
      LootEngine.SpawnCurrency(enemy.aiActor.CenterPosition, UnityEngine.Random.Range(1, 6));
    }

    public void BraveOnLevelWasLoaded() => this.m_lastProjectileTimeslice = -1f;

    private void UpdateContributions()
    {
      if (!(bool) (UnityEngine.Object) this.Owner)
        return;
      if (this.DamageStat == null)
      {
        this.DamageStat = new StatModifier();
        this.DamageStat.amount = 1f;
        this.DamageStat.modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE;
        this.DamageStat.statToBoost = PlayerStats.StatType.Damage;
        this.Owner.ownerlessStatModifiers.Add(this.DamageStat);
      }
      if (this.RateOfFireStat == null)
      {
        this.RateOfFireStat = new StatModifier();
        this.RateOfFireStat.amount = 1f;
        this.RateOfFireStat.modifyType = StatModifier.ModifyMethod.MULTIPLICATIVE;
        this.RateOfFireStat.statToBoost = PlayerStats.StatType.RateOfFire;
        this.Owner.ownerlessStatModifiers.Add(this.RateOfFireStat);
      }
      this.DamageStat.amount = Mathf.Min(this.MaximumDamageMultiplier, (float) (1.0 + (double) this.m_totalBulletsFiredNormalizedByFireRate / (double) this.ShootSecondsPerDamageDouble));
      this.RateOfFireStat.amount = Mathf.Min(this.MaximumRateOfFireMultiplier, (float) (1.0 + (double) this.m_totalBulletsFiredNormalizedByFireRate / (double) this.ShootSecondsPerRateOfFireDouble));
    }

    private void HandlePostProcessProjectile(Projectile targetProjectile, float effectChanceScalar)
    {
      targetProjectile.OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy);
    }

    private void HandleHitEnemy(
      Projectile sourceProjectile,
      SpeculativeRigidbody hitRigidbody,
      bool fatal)
    {
      if (!(bool) (UnityEngine.Object) sourceProjectile.PossibleSourceGun || (double) sourceProjectile.PlayerProjectileSourceGameTimeslice <= (double) this.m_lastProjectileTimeslice)
        return;
      this.m_lastProjectileTimeslice = sourceProjectile.PlayerProjectileSourceGameTimeslice;
      float num = 1f / sourceProjectile.PossibleSourceGun.DefaultModule.cooldownTime;
      this.m_totalBulletsFiredNormalizedByFireRate += (double) num <= 0.0 ? 1f : 1f / num;
      this.UpdateContributions();
    }

    private void HandlePostProcessBeam(BeamController targetBeam) => this.UpdateContributions();

    private void HandlePostProcessBeamTick(
      BeamController arg1,
      SpeculativeRigidbody arg2,
      float arg3)
    {
      this.m_totalBulletsFiredNormalizedByFireRate += BraveTime.DeltaTime;
    }

    public override DebrisObject Drop(PlayerController player)
    {
      if ((bool) (UnityEngine.Object) player)
      {
        if ((bool) (UnityEngine.Object) player.CurrentGun)
          this.RemoveGunShader(player.CurrentGun);
        player.PostProcessProjectile -= new Action<Projectile, float>(this.HandlePostProcessProjectile);
        player.PostProcessBeam -= new Action<BeamController>(this.HandlePostProcessBeam);
        player.PostProcessBeamTick -= new Action<BeamController, SpeculativeRigidbody, float>(this.HandlePostProcessBeamTick);
        player.GunChanged -= new Action<Gun, Gun, bool>(this.HandleGunChanged);
        player.ownerlessStatModifiers.Remove(this.DamageStat);
        player.ownerlessStatModifiers.Remove(this.RateOfFireStat);
        this.DamageStat = (StatModifier) null;
        this.RateOfFireStat = (StatModifier) null;
      }
      return base.Drop(player);
    }

    protected override void OnDestroy()
    {
      if ((bool) (UnityEngine.Object) this.Owner)
      {
        if ((bool) (UnityEngine.Object) this.Owner.CurrentGun)
          this.RemoveGunShader(this.Owner.CurrentGun);
        this.Owner.PostProcessProjectile -= new Action<Projectile, float>(this.HandlePostProcessProjectile);
        this.Owner.PostProcessBeam -= new Action<BeamController>(this.HandlePostProcessBeam);
        this.Owner.PostProcessBeamTick -= new Action<BeamController, SpeculativeRigidbody, float>(this.HandlePostProcessBeamTick);
        this.Owner.GunChanged -= new Action<Gun, Gun, bool>(this.HandleGunChanged);
        this.Owner.ownerlessStatModifiers.Remove(this.DamageStat);
        this.Owner.ownerlessStatModifiers.Remove(this.RateOfFireStat);
        this.DamageStat = (StatModifier) null;
        this.RateOfFireStat = (StatModifier) null;
      }
      base.OnDestroy();
    }
  }

