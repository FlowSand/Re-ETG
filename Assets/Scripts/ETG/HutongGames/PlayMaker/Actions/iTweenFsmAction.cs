using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("iTween base action - don't use!")]
    public abstract class iTweenFsmAction : FsmStateAction
    {
        [ActionSection("Events")]
        public FsmEvent startEvent;
        public FsmEvent finishEvent;
        [HutongGames.PlayMaker.Tooltip("Setting this to true will allow the animation to continue independent of the current time which is helpful for animating menus after a game has been paused by setting Time.timeScale=0.")]
        public FsmBool realTime;
        public FsmBool stopOnExit;
        public FsmBool loopDontFinish;
        internal iTweenFSMEvents itweenEvents;
        protected string itweenType = string.Empty;
        protected int itweenID = -1;

        public override void Reset()
        {
            this.startEvent = (FsmEvent) null;
            this.finishEvent = (FsmEvent) null;
            this.realTime = new FsmBool() { Value = false };
            this.stopOnExit = new FsmBool() { Value = true };
            this.loopDontFinish = new FsmBool() { Value = true };
            this.itweenType = string.Empty;
        }

        protected void OnEnteriTween(FsmOwnerDefault anOwner)
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(anOwner);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            this.itweenEvents = ownerDefaultTarget.AddComponent<iTweenFSMEvents>();
            this.itweenEvents.itweenFSMAction = this;
            ++iTweenFSMEvents.itweenIDCount;
            this.itweenID = iTweenFSMEvents.itweenIDCount;
            this.itweenEvents.itweenID = iTweenFSMEvents.itweenIDCount;
            this.itweenEvents.donotfinish = !this.loopDontFinish.IsNone && this.loopDontFinish.Value;
        }

        protected void IsLoop(bool aValue)
        {
            if (!((Object) this.itweenEvents != (Object) null))
                return;
            this.itweenEvents.islooping = aValue;
        }

        protected void OnExitiTween(FsmOwnerDefault anOwner)
        {
            GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(anOwner);
            if ((Object) ownerDefaultTarget == (Object) null)
                return;
            if ((bool) (Object) this.itweenEvents)
                Object.Destroy((Object) this.itweenEvents);
            if (this.stopOnExit.IsNone)
            {
                iTween.Stop(ownerDefaultTarget, this.itweenType);
            }
            else
            {
                if (!this.stopOnExit.Value)
                    return;
                iTween.Stop(ownerDefaultTarget, this.itweenType);
            }
        }

        public enum AxisRestriction
        {
            none,
            x,
            y,
            z,
            xy,
            xz,
            yz,
        }
    }
}
