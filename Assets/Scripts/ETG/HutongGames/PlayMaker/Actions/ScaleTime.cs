using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Scales time: 1 = normal, 0.5 = half speed, 2 = double speed.")]
    [ActionCategory(ActionCategory.Time)]
    public class ScaleTime : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Scales time: 1 = normal, 0.5 = half speed, 2 = double speed.")]
        [HasFloatSlider(0.0f, 4f)]
        [RequiredField]
        public FsmFloat timeScale;
        [HutongGames.PlayMaker.Tooltip("Adjust the fixed physics time step to match the time scale.")]
        public FsmBool adjustFixedDeltaTime;
        [HutongGames.PlayMaker.Tooltip("Repeat every frame. Useful when animating the value.")]
        public bool everyFrame;

        public override void Reset()
        {
            this.timeScale = (FsmFloat) 1f;
            this.adjustFixedDeltaTime = (FsmBool) true;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoTimeScale();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoTimeScale();

        private void DoTimeScale()
        {
            Time.timeScale = this.timeScale.Value;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }
}
