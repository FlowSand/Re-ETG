using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [HutongGames.PlayMaker.Tooltip("If true, additionnal layers affects the mass center")]
    public class SetAnimatorLayersAffectMassCenter : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        [CheckForComponent(typeof (Animator))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("If true, additionnal layers affects the mass center")]
        public FsmBool affectMassCenter;
        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.affectMassCenter = (FsmBool) null;
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
                    this.SetAffectMassCenter();
                    this.Finish();
                }
            }
        }

        private void SetAffectMassCenter()
        {
            if ((Object) this._animator == (Object) null)
                return;
            this._animator.layersAffectMassCenter = this.affectMassCenter.Value;
        }
    }
}
