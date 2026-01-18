using FullInspector;
using System;
using System.Collections.ObjectModel;
using UnityEngine;

#nullable disable

public class DestroyBulletsBehavior : BasicAttackBehavior
  {
    public float SkippableCooldown;
    public float SkippableRadius;
    public float Radius;
    public float BlankTime;
    public AnimationCurve RadiusCurve;
    [InspectorCategory("Visuals")]
    public string TellAnimation;
    [InspectorCategory("Visuals")]
    public string BlankAnimation;
    [InspectorCategory("Visuals")]
    public string BlankVfx;
    [InspectorCategory("Visuals")]
    public GameObject OverrideHitVfx;
    private DestroyBulletsBehavior.State m_state;
    private float m_timer;

    public override void Start()
    {
      base.Start();
      if (string.IsNullOrEmpty(this.TellAnimation))
        return;
      this.m_aiAnimator.spriteAnimator.AnimationEventTriggered += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip, int>(this.AnimEventTriggered);
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_timer);
      if ((double) this.m_behaviorSpeculator.AttackCooldown > 0.0 || (double) this.m_behaviorSpeculator.GlobalCooldown > 0.0 || (double) this.m_cooldownTimer >= (double) this.SkippableCooldown)
        return;
      bool flag = false;
      Vector2 unitCenter = this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitCenter;
      ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
      for (int index = 0; index < allProjectiles.Count; ++index)
      {
        Projectile projectile = allProjectiles[index];
        if (projectile.Owner is PlayerController && (bool) (UnityEngine.Object) projectile.specRigidbody && (double) Vector2.Distance(unitCenter, projectile.specRigidbody.UnitCenter) <= (double) this.SkippableRadius)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return;
      this.m_cooldownTimer = 0.0f;
    }

    public override BehaviorResult Update()
    {
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady() || !(bool) (UnityEngine.Object) this.m_aiActor.TargetRigidbody)
        return BehaviorResult.Continue;
      if (!string.IsNullOrEmpty(this.TellAnimation))
      {
        if (!string.IsNullOrEmpty(this.TellAnimation))
          this.m_aiAnimator.PlayUntilFinished(this.TellAnimation);
        this.m_state = DestroyBulletsBehavior.State.WaitingForTell;
      }
      else
        this.StartBlanking();
      this.m_aiActor.ClearPath();
      if ((bool) (UnityEngine.Object) this.m_aiActor && (bool) (UnityEngine.Object) this.m_aiActor.knockbackDoer)
        this.m_aiActor.knockbackDoer.SetImmobile(true, nameof (DestroyBulletsBehavior));
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      if (this.m_state == DestroyBulletsBehavior.State.WaitingForTell)
      {
        if (!this.m_aiAnimator.IsPlaying(this.TellAnimation))
        {
          this.StartBlanking();
          return ContinuousBehaviorResult.Continue;
        }
      }
      else if (this.m_state == DestroyBulletsBehavior.State.Blanking)
      {
        Vector2 unitCenter = this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitCenter;
        float num = this.RadiusCurve.Evaluate(Mathf.InverseLerp(this.BlankTime, 0.0f, this.m_timer)) * this.Radius;
        ReadOnlyCollection<Projectile> allProjectiles = StaticReferenceManager.AllProjectiles;
        for (int index = 0; index < allProjectiles.Count; ++index)
        {
          Projectile projectile = allProjectiles[index];
          if (projectile.Owner is PlayerController && (bool) (UnityEngine.Object) projectile.specRigidbody && (double) Vector2.Distance(unitCenter, projectile.specRigidbody.UnitCenter) <= (double) num)
          {
            if ((bool) (UnityEngine.Object) this.OverrideHitVfx)
              projectile.hitEffects.overrideMidairDeathVFX = this.OverrideHitVfx;
            projectile.DieInAir();
          }
        }
        if ((double) this.m_timer <= 0.0)
          return ContinuousBehaviorResult.Finished;
      }
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      if (!string.IsNullOrEmpty(this.TellAnimation))
        this.m_aiAnimator.EndAnimationIf(this.TellAnimation);
      if (!string.IsNullOrEmpty(this.BlankAnimation))
        this.m_aiAnimator.EndAnimationIf(this.BlankAnimation);
      if (!string.IsNullOrEmpty(this.BlankVfx))
        this.m_aiAnimator.StopVfx(this.BlankVfx);
      if ((bool) (UnityEngine.Object) this.m_aiActor && (bool) (UnityEngine.Object) this.m_aiActor.knockbackDoer)
        this.m_aiActor.knockbackDoer.SetImmobile(false, nameof (DestroyBulletsBehavior));
      this.m_state = DestroyBulletsBehavior.State.Idle;
      this.m_updateEveryFrame = false;
      this.UpdateCooldowns();
    }

    private void AnimEventTriggered(
      tk2dSpriteAnimator sprite,
      tk2dSpriteAnimationClip clip,
      int frameNum)
    {
      tk2dSpriteAnimationFrame frame = clip.GetFrame(frameNum);
      if (this.m_state != DestroyBulletsBehavior.State.WaitingForTell || !(frame.eventInfo == "blank"))
        return;
      this.StartBlanking();
    }

    private void StartBlanking()
    {
      if (!string.IsNullOrEmpty(this.BlankAnimation))
        this.m_aiAnimator.PlayUntilFinished(this.BlankAnimation);
      if (!string.IsNullOrEmpty(this.BlankVfx))
        this.m_aiAnimator.PlayVfx(this.BlankVfx);
      this.m_timer = this.BlankTime;
      this.m_state = DestroyBulletsBehavior.State.Blanking;
    }

    private enum State
    {
      Idle,
      WaitingForTell,
      Blanking,
    }
  }

