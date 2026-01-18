using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("End a centered GUILayout block started with GUILayoutBeginCentered.")]
    [ActionCategory(ActionCategory.GUILayout)]
    public class GUILayoutEndCentered : FsmStateAction
    {
        public override void Reset()
        {
        }

        public override void OnGUI()
        {
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }
    }
}
