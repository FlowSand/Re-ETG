using UnityEngine;

#nullable disable

public class ModifySpeedBehavior : OverrideBehaviorBase
  {
    public float minSpeed;
    public float minSpeedDistance;
    public float maxSpeed;
    public float maxSpeedDistance;

    public override BehaviorResult Update()
    {
      if ((bool) (Object) this.m_aiActor.TargetRigidbody)
      {
        this.m_aiActor.MovementSpeed = TurboModeController.MaybeModifyEnemyMovementSpeed(Mathf.Lerp(this.minSpeed, this.maxSpeed, Mathf.InverseLerp(this.minSpeedDistance, this.maxSpeedDistance, this.m_aiActor.DistanceToTarget)));
        if (this.m_aiActor.IsBlackPhantom)
          this.m_aiActor.MovementSpeed *= this.m_aiActor.BlackPhantomProperties.MovementSpeedMultiplier;
      }
      return BehaviorResult.Continue;
    }
  }

