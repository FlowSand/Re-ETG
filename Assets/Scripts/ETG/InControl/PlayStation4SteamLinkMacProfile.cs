// Decompiled with JetBrains decompiler
// Type: InControl.PlayStation4SteamLinkMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl;

[AutoDiscover]
public class PlayStation4SteamLinkMacProfile : UnityInputDeviceProfile
{
  public PlayStation4SteamLinkMacProfile()
  {
    this.Name = "PlayStation 4 Controller via Steam Link";
    this.Meta = "PlayStation 4 Controller on Mac via Steam Link";
    this.DeviceClass = InputDeviceClass.Controller;
    this.IncludePlatforms = new string[1]{ "OS X" };
    this.JoystickNames = new string[1]
    {
      "Microsoft StreamingGamePad-1"
    };
    this.ButtonMappings = new InputControlMapping[15]
    {
      new InputControlMapping()
      {
        Handle = "Cross",
        Target = InputControlType.Action1,
        Source = UnityInputDeviceProfile.Button16
      },
      new InputControlMapping()
      {
        Handle = "Circle",
        Target = InputControlType.Action2,
        Source = UnityInputDeviceProfile.Button17
      },
      new InputControlMapping()
      {
        Handle = "Square",
        Target = InputControlType.Action3,
        Source = UnityInputDeviceProfile.Button18
      },
      new InputControlMapping()
      {
        Handle = "Triangle",
        Target = InputControlType.Action4,
        Source = UnityInputDeviceProfile.Button19
      },
      new InputControlMapping()
      {
        Handle = "Left Bumper",
        Target = InputControlType.LeftBumper,
        Source = UnityInputDeviceProfile.Button13
      },
      new InputControlMapping()
      {
        Handle = "Right Bumper",
        Target = InputControlType.RightBumper,
        Source = UnityInputDeviceProfile.Button14
      },
      new InputControlMapping()
      {
        Handle = "Share",
        Target = InputControlType.Share,
        Source = UnityInputDeviceProfile.Button10
      },
      new InputControlMapping()
      {
        Handle = "Options",
        Target = InputControlType.Options,
        Source = UnityInputDeviceProfile.Button9
      },
      new InputControlMapping()
      {
        Handle = "DPad Up",
        Target = InputControlType.DPadUp,
        Source = UnityInputDeviceProfile.Button5
      },
      new InputControlMapping()
      {
        Handle = "DPad Down",
        Target = InputControlType.DPadDown,
        Source = UnityInputDeviceProfile.Button6
      },
      new InputControlMapping()
      {
        Handle = "DPad Left",
        Target = InputControlType.DPadLeft,
        Source = UnityInputDeviceProfile.Button7
      },
      new InputControlMapping()
      {
        Handle = "DPad Right",
        Target = InputControlType.DPadRight,
        Source = UnityInputDeviceProfile.Button8
      },
      new InputControlMapping()
      {
        Handle = "Left Stick Button",
        Target = InputControlType.LeftStickButton,
        Source = UnityInputDeviceProfile.Button11
      },
      new InputControlMapping()
      {
        Handle = "Right Stick Button",
        Target = InputControlType.RightStickButton,
        Source = UnityInputDeviceProfile.Button12
      },
      new InputControlMapping()
      {
        Handle = "System",
        Target = InputControlType.System,
        Source = UnityInputDeviceProfile.Button15
      }
    };
    this.AnalogMappings = new InputControlMapping[10]
    {
      UnityInputDeviceProfile.LeftStickLeftMapping(UnityInputDeviceProfile.Analog0),
      UnityInputDeviceProfile.LeftStickRightMapping(UnityInputDeviceProfile.Analog0),
      UnityInputDeviceProfile.LeftStickUpMapping(UnityInputDeviceProfile.Analog1),
      UnityInputDeviceProfile.LeftStickDownMapping(UnityInputDeviceProfile.Analog1),
      UnityInputDeviceProfile.RightStickLeftMapping(UnityInputDeviceProfile.Analog2),
      UnityInputDeviceProfile.RightStickRightMapping(UnityInputDeviceProfile.Analog2),
      UnityInputDeviceProfile.RightStickUpMapping(UnityInputDeviceProfile.Analog3),
      UnityInputDeviceProfile.RightStickDownMapping(UnityInputDeviceProfile.Analog3),
      UnityInputDeviceProfile.LeftTriggerMapping(UnityInputDeviceProfile.Analog4),
      UnityInputDeviceProfile.RightTriggerMapping(UnityInputDeviceProfile.Analog5)
    };
  }
}
