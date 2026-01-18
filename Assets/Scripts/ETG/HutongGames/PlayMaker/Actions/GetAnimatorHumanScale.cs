using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [HutongGames.PlayMaker.Tooltip("Returns the scale of the current Avatar for a humanoid rig, (1 by default if the rig is generic).\n The scale is relative to Unity's Default Avatar")]
    public class GetAnimatorHumanScale : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        [CheckForComponent(typeof (Animator))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("the scale of the current Avatar")]
        [UIHint(UIHint.Variable)]
        [ActionSection("Result")]
        public FsmFloat humanScale;
        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.humanScale = (FsmFloat) null;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget == (Object) null)
            {
                this.Finish();
            }
            else
            {
                this._animator = ownerDefaultTarget.GetComponent<Animator>();
                if ((Object) this._animator == (Object) null)
                {
                    this.Finish();
                }
                else
                {
                    this.DoGetHumanScale();
                    this.Finish();
                }
            }
        }

        private void DoGetHumanScale()
        {
            if ((Object) this._animator == (Object) null)
                return;
            this.humanScale.Value = this._animator.humanScale;
        }
    }
}
