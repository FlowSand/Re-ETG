using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [HutongGames.PlayMaker.Tooltip("Returns the pivot weight and/or position. The pivot is the most stable point between the avatar's left and right foot.\n For a weight value of 0, the left foot is the most stable point For a value of 1, the right foot is the most stable point")]
    public class GetAnimatorPivot : FsmStateActionAnimatorBase
    {
        [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        [CheckForComponent(typeof (Animator))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The pivot is the most stable point between the avatar's left and right foot.\n For a value of 0, the left foot is the most stable point For a value of 1, the right foot is the most stable point")]
        [UIHint(UIHint.Variable)]
        [ActionSection("Results")]
        public FsmFloat pivotWeight;
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("The pivot is the most stable point between the avatar's left and right foot.\n For a value of 0, the left foot is the most stable point For a value of 1, the right foot is the most stable point")]
        public FsmVector3 pivotPosition;
        private Animator _animator;

        public override void Reset()
        {
            base.Reset();
            this.gameObject = (FsmOwnerDefault) null;
            this.pivotWeight = (FsmFloat) null;
            this.pivotPosition = (FsmVector3) null;
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
                    this.DoCheckPivot();
                    if (this.everyFrame)
                        return;
                    this.Finish();
                }
            }
        }

        public override void OnActionUpdate() => this.DoCheckPivot();

        private void DoCheckPivot()
        {
            if ((Object) this._animator == (Object) null)
                return;
            if (!this.pivotWeight.IsNone)
                this.pivotWeight.Value = this._animator.pivotWeight;
            if (this.pivotPosition.IsNone)
                return;
            this.pivotPosition.Value = this._animator.pivotPosition;
        }
    }
}
