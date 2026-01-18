using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Time)]
    [HutongGames.PlayMaker.Tooltip("Delays a State from finishing by the specified time. NOTE: Other actions continue, but FINISHED can't happen before Time.")]
    public class Wait : FsmStateAction
    {
        [RequiredField]
        public FsmFloat time;
        public FsmEvent finishEvent;
        public bool realTime;
        private float startTime;
        private float timer;

        public override void Reset()
        {
            this.time = (FsmFloat) 1f;
            this.finishEvent = (FsmEvent) null;
            this.realTime = false;
        }

        public override void OnEnter()
        {
            if ((double) this.time.Value <= 0.0)
            {
                this.Fsm.Event(this.finishEvent);
                this.Finish();
            }
            else
            {
                this.startTime = FsmTime.RealtimeSinceStartup;
                this.timer = 0.0f;
            }
        }

        public override void OnUpdate()
        {
            if (this.realTime)
                this.timer = FsmTime.RealtimeSinceStartup - this.startTime;
            else
                this.timer += Time.deltaTime;
            if ((double) this.timer < (double) this.time.Value)
                return;
            this.Finish();
            if (this.finishEvent == null)
                return;
            this.Fsm.Event(this.finishEvent);
        }
    }
}
