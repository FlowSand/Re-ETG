using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("GUILayout Password Field. Optionally send an event if the text has been edited.")]
    [ActionCategory(ActionCategory.GUILayout)]
    public class GUILayoutEmailField : GUILayoutAction
    {
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("The email Text")]
        public FsmString text;
        [HutongGames.PlayMaker.Tooltip("The Maximum Length of the field")]
        public FsmInt maxLength;
        [HutongGames.PlayMaker.Tooltip("The Style of the Field")]
        public FsmString style;
        [HutongGames.PlayMaker.Tooltip("Event sent when field content changed")]
        public FsmEvent changedEvent;
        [HutongGames.PlayMaker.Tooltip("Email valid format flag")]
        public FsmBool valid;

        public override void Reset()
        {
            this.text = (FsmString) null;
            this.maxLength = (FsmInt) 25;
            this.style = (FsmString) "TextField";
            this.valid = (FsmBool) true;
            this.changedEvent = (FsmEvent) null;
        }

        public override void OnGUI()
        {
            bool changed = GUI.changed;
            GUI.changed = false;
            this.text.Value = GUILayout.TextField(this.text.Value, (GUIStyle) this.style.Value, this.LayoutOptions);
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
