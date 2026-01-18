using System;
using UnityEngine;

#nullable disable

public class StealthOnReloadPressed : MonoBehaviour
  {
    public GameObject poofVfx;
    public bool OnlyOnClipEmpty = true;
    [Header("Synergues")]
    public bool SynergyContingent;
    [LongNumericEnum]
    public CustomSynergyType RequiredSynergy;
    private Gun m_gun;

    private void Awake()
    {
      this.m_gun = this.GetComponent<Gun>();
      if (this.OnlyOnClipEmpty)
        this.m_gun.OnAutoReload += new Action<PlayerController, Gun>(this.HandleReloadPressedSimple);
      else
        this.m_gun.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.HandleReloadPressed);
    }

    private void OnDamaged(
      float resultValue,
      float maxValue,
      CoreDamageTypes damageTypes,
      DamageCategory damageCategory,
      Vector2 damageDirection)
    {
      this.BreakStealth(this.m_gun.CurrentOwner as PlayerController);
    }

    private void BreakStealth(PlayerController obj)
    {
      obj.PlayEffectOnActor(this.poofVfx, Vector3.zero, false, true);
      obj.ChangeSpecialShaderFlag(1, 0.0f);
      obj.OnDidUnstealthyAction -= new Action<PlayerController>(this.BreakStealth);
      obj.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnDamaged);
      obj.SetIsStealthed(false, "box");
      obj.SetCapableOfStealing(false, nameof (StealthOnReloadPressed));
    }

    private void HandleReloadPressedSimple(PlayerController user, Gun sourceGun)
    {
      this.HandleReloadPressed(user, sourceGun, false);
    }

    private void HandleReloadPressed(PlayerController user, Gun sourceGun, bool actual)
    {
      if (this.SynergyContingent && !user.HasActiveBonusSynergy(this.RequiredSynergy))
        return;
      if (this.SynergyContingent)
      {
        sourceGun.CanSneakAttack = true;
        sourceGun.SneakAttackDamageMultiplier = 4f;
      }
      if (!this.OnlyOnClipEmpty && this.m_gun.IsFiring)
        return;
      user.PlayEffectOnActor(this.poofVfx, Vector3.zero, false, true);
      user.ChangeSpecialShaderFlag(1, 1f);
      user.OnDidUnstealthyAction += new Action<PlayerController>(this.BreakStealth);
      user.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnDamaged);
      user.SetIsStealthed(true, "box");
      user.SetCapableOfStealing(true, nameof (StealthOnReloadPressed));
    }
  }

