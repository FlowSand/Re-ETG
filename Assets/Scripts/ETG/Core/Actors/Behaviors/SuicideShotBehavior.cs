using System;
using UnityEngine;

#nullable disable

public class SuicideShotBehavior : BasicAttackBehavior
  {
    public float minRange;
    public float leadAmount;
    public string chargeAnim;
    public int numBullets = 1;
    public int degreesBetween;
    public string bulletBankName;
    public bool suppressNormalDeath;
    public bool invulnerableDuringAnimatoin;
    public string fireVfx;
    private float m_cachedProjectileSpeed;
    private Vector2 m_cachedTargetCenter;

    public override void Start()
    {
      base.Start();
      AIBulletBank.Entry bullet = this.m_aiActor.bulletBank.GetBullet(this.bulletBankName);
      this.m_cachedProjectileSpeed = !bullet.OverrideProjectile ? bullet.BulletObject.GetComponent<Projectile>().baseData.speed : bullet.ProjectileData.speed;
      this.m_aiActor.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimEventTriggered);
    }

    public override BehaviorResult Update()
    {
      int num = (int) base.Update();
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady() || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
        return BehaviorResult.Continue;
      Vector2 vector2 = this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
      if ((double) this.leadAmount > 0.0)
      {
        Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(vector2, this.m_aiActor.TargetVelocity, this.m_aiActor.specRigidbody.UnitCenter, this.m_cachedProjectileSpeed);
        vector2 = Vector2.Lerp(vector2, predictedPosition, this.leadAmount);
      }
      this.m_cachedTargetCenter = vector2;
      if ((double) Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, vector2) <= (double) this.minRange)
        return BehaviorResult.Continue;
      this.m_aiAnimator.PlayUntilFinished(this.chargeAnim, true);
      this.m_aiActor.ClearPath();
      if (this.invulnerableDuringAnimatoin)
        this.m_aiActor.healthHaver.minimumHealth = 1f;
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      if (this.m_aiAnimator.IsPlaying(this.chargeAnim))
        return ContinuousBehaviorResult.Continue;
      Vector2 vector2 = this.m_cachedTargetCenter;
      if ((bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
      {
        vector2 = this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
        if ((double) this.leadAmount > 0.0)
        {
          Vector2 predictedPosition = BraveMathCollege.GetPredictedPosition(vector2, this.m_aiActor.TargetVelocity, this.m_aiActor.specRigidbody.UnitCenter, this.m_cachedProjectileSpeed);
          vector2 = Vector2.Lerp(vector2, predictedPosition, this.leadAmount);
        }
      }
      float num = (float) ((this.numBullets - 1) * -this.degreesBetween) * 0.5f;
      for (int index = 0; index < this.numBullets; ++index)
      {
        Vector2 unitCenter = this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitCenter;
        this.m_aiActor.bulletBank.CreateProjectileFromBank(unitCenter, (vector2 - unitCenter).ToAngle() + num, this.bulletBankName);
        num += (float) this.degreesBetween;
      }
      if (!string.IsNullOrEmpty(this.fireVfx))
        this.m_aiActor.aiAnimator.PlayVfx(this.fireVfx);
      if (this.suppressNormalDeath)
      {
        this.m_aiActor.healthHaver.ManualDeathHandling = true;
        this.m_aiActor.healthHaver.DisableStickyFriction = true;
      }
      this.m_aiActor.AdditionalSingleCoinDropChance = 0.0f;
      this.m_aiActor.healthHaver.SuppressDeathSounds = true;
      if (this.invulnerableDuringAnimatoin)
        this.m_aiActor.healthHaver.minimumHealth = 0.0f;
      this.m_aiActor.healthHaver.ApplyDamage(10000f, Vector2.zero, "Suicide Attack", damageCategory: DamageCategory.Unstoppable, ignoreInvulnerabilityFrames: true);
      if (this.suppressNormalDeath)
      {
        this.m_aiActor.ParentRoom.DeregisterEnemy(this.m_aiActor);
        UnityEngine.Object.Destroy((UnityEngine.Object) this.m_aiActor.gameObject);
      }
      return ContinuousBehaviorResult.Finished;
    }

    public override void EndContinuousUpdate()
    {
      if (this.invulnerableDuringAnimatoin)
        this.m_aiActor.healthHaver.minimumHealth = 0.0f;
      base.EndContinuousUpdate();
    }

    private void AnimEventTriggered(
      tk2dSpriteAnimator sprite,
      tk2dSpriteAnimationClip clip,
      int frameNum)
    {
      if (!(clip.GetFrame(frameNum).eventInfo == "disable_shadow") || !(bool) (UnityEngine.Object) this.m_aiActor.ShadowObject)
        return;
      this.m_aiActor.ShadowObject.SetActive(false);
    }
  }

