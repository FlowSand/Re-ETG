using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Animator)]
    [HutongGames.PlayMaker.Tooltip("Sets the animator in playback mode.")]
    public class AnimatorStartPlayback : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        [CheckForComponent(typeof (Animator))]
        [RequiredField]
        public FsmOwnerDefault gameObject;

        public override void Reset() => this.gameObject = (FsmOwnerDefault) null;

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
                    component.StartPlayback();
                this.Finish();
            }
        }
    }
}
