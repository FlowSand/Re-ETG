using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Rotates a GameObject based on mouse movement. Minimum and Maximum values can be used to constrain the rotation.")]
  [ActionCategory(ActionCategory.Input)]
  public class MouseLook2 : ComponentAction<Rigidbody>
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject to rotate.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The axes to rotate around.")]
    public MouseLook2.RotationAxes axes;
    [RequiredField]
    public FsmFloat sensitivityX;
    [RequiredField]
    public FsmFloat sensitivityY;
    [HasFloatSlider(-360f, 360f)]
    [RequiredField]
    public FsmFloat minimumX;
    [HasFloatSlider(-360f, 360f)]
    [RequiredField]
    public FsmFloat maximumX;
    [HasFloatSlider(-360f, 360f)]
    [RequiredField]
    public FsmFloat minimumY;
    [HasFloatSlider(-360f, 360f)]
    [RequiredField]
    public FsmFloat maximumY;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;
    private float rotationX;
    private float rotationY;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.axes = MouseLook2.RotationAxes.MouseXAndY;
      this.sensitivityX = (FsmFloat) 15f;
      this.sensitivityY = (FsmFloat) 15f;
      this.minimumX = (FsmFloat) -360f;
      this.maximumX = (FsmFloat) 360f;
      this.minimumY = (FsmFloat) -60f;
      this.maximumY = (FsmFloat) 60f;
      this.everyFrame = true;
    }

    public override void OnEnter()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
      {
        this.Finish();
      }
      else
      {
        if (!this.UpdateCache(ownerDefaultTarget) && (bool) (Object) this.rigidbody)
          this.rigidbody.freezeRotation = true;
        this.DoMouseLook();
        if (this.everyFrame)
          return;
        this.Finish();
      }
    }

    public override void OnUpdate() => this.DoMouseLook();

    private void DoMouseLook()
    {
      GameObject ownerDefaultTarget = this.Fsm.GetOwnerDefaultTarget(this.gameObject);
      if ((Object) ownerDefaultTarget == (Object) null)
        return;
      Transform transform = ownerDefaultTarget.transform;
      switch (this.axes)
      {
        case MouseLook2.RotationAxes.MouseXAndY:
          transform.localEulerAngles = new Vector3(this.GetYRotation(), this.GetXRotation(), 0.0f);
          break;
        case MouseLook2.RotationAxes.MouseX:
          transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, this.GetXRotation(), 0.0f);
          break;
        case MouseLook2.RotationAxes.MouseY:
          transform.localEulerAngles = new Vector3(-this.GetYRotation(), transform.localEulerAngles.y, 0.0f);
          break;
      }
    }

    private float GetXRotation()
    {
      this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX.Value;
      this.rotationX = MouseLook2.ClampAngle(this.rotationX, this.minimumX, this.maximumX);
      return this.rotationX;
    }

    private float GetYRotation()
    {
      this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY.Value;
      this.rotationY = MouseLook2.ClampAngle(this.rotationY, this.minimumY, this.maximumY);
      return this.rotationY;
    }

    private static float ClampAngle(float angle, FsmFloat min, FsmFloat max)
    {
      if (!min.IsNone && (double) angle < (double) min.Value)
        angle = min.Value;
      if (!max.IsNone && (double) angle > (double) max.Value)
        angle = max.Value;
      return angle;
    }

    public enum RotationAxes
    {
      MouseXAndY,
      MouseX,
      MouseY,
    }
  }
}
