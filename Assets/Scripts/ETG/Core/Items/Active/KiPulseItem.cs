// Decompiled with JetBrains decompiler
// Type: KiPulseItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

public class KiPulseItem : PlayerItem
  {
    public string IdleAnimation;
    public string CooldownAnimation;
    public string InactiveAnimation;
    public string ActiveAnimation;
    public float DetectionRadius = 0.75f;
    public float PreTriggerPeriod;
    public float TriggerPeriod = 0.25f;
    public float SynergyTriggerPeriod;
    public GameObject KiOverheadVFX;
    private GameObject m_extantVFX;
    private int m_activated;

    public override void Pickup(PlayerController player)
    {
      base.Pickup(player);
      this.m_activated = 0;
      player.OnIsRolling += new Action<PlayerController>(this.HandleRollFrame);
      player.OnDodgedBeam += new Action<BeamController, PlayerController>(this.HandleDodgedBeam);
    }

    private void HandleDodgedBeam(BeamController beam, PlayerController player)
    {
      if (this.IsOnCooldown || player.CurrentRollState != PlayerController.DodgeRollState.InAir)
        return;
      this.StartCoroutine(this.Activate());
    }

    private void HandleRollFrame(PlayerController obj)
    {
      if (this.IsOnCooldown || this.m_activated > 0 || obj.CurrentRollState != PlayerController.DodgeRollState.InAir)
        return;
      Vector2 centerPosition = obj.CenterPosition;
      for (int index = 0; index < StaticReferenceManager.AllProjectiles.Count; ++index)
      {
        Projectile allProjectile = StaticReferenceManager.AllProjectiles[index];
        if ((bool) (UnityEngine.Object) allProjectile && allProjectile.Owner is AIActor && (double) (allProjectile.transform.position.XY() - centerPosition).sqrMagnitude < (double) this.DetectionRadius)
        {
          this.StartCoroutine(this.Activate());
          break;
        }
      }
    }

    public override bool CanBeUsed(PlayerController user)
    {
      return base.CanBeUsed(user) && this.m_activated > 0 && !this.IsOnCooldown;
    }

    private void HandleDodgedProjectile(Projectile obj)
    {
      if (this.IsOnCooldown)
        return;
      this.StartCoroutine(this.Activate());
    }

    [DebuggerHidden]
    private IEnumerator Activate()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new KiPulseItem__Activatec__Iterator0()
      {
        _this = this
      };
    }

    private void LateUpdate()
    {
      bool flag = false;
      if (!this.m_pickedUp)
      {
        if (!this.spriteAnimator.IsPlaying(this.IdleAnimation))
          this.spriteAnimator.Play(this.IdleAnimation);
      }
      else if (this.IsOnCooldown)
      {
        if (!this.spriteAnimator.IsPlaying(this.CooldownAnimation))
          this.spriteAnimator.Play(this.CooldownAnimation);
      }
      else if (this.m_activated <= 0)
      {
        if (!this.spriteAnimator.IsPlaying(this.InactiveAnimation))
          this.spriteAnimator.Play(this.InactiveAnimation);
      }
      else
      {
        flag = true;
        if (!this.spriteAnimator.IsPlaying(this.ActiveAnimation))
          this.spriteAnimator.Play(this.ActiveAnimation);
      }
      if (flag && (bool) (UnityEngine.Object) this.LastOwner)
      {
        if ((bool) (UnityEngine.Object) this.m_extantVFX)
          return;
        this.m_extantVFX = this.LastOwner.PlayEffectOnActor(this.KiOverheadVFX, new Vector3(-1f / 16f, 1.25f, 0.0f));
      }
      else
      {
        if (flag || !(bool) (UnityEngine.Object) this.m_extantVFX || this.m_extantVFX.GetComponent<tk2dSpriteAnimator>().Playing)
          return;
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_extantVFX);
        this.m_extantVFX = (GameObject) null;
      }
    }

    protected override void DoEffect(PlayerController user)
    {
      base.DoEffect(user);
      user.ForceBlank();
      if (!(bool) (UnityEngine.Object) this.m_extantVFX)
        return;
      this.m_extantVFX.GetComponent<tk2dSpriteAnimator>().PlayAndDestroyObject(string.Empty);
    }

    protected override void AfterCooldownApplied(PlayerController user)
    {
      base.AfterCooldownApplied(user);
      if (!user.HasActiveBonusSynergy(CustomSynergyType.GUON_UPGRADE_WHITE))
        return;
      this.CurrentDamageCooldown /= 2f;
    }

    protected override void OnPreDrop(PlayerController user)
    {
      base.OnPreDrop(user);
      user.OnIsRolling -= new Action<PlayerController>(this.HandleRollFrame);
      user.OnDodgedProjectile -= new Action<Projectile>(this.HandleDodgedProjectile);
      user.OnDodgedBeam -= new Action<BeamController, PlayerController>(this.HandleDodgedBeam);
    }

    protected override void OnDestroy()
    {
      base.OnDestroy();
      if (!(bool) (UnityEngine.Object) this.LastOwner)
        return;
      this.LastOwner.OnIsRolling -= new Action<PlayerController>(this.HandleRollFrame);
      this.LastOwner.OnDodgedProjectile -= new Action<Projectile>(this.HandleDodgedProjectile);
      this.LastOwner.OnDodgedBeam -= new Action<BeamController, PlayerController>(this.HandleDodgedBeam);
    }
  }

