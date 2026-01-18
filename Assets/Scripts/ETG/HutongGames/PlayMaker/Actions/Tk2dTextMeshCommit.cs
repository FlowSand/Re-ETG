using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HelpUrl("https://hutonggames.fogbugz.com/default.asp?W723")]
    [HutongGames.PlayMaker.Tooltip("Commit a TextMesh. This is so you can change multiple parameters without reconstructing the mesh repeatedly, simply use that after you have set all the different properties. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
    [ActionCategory("2D Toolkit/TextMesh")]
    public class Tk2dTextMeshCommit : FsmStateAction
    {
        [CheckForComponent(typeof (tk2dTextMesh))]
        [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        private tk2dTextMesh _textMesh;

        private void _getTextMesh()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            this._textMesh = ownerDefaultTarget.GetComponent<tk2dTextMesh>();
        }

        public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

        public override void OnEnter()
        {
            this._getTextMesh();
            this.DoCommit();
            this.Finish();
        }

        private void DoCommit()
        {
            if ((Object) this._textMesh == (Object) null)
                this.LogWarning("Missing tk2dTextMesh component: " + this._textMesh.gameObject.name);
            else
                this._textMesh.Commit();
        }
    }
}
