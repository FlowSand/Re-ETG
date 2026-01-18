using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [HutongGames.PlayMaker.Tooltip("Stops the animator record mode. It will lock the recording buffer's contents in its current state. The data get saved for subsequent playback with StartPlayback.")]
    public class AnimatorStopRecording : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
        [CheckForComponent(typeof (Animator))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("The recorder StartTime")]
        [UIHint(UIHint.Variable)]
        [ActionSection("Results")]
        public FsmFloat recorderStartTime;
        [HutongGames.PlayMaker.Tooltip("The recorder StopTime")]
        [UIHint(UIHint.Variable)]
        public FsmFloat recorderStopTime;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.recorderStartTime = (FsmFloat) null;
            this.recorderStopTime = (FsmFloat) null;
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
                Animator component = ownerDefaultTarget.GetComponent<Animator>();
                if ((Object) component != (Object) null)
                {
                    component.StopRecording();
                    this.recorderStartTime.Value = component.recorderStartTime;
                    this.recorderStopTime.Value = component.recorderStopTime;
                }
                this.Finish();
            }
        }
    }
}
