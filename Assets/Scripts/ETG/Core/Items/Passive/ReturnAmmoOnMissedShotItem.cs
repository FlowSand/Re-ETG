// Decompiled with JetBrains decompiler
// Type: ReturnAmmoOnMissedShotItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class ReturnAmmoOnMissedShotItem : PassiveItem, ILevelLoadedListener
    {
      public float ChanceToRegainAmmoOnMiss = 0.25f;
      public bool UsesZombieBulletsSynergy;
      public float SynergyChance = 0.5f;
      private PlayerController m_player;
      private Dictionary<float, int> m_slicesFired = new Dictionary<float, int>();

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        this.m_player = player;
        base.Pickup(player);
        player.PostProcessProjectile += new Action<Projectile, float>(this.PostProcessProjectile);
      }

      public void BraveOnLevelWasLoaded() => this.m_slicesFired.Clear();

      private void PostProcessProjectile(Projectile obj, float effectChanceScalar)
      {
        if ((double) obj.PlayerProjectileSourceGameTimeslice == -1.0)
          return;
        if (this.m_slicesFired.ContainsKey(obj.PlayerProjectileSourceGameTimeslice))
          this.m_slicesFired[obj.PlayerProjectileSourceGameTimeslice] = this.m_slicesFired[obj.PlayerProjectileSourceGameTimeslice] + 1;
        else
          this.m_slicesFired.Add(obj.PlayerProjectileSourceGameTimeslice, 1);
        obj.OnDestruction += new Action<Projectile>(this.HandleProjectileDestruction);
      }

      private void HandleProjectileDestruction(Projectile source)
      {
        if ((double) source.PlayerProjectileSourceGameTimeslice == -1.0 || !this.m_slicesFired.ContainsKey(source.PlayerProjectileSourceGameTimeslice) || !(bool) (UnityEngine.Object) this.m_player || !(bool) (UnityEngine.Object) source || !(bool) (UnityEngine.Object) source.PossibleSourceGun || source.PossibleSourceGun.InfiniteAmmo || source.HasImpactedEnemy)
          return;
        this.m_slicesFired[source.PlayerProjectileSourceGameTimeslice] = this.m_slicesFired[source.PlayerProjectileSourceGameTimeslice] - 1;
        if (this.m_slicesFired[source.PlayerProjectileSourceGameTimeslice] != 0)
          return;
        float num = this.ChanceToRegainAmmoOnMiss;
        if ((bool) (UnityEngine.Object) this.Owner && this.Owner.HasActiveBonusSynergy(CustomSynergyType.ZOMBIE_AMMO))
          num = this.SynergyChance;
        if ((double) UnityEngine.Random.value >= (double) num)
          return;
        source.PossibleSourceGun.GainAmmo(1);
      }

      public override DebrisObject Drop(PlayerController player)
      {
        DebrisObject debrisObject = base.Drop(player);
        this.m_player = (PlayerController) null;
        debrisObject.GetComponent<ReturnAmmoOnMissedShotItem>().m_pickedUpThisRun = true;
        player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
        return debrisObject;
      }

      protected override void OnDestroy()
      {
        base.OnDestroy();
        if (!(bool) (UnityEngine.Object) this.m_player)
          return;
        this.m_player.PostProcessProjectile -= new Action<Projectile, float>(this.PostProcessProjectile);
      }
    }

}
