using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Sets the playback speed of the Animator. 1 is normal playback speed")]
    [ActionCategory(ActionCategory.Animator)]
    public class SetAnimatorSpeed : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        [CheckForComponent(typeof (Animator))]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The playBack speed")]
        public FsmFloat speed;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful for changing over time.")]
        public bool everyFrame;
        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.speed = (FsmFloat) null;
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
                    this.DoPlaybackSpeed();
                    if (this.everyFrame)
                        return;
                    this.Finish();
                }
            }
        }

        public override void OnUpdate() => this.DoPlaybackSpeed();

        private void DoPlaybackSpeed()
        {
            if ((Object) this._animator == (Object) null)
                return;
            this._animator.speed = this.speed.Value;
        }
    }
}
