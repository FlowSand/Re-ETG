// Decompiled with JetBrains decompiler
// Type: InControl.PlayStation3LinuxProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl
{
  [AutoDiscover]
  public class PlayStation3LinuxProfile : UnityInputDeviceProfile
  {
    public PlayStation3LinuxProfile()
    {
      this.Name = "PlayStation 3 Controller";
      this.Meta = "PlayStation 3 Controller on Linux";
      this.DeviceClass = InputDeviceClass.Controller;
      this.DeviceStyle = InputDeviceStyle.PlayStation3;
      this.IncludePlatforms = new string[1]{ "Linux" };
      this.JoystickNames = new string[2]
      {
        "Sony PLAYSTATION(R)3 Controller",
        "SHENGHIC 2009/0708ZXW-V1Inc. PLAYSTATION(R)3Conteroller"
      };
      this.ControllerSymbology = GameOptions.ControllerSymbology.PS4;
      this.MaxUnityVersion = new VersionInfo(4, 9, 0, 0);
      this.ButtonMappings = new InputControlMapping[17]
      {
        new InputControlMapping()
        {
          Handle = "Cross",
          Target = InputControlType.Action1,
          Source = UnityInputDeviceProfile.Button14
        },
        new InputControlMapping()
        {
          Handle = "Circle",
          Target = InputControlType.Action2,
          Source = UnityInputDeviceProfile.Button13
        },
        new InputControlMapping()
        {
          Handle = "Square",
          Target = InputControlType.Action3,
          Source = UnityInputDeviceProfile.Button15
        },
        new InputControlMapping()
        {
          Handle = "Triangle",
          Target = InputControlType.Action4,
          Source = UnityInputDeviceProfile.Button12
        },
        new InputControlMapping()
        {
          Handle = "DPad Up",
          Target = InputControlType.DPadUp,
          Source = UnityInputDeviceProfile.Button4
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
          Source = UnityInputDeviceProfile.Button5
        },
        new InputControlMapping()
        {
          Handle = "Left Bumper",
          Target = InputControlType.LeftBumper,
          Source = UnityInputDeviceProfile.Button10
        },
        new InputControlMapping()
        {
          Handle = "Right Bumper",
          Target = InputControlType.RightBumper,
          Source = UnityInputDeviceProfile.Button11
        },
        new InputControlMapping()
        {
          Handle = "Start",
          Target = InputControlType.Start,
          Source = UnityInputDeviceProfile.Button3
        },
        new InputControlMapping()
        {
          Handle = "Select",
          Target = InputControlType.Select,
          Source = UnityInputDeviceProfile.Button0
        },
        new InputControlMapping()
        {
          Handle = "Left Trigger",
          Target = InputControlType.LeftTrigger,
          Source = UnityInputDeviceProfile.Button8
        },
        new InputControlMapping()
        {
          Handle = "Right Trigger",
          Target = InputControlType.RightTrigger,
          Source = UnityInputDeviceProfile.Button9
        },
        new InputControlMapping()
        {
          Handle = "Left Stick Button",
          Target = InputControlType.LeftStickButton,
          Source = UnityInputDeviceProfile.Button1
        },
        new InputControlMapping()
        {
          Handle = "Right Stick Button",
          Target = InputControlType.RightStickButton,
          Source = UnityInputDeviceProfile.Button2
        },
        new InputControlMapping()
        {
          Handle = "System",
          Target = InputControlType.System,
          Source = UnityInputDeviceProfile.Button16
        }
      };
      this.AnalogMappings = new InputControlMapping[8]
      {
        UnityInputDeviceProfile.LeftStickLeftMapping(UnityInputDeviceProfile.Analog0),
        UnityInputDeviceProfile.LeftStickRightMapping(UnityInputDeviceProfile.Analog0),
        UnityInputDeviceProfile.LeftStickUpMapping(UnityInputDeviceProfile.Analog1),
        UnityInputDeviceProfile.LeftStickDownMapping(UnityInputDeviceProfile.Analog1),
        UnityInputDeviceProfile.RightStickLeftMapping(UnityInputDeviceProfile.Analog2),
        UnityInputDeviceProfile.RightStickRightMapping(UnityInputDeviceProfile.Analog2),
        UnityInputDeviceProfile.RightStickUpMapping(UnityInputDeviceProfile.Analog3),
        UnityInputDeviceProfile.RightStickDownMapping(UnityInputDeviceProfile.Analog3)
      };
    }
  }
}
