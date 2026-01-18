using System;
using UnityEngine;

#nullable disable
namespace InControl
{
  public abstract class TouchControl : MonoBehaviour
  {
    public abstract void CreateControl();

    public abstract void DestroyControl();

    public abstract void ConfigureControl();

    public abstract void SubmitControlState(ulong updateTick, float deltaTime);

    public abstract void CommitControlState(ulong updateTick, float deltaTime);

    public abstract void TouchBegan(Touch touch);

    public abstract void TouchMoved(Touch touch);

    public abstract void TouchEnded(Touch touch);

    public abstract void DrawGizmos();

    private void OnEnable() => TouchManager.OnSetup += new Action(this.Setup);

    private void OnDisable()
    {
      this.DestroyControl();
      Resources.UnloadUnusedAssets();
    }

    private void Setup()
    {
      if (!this.enabled)
        return;
      this.CreateControl();
      this.ConfigureControl();
    }

    protected Vector3 OffsetToWorldPosition(
      TouchControlAnchor anchor,
      Vector2 offset,
      TouchUnitType offsetUnitType,
      bool lockAspectRatio)
    {
      Vector3 vector3 = offsetUnitType != TouchUnitType.Pixels ? (!lockAspectRatio ? Vector3.Scale((Vector3) offset, TouchManager.ViewSize) : (Vector3) offset * TouchManager.PercentToWorld) : (Vector3) (TouchUtility.RoundVector(offset) * TouchManager.PixelToWorld);
      return TouchManager.ViewToWorldPoint(TouchUtility.AnchorToViewPoint(anchor)) + vector3;
    }

    protected void SubmitButtonState(
      TouchControl.ButtonTarget target,
      bool state,
      ulong updateTick,
      float deltaTime)
    {
      if (TouchManager.Device == null || target == TouchControl.ButtonTarget.None)
        return;
      InputControl control = TouchManager.Device.GetControl((InputControlType) target);
      if (control == null || control == InputControl.Null)
        return;
      control.UpdateWithState(state, updateTick, deltaTime);
    }

    protected void SubmitButtonValue(
      TouchControl.ButtonTarget target,
      float value,
      ulong updateTick,
      float deltaTime)
    {
      if (TouchManager.Device == null || target == TouchControl.ButtonTarget.None)
        return;
      InputControl control = TouchManager.Device.GetControl((InputControlType) target);
      if (control == null || control == InputControl.Null)
        return;
      control.UpdateWithValue(value, updateTick, deltaTime);
    }

    protected void CommitButton(TouchControl.ButtonTarget target)
    {
      if (TouchManager.Device == null || target == TouchControl.ButtonTarget.None)
        return;
      InputControl control = TouchManager.Device.GetControl((InputControlType) target);
      if (control == null || control == InputControl.Null)
        return;
      control.Commit();
    }

    protected void SubmitAnalogValue(
      TouchControl.AnalogTarget target,
      Vector2 value,
      float lowerDeadZone,
      float upperDeadZone,
      ulong updateTick,
      float deltaTime)
    {
      if (TouchManager.Device == null || target == TouchControl.AnalogTarget.None)
        return;
      Vector2 vector2 = Utility.ApplyCircularDeadZone(value, lowerDeadZone, upperDeadZone);
      if (target == TouchControl.AnalogTarget.LeftStick || target == TouchControl.AnalogTarget.Both)
        TouchManager.Device.UpdateLeftStickWithValue(vector2, updateTick, deltaTime);
      if (target != TouchControl.AnalogTarget.RightStick && target != TouchControl.AnalogTarget.Both)
        return;
      TouchManager.Device.UpdateRightStickWithValue(vector2, updateTick, deltaTime);
    }

    protected void CommitAnalog(TouchControl.AnalogTarget target)
    {
      if (TouchManager.Device == null || target == TouchControl.AnalogTarget.None)
        return;
      if (target == TouchControl.AnalogTarget.LeftStick || target == TouchControl.AnalogTarget.Both)
        TouchManager.Device.CommitLeftStick();
      if (target != TouchControl.AnalogTarget.RightStick && target != TouchControl.AnalogTarget.Both)
        return;
      TouchManager.Device.CommitRightStick();
    }

