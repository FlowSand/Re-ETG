// Decompiled with JetBrains decompiler
// Type: MimicGunController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class MimicGunController : MonoBehaviour
    {
      public float DamageRequired = 300f;
      public GameObject BecomeMimicVFX;
      public GameObject UnbecomeMimicVfx;
      [Header("Audio")]
      public string AcquisitionAudioEvent;
      public string RefillingAmmoAudioEvent;
      private Gun m_gun;
      private bool m_initialized;
      private float m_damageDealt;
      private Gun m_sourceGun;
      private bool m_selfRefilling;
      private bool m_isClearing;

      public void Initialize(PlayerController p, Gun sourceGun)
      {
        this.m_initialized = true;
        this.m_gun = this.GetComponent<Gun>();
        p.inventory.GunLocked.AddOverride("mimic gun");
        p.OnDealtDamage += new Action<PlayerController, float>(this.HandleDealtDamage);
        this.m_gun.OnAmmoChanged += new Action<PlayerController, Gun>(this.HandleAmmoChanged);
        this.m_sourceGun = sourceGun;
        if (!string.IsNullOrEmpty(this.AcquisitionAudioEvent))
        {
          int num = (int) AkSoundEngine.PostEvent(this.AcquisitionAudioEvent, this.gameObject);
        }
        if ((bool) (UnityEngine.Object) this.BecomeMimicVFX)
          SpawnManager.SpawnVFX(this.BecomeMimicVFX, (Vector3) this.m_gun.GetSprite().WorldCenter, Quaternion.identity);
        this.m_gun.OverrideAnimations = true;
        this.StartCoroutine(this.HandleDeferredAnimationOverride(1f));
        this.m_gun.spriteAnimator.PlayForDuration("mimic_gun_intro", 1f, this.m_gun.idleAnimation);
      }

      private void Update()
      {
        if (this.m_isClearing || !(bool) (UnityEngine.Object) this.m_gun || this.m_gun.ammo > 0)
          return;
        if (!string.IsNullOrEmpty(this.RefillingAmmoAudioEvent))
        {
          int num = (int) AkSoundEngine.PostEvent(this.RefillingAmmoAudioEvent, this.gameObject);
        }
        this.m_gun.OverrideAnimations = true;
        this.StartCoroutine(this.HandleDeferredAnimationOverride(3f));
        this.m_gun.spriteAnimator.PlayForDuration("mimic_gun_laugh", 3f, this.m_gun.idleAnimation);
        this.m_selfRefilling = true;
        this.m_gun.GainAmmo(this.m_gun.AdjustedMaxAmmo);
        this.m_selfRefilling = false;
      }

      public void OnDestroy()
      {
        if (!(bool) (UnityEngine.Object) this.m_gun)
          return;
        PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
        if (!(bool) (UnityEngine.Object) currentOwner)
          return;
        currentOwner.OnDealtDamage -= new Action<PlayerController, float>(this.HandleDealtDamage);
        currentOwner.inventory.GunLocked.RemoveOverride("mimic gun");
      }

      [DebuggerHidden]
      private IEnumerator HandleDeferredAnimationOverride(float t)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MimicGunController.<HandleDeferredAnimationOverride>c__Iterator0()
        {
          t = t,
          _this = this
        };
      }

      private void HandleDealtDamage(PlayerController source, float dmg)
      {
        if (this.m_isClearing)
          return;
        this.m_damageDealt += dmg;
        if ((double) this.m_damageDealt < (double) this.DamageRequired)
          return;
        this.ClearMimic();
      }

      private void HandleAmmoChanged(PlayerController sourcePlayer, Gun sourceGun)
      {
        if (this.m_isClearing || !((UnityEngine.Object) sourceGun == (UnityEngine.Object) this.m_gun) || this.m_selfRefilling || sourceGun.ammo < sourceGun.AdjustedMaxAmmo)
          return;
        this.ForceClearMimic();
      }

      public void ForceClearMimic(bool instant = false)
      {
        if (this.m_isClearing)
          return;
        this.m_damageDealt = 10000f;
        if (instant)
        {
          if (!(bool) (UnityEngine.Object) this.m_gun || !(bool) (UnityEngine.Object) this.m_gun.CurrentOwner)
            return;
          if ((bool) (UnityEngine.Object) this.UnbecomeMimicVfx)
            SpawnManager.SpawnVFX(this.UnbecomeMimicVfx, (Vector3) this.m_gun.GetSprite().WorldCenter, Quaternion.identity);
          PlayerController currentOwner = this.m_gun.CurrentOwner as PlayerController;
          currentOwner.OnDealtDamage -= new Action<PlayerController, float>(this.HandleDealtDamage);
          currentOwner.inventory.GunLocked.RemoveOverride("mimic gun");
          currentOwner.inventory.DestroyGun(this.m_gun);
          currentOwner.ChangeToGunSlot(currentOwner.inventory.AllGuns.IndexOf(this.m_sourceGun), true);
        }
        else
          this.ClearMimic();
      }

      [DebuggerHidden]
      private IEnumerator HandleClearMimic()
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new MimicGunController.<HandleClearMimic>c__Iterator1()
        {
          _this = this
        };
      }

      private void ClearMimic()
      {
        if (this.m_isClearing)
          return;
        this.StartCoroutine(this.HandleClearMimic());
      }
    }

}
