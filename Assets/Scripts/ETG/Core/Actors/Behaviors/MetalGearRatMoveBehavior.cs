using FullInspector;
using UnityEngine;

#nullable disable

[InspectorDropdownName("Bosses/MetalGearRat/MoveBehavior")]
public class MetalGearRatMoveBehavior : BasicAttackBehavior
    {
        public float HorizontalMovePixels = 5f;
        private MetalGearRatMoveBehavior.State m_state;
        private Vector2 m_moveDirection;
        private GameObject m_shadow;
        private Vector2 m_shadowStartingPos;
        private GameObject m_cameraPoint;
        private Vector2 m_cameraStartingPos;

        public override void Start() => this.m_moveDirection = Vector2.right;

        public override bool IsOverridable() => this.m_state == MetalGearRatMoveBehavior.State.Idle;

        public override BehaviorResult Update()
        {
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            if (!this.IsReady())
                return BehaviorResult.Continue;
            this.m_moveDirection = this.UpdateMoveDirection();
            this.m_shadow = this.m_aiActor.ShadowObject;
            this.m_shadowStartingPos = (Vector2) this.m_shadow.transform.localPosition;
            this.m_cameraPoint = this.m_aiActor.gameObject.transform.Find("camera point").gameObject;
            this.m_cameraStartingPos = (Vector2) this.m_cameraPoint.transform.localPosition;
            if ((double) this.m_moveDirection.x < 0.0)
                this.m_aiAnimator.PlayUntilFinished("move_left");
            else
                this.m_aiAnimator.PlayUntilFinished("move_right");
            this.m_updateEveryFrame = true;
            this.m_state = MetalGearRatMoveBehavior.State.Moving;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            int num = (int) base.ContinuousUpdate();
            if (this.m_state == MetalGearRatMoveBehavior.State.Moving)
            {
                Vector2 vector2 = Vector2.Lerp(Vector2.zero, this.m_moveDirection * (this.HorizontalMovePixels / 16f), this.m_aiAnimator.CurrentClipProgress * 2.2f);
                this.m_shadow.transform.localPosition = (Vector3) (this.m_shadowStartingPos + vector2);
                this.m_cameraPoint.transform.localPosition = (Vector3) (this.m_cameraStartingPos + vector2);
                if (!this.m_aiAnimator.IsPlaying("move_left") && !this.m_aiAnimator.IsPlaying("move_right"))
                {
                    this.m_aiActor.transform.position += (Vector3) (this.m_moveDirection * (this.HorizontalMovePixels / 16f));
                    this.m_shadow.transform.localPosition = (Vector3) this.m_shadowStartingPos;
                    this.m_cameraPoint.transform.localPosition = (Vector3) this.m_cameraStartingPos;
                    this.m_aiActor.specRigidbody.Reinitialize();
                    this.m_state = MetalGearRatMoveBehavior.State.Done;
                    return ContinuousBehaviorResult.Finished;
                }
            }
            else if (this.m_state == MetalGearRatMoveBehavior.State.Done)
                return ContinuousBehaviorResult.Finished;
            return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            this.m_state = MetalGearRatMoveBehavior.State.Idle;
            this.m_aiAnimator.EndAnimationIf("move_left");
            this.m_shadow.transform.localPosition = (Vector3) this.m_shadowStartingPos;
            this.m_cameraPoint.transform.localPosition = (Vector3) this.m_cameraStartingPos;
            this.m_updateEveryFrame = false;
            this.UpdateCooldowns();
        }

        private Vector2 UpdateMoveDirection()
        {
            Vector2 unitCenter1 = this.m_aiActor.ParentRoom.area.UnitCenter;
            Vector2 unitCenter2 = this.m_aiActor.specRigidbody.GetUnitCenter(ColliderType.HitBox);
            if ((double) unitCenter2.x < (double) unitCenter1.x - 7.0)
                return Vector2.right;
            if ((double) unitCenter2.x > (double) unitCenter1.x + 7.0)
                return Vector2.left;
            if ((bool) (Object) this.m_aiActor.TargetRigidbody)
            {
                float num = this.m_aiActor.TargetRigidbody.GetUnitCenter(ColliderType.HitBox).x - unitCenter2.x;
                if ((double) num > 0.0)
                    return Vector2.right;
                if ((double) num < 0.0)
                    return Vector2.left;
            }
            return BraveUtility.RandomBool() ? Vector2.left : Vector2.right;
        }

        private enum State
        {
            Idle,
            Moving,
            Done,
        }
    }

