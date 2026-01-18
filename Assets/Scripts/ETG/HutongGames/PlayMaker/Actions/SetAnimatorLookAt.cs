using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Sets look at position and weights. A GameObject can be set to control the look at position, or it can be manually expressed.")]
    [ActionCategory(ActionCategory.Animator)]
    public class SetAnimatorLookAt : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required.")]
        [CheckForComponent(typeof (Animator))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The gameObject to look at")]
        public FsmGameObject target;
        [HutongGames.PlayMaker.Tooltip("The lookat position. If Target GameObject set, targetPosition is used as an offset from Target")]
        public FsmVector3 targetPosition;
        [HutongGames.PlayMaker.Tooltip("The global weight of the LookAt, multiplier for other parameters. Range from 0 to 1")]
        [HasFloatSlider(0.0f, 1f)]
        public FsmFloat weight;
        [HutongGames.PlayMaker.Tooltip("determines how much the body is involved in the LookAt. Range from 0 to 1")]
        [HasFloatSlider(0.0f, 1f)]
        public FsmFloat bodyWeight;
        [HutongGames.PlayMaker.Tooltip("determines how much the head is involved in the LookAt. Range from 0 to 1")]
        [HasFloatSlider(0.0f, 1f)]
        public FsmFloat headWeight;
        [HutongGames.PlayMaker.Tooltip("determines how much the eyes are involved in the LookAt. Range from 0 to 1")]
        [HasFloatSlider(0.0f, 1f)]
        public FsmFloat eyesWeight;
        [HutongGames.PlayMaker.Tooltip("0.0 means the character is completely unrestrained in motion, 1.0 means he's completely clamped (look at becomes impossible), and 0.5 means he'll be able to move on half of the possible range (180 degrees).")]
        [HasFloatSlider(0.0f, 1f)]
        public FsmFloat clampWeight;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame during OnAnimatorIK(). Useful for changing over time.")]
        public bool everyFrame;
        private Animator _animator;
        private Transform _transform;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.target = (FsmGameObject) null;
            FsmVector3 fsmVector3 = new FsmVector3();
            fsmVector3.UseVariable = true;
            this.targetPosition = fsmVector3;
            this.weight = (FsmFloat) 1f;
            this.bodyWeight = (FsmFloat) 0.3f;
            this.headWeight = (FsmFloat) 0.6f;
            this.eyesWeight = (FsmFloat) 1f;
            this.clampWeight = (FsmFloat) 0.5f;
            this.everyFrame = false;
        }

        public override void OnPreprocess() => this.Fsm.HandleAnimatorIK = true;

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
                    GameObject gameObject = this.target.Value;
                    if (!((Object) gameObject != (Object) null))
                        return;
                    this._transform = gameObject.transform;
                }
            }
        }

        public override void DoAnimatorIK(int layerIndex)
        {
            this.DoSetLookAt();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        private void DoSetLookAt()
        {
            if ((Object) this._animator == (Object) null)
                return;
            if ((Object) this._transform != (Object) null)
            {
                if (this.targetPosition.IsNone)
                    this._animator.SetLookAtPosition(this._transform.position);
                else
                    this._animator.SetLookAtPosition(this._transform.position + this.targetPosition.Value);
            }
            else if (!this.targetPosition.IsNone)
                this._animator.SetLookAtPosition(this.targetPosition.Value);
            if (!this.clampWeight.IsNone)
                this._animator.SetLookAtWeight(this.weight.Value, this.bodyWeight.Value, this.headWeight.Value, this.eyesWeight.Value, this.clampWeight.Value);
            else if (!this.eyesWeight.IsNone)
                this._animator.SetLookAtWeight(this.weight.Value, this.bodyWeight.Value, this.headWeight.Value, this.eyesWeight.Value);
            else if (!this.headWeight.IsNone)
                this._animator.SetLookAtWeight(this.weight.Value, this.bodyWeight.Value, this.headWeight.Value);
            else if (!this.bodyWeight.IsNone)
            {
                this._animator.SetLookAtWeight(this.weight.Value, this.bodyWeight.Value);
            }
            else
            {
                if (this.weight.IsNone)
                    return;
                this._animator.SetLookAtWeight(this.weight.Value);
            }
        }
    }
}
