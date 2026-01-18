using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Inserts a flexible space element.")]
    [ActionCategory(ActionCategory.GUILayout)]
    public class GUILayoutFlexibleSpace : FsmStateAction
    {
        public override void Reset()
        {
        }

        public override void OnGUI() => GUILayout.FlexibleSpace();
    }
}
