using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Interpolate through an array of Colors over a specified amount of Time.")]
    [ActionCategory(ActionCategory.Color)]
    public class ColorInterpolate : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Array of colors to interpolate through.")]
        [RequiredField]
        public FsmColor[] colors;
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("Interpolation time.")]
        public FsmFloat time;
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("Store the interpolated color in a Color variable.")]
        [UIHint(UIHint.Variable)]
        public FsmColor storeColor;
        [HutongGames.PlayMaker.Tooltip("Event to send when the interpolation finishes.")]
        public FsmEvent finishEvent;
        [HutongGames.PlayMaker.Tooltip("Ignore TimeScale")]
        public bool realTime;
        private float startTime;
        private float currentTime;

        public override void Reset()
        {
            this.colors = new FsmColor[3];
            this.time = (FsmFloat) 1f;
            this.storeColor = (FsmColor) null;
            this.finishEvent = (FsmEvent) null;
            this.realTime = false;
        }

        public override void OnEnter()
        {
            this.startTime = FsmTime.RealtimeSinceStartup;
            this.currentTime = 0.0f;
            if (this.colors.Length < 2)
            {
                if (this.colors.Length == 1)
                    this.storeColor.Value = this.colors[0].Value;
                this.Finish();
            }
            else
                this.storeColor.Value = this.colors[0].Value;
        }

        public override void OnUpdate()
        {
            if (this.realTime)
                this.currentTime = FsmTime.RealtimeSinceStartup - this.startTime;
            else
                this.currentTime += Time.deltaTime;
            if ((double) this.currentTime > (double) this.time.Value)
            {
                this.Finish();
                this.storeColor.Value = this.colors[this.colors.Length - 1].Value;
                if (this.finishEvent == null)
                    return;
                this.Fsm.Event(this.finishEvent);
            }
            else
            {
                float num = (float) (this.colors.Length - 1) * this.currentTime / this.time.Value;
                Color color;
                if (num.Equals(0.0f))
                    color = this.colors[0].Value;
                else if (num.Equals((float) (this.colors.Length - 1)))
                {
                    color = this.colors[this.colors.Length - 1].Value;
                }
                else
                {
                    Color a = this.colors[Mathf.FloorToInt(num)].Value;
                    Color b = this.colors[Mathf.CeilToInt(num)].Value;
                    num -= Mathf.Floor(num);
                    color = Color.Lerp(a, b, num);
                }
                this.storeColor.Value = color;
            }
        }

        public override string ErrorCheck()
        {
            return this.colors.Length < 2 ? "Define at least 2 colors to make a gradient." : (string) null;
        }
    }
}