    protected void SubmitRawAnalogValue(
      TouchControl.AnalogTarget target,
      Vector2 rawValue,
      ulong updateTick,
      float deltaTime)
    {
      if (TouchManager.Device == null || target == TouchControl.AnalogTarget.None)
        return;
      if (target == TouchControl.AnalogTarget.LeftStick || target == TouchControl.AnalogTarget.Both)
        TouchManager.Device.UpdateLeftStickWithRawValue(rawValue, updateTick, deltaTime);
      if (target != TouchControl.AnalogTarget.RightStick && target != TouchControl.AnalogTarget.Both)
        return;
      TouchManager.Device.UpdateRightStickWithRawValue(rawValue, updateTick, deltaTime);
    }

    protected static Vector3 SnapTo(Vector2 vector, TouchControl.SnapAngles snapAngles)
    {
      if (snapAngles == TouchControl.SnapAngles.None)
        return (Vector3) vector;
      float snapAngle = 360f / (float) snapAngles;
      return TouchControl.SnapTo(vector, snapAngle);
    }

    protected static Vector3 SnapTo(Vector2 vector, float snapAngle)
    {
      float num = Vector2.Angle(vector, Vector2.up);
      if ((double) num < (double) snapAngle / 2.0)
        return (Vector3) (Vector2.up * vector.magnitude);
      return (double) num > 180.0 - (double) snapAngle / 2.0 ? (Vector3) (-Vector2.up * vector.magnitude) : Quaternion.AngleAxis(Mathf.Round(num / snapAngle) * snapAngle - num, Vector3.Cross((Vector3) Vector2.up, (Vector3) vector)) * (Vector3) vector;
    }

    private void OnDrawGizmosSelected()
    {
      if (!this.enabled || TouchManager.ControlsShowGizmos != TouchManager.GizmoShowOption.WhenSelected || Utility.GameObjectIsCulledOnCurrentCamera(this.gameObject))
        return;
      if (!Application.isPlaying)
        this.ConfigureControl();
      this.DrawGizmos();
    }

    private void OnDrawGizmos()
    {
      if (!this.enabled)
        return;
      switch (TouchManager.ControlsShowGizmos)
      {
        case TouchManager.GizmoShowOption.UnlessPlaying:
          if (Application.isPlaying)
            break;
          goto case TouchManager.GizmoShowOption.Always;
        case TouchManager.GizmoShowOption.Always:
          if (Utility.GameObjectIsCulledOnCurrentCamera(this.gameObject))
            break;
          if (!Application.isPlaying)
            this.ConfigureControl();
          this.DrawGizmos();
          break;
      }
    }

    public enum ButtonTarget
    {
      None = 0,
      DPadUp = 11, // 0x0000000B
      DPadDown = 12, // 0x0000000C
      DPadLeft = 13, // 0x0000000D
      DPadRight = 14, // 0x0000000E
      LeftTrigger = 15, // 0x0000000F
      RightTrigger = 16, // 0x00000010
      LeftBumper = 17, // 0x00000011
      RightBumper = 18, // 0x00000012
      Action1 = 19, // 0x00000013
      Action2 = 20, // 0x00000014
      Action3 = 21, // 0x00000015
      Action4 = 22, // 0x00000016
      Action5 = 23, // 0x00000017
      Action6 = 24, // 0x00000018
      Action7 = 25, // 0x00000019
      Action8 = 26, // 0x0000001A
      Action9 = 27, // 0x0000001B
      Action10 = 28, // 0x0000001C
      Action11 = 29, // 0x0000001D
      Action12 = 30, // 0x0000001E
      Menu = 106, // 0x0000006A
      Button0 = 500, // 0x000001F4
      Button1 = 501, // 0x000001F5
      Button2 = 502, // 0x000001F6
      Button3 = 503, // 0x000001F7
      Button4 = 504, // 0x000001F8
      Button5 = 505, // 0x000001F9
      Button6 = 506, // 0x000001FA
      Button7 = 507, // 0x000001FB
      Button8 = 508, // 0x000001FC
      Button9 = 509, // 0x000001FD
      Button10 = 510, // 0x000001FE
      Button11 = 511, // 0x000001FF
      Button12 = 512, // 0x00000200
      Button13 = 513, // 0x00000201
      Button14 = 514, // 0x00000202
      Button15 = 515, // 0x00000203
      Button16 = 516, // 0x00000204
      Button17 = 517, // 0x00000205
      Button18 = 518, // 0x00000206
      Button19 = 519, // 0x00000207
    }

    public enum AnalogTarget
    {
      None,
      LeftStick,
      RightStick,
      Both,
    }

    public enum SnapAngles
    {
      None = 0,
      Four = 4,
      Eight = 8,
      Sixteen = 16, // 0x00000010
    }
  }
}
