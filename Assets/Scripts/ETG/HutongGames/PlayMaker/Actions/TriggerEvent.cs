using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Physics)]
    [HutongGames.PlayMaker.Tooltip("Detect collisions with objects that have RigidBody components. \nNOTE: The system events, TRIGGER ENTER, TRIGGER STAY, and TRIGGER EXIT are sent when any object collides with the trigger. Use this action to filter collisions by Tag.")]
    public class TriggerEvent : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The type of trigger event to detect.")]
        public TriggerType trigger;
        [HutongGames.PlayMaker.Tooltip("Filter by Tag.")]
        [UIHint(UIHint.Tag)]
        public FsmString collideTag;
        [HutongGames.PlayMaker.Tooltip("Event to send if the trigger event is detected.")]
        public FsmEvent sendEvent;
        [HutongGames.PlayMaker.Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
        [UIHint(UIHint.Variable)]
        public FsmGameObject storeCollider;

        public override void Reset()
        {
            this.trigger = TriggerType.OnTriggerEnter;
            this.collideTag = (FsmString) "Untagged";
            this.sendEvent = (FsmEvent) null;
            this.storeCollider = (FsmGameObject) null;
        }

        public override void OnPreprocess()
        {
            switch (this.trigger)
            {
                case TriggerType.OnTriggerEnter:
                    this.Fsm.HandleTriggerEnter = true;
                    break;
                case TriggerType.OnTriggerStay:
                    this.Fsm.HandleTriggerStay = true;
                    break;
                case TriggerType.OnTriggerExit:
                    this.Fsm.HandleTriggerExit = true;
                    break;
            }
        }

        private void StoreCollisionInfo(Collider collisionInfo)
        {
            this.storeCollider.Value = collisionInfo.gameObject;
        }

        public override void DoTriggerEnter(Collider other)
        {
            if (this.trigger != TriggerType.OnTriggerEnter || !(other.gameObject.tag == this.collideTag.Value))
                return;
            this.StoreCollisionInfo(other);
            this.Fsm.Event(this.sendEvent);
        }

        public override void DoTriggerStay(Collider other)
        {
            if (this.trigger != TriggerType.OnTriggerStay || !(other.gameObject.tag == this.collideTag.Value))
                return;
            this.StoreCollisionInfo(other);
            this.Fsm.Event(this.sendEvent);
        }

        public override void DoTriggerExit(Collider other)
        {
            if (this.trigger != TriggerType.OnTriggerExit || !(other.gameObject.tag == this.collideTag.Value))
                return;
            this.StoreCollisionInfo(other);
            this.Fsm.Event(this.sendEvent);
        }

        public override string ErrorCheck() => ActionHelpers.CheckOwnerPhysicsSetup(this.Owner);
    }
}
