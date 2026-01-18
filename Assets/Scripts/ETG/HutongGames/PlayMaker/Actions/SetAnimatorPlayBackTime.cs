using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [HutongGames.PlayMaker.Tooltip("Sets the playback position in the recording buffer. When in playback mode (use AnimatorStartPlayback), this value is used for controlling the current playback position in the buffer (in seconds). The value can range between recordingStartTime and recordingStopTime ")]
    public class SetAnimatorPlayBackTime : FsmStateAction
    {
        [RequiredField]
        [CheckForComponent(typeof (Animator))]
        [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The playBack time")]
        public FsmFloat playbackTime;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful for changing over time.")]
        public bool everyFrame;
        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.playbackTime = (FsmFloat) null;
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
                    this.DoPlaybackTime();
                    if (this.everyFrame)
                        return;
                    this.Finish();
                }
            }
        }

        public override void OnUpdate() => this.DoPlaybackTime();

        private void DoPlaybackTime()
        {
            if ((Object) this._animator == (Object) null)
                return;
            this._animator.playbackTime = this.playbackTime.Value;
        }
    }
}
