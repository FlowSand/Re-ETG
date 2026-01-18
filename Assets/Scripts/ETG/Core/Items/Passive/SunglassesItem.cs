using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class SunglassesItem : PassiveItem
  {
    public static bool SunglassesActive;
    public float timeScaleMultiplier = 0.25f;
    public float Duration = 3f;
    public float InternalCooldown = 5f;
    private float m_remainingSlowTime;
    private float m_internalCooldown;
    public float MIBSynergyScale = 1.33f;
    public float MIBSynergyDamage = 1.8f;

    protected override void Update()
    {
      base.Update();
      if ((UnityEngine.Object) this.m_owner != (UnityEngine.Object) null && this.m_pickedUp)
      {
        if ((double) this.m_remainingSlowTime <= 0.0)
        {
          this.m_internalCooldown -= BraveTime.DeltaTime;
          BraveTime.ClearMultiplier(this.gameObject);
          SunglassesItem.SunglassesActive = false;
        }
        else
        {
          SunglassesItem.SunglassesActive = true;
          this.m_remainingSlowTime -= GameManager.INVARIANT_DELTA_TIME;
          BraveTime.SetTimeScaleMultiplier(this.timeScaleMultiplier, this.gameObject);
        }
      }
      else
        BraveTime.ClearMultiplier(this.gameObject);
    }

    private void OnExplosion()
    {
      if ((double) this.m_internalCooldown > 0.0)
        return;
      this.m_internalCooldown = this.InternalCooldown;
      this.m_remainingSlowTime = this.Duration;
      if (!(bool) (UnityEngine.Object) this.Owner || !this.Owner.HasActiveBonusSynergy(CustomSynergyType.MEN_IN_BLACK) || !(bool) (UnityEngine.Object) this.Owner.CurrentGun)
        return;
      int num = (int) AkSoundEngine.PostEvent("Play_WPN_active_reload_01", this.gameObject);
      this.Owner.CurrentGun.ForceImmediateReload();
      this.Owner.CurrentGun.TriggerTemporaryBoost(this.MIBSynergyDamage, this.MIBSynergyScale, this.Duration, true);
    }

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      Exploder.OnExplosionTriggered += new System.Action(this.OnExplosion);
      if (!PassiveItem.ActiveFlagItems.ContainsKey(player))
        PassiveItem.ActiveFlagItems.Add(player, new Dictionary<System.Type, int>());
      if (!PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
        PassiveItem.ActiveFlagItems[player].Add(this.GetType(), 1);
      else
        PassiveItem.ActiveFlagItems[player][this.GetType()] = PassiveItem.ActiveFlagItems[player][this.GetType()] + 1;
      base.Pickup(player);
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      Exploder.OnExplosionTriggered -= new System.Action(this.OnExplosion);
      BraveTime.ClearMultiplier(this.gameObject);
      if (PassiveItem.ActiveFlagItems.ContainsKey(player) && PassiveItem.ActiveFlagItems[player].ContainsKey(this.GetType()))
      {
        PassiveItem.ActiveFlagItems[player][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[player][this.GetType()] - 1);
        if (PassiveItem.ActiveFlagItems[player][this.GetType()] == 0)
          PassiveItem.ActiveFlagItems[player].Remove(this.GetType());
      }
      debrisObject.GetComponent<SunglassesItem>().m_pickedUpThisRun = true;
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      SunglassesItem.SunglassesActive = false;
      BraveTime.ClearMultiplier(this.gameObject);
      Exploder.OnExplosionTriggered -= new System.Action(this.OnExplosion);
      BraveTime.ClearMultiplier(this.gameObject);
      if (this.m_pickedUp && PassiveItem.ActiveFlagItems.ContainsKey(this.m_owner) && PassiveItem.ActiveFlagItems[this.m_owner].ContainsKey(this.GetType()))
      {
        PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] = Mathf.Max(0, PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] - 1);
        if (PassiveItem.ActiveFlagItems[this.m_owner][this.GetType()] == 0)
          PassiveItem.ActiveFlagItems[this.m_owner].Remove(this.GetType());
      }
      base.OnDestroy();
    }
  }

