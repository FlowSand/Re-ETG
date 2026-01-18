using Dungeonator;
using UnityEngine;

#nullable disable

public class SimpleTurtleMillAboutBehavior : MovementBehaviorBase
  {
    public float PathInterval = 0.25f;
    public float TargetInterval = 3f;
    public float MillRadius = 5f;
    private Vector2 m_currentTargetPosition;
    private float m_repathTimer;
    private float m_newPositionTimer;

    public override void Start() => base.Start();

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_repathTimer);
      this.DecrementTimer(ref this.m_newPositionTimer);
    }

    private Vector2 GetNewTargetPosition(PlayerController owner)
    {
      Vector2? nullable = new Vector2?();
      int num = 30;
      while (!nullable.HasValue && num > 0)
      {
        --num;
        Vector2 vector2 = Random.insideUnitCircle.normalized * Random.Range(0.5f, 1f);
        nullable = new Vector2?(owner.specRigidbody.HitboxPixelCollider.UnitCenter + vector2 * this.MillRadius);
        nullable = !nullable.HasValue ? new Vector2?() : new Vector2?(nullable.GetValueOrDefault() + owner.specRigidbody.Velocity.normalized * Random.Range(0.0f, this.MillRadius * 1.5f));
        CellData cell = nullable.Value.GetCell();
        if (cell == null || cell.type != CellType.FLOOR || !cell.IsPassable)
          nullable = new Vector2?();
      }
      return !nullable.HasValue ? owner.specRigidbody.HitboxPixelCollider.UnitBottomCenter : nullable.Value;
    }

    public override BehaviorResult Update()
    {
      if (!GameManager.HasInstance || GameManager.Instance.IsLoadingLevel)
        return BehaviorResult.SkipAllRemainingBehaviors;
      if (GameManager.Instance.CurrentLevelOverrideState == GameManager.LevelOverrideState.END_TIMES)
      {
        this.m_aiActor.ClearPath();
        return BehaviorResult.SkipAllRemainingBehaviors;
      }
      PlayerController owner = GameManager.Instance.PrimaryPlayer;
      if ((bool) (Object) this.m_aiActor && (bool) (Object) this.m_aiActor.CompanionOwner)
        owner = this.m_aiActor.CompanionOwner;
      this.m_aiActor.MovementSpeed = this.m_aiActor.BaseMovementSpeed;
      if (!(bool) (Object) owner || !owner.IsInCombat)
        return BehaviorResult.Continue;
      float num1 = Vector2.Distance(owner.CenterPosition, this.m_currentTargetPosition);
      float num2 = Vector2.Distance(this.m_aiActor.CenterPosition, this.m_currentTargetPosition);
      if ((double) this.m_newPositionTimer <= 0.0 || (double) num1 > (double) this.MillRadius * 1.75 || (double) num2 <= 0.25)
      {
        this.m_aiActor.ClearPath();
        this.m_currentTargetPosition = this.GetNewTargetPosition(owner);
        this.m_newPositionTimer = this.TargetInterval;
      }
      else if ((double) num2 > 30.0)
        this.m_aiActor.CompanionWarp((Vector3) this.m_aiActor.CompanionOwner.CenterPosition);
      this.m_aiActor.MovementSpeed = Mathf.Lerp(this.m_aiActor.BaseMovementSpeed, this.m_aiActor.BaseMovementSpeed * 2f, Mathf.Clamp01(num2 / 30f));
      if ((double) this.m_repathTimer <= 0.0 && !owner.IsOverPitAtAll && !owner.IsInMinecart)
      {
        this.m_repathTimer = this.PathInterval;
        this.m_aiActor.FallingProhibited = false;
        this.m_aiActor.PathfindToPosition(this.m_currentTargetPosition);
        if (this.m_aiActor.Path != null && (double) this.m_aiActor.Path.InaccurateLength > 50.0)
        {
          this.m_aiActor.ClearPath();
          this.m_aiActor.CompanionWarp((Vector3) this.m_aiActor.CompanionOwner.CenterPosition);
        }
        else if (this.m_aiActor.Path != null && !this.m_aiActor.Path.WillReachFinalGoal)
          this.m_aiActor.CompanionWarp((Vector3) this.m_aiActor.CompanionOwner.CenterPosition);
      }
      return BehaviorResult.SkipRemainingClassBehaviors;
    }
  }

