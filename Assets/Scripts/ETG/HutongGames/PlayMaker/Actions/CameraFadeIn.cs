using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.Camera)]
    [HutongGames.PlayMaker.Tooltip("Fade from a fullscreen Color. NOTE: Uses OnGUI so requires a PlayMakerGUI component in the scene.")]
    public class CameraFadeIn : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("Color to fade from. E.g., Fade up from black.")]
        [RequiredField]
        public FsmColor color;
        [HasFloatSlider(0.0f, 10f)]
        [RequiredField]
        [HutongGames.PlayMaker.Tooltip("Fade in time in seconds.")]
        public FsmFloat time;
        [HutongGames.PlayMaker.Tooltip("Event to send when finished.")]
        public FsmEvent finishEvent;
        [HutongGames.PlayMaker.Tooltip("Ignore TimeScale. Useful if the game is paused.")]
        public bool realTime;
        private float startTime;
        private float currentTime;
        private Color colorLerp;

        public override void Reset()
        {
            this.color = (FsmColor) Color.black;
            this.time = (FsmFloat) 1f;
            this.finishEvent = (FsmEvent) null;
        }

        public override void OnEnter()
        {
            this.startTime = FsmTime.RealtimeSinceStartup;
            this.currentTime = 0.0f;
            this.colorLerp = this.color.Value;
        }

        public override void OnUpdate()
        {
            if (this.realTime)
                this.currentTime = FsmTime.RealtimeSinceStartup - this.startTime;
            else
                this.currentTime += Time.deltaTime;
            this.colorLerp = Color.Lerp(this.color.Value, Color.clear, this.currentTime / this.time.Value);
            if ((double) this.currentTime <= (double) this.time.Value)
                return;
            if (this.finishEvent != null)
                this.Fsm.Event(this.finishEvent);
            this.Finish();
        }

        public override void OnGUI()
        {
            Color color = GUI.color;
            GUI.color = this.colorLerp;
            GUI.DrawTexture(new Rect(0.0f, 0.0f, (float) Screen.width, (float) Screen.height), (Texture) ActionHelpers.WhiteTexture);
            GUI.color = color;
        }
    }
}
