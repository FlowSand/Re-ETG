using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Sets the Text used by the GUIText Component attached to a Game Object.")]
    [ActionCategory(ActionCategory.GUIElement)]
    public class SetGUIText : ComponentAction<GUIText>
    {
        [RequiredField]
        [CheckForComponent(typeof (GUIText))]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.TextArea)]
        public FsmString text;
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.text = (FsmString) string.Empty;
        }

        public override void OnEnter()
        {
            this.DoSetGUIText();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoSetGUIText();

        private void DoSetGUIText()
        {
            if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
                return;
            this.guiText.text = this.text.Value;
        }
    }
}
