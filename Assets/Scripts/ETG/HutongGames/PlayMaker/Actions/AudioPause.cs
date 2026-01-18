using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Audio)]
    [HutongGames.PlayMaker.Tooltip("Pauses playing the Audio Clip played by an Audio Source component on a Game Object.")]
    public class AudioPause : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The GameObject with an Audio Source component.")]
        [RequiredField]
        [CheckForComponent(typeof (AudioSource))]
        public FsmOwnerDefault gameObject;

        public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget != (Object) null)
            {
                AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
                if ((Object) component != (Object) null)
                    component.Pause();
            }
            this.Finish();
        }
    }
}
