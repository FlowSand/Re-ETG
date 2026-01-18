using FullInspector;
using UnityEngine;

using Dungeonator;

#nullable disable

[InspectorDropdownName("Bosses/BossStatues/LineBehavior")]
public class BossStatuesLineBehavior : BossStatuesPatternBehavior
    {
        public float Duration;
        public BossStatuesLineBehavior.Direction direction;
        private Vector2[] m_statuePositions;
        protected float m_durationTimer;
        private Vector2 m_minPos;
        private Vector2 m_maxPos;
        private Vector2 m_deltaPos;
        private Vector2 m_velocity;

        public override void Start() => base.Start();

        public override void Upkeep()
        {
            base.Upkeep();
            if (this.m_state != BossStatuesPatternBehavior.PatternState.InProgress)
                return;
            this.DecrementTimer(ref this.m_durationTimer);
        }

        public override void EndContinuousUpdate() => base.EndContinuousUpdate();

        protected override void InitPositions()
        {
            this.m_statuePositions = new Vector2[this.m_activeStatueCount];
            RoomHandler parentRoom = this.m_activeStatues[0].aiActor.ParentRoom;
            Vector2 vector2_1 = parentRoom.area.basePosition.ToVector2() + new Vector2(1f, 1f);
            Vector2 vector2_2 = (parentRoom.area.basePosition + parentRoom.area.dimensions).ToVector2() + new Vector2(-1f, -2f);
            float num = 0.0f;
            switch (this.direction)
            {
                case BossStatuesLineBehavior.Direction.LeftToRight:
                    this.m_minPos = new Vector2(vector2_1.x, vector2_2.y);
                    this.m_maxPos = new Vector2(vector2_1.x, vector2_1.y);
                    this.m_velocity = Vector2.right;
                    num = vector2_2.x - vector2_1.x;
                    break;
                case BossStatuesLineBehavior.Direction.RightToLeft:
                    this.m_minPos = new Vector2(vector2_2.x, vector2_2.y);
                    this.m_maxPos = new Vector2(vector2_2.x, vector2_1.y);
                    this.m_velocity = -Vector2.right;
                    num = vector2_2.x - vector2_1.x;
                    break;
                case BossStatuesLineBehavior.Direction.TopToBottom:
                    this.m_minPos = new Vector2(vector2_1.x, vector2_2.y);
                    this.m_maxPos = new Vector2(vector2_2.x, vector2_2.y);
                    this.m_velocity = -Vector2.up;
                    num = vector2_2.y - vector2_1.y;
                    break;
                case BossStatuesLineBehavior.Direction.BottomToTop:
                    this.m_minPos = new Vector2(vector2_1.x, vector2_1.y);
                    this.m_maxPos = new Vector2(vector2_2.x, vector2_1.y);
                    this.m_velocity = Vector2.up;
                    num = vector2_2.y - vector2_1.y;
                    break;
            }
            this.m_deltaPos = (this.m_maxPos - this.m_minPos) / (float) this.m_activeStatueCount;
            float effectiveMoveSpeed = this.m_statuesController.GetEffectiveMoveSpeed((double) this.OverrideMoveSpeed <= 0.0 ? this.m_statuesController.moveSpeed : this.OverrideMoveSpeed);
            this.m_velocity *= effectiveMoveSpeed;
            this.m_durationTimer = (double) this.Duration <= 0.0 ? num / effectiveMoveSpeed : this.Duration;
            for (int index = 0; index < this.m_activeStatueCount; ++index)
                this.m_statuePositions[index] = this.m_minPos + ((float) index + 0.5f) * this.m_deltaPos;
            this.ReorderStatues(this.m_statuePositions);
            for (int index = 0; index < this.m_activeStatueCount; ++index)
                this.m_activeStatues[index].Target = new Vector2?(this.m_statuePositions[index]);
        }

        protected override void UpdatePositions()
        {
            for (int index = 0; index < this.m_activeStatueCount; ++index)
            {
                this.m_statuePositions[index] += this.m_velocity * this.m_deltaTime;
                this.m_activeStatues[index].Target = new Vector2?(this.m_statuePositions[index]);
            }
        }

        protected override bool IsFinished() => (double) this.m_durationTimer <= 0.0;

        public enum Direction
        {
            LeftToRight,
            RightToLeft,
            TopToBottom,
            BottomToTop,
        }
    }

