using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("GUILayout Button. Sends an Event when pressed. Optionally stores the button state in a Bool Variable.")]
    [ActionCategory(ActionCategory.GUILayout)]
    public class GUILayoutButton : GUILayoutAction
    {
        public FsmEvent sendEvent;
        [UIHint(UIHint.Variable)]
        public FsmBool storeButtonState;
        public FsmTexture image;
        public FsmString text;
        public FsmString tooltip;
        public FsmString style;

        public override void Reset()
        {
            base.Reset();
            this.sendEvent = (FsmEvent) null;
            this.storeButtonState = (FsmBool) null;
            this.text = (FsmString) string.Empty;
            this.image = (FsmTexture) null;
            this.tooltip = (FsmString) string.Empty;
            this.style = (FsmString) string.Empty;
        }

        public override void OnGUI()
        {
            bool flag = !string.IsNullOrEmpty(this.style.Value) ? GUILayout.Button(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), (GUIStyle) this.style.Value, this.LayoutOptions) : GUILayout.Button(new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value), this.LayoutOptions);
            if (flag)
                this.Fsm.Event(this.sendEvent);
            if (this.storeButtonState == null)
                return;
            this.storeButtonState.Value = flag;
        }
    }
}
