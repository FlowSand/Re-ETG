using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the Tinting Color for all background elements rendered by the GUI. By default only effects GUI rendered by this FSM, check Apply Globally to effect all GUI controls.")]
  [ActionCategory(ActionCategory.GUI)]
  public class SetGUIBackgroundColor : FsmStateAction
  {
    [RequiredField]
    public FsmColor backgroundColor;
    public FsmBool applyGlobally;

    public override void Reset() => this.backgroundColor = (FsmColor) Color.white;

    public override void OnGUI()
    {
      GUI.backgroundColor = this.backgroundColor.Value;
      if (!this.applyGlobally.Value)
        return;
      PlayMakerGUI.GUIBackgroundColor = GUI.backgroundColor;
    }
  }
}
