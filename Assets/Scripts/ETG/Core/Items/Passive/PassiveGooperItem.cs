// Decompiled with JetBrains decompiler
// Type: PassiveGooperItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class PassiveGooperItem : PassiveItem
  {
    public PassiveGooperItem.Condition condition;
    public bool IsDegooperator;
    public bool TranslatesGleepGlorp;
    public GoopDefinition goopType;
    public float goopRadius;
    public DamageTypeModifier[] modifiers;
    public PassiveGooperSynergy[] Synergies;
    public float AirSoftSynergyAmmoGainRate = 0.05f;
    private GoopDefinition m_cachedGoopType;
    private float m_synergyAccumAmmo;
    private PlayerController m_player;

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      this.m_cachedGoopType = this.goopType;
      base.Pickup(player);
      if (this.TranslatesGleepGlorp)
        player.UnderstandsGleepGlorp = true;
      if (this.condition == PassiveGooperItem.Condition.WhileDodgeRolling)
        player.OnIsRolling += new Action<PlayerController>(this.OnRollFrame);
      else if (this.condition == PassiveGooperItem.Condition.OnDamaged)
        player.OnReceivedDamage += new Action<PlayerController>(this.HandleReceivedDamage);
      this.m_player = player;
      for (int index = 0; index < this.modifiers.Length; ++index)
        player.healthHaver.damageTypeModifiers.Add(this.modifiers[index]);
    }

    protected override void Update()
    {
      base.Update();
      if (!this.m_pickedUp || !((UnityEngine.Object) this.m_player != (UnityEngine.Object) null) || GameManager.Instance.IsLoadingLevel)
        return;
      if (this.condition == PassiveGooperItem.Condition.Always)
        this.DoGoop();
      for (int index = 0; index < this.Synergies.Length; ++index)
      {
        if (!this.Synergies[index].m_processed && this.m_player.HasActiveBonusSynergy(this.Synergies[index].RequiredSynergy))
        {
          this.Synergies[index].m_processed = true;
          this.goopType = this.Synergies[index].overrideGoopType;
          this.m_player.healthHaver.damageTypeModifiers.Add(this.Synergies[index].AdditionalDamageModifier);
        }
        else if (this.Synergies[index].m_processed && !this.m_player.HasActiveBonusSynergy(this.Synergies[index].RequiredSynergy))
        {
          this.Synergies[index].m_processed = false;
          this.goopType = this.m_cachedGoopType;
          this.m_player.healthHaver.damageTypeModifiers.Remove(this.Synergies[index].AdditionalDamageModifier);
        }
      }
    }

    private void DoGoop()
    {
      if (this.IsDegooperator)
      {
        if ((bool) (UnityEngine.Object) this.Owner && this.Owner.HasActiveBonusSynergy(CustomSynergyType.AIR_SOFT) && (bool) (UnityEngine.Object) this.Owner.CurrentGun)
        {
          int num = DeadlyDeadlyGoopManager.CountGoopsInRadius(this.m_player.specRigidbody.UnitCenter, this.goopRadius);
          if (num > 0)
          {
            this.m_synergyAccumAmmo += (float) num * this.AirSoftSynergyAmmoGainRate;
            if ((double) this.m_synergyAccumAmmo > 1.0)
            {
              this.Owner.CurrentGun.GainAmmo(Mathf.FloorToInt(this.m_synergyAccumAmmo));
              this.m_synergyAccumAmmo -= (float) Mathf.FloorToInt(this.m_synergyAccumAmmo);
            }
          }
        }
        DeadlyDeadlyGoopManager.DelayedClearGoopsInRadius(this.m_player.specRigidbody.UnitCenter, this.goopRadius);
      }
      else if (this.condition == PassiveGooperItem.Condition.OnDamaged)
        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopType).TimedAddGoopCircle(this.m_player.specRigidbody.UnitCenter, this.goopRadius);
      else
        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.goopType).AddGoopCircle(this.m_player.specRigidbody.UnitCenter, this.goopRadius);
    }

    private void HandleReceivedDamage(PlayerController obj) => this.DoGoop();

    private void OnRollFrame(PlayerController obj) => this.DoGoop();

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      debrisObject.GetComponent<PassiveGooperItem>().m_pickedUpThisRun = true;
      for (int index = 0; index < this.modifiers.Length; ++index)
        player.healthHaver.damageTypeModifiers.Remove(this.modifiers[index]);
      if (this.condition == PassiveGooperItem.Condition.WhileDodgeRolling)
        player.OnIsRolling -= new Action<PlayerController>(this.OnRollFrame);
      else if (this.condition == PassiveGooperItem.Condition.OnDamaged)
        player.OnReceivedDamage -= new Action<PlayerController>(this.HandleReceivedDamage);
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      if ((UnityEngine.Object) this.m_player != (UnityEngine.Object) null)
      {
        if (this.condition == PassiveGooperItem.Condition.WhileDodgeRolling)
          this.m_player.OnIsRolling -= new Action<PlayerController>(this.OnRollFrame);
        else if (this.condition == PassiveGooperItem.Condition.OnDamaged)
          this.m_player.OnReceivedDamage -= new Action<PlayerController>(this.HandleReceivedDamage);
        for (int index = 0; index < this.modifiers.Length; ++index)
          this.m_player.healthHaver.damageTypeModifiers.Remove(this.modifiers[index]);
      }
      base.OnDestroy();
    }

    public enum Condition
    {
      WhileDodgeRolling,
      Always,
      OnDamaged,
    }
  }

