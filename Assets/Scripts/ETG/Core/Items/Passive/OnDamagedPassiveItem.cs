using System;
using UnityEngine;

#nullable disable

public class OnDamagedPassiveItem : PassiveItem
  {
    public int ArmorToGive;
    public int FlatAmmoToGive;
    public float PercentAmmoToGive;
    public bool DoesEffectOnArmorLoss;
    public bool DoesDamageToEnemiesInRoom;
    public float DamageToEnemiesInRoom = 25f;
    public bool HasSynergy;
    [LongNumericEnum]
    public CustomSynergyType RequiredSynergy;
    public bool SynergyAugmentsNextShot;

    public override void Pickup(PlayerController player)
    {
      if (this.m_pickedUp)
        return;
      base.Pickup(player);
      player.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.PlayerTookDamage);
    }

    private void PlayerTookDamage(
      float resultValue,
      float maxValue,
      CoreDamageTypes damageTypes,
      DamageCategory damageCategory,
      Vector2 damageDirection)
    {
      if ((double) resultValue >= (double) maxValue && !this.DoesEffectOnArmorLoss)
        return;
      if ((UnityEngine.Object) this.Owner.CurrentGun != (UnityEngine.Object) null && this.FlatAmmoToGive > 0)
        this.Owner.CurrentGun.GainAmmo(this.FlatAmmoToGive);
      if ((UnityEngine.Object) this.Owner.CurrentGun != (UnityEngine.Object) null && (double) this.PercentAmmoToGive > 0.0)
        this.Owner.CurrentGun.GainAmmo(Mathf.CeilToInt((float) this.Owner.CurrentGun.AdjustedMaxAmmo * this.PercentAmmoToGive));
      if (this.ArmorToGive > 0)
        this.Owner.healthHaver.Armor += (float) this.ArmorToGive;
      if (this.DoesDamageToEnemiesInRoom)
        this.Owner.CurrentRoom.ApplyActionToNearbyEnemies(this.Owner.CenterPosition, 100f, (Action<AIActor, float>) ((enemy, dist) =>
        {
          if (!(bool) (UnityEngine.Object) enemy || !(bool) (UnityEngine.Object) enemy.healthHaver)
            return;
          enemy.healthHaver.ApplyDamage(this.DamageToEnemiesInRoom, Vector2.zero, string.Empty);
        }));
      if (!this.HasSynergy || !this.Owner.HasActiveBonusSynergy(this.RequiredSynergy) || !this.SynergyAugmentsNextShot || !this.Owner.CurrentGun.CanCriticalFire)
        return;
      this.Owner.CurrentGun.ForceNextShotCritical = true;
    }

    public override DebrisObject Drop(PlayerController player)
    {
      DebrisObject debrisObject = base.Drop(player);
      OnDamagedPassiveItem component = debrisObject.GetComponent<OnDamagedPassiveItem>();
      player.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.PlayerTookDamage);
      component.m_pickedUpThisRun = true;
      return debrisObject;
    }

    protected override void OnDestroy()
    {
      if ((bool) (UnityEngine.Object) this.m_owner)
        this.m_owner.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.PlayerTookDamage);
      base.OnDestroy();
    }
  }

