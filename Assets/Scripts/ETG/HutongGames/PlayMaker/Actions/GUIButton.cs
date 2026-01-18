using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("GUI button. Sends an Event when pressed. Optionally store the button state in a Bool Variable.")]
    [ActionCategory(ActionCategory.GUI)]
    public class GUIButton : GUIContentAction
    {
        public FsmEvent sendEvent;
        [UIHint(UIHint.Variable)]
        public FsmBool storeButtonState;

        public override void Reset()
        {
            base.Reset();
            this.sendEvent = (FsmEvent) null;
            this.storeButtonState = (FsmBool) null;
            this.style = (FsmString) "Button";
        }

        public override void OnGUI()
        {
            base.OnGUI();
            bool flag = false;
            if (GUI.Button(this.rect, this.content, (GUIStyle) this.style.Value))
            {
                this.Fsm.Event(this.sendEvent);
                flag = true;
            }
            if (this.storeButtonState == null)
                return;
            this.storeButtonState.Value = flag;
        }
    }
}
