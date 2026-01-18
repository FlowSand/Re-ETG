// Decompiled with JetBrains decompiler
// Type: FireAdditionalProjectileOnDeathSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

public class FireAdditionalProjectileOnDeathSynergyProcessor : MonoBehaviour
  {
    [LongNumericEnum]
    public CustomSynergyType SynergyToCheck;
    public Projectile ProjectileToFire;
    public FireAdditionalProjectileOnDeathSynergyProcessor.ProjectileSource Source;
    private Projectile m_projectile;

    private void Awake()
    {
      this.m_projectile = this.GetComponent<Projectile>();
      this.m_projectile.OnDestruction += new Action<Projectile>(this.HandleDestruction);
    }

    private void HandleDestruction(Projectile obj)
    {
      if (!(this.m_projectile.Owner is PlayerController) || !(this.m_projectile.Owner as PlayerController).HasActiveBonusSynergy(this.SynergyToCheck) || this.Source != FireAdditionalProjectileOnDeathSynergyProcessor.ProjectileSource.GUN_THROUGH_CURRENT || !(bool) (UnityEngine.Object) this.m_projectile || !(bool) (UnityEngine.Object) this.m_projectile.specRigidbody || !(bool) (UnityEngine.Object) this.m_projectile.PossibleSourceGun || !this.m_projectile.PossibleSourceGun.gameObject.activeSelf || this.m_projectile.specRigidbody.UnitCenter.GetAbsoluteRoom() != (this.m_projectile.Owner as PlayerController).CurrentRoom)
        return;
      Projectile component = SpawnManager.SpawnProjectile(this.ProjectileToFire.gameObject, this.m_projectile.PossibleSourceGun.barrelOffset.position, Quaternion.Euler(0.0f, 0.0f, (this.m_projectile.specRigidbody.UnitCenter - this.m_projectile.PossibleSourceGun.barrelOffset.PositionVector2()).ToAngle())).GetComponent<Projectile>();
      component.Owner = this.m_projectile.Owner;
      component.Shooter = this.m_projectile.Shooter;
      component.collidesWithPlayer = false;
      if (!(bool) (UnityEngine.Object) component)
        return;
      component.SpawnedFromOtherPlayerProjectile = true;
    }

    public enum ProjectileSource
    {
      GUN_THROUGH_CURRENT,
    }
  }

