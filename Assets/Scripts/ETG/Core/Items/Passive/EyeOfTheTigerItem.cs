// Decompiled with JetBrains decompiler
// Type: EyeOfTheTigerItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class EyeOfTheTigerItem : PassiveItem
  {
    public float HealthThreshold = 1f;
    public float ChanceToIgnoreDamage = 0.5f;
    public float DamageMultiplier = 2f;
    public bool ModifiesDodgeRoll;
    public float DodgeRollTimeMultiplier = 1f;
    public float DodgeRollDistanceMultiplier = 1f;
    public bool DoesFullHeal;
    public GameObject OnIgnoredDamageVFX;
    public bool HasTimedStatSynergyBuffOnKill;
    [ShowInInspectorIf("HasTimedStatSynergyBuffOnKill", false)]
    public TimedSynergyStatBuff KillTimedSynergyBuff;
    private float m_remainingKillModifierTime;
    private StatModifier m_temporaryKillStatModifier;

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      player.healthHaver.ModifyDamage += new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage);
      player.PostProcessProjectile += new Action<Projectile, float>(this.PostProcessProjectile);
      player.PostProcessBeam += new Action<BeamController>(this.PostProcessBeam);
      if (this.ModifiesDodgeRoll)
      {
        player.rollStats.rollTimeMultiplier *= this.DodgeRollTimeMultiplier;
        player.rollStats.rollDistanceMultiplier *= this.DodgeRollDistanceMultiplier;
      }
      player.OnKilledEnemy += new Action<PlayerController>(this.HandleKilledEnemy);
      if (!this.m_pickedUpThisRun && this.DoesFullHeal)
        player.healthHaver.FullHeal();
      base.Pickup(player);
    }

    private void HandleKilledEnemy(PlayerController targetPlayer)
    {
      if (!this.HasTimedStatSynergyBuffOnKill || !targetPlayer.HasActiveBonusSynergy(this.KillTimedSynergyBuff.RequiredSynergy))
        return;
      if (this.m_temporaryKillStatModifier == null)
      {
        this.m_temporaryKillStatModifier = new StatModifier();
        this.m_temporaryKillStatModifier.statToBoost = this.KillTimedSynergyBuff.statToBoost;
        this.m_temporaryKillStatModifier.modifyType = this.KillTimedSynergyBuff.modifyType;
        this.m_temporaryKillStatModifier.amount = this.KillTimedSynergyBuff.amount;
      }
      if ((double) this.m_remainingKillModifierTime <= 0.0)
        targetPlayer.StartCoroutine(this.HandleTimedKillStatBoost(targetPlayer));
      this.m_remainingKillModifierTime = this.KillTimedSynergyBuff.duration;
    }

    [DebuggerHidden]
    private IEnumerator HandleTimedKillStatBoost(PlayerController target)
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new EyeOfTheTigerItem__HandleTimedKillStatBoostc__Iterator0()
      {
        target = target,
        _this = this
      };
    }

    private void PostProcessBeam(BeamController obj)
    {
      obj.projectile.baseData.damage *= this.DamageMultiplier;
    }

    private void PostProcessProjectile(Projectile obj, float effectChanceScalar)
    {
      obj.baseData.damage *= this.DamageMultiplier;
    }

    private void ModifyIncomingDamage(HealthHaver source, HealthHaver.ModifyDamageEventArgs args)
    {
      if (args == EventArgs.Empty || (double) source.GetCurrentHealth() > (double) this.HealthThreshold || (double) UnityEngine.Random.value >= (double) this.ChanceToIgnoreDamage)
        return;
      if ((UnityEngine.Object) this.OnIgnoredDamageVFX != (UnityEngine.Object) null)
        source.GetComponent<PlayerController>().PlayEffectOnActor(this.OnIgnoredDamageVFX, Vector3.zero);
      args.ModifiedDamage = 0.0f;
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      player.healthHaver.ModifyDamage -= new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage);
      player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
      player.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
      player.OnKilledEnemy -= new Action<PlayerController>(this.HandleKilledEnemy);
      if (this.ModifiesDodgeRoll)
      {
        player.rollStats.rollTimeMultiplier /= this.DodgeRollTimeMultiplier;
        player.rollStats.rollDistanceMultiplier /= this.DodgeRollDistanceMultiplier;
      }
      debrisObject.GetComponent<EyeOfTheTigerItem>().m_pickedUpThisRun = true;
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      if (this.m_pickedUp)
      {
        this.m_owner.healthHaver.ModifyDamage += new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.ModifyIncomingDamage);
        this.m_owner.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
        this.m_owner.PostProcessBeam -= new Action<BeamController>(this.PostProcessBeam);
        this.m_owner.OnKilledEnemy -= new Action<PlayerController>(this.HandleKilledEnemy);
        if (this.ModifiesDodgeRoll)
        {
          this.m_owner.rollStats.rollTimeMultiplier /= this.DodgeRollTimeMultiplier;
          this.m_owner.rollStats.rollDistanceMultiplier /= this.DodgeRollDistanceMultiplier;
        }
      }
      base.OnDestroy();
    }
  }

