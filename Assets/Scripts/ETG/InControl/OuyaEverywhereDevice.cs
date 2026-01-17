// Decompiled with JetBrains decompiler
// Type: InControl.OuyaEverywhereDevice
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl
{
  public class OuyaEverywhereDevice : InputDevice
  {
    private const float LowerDeadZone = 0.2f;
    private const float UpperDeadZone = 0.9f;

    public OuyaEverywhereDevice(int deviceIndex)
      : base("OUYA Controller")
    {
      this.DeviceIndex = deviceIndex;
      this.SortOrder = deviceIndex;
      this.Meta = "OUYA Everywhere Device #" + (object) deviceIndex;
      this.AddControl(InputControlType.LeftStickLeft, "Left Stick Left");
      this.AddControl(InputControlType.LeftStickRight, "Left Stick Right");
      this.AddControl(InputControlType.LeftStickUp, "Left Stick Up");
      this.AddControl(InputControlType.LeftStickDown, "Left Stick Down");
      this.AddControl(InputControlType.RightStickLeft, "Right Stick Left");
      this.AddControl(InputControlType.RightStickRight, "Right Stick Right");
      this.AddControl(InputControlType.RightStickUp, "Right Stick Up");
      this.AddControl(InputControlType.RightStickDown, "Right Stick Down");
      this.AddControl(InputControlType.LeftTrigger, "Left Trigger");
      this.AddControl(InputControlType.RightTrigger, "Right Trigger");
      this.AddControl(InputControlType.DPadUp, "DPad Up");
      this.AddControl(InputControlType.DPadDown, "DPad Down");
      this.AddControl(InputControlType.DPadLeft, "DPad Left");
      this.AddControl(InputControlType.DPadRight, "DPad Right");
      this.AddControl(InputControlType.Action1, "O");
      this.AddControl(InputControlType.Action2, "A");
      this.AddControl(InputControlType.Action3, "Y");
      this.AddControl(InputControlType.Action4, "U");
      this.AddControl(InputControlType.LeftBumper, "Left Bumper");
      this.AddControl(InputControlType.RightBumper, "Right Bumper");
      this.AddControl(InputControlType.LeftStickButton, "Left Stick Button");
      this.AddControl(InputControlType.RightStickButton, "Right Stick Button");
      this.AddControl(InputControlType.Menu, "Menu");
    }

    public int DeviceIndex { get; private set; }

    public void BeforeAttach()
    {
    }

    public override void Update(ulong updateTick, float deltaTime)
    {
    }

    public bool IsConnected => false;
  }
}
