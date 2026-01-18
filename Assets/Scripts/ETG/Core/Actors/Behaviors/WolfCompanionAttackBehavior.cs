using Dungeonator;
using UnityEngine;

#nullable disable

public class WolfCompanionAttackBehavior : AttackBehaviorBase
  {
    public float minLeapDistance = 1f;
    public float leapDistance = 4f;
    public float maxTravelDistance = 5f;
    public float leadAmount;
    public float leapTime = 0.75f;
    public float maximumChargeTime = 0.25f;
    public string chargeAnim;
    public string leapAnim;
    [LongNumericEnum]
    public CustomSynergyType DebuffSynergy;
    public AIActorDebuffEffect EnemyDebuff;
    private float m_elapsed;
    private WolfCompanionAttackBehavior.State m_state;

    public override BehaviorResult Update()
    {
      int num1 = (int) base.Update();
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!(bool) (Object) this.m_aiActor.TargetRigidbody)
        return BehaviorResult.Continue;
      Vector2 vector2 = this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
      if ((double) this.leadAmount > 0.0)
      {
        Vector2 b = vector2 + this.m_aiActor.TargetRigidbody.specRigidbody.Velocity * 0.75f;
        vector2 = Vector2.Lerp(vector2, b, this.leadAmount);
      }
      float num2 = Vector2.Distance(this.m_aiActor.specRigidbody.UnitCenter, vector2);
      if ((double) num2 <= (double) this.minLeapDistance || (double) num2 >= (double) this.leapDistance)
        return BehaviorResult.Continue;
      this.m_state = WolfCompanionAttackBehavior.State.Charging;
      this.m_aiAnimator.PlayForDuration(this.chargeAnim, this.maximumChargeTime, true);
      this.m_aiActor.ClearPath();
      this.m_aiActor.BehaviorOverridesVelocity = true;
      this.m_aiActor.BehaviorVelocity = Vector2.zero;
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      if (this.m_state == WolfCompanionAttackBehavior.State.Charging)
      {
        if (!this.m_aiAnimator.IsPlaying(this.chargeAnim))
        {
          this.m_state = WolfCompanionAttackBehavior.State.Leaping;
          if (!(bool) (Object) this.m_aiActor.TargetRigidbody || !this.m_aiActor.TargetRigidbody.enabled)
          {
            this.m_state = WolfCompanionAttackBehavior.State.Idle;
            return ContinuousBehaviorResult.Finished;
          }
          Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
          Vector2 vector2 = this.m_aiActor.TargetRigidbody.specRigidbody.GetUnitCenter(ColliderType.HitBox);
          if ((double) this.leadAmount > 0.0)
          {
            Vector2 b = vector2 + this.m_aiActor.TargetRigidbody.specRigidbody.Velocity * 0.75f;
            vector2 = Vector2.Lerp(vector2, b, this.leadAmount);
          }
          float num = Vector2.Distance(unitCenter, vector2);
          if ((double) num > (double) this.maxTravelDistance)
          {
            vector2 = unitCenter + (vector2 - unitCenter).normalized * this.maxTravelDistance;
            num = Vector2.Distance(unitCenter, vector2);
          }
          this.m_aiActor.ClearPath();
          this.m_aiActor.BehaviorOverridesVelocity = true;
          this.m_aiActor.BehaviorVelocity = (vector2 - unitCenter).normalized * (num / this.leapTime);
          float angle = this.m_aiActor.BehaviorVelocity.ToAngle();
          this.m_aiAnimator.LockFacingDirection = true;
          this.m_aiAnimator.FacingDirection = angle;
          this.m_aiActor.PathableTiles = CellTypes.FLOOR | CellTypes.PIT;
          this.m_aiActor.DoDustUps = false;
          this.m_aiAnimator.PlayUntilFinished(this.leapAnim, true);
        }
      }
      else if (this.m_state == WolfCompanionAttackBehavior.State.Leaping)
      {
        this.m_elapsed += this.m_deltaTime;
        if ((double) this.m_elapsed >= (double) this.leapTime)
          return ContinuousBehaviorResult.Finished;
      }
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      if ((bool) (Object) this.m_aiActor.TargetRigidbody && (bool) (Object) this.m_aiActor.TargetRigidbody.healthHaver)
      {
        this.m_aiActor.TargetRigidbody.healthHaver.ApplyDamage(5f, this.m_aiActor.specRigidbody.Velocity, "Wolf");
        if ((bool) (Object) this.m_aiActor.CompanionOwner && this.m_aiActor.CompanionOwner.HasActiveBonusSynergy(this.DebuffSynergy))
          this.m_aiActor.TargetRigidbody.aiActor.ApplyEffect((GameActorEffect) this.EnemyDebuff);
      }
      this.m_state = WolfCompanionAttackBehavior.State.Idle;
      this.m_aiActor.PathableTiles = CellTypes.FLOOR;
      this.m_aiActor.DoDustUps = true;
      this.m_aiActor.BehaviorOverridesVelocity = false;
      this.m_aiAnimator.LockFacingDirection = false;
      this.m_updateEveryFrame = false;
    }

    public override bool IsReady() => true;

    public override float GetMinReadyRange() => this.leapDistance;

    public override float GetMaxRange() => this.leapDistance;

    private enum State
    {
      Idle,
      Charging,
      Leaping,
    }
  }

