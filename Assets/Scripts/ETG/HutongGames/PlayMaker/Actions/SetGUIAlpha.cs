using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Sets the global Alpha for the GUI. Useful for fading GUI up/down. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
    [ActionCategory(ActionCategory.GUI)]
    public class SetGUIAlpha : FsmStateAction
    {
        [RequiredField]
        public FsmFloat alpha;
        public FsmBool applyGlobally;

        public override void Reset() => this.alpha = (FsmFloat) 1f;

        public override void OnGUI()
        {
            GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.alpha.Value);
            if (!this.applyGlobally.Value)
                return;
            PlayMakerGUI.GUIColor = GUI.color;
        }
    }
}
