using System;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Obsolete("This action is obsolete; use Send Event with Event Target instead.")]
    [HutongGames.PlayMaker.Tooltip("Sends an Event to all FSMs in the scene or to all FSMs on a Game Object.\nNOTE: This action won't work on the very first frame of the game...")]
    [ActionCategory(ActionCategory.StateMachine)]
    public class BroadcastEvent : FsmStateAction
    {
        [RequiredField]
        public FsmString broadcastEvent;
        [HutongGames.PlayMaker.Tooltip("Optionally specify a game object to broadcast the event to all FSMs on that game object.")]
        public FsmGameObject gameObject;
        [HutongGames.PlayMaker.Tooltip("Broadcast to all FSMs on the game object's children.")]
        public FsmBool sendToChildren;
        public FsmBool excludeSelf;

        public override void Reset()
        {
            this.broadcastEvent = (FsmString) null;
            this.gameObject = (FsmGameObject) null;
            this.sendToChildren = (FsmBool) false;
            this.excludeSelf = (FsmBool) false;
        }

        public override void OnEnter()
        {
            if (!string.IsNullOrEmpty(this.broadcastEvent.Value))
            {
                if ((UnityEngine.Object) this.gameObject.Value != (UnityEngine.Object) null)
                    this.Fsm.BroadcastEventToGameObject(this.gameObject.Value, this.broadcastEvent.Value, this.sendToChildren.Value, this.excludeSelf.Value);
                else
                    this.Fsm.BroadcastEvent(this.broadcastEvent.Value, this.excludeSelf.Value);
            }
            this.Finish();
        }
    }
}
