using System;

#nullable disable
namespace InControl
{
  public class XboxOneInputDevice : InputDevice
  {
    private const uint AnalogLeftStickX = 0;
    private const uint AnalogLeftStickY = 1;
    private const uint AnalogRightStickX = 3;
    private const uint AnalogRightStickY = 4;
    private const uint AnalogLeftTrigger = 8;
    private const uint AnalogRightTrigger = 9;
    private const float LowerDeadZone = 0.2f;
    private const float UpperDeadZone = 0.9f;
    private string[] analogAxisNameForId;

    public XboxOneInputDevice(uint joystickId)
      : base("Xbox One Controller")
    {
      this.JoystickId = joystickId;
      this.SortOrder = (int) joystickId;
      this.Meta = "Xbox One Device #" + (object) joystickId;
      this.DeviceClass = InputDeviceClass.Controller;
      this.DeviceStyle = InputDeviceStyle.XboxOne;
      this.CacheAnalogAxisNames();
      this.AddControl(InputControlType.LeftStickLeft, "Left Stick Left", 0.2f, 0.9f);
      this.AddControl(InputControlType.LeftStickRight, "Left Stick Right", 0.2f, 0.9f);
      this.AddControl(InputControlType.LeftStickUp, "Left Stick Up", 0.2f, 0.9f);
      this.AddControl(InputControlType.LeftStickDown, "Left Stick Down", 0.2f, 0.9f);
      this.AddControl(InputControlType.RightStickLeft, "Right Stick Left", 0.2f, 0.9f);
      this.AddControl(InputControlType.RightStickRight, "Right Stick Right", 0.2f, 0.9f);
      this.AddControl(InputControlType.RightStickUp, "Right Stick Up", 0.2f, 0.9f);
      this.AddControl(InputControlType.RightStickDown, "Right Stick Down", 0.2f, 0.9f);
      this.AddControl(InputControlType.LeftTrigger, "Left Trigger", 0.2f, 0.9f);
      this.AddControl(InputControlType.RightTrigger, "Right Trigger", 0.2f, 0.9f);
      this.AddControl(InputControlType.DPadUp, "DPad Up", 0.2f, 0.9f);
      this.AddControl(InputControlType.DPadDown, "DPad Down", 0.2f, 0.9f);
      this.AddControl(InputControlType.DPadLeft, "DPad Left", 0.2f, 0.9f);
      this.AddControl(InputControlType.DPadRight, "DPad Right", 0.2f, 0.9f);
      this.AddControl(InputControlType.Action1, "A");
      this.AddControl(InputControlType.Action2, "B");
      this.AddControl(InputControlType.Action3, "X");
      this.AddControl(InputControlType.Action4, "Y");
      this.AddControl(InputControlType.LeftBumper, "Left Bumper");
      this.AddControl(InputControlType.RightBumper, "Right Bumper");
      this.AddControl(InputControlType.LeftStickButton, "Left Stick Button");
      this.AddControl(InputControlType.RightStickButton, "Right Stick Button");
      this.AddControl(InputControlType.View, "View");
      this.AddControl(InputControlType.Menu, "Menu");
    }

    internal uint JoystickId { get; private set; }

    public ulong ControllerId { get; private set; }

    public override void Update(ulong updateTick, float deltaTime)
    {
    }

    public bool IsConnected => false;

    public override void Vibrate(float leftMotor, float rightMotor)
    {
    }

    public void Vibrate(float leftMotor, float rightMotor, float leftTrigger, float rightTrigger)
    {
    }

    private string AnalogAxisNameForId(uint analogId) => this.analogAxisNameForId[(IntPtr) analogId];

    private void CacheAnalogAxisNameForId(uint analogId)
    {
      this.analogAxisNameForId[(IntPtr) analogId] = $"joystick {(object) this.JoystickId} analog {(object) analogId}";
    }

    private void CacheAnalogAxisNames()
    {
      this.analogAxisNameForId = new string[16 /*0x10*/];
      this.CacheAnalogAxisNameForId(0U);
      this.CacheAnalogAxisNameForId(1U);
      this.CacheAnalogAxisNameForId(3U);
      this.CacheAnalogAxisNameForId(4U);
      this.CacheAnalogAxisNameForId(8U);
      this.CacheAnalogAxisNameForId(9U);
    }
  }
}
