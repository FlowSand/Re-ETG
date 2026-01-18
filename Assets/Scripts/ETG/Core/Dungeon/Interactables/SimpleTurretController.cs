// Decompiled with JetBrains decompiler
// Type: SimpleTurretController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using Dungeonator;
using System;
using UnityEngine;

#nullable disable

public class SimpleTurretController : DungeonPlaceableBehaviour
  {
    public bool ControlledByPlaymaker = true;
    public float AwakeTimer = 3f;
    public float TimeBetweenShots = 0.2f;
    public Vector2 ShootDirection;
    public Transform BarrelTransform;
    private bool m_active;
    private RoomHandler m_parentRoom;
    private AIBulletBank m_bank;
    private float m_awakeTimer;
    private float m_fireTimer;

    public bool Inactive => !this.m_active;

    private void Start()
    {
      if (!(bool) (UnityEngine.Object) this.specRigidbody)
        this.specRigidbody = this.GetComponentInChildren<SpeculativeRigidbody>();
      this.m_bank = this.bulletBank;
      if (!(bool) (UnityEngine.Object) this.bulletBank)
        this.m_bank = this.GetComponentInChildren<AIBulletBank>();
      this.m_parentRoom = GameManager.Instance.Dungeon.data.GetAbsoluteRoomFromPosition(this.transform.position.IntXY());
      if (this.ControlledByPlaymaker)
        return;
      this.m_parentRoom.Entered += new RoomHandler.OnEnteredEventHandler(this.Activate);
    }

    public void DeactivateManual() => this.m_active = false;

    public void ActivateManual()
    {
      if (this.m_active)
        return;
      this.m_active = true;
      this.m_awakeTimer = this.AwakeTimer;
    }

    private void Activate(PlayerController p) => this.ActivateManual();

    private void Update()
    {
      if (!this.m_active)
        return;
      if (!this.ControlledByPlaymaker && !GameManager.Instance.IsAnyPlayerInRoom(this.m_parentRoom))
      {
        this.m_active = false;
      }
      else
      {
        this.m_awakeTimer -= BraveTime.DeltaTime;
        if ((double) this.m_awakeTimer > 0.0)
          return;
        this.Fire();
      }
    }

    private void Fire()
    {
      this.m_fireTimer -= BraveTime.DeltaTime;
      if ((double) this.m_fireTimer > 0.0)
        return;
      this.FireBullet(this.BarrelTransform, this.ShootDirection, "default");
      this.m_fireTimer = this.TimeBetweenShots;
    }

    private void FireBullet(Transform shootPoint, Vector2 dirVec, string bulletType)
    {
      Projectile component = this.m_bank.CreateProjectileFromBank((Vector2) shootPoint.position, BraveMathCollege.Atan2Degrees(dirVec.normalized), bulletType).GetComponent<Projectile>();
      component.Shooter = this.specRigidbody;
      component.specRigidbody.RegisterSpecificCollisionException(this.specRigidbody);
    }

    public AIBulletBank.Entry GetBulletEntry(string bulletName)
    {
      if (string.IsNullOrEmpty(bulletName))
        return (AIBulletBank.Entry) null;
      AIBulletBank.Entry bulletEntry = this.m_bank.Bullets.Find((Predicate<AIBulletBank.Entry>) (b => b.Name == bulletName));
      if (bulletEntry != null)
        return bulletEntry;
      Debug.LogError((object) $"Unknown bullet type {this.transform.name} on {bulletName}", (UnityEngine.Object) this.gameObject);
      return (AIBulletBank.Entry) null;
    }
  }

