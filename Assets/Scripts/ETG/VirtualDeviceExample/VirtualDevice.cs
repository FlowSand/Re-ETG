// Decompiled with JetBrains decompiler
// Type: VirtualDeviceExample.VirtualDevice
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using InControl;
using UnityEngine;

#nullable disable
namespace VirtualDeviceExample
{
  public class VirtualDevice : InputDevice
  {
    private const float sensitivity = 0.1f;
    private const float mouseScale = 0.05f;
    private float kx;
    private float ky;
    private float mx;
    private float my;

    public VirtualDevice()
      : base("Virtual Controller")
    {
      this.AddControl(InputControlType.LeftStickLeft, "Left Stick Left");
      this.AddControl(InputControlType.LeftStickRight, "Left Stick Right");
      this.AddControl(InputControlType.LeftStickUp, "Left Stick Up");
      this.AddControl(InputControlType.LeftStickDown, "Left Stick Down");
      this.AddControl(InputControlType.RightStickLeft, "Right Stick Left");
      this.AddControl(InputControlType.RightStickRight, "Right Stick Right");
      this.AddControl(InputControlType.RightStickUp, "Right Stick Up");
      this.AddControl(InputControlType.RightStickDown, "Right Stick Down");
      this.AddControl(InputControlType.Action1, "A");
      this.AddControl(InputControlType.Action2, "B");
      this.AddControl(InputControlType.Action3, "X");
      this.AddControl(InputControlType.Action4, "Y");
    }

    public override void Update(ulong updateTick, float deltaTime)
    {
      this.UpdateLeftStickWithValue(this.GetVectorFromKeyboard(deltaTime, true), updateTick, deltaTime);
      this.UpdateRightStickWithRawValue(this.GetVectorFromMouse(deltaTime, true), updateTick, deltaTime);
      this.UpdateWithState(InputControlType.Action1, Input.GetKey(KeyCode.Space), updateTick, deltaTime);
      this.UpdateWithState(InputControlType.Action2, Input.GetKey(KeyCode.S), updateTick, deltaTime);
      this.UpdateWithState(InputControlType.Action3, Input.GetKey(KeyCode.D), updateTick, deltaTime);
      this.UpdateWithState(InputControlType.Action4, Input.GetKey(KeyCode.F), updateTick, deltaTime);
      this.Commit(updateTick, deltaTime);
    }

    private Vector2 GetVectorFromKeyboard(float deltaTime, bool smoothed)
    {
      if (smoothed)
      {
        this.kx = this.ApplySmoothing(this.kx, this.GetXFromKeyboard(), deltaTime, 0.1f);
        this.ky = this.ApplySmoothing(this.ky, this.GetYFromKeyboard(), deltaTime, 0.1f);
      }
      else
      {
        this.kx = this.GetXFromKeyboard();
        this.ky = this.GetYFromKeyboard();
      }
      return new Vector2(this.kx, this.ky);
    }

    private float GetXFromKeyboard()
    {
      return (!Input.GetKey(KeyCode.LeftArrow) ? 0.0f : -1f) + (!Input.GetKey(KeyCode.RightArrow) ? 0.0f : 1f);
    }

    private float GetYFromKeyboard()
    {
      return (!Input.GetKey(KeyCode.UpArrow) ? 0.0f : 1f) + (!Input.GetKey(KeyCode.DownArrow) ? 0.0f : -1f);
    }

    private Vector2 GetVectorFromMouse(float deltaTime, bool smoothed)
    {
      if (smoothed)
      {
        this.mx = this.ApplySmoothing(this.mx, Input.GetAxisRaw("mouse x") * 0.05f, deltaTime, 0.1f);
        this.my = this.ApplySmoothing(this.my, Input.GetAxisRaw("mouse y") * 0.05f, deltaTime, 0.1f);
      }
      else
      {
        this.mx = Input.GetAxisRaw("mouse x") * 0.05f;
        this.my = Input.GetAxisRaw("mouse y") * 0.05f;
      }
      return new Vector2(this.mx, this.my);
    }

    private float ApplySmoothing(
      float lastValue,
      float thisValue,
      float deltaTime,
      float sensitivity)
    {
      sensitivity = Mathf.Clamp(sensitivity, 1f / 1000f, 1f);
      return Mathf.Approximately(sensitivity, 1f) ? thisValue : Mathf.Lerp(lastValue, thisValue, (float) ((double) deltaTime * (double) sensitivity * 100.0));
    }
  }
}
