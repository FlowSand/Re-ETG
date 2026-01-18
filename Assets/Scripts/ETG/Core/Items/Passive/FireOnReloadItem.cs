using System;
using System.Collections.Generic;

#nullable disable

public class FireOnReloadItem : PassiveItem
  {
    public float ActivationChance = 1f;
    public float InternalCooldown = 1f;
    public bool TriggersRadialBulletBurst;
    [ShowInInspectorIf("TriggersRadialBulletBurst", false)]
    public RadialBurstInterface RadialBurstSettings;
    private float m_lastUsedTime;
    public bool IsHipHolster;

    private void Awake()
    {
      if (!this.IsHipHolster)
        return;
      this.RadialBurstSettings.CustomPostProcessProjectile += new Action<Projectile>(this.HandleHipHolsterProcessing);
    }

    private void HandleHipHolsterProcessing(Projectile proj)
    {
      if (!(bool) (UnityEngine.Object) this.Owner)
        return;
      if (this.Owner.HasActiveBonusSynergy(CustomSynergyType.DOUBLE_HOLSTER))
      {
        HomingModifier homingModifier = proj.gameObject.GetComponent<HomingModifier>();
        if ((UnityEngine.Object) homingModifier == (UnityEngine.Object) null)
        {
          homingModifier = proj.gameObject.AddComponent<HomingModifier>();
          homingModifier.HomingRadius = 0.0f;
          homingModifier.AngularVelocity = 0.0f;
        }
        homingModifier.HomingRadius += 20f;
        homingModifier.AngularVelocity += 1080f;
      }
      if (!this.Owner.HasActiveBonusSynergy(CustomSynergyType.EXPLOSIVE_HOLSTER) || !((UnityEngine.Object) proj.gameObject.GetComponent<ExplosiveModifier>() == (UnityEngine.Object) null))
        return;
      ExplosiveModifier explosiveModifier = proj.gameObject.AddComponent<ExplosiveModifier>();
      explosiveModifier.explosionData = new ExplosionData();
      explosiveModifier.explosionData.ignoreList = new List<SpeculativeRigidbody>();
      explosiveModifier.explosionData.CopyFrom(GameManager.Instance.Dungeon.sharedSettingsPrefab.DefaultSmallExplosionData);
      explosiveModifier.explosionData.damageToPlayer = 0.0f;
      explosiveModifier.explosionData.useDefaultExplosion = false;
    }

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      base.Pickup(player);
      player.OnReloadedGun += new Action<PlayerController, Gun>(this.DoEffect);
    }

    private void DoEffect(PlayerController usingPlayer, Gun usedGun)
    {
      if ((double) UnityEngine.Time.realtimeSinceStartup - (double) this.m_lastUsedTime < (double) this.InternalCooldown || (bool) (UnityEngine.Object) usedGun && usedGun.HasFiredHolsterShot)
        return;
      usedGun.HasFiredHolsterShot = true;
      this.m_lastUsedTime = UnityEngine.Time.realtimeSinceStartup;
      if ((double) UnityEngine.Random.value >= (double) this.ActivationChance || !this.TriggersRadialBulletBurst)
        return;
      this.RadialBurstSettings.DoBurst(usingPlayer);
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      FireOnReloadItem component = debrisObject.GetComponent<FireOnReloadItem>();
      player.OnReloadedGun -= new Action<PlayerController, Gun>(this.DoEffect);
      component.m_pickedUpThisRun = true;
      return debrisObject;
    }

    protected override void OnDestroy() => base.OnDestroy();
  }

