using System;
using System.Collections;
using System.Diagnostics;

#nullable disable

public class IounStoneOrbitalItem : PlayerOrbitalItem
  {
    public bool SlowBulletsOnDamage;
    public float SlowBulletsDuration = 15f;
    public float SlowBulletsMultiplier = 0.5f;
    public bool ChanceToHealOnDamage;
    public float HealChanceNormal = 0.2f;
    public float HealChanceCritical = 0.5f;
    public int GreenerSynergyMoneyGain = 20;
    public float GreenerChanceCritical = 0.7f;
    public bool ModifiesDodgeRoll;
    public float DodgeRollTimeMultiplier = 0.9f;
    public float DodgeRollDistanceMultiplier = 1.25f;
    public bool SynergyCharmsEnemiesOnDamage;
    public GameActorCharmEffect CharmEffect;
    public GameActorFreezeEffect DefaultFreezeEffect;
    public IounStoneOrbitalItem.IounStoneIdentifier Identifier;
    private bool m_isSlowingBullets;
    private float m_slowDurationRemaining;

    public override void Pickup(PlayerController player)
    {
      player.OnReceivedDamage += new Action<PlayerController>(this.OwnerTookDamage);
      player.OnHitByProjectile += new Action<Projectile, PlayerController>(this.OwnerHitByProjectile);
      if (this.Identifier == IounStoneOrbitalItem.IounStoneIdentifier.CLEAR)
        player.PostProcessProjectile += new Action<Projectile, float>(this.HandlePostProcessClearSynergy);
      if (this.ModifiesDodgeRoll)
      {
        player.rollStats.rollDistanceMultiplier *= this.DodgeRollDistanceMultiplier;
        player.rollStats.rollTimeMultiplier *= this.DodgeRollTimeMultiplier;
      }
      base.Pickup(player);
    }

    private void HandlePostProcessClearSynergy(Projectile targetProjectile, float arg2)
    {
      if (!(bool) (UnityEngine.Object) this.m_owner || !this.m_synergyUpgradeActive || !(bool) (UnityEngine.Object) this.m_owner.CurrentGoop)
        return;
      if (this.m_owner.CurrentGoop.CanBeIgnited)
      {
        if (targetProjectile.AppliesFire)
          return;
        targetProjectile.AppliesFire = true;
        targetProjectile.FireApplyChance = 1f;
        targetProjectile.fireEffect = this.m_owner.CurrentGoop.fireEffect;
      }
      else
      {
        if (!this.m_owner.CurrentGoop.CanBeFrozen || targetProjectile.AppliesFreeze)
          return;
        targetProjectile.AppliesFreeze = true;
        targetProjectile.FreezeApplyChance = 1f;
        targetProjectile.freezeEffect = this.DefaultFreezeEffect;
      }
    }

    private void OwnerHitByProjectile(Projectile incomingProjectile, PlayerController arg2)
    {
      if (!this.SynergyCharmsEnemiesOnDamage || !this.m_synergyUpgradeActive || !(bool) (UnityEngine.Object) incomingProjectile || !(bool) (UnityEngine.Object) incomingProjectile.Owner || !(incomingProjectile.Owner is AIActor))
        return;
      incomingProjectile.Owner.ApplyEffect((GameActorEffect) this.CharmEffect);
    }

    private void OwnerTookDamage(PlayerController obj)
    {
      if (this.SlowBulletsOnDamage)
      {
        if (!this.m_isSlowingBullets)
          obj.StartCoroutine(this.HandleSlowBullets());
        else
          this.m_slowDurationRemaining = this.SlowBulletsDuration;
      }
      if (!this.ChanceToHealOnDamage)
        return;
      if ((double) obj.healthHaver.GetCurrentHealth() < 0.5)
      {
        bool flag = obj.HasActiveBonusSynergy(CustomSynergyType.GUON_UPGRADE_GREEN);
        if ((double) UnityEngine.Random.value >= (!flag ? (double) this.HealChanceCritical : (double) this.GreenerChanceCritical))
          return;
        UnityEngine.Debug.Log((object) "Green Guon Critical Heal");
        obj.healthHaver.ApplyHealing(0.5f);
        if (!flag)
          return;
        LootEngine.SpawnCurrency(obj.CenterPosition, this.GreenerSynergyMoneyGain);
      }
      else
      {
        if ((double) UnityEngine.Random.value >= (double) this.HealChanceNormal)
          return;
        UnityEngine.Debug.Log((object) "Green Guon Normal Heal");
        obj.healthHaver.ApplyHealing(0.5f);
        if (!obj.HasActiveBonusSynergy(CustomSynergyType.GUON_UPGRADE_GREEN))
          return;
        LootEngine.SpawnCurrency(obj.CenterPosition, this.GreenerSynergyMoneyGain);
      }
    }

    [DebuggerHidden]
    private IEnumerator HandleSlowBullets()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new IounStoneOrbitalItem__HandleSlowBulletsc__Iterator0()
      {
        _this = this
      };
    }

    public override DebrisObject Drop(PlayerController player)
    {
      player.OnHitByProjectile -= new Action<Projectile, PlayerController>(this.OwnerHitByProjectile);
      player.OnReceivedDamage -= new Action<PlayerController>(this.OwnerTookDamage);
      player.PostProcessProjectile -= new Action<Projectile, float>(this.HandlePostProcessClearSynergy);
      if (this.ModifiesDodgeRoll)
      {
        player.rollStats.rollDistanceMultiplier /= this.DodgeRollDistanceMultiplier;
        player.rollStats.rollTimeMultiplier /= this.DodgeRollTimeMultiplier;
      }
      return base.Drop(player);
    }

    protected override void OnDestroy()
    {
      if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null)
      {
        this.m_owner.OnHitByProjectile -= new Action<Projectile, PlayerController>(this.OwnerHitByProjectile);
        this.m_owner.OnReceivedDamage -= new Action<PlayerController>(this.OwnerTookDamage);
        this.m_owner.PostProcessProjectile -= new Action<Projectile, float>(this.HandlePostProcessClearSynergy);
        if (this.ModifiesDodgeRoll)
        {
          this.m_owner.rollStats.rollDistanceMultiplier /= this.DodgeRollDistanceMultiplier;
          this.m_owner.rollStats.rollTimeMultiplier /= this.DodgeRollTimeMultiplier;
        }
      }
      base.OnDestroy();
    }

    public enum IounStoneIdentifier
    {
      GENERIC,
      CLEAR,
    }
  }

