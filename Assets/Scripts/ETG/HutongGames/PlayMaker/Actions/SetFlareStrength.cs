using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Sets the intensity of all Flares in the scene.")]
    [ActionCategory(ActionCategory.RenderSettings)]
    public class SetFlareStrength : FsmStateAction
    {
        [RequiredField]
        public FsmFloat flareStrength;
        public bool everyFrame;

        public override void Reset()
        {
            this.flareStrength = (FsmFloat) 0.2f;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetFlareStrength();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoSetFlareStrength();

        private void DoSetFlareStrength() => RenderSettings.flareStrength = this.flareStrength.Value;
    }
}
