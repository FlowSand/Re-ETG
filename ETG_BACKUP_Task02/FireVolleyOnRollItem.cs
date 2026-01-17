// Decompiled with JetBrains decompiler
// Type: FireVolleyOnRollItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable
public class FireVolleyOnRollItem : PassiveItem
{
  public float FireCooldown = 2f;
  public ProjectileVolleyData Volley;
  public string AudioEvent;
  public bool MetroidCrawlUsesCustomExplosion;
  public ExplosionData MetroidCrawlSynergyExplosion;
  private ProjectileVolleyData m_modVolley;
  private float m_cooldown;
  private PlayerController m_player;

  public ProjectileVolleyData ModVolley
  {
    get => (UnityEngine.Object) this.m_modVolley == (UnityEngine.Object) null ? this.Volley : this.m_modVolley;
    set => this.m_modVolley = value;
  }

  public override void Pickup(PlayerController player)
  {
    if (this.m_pickedUp)
      return;
    base.Pickup(player);
    player.OnRollStarted += new Action<PlayerController, Vector2>(this.OnRollStarted);
  }

  private void OnRollStarted(PlayerController obj, Vector2 dirVec)
  {
    if ((double) this.m_cooldown > 0.0 || GameManager.HasInstance && GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
      return;
    this.FireVolley(obj, dirVec);
    this.m_cooldown = this.FireCooldown;
    if (string.IsNullOrEmpty(this.AudioEvent))
      return;
    int num = (int) AkSoundEngine.PostEvent(this.AudioEvent, this.gameObject);
  }

  protected override void Update()
  {
    base.Update();
    this.m_cooldown -= BraveTime.DeltaTime;
  }

  public override DebrisObject Drop(PlayerController player)
  {
    DebrisObject debrisObject = base.Drop(player);
    debrisObject.GetComponent<FireVolleyOnRollItem>().m_pickedUpThisRun = true;
    player.OnRollStarted -= new Action<PlayerController, Vector2>(this.OnRollStarted);
    return debrisObject;
  }

  protected override void OnDestroy()
  {
    if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null)
      this.m_owner.OnRollStarted -= new Action<PlayerController, Vector2>(this.OnRollStarted);
    base.OnDestroy();
  }

  private void FireVolley(PlayerController p, Vector2 dirVec)
  {
    for (int index = 0; index < this.ModVolley.projectiles.Count; ++index)
    {
      ProjectileModule projectile = this.ModVolley.projectiles[index];
      this.ShootSingleProjectile((Vector3) p.CenterPosition, BraveMathCollege.Atan2Degrees(dirVec * -1f), projectile, 100f);
    }
  }

  private void ShootSingleProjectile(
    Vector3 offset,
    float fireAngle,
    ProjectileModule mod,
    float chargeTime)
  {
    PlayerController owner = this.m_owner;
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
    if (!(bool) (UnityEngine.Object) projectile)
    {
      if (mod.shootStyle == ProjectileModule.ShootStyle.Charged)
        return;
      mod.IncrementShootCount();
    }
    else
    {
      if (GameManager.Instance.InTutorial && (UnityEngine.Object) owner != (UnityEngine.Object) null)
        GameManager.BroadcastRoomTalkDoerFsmEvent("playerFiredGun");
      offset = new Vector3(offset.x, offset.y, -1f);
      float angleForShot = mod.GetAngleForShot();
      Projectile component1 = SpawnManager.SpawnProjectile(projectile.gameObject, offset + Quaternion.Euler(0.0f, 0.0f, fireAngle) * mod.positionOffset, Quaternion.Euler(0.0f, 0.0f, fireAngle + angleForShot)).GetComponent<Projectile>();
      component1.Owner = (GameActor) this.m_owner;
      component1.Shooter = this.m_owner.specRigidbody;
      component1.Inverted = mod.inverted;
      if ((bool) (UnityEngine.Object) this.m_owner.aiShooter)
        component1.collidesWithEnemies = this.m_owner.aiShooter.CanShootOtherEnemies;
      if (this.m_owner != null)
      {
        PlayerStats stats = owner.stats;
        component1.baseData.damage *= stats.GetStatValue(PlayerStats.StatType.Damage);
        component1.baseData.speed *= stats.GetStatValue(PlayerStats.StatType.ProjectileSpeed);
        component1.baseData.force *= stats.GetStatValue(PlayerStats.StatType.KnockbackMultiplier);
        component1.baseData.range *= stats.GetStatValue(PlayerStats.StatType.RangeMultiplier);
      }
      if ((bool) (UnityEngine.Object) this.m_owner && this.m_owner.HasActiveBonusSynergy(CustomSynergyType.METROID_CAN_CRAWL) && this.itemName == "Roll Bombs")
      {
        ExplosiveModifier component2 = component1.GetComponent<ExplosiveModifier>();
        if ((bool) (UnityEngine.Object) component2)
        {
          if (this.MetroidCrawlUsesCustomExplosion)
          {
            component2.explosionData = this.MetroidCrawlSynergyExplosion;
          }
          else
          {
            component2.explosionData.damage = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData.damage;
            component2.explosionData.damageRadius = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData.damageRadius;
            component2.explosionData.damageToPlayer = 0.0f;
            component2.explosionData.effect = GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultExplosionData.effect;
          }
        }
      }
      if ((UnityEngine.Object) this.Volley != (UnityEngine.Object) null && this.Volley.UsesShotgunStyleVelocityRandomizer)
        component1.baseData.speed *= this.Volley.GetVolleySpeedMod();
      component1.PlayerProjectileSourceGameTimeslice = UnityEngine.Time.time;
      if (this.m_owner != null)
        owner.DoPostProcessProjectile(component1);
      if (!mod.mirror)
        return;
      Projectile component3 = SpawnManager.SpawnProjectile(projectile.gameObject, offset + Quaternion.Euler(0.0f, 0.0f, fireAngle) * mod.InversePositionOffset, Quaternion.Euler(0.0f, 0.0f, fireAngle - angleForShot)).GetComponent<Projectile>();
      component3.Inverted = true;
      component3.Owner = (GameActor) this.m_owner;
      component3.Shooter = this.m_owner.specRigidbody;
      if ((bool) (UnityEngine.Object) this.m_owner.aiShooter)
        component3.collidesWithEnemies = this.m_owner.aiShooter.CanShootOtherEnemies;
      component3.PlayerProjectileSourceGameTimeslice = UnityEngine.Time.time;
      if (this.m_owner != null)
        owner.DoPostProcessProjectile(component3);
      component3.baseData.SetAll(component1.baseData);
      component3.IsCritical = component1.IsCritical;
    }
  }
}
