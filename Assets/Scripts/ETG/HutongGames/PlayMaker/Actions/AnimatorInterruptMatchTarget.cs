using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Interrupts the automatic target matching. CompleteMatch will make the gameobject match the target completely at the next frame.")]
    [ActionCategory(ActionCategory.Animator)]
    public class AnimatorInterruptMatchTarget : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The target. An Animator component is required")]
        [CheckForComponent(typeof (Animator))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [HutongGames.PlayMaker.Tooltip("Will make the gameobject match the target completely at the next frame")]
        public FsmBool completeMatch;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.completeMatch = (FsmBool) true;
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
                    component.InterruptMatchTarget(this.completeMatch.Value);
                this.Finish();
            }
        }
    }
}
