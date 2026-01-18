using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [ActionCategory(ActionCategory.Camera)]
  [HutongGames.PlayMaker.Tooltip("Sets Field of View used by the Camera.")]
  public class SetCameraFOV : ComponentAction<Camera>
  {
    [CheckForComponent(typeof (Camera))]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [RequiredField]
    public FsmFloat fieldOfView;
    public bool everyFrame;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.fieldOfView = (FsmFloat) 50f;
      this.everyFrame = false;
    }

    public override void OnEnter()
    {
      this.DoSetCameraFOV();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoSetCameraFOV();

    private void DoSetCameraFOV()
    {
      if (!this.UpdateCache(this.Fsm.GetOwnerDefaultTarget(this.gameObject)))
        return;
      this.camera.fieldOfView = this.fieldOfView.Value;
    }
  }
}
