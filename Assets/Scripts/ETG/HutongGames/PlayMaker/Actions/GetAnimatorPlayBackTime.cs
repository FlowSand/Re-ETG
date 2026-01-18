using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Gets the playback position in the recording buffer. When in playback mode (use  AnimatorStartPlayback), this value is used for controlling the current playback position in the buffer (in seconds). The value can range between recordingStartTime and recordingStopTime See Also: StartPlayback, StopPlayback.")]
    [ActionCategory(ActionCategory.Animator)]
    public class GetAnimatorPlayBackTime : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The Target. An Animator component is required")]
        [CheckForComponent(typeof (Animator))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Variable)]
        [HutongGames.PlayMaker.Tooltip("The playBack time of the animator.")]
        [ActionSection("Result")]
        [RequiredField]
        public FsmFloat playBackTime;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
        public bool everyFrame;
        private Animator _animator;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.playBackTime = (FsmFloat) null;
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
                    this.GetPlayBackTime();
                    if (this.everyFrame)
                        return;
                    this.Finish();
                }
            }
        }

        public override void OnUpdate() => this.GetPlayBackTime();

        private void GetPlayBackTime()
        {
            if (!((Object) this._animator != (Object) null))
                return;
            this.playBackTime.Value = this._animator.playbackTime;
        }
    }
}
