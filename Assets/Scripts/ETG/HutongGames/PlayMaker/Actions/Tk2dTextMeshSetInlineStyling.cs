using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Set the inlineStyling flag of a TextMesh. \nChanges will not be updated if commit is OFF. This is so you can change multiple parameters without reconstructing the mesh repeatedly.\n Use tk2dtextMeshCommit or set commit to true on your last change for that mesh. \nNOTE: The Game Object must have a tk2dTextMesh attached.")]
    [ActionCategory("2D Toolkit/TextMesh")]
    public class Tk2dTextMeshSetInlineStyling : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The Game Object to work with. NOTE: The Game Object must have a tk2dTextMesh component attached.")]
        [CheckForComponent(typeof (tk2dTextMesh))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Does the text features inline styling?")]
        [UIHint(UIHint.FsmBool)]
        public FsmBool inlineStyling;
        [UIHint(UIHint.FsmString)]
        [HutongGames.PlayMaker.Tooltip("Commit changes")]
        public FsmBool commit;
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
            this.inlineStyling = (FsmBool) true;
            this.commit = (FsmBool) true;
        }

        public override void OnEnter()
        {
            this._getTextMesh();
            this.DoSetInlineStyling();
            this.Finish();
        }

        private void DoSetInlineStyling()
        {
            if ((Object) this._textMesh == (Object) null)
            {
                this.LogWarning("Missing tk2dTextMesh component: ");
            }
            else
            {
                if (this._textMesh.inlineStyling == this.inlineStyling.Value)
                    return;
                this._textMesh.inlineStyling = this.inlineStyling.Value;
                if (!this.commit.Value)
                    return;
                this._textMesh.Commit();
            }
        }
    }
}
