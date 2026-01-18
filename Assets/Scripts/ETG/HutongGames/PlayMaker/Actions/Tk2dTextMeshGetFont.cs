using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Get the font of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
    [ActionCategory("2D Toolkit/TextMesh")]
    public class Tk2dTextMeshGetFont : FsmStateAction
    {
        [CheckForComponent(typeof (tk2dTextMesh))]
        [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.FsmGameObject)]
        [HutongGames.PlayMaker.Tooltip("The font gameObject")]
        [RequiredField]
        public FsmGameObject font;
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
            this.font = (FsmGameObject) null;
        }

        public override void OnEnter()
        {
            this._getTextMesh();
            this.DoGetFont();
            this.Finish();
        }

        private void DoGetFont()
        {
            if ((Object) this._textMesh == (Object) null)
            {
                this.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
            }
            else
            {
                GameObject gameObject = this.font.Value;
                if ((Object) gameObject == (Object) null || !((Object) gameObject.GetComponent<tk2dFont>() == (Object) null))
                    ;
            }
        }
    }
}
