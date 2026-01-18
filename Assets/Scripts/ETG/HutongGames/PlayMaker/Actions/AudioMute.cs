using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Mute/unmute the Audio Clip played by an Audio Source component on a Game Object.")]
    [ActionCategory(ActionCategory.Audio)]
    public class AudioMute : FsmStateAction
    {
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The GameObject with an Audio Source component.")]
        [CheckForComponent(typeof (AudioSource))]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Check to mute, uncheck to unmute.")]
        [RequiredField]
        public FsmBool mute;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.mute = (FsmBool) false;
        }

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget != (Object) null)
            {
                AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
                if ((Object) component != (Object) null)
                    component.mute = this.mute.Value;
            }
            this.Finish();
        }
    }
}
