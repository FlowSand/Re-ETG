// Decompiled with JetBrains decompiler
// Type: CartTurretController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;

#nullable disable

public class CartTurretController : BraveBehaviour
  {
    public float AwakeTimer = 3f;
    public float TimeBetweenShots = 0.2f;
    public float ReacquisitonTimer = 1f;
    public float TrackingPercentage;
    public Transform BarrelTransform;
    private bool m_active;
    private RoomHandler m_parentRoom;
    private PlayerController m_targetPlayer;
    private float m_awakeTimer;
    private float m_acquisitionTimer;
    private float m_fireTimer;

    public bool Inactive => !this.m_active;

    private void Start()
    {
      this.m_parentRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY());
      this.m_parentRoom.Entered += new RoomHandler.OnEnteredEventHandler(this.Activate);
    }

    private void Activate(PlayerController p)
    {
      if (this.m_active || !this.m_parentRoom.HasActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear))
        return;
      this.m_active = true;
      this.m_awakeTimer = this.AwakeTimer;
      this.m_parentRoom.OnEnemiesCleared += new System.Action(this.HandleEnemiesCleared);
    }

    private void HandleEnemiesCleared()
    {
      this.m_active = false;
      this.m_parentRoom.OnEnemiesCleared -= new System.Action(this.HandleEnemiesCleared);
    }

    private void Update()
    {
      if (!this.m_active)
        return;
      this.m_awakeTimer -= BraveTime.DeltaTime;
      if ((double) this.m_awakeTimer > 0.0)
        return;
      this.m_acquisitionTimer -= BraveTime.DeltaTime;
      if ((UnityEngine.Object) this.m_targetPlayer != (UnityEngine.Object) null && this.m_targetPlayer.healthHaver.IsDead)
        this.m_targetPlayer = (PlayerController) null;
      if ((UnityEngine.Object) this.m_targetPlayer == (UnityEngine.Object) null || (double) this.m_acquisitionTimer <= 0.0)
        this.AcquireTarget();
      if (!((UnityEngine.Object) this.m_targetPlayer != (UnityEngine.Object) null))
        return;
      this.FaceTarget();
      this.Fire();
    }

    private void Fire()
    {
      this.m_fireTimer -= BraveTime.DeltaTime;
      if ((double) this.m_fireTimer > 0.0)
        return;
      float angle = (this.m_targetPlayer.CenterPosition - this.transform.position.XY()).ToAngle();
      if (float.IsNaN(angle) || float.IsInfinity(angle))
        return;
      this.FireBullet(this.BarrelTransform, angle, "default");
      this.m_fireTimer = this.TimeBetweenShots;
    }

    private void FaceTarget()
    {
      float num = BraveMathCollege.Atan2Degrees(this.m_targetPlayer.CenterPosition - this.transform.position.XY());
      if (float.IsNaN(num) || float.IsInfinity(num))
        return;
      this.transform.rotation = Quaternion.Euler(0.0f, 0.0f, num);
    }

    private void AcquireTarget()
    {
      this.m_targetPlayer = GameManager.Instance.GetActivePlayerClosestToPoint(this.transform.position.XY());
      this.m_acquisitionTimer = this.ReacquisitonTimer;
    }

    private void FireBullet(Transform shootPoint, float dir, string bulletType)
    {
      Projectile component = this.bulletBank.CreateProjectileFromBank((Vector2) shootPoint.position, dir, bulletType).GetComponent<Projectile>();
      component.Shooter = this.specRigidbody;
      component.specRigidbody.RegisterSpecificCollisionException(this.specRigidbody);
      component.OwnerName = StringTableManager.GetEnemiesString("#TRAP");
    }

    private Vector2 FindPredictedTargetPosition(string bulletName)
    {
      AIBulletBank.Entry bulletEntry = this.GetBulletEntry(bulletName);
      float firingSpeed = !bulletEntry.OverrideProjectile ? bulletEntry.BulletObject.GetComponent<Projectile>().baseData.speed : bulletEntry.ProjectileData.speed;
      if ((double) firingSpeed < 0.0)
        firingSpeed = float.MaxValue;
      return BraveMathCollege.GetPredictedPosition(this.m_targetPlayer.specRigidbody.GetUnitCenter(ColliderType.HitBox), this.m_targetPlayer.AverageVelocity, this.m_targetPlayer.specRigidbody.UnitCenter, firingSpeed);
    }

    public AIBulletBank.Entry GetBulletEntry(string bulletName)
    {
      if (string.IsNullOrEmpty(bulletName))
        return (AIBulletBank.Entry) null;
      AIBulletBank.Entry bulletEntry = this.bulletBank.Bullets.Find((Predicate<AIBulletBank.Entry>) (b => b.Name == bulletName));
      if (bulletEntry != null)
        return bulletEntry;
      Debug.LogError((object) $"Unknown bullet type {this.transform.name} on {bulletName}", (UnityEngine.Object) this.gameObject);
      return (AIBulletBank.Entry) null;
    }
  }

