// Decompiled with JetBrains decompiler
// Type: HutongGames.PlayMaker.Actions.GetAxisVector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [NoActionTargets]
  [ActionCategory(ActionCategory.Input)]
  [HutongGames.PlayMaker.Tooltip("Gets a world direction Vector from 2 Input Axis. Typically used for a third person controller with Relative To set to the camera.")]
  public class GetAxisVector : FsmStateAction
  {
    [HutongGames.PlayMaker.Tooltip("The name of the horizontal input axis. See Unity Input Manager.")]
    public FsmString horizontalAxis;
    [HutongGames.PlayMaker.Tooltip("The name of the vertical input axis. See Unity Input Manager.")]
    public FsmString verticalAxis;
    [HutongGames.PlayMaker.Tooltip("Input axis are reported in the range -1 to 1, this multiplier lets you set a new range.")]
    public FsmFloat multiplier;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("The world plane to map the 2d input onto.")]
    public GetAxisVector.AxisPlane mapToPlane;
    [HutongGames.PlayMaker.Tooltip("Make the result relative to a GameObject, typically the main camera.")]
    public FsmGameObject relativeTo;
    [RequiredField]
    [HutongGames.PlayMaker.Tooltip("Store the direction vector.")]
    [UIHint(UIHint.Variable)]
    public FsmVector3 storeVector;
    [HutongGames.PlayMaker.Tooltip("Store the length of the direction vector.")]
    [UIHint(UIHint.Variable)]
    public FsmFloat storeMagnitude;

    public override void Reset()
    {
      this.horizontalAxis = (FsmString) "Horizontal";
      this.verticalAxis = (FsmString) "Vertical";
      this.multiplier = (FsmFloat) 1f;
      this.mapToPlane = GetAxisVector.AxisPlane.XZ;
      this.storeVector = (FsmVector3) null;
      this.storeMagnitude = (FsmFloat) null;
    }

    public override void OnUpdate()
    {
      Vector3 vector3_1 = new Vector3();
      Vector3 vector3_2 = new Vector3();
      if ((Object) this.relativeTo.Value == (Object) null)
      {
        switch (this.mapToPlane)
        {
          case GetAxisVector.AxisPlane.XZ:
            vector3_1 = Vector3.forward;
            vector3_2 = Vector3.right;
            break;
          case GetAxisVector.AxisPlane.XY:
            vector3_1 = Vector3.up;
            vector3_2 = Vector3.right;
            break;
          case GetAxisVector.AxisPlane.YZ:
            vector3_1 = Vector3.up;
            vector3_2 = Vector3.forward;
            break;
        }
      }
      else
      {
        Transform transform = this.relativeTo.Value.transform;
        switch (this.mapToPlane)
        {
          case GetAxisVector.AxisPlane.XZ:
            vector3_1 = transform.TransformDirection(Vector3.forward) with
            {
              y = 0.0f
            };
            vector3_1 = vector3_1.normalized;
            vector3_2 = new Vector3(vector3_1.z, 0.0f, -vector3_1.x);
            break;
          case GetAxisVector.AxisPlane.XY:
          case GetAxisVector.AxisPlane.YZ:
            vector3_1 = Vector3.up with { z = 0.0f };
            vector3_1 = vector3_1.normalized;
            vector3_2 = transform.TransformDirection(Vector3.right);
            break;
        }
      }
      float num1 = this.horizontalAxis.IsNone || string.IsNullOrEmpty(this.horizontalAxis.Value) ? 0.0f : Input.GetAxis(this.horizontalAxis.Value);
      float num2 = this.verticalAxis.IsNone || string.IsNullOrEmpty(this.verticalAxis.Value) ? 0.0f : Input.GetAxis(this.verticalAxis.Value);
      Vector3 vector3_3 = (num1 * vector3_2 + num2 * vector3_1) * this.multiplier.Value;
      this.storeVector.Value = vector3_3;
      if (this.storeMagnitude.IsNone)
        return;
      this.storeMagnitude.Value = vector3_3.magnitude;
    }

    public enum AxisPlane
    {
      XZ,
      XY,
      YZ,
    }
  }
}
