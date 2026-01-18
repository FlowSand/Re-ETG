using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Activates a Camera in the scene.")]
    [ActionCategory(ActionCategory.Camera)]
    public class CutToCamera : FsmStateAction
    {
        [HutongGames.PlayMaker.Tooltip("The Camera to activate.")]
        [RequiredField]
        public Camera camera;
        [HutongGames.PlayMaker.Tooltip("Makes the camera the new MainCamera. The old MainCamera will be untagged.")]
        public bool makeMainCamera;
        [HutongGames.PlayMaker.Tooltip("Cut back to the original MainCamera when exiting this state.")]
        public bool cutBackOnExit;
        private Camera oldCamera;

        public override void Reset()
        {
            this.camera = (Camera) null;
            this.makeMainCamera = true;
            this.cutBackOnExit = false;
        }

        public override void OnEnter()
        {
            if ((Object) this.camera == (Object) null)
            {
                this.LogError("Missing camera!");
            }
            else
            {
                this.oldCamera = Camera.main;
                CutToCamera.SwitchCamera(Camera.main, this.camera);
                if (this.makeMainCamera)
                    this.camera.tag = "MainCamera";
                this.Finish();
            }
        }

        public override void OnExit()
        {
            if (!this.cutBackOnExit)
                return;
            CutToCamera.SwitchCamera(this.camera, this.oldCamera);
        }

        private static void SwitchCamera(Camera camera1, Camera camera2)
        {
            if ((Object) camera1 != (Object) null)
                camera1.enabled = false;
            if (!((Object) camera2 != (Object) null))
                return;
            camera2.enabled = true;
        }
    }
}
