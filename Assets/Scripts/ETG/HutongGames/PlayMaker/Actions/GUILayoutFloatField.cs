using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("GUILayout Text Field to edit a Float Variable. Optionally send an event if the text has been edited.")]
    [ActionCategory(ActionCategory.GUILayout)]
    public class GUILayoutFloatField : GUILayoutAction
    {
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("Float Variable to show in the edit field.")]
        public FsmFloat floatVariable;
        [HutongGames.PlayMaker.Tooltip("Optional GUIStyle in the active GUISKin.")]
        public FsmString style;
        [HutongGames.PlayMaker.Tooltip("Optional event to send when the value changes.")]
        public FsmEvent changedEvent;

        public override void Reset()
        {
            base.Reset();
            this.floatVariable = (FsmFloat) null;
            this.style = (FsmString) string.Empty;
            this.changedEvent = (FsmEvent) null;
        }

        public override void OnGUI()
        {
            bool changed = GUI.changed;
            GUI.changed = false;
            this.floatVariable.Value = string.IsNullOrEmpty(this.style.Value) ? float.Parse(GUILayout.TextField(this.floatVariable.Value.ToString(), this.LayoutOptions)) : float.Parse(GUILayout.TextField(this.floatVariable.Value.ToString(), (GUIStyle) this.style.Value, this.LayoutOptions));
            if (GUI.changed)
            {
                this.Fsm.Event(this.changedEvent);
                GUIUtility.ExitGUI();
            }
            else
                GUI.changed = changed;
        }
    }
}
