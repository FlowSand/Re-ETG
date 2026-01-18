using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/Beholster/ShootGunBehavior")]
public class BeholsterShootGunBehavior : BasicAttackBehavior
  {
    public bool LineOfSight = true;
    public BeholsterTentacleController[] Tentacles;
    private BeholsterController m_beholster;

    public override void Start()
    {
      base.Start();
      this.m_beholster = this.m_aiActor.GetComponent<BeholsterController>();
    }

    public override void Upkeep() => base.Upkeep();

    public override BehaviorResult Update()
    {
      int num = (int) base.Update();
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady())
        return BehaviorResult.Continue;
      bool flag = this.LineOfSight && !this.m_aiActor.HasLineOfSightToTarget;
      if ((Object) this.m_aiActor.TargetRigidbody == (Object) null || flag)
      {
        this.m_beholster.StopFiringTentacles(this.Tentacles);
        return BehaviorResult.Continue;
      }
      this.m_beholster.StartFiringTentacles(this.Tentacles);
      this.UpdateCooldowns();
      return BehaviorResult.SkipRemainingClassBehaviors;
    }

    public override bool IsReady()
    {
      if (!base.IsReady())
        return false;
      for (int index = 0; index < this.Tentacles.Length; ++index)
      {
        if (this.Tentacles[index].IsReady)
          return true;
      }
      return false;
    }
  }

