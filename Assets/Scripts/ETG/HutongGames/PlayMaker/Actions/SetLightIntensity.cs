using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Sets the Intensity of a Light.")]
    [ActionCategory(ActionCategory.Lights)]
    public class SetLightIntensity : ComponentAction<Light>
    {
        [RequiredField]
        [CheckForComponent(typeof (Light))]
        public FsmOwnerDefault gameObject;
        public FsmFloat lightIntensity;
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.lightIntensity = (FsmFloat) 1f;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetLightIntensity();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoSetLightIntensity();

        private void DoSetLightIntensity()
        {
            if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
                return;
            this.light.intensity = this.lightIntensity.Value;
        }
    }
}
