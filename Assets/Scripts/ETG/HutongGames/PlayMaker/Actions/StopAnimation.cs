using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Stops all playing Animations on a Game Object. Optionally, specify a single Animation to Stop.")]
    [ActionCategory(ActionCategory.Animation)]
    public class StopAnimation : BaseAnimationAction
    {
        [CheckForComponent(typeof (Animation))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Animation)]
        [HutongGames.PlayMaker.Tooltip("Leave empty to stop all playing animations.")]
        public FsmString animName;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.animName = (FsmString) null;
        }

        public override void OnEnter()
        {
            this.DoStopAnimation();
            this.Finish();
        }

        private void DoStopAnimation()
        {
            if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
                return;
            if (FsmString.IsNullOrEmpty(this.animName))
                this.animation.Stop();
            else
                this.animation.Stop(this.animName.Value);
        }
    }
}
