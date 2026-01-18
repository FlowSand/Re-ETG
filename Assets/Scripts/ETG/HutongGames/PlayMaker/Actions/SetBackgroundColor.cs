using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
    [HutongGames.PlayMaker.Tooltip("Sets the Background Color used by the Camera.")]
    [ActionCategory(ActionCategory.Camera)]
    public class SetBackgroundColor : ComponentAction<Camera>
    {
        [CheckForComponent(typeof (Camera))]
        [RequiredField]
        public FsmOwnerDefault gameObject;
        [RequiredField]
        public FsmColor backgroundColor;
        public bool everyFrame;

        public override void Reset()
        {
            this.gameObject = (FsmOwnerDefault) null;
            this.backgroundColor = (FsmColor) Color.black;
            this.everyFrame = false;
        }

        public override void OnEnter()
        {
            this.DoSetBackgroundColor();
            if (this.everyFrame)
                return;
            this.Finish();
        }

        public override void OnUpdate() => this.DoSetBackgroundColor();

        private void DoSetBackgroundColor()
        {
            if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
                return;
            this.camera.backgroundColor = this.backgroundColor.Value;
        }
    }
}
