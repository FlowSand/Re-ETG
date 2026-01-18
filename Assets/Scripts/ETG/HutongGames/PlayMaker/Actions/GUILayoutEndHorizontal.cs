using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Close a group started with BeginHorizontal.")]
  [ActionCategory(ActionCategory.GUILayout)]
  public class GUILayoutEndHorizontal : FsmStateAction
  {
    public override void Reset()
    {
    }

    public override void OnGUI() => GUILayout.EndHorizontal();
  }
}
