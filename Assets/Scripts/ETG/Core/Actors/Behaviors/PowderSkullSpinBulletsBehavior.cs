using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class PowderSkullSpinBulletsBehavior : BehaviorBase
  {
    public string OverrideBulletName;
    public GameObject ShootPoint;
    public int NumBullets;
    public float BulletMinRadius;
    public float BulletMaxRadius;
    public int BulletCircleSpeed;
    public bool BulletsIgnoreTiles;
    public float RegenTimer;
    private readonly List<PowderSkullSpinBulletsBehavior.ProjectileContainer> m_projectiles = new List<PowderSkullSpinBulletsBehavior.ProjectileContainer>();
    private AIBulletBank m_bulletBank;
    private bool m_cachedCharm;
    private float m_regenTimer;

    public override void Start()
    {
      base.Start();
      this.m_bulletBank = this.m_gameObject.GetComponent<AIBulletBank>();
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_regenTimer);
    }

    public override BehaviorResult Update()
    {
      int num1 = (int) base.Update();
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      float num2 = float.MaxValue;
      if ((bool) (Object) this.m_aiActor && (bool) (Object) this.m_aiActor.TargetRigidbody)
        num2 = (this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox) - this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox)).magnitude;
      for (int index = 0; index < this.NumBullets; ++index)
      {
        float distFromCenter = Mathf.Lerp(this.BulletMinRadius, this.BulletMaxRadius, (float) index / ((float) this.NumBullets - 1f));
        if ((double) distFromCenter * 2.0 > (double) num2)
        {
          this.m_projectiles.Add(new PowderSkullSpinBulletsBehavior.ProjectileContainer()
          {
            projectile = (Projectile) null,
            angle = 0.0f,
            distFromCenter = distFromCenter
          });
          this.m_projectiles.Add(new PowderSkullSpinBulletsBehavior.ProjectileContainer()
          {
            projectile = (Projectile) null,
            angle = 180f,
            distFromCenter = distFromCenter
          });
        }
        else
        {
          Projectile component1 = this.m_bulletBank.CreateProjectileFromBank(this.GetBulletPosition(0.0f, distFromCenter), 90f, this.OverrideBulletName).GetComponent<Projectile>();
          component1.specRigidbody.Velocity = Vector2.zero;
          component1.ManualControl = true;
          if (this.BulletsIgnoreTiles)
            component1.specRigidbody.CollideWithTileMap = false;
          this.m_projectiles.Add(new PowderSkullSpinBulletsBehavior.ProjectileContainer()
          {
            projectile = component1,
            angle = 0.0f,
            distFromCenter = distFromCenter
          });
          Projectile component2 = this.m_bulletBank.CreateProjectileFromBank(this.GetBulletPosition(180f, distFromCenter), -90f, this.OverrideBulletName).GetComponent<Projectile>();
          component2.specRigidbody.Velocity = Vector2.zero;
          component2.ManualControl = true;
          if (this.BulletsIgnoreTiles)
            component2.specRigidbody.CollideWithTileMap = false;
          this.m_projectiles.Add(new PowderSkullSpinBulletsBehavior.ProjectileContainer()
          {
            projectile = component2,
            angle = 180f,
            distFromCenter = distFromCenter
          });
        }
      }
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuousInClass;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      if ((bool) (Object) this.m_aiActor)
      {
        bool flag = this.m_aiActor.CanTargetEnemies && !this.m_aiActor.CanTargetPlayers;
        if (this.m_cachedCharm != flag)
        {
          for (int index = 0; index < this.m_projectiles.Count; ++index)
          {
            if (this.m_projectiles[index] != null && (bool) (Object) this.m_projectiles[index].projectile && this.m_projectiles[index].projectile.gameObject.activeSelf)
            {
              this.m_projectiles[index].projectile.DieInAir(allowActorSpawns: false);
              this.m_projectiles[index].projectile = (Projectile) null;
            }
          }
          this.m_cachedCharm = flag;
        }
      }
      for (int index = 0; index < this.m_projectiles.Count; ++index)
      {
        if (!(bool) (Object) this.m_projectiles[index].projectile || !this.m_projectiles[index].projectile.gameObject.activeSelf)
          this.m_projectiles[index].projectile = (Projectile) null;
      }
      for (int index = 0; index < this.m_projectiles.Count; ++index)
      {
        float angle = this.m_projectiles[index].angle + this.m_deltaTime * (float) this.BulletCircleSpeed;
        this.m_projectiles[index].angle = angle;
        Projectile projectile = this.m_projectiles[index].projectile;
        if ((bool) (Object) projectile)
        {
          Vector2 bulletPosition = this.GetBulletPosition(angle, this.m_projectiles[index].distFromCenter);
          projectile.specRigidbody.Velocity = (bulletPosition - (Vector2) projectile.transform.position) / BraveTime.DeltaTime;
          if (projectile.shouldRotate)
            projectile.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180f + (Quaternion.Euler(0.0f, 0.0f, 90f) * (Vector3) (this.ShootPoint.transform.position.XY() - bulletPosition)).XY().ToAngle());
          projectile.ResetDistance();
        }
        else if ((double) this.m_regenTimer <= 0.0)
        {
          Vector2 bulletPosition = this.GetBulletPosition(this.m_projectiles[index].angle, this.m_projectiles[index].distFromCenter);
          if (GameManager.Instance.Dungeon.CellExists(bulletPosition) && !GameManager.Instance.Dungeon.data.isWall((int) bulletPosition.x, (int) bulletPosition.y))
          {
            Projectile component = this.m_bulletBank.CreateProjectileFromBank(bulletPosition, 0.0f, this.OverrideBulletName).GetComponent<Projectile>();
            component.specRigidbody.Velocity = Vector2.zero;
            component.ManualControl = true;
            if (this.BulletsIgnoreTiles)
              component.specRigidbody.CollideWithTileMap = false;
            this.m_projectiles[index].projectile = component;
            this.m_regenTimer = this.RegenTimer;
          }
        }
      }
      for (int index = 0; index < this.m_projectiles.Count; ++index)
      {
        if (this.m_projectiles[index] != null && (bool) (Object) this.m_projectiles[index].projectile)
        {
          bool flag = (bool) (Object) this.m_aiActor && this.m_aiActor.CanTargetEnemies;
          this.m_projectiles[index].projectile.collidesWithEnemies = this.m_projectiles[index].projectile.collidesWithEnemies || flag;
        }
      }
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      this.DestroyProjectiles();
      this.m_updateEveryFrame = false;
    }

    public override void OnActorPreDeath()
    {
      base.OnActorPreDeath();
      this.DestroyProjectiles();
    }

    public override void Destroy() => base.Destroy();

    private Vector2 GetBulletPosition(float angle, float distFromCenter)
    {
      return this.ShootPoint.transform.position.XY() + BraveMathCollege.DegreesToVector(angle, distFromCenter);
    }

    private void DestroyProjectiles()
    {
      for (int index = 0; index < this.m_projectiles.Count; ++index)
      {
        Projectile projectile = this.m_projectiles[index].projectile;
        if ((Object) projectile != (Object) null)
          projectile.DieInAir();
      }
      this.m_projectiles.Clear();
    }

    private class ProjectileContainer
    {
      public Projectile projectile;
      public float angle;
      public float distFromCenter;
    }
  }

