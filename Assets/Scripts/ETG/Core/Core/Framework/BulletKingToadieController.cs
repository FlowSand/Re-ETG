using Dungeonator;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

public class BulletKingToadieController : BraveBehaviour
  {
    public bool ShouldCry;
    public bool CanReturnAngry;
    public float AutoCrazeTime = -1f;
    public float PostCrazeHealth = 1f;
    public tk2dSpriteAnimator scepterAnimator;
    public Transform hatPoint;
    public GameObject hatVfx;
    private bool m_isCrazed;
    private bool m_isCrying;
    private float m_timer;

    public AIActor MyKing { get; set; }

    public void Start()
    {
      this.specRigidbody.OnPreRigidbodyCollision += new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.PreRigidbodyCollision);
      this.healthHaver.OnDamaged += new HealthHaver.OnDamagedEvent(this.OnDamaged);
      if (this.ShouldCry)
        this.aiActor.PreventAutoKillOnBossDeath = true;
      if (this.CanReturnAngry)
        this.healthHaver.OnDeath += new Action<Vector2>(this.OnDeath);
      List<AIActor> activeEnemies = this.aiActor.ParentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
      for (int index = 0; index < activeEnemies.Count; ++index)
      {
        if ((bool) (UnityEngine.Object) activeEnemies[index] && (bool) (UnityEngine.Object) activeEnemies[index].healthHaver && activeEnemies[index].healthHaver.IsBoss)
        {
          this.MyKing = activeEnemies[index];
          break;
        }
      }
    }

    public void Update()
    {
      if ((bool) (UnityEngine.Object) this && (bool) (UnityEngine.Object) this.healthHaver && this.healthHaver.IsDead)
        return;
      if (this.ShouldCry && (bool) (UnityEngine.Object) this.MyKing && this.MyKing.healthHaver.IsDead)
      {
        if (!this.m_isCrazed && (bool) (UnityEngine.Object) this.scepterAnimator)
        {
          this.scepterAnimator.Play("scepter_drop");
          this.scepterAnimator.transform.parent = SpawnManager.Instance.VFX;
        }
        this.aiAnimator.LockFacingDirection = true;
        this.aiAnimator.FacingDirection = (this.MyKing.CenterPosition - this.aiActor.CenterPosition).ToAngle();
        this.aiAnimator.PlayUntilCancelled("cry");
        this.aiActor.DiesOnCollison = true;
        this.aiActor.CollisionDamage = 0.0f;
        this.aiActor.healthHaver.ForceSetCurrentHealth(this.PostCrazeHealth);
        this.aiActor.ClearPath();
        this.aiActor.BehaviorVelocity = Vector2.zero;
        this.behaviorSpeculator.InterruptAndDisable();
        if (this.CanReturnAngry && GameManager.HasInstance && (bool) (UnityEngine.Object) this && (bool) (UnityEngine.Object) this.healthHaver && this.healthHaver.IsAlive)
          GameManager.Instance.RunData.SpawnAngryToadie = true;
        this.MyKing = (AIActor) null;
        this.specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.PreRigidbodyCollision);
        this.m_isCrying = true;
      }
      if (this.m_isCrazed || this.m_isCrying || (double) this.AutoCrazeTime <= 0.0)
        return;
      this.m_timer += BraveTime.DeltaTime;
      if ((double) this.m_timer <= (double) this.AutoCrazeTime)
        return;
      this.LoseHat();
      this.aiAnimator.SetBaseAnim("crazed");
      this.behaviorSpeculator.enabled = true;
      this.MakeVulnerable();
      this.m_isCrazed = true;
      this.specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.PreRigidbodyCollision);
      this.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnDamaged);
    }

    protected override void OnDestroy()
    {
      this.specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.PreRigidbodyCollision);
      this.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnDamaged);
      if (this.CanReturnAngry)
        this.healthHaver.OnDeath -= new Action<Vector2>(this.OnDeath);
      base.OnDestroy();
    }

    private void PreRigidbodyCollision(
      SpeculativeRigidbody myRigidbody,
      PixelCollider myPixelCollider,
      SpeculativeRigidbody otherRigidbody,
      PixelCollider otherPixelCollider)
    {
      Projectile projectile = otherRigidbody.projectile;
      if ((bool) (UnityEngine.Object) projectile && projectile.Owner is PlayerController)
      {
        this.WasHit(otherRigidbody.Velocity);
      }
      else
      {
        if (!(bool) (UnityEngine.Object) otherRigidbody.gameActor)
          return;
        PhysicsEngine.SkipCollision = true;
      }
    }

    private void OnDamaged(
      float resultvalue,
      float maxvalue,
      CoreDamageTypes damagetypes,
      DamageCategory damagecategory,
      Vector2 damagedirection)
    {
      this.WasHit(damagedirection);
    }

    private void OnDeath(Vector2 vector2)
    {
      if (!this.CanReturnAngry || !GameManager.HasInstance)
        return;
      GameManager.Instance.RunData.SpawnAngryToadie = false;
    }

    private void WasHit(Vector2 hatDirection)
    {
      if (this.m_isCrazed)
        return;
      this.DropScepter();
      this.LoseHat(new Vector2?(hatDirection));
      this.aiAnimator.SetBaseAnim("crazed");
      this.behaviorSpeculator.enabled = true;
      this.m_isCrazed = true;
      this.Invoke("MakeVulnerable", 1f);
      this.specRigidbody.OnPreRigidbodyCollision -= new SpeculativeRigidbody.OnPreRigidbodyCollisionDelegate(this.PreRigidbodyCollision);
      this.healthHaver.OnDamaged -= new HealthHaver.OnDamagedEvent(this.OnDamaged);
    }

    private void DropScepter()
    {
      if (!(bool) (UnityEngine.Object) this.scepterAnimator)
        return;
      this.scepterAnimator.Play("scepter_drop");
      this.scepterAnimator.transform.parent = SpawnManager.Instance.VFX;
    }

    private void LoseHat(Vector2? hatDirection = null)
    {
      if (!(bool) (UnityEngine.Object) this.hatPoint || !(bool) (UnityEngine.Object) this.hatVfx)
        return;
      if (!hatDirection.HasValue)
        hatDirection = new Vector2?(Vector2.one);
      GameObject gameObject = SpawnManager.SpawnVFX(this.hatVfx, this.hatPoint.position, Quaternion.identity);
      gameObject.transform.parent = SpawnManager.Instance.VFX;
      DebrisObject orAddComponent = gameObject.GetOrAddComponent<DebrisObject>();
      orAddComponent.angularVelocity = 270f;
      orAddComponent.angularVelocityVariance = 0.0f;
      orAddComponent.decayOnBounce = 0.5f;
      orAddComponent.bounceCount = 3;
      orAddComponent.canRotate = true;
      orAddComponent.Trigger((Vector3) (hatDirection.Value.normalized * 10f), 1f);
    }

    private void MakeVulnerable()
    {
      this.healthHaver.IsVulnerable = true;
      this.healthHaver.ForceSetCurrentHealth(this.PostCrazeHealth);
    }
  }

