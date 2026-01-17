// Decompiled with JetBrains decompiler
// Type: OnGunDamagedModifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Systems.Utilities
{
    public class OnGunDamagedModifier : MonoBehaviour, IGunInheritable
    {
      [CheckAnimation(null)]
      public string BrokenAnimation;
      public bool DepleteAmmoOnDamage;
      public bool NondepletedGunGrantsFlight;
      public bool DisableHandsOnDepletion;
      public bool PreventDepleteWithSynergy;
      [LongNumericEnum]
      public CustomSynergyType PreventDepleteSynergy;
      private Gun m_gun;
      private PlayerController m_playerOwner;
      private string m_cachedIdleAnimation;
      private string m_cachedEmptyAnimation;
      private string m_cachedChargeAnimation;
      private string m_cachedIntroAnimation;
      private int m_cachedDefaultID = -1;
      private bool m_hasAwoken;
      private bool m_gunBroken;
      private int m_lastFramePlayerHadSynergy = -1;

      public bool Broken
      {
        get => this.m_gunBroken;
        set => this.m_gunBroken = value;
      }

      private void Awake()
      {
        this.m_hasAwoken = true;
        this.m_gun = this.GetComponent<Gun>();
        this.m_cachedIdleAnimation = this.m_gun.idleAnimation;
        this.m_cachedEmptyAnimation = this.m_gun.emptyAnimation;
        this.m_cachedChargeAnimation = this.m_gun.chargeAnimation;
        this.m_cachedIntroAnimation = this.m_gun.introAnimation;
        this.m_cachedDefaultID = this.m_gun.DefaultSpriteID;
        if (this.m_gunBroken && !string.IsNullOrEmpty(this.BrokenAnimation))
          this.SetBrokenAnims();
        this.m_gun.OnInitializedWithOwner += new Action<GameActor>(this.OnGunInitialized);
        this.m_gun.OnDropped += new System.Action(this.OnGunDroppedOrDestroyed);
        this.m_gun.OnAmmoChanged += new Action<PlayerController, Gun>(this.HandleAmmoChanged);
        this.m_gun.OnPostFired += new Action<PlayerController, Gun>(this.HandleAmmoChanged);
        if (!((UnityEngine.Object) this.m_gun.CurrentOwner != (UnityEngine.Object) null))
          return;
        this.OnGunInitialized(this.m_gun.CurrentOwner);
      }

      private void Start() => this.m_cachedDefaultID = this.m_gun.DefaultSpriteID;

      private void Update()
      {
        if (!(bool) (UnityEngine.Object) this.m_playerOwner || !this.PreventDepleteWithSynergy || !this.m_playerOwner.HasActiveBonusSynergy(this.PreventDepleteSynergy))
          return;
        this.m_lastFramePlayerHadSynergy = UnityEngine.Time.frameCount;
      }

      private void SetBrokenAnims()
      {
        this.m_gun.CanBeDropped = false;
        this.m_gun.idleAnimation = this.BrokenAnimation;
        this.m_gun.emptyAnimation = this.BrokenAnimation;
        this.m_gun.chargeAnimation = string.Empty;
        this.m_gun.introAnimation = string.Empty;
        tk2dSpriteAnimationClip clipByName = this.m_gun.spriteAnimator.GetClipByName(this.BrokenAnimation);
        this.m_gun.DefaultSpriteID = clipByName.frames[clipByName.frames.Length - 1].spriteId;
      }

      private void HandleAmmoChanged(PlayerController player, Gun ammoGun)
      {
        if (!(bool) (UnityEngine.Object) this.m_playerOwner)
          return;
        if ((UnityEngine.Object) ammoGun == (UnityEngine.Object) this.m_gun && ammoGun.ammo >= 1 && this.m_gunBroken)
        {
          this.m_gunBroken = false;
          if (this.DisableHandsOnDepletion)
          {
            this.m_gun.additionalHandState = AdditionalHandState.None;
            player.ToggleHandRenderers(true, string.Empty);
            player.ProcessHandAttachment();
            GameManager.Instance.Dungeon.StartCoroutine(this.FrameDelayedProcessing(player));
          }
          if (!string.IsNullOrEmpty(this.BrokenAnimation))
          {
            this.m_gun.CanBeDropped = true;
            this.m_gun.idleAnimation = this.m_cachedIdleAnimation;
            this.m_gun.emptyAnimation = this.m_cachedEmptyAnimation;
            this.m_gun.chargeAnimation = this.m_cachedChargeAnimation;
            this.m_gun.introAnimation = this.m_cachedIntroAnimation;
            this.m_gun.DefaultSpriteID = this.m_cachedDefaultID;
            this.m_gun.PlayIdleAnimation();
          }
        }
        this.CheckFlightStatus(this.m_playerOwner.CurrentGun);
      }

      [DebuggerHidden]
      private IEnumerator FrameDelayedProcessing(PlayerController p)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new OnGunDamagedModifier__FrameDelayedProcessingc__Iterator0()
        {
          p = p,
          _this = this
        };
      }

      private void OnGunInitialized(GameActor actor)
      {
        if ((UnityEngine.Object) this.m_playerOwner != (UnityEngine.Object) null)
          this.OnGunDroppedOrDestroyed();
        if ((UnityEngine.Object) actor == (UnityEngine.Object) null)
          return;
        if (actor is PlayerController)
        {
          this.m_playerOwner = actor as PlayerController;
          this.m_playerOwner.OnReceivedDamage += new Action<PlayerController>(this.OnReceivedDamage);
          this.m_playerOwner.GunChanged += new Action<Gun, Gun, bool>(this.HandleGunChanged);
        }
        if (!(bool) (UnityEngine.Object) this.m_playerOwner)
          return;
        this.CheckFlightStatus(this.m_playerOwner.CurrentGun);
      }

      private void CheckFlightStatus(Gun currentGun)
      {
        if ((bool) (UnityEngine.Object) this.m_gun)
          this.m_gun.overrideOutOfAmmoHandedness = GunHandedness.NoHanded;
        if (!this.NondepletedGunGrantsFlight || !(bool) (UnityEngine.Object) this.m_playerOwner || !(bool) (UnityEngine.Object) currentGun)
          return;
        OnGunDamagedModifier component = currentGun.GetComponent<OnGunDamagedModifier>();
        if ((bool) (UnityEngine.Object) component && component.NondepletedGunGrantsFlight)
        {
          this.m_playerOwner.SetIsFlying(!component.m_gunBroken, "balloon gun", false);
          this.m_playerOwner.AdditionalCanDodgeRollWhileFlying.SetOverride("balloon gun", true);
        }
        else
        {
          this.m_playerOwner.SetIsFlying(false, "balloon gun", false);
          this.m_playerOwner.AdditionalCanDodgeRollWhileFlying.RemoveOverride("balloon gun");
        }
      }

      private void HandleGunChanged(Gun previous, Gun current, bool isNew)
      {
        this.CheckFlightStatus(current);
      }

      private void OnReceivedDamage(PlayerController player)
      {
        if (!(bool) (UnityEngine.Object) player || !((UnityEngine.Object) player.CurrentGun == (UnityEngine.Object) this.m_gun) || this.PreventDepleteWithSynergy && player.HasActiveBonusSynergy(this.PreventDepleteSynergy) || this.PreventDepleteWithSynergy && this.m_lastFramePlayerHadSynergy == UnityEngine.Time.frameCount)
          return;
        if (!this.m_gunBroken)
        {
          this.m_gunBroken = true;
          if (!string.IsNullOrEmpty(this.BrokenAnimation))
          {
            this.SetBrokenAnims();
            this.m_gun.PlayIdleAnimation();
          }
        }
        if (this.DepleteAmmoOnDamage && (!this.PreventDepleteWithSynergy || !player.HasActiveBonusSynergy(this.PreventDepleteSynergy)))
        {
          this.m_gun.ammo = 0;
          if (this.DisableHandsOnDepletion)
            this.m_gun.additionalHandState = AdditionalHandState.HideBoth;
        }
        this.CheckFlightStatus(player.CurrentGun);
      }

      private void OnDestroy() => this.OnGunDroppedOrDestroyed();

      private void OnGunDroppedOrDestroyed()
      {
        if (!((UnityEngine.Object) this.m_playerOwner != (UnityEngine.Object) null))
          return;
        this.m_playerOwner.OnReceivedDamage -= new Action<PlayerController>(this.OnReceivedDamage);
        this.m_playerOwner.GunChanged -= new Action<Gun, Gun, bool>(this.HandleGunChanged);
        this.m_playerOwner = (PlayerController) null;
      }

      public void InheritData(Gun sourceGun)
      {
        if (!(bool) (UnityEngine.Object) sourceGun)
          return;
        if (!this.m_hasAwoken)
        {
          this.m_gun = this.GetComponent<Gun>();
          this.m_cachedIdleAnimation = this.m_gun.idleAnimation;
          this.m_cachedEmptyAnimation = this.m_gun.emptyAnimation;
          this.m_cachedChargeAnimation = this.m_gun.chargeAnimation;
          this.m_cachedIntroAnimation = this.m_gun.introAnimation;
          this.m_cachedDefaultID = this.m_gun.DefaultSpriteID;
        }
        OnGunDamagedModifier component = sourceGun.GetComponent<OnGunDamagedModifier>();
        if (!(bool) (UnityEngine.Object) component)
          return;
        this.m_gunBroken = component.m_gunBroken;
        if (!string.IsNullOrEmpty(component.m_cachedEmptyAnimation))
          this.m_cachedEmptyAnimation = component.m_cachedEmptyAnimation;
        if (!string.IsNullOrEmpty(component.m_cachedIdleAnimation))
          this.m_cachedIdleAnimation = component.m_cachedIdleAnimation;
        if (!string.IsNullOrEmpty(component.m_cachedChargeAnimation))
          this.m_cachedChargeAnimation = component.m_cachedChargeAnimation;
        if (!string.IsNullOrEmpty(component.m_cachedIntroAnimation))
          this.m_cachedIntroAnimation = component.m_cachedIntroAnimation;
        if (component.m_cachedDefaultID != -1)
          this.m_cachedDefaultID = component.m_cachedDefaultID;
        this.GetComponent<Gun>().idleAnimation = this.m_cachedIdleAnimation;
        this.GetComponent<Gun>().emptyAnimation = this.m_cachedEmptyAnimation;
        this.GetComponent<Gun>().chargeAnimation = this.m_cachedChargeAnimation;
        this.GetComponent<Gun>().introAnimation = this.m_cachedIntroAnimation;
      }

      public void MidGameSerialize(List<object> data, int dataIndex) => data.Add((object) this.Broken);

      public void MidGameDeserialize(List<object> data, ref int dataIndex)
      {
        this.Broken = (bool) data[dataIndex];
        if (this.m_gunBroken && !string.IsNullOrEmpty(this.BrokenAnimation))
        {
          this.SetBrokenAnims();
          this.m_gun.PlayIdleAnimation();
        }
        ++dataIndex;
      }
    }

}
