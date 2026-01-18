using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Returns The current gravity weight based on current animations that are played")]
    [ActionCategory(ActionCategory.Animator)]
    public class GetAnimatorGravityWeight : FsmStateActionAnimatorBase
    {
        [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        [CheckForComponent(typeof (Animator))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The current gravity weight based on current animations that are played")]
        [UIHint(UIHint.Variable)]
        [ActionSection("Results")]
        public FsmFloat gravityWeight;
        private Animator _animator;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = (FsmOwnerDefault) null;
            this.gravityWeight = (FsmFloat) null;
            this.everyFrame = false;
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
                    this.DoGetGravityWeight();
                    if (this.everyFrame)
                        return;
                    this.Finish();
                }
            }
        }

        public override void OnActionUpdate() => this.DoGetGravityWeight();

        private void DoGetGravityWeight()
        {
            if ((Object) this._animator == (Object) null)
                return;
            this.gravityWeight.Value = this._animator.gravityWeight;
        }
    }
}
