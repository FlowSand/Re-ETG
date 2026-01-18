using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Gets the Collision Flags from a Character Controller on a Game Object. Collision flags give you a broad overview of where the character collided with any other object.")]
    [ActionCategory(ActionCategory.Character)]
    public class GetControllerCollisionFlags : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject with a Character Controller component.")]
        [CheckForComponent(typeof (CharacterController))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("True if the Character Controller capsule is on the ground")]
        [UIHint(UIHint.Variable)]
        public FsmBool isGrounded;
        [HutongGames.PlayMaker.Tooltip("True if no collisions in last move.")]
        [UIHint(UIHint.Variable)]
        public FsmBool none;
        [HutongGames.PlayMaker.Tooltip("True if the Character Controller capsule was hit on the sides.")]
        [UIHint(UIHint.Variable)]
        public FsmBool sides;
        [HutongGames.PlayMaker.Tooltip("True if the Character Controller capsule was hit from above.")]
        [UIHint(UIHint.Variable)]
        public FsmBool above;
        [HutongGames.PlayMaker.Tooltip("True if the Character Controller capsule was hit from below.")]
        [UIHint(UIHint.Variable)]
        public FsmBool below;
        private GameObject previousGo;
        private CharacterController controller;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.isGrounded = (FsmBool) null;
            this.none = (FsmBool) null;
            this.sides = (FsmBool) null;
            this.above = (FsmBool) null;
            this.below = (FsmBool) null;
        }

        public override void OnUpdate()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            if ((Object) ownerDefaultTarget != (Object) this.previousGo)
            {
                this.controller = ownerDefaultTarget.GetComponent<CharacterController>();
                this.previousGo = ownerDefaultTarget;
            }
            if (!((Object) this.controller != (Object) null))
                return;
            this.isGrounded.Value = this.controller.isGrounded;
            FsmBool none = this.none;
            int collisionFlags = (int) this.controller.collisionFlags;
            none.Value = false;
            this.sides.Value = (this.controller.collisionFlags & CollisionFlags.Sides) != CollisionFlags.None;
            this.above.Value = (this.controller.collisionFlags & CollisionFlags.Above) != CollisionFlags.None;
            this.below.Value = (this.controller.collisionFlags & CollisionFlags.Below) != CollisionFlags.None;
        }
    }
}
