using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Returns the feet pivot. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
    [ActionCategory(ActionCategory.Animator)]
    public class GetAnimatorFeetPivotActive : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        [CheckForComponent(typeof (Animator))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The feet pivot Blending. At 0% blending point is body mass center. At 100% blending point is feet pivot")]
        [UIHint(UIHint.Variable)]
        [RequiredField]
        public FsmFloat feetPivotActive;
        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.feetPivotActive = (FsmFloat) null;
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
                    this.DoGetFeetPivotActive();
                    this.Finish();
                }
            }
        }

        private void DoGetFeetPivotActive()
        {
            if ((Object) this._animator == (Object) null)
                return;
            this.feetPivotActive.Value = this._animator.feetPivotActive;
        }
    }
}
