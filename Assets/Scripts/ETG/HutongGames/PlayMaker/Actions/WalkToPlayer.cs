using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Paths to near the player's current location.")]
    [ActionCategory(".NPCs")]
    public class WalkToPlayer : FsmStateAction
    {
        public WalkToPlayer.TargetPathType pathDestinationType;
        private TalkDoerLite m_owner;
        private Vector2 m_lastPosition;

        public override string ErrorCheck() => string.Empty;

        public override void OnEnter()
        {
            this.m_owner = this.Owner.GetComponent<TalkDoerLite>();
            this.m_lastPosition = this.m_owner.specRigidbody.UnitCenter;
            Vector2 targetPosition = this.m_lastPosition;
            switch (this.pathDestinationType)
            {
                case WalkToPlayer.TargetPathType.PLAYER:
                    targetPosition = GameManager.Instance.BestActivePlayer.CenterPosition;
                    break;
                case WalkToPlayer.TargetPathType.PLAYER_ROOM_CENTER:
                    targetPosition = (Vector2) GameManager.Instance.BestActivePlayer.CurrentRoom.GetCenterCell().ToCenterVector3(0.0f);
                    break;
            }
            this.m_owner.PathfindToPosition(targetPosition);
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            this.m_owner.specRigidbody.Velocity = this.m_owner.GetPathVelocityContribution(this.m_lastPosition, 32);
            if (this.m_owner.CurrentPath == null)
                this.Finish();
            this.m_lastPosition = this.m_owner.specRigidbody.UnitCenter;
        }

        public enum TargetPathType
        {
            PLAYER,
            PLAYER_ROOM_CENTER,
        }
    }
}
