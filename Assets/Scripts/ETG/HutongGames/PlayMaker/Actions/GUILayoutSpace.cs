using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Inserts a space in the current layout group.")]
  [ActionCategory(ActionCategory.GUILayout)]
  public class GUILayoutSpace : FsmStateAction
  {
    public FsmFloat space;

    public override void Reset() => this.space = (FsmFloat) 10f;

    public override void OnGUI() => GUILayout.Space(this.space.Value);
  }
}
