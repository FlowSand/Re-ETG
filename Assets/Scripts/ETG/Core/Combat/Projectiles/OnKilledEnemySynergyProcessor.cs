// Decompiled with JetBrains decompiler
// Type: OnKilledEnemySynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class OnKilledEnemySynergyProcessor : MonoBehaviour
    {
      [LongNumericEnum]
      public CustomSynergyType SynergyToCheck;
      public bool DoesRadialBurst;
      public RadialBurstInterface RadialBurst;
      public bool DoesRadialSlow;
      public RadialSlowInterface RadialSlow;
      public bool UsesCooldown;
      public float Cooldown;
      public bool AddsDroppedCurrency;
      public int MinCurrency;
      public int MaxCurrency = 5;
      public bool TriggersEvenOnJustDamagedEnemy;
      public bool SpawnsObject;
      public GameObject ObjectToSpawn;
      private static float m_lastActiveTime;
      private Projectile m_projectile;

      private void Awake()
      {
        this.m_projectile = this.GetComponent<Projectile>();
        this.m_projectile.OnHitEnemy += new Action<Projectile, SpeculativeRigidbody, bool>(this.HandleHitEnemy);
      }

      private void HandleHitEnemy(
        Projectile sourceProjectile,
        SpeculativeRigidbody hitRigidbody,
        bool killed)
      {
        if (this.UsesCooldown && (double) UnityEngine.Time.time - (double) OnKilledEnemySynergyProcessor.m_lastActiveTime <= (double) this.Cooldown || !killed && !this.TriggersEvenOnJustDamagedEnemy || !(bool) (UnityEngine.Object) hitRigidbody || !(bool) (UnityEngine.Object) this.m_projectile.PossibleSourceGun || !this.m_projectile.PossibleSourceGun.OwnerHasSynergy(this.SynergyToCheck))
          return;
        if (this.UsesCooldown)
          OnKilledEnemySynergyProcessor.m_lastActiveTime = UnityEngine.Time.time;
        if (this.DoesRadialBurst)
          this.RadialBurst.DoBurst(this.m_projectile.PossibleSourceGun.CurrentOwner as PlayerController, new Vector2?(hitRigidbody.UnitCenter));
        if (this.DoesRadialSlow)
          this.RadialSlow.DoRadialSlow(hitRigidbody.UnitCenter, hitRigidbody.UnitCenter.GetAbsoluteRoom());
        if (this.AddsDroppedCurrency)
          LootEngine.SpawnCurrency(hitRigidbody.UnitCenter, UnityEngine.Random.Range(this.MinCurrency, this.MaxCurrency + 1));
        if (!this.SpawnsObject)
          return;
        UnityEngine.Object.Instantiate<GameObject>(this.ObjectToSpawn, (Vector3) hitRigidbody.UnitCenter, Quaternion.identity);
      }
    }

}
