// Decompiled with JetBrains decompiler
// Type: CrisisStoneItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

#nullable disable

namespace ETG.Core.Items.Passive
{
    public class CrisisStoneItem : PassiveItem
    {
      public string ReloadAudioEvent;
      public VFXPool ImpactVFX;
      public GameObject WallVFX;
      private bool m_hasPlayedAudioForOutOfAmmo;

      protected override void Update()
      {
        base.Update();
        if (!(bool) (UnityEngine.Object) this.m_owner || !(bool) (UnityEngine.Object) this.m_owner.CurrentGun)
          return;
        if (this.m_owner.CurrentGun.ClipShotsRemaining == 0 && !this.m_hasPlayedAudioForOutOfAmmo)
        {
          this.m_hasPlayedAudioForOutOfAmmo = true;
          int num = (int) AkSoundEngine.PostEvent(this.ReloadAudioEvent, this.gameObject);
        }
        else
        {
          if (!this.m_hasPlayedAudioForOutOfAmmo || this.m_owner.CurrentGun.ClipShotsRemaining <= 0)
            return;
          this.m_hasPlayedAudioForOutOfAmmo = false;
        }
      }

      public override void Pickup(PlayerController player)
      {
        base.Pickup(player);
        this.m_owner = player;
        player.healthHaver.ModifyDamage += new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.HandleDamageModification);
        player.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
        player.OnReloadedGun += new Action<PlayerController, Gun>(this.HandleReloadedGun);
      }

      private void HandleReloadedGun(PlayerController sourcePlayer, Gun sourceGun)
      {
        if ((bool) (UnityEngine.Object) sourceGun && sourceGun.IsHeroSword)
          return;
        sourcePlayer.StartCoroutine(this.HandleWallVFX(sourcePlayer, sourceGun));
        int num = (int) AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Shield_01", this.gameObject);
      }

      [DebuggerHidden]
      private IEnumerator HandleWallVFX(PlayerController sourcePlayer, Gun sourceGun)
      {
        // ISSUE: object of a compiler-generated type is created
        return (IEnumerator) new CrisisStoneItem.<HandleWallVFX>c__Iterator0()
        {
          sourcePlayer = sourcePlayer,
          sourceGun = sourceGun,
          _this = this
        };
      }

      private void HandleRigidbodyCollision(CollisionData rigidbodyCollision)
      {
        if (!(bool) (UnityEngine.Object) this.m_owner || !(bool) (UnityEngine.Object) this.m_owner.CurrentGun || !this.m_owner.CurrentGun.IsReloading || this.m_owner.CurrentGun.ClipShotsRemaining != 0 || this.m_owner.CurrentGun.IsHeroSword || !(bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody || !(bool) (UnityEngine.Object) rigidbodyCollision.OtherRigidbody.GetComponent<Projectile>())
          return;
        this.ImpactVFX.SpawnAtPosition((Vector3) rigidbodyCollision.Contact);
        int num = (int) AkSoundEngine.PostEvent("Play_ITM_Crisis_Stone_Impact_01", this.gameObject);
      }

      private void HandleDamageModification(HealthHaver source, HealthHaver.ModifyDamageEventArgs args)
      {
        if (args == EventArgs.Empty || (double) args.ModifiedDamage <= 0.0 || !source.IsVulnerable || !(bool) (UnityEngine.Object) this.m_owner || !(bool) (UnityEngine.Object) this.m_owner.CurrentGun || !this.m_owner.CurrentGun.IsReloading || this.m_owner.CurrentGun.ClipShotsRemaining != 0 || this.m_owner.CurrentGun.IsHeroSword)
          return;
        args.ModifiedDamage = 0.0f;
      }

      protected override void DisableEffect(PlayerController disablingPlayer)
      {
        if ((bool) (UnityEngine.Object) disablingPlayer)
          disablingPlayer.healthHaver.ModifyDamage -= new Action<HealthHaver, HealthHaver.ModifyDamageEventArgs>(this.HandleDamageModification);
        if ((bool) (UnityEngine.Object) disablingPlayer)
          disablingPlayer.specRigidbody.OnRigidbodyCollision -= new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
        if ((bool) (UnityEngine.Object) disablingPlayer)
          disablingPlayer.OnReloadedGun -= new Action<PlayerController, Gun>(this.HandleReloadedGun);
        base.DisableEffect(disablingPlayer);
      }
    }

}
