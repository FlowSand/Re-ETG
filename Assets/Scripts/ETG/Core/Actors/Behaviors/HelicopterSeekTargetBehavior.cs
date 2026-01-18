using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/Helicopter/SeekTargetBehavior")]
public class HelicopterSeekTargetBehavior : MovementBehaviorBase
    {
        public Vector2 minPoint;
        public Vector2 maxPoint;
        public Vector2 period;
        public float MaxSpeed = 6f;
        private float m_timer;
        private bool m_isMoving;

        public override void Start()
        {
            base.Start();
            this.m_updateEveryFrame = true;
        }

        public override void Upkeep()
        {
            base.Upkeep();
            this.m_aiActor.OverridePathVelocity = new Vector2?();
        }

        public override BehaviorResult Update()
        {
            this.m_timer += this.m_deltaTime;
            Vector2 unitBottomLeft = this.m_aiActor.ParentRoom.area.UnitBottomLeft;
            float a = 0.0f;
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
            {
                PlayerController allPlayer = GameManager.Instance.AllPlayers[index];
                if (allPlayer.healthHaver.IsAlive)
                    a = Mathf.Max(a, allPlayer.specRigidbody.UnitCenter.y);
            }
            float num = Mathf.Max(0.0f, a - unitBottomLeft.y);
            Vector2 vector2_1 = unitBottomLeft + new Vector2(Mathf.SmoothStep(this.minPoint.x, this.maxPoint.x, Mathf.PingPong(this.m_timer, this.period.x) / this.period.x), Mathf.SmoothStep(this.minPoint.y, this.maxPoint.y, Mathf.PingPong(this.m_timer, this.period.y) / this.period.y) + num) - this.m_aiActor.specRigidbody.UnitCenter;
            Vector2 vector2_2;
            if ((double) this.m_deltaTime > 0.0 && (double) vector2_1.magnitude > 0.0)
            {
                vector2_2 = vector2_1 / BraveTime.DeltaTime;
                if ((double) this.MaxSpeed >= 0.0 && (double) vector2_2.magnitude > (double) this.MaxSpeed)
                    vector2_2 = this.MaxSpeed * vector2_2.normalized;
            }
            else
                vector2_2 = Vector2.zero;
            this.m_isMoving = true;
            this.m_aiActor.OverridePathVelocity = new Vector2?(vector2_2);
            return BehaviorResult.Continue;
        }
    }

