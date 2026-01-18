using Pathfinding;
using UnityEngine;

using Dungeonator;

#nullable disable

public class MoveErraticallyBehavior : MovementBehaviorBase
    {
        public float PathInterval = 0.25f;
        public float PointReachedPauseTime;
        public bool PreventFiringWhileMoving;
        public float InitialDelay;
        public bool StayOnScreen = true;
        public bool AvoidTarget = true;
        public bool UseTargetsRoom;
        private float m_repathTimer;
        private float m_pauseTimer;
        private IntVector2? m_targetPos;
        private IntVector2 m_cachedCameraBottomLeft;
        private IntVector2 m_cachedCameraBottomRight;
        private float m_cachedAngleFromTarget;
        private Vector2 m_cachedTargetPos;
        private float? m_cachedAngleFromOtherTarget;
        private Vector2? m_cachedOtherTargetPos;

        public override void Start()
        {
            base.Start();
            this.m_pauseTimer = this.InitialDelay;
        }

        public override void Upkeep()
        {
            base.Upkeep();
            this.DecrementTimer(ref this.m_repathTimer);
            this.DecrementTimer(ref this.m_pauseTimer);
            if (this.StayOnScreen)
            {
                Vector2 worldpoint1 = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(0.0f, 0.0f), ViewportType.Gameplay);
                Vector2 worldpoint2 = (Vector2) BraveUtility.ViewportToWorldpoint(new Vector2(1f, 1f), ViewportType.Gameplay);
                this.m_cachedCameraBottomLeft = worldpoint1.ToIntVector2(VectorConversions.Ceil);
                this.m_cachedCameraBottomRight = worldpoint2.ToIntVector2(VectorConversions.Floor) - IntVector2.One;
            }
            if (!this.AvoidTarget || !(bool) (Object) this.m_aiActor.TargetRigidbody)
                return;
            this.m_cachedTargetPos = this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.Ground);
            this.m_cachedAngleFromTarget = (this.m_aiActor.specRigidbody.UnitCenter - this.m_cachedTargetPos).ToAngle();
            PlayerController playerTarget = this.m_aiActor.PlayerTarget as PlayerController;
            if (!(bool) (Object) playerTarget || GameManager.Instance.CurrentGameType != GameManager.GameType.COOP_2_PLAYER)
                return;
            this.m_cachedOtherTargetPos = new Vector2?(GameManager.Instance.GetOtherPlayer(playerTarget).specRigidbody.GetUnitCenter(ColliderType.Ground));
            this.m_cachedAngleFromOtherTarget = new float?((this.m_aiActor.specRigidbody.UnitCenter - this.m_cachedTargetPos).ToAngle());
        }

        public override BehaviorResult Update()
        {
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            if (!this.m_targetPos.HasValue && (double) this.m_repathTimer > 0.0)
                return BehaviorResult.Continue;
            if ((double) this.m_pauseTimer > 0.0)
                return BehaviorResult.SkipRemainingClassBehaviors;
            if (this.m_targetPos.HasValue && this.m_aiActor.PathComplete)
            {
                this.m_targetPos = new IntVector2?();
                if ((double) this.PointReachedPauseTime > 0.0)
                {
                    this.m_pauseTimer = this.PointReachedPauseTime;
                    return BehaviorResult.SkipAllRemainingBehaviors;
                }
            }
            if ((double) this.m_repathTimer <= 0.0)
            {
                this.m_repathTimer = this.PathInterval;
                if (this.m_targetPos.HasValue && !this.SimpleCellValidator(this.m_targetPos.Value))
                    this.m_targetPos = new IntVector2?();
                if (!this.m_targetPos.HasValue)
                {
                    RoomHandler roomHandler = this.m_aiActor.ParentRoom;
                    if (this.UseTargetsRoom && (bool) (Object) this.m_aiActor.TargetRigidbody)
                    {
                        PlayerController gameActor = !(bool) (Object) this.m_aiActor.TargetRigidbody.gameActor ? (PlayerController) null : this.m_aiActor.TargetRigidbody.gameActor as PlayerController;
                        if ((bool) (Object) gameActor)
                            roomHandler = gameActor.CurrentRoom;
                    }
                    this.m_targetPos = roomHandler.GetRandomAvailableCell(new IntVector2?(this.m_aiActor.Clearance), new CellTypes?(this.m_aiActor.PathableTiles), cellValidator: new CellValidator(this.FullCellValidator));
                }
                if (!this.m_targetPos.HasValue)
                    return BehaviorResult.Continue;
                this.m_aiActor.PathfindToPosition(this.m_targetPos.Value.ToCenterVector2());
            }
            return this.PreventFiringWhileMoving ? BehaviorResult.SkipAllRemainingBehaviors : BehaviorResult.SkipRemainingClassBehaviors;
        }

        public void ResetPauseTimer() => this.m_pauseTimer = 0.0f;

        public override bool AllowFearRunState => true;

        private bool SimpleCellValidator(IntVector2 c)
        {
            for (int index1 = 0; index1 < this.m_aiActor.Clearance.x; ++index1)
            {
                for (int index2 = 0; index2 < this.m_aiActor.Clearance.y; ++index2)
                {
                    if (GameManager.Instance.Dungeon.data.isTopWall(c.x + index1, c.y + index2))
                        return false;
                }
            }
            return !this.StayOnScreen || c.x >= this.m_cachedCameraBottomLeft.x && c.y >= this.m_cachedCameraBottomLeft.y && c.x + this.m_aiActor.Clearance.x - 1 <= this.m_cachedCameraBottomRight.x && c.y + this.m_aiActor.Clearance.y - 1 <= this.m_cachedCameraBottomRight.y;
        }

        private bool FullCellValidator(IntVector2 c)
        {
            return this.SimpleCellValidator(c) && (!this.AvoidTarget || !(bool) (Object) this.m_aiActor.TargetRigidbody || (double) BraveMathCollege.AbsAngleBetween((Pathfinder.GetClearanceOffset(c, this.m_aiActor.Clearance) - this.m_cachedTargetPos).ToAngle(), this.m_cachedAngleFromTarget) <= 90.0 && (!this.m_cachedOtherTargetPos.HasValue || !this.m_cachedAngleFromOtherTarget.HasValue || (double) BraveMathCollege.AbsAngleBetween((Pathfinder.GetClearanceOffset(c, this.m_aiActor.Clearance) - this.m_cachedOtherTargetPos.Value).ToAngle(), this.m_cachedAngleFromOtherTarget.Value) <= 90.0));
        }
    }

