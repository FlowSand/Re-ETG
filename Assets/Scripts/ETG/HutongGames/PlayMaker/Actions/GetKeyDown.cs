using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Sends an Event when a Key is pressed.")]
    [ActionCategory(ActionCategory.Input)]
    public class GetKeyDown : FsmStateAction
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
            bool keyDown = Input.GetKeyDown(this.key);
            if (keyDown)
                this.Fsm.Event(this.sendEvent);
            this.storeResult.Value = keyDown;
        }
    }
}
