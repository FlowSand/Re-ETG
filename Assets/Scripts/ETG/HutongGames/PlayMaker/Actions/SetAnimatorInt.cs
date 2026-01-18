using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [HutongGames.PlayMaker.Tooltip("Sets the value of a int parameter")]
    public class SetAnimatorInt : FsmStateActionAnimatorBase
    {
        [HutongGames.PlayMaker.Tooltip("The target.")]
        [CheckForComponent(typeof (Animator))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The animator parameter")]
        [UIHint(UIHint.AnimatorInt)]
        [RequiredField]
        public FsmString parameter;
        [HutongGames.PlayMaker.Tooltip("The Int value to assign to the animator parameter")]
        public FsmInt Value;
        private Animator _animator;
        private int _paramID;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = (FsmOwnerDefault) null;
            this.parameter = (FsmString) null;
            this.Value = (FsmInt) null;
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
            this._animator.SetInteger(this._paramID, this.Value.Value);
        }
    }
}
