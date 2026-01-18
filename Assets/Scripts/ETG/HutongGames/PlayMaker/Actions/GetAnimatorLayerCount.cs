using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Returns the Animator controller layer count")]
    [ActionCategory(ActionCategory.Animator)]
    public class GetAnimatorLayerCount : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        [CheckForComponent(typeof (Animator))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [RequiredField]
        [ActionSection("Results")]
        [HutongGames.PlayMaker.Tooltip("The Animator controller layer count")]
        [UIHint(UIHint.Variable)]
        public FsmInt layerCount;
        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.layerCount = (FsmInt) null;
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
                    this.DoGetLayerCount();
                    this.Finish();
                }
            }
        }

        private void DoGetLayerCount()
        {
            if ((Object) this._animator == (Object) null)
                return;
            this.layerCount.Value = this._animator.layerCount;
        }
    }
}
