using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Sets the color of the Fog in the scene.")]
    [ActionCategory(ActionCategory.RenderSettings)]
    public class SetFogColor : FsmStateAction
    {
        [RequiredField]
        public FsmColor fogColor;
        public bool everyFrame;

        public override void Reset()
        {
            this.fogColor = (FsmColor) Color.white;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetFogColor();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoSetFogColor();

        private void DoSetFogColor() => RenderSettings.fogColor = this.fogColor.Value;
    }
}
