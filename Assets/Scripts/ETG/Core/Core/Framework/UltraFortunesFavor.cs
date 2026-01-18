using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class UltraFortunesFavor : BraveBehaviour
  {
    public GameObject sparkOctantVFX;
    public float vfxOffset = 0.625f;
    public float bulletRadius = 2f;
    public float bulletSpeedModifier = 0.8f;
    public float beamRadius = 2f;
    public float goopRadius = 2f;
    private readonly List<UltraFortunesFavor.ProjectileData> m_caughtBullets = new List<UltraFortunesFavor.ProjectileData>();
    private GameObject[] m_octantVfx = new GameObject[8];
    private PixelCollider m_bulletBlocker;
    private PixelCollider m_beamReflector;
    private Vector2 m_lastPosition;
    private int m_goopExceptionId = -1;
    private float m_enemyOverlapTimer;

    public Vector2 BulletCircleCenter => this.specRigidbody.GetUnitCenter(ColliderType.HitBox);

    public Vector2 BeamCircleCenter => this.specRigidbody.GetUnitCenter(ColliderType.HitBox);

    public Vector2 GoopCircleCenter => this.specRigidbody.GetUnitCenter(ColliderType.HitBox);

    public void Awake() => this.m_enemyOverlapTimer = UnityEngine.Random.Range(2f, 4f);

    public void Start()
    {
      this.specRigidbody.Initialize();
      if ((double) this.bulletRadius > 0.0)
      {
        IntVector2 pixel1 = PhysicsEngine.UnitToPixel(this.BulletCircleCenter - this.transform.position.XY());
        int pixel2 = PhysicsEngine.UnitToPixel(this.bulletRadius);
        this.m_bulletBlocker = new PixelCollider();
        this.m_bulletBlocker.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Circle;
        this.m_bulletBlocker.CollisionLayer = CollisionLayer.BulletBlocker;
        this.m_bulletBlocker.IsTrigger = true;
        this.m_bulletBlocker.ManualOffsetX = pixel1.x - pixel2;
        this.m_bulletBlocker.ManualOffsetY = pixel1.y - pixel2;
        this.m_bulletBlocker.ManualDiameter = pixel2 * 2;
        this.m_bulletBlocker.Regenerate(this.transform);
        this.specRigidbody.PixelColliders.Add(this.m_bulletBlocker);
        this.specRigidbody.OnTriggerCollision += new SpeculativeRigidbody.OnTriggerDelegate(this.OnTriggerCollision);
      }
      if ((double) this.beamRadius > 0.0)
      {
        IntVector2 pixel3 = PhysicsEngine.UnitToPixel(this.BeamCircleCenter - this.transform.position.XY());
        int pixel4 = PhysicsEngine.UnitToPixel(this.beamRadius);
        this.m_beamReflector = new PixelCollider();
        this.m_beamReflector.ColliderGenerationMode = PixelCollider.PixelColliderGeneration.Circle;
        this.m_beamReflector.CollisionLayer = CollisionLayer.BeamBlocker;
        this.m_beamReflector.IsTrigger = false;
        this.m_beamReflector.ManualOffsetX = pixel3.x - pixel4;
        this.m_beamReflector.ManualOffsetY = pixel3.y - pixel4;
        this.m_beamReflector.ManualDiameter = pixel4 * 2;
        this.m_beamReflector.Regenerate(this.transform);
        this.specRigidbody.PixelColliders.Add(this.m_beamReflector);
      }
      if ((double) this.bulletRadius > 0.0 || (double) this.beamRadius > 0.0)
        PhysicsEngine.UpdatePosition(this.specRigidbody);
      if ((double) this.goopRadius <= 0.0)
        return;
      this.m_goopExceptionId = DeadlyDeadlyGoopManager.RegisterUngoopableCircle(this.GoopCircleCenter, this.goopRadius);
      this.m_lastPosition = this.transform.position.XY();
    }

    public void Update()
    {
      for (int index = this.m_caughtBullets.Count - 1; index >= 0; --index)
      {
        UltraFortunesFavor.ProjectileData caughtBullet = this.m_caughtBullets[index];
        caughtBullet.positionDeg += BraveTime.DeltaTime * caughtBullet.degPerSecond;
        Projectile projectile = this.m_caughtBullets[index].projectile;
        if (!((UnityEngine.Object) projectile == (UnityEngine.Object) null))
        {
          if (!(bool) (UnityEngine.Object) projectile)
          {
            this.m_caughtBullets[index] = (UltraFortunesFavor.ProjectileData) null;
          }
          else
          {
            this.HitFromPoint(projectile.transform.position.XY());
            if ((double) Mathf.Abs(BraveMathCollege.ClampAngle180((this.BulletCircleCenter - projectile.transform.position.XY()).ToAngle() - this.m_caughtBullets[index].initialVelocity.ToAngle())) > 90.0)
            {
              Vector2 vector2 = (Vector2) (Quaternion.Euler(0.0f, 0.0f, -90f * Mathf.Sign(this.m_caughtBullets[index].degPerSecond)) * (Vector3) (this.BulletCircleCenter - projectile.transform.position.XY()));
              projectile.ManualControl = false;
              projectile.SendInDirection(this.m_caughtBullets[index].initialVelocity.magnitude * vector2.normalized, true);
              this.m_caughtBullets[index].projectile = (Projectile) null;
            }
            else
            {
              Vector2 bulletPosition = this.GetBulletPosition(caughtBullet.positionDeg);
              projectile.specRigidbody.Velocity = (bulletPosition - (Vector2) projectile.transform.position) / BraveTime.DeltaTime;
              if (projectile.shouldRotate)
                projectile.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 180f + (Quaternion.Euler(0.0f, 0.0f, 90f) * (Vector3) (this.BulletCircleCenter - bulletPosition)).XY().ToAngle());
            }
          }
        }
      }
      if ((double) this.goopRadius > 0.0)
      {
        Vector2 vector2 = this.transform.position.XY();
        if (vector2 != this.m_lastPosition)
        {
          DeadlyDeadlyGoopManager.UpdateUngoopableCircle(this.m_goopExceptionId, this.GoopCircleCenter, this.goopRadius);
          this.m_lastPosition = vector2;
        }
      }
      this.m_enemyOverlapTimer -= BraveTime.DeltaTime;
      if (!PhysicsEngine.HasInstance || (double) this.m_enemyOverlapTimer > 0.0)
        return;
      List<SpeculativeRigidbody> overlappingRigidbodies = PhysicsEngine.Instance.GetOverlappingRigidbodies(this.specRigidbody);
      for (int index = 0; index < overlappingRigidbodies.Count; ++index)
      {
        SpeculativeRigidbody specRigidbody = overlappingRigidbodies[index];
        if ((bool) (UnityEngine.Object) specRigidbody && (bool) (UnityEngine.Object) specRigidbody.aiActor)
        {
          this.specRigidbody.RegisterGhostCollisionException(specRigidbody);
          specRigidbody.RegisterGhostCollisionException(this.specRigidbody);
        }
      }
      this.m_enemyOverlapTimer = 2f;
    }

    protected override void OnDestroy()
    {
      if ((double) this.bulletRadius > 0.0)
        this.specRigidbody.OnTriggerCollision -= new SpeculativeRigidbody.OnTriggerDelegate(this.OnTriggerCollision);
      DeadlyDeadlyGoopManager.DeregisterUngoopableCircle(this.m_goopExceptionId);
      base.OnDestroy();
    }

    private void OnTriggerCollision(
      SpeculativeRigidbody specRigidbody,
      SpeculativeRigidbody sourceSpecRigidbody,
      CollisionData collisionData)
    {
      if (!this.enabled || collisionData.MyPixelCollider != this.m_bulletBlocker || !((UnityEngine.Object) collisionData.OtherRigidbody != (UnityEngine.Object) null) || !((UnityEngine.Object) collisionData.OtherRigidbody.projectile != (UnityEngine.Object) null))
        return;
      Projectile projectile = collisionData.OtherRigidbody.projectile;
      Vector2 vector = this.BulletCircleCenter - projectile.transform.position.XY();
      float f = BraveMathCollege.ClampAngle180(vector.ToAngle() - projectile.specRigidbody.Velocity.ToAngle());
      float positionDeg = BraveMathCollege.ClampAngle360((collisionData.Contact - this.BulletCircleCenter).ToAngle());
      float num = (float) ((double) Mathf.Sign(f) * (double) projectile.specRigidbody.Velocity.magnitude / (3.1415927410125732 * (double) this.bulletRadius) * 180.0);
      this.m_caughtBullets.Insert(Mathf.Max(0, this.m_caughtBullets.Count - 1), new UltraFortunesFavor.ProjectileData(projectile, positionDeg, projectile.specRigidbody.Velocity, num * this.bulletSpeedModifier));
      projectile.specRigidbody.Velocity = Vector2.zero;
      projectile.ManualControl = true;
      collisionData.OtherRigidbody.RegisterSpecificCollisionException(collisionData.MyRigidbody);
      this.HitFromDirection(-vector);
      if ((bool) (UnityEngine.Object) this.talkDoer && this.talkDoer.IsTalking)
        return;
      this.SendPlaymakerEvent("takePlayerDamage");
    }

    public Vector2 GetBeamNormal(Vector2 targetPoint)
    {
      return (targetPoint - this.BulletCircleCenter).normalized;
    }

    public void HitFromPoint(Vector2 targetPoint)
    {
      this.HitFromDirection(targetPoint - this.BulletCircleCenter);
    }

    private Vector2 GetBulletPosition(float angle)
    {
      return this.BulletCircleCenter + new Vector2(Mathf.Cos(angle * ((float) Math.PI / 180f)), Mathf.Sin(angle * ((float) Math.PI / 180f))) * this.bulletRadius;
    }

    private void HitFromDirection(Vector2 dir)
    {
      int octant = BraveMathCollege.VectorToOctant(dir);
      if ((bool) (UnityEngine.Object) this.m_octantVfx[octant])
        return;
      Vector3 offset = Quaternion.Euler(0.0f, 0.0f, (float) (-octant * 45 - 90)) * new Vector3(-this.vfxOffset, 0.0f, 0.0f);
      this.m_octantVfx[octant] = this.PlayEffectOnActor(this.sparkOctantVFX, offset, alreadyMiddleCenter: true);
      this.m_octantVfx[octant].transform.rotation = Quaternion.Euler(0.0f, 0.0f, (float) (-45 * octant - 45));
    }

    private GameObject PlayEffectOnActor(
      GameObject effect,
      Vector3 offset,
      bool attached = true,
      bool alreadyMiddleCenter = false)
    {
      GameObject gameObject = SpawnManager.SpawnVFX(effect, true);
      tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
      if (!alreadyMiddleCenter)
        component.PlaceAtPositionByAnchor(this.sprite.WorldCenter.ToVector3ZUp() + offset, tk2dBaseSprite.Anchor.MiddleCenter);
      else
        component.transform.position = this.sprite.WorldCenter.ToVector3ZUp() + offset;
      if (attached)
      {
        gameObject.transform.parent = this.transform;
        component.HeightOffGround = 0.2f;
        this.sprite.AttachRenderer(component);
      }
      return gameObject;
    }

    private class ProjectileData
    {
      public Projectile projectile;
      public float positionDeg;
      public Vector2 initialVelocity;
      public float degPerSecond;

      public ProjectileData(
        Projectile projectile,
        float positionDeg,
        Vector2 initialVelocity,
        float degPerSecond)
      {
        this.projectile = projectile;
        this.positionDeg = positionDeg;
        this.initialVelocity = initialVelocity;
        this.degPerSecond = degPerSecond;
      }
    }
  }

