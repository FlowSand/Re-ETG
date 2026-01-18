using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("2D Toolkit/TextMesh")]
    [HutongGames.PlayMaker.Tooltip("Get the text of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
    public class Tk2dTextMeshGetText : FsmStateAction
    {
        [CheckForComponent(typeof (tk2dTextMesh))]
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The text")]
        [UIHint(UIHint.Variable)]
        public FsmString text;
        [ActionSection("")]
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        public bool everyframe;
        private tk2dTextMesh _textMesh;

        private void _getTextMesh()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
        }

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.text = (FsmString) null;
            this.everyframe = false;
        }

        public override void OnEnter()
        {
            this._getTextMesh();
            this.DoGetText();
            if (this.everyframe)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoGetText();

        private void DoGetText()
        {
            if ((Object) this._textMesh == (Object) null)
                this.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
            else
                this.text.Value = this._textMesh.text;
        }
    }
}
