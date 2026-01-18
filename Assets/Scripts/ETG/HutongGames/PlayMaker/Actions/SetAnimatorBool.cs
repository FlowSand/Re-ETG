using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Sets the value of a bool parameter")]
    [ActionCategory(ActionCategory.Animator)]
    public class SetAnimatorBool : FsmStateActionAnimatorBase
    {
        [HutongGames.PlayMaker.Tooltip("The target")]
        [RequiredField]
        [CheckForComponent(typeof (Animator))]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The animator parameter")]
        [UIHint(UIHint.AnimatorBool)]
        [RequiredField]
        public FsmString parameter;
        [HutongGames.PlayMaker.Tooltip("The Bool value to assign to the animator parameter")]
        public FsmBool Value;
        private Animator _animator;
        private int _paramID;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = (FsmOwnerDefault) null;
            this.parameter = (FsmString) null;
            this.Value = (FsmBool) null;
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
                    this._paramID = Animator.StringToHash(this.parameter.Value);
                    this.SetParameter();
                    if (this.everyFrame)
                        return;
                    this.Finish();
                }
            }
        }

        public override void OnActionUpdate() => this.SetParameter();

        private void SetParameter()
        {
            if (!((Object) this._animator != (Object) null))
                return;
            this._animator.SetBool(this._paramID, this.Value.Value);
        }
    }
}
