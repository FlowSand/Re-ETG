using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Sets the Tinting Color for all text rendered by the GUI. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
    [ActionCategory(ActionCategory.GUI)]
    public class SetGUIContentColor : FsmStateAction
    {
        [RequiredField]
        public FsmColor contentColor;
        public FsmBool applyGlobally;

        public override void Reset() => this.contentColor = (FsmColor) Color.white;

        public override void OnGUI()
        {
            GUI.contentColor = this.contentColor.Value;
            if (!this.applyGlobally.Value)
                return;
            PlayMakerGUI.GUIContentColor = GUI.contentColor;
        }
    }
}
