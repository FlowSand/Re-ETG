#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Character)]
    [Tooltip("Gets info on the last Character Controller collision and store in variables.")]
    public class GetControllerHitInfo : FsmStateAction
    {
        [Tooltip("Store the GameObject hit in the last collision.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject gameObjectHit;
        [Tooltip("Store the contact point of the last collision in world coordinates.")]
        [UIHint(UIHint.Variable)]
        public FsmVector3 contactPoint;
        [Tooltip("Store the normal of the last collision.")]
        [UIHint(UIHint.Variable)]
        public FsmVector3 contactNormal;
        [Tooltip("Store the direction of the last move before the collision.")]
        [UIHint(UIHint.Variable)]
        public FsmVector3 moveDirection;
        [Tooltip("Store the distance of the last move before the collision.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat moveLength;
        [Tooltip("Store the physics material of the Game Object Hit. Useful for triggering different effects. Audio, particles...")]
        [UIHint(UIHint.Variable)]
        public FsmString physicsMaterialName;

        public override void Reset()
        {
            this.gameObjectHit = (FsmGameObject) null;
            this.contactPoint = (FsmVector3) null;
            this.contactNormal = (FsmVector3) null;
            this.moveDirection = (FsmVector3) null;
            this.moveLength = (FsmFloat) null;
            this.physicsMaterialName = (FsmString) null;
        }

        public override void OnPreprocess() => this.Fsm.HandleControllerColliderHit = true;

        private void StoreTriggerInfo()
        {
            if (this.Fsm.ControllerCollider == null)
                return;
            this.gameObjectHit.Value = this.Fsm.ControllerCollider.gameObject;
            this.contactPoint.Value = this.Fsm.ControllerCollider.point;
            this.contactNormal.Value = this.Fsm.ControllerCollider.normal;
            this.moveDirection.Value = this.Fsm.ControllerCollider.moveDirection;
            this.moveLength.Value = this.Fsm.ControllerCollider.moveLength;
            this.physicsMaterialName.Value = this.Fsm.ControllerCollider.collider.material.name;
        }

        public override void OnEnter()
        {
            this.StoreTriggerInfo();
            this.Finish();
        }

        public override string ErrorCheck() => ActionHelpers.CheckOwnerPhysicsSetup(this.Owner);
    }
}
