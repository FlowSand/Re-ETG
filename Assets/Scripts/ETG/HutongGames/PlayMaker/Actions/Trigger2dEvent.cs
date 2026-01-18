using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Physics2D)]
  [HutongGames.PlayMaker.Tooltip("Detect 2D trigger collisions between the Owner of this FSM and other Game Objects that have RigidBody2D components.\nNOTE: The system events, TRIGGER ENTER 2D, TRIGGER STAY 2D, and TRIGGER EXIT 2D are sent automatically on collisions triggers with any object. Use this action to filter collision triggers by Tag.")]
  public class Trigger2dEvent : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The type of trigger event to detect.")]
    public Trigger2DType trigger;
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
      this.trigger = Trigger2DType.OnTriggerEnter2D;
      this.collideTag = (FsmString) "Untagged";
      this.sendEvent = (FsmEvent) null;
      this.storeCollider = (FsmGameObject) null;
    }

    public override void OnPreprocess()
    {
      switch (this.trigger)
      {
        case Trigger2DType.OnTriggerEnter2D:
          this.Fsm.HandleTriggerEnter2D = true;
          break;
        case Trigger2DType.OnTriggerStay2D:
          this.Fsm.HandleTriggerStay2D = true;
          break;
        case Trigger2DType.OnTriggerExit2D:
          this.Fsm.HandleTriggerExit2D = true;
          break;
      }
    }

    private void StoreCollisionInfo(Collider2D collisionInfo)
    {
      this.storeCollider.Value = collisionInfo.gameObject;
    }

    public override void DoTriggerEnter2D(Collider2D other)
    {
      if (this.trigger != Trigger2DType.OnTriggerEnter2D || !(other.gameObject.tag == this.collideTag.Value))
        return;
      this.StoreCollisionInfo(other);
      this.Fsm.Event(this.sendEvent);
    }

    public override void DoTriggerStay2D(Collider2D other)
    {
      if (this.trigger != Trigger2DType.OnTriggerStay2D || !(other.gameObject.tag == this.collideTag.Value))
        return;
      this.StoreCollisionInfo(other);
      this.Fsm.Event(this.sendEvent);
    }

    public override void DoTriggerExit2D(Collider2D other)
    {
      if (this.trigger != Trigger2DType.OnTriggerExit2D || !(other.gameObject.tag == this.collideTag.Value))
        return;
      this.StoreCollisionInfo(other);
      this.Fsm.Event(this.sendEvent);
    }

    public override string ErrorCheck() => ActionHelpers.CheckOwnerPhysics2dSetup(this.Owner);
  }
}
