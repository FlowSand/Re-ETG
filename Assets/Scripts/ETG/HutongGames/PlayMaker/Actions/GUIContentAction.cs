using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("GUI base action - don't use!")]
    public abstract class GUIContentAction : GUIAction
    {
        public FsmTexture image;
        public FsmString text;
        public FsmString tooltip;
        public FsmString style;
        internal GUIContent content;

        public override void Reset()
        {
            base.Reset();
            this.image = (FsmTexture) null;
            this.text = (FsmString) string.Empty;
            this.tooltip = (FsmString) string.Empty;
            this.style = (FsmString) string.Empty;
        }

        public override void OnGUI()
        {
            base.OnGUI();
            this.content = new GUIContent(this.text.Value, this.image.Value, this.tooltip.Value);
        }
    }
}
