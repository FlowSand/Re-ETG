using Dungeonator;
using FullInspector;
using Pathfinding;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/BossFinalBullet/AgunimMoveBehavior")]
public class BossFinalBulletAgunimMoveBehavior : BasicAttackBehavior
  {
    public float MoveTime = 1f;
    public float MinSpeed;
    public float MaxSpeed;
    public bool DisableCollisionDuringMove;
    public int MinDistFromHorizontalWall = 4;
    public int MinDistFromNorthWall = 2;
    public int MinTilesAbovePlayer = 4;
    public int MaxTilesAbovePlayer = 8;
    public int MinDistanceFromPlayer = 4;
    public bool UseSouthWall;
    [InspectorIndent]
    [InspectorShowIf("UseSouthWall")]
    public int MinTilesBelowPlayer = 4;
    [InspectorShowIf("UseSouthWall")]
    [InspectorIndent]
    public int MaxTilesBelowPlayer = 4;
    [InspectorShowIf("UseSouthWall")]
    [InspectorIndent]
    public int MinDistFromSouthWall = 2;
    [InspectorCategory("Visuals")]
    public string preMoveAnimation;
    [InspectorCategory("Visuals")]
    public string moveAnimation;
    [InspectorCategory("Visuals")]
    public string postMoveAnimation;
    [InspectorCategory("Visuals")]
    public bool enableShadowTrail;
    private BossFinalBulletAgunimMoveBehavior.MoveState m_state;
    private Vector2 m_startPoint;
    private Vector2 m_targetPoint;
    private float m_moveTime;
    private float m_setupTimer;
    private AfterImageTrailController m_shadowTrail;

    public override void Start()
    {
      base.Start();
      this.m_shadowTrail = this.m_aiActor.GetComponent<AfterImageTrailController>();
    }

    public override BehaviorResult Update()
    {
      BehaviorResult behaviorResult = base.Update();
      if (behaviorResult != BehaviorResult.Continue)
        return behaviorResult;
      if (!this.IsReady() || !(bool) (Object) this.m_aiActor.TargetRigidbody)
        return BehaviorResult.Continue;
      this.m_aiActor.ClearPath();
      this.m_aiActor.BehaviorOverridesVelocity = true;
      this.m_aiActor.BehaviorVelocity = Vector2.zero;
      this.m_aiAnimator.LockFacingDirection = true;
      this.m_aiAnimator.FacingDirection = -90f;
      this.State = string.IsNullOrEmpty(this.preMoveAnimation) ? BossFinalBulletAgunimMoveBehavior.MoveState.Move : BossFinalBulletAgunimMoveBehavior.MoveState.PreMove;
      this.m_updateEveryFrame = true;
      return BehaviorResult.RunContinuous;
    }

    public override ContinuousBehaviorResult ContinuousUpdate()
    {
      int num = (int) base.ContinuousUpdate();
      if (this.State == BossFinalBulletAgunimMoveBehavior.MoveState.PreMove)
      {
        if (!this.m_aiAnimator.IsPlaying(this.preMoveAnimation))
        {
          this.State = BossFinalBulletAgunimMoveBehavior.MoveState.Move;
          return ContinuousBehaviorResult.Continue;
        }
      }
      else if (this.State == BossFinalBulletAgunimMoveBehavior.MoveState.Move)
      {
        if ((double) this.m_setupTimer > (double) this.m_moveTime)
        {
          this.m_aiActor.BehaviorVelocity = Vector2.zero;
          if (string.IsNullOrEmpty(this.postMoveAnimation))
            return ContinuousBehaviorResult.Finished;
          this.State = BossFinalBulletAgunimMoveBehavior.MoveState.PostMove;
          return ContinuousBehaviorResult.Continue;
        }
        if ((double) this.m_deltaTime > 0.0)
        {
          Vector2 unitCenter = this.m_aiActor.specRigidbody.UnitCenter;
          this.m_aiActor.BehaviorVelocity = (Vector2Extensions.SmoothStep(this.m_startPoint, this.m_targetPoint, this.m_setupTimer / this.m_moveTime) - unitCenter) / this.m_deltaTime;
          this.m_setupTimer += this.m_deltaTime;
        }
      }
      else if (this.State == BossFinalBulletAgunimMoveBehavior.MoveState.PostMove && !this.m_aiAnimator.IsPlaying(this.postMoveAnimation))
        return ContinuousBehaviorResult.Finished;
      return ContinuousBehaviorResult.Continue;
    }

    public override void EndContinuousUpdate()
    {
      base.EndContinuousUpdate();
      if (this.DisableCollisionDuringMove)
      {
        this.m_aiActor.specRigidbody.CollideWithOthers = true;
        this.m_aiActor.IsGone = false;
      }
      this.State = BossFinalBulletAgunimMoveBehavior.MoveState.None;
      if (!string.IsNullOrEmpty(this.preMoveAnimation))
        this.m_aiAnimator.EndAnimationIf(this.preMoveAnimation);
      if (!string.IsNullOrEmpty(this.moveAnimation))
        this.m_aiAnimator.EndAnimationIf(this.moveAnimation);
      if (!string.IsNullOrEmpty(this.postMoveAnimation))
        this.m_aiAnimator.EndAnimationIf(this.postMoveAnimation);
      this.m_aiAnimator.LockFacingDirection = false;
      this.m_aiActor.BehaviorOverridesVelocity = false;
      this.m_updateEveryFrame = false;
      this.UpdateCooldowns();
    }

    private void UpdateTargetPoint()
    {
      float minDistanceFromPlayerSquared = (float) (this.MinDistanceFromPlayer * this.MinDistanceFromPlayer);
      bool hasOtherPlayer = false;
      Vector2 playerLowerLeft = this.m_aiActor.TargetRigidbody.HitboxPixelCollider.UnitBottomLeft;
      Vector2 playerUpperRight = this.m_aiActor.TargetRigidbody.HitboxPixelCollider.UnitTopRight;
      Vector2 otherPlayerLowerLeft = Vector2.zero;
      Vector2 otherPlayerUpperRight = Vector2.zero;
      float maxPlayerY = playerLowerLeft.y;
      float minPlayerY = playerLowerLeft.y;
      PlayerController playerTarget = this.m_behaviorSpeculator.PlayerTarget as PlayerController;
      if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER && (bool) (Object) playerTarget)
      {
        PlayerController otherPlayer = GameManager.Instance.GetOtherPlayer(playerTarget);
        if ((bool) (Object) otherPlayer && otherPlayer.healthHaver.IsAlive)
        {
          hasOtherPlayer = true;
          otherPlayerLowerLeft = otherPlayer.specRigidbody.HitboxPixelCollider.UnitBottomLeft;
          otherPlayerUpperRight = otherPlayer.specRigidbody.HitboxPixelCollider.UnitTopRight;
          maxPlayerY = Mathf.Max(maxPlayerY, otherPlayerLowerLeft.y);
          minPlayerY = Mathf.Min(minPlayerY, otherPlayerLowerLeft.y);
        }
      }
      int minDx = -this.MinDistFromHorizontalWall;
      int maxDx = this.MinDistFromHorizontalWall + this.m_aiActor.Clearance.x - 2;
      float roomMinY = this.m_aiActor.ParentRoom.area.UnitBottomLeft.y;
      float roomMaxY = this.m_aiActor.ParentRoom.area.UnitTopRight.y;
      int minTilesAbovePlayer = this.MinTilesAbovePlayer;
      int minTilesBelowPlayer = this.MinTilesBelowPlayer;
      CellValidator cellValidator = (CellValidator) (c =>
      {
        for (int index1 = 0; index1 < this.m_aiActor.Clearance.x; ++index1)
        {
          int x = c.x + index1;
          for (int index2 = 0; index2 < this.m_aiActor.Clearance.y; ++index2)
          {
            int y = c.y + index2;
            if (GameManager.Instance.Dungeon.data.isTopWall(x, y))
              return false;
          }
        }
        float num1 = (float) c.y - maxPlayerY;
        float num2 = minPlayerY - (float) c.y;
        bool flag1 = (double) num1 >= (double) minTilesAbovePlayer && (double) num1 <= (double) this.MaxTilesAbovePlayer;
        bool flag2 = this.UseSouthWall && (double) num2 >= (double) minTilesBelowPlayer && (double) num1 <= (double) this.MaxTilesBelowPlayer;
        if (!flag1 && !flag2)
          return false;
        if (this.MinDistanceFromPlayer > 0)
        {
          PixelCollider hitboxPixelCollider = this.m_aiActor.specRigidbody.HitboxPixelCollider;
          Vector2 aMin = new Vector2((float) c.x + (float) (0.5 * ((double) this.m_aiActor.Clearance.x - (double) hitboxPixelCollider.UnitWidth)), (float) c.y);
          Vector2 aMax = aMin + hitboxPixelCollider.UnitDimensions;
          if (this.MinDistanceFromPlayer > 0 && ((double) BraveMathCollege.AABBDistanceSquared(aMin, aMax, playerLowerLeft, playerUpperRight) < (double) minDistanceFromPlayerSquared || hasOtherPlayer && (double) BraveMathCollege.AABBDistanceSquared(aMin, aMax, otherPlayerLowerLeft, otherPlayerUpperRight) < (double) minDistanceFromPlayerSquared))
            return false;
        }
        for (int index = minDx; index <= maxDx; ++index)
        {
          if (GameManager.Instance.Dungeon.data.isWall(c.x + index, c.y))
            return false;
        }
        return (double) roomMaxY - (double) c.y >= (double) (this.MinDistFromNorthWall + 1) && (!this.UseSouthWall || (double) c.y - (double) roomMinY >= (double) this.MinDistFromSouthWall);
      });
      IntVector2? randomAvailableCell = this.m_aiActor.ParentRoom.GetRandomAvailableCell(new IntVector2?(this.m_aiActor.Clearance), new CellTypes?(this.m_aiActor.PathableTiles), cellValidator: cellValidator);
      if (!randomAvailableCell.HasValue)
      {
        minTilesAbovePlayer = 0;
        minTilesBelowPlayer = 0;
        randomAvailableCell = this.m_aiActor.ParentRoom.GetRandomAvailableCell(new IntVector2?(this.m_aiActor.Clearance), new CellTypes?(this.m_aiActor.PathableTiles), cellValidator: cellValidator);
      }
      if (randomAvailableCell.HasValue)
      {
        this.m_targetPoint = randomAvailableCell.Value.ToCenterVector2();
      }
      else
      {
        Debug.LogWarning((object) "AGUNIM MOVE FAILED!", (Object) this.m_aiActor);
        this.m_targetPoint = this.m_aiActor.specRigidbody.UnitCenter;
      }
    }

    private BossFinalBulletAgunimMoveBehavior.MoveState State
    {
      get => this.m_state;
      set
      {
        if (this.m_state == value)
          return;
        this.EndState(this.m_state);
        this.m_state = value;
        this.BeginState(this.m_state);
      }
    }

    private void BeginState(BossFinalBulletAgunimMoveBehavior.MoveState state)
    {
      switch (state)
      {
        case BossFinalBulletAgunimMoveBehavior.MoveState.PreMove:
          this.m_aiActor.ClearPath();
          this.m_aiActor.BehaviorOverridesVelocity = true;
          this.m_aiActor.BehaviorVelocity = Vector2.zero;
          this.m_aiAnimator.PlayUntilCancelled(this.preMoveAnimation);
          if (!this.DisableCollisionDuringMove)
            break;
          this.m_aiActor.specRigidbody.CollideWithOthers = false;
          this.m_aiActor.IsGone = true;
          break;
        case BossFinalBulletAgunimMoveBehavior.MoveState.Move:
          this.m_startPoint = this.m_aiActor.specRigidbody.UnitCenter;
          this.UpdateTargetPoint();
          Vector2 vector = this.m_targetPoint - this.m_startPoint;
          float magnitude = vector.magnitude;
          this.m_moveTime = this.MoveTime;
          if ((double) this.MinSpeed > 0.0)
            this.m_moveTime = Mathf.Min(this.m_moveTime, magnitude / this.MinSpeed);
          if ((double) this.MaxSpeed > 0.0)
            this.m_moveTime = Mathf.Max(this.m_moveTime, magnitude / this.MaxSpeed);
          this.m_aiAnimator.FacingDirection = vector.ToAngle();
          this.m_aiAnimator.LockFacingDirection = true;
          this.m_aiAnimator.PlayUntilCancelled(this.moveAnimation);
          this.m_setupTimer = 0.0f;
          if (this.DisableCollisionDuringMove)
          {
            this.m_aiActor.specRigidbody.CollideWithOthers = false;
            this.m_aiActor.IsGone = true;
          }
          if (!this.enableShadowTrail)
            break;
          this.m_shadowTrail.spawnShadows = true;
          break;
        case BossFinalBulletAgunimMoveBehavior.MoveState.PostMove:
          this.m_aiActor.ClearPath();
          this.m_aiActor.BehaviorOverridesVelocity = true;
          this.m_aiActor.BehaviorVelocity = Vector2.zero;
          this.m_aiAnimator.PlayUntilCancelled(this.postMoveAnimation);
          if (!this.DisableCollisionDuringMove)
            break;
          this.m_aiActor.specRigidbody.CollideWithOthers = true;
          this.m_aiActor.IsGone = false;
          break;
      }
    }

    private void EndState(BossFinalBulletAgunimMoveBehavior.MoveState state)
    {
      if (state != BossFinalBulletAgunimMoveBehavior.MoveState.Move || !this.enableShadowTrail)
        return;
      this.m_shadowTrail.spawnShadows = false;
    }

    private enum MoveState
    {
      None,
      PreMove,
      Move,
      PostMove,
    }
  }

