using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Sends an Event when a Key is released.")]
    [ActionCategory(ActionCategory.Input)]
    public class GetKeyUp : FsmStateAction
    {
        [RequiredField]
        public KeyCode key;
        public FsmEvent sendEvent;
        [UIHint(UIHint.Variable)]
        public FsmBool storeResult;

        public override void Reset()
        {
            this.sendEvent = (FsmEvent) null;
            this.key = KeyCode.None;
            this.storeResult = (FsmBool) null;
        }

        public override void OnUpdate()
        {
            bool keyUp = Input.GetKeyUp(this.key);
            if (keyUp)
                this.Fsm.Event(this.sendEvent);
            this.storeResult.Value = keyUp;
        }
    }
}
