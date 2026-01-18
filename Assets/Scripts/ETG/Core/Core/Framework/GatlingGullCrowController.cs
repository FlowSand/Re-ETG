using UnityEngine;

using Dungeonator;

#nullable disable

public class GatlingGullCrowController : BraveBehaviour
    {
        public bool useFacePoint;
        public Vector2 facePoint;
        public bool destroyOnArrival;
        protected Vector2 m_currentPosition;
        protected float m_speed;
        protected float m_currentSpeed;

        public Vector2 CurrentTargetPosition { get; set; }

        public float CurrentTargetHeight { get; set; }

        private void Start()
        {
            this.spriteAnimator.ClipFps *= Random.Range(0.7f, 1.4f);
            this.m_currentPosition = this.transform.position.XY();
            this.m_speed = Random.Range(7f, 10f);
            this.sprite.UpdateZDepth();
        }

        private void Update()
        {
            if (this.destroyOnArrival && (GameManager.Instance.CurrentGameMode == GameManager.GameMode.BOSSRUSH || GameManager.Instance.CurrentGameMode == GameManager.GameMode.SUPERBOSSRUSH))
            {
                IntVector2 intVector2 = this.transform.position.IntXY(VectorConversions.Floor);
                if (!GameManager.Instance.Dungeon.data.CheckInBoundsAndValid(intVector2) || GameManager.Instance.Dungeon.data[intVector2].type == CellType.WALL)
                {
                    Object.Destroy((Object) this.gameObject);
                    return;
                }
            }
            if ((double) this.sprite.HeightOffGround != (double) this.CurrentTargetHeight)
            {
                float f1 = this.CurrentTargetHeight - this.sprite.HeightOffGround;
                float f2 = Mathf.Sign(f1) * 3f * BraveTime.DeltaTime;
                if ((double) Mathf.Abs(f2) > (double) Mathf.Abs(f1))
                    f2 = f1;
                this.sprite.HeightOffGround += f2;
            }
            if (this.m_currentPosition != this.CurrentTargetPosition)
            {
                if ((double) this.m_currentSpeed < (double) this.m_speed)
                    this.m_currentSpeed = Mathf.Clamp(this.m_currentSpeed + 4f * BraveTime.DeltaTime, 0.0f, this.m_speed);
                Vector2 vector2 = this.CurrentTargetPosition - this.m_currentPosition;
                this.sprite.FlipX = !this.useFacePoint ? (double) vector2.x < 0.0 : (double) (this.facePoint - this.m_currentPosition).x < 0.0;
                this.m_currentPosition += Mathf.Clamp(this.m_currentSpeed * BraveTime.DeltaTime, 0.0f, vector2.magnitude) * vector2.normalized;
                this.transform.position = this.m_currentPosition.ToVector3ZUp();
                this.sprite.UpdateZDepth();
            }
            else
            {
                if (this.destroyOnArrival)
                    Object.Destroy((Object) this.gameObject);
                this.m_currentSpeed = 0.0f;
            }
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

