// Decompiled with JetBrains decompiler
// Type: RandomProjectileReplacementItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class RandomProjectileReplacementItem : PassiveItem
    {
      public float ChancePerSecondToTrigger = 0.01f;
      public Projectile ReplacementProjectile;
      public string ReplacementAudioEvent;
      private PlayerController m_player;

      public override void Pickup(PlayerController player)
      {
        if (this.m_pickedUp)
          return;
        this.m_player = player;
        base.Pickup(player);
        player.OnPreFireProjectileModifier += new Func<Gun, Projectile, Projectile>(this.HandlePreFireProjectileModification);
      }

      private Projectile HandlePreFireProjectileModification(Gun sourceGun, Projectile sourceProjectile)
      {
        if ((bool) (UnityEngine.Object) sourceGun && sourceGun.IsHeroSword || sourceGun.MovesPlayerForwardOnChargeFire)
          return sourceProjectile;
        float num1 = 1f / sourceGun.DefaultModule.cooldownTime;
        if ((UnityEngine.Object) sourceGun.Volley != (UnityEngine.Object) null)
        {
          float num2 = 0.0f;
          for (int index = 0; index < sourceGun.Volley.projectiles.Count; ++index)
          {
            ProjectileModule projectile = sourceGun.Volley.projectiles[index];
            num2 += projectile.GetEstimatedShotsPerSecond(sourceGun.reloadTime);
          }
          if ((double) num2 > 0.0)
            num1 = num2;
        }
        if ((double) UnityEngine.Random.value > (double) Mathf.Max(0.0001f, Mathf.Clamp01(this.ChancePerSecondToTrigger / num1)))
          return sourceProjectile;
        if (!string.IsNullOrEmpty(this.ReplacementAudioEvent))
        {
          int num3 = (int) AkSoundEngine.PostEvent(this.ReplacementAudioEvent, this.gameObject);
        }
        return this.ReplacementProjectile;
      }

      public override DebrisObject Drop(PlayerController player)
      {
        DebrisObject debrisObject = base.Drop(player);
        this.m_player = (PlayerController) null;
        debrisObject.GetComponent<RandomProjectileReplacementItem>().m_pickedUpThisRun = true;
        player.OnPreFireProjectileModifier -= new Func<Gun, Projectile, Projectile>(this.HandlePreFireProjectileModification);
        return debrisObject;
      }

      protected override void OnDestroy()
      {
        base.OnDestroy();
        if (!(bool) (UnityEngine.Object) this.m_player)
          return;
        this.m_player.OnPreFireProjectileModifier -= new Func<Gun, Projectile, Projectile>(this.HandlePreFireProjectileModification);
      }
    }

}
