using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Audio)]
    [HutongGames.PlayMaker.Tooltip("Stops playing the Audio Clip played by an Audio Source component on a Game Object.")]
    public class AudioStop : FsmStateAction
    {
        [CheckForComponent(typeof (AudioSource))]
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("The GameObject with an AudioSource component.")]
        public FsmOwnerDefault gameObject;

        public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

        public override void OnEnter()
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
            if ((Object) ownerDefaultTarget != (Object) null)
            {
                AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
                if ((Object) component != (Object) null)
                    component.Stop();
            }
            this.Finish();
        }
    }
}
