// Decompiled with JetBrains decompiler
// Type: ShootProjectileOnGunfireDoer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class ShootProjectileOnGunfireDoer : BraveBehaviour, SingleSpawnableGunPlacedObject
    {
      [CheckAnimation(null)]
      public string inAnimation;
      [CheckAnimation(null)]
      public string fireAnimation;
      [CheckAnimation(null)]
      public string idleAnimation;
      [CheckAnimation(null)]
      public string outAnimation;
      public bool HasOverrideSynergy;
      [LongNumericEnum]
      public CustomSynergyType OverrideSynergy;
      public ProjectileVolleyData OverrideSynergyVolley;
      private Gun m_sourceGun;
      private PlayerController m_ownerPlayer;
      private bool m_isActive;
      private RoomHandler m_room;
      private bool m_firedThisFrame;
      private int m_lastFiredFrame = -1;

      public void Initialize(Gun sourceGun)
      {
        if (!(bool) (UnityEngine.Object) sourceGun || !(bool) (UnityEngine.Object) sourceGun.CurrentOwner || !(sourceGun.CurrentOwner is PlayerController))
          return;
        if (!string.IsNullOrEmpty(this.inAnimation))
          this.spriteAnimator.Play(this.inAnimation);
        this.m_isActive = true;
        this.m_sourceGun = sourceGun;
        this.m_sourceGun.PostProcessProjectile += new Action<Projectile>(this.HandleProjectileFired);
        this.m_room = this.transform.position.GetAbsoluteRoom();
        this.m_ownerPlayer = sourceGun.CurrentOwner as PlayerController;
      }

      private void Update()
      {
        if (!this.m_isActive)
          return;
        if (this.m_ownerPlayer.CurrentRoom != this.m_room)
          this.Deactivate();
        this.m_firedThisFrame = false;
      }

      public void Deactivate()
      {
        this.m_isActive = false;
        if ((bool) (UnityEngine.Object) this.m_sourceGun)
          this.m_sourceGun.PostProcessProjectile -= new Action<Projectile>(this.HandleProjectileFired);
        if (!(bool) (UnityEngine.Object) this)
          return;
        if (!string.IsNullOrEmpty(this.inAnimation))
          this.spriteAnimator.PlayAndDestroyObject(this.outAnimation);
        else
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }

      private void HandleProjectileFired(Projectile obj)
      {
        if (!(bool) (UnityEngine.Object) this || !(bool) (UnityEngine.Object) this.sprite)
        {
          this.Deactivate();
        }
        else
        {
          if (!this.m_isActive)
            return;
          if (!this.m_firedThisFrame)
            this.m_sourceGun.muzzleFlashEffects.SpawnAtPosition(this.sprite.WorldCenter.ToVector3ZUp());
          this.m_firedThisFrame = true;
          if (!string.IsNullOrEmpty(this.inAnimation) && !this.spriteAnimator.IsPlaying(this.fireAnimation))
            this.spriteAnimator.PlayForDuration(this.fireAnimation, -1f, this.idleAnimation);
          if (this.HasOverrideSynergy && this.m_ownerPlayer.HasActiveBonusSynergy(this.OverrideSynergy))
          {
            Vector2 worldCenter = this.sprite.WorldCenter;
            float nearestDistance = -1f;
            AIActor nearestEnemy = this.transform.position.GetAbsoluteRoom().GetNearestEnemy(worldCenter, out nearestDistance);
            if (!(bool) (UnityEngine.Object) nearestEnemy || this.m_lastFiredFrame == UnityEngine.Time.frameCount)
              return;
            this.m_lastFiredFrame = UnityEngine.Time.frameCount;
            VolleyUtility.FireVolley(this.OverrideSynergyVolley, worldCenter, nearestEnemy.CenterPosition - worldCenter, obj.Owner);
          }
          else
          {
            if (!(bool) (UnityEngine.Object) obj)
              return;
            Vector3 vector = obj.transform.position - this.m_sourceGun.barrelOffset.position;
            GameObject gameObject = SpawnManager.SpawnProjectile(obj.gameObject, (Vector3) (this.sprite.WorldCenter + vector.XY()), obj.transform.rotation);
            if (!(bool) (UnityEngine.Object) gameObject)
              return;
            Projectile component = gameObject.GetComponent<Projectile>();
            component.Owner = obj.Owner;
            component.Shooter = obj.Shooter;
            component.PossibleSourceGun = obj.PossibleSourceGun;
            component.collidesWithPlayer = false;
            component.collidesWithEnemies = true;
          }
        }
      }
    }

}
