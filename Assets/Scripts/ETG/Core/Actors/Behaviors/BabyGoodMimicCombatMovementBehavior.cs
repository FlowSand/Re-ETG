using Dungeonator;
using UnityEngine;

#nullable disable

public class BabyGoodMimicCombatMovementBehavior : MovementBehaviorBase
  {
    public float PathInterval = 0.25f;
    private float m_repathTimer;
    private Vector2? m_targetPos;
    private RoomHandler m_cachedRoom;

    public override void Start() => base.Start();

    public override void Upkeep()
    {
      base.Upkeep();
      this.DecrementTimer(ref this.m_repathTimer);
    }

    public override BehaviorResult Update()
    {
      if ((double) this.m_repathTimer > 0.0)
        return this.m_targetPos.HasValue ? BehaviorResult.SkipRemainingClassBehaviors : BehaviorResult.Continue;
      PlayerController playerController = GameManager.Instance.BestActivePlayer;
      if ((bool) (Object) this.m_aiActor && (bool) (Object) this.m_aiActor.CompanionOwner)
        playerController = this.m_aiActor.CompanionOwner;
      if (!(bool) (Object) playerController)
        return BehaviorResult.Continue;
      if (this.m_cachedRoom != null && playerController.CurrentRoom != this.m_cachedRoom)
      {
        this.m_cachedRoom = (RoomHandler) null;
        this.m_targetPos = new Vector2?();
      }
      if (playerController.IsInCombat && playerController.CurrentRoom.IsSealed)
      {
        if (!this.m_targetPos.HasValue)
        {
          IntVector2? nullable = new IntVector2?();
          IntVector2 intVector2 = new IntVector2(3, 3);
          while (!nullable.HasValue && intVector2.x > 0)
          {
            nullable = new IntVector2?(playerController.CurrentRoom.GetRandomAvailableCell(new IntVector2?(intVector2), new CellTypes?(CellTypes.FLOOR)).Value);
            intVector2 -= IntVector2.One;
          }
          if (nullable.HasValue)
          {
            this.m_targetPos = new Vector2?(nullable.Value.ToVector2() + intVector2.ToVector2() / 2f);
            this.m_cachedRoom = playerController.CurrentRoom;
          }
        }
      }
      else
        this.m_targetPos = new Vector2?();
      if (this.m_targetPos.HasValue)
      {
        this.m_aiActor.PathfindToPosition(this.m_targetPos.Value);
        this.m_repathTimer = this.PathInterval;
      }
      return this.m_targetPos.HasValue ? BehaviorResult.SkipRemainingClassBehaviors : BehaviorResult.Continue;
    }
  }

