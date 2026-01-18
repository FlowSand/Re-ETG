using UnityEngine;

#nullable disable

public class TimedAttackBehavior : BasicAttackBehavior
  {
    public float Duration;
    public BasicAttackBehavior AttackBehavior;
    private BehaviorResult m_defaultBehaviorResult;
    private bool m_runChildContinuous;
    private float m_runTimer;

    public override void Start()
    {
      base.Start();
      this.AttackBehavior.Start();
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_runTimer);
      this.AttackBehavior.Upkeep();
    }

    public override bool OverrideOtherBehaviors() => this.AttackBehavior.OverrideOtherBehaviors();

    public override BehaviorResult Update()
    {
      BehaviorResult behaviorResult1 = base.Update();
      if (behaviorResult1 != BehaviorResult.Continue)
        return behaviorResult1;
      if (!this.IsReady())
        return BehaviorResult.Continue;
      BehaviorResult behaviorResult2 = this.AttackBehavior.Update();
      if (behaviorResult2 == BehaviorResult.Continue)
        return behaviorResult2;
      if (behaviorResult2 == BehaviorResult.RunContinuousInClass || behaviorResult2 == BehaviorResult.SkipRemainingClassBehaviors)
        this.m_defaultBehaviorResult = BehaviorResult.RunContinuousInClass;
      if (behaviorResult2 == BehaviorResult.RunContinuous || behaviorResult2 == BehaviorResult.SkipAllRemainingBehaviors)
        this.m_defaultBehaviorResult = BehaviorResult.RunContinuous;
      this.m_runChildContinuous = behaviorResult2 == BehaviorResult.RunContinuous || behaviorResult2 == BehaviorResult.RunContinuousInClass;
      this.m_runTimer = this.Duration;
      return this.m_defaultBehaviorResult;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      if (!this.m_runChildContinuous)
      {
        if ((double) this.m_runTimer <= 0.0)
          return ContinuousBehaviorResult.Finished;
        BehaviorResult behaviorResult = this.AttackBehavior.Update();
        this.m_runChildContinuous = behaviorResult == BehaviorResult.RunContinuous || behaviorResult == BehaviorResult.RunContinuousInClass;
        return ContinuousBehaviorResult.Continue;
      }
      if (this.AttackBehavior.ContinuousUpdate() == ContinuousBehaviorResult.Finished)
      {
        this.AttackBehavior.EndContinuousUpdate();
        this.m_runChildContinuous = false;
      }
      return (double) this.m_runTimer <= 0.0 ? ContinuousBehaviorResult.Finished : ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      if (this.m_runChildContinuous)
      {
        this.AttackBehavior.EndContinuousUpdate();
        this.m_runChildContinuous = false;
      }
      this.UpdateCooldowns();
    }

    public override void Destroy()
    {
      this.AttackBehavior.Destroy();
      base.Destroy();
    }

    public override void Init(GameObject gameObject, AIActor aiActor, AIShooter aiShooter)
    {
      base.Init(gameObject, aiActor, aiShooter);
      this.AttackBehavior.Init(gameObject, aiActor, aiShooter);
    }

    public override void SetDeltaTime(float deltaTime)
    {
      base.SetDeltaTime(deltaTime);
      this.AttackBehavior.SetDeltaTime(deltaTime);
    }

    public override bool IsReady() => base.IsReady() && this.AttackBehavior.IsReady();

    public override bool UpdateEveryFrame() => this.AttackBehavior.UpdateEveryFrame();

    public override bool IsOverridable() => this.AttackBehavior.IsOverridable();
  }

