using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Sets the Culling Mask used by the Camera.")]
    [ActionCategory(ActionCategory.Camera)]
    public class SetCameraCullingMask : ComponentAction<Camera>
    {
        [CheckForComponent(typeof (Camera))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [UIHint(UIHint.Layer)]
        [HutongGames.PlayMaker.Tooltip("Cull these layers.")]
        public FsmInt[] cullingMask;
        [HutongGames.PlayMaker.Tooltip("Invert the mask, so you cull all layers except those defined above.")]
        public FsmBool invertMask;
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.cullingMask = new FsmInt[0];
            this.invertMask = (FsmBool) false;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetCameraCullingMask();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoSetCameraCullingMask();

        private void DoSetCameraCullingMask()
        {
            if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
                return;
            this.camera.cullingMask = ActionHelpers.LayerArrayToLayerMask(this.cullingMask, this.invertMask.Value);
        }
    }
}
