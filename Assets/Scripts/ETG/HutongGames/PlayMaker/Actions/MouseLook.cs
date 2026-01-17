// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.MouseLook
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HutongGames.PlayMaker.Tooltip("Rotates a GameObject based on mouse movement. Minimum and Maximum values can be used to constrain the rotation.")]
  [ActionCategory(ActionCategory.Input)]
  public class MouseLook : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The GameObject to rotate.")]
    [RequiredField]
    public FsmOwnerDefault gameObject;
    [HutongGames.PlayMaker.Tooltip("The axes to rotate around.")]
    public MouseLook.RotationAxes axes;
    [HutongGames.PlayMaker.Tooltip("Sensitivity of movement in X direction.")]
    [RequiredField]
    public FsmFloat sensitivityX;
    [HutongGames.PlayMaker.Tooltip("Sensitivity of movement in Y direction.")]
    [RequiredField]
    public FsmFloat sensitivityY;
    [HutongGames.PlayMaker.Tooltip("Clamp rotation around X axis. Set to None for no clamping.")]
    [HasFloatSlider(-360f, 360f)]
    public FsmFloat minimumX;
    [HutongGames.PlayMaker.Tooltip("Clamp rotation around X axis. Set to None for no clamping.")]
    [HasFloatSlider(-360f, 360f)]
    public FsmFloat maximumX;
    [HutongGames.PlayMaker.Tooltip("Clamp rotation around Y axis. Set to None for no clamping.")]
    [HasFloatSlider(-360f, 360f)]
    public FsmFloat minimumY;
    [HutongGames.PlayMaker.Tooltip("Clamp rotation around Y axis. Set to None for no clamping.")]
    [HasFloatSlider(-360f, 360f)]
    public FsmFloat maximumY;
    [HutongGames.PlayMaker.Tooltip("Repeat every frame.")]
    public bool everyFrame;
    private float rotationX;
    private float rotationY;

    public override void Reset()
    {
      this.gameObject = (FsmOwnerDefault) null;
      this.axes = MouseLook.RotationAxes.MouseXAndY;
      this.sensitivityX = (FsmFloat) 15f;
      this.sensitivityY = (FsmFloat) 15f;
      FsmFloat fsmFloat1 = new FsmFloat();
      fsmFloat1.UseVariable = true;
      this.minimumX = fsmFloat1;
      FsmFloat fsmFloat2 = new FsmFloat();
      fsmFloat2.UseVariable = true;
      this.maximumX = fsmFloat2;
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
        Rigidbody component = ownerDefaultTarget.GetComponent<Rigidbody>();
        if ((Object) component != (Object) null)
          component.freezeRotation = true;
        this.rotationX = ownerDefaultTarget.transform.localRotation.eulerAngles.y;
        this.rotationY = ownerDefaultTarget.transform.localRotation.eulerAngles.x;
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
        case MouseLook.RotationAxes.MouseXAndY:
          transform.localEulerAngles = new Vector3(this.GetYRotation(), this.GetXRotation(), 0.0f);
          break;
        case MouseLook.RotationAxes.MouseX:
          transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, this.GetXRotation(), 0.0f);
          break;
        case MouseLook.RotationAxes.MouseY:
          transform.localEulerAngles = new Vector3(-this.GetYRotation(), transform.localEulerAngles.y, 0.0f);
          break;
      }
    }

    private float GetXRotation()
    {
      this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX.Value;
      this.rotationX = MouseLook.ClampAngle(this.rotationX, this.minimumX, this.maximumX);
      return this.rotationX;
    }

    private float GetYRotation()
    {
      this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY.Value;
      this.rotationY = MouseLook.ClampAngle(this.rotationY, this.minimumY, this.maximumY);
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
