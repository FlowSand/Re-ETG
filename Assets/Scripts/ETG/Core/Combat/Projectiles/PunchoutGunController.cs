// Decompiled with JetBrains decompiler
// Type: PunchoutGunController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class PunchoutGunController : MonoBehaviour
    {
      public string UIStarSpriteName;
      public Projectile BaseProjectile;
      public Projectile OverrideProjectile;
      public float ChargeTimeNormal;
      public float ChargeTimeStar = 0.5f;
      [CheckAnimation(null)]
      public string OverrideFireAnimation;
      [CheckAnimation(null)]
      public string OverrideChargeAnimation;
      private string CachedFireAnimation;
      private string CachedChargeAnimation;
      private Gun m_gun;
      private List<dfSprite> m_extantStars = new List<dfSprite>();
      private PlayerController m_cachedPlayer;

      public void Awake()
      {
        this.m_gun = this.GetComponent<Gun>();
        this.m_gun.OnPreFireProjectileModifier += new Func<Gun, Projectile, ProjectileModule, Projectile>(this.HandlePrefireModifier);
        this.m_gun.PostProcessProjectile += new Action<Projectile>(this.HandleProjectileFired);
        this.m_gun.OnDropped += new System.Action(this.HandleDropped);
        this.CachedFireAnimation = this.m_gun.shootAnimation;
        this.CachedChargeAnimation = this.m_gun.chargeAnimation;
      }

      private void Update()
      {
        if (!(bool) (UnityEngine.Object) this.m_cachedPlayer && (bool) (UnityEngine.Object) this.m_gun && this.m_gun.CurrentOwner is PlayerController)
        {
          this.m_cachedPlayer = this.m_gun.CurrentOwner as PlayerController;
          this.m_cachedPlayer.OnReceivedDamage += new Action<PlayerController>(this.HandleWasDamaged);
        }
        else
        {
          if (!(bool) (UnityEngine.Object) this.m_cachedPlayer || !(bool) (UnityEngine.Object) this.m_gun || (bool) (UnityEngine.Object) this.m_gun.CurrentOwner)
            return;
          this.m_cachedPlayer.OnReceivedDamage -= new Action<PlayerController>(this.HandleWasDamaged);
          this.m_cachedPlayer = (PlayerController) null;
        }
      }

      private void HandleWasDamaged(PlayerController obj)
      {
        if (!this.enabled)
          return;
        this.RemoveAllStars();
      }

      private void HandleDropped() => this.RemoveAllStars();

      public void OnDisable() => this.RemoveAllStars();

      public void OnDestroy()
      {
        this.RemoveAllStars();
        if (!(bool) (UnityEngine.Object) this.m_cachedPlayer)
          return;
        this.m_cachedPlayer.OnReceivedDamage -= new Action<PlayerController>(this.HandleWasDamaged);
        this.m_cachedPlayer = (PlayerController) null;
      }

      private Projectile HandlePrefireModifier(
        Gun sourceGun,
        Projectile sourceProjectile,
        ProjectileModule sourceModule)
      {
        bool flag = (UnityEngine.Object) sourceProjectile == (UnityEngine.Object) this.OverrideProjectile;
        if (this.m_extantStars.Count < 3 || !flag || !(bool) (UnityEngine.Object) this.m_cachedPlayer)
          return this.BaseProjectile;
        this.RemoveAllStars();
        if ((bool) (UnityEngine.Object) this.m_gun.spriteAnimator)
        {
          this.m_gun.OverrideAnimations = true;
          this.m_gun.spriteAnimator.Play(this.OverrideFireAnimation);
        }
        return sourceProjectile;
      }

      private void HandleProjectileFired(Projectile spawnedProjectile)
      {
        spawnedProjectile.OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleEnemyHit);
        this.m_gun.OverrideAnimations = false;
        if (this.m_extantStars.Count >= 3 || !(this.m_gun.chargeAnimation != this.CachedChargeAnimation))
          return;
        this.m_gun.shootAnimation = this.CachedFireAnimation;
        this.m_gun.chargeAnimation = this.CachedChargeAnimation;
      }

      private void RemoveAllStars()
      {
        if ((bool) (UnityEngine.Object) this.m_cachedPlayer && GameUIRoot.HasInstance)
        {
          GameUIAmmoController controllerForPlayerId = GameUIRoot.Instance.GetAmmoControllerForPlayerID(this.m_cachedPlayer.PlayerIDX);
          for (int index = this.m_extantStars.Count - 1; index >= 0; --index)
            controllerForPlayerId.DeregisterAdditionalSprite(this.m_extantStars[index]);
        }
        this.m_extantStars.Clear();
      }

      private void HandleEnemyHit(
        Projectile sourceProjectile,
        SpeculativeRigidbody hitRigidbody,
        bool fatal)
      {
        if (!(this.m_gun.CurrentOwner is PlayerController) || !fatal || this.m_extantStars.Count >= 3)
          return;
        this.m_extantStars.Add(GameUIRoot.Instance.GetAmmoControllerForPlayerID((this.m_gun.CurrentOwner as PlayerController).PlayerIDX).RegisterNewAdditionalSprite(this.UIStarSpriteName));
        if (this.m_extantStars.Count < 3)
          return;
        this.m_gun.chargeAnimation = this.OverrideChargeAnimation;
      }
    }

}
