// Decompiled with JetBrains decompiler
// Type: GunHandController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Core.Framework
{
    public class GunHandController : BraveBehaviour
    {
      [PickupIdentifier]
      [Header("Gun")]
      public int GunId = -1;
      public Projectile OverrideProjectile;
      public bool UsesOverrideProjectileData;
      public ProjectileData OverrideProjectileData;
      public GunHandController GunFlipMaster;
      [Header("Hands")]
      public PlayerHandController handObject;
      public bool isEightWay;
      public GunHandController.DirectionalAnimationBoolSixWay gunBehindBody;
      public GunHandController.DirectionalAnimationBoolEightWay gunBehindBodyEight;
      [Header("Shooting")]
      public float PreFireDelay;
      public int NumShots;
      public float ShotCooldown;
      public float Cooldown;
      public bool RampBullets;
      [ShowInInspectorIf("RampBullets", false)]
      public float RampStartHeight = 2f;
      [ShowInInspectorIf("RampBullets", false)]
      public float RampTime = 1f;
      private AIActor m_body;
      private Gun m_gun;
      private ProjectileData m_overrideProjectileData;
      private Transform m_gunAttachPoint;
      private float m_gunAngle;
      private bool m_gunFlipped;
      private bool m_isFiring;
      private int m_shotsFired;
      private float m_shotCooldown;
      private float m_cooldown;
      private Vector2 m_targetLocation;
      private List<PlayerHandController> m_attachedHands = new List<PlayerHandController>();

      public Gun Gun => this.m_gun;

      public void Start()
      {
        this.m_body = this.transform.parent.GetComponent<AIActor>();
        Transform transform = new GameObject("gun").transform;
        transform.parent = this.transform;
        transform.localPosition = Vector3.zero;
        this.m_gun = UnityEngine.Object.Instantiate<PickupObject>(PickupObjectDatabase.GetById(this.GunId)) as Gun;
        this.m_gun.transform.parent = transform;
        this.m_gun.NoOwnerOverride = true;
        this.m_gun.Initialize((GameActor) this.m_body);
        this.m_gun.gameObject.SetActive(true);
        this.m_gun.sprite.HeightOffGround = 0.05f;
        this.m_body.sprite.AttachRenderer(this.m_gun.sprite);
        if ((bool) (UnityEngine.Object) this.handObject && (bool) (UnityEngine.Object) this.m_gun)
          this.m_body.healthHaver.RegisterBodySprite(this.AttachNewHandToTransform(this.m_gun.PrimaryHandAttachPoint).sprite);
        if ((bool) (UnityEngine.Object) this.OverrideProjectile)
          this.m_gun.DefaultModule.projectiles = new List<Projectile>()
          {
            this.OverrideProjectile
          };
        if (this.UsesOverrideProjectileData)
          this.m_overrideProjectileData = this.OverrideProjectileData;
        else
          this.m_overrideProjectileData = new ProjectileData(this.m_gun.singleModule.projectiles[0].baseData)
          {
            damage = 0.5f
          };
        this.m_gun.ammo = int.MaxValue;
        this.m_gun.DefaultModule.numberOfShotsInClip = 0;
        if (this.RampBullets)
        {
          this.m_gun.rampBullets = true;
          this.m_gun.rampStartHeight = this.RampStartHeight;
          this.m_gun.rampTime = this.RampTime;
        }
        SpriteOutlineManager.AddOutlineToSprite(this.m_gun.sprite, Color.black, 0.35f);
        this.m_cooldown = this.Cooldown;
      }

      public void Update()
      {
        float facingDirection = this.m_body.aiAnimator.FacingDirection;
        this.m_gun.sprite.HeightOffGround = !(!this.isEightWay ? this.gunBehindBody.IsBehindBody(facingDirection) : this.gunBehindBodyEight.IsBehindBody(facingDirection)) ? 0.1f : -0.2f;
        if ((bool) (UnityEngine.Object) this.m_body.TargetRigidbody)
          this.m_targetLocation = this.m_body.aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox);
        this.m_gunAngle = this.m_gun.HandleAimRotation((Vector3) this.m_targetLocation);
        if ((bool) (UnityEngine.Object) this.GunFlipMaster)
        {
          if (this.GunFlipMaster.m_gunFlipped != this.m_gunFlipped)
          {
            this.m_gun.HandleSpriteFlip(this.GunFlipMaster.m_gunFlipped);
            this.m_gunFlipped = this.GunFlipMaster.m_gunFlipped;
          }
        }
        else if (!this.m_gunFlipped && (double) Mathf.Abs(this.m_gunAngle) > 105.0)
        {
          this.m_gun.HandleSpriteFlip(true);
          this.m_gunFlipped = true;
        }
        else if (this.m_gunFlipped && (double) Mathf.Abs(this.m_gunAngle) < 75.0)
        {
          this.m_gun.HandleSpriteFlip(false);
          this.m_gunFlipped = false;
        }
        if (this.m_isFiring)
        {
          this.m_shotCooldown -= BraveTime.DeltaTime;
          if ((double) this.m_shotCooldown > 0.0)
            return;
          this.Fire();
          this.m_shotCooldown = this.ShotCooldown;
          if (this.m_shotsFired < this.NumShots)
            return;
          this.CeaseAttack();
          this.m_isFiring = false;
        }
        else
          this.m_cooldown = Mathf.Max(0.0f, this.m_cooldown - BraveTime.DeltaTime);
      }

      protected override void OnDestroy() => base.OnDestroy();

      public void StartFiring()
      {
        this.m_isFiring = true;
        this.m_shotsFired = 0;
        if ((double) this.PreFireDelay > 0.0)
        {
          this.m_shotCooldown = this.PreFireDelay;
        }
        else
        {
          this.Fire();
          this.m_shotCooldown = this.ShotCooldown;
        }
      }

      public void CeaseAttack()
      {
        if ((bool) (UnityEngine.Object) this.m_gun)
          this.m_gun.CeaseAttack();
        this.m_cooldown = this.Cooldown;
      }

      public bool IsReady => !this.m_isFiring && (double) this.m_cooldown <= 0.0;

      private void Fire(float? angleOffset = null)
      {
        if (!(bool) (UnityEngine.Object) this.m_gun)
          return;
        if (angleOffset.HasValue)
        {
          this.m_gun.DefaultModule.angleFromAim = angleOffset.Value;
          this.m_gun.DefaultModule.angleVariance = 0.0f;
          this.m_gun.DefaultModule.alternateAngle = false;
        }
        this.m_gun.CeaseAttack();
        this.m_gun.ClearCooldowns();
        this.m_gun.ClearReloadData();
        int num = (int) this.m_gun.Attack(this.m_overrideProjectileData);
        ++this.m_shotsFired;
      }

      private PlayerHandController AttachNewHandToTransform(Transform target)
      {
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.handObject.gameObject);
        gameObject.transform.parent = this.transform;
        PlayerHandController component = gameObject.GetComponent<PlayerHandController>();
        this.m_gun.GetSprite().AttachRenderer(component.sprite);
        component.attachPoint = target;
        this.m_attachedHands.Add(component);
        if ((bool) (UnityEngine.Object) this.healthHaver)
          this.healthHaver.RegisterBodySprite((tk2dBaseSprite) component.GetComponent<tk2dSprite>());
        return component;
      }

      [Serializable]
      public class DirectionalAnimationBoolSixWay
      {
        public bool Back;
        public bool BackRight;
        public bool ForwardRight;
        public bool Forward;
        public bool ForwardLeft;
        public bool BackLeft;

        public bool IsBehindBody(float angle)
        {
          if ((double) angle <= 155.0 && (double) angle >= 25.0)
          {
            if ((double) angle < 120.0 && (double) angle >= 60.0)
              return this.Back;
            return (double) Mathf.Abs(angle) < 90.0 ? this.BackLeft : this.BackRight;
          }
          if ((double) angle <= -60.0 && (double) angle >= -120.0)
            return this.Forward;
          return (double) Mathf.Abs(angle) >= 90.0 ? this.ForwardLeft : this.ForwardRight;
        }
      }

      [Serializable]
      public class DirectionalAnimationBoolEightWay
      {
        public bool North;
        public bool NorthEast;
        public bool East;
        public bool SouthEast;
        public bool South;
        public bool SouthWest;
        public bool West;
        public bool NorthWest;

        public bool IsBehindBody(float angle)
        {
          angle = BraveMathCollege.ClampAngle360(angle);
          if ((double) angle < 22.5)
            return this.East;
          if ((double) angle < 67.5)
            return this.NorthEast;
          if ((double) angle < 112.5)
            return this.North;
          if ((double) angle < 157.5)
            return this.NorthWest;
          if ((double) angle < 202.5)
            return this.West;
          if ((double) angle < 247.5)
            return this.SouthWest;
          if ((double) angle < 292.5)
            return this.South;
          return (double) angle < 337.5 ? this.SouthEast : this.East;
        }
      }
    }

}
