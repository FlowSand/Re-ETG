using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory("2D Toolkit/TextMesh")]
    [HutongGames.PlayMaker.Tooltip("Set the pixelPerfect flag of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
    public class Tk2dTextMeshSetPixelPerfect : FsmStateAction
    {
        [CheckForComponent(typeof (tk2dTextMesh))]
        [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Does the text needs to be pixelPerfect")]
        [UIHint(UIHint.FsmBool)]
        public FsmBool pixelPerfect;
        [HutongGames.PlayMaker.Tooltip("Commit changes")]
        [UIHint(UIHint.FsmString)]
        public FsmBool commit;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
        [ActionSection("")]
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
            this.pixelPerfect = (FsmBool) true;
            this.commit = (FsmBool) true;
            this.everyframe = false;
        }

        public override void OnEnter()
        {
            this._getTextMesh();
            this.DoSetPixelPerfect();
            if (this.everyframe)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoSetPixelPerfect();

        private void DoSetPixelPerfect()
        {
            if ((Object) this._textMesh == (Object) null)
            {
                this.LogWarning("Missing tk2dTextMesh component: ");
            }
            else
            {
                if (!this.pixelPerfect.Value)
                    return;
                this._textMesh.MakePixelPerfect();
                if (!this.commit.Value)
                    return;
                this._textMesh.Commit();
            }
        }
    }
}
