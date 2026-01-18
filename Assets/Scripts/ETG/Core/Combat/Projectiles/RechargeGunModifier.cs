using System;
using UnityEngine;

#nullable disable

public class RechargeGunModifier : MonoBehaviour
  {
    public float MaxDamageMultiplierPerStack = 0.1f;
    public float MinDamageMultiplierPerStack = 0.05f;
    public float MultiplierCap = 4f;
    public int StackFalloffPoint = 5;
    public RechargeGunProjectileTier[] Projectiles;
    private Gun m_gun;
    private bool m_callbackInitialized;
    private PlayerController m_cachedPlayer;
    private int m_counter = 1;
    private bool m_wasReloading;

    public void Start()
    {
      this.m_gun = this.GetComponent<Gun>();
      this.m_gun.OnPreFireProjectileModifier += new Func<Gun, Projectile, ProjectileModule, Projectile>(this.HandleReplaceProjectile);
      this.m_gun.PostProcessProjectile += new Action<Projectile>(this.PostProcessProjectile);
      this.m_gun.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.HandleReloadPressed);
    }

    private Projectile HandleReplaceProjectile(Gun arg1, Projectile arg2, ProjectileModule arg3)
    {
      Projectile projectile = arg2;
      int num = 0;
      for (int index = 0; index < this.Projectiles.Length; ++index)
      {
        if (this.m_counter > this.Projectiles[index].RequiredStacks && num <= this.Projectiles[index].RequiredStacks)
        {
          num = this.Projectiles[index].RequiredStacks;
          projectile = this.Projectiles[index].ReplacementProjectile;
        }
      }
      return projectile;
    }

    private void PostProcessProjectile(Projectile obj)
    {
      float num = Mathf.Min(this.MultiplierCap, (float) (1.0 + (double) this.MaxDamageMultiplierPerStack * (double) Mathf.Min(this.StackFalloffPoint, this.m_counter) + (double) this.MinDamageMultiplierPerStack * (double) Mathf.Max(0, this.m_counter - this.StackFalloffPoint)));
      obj.baseData.damage *= num;
    }

    private void Update()
    {
      if (!this.m_callbackInitialized && this.m_gun.CurrentOwner is PlayerController)
      {
        this.m_callbackInitialized = true;
        this.m_cachedPlayer = this.m_gun.CurrentOwner as PlayerController;
        this.m_cachedPlayer.OnTriedToInitiateAttack += new Action<PlayerController>(this.HandleTriedToInitiateAttack);
      }
      else if (this.m_callbackInitialized && !(this.m_gun.CurrentOwner is PlayerController))
      {
        this.m_callbackInitialized = false;
        if ((bool) (UnityEngine.Object) this.m_cachedPlayer)
        {
          this.m_cachedPlayer.OnTriedToInitiateAttack -= new Action<PlayerController>(this.HandleTriedToInitiateAttack);
          this.m_cachedPlayer = (PlayerController) null;
        }
      }
      if (!this.m_wasReloading || !(bool) (UnityEngine.Object) this.m_gun || this.m_gun.IsReloading)
        return;
      this.m_wasReloading = false;
    }

    private void HandleTriedToInitiateAttack(PlayerController sourcePlayer)
    {
    }

    private void HandleReloadPressed(PlayerController ownerPlayer, Gun sourceGun, bool something)
    {
      if (sourceGun.IsReloading)
      {
        if (!this.m_wasReloading)
        {
          this.m_counter = 0;
          this.m_wasReloading = true;
        }
        ++this.m_counter;
        int num = (int) AkSoundEngine.PostEvent("Play_WPN_RechargeGun_Recharge_01", this.gameObject);
      }
      else
        this.m_wasReloading = false;
    }
  }

