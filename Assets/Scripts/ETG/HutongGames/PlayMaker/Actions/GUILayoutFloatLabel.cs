using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GUILayout)]
    [HutongGames.PlayMaker.Tooltip("GUILayout Label for a Float Variable.")]
    public class GUILayoutFloatLabel : GUILayoutAction
    {
        [HutongGames.PlayMaker.Tooltip("Text to put before the float variable.")]
        public FsmString prefix;
        [HutongGames.PlayMaker.Tooltip("Float variable to display.")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmFloat floatVariable;
        [HutongGames.PlayMaker.Tooltip("Optional GUIStyle in the active GUISKin.")]
        public FsmString style;

        public override void Reset()
        {
            base.Reset();
            this.prefix = (FsmString) string.Empty;
            this.style = (FsmString) string.Empty;
            this.floatVariable = (FsmFloat) null;
        }

        public override void OnGUI()
        {
            if (string.IsNullOrEmpty(this.style.Value))
                GUILayout.Label(new GUIContent(this.prefix.Value + (object) this.floatVariable.Value), this.LayoutOptions);
            else
                GUILayout.Label(new GUIContent(this.prefix.Value + (object) this.floatVariable.Value), (GUIStyle) this.style.Value, this.LayoutOptions);
        }
    }
}
