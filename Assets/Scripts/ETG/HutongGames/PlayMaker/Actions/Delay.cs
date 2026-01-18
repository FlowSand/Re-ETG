#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [Tooltip("Delays for a specified amount of time.")]
    [ActionCategory(".Brave")]
    public class Delay : FsmStateAction
    {
        [Tooltip("How many seconds to delay for (this action will not finish until the time has passed).")]
        public FsmFloat time;
        private bool firstFrame;
        private float timer;

        public override void OnEnter()
        {
            this.timer = 0.0f;
            this.firstFrame = true;
            if ((double) this.time.Value > 0.0)
                return;
            this.Finish();
        }

        public override void OnUpdate()
        {
            if (this.firstFrame)
            {
                this.firstFrame = false;
            }
            else
            {
                this.timer += BraveTime.DeltaTime;
                if ((double) this.timer < (double) this.time.Value)
                    return;
                this.Finish();
            }
        }
    }
}
