using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Gets the Tooltip of the control the mouse is currently over and store it in a String Variable.")]
  [ActionCategory(ActionCategory.GUI)]
  public class GUITooltip : FsmStateAction
  {
    [UIHint(UIHint.Variable)]
    public FsmString storeTooltip;

    public override void Reset() => this.storeTooltip = (FsmString) null;

    public override void OnGUI() => this.storeTooltip.Value = GUI.tooltip;
  }
}
