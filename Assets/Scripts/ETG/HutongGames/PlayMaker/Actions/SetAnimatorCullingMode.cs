using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [HutongGames.PlayMaker.Tooltip("Controls culling of this Animator component.\nIf true, set to 'AlwaysAnimate': always animate the entire character. Object is animated even when offscreen.\nIf False, set to 'BasedOnRenderes' or CullUpdateTransforms ( On Unity 5) animation is disabled when renderers are not visible.")]
    public class SetAnimatorCullingMode : FsmStateAction
    {
        [CheckForComponent(typeof (Animator))]
        [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("If true, always animate the entire character, else animation is disabled when renderers are not visible")]
        public FsmBool alwaysAnimate;
        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.alwaysAnimate = (FsmBool) null;
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
                    this.SetCullingMode();
                    this.Finish();
                }
            }
        }

        private void SetCullingMode()
        {
            if ((Object) this._animator == (Object) null)
                return;
            this._animator.cullingMode = !this.alwaysAnimate.Value ? AnimatorCullingMode.CullUpdateTransforms : AnimatorCullingMode.AlwaysAnimate;
        }
    }
}
