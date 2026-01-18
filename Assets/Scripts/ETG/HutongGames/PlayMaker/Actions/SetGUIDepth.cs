using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Sets the sorting depth of subsequent GUI elements.")]
  [ActionCategory(ActionCategory.GUI)]
  public class SetGUIDepth : FsmStateAction
  {
    [RequiredField]
    public FsmInt depth;

    public override void Reset() => this.depth = (FsmInt) 0;

    public override void OnPreprocess() => this.Fsm.HandleOnGUI = true;

    public override void OnGUI() => GUI.depth = this.depth.Value;
  }
}
