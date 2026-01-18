using Dungeonator;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

#nullable disable

public class CompanionShootsRadialBurstBehavior : AttackBehaviorBase
  {
    [CheckAnimation(null)]
    public string BurstAnimation;
    public float AnimationDelay = 0.125f;
    public float DetectRadius = 8f;
    public float WaveRadius = 15f;
    public float Cooldown = 15f;
    public RadialBurstInterface Burst;
    public string BurstAudioEvent;
    private float m_cooldownTimer;

    public override BehaviorResult Update()
    {
      int num1 = (int) base.Update();
      this.DecrementTimer(ref this.m_cooldownTimer);
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if ((double) this.m_cooldownTimer > 0.0)
        return BehaviorResult.Continue;
      List<AIActor> activeEnemies = this.m_aiActor.CompanionOwner.CurrentRoom.GetActiveEnemies(RoomHandler.ActiveEnemyType.RoomClear);
      if (activeEnemies == null)
        return BehaviorResult.Continue;
      bool flag = false;
      float num2 = this.DetectRadius * this.DetectRadius;
      for (int index = 0; index < activeEnemies.Count; ++index)
      {
        if ((double) (activeEnemies[index].CenterPosition - this.m_aiActor.CenterPosition).sqrMagnitude < (double) num2)
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return BehaviorResult.Continue;
      this.m_aiActor.StartCoroutine(this.DoRadialBurst());
      this.m_cooldownTimer = this.Cooldown;
      return BehaviorResult.SkipRemainingClassBehaviors;
    }

    [DebuggerHidden]
    private IEnumerator DoRadialBurst()
    {
      // ISSUE: object of a compiler-generated type is created
      return (IEnumerator) new CompanionShootsRadialBurstBehavior__DoRadialBurstc__Iterator0()
      {
        _this = this
      };
    }

    public override float GetMaxRange() => this.DetectRadius;

    public override float GetMinReadyRange() => 0.0f;

    public override bool IsReady() => (double) this.m_cooldownTimer <= 0.0;
  }

