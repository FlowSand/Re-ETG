using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("2D Toolkit/TextMesh")]
    [HutongGames.PlayMaker.Tooltip("Get the scale of a TextMesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
    public class Tk2dTextMeshGetScale : FsmStateAction
    {
        [CheckForComponent(typeof (tk2dTextMesh))]
        [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [RequiredField]
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("The scale")]
        public FsmVector3 scale;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        [ActionSection("")]
        public bool everyframe;
        private GameObject go;
        private tk2dTextMesh _textMesh;

        private void _getTextMesh()
        {
            this.go = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) this.go == (Object) null)
                return;
            this._textMesh = this.go.GetComponent<tk2dTextMesh>();
        }

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.scale = (FsmVector3) null;
            this.everyframe = false;
        }

        public override void OnEnter()
        {
            this._getTextMesh();
            this.DoGetScale();
            if (this.everyframe)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoGetScale();

        private void DoGetScale()
        {
            if ((Object) this.go == (Object) null)
                return;
            if ((Object) this._textMesh == (Object) null)
            {
                Debug.Log((object) this._textMesh);
                this.LogError("Missing tk2dTextMesh component: " + this.go.name);
            }
            else
                this.scale.Value = this._textMesh.scale;
        }
    }
}
