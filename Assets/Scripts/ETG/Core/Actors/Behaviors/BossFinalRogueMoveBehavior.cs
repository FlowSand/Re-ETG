using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalRogue/MoveBehavior")]
public class BossFinalRogueMoveBehavior : MovementBehaviorBase
  {
    public Vector2 maxMoveSpeed = new Vector2(3f, 0.0f);
    public Vector2 moveAcceleration = new Vector2(2f, 0.0f);
    public float ramMultiplier = 3f;
    public float minPlayerDist = 5f;
    public float maxPlayerDist = 12f;
    public float minYHeight;
    public float maxYHeight;
    private Vector2 m_targetCenter;
    private float? m_centerX;

    public override void Start()
    {
      base.Start();
      this.m_updateEveryFrame = true;
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.m_aiActor.BehaviorOverridesVelocity = true;
    }

    public override BehaviorResult Update()
    {
      if (!(bool) (Object) this.m_aiActor.TargetRigidbody)
        return BehaviorResult.Continue;
      if (!this.m_centerX.HasValue)
        this.m_centerX = new float?(this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitCenter.x);
      Vector2 unitCenter = this.m_aiActor.TargetRigidbody.UnitCenter;
      Vector2 zero = Vector2.zero;
      if ((double) this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitCenter.x < (double) this.m_centerX.Value - 2.0)
        zero.x = 1f;
      else if ((double) this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitCenter.x > (double) this.m_centerX.Value + 2.0)
        zero.x = -1f;
      float num = this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitBottom - unitCenter.y;
      bool useRamingSpeed = false;
      if ((double) num < -1.5)
      {
        if ((double) unitCenter.x < (double) this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitLeft)
        {
          useRamingSpeed = true;
          zero.x = -1f;
        }
        else if ((double) unitCenter.x > (double) this.m_aiActor.specRigidbody.HitboxPixelCollider.UnitRight)
        {
          useRamingSpeed = true;
          zero.x = 1f;
        }
      }
      this.m_aiActor.BehaviorVelocity.x = this.RamMoveTowards(this.m_aiActor.BehaviorVelocity.x, zero.x * this.maxMoveSpeed.x, this.moveAcceleration.x * this.m_deltaTime, useRamingSpeed);
      this.m_aiActor.BehaviorVelocity.y = 0.0f;
      return BehaviorResult.Continue;
    }

    private float RamMoveTowards(float current, float target, float maxDelta, bool useRamingSpeed)
    {
      float num1 = target;
      float num2 = maxDelta;
      if (useRamingSpeed)
      {
        num1 = target * this.ramMultiplier;
        num2 = maxDelta * this.ramMultiplier;
      }
      if ((double) num1 < 0.0 && ((double) current < (double) num1 || (double) current >= 0.0) || (double) num1 > 0.0 && ((double) current > (double) num1 || (double) current <= 0.0))
        num2 = maxDelta * this.ramMultiplier;
      return (double) Mathf.Abs(num1 - current) <= (double) num2 ? num1 : current + Mathf.Sign(num1 - current) * num2;
    }
  }

