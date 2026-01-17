// Decompiled with JetBrains decompiler
// Type: DraGunNearDeathBehavior
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using FullInspector;
using UnityEngine;

#nullable disable

namespace ETG.Core.Actors.Behaviors
{
    [InspectorDropdownName("Bosses/DraGun/NearDeathBehavior")]
    public class DraGunNearDeathBehavior : BasicAttackBehavior
    {
      public float DamageTime = 5f;
      public AIAnimator LeftHandAnimator;
      public AIAnimator RightHandAnimator;
      public AIAnimator WingsAnimator;
      public AIAnimator EyesAnimator;
      public AttackBehaviorGroup Attacks;
      private DraGunController m_dragun;
      private DraGunHeadController m_head;
      private AIAnimator m_headAnimator;
      private AutoAimTarget m_heartAutoAimTarget;
      private HealthHaver m_healthHaver;
      private HitEffectHandler m_hitEffectHandler;
      private DraGunNearDeathBehavior.State m_state;
      private float m_timer;

      public override void Start()
      {
        base.Start();
        this.m_dragun = this.m_aiActor.GetComponent<DraGunController>();
        this.m_head = this.m_dragun.head;
        this.m_headAnimator = this.m_head.aiAnimator;
        this.m_heartAutoAimTarget = this.m_dragun.GetComponentsInChildren<AutoAimTarget>(true)[0];
        this.m_heartAutoAimTarget.enabled = false;
        this.m_healthHaver = this.m_aiActor.healthHaver;
        this.m_hitEffectHandler = this.m_aiActor.hitEffectHandler;
        this.Attacks.Start();
      }

      public override void Upkeep()
      {
        base.Upkeep();
        if (this.m_state != DraGunNearDeathBehavior.State.Inactive)
          this.DecrementTimer(ref this.m_timer);
        this.Attacks.Upkeep();
      }

      public override BehaviorResult Update()
      {
        BehaviorResult behaviorResult = base.Update();
        if (behaviorResult != BehaviorResult.Continue)
          return behaviorResult;
        if (!this.IsReady())
          return BehaviorResult.Continue;
        this.m_state = DraGunNearDeathBehavior.State.Attacking;
        this.EyesAnimator.PlayUntilFinished("eyes_idle");
        int num = (int) this.Attacks.Update();
        SilencerInstance.s_MaxRadiusLimiter = new float?(8f);
        this.m_healthHaver.IsVulnerable = false;
        this.m_updateEveryFrame = true;
        return BehaviorResult.RunContinuous;
      }

      public override ContinuousBehaviorResult ContinuousUpdate()
      {
        int num1 = (int) base.ContinuousUpdate();
        if (this.m_state == DraGunNearDeathBehavior.State.Attacking)
        {
          if (this.Attacks.ContinuousUpdate() == ContinuousBehaviorResult.Finished)
          {
            this.Attacks.EndContinuousUpdate();
            SilencerInstance.s_MaxRadiusLimiter = new float?();
            this.m_state = DraGunNearDeathBehavior.State.WeakIntro;
            this.EyesAnimator.PlayUntilFinished("eyes_out");
            this.m_aiAnimator.PlayUntilCancelled("weak_intro");
            this.LeftHandAnimator.PlayUntilCancelled("weak_idle");
            this.RightHandAnimator.PlayUntilCancelled("weak_idle");
            this.WingsAnimator.PlayUntilCancelled("weak_idle");
            this.m_head.OverrideDesiredPosition = new Vector2?(this.m_aiActor.specRigidbody.UnitCenter + new Vector2(-3f, 7f));
            this.m_heartAutoAimTarget.enabled = true;
          }
        }
        else if (this.m_state == DraGunNearDeathBehavior.State.WeakIntro)
        {
          if (!this.m_aiAnimator.IsPlaying("weak_intro"))
          {
            this.m_state = DraGunNearDeathBehavior.State.Weak;
            this.m_aiAnimator.PlayUntilCancelled("weak_idle");
            this.m_headAnimator.PlayUntilCancelled("weak_idle");
            this.m_healthHaver.IsVulnerable = true;
            this.m_hitEffectHandler.additionalHitEffects[0].chance = 1f;
            this.m_timer = this.DamageTime;
            if (this.m_dragun.MaybeConvertToGold())
            {
              this.m_timer = 100000f;
              this.m_aiActor.healthHaver.minimumHealth = 1f;
            }
          }
        }
        else if (this.m_state == DraGunNearDeathBehavior.State.Weak)
        {
          if ((double) this.m_timer <= 0.0)
          {
            this.m_state = DraGunNearDeathBehavior.State.WeakOutro;
            this.m_aiAnimator.PlayUntilCancelled("weak_outro");
            this.m_headAnimator.EndAnimation();
            this.EyesAnimator.PlayUntilFinished("eyes_idle");
            this.m_healthHaver.IsVulnerable = false;
            this.m_hitEffectHandler.additionalHitEffects[0].chance = 0.0f;
            this.m_head.OverrideDesiredPosition = new Vector2?();
          }
        }
        else if (this.m_state == DraGunNearDeathBehavior.State.WeakOutro && !this.m_aiAnimator.IsPlaying("weak_outro"))
        {
          this.m_state = DraGunNearDeathBehavior.State.Attacking;
          this.m_aiAnimator.EndAnimation();
          this.LeftHandAnimator.EndAnimation();
          this.RightHandAnimator.EndAnimation();
          this.WingsAnimator.EndAnimation();
          int num2 = (int) this.Attacks.Update();
          this.m_heartAutoAimTarget.enabled = false;
          SilencerInstance.s_MaxRadiusLimiter = new float?(8f);
        }
        return ContinuousBehaviorResult.Continue;
      }

      public override void EndContinuousUpdate()
      {
        base.EndContinuousUpdate();
        this.m_state = DraGunNearDeathBehavior.State.Inactive;
        this.m_aiAnimator.EndAnimation();
        this.m_headAnimator.EndAnimation();
        this.LeftHandAnimator.EndAnimation();
        this.RightHandAnimator.EndAnimation();
        this.WingsAnimator.EndAnimation();
        this.m_updateEveryFrame = false;
        this.UpdateCooldowns();
        this.m_heartAutoAimTarget.enabled = false;
        SilencerInstance.s_MaxRadiusLimiter = new float?();
      }

      public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
      {
        base.Init(gameObject, aiActor, aiShooter);
        this.Attacks.Init(gameObject, aiActor, aiShooter);
      }

      public override void SetDeltaTime(float deltaTime)
      {
        base.SetDeltaTime(deltaTime);
        this.Attacks.SetDeltaTime(deltaTime);
      }

      public override bool IsReady() => this.m_dragun.IsNearDeath && base.IsReady();

      public override bool IsOverridable() => false;

      public override void OnActorPreDeath()
      {
        SilencerInstance.s_MaxRadiusLimiter = new float?();
        base.OnActorPreDeath();
        this.Attacks.OnActorPreDeath();
      }

      private enum State
      {
        Inactive,
        Attacking,
        WeakIntro,
        Weak,
        WeakOutro,
      }
    }

}
