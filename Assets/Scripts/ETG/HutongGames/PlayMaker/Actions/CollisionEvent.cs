using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Detect collisions between the Owner of this FSM and other Game Objects that have RigidBody components.\nNOTE: The system events, COLLISION ENTER, COLLISION STAY, and COLLISION EXIT are sent automatically on collisions with any object. Use this action to filter collisions by Tag.")]
    [ActionCategory(ActionCategory.Physics)]
    public class CollisionEvent : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The type of collision to detect.")]
        public CollisionType collision;
        [HutongGames.PlayMaker.Tooltip("Filter by Tag.")]
        [UIHint(UIHint.Tag)]
        public FsmString collideTag;
        [HutongGames.PlayMaker.Tooltip("Event to send if a collision is detected.")]
        public FsmEvent sendEvent;
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
        public FsmGameObject storeCollider;
        [HutongGames.PlayMaker.Tooltip("Store the force of the collision. NOTE: Use Get Collision Info to get more info about the collision.")]
        [UIHint(UIHint.Variable)]
        public FsmFloat storeForce;

        public override void Reset()
        {
            this.collision = CollisionType.OnCollisionEnter;
            this.collideTag = (FsmString) "Untagged";
            this.sendEvent = (FsmEvent) null;
            this.storeCollider = (FsmGameObject) null;
            this.storeForce = (FsmFloat) null;
        }

        public override void OnPreprocess()
        {
            switch (this.collision)
            {
                case CollisionType.OnCollisionEnter:
                    this.Fsm.HandleCollisionEnter = true;
                    break;
                case CollisionType.OnCollisionStay:
                    this.Fsm.HandleCollisionStay = true;
                    break;
                case CollisionType.OnCollisionExit:
                    this.Fsm.HandleCollisionExit = true;
                    break;
                case CollisionType.OnControllerColliderHit:
                    this.Fsm.HandleControllerColliderHit = true;
                    break;
                case CollisionType.OnParticleCollision:
                    this.Fsm.HandleParticleCollision = true;
                    break;
            }
        }

        private void StoreCollisionInfo(Collision collisionInfo)
        {
            this.storeCollider.Value = collisionInfo.gameObject;
            this.storeForce.Value = collisionInfo.relativeVelocity.magnitude;
        }

        public override void DoCollisionEnter(Collision collisionInfo)
        {
            if (this.collision != CollisionType.OnCollisionEnter || !(collisionInfo.collider.gameObject.tag == this.collideTag.Value))
                return;
            this.StoreCollisionInfo(collisionInfo);
            this.Fsm.Event(this.sendEvent);
        }

        public override void DoCollisionStay(Collision collisionInfo)
        {
            if (this.collision != CollisionType.OnCollisionStay || !(collisionInfo.collider.gameObject.tag == this.collideTag.Value))
                return;
            this.StoreCollisionInfo(collisionInfo);
            this.Fsm.Event(this.sendEvent);
        }

        public override void DoCollisionExit(Collision collisionInfo)
        {
            if (this.collision != CollisionType.OnCollisionExit || !(collisionInfo.collider.gameObject.tag == this.collideTag.Value))
                return;
            this.StoreCollisionInfo(collisionInfo);
            this.Fsm.Event(this.sendEvent);
        }

        public override void DoControllerColliderHit(ControllerColliderHit collisionInfo)
        {
            if (this.collision != CollisionType.OnControllerColliderHit || !(collisionInfo.collider.gameObject.tag == this.collideTag.Value))
                return;
            if (this.storeCollider != null)
                this.storeCollider.Value = collisionInfo.gameObject;
            this.storeForce.Value = 0.0f;
            this.Fsm.Event(this.sendEvent);
        }

        public override void DoParticleCollision(GameObject other)
        {
            if (this.collision != CollisionType.OnParticleCollision || !(other.tag == this.collideTag.Value))
                return;
            if (this.storeCollider != null)
                this.storeCollider.Value = other;
            this.storeForce.Value = 0.0f;
            this.Fsm.Event(this.sendEvent);
        }

        public override string ErrorCheck() => ActionHelpers.CheckOwnerPhysicsSetup(this.Owner);
    }
}
