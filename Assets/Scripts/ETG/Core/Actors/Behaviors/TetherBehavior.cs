using UnityEngine;

#nullable disable

public class TetherBehavior : MovementBehaviorBase
  {
    public float KnockbackInvulnerabilityDelay = 0.5f;
    public float PathInterval = 0.25f;
    private float m_repathTimer;
    private float m_preventKnockbackTimer;
    private Vector2 m_tetherPosition;
    private TetherBehavior.State m_state;

    public override void Start()
    {
      base.Start();
      this.m_tetherPosition = this.m_aiActor.specRigidbody.UnitCenter;
    }

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_repathTimer);
    }

    public override BehaviorResult Update()
    {
      if (this.m_state == TetherBehavior.State.Idle)
      {
        if ((double) Vector2.Distance(this.m_tetherPosition, this.m_aiActor.specRigidbody.UnitCenter) > 0.10000000149011612)
        {
          this.m_state = TetherBehavior.State.ReturningToSpawn;
          this.m_aiActor.PathfindToPosition(this.m_tetherPosition, new Vector2?(this.m_tetherPosition));
          this.m_repathTimer = this.PathInterval;
          this.m_preventKnockbackTimer = this.KnockbackInvulnerabilityDelay;
        }
      }
      else if (this.m_state == TetherBehavior.State.ReturningToSpawn)
      {
        if ((double) this.m_preventKnockbackTimer > 0.0)
        {
          this.m_preventKnockbackTimer -= this.m_deltaTime;
          if ((double) this.m_preventKnockbackTimer <= 0.0)
            this.m_aiActor.knockbackDoer.SetImmobile(true, nameof (TetherBehavior));
        }
        if (this.m_aiActor.PathComplete)
        {
          this.m_state = TetherBehavior.State.Idle;
          this.m_aiActor.knockbackDoer.SetImmobile(false, nameof (TetherBehavior));
        }
        else if ((double) this.m_repathTimer <= 0.0)
        {
          this.m_aiActor.PathfindToPosition(this.m_tetherPosition, new Vector2?(this.m_tetherPosition));
          this.m_repathTimer = this.PathInterval;
        }
      }
      return BehaviorResult.Continue;
    }

    public override float DesiredCombatDistance => 0.0f;

    private enum State
    {
      Idle,
      PathingToTarget,
      ReturningToSpawn,
    }
  }

