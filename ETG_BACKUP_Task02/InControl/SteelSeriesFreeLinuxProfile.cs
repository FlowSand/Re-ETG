// Decompiled with JetBrains decompiler
// Type: InControl.SteelSeriesFreeLinuxProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl;

[AutoDiscover]
public class SteelSeriesFreeLinuxProfile : UnityInputDeviceProfile
{
  public SteelSeriesFreeLinuxProfile()
  {
    this.Name = "SteelSeries Free";
    this.Meta = "SteelSeries Free on Linux";
    this.DeviceClass = InputDeviceClass.Controller;
    this.IncludePlatforms = new string[1]{ "Linux" };
    this.JoystickNames = new string[1]
    {
      "Zeemote: SteelSeries FREE"
    };
    this.ButtonMappings = new InputControlMapping[8]
    {
      new InputControlMapping()
      {
        Handle = "4",
        Target = InputControlType.Action1,
        Source = UnityInputDeviceProfile.Button0
      },
      new InputControlMapping()
      {
        Handle = "3",
        Target = InputControlType.Action2,
        Source = UnityInputDeviceProfile.Button1
      },
      new InputControlMapping()
      {
        Handle = "1",
        Target = InputControlType.Action3,
        Source = UnityInputDeviceProfile.Button3
      },
      new InputControlMapping()
      {
        Handle = "2",
        Target = InputControlType.Action4,
        Source = UnityInputDeviceProfile.Button4
      },
      new InputControlMapping()
      {
        Handle = "Left Bumper",
        Target = InputControlType.LeftBumper,
        Source = UnityInputDeviceProfile.Button6
      },
      new InputControlMapping()
      {
        Handle = "Right Bumper",
        Target = InputControlType.RightBumper,
        Source = UnityInputDeviceProfile.Button7
      },
      new InputControlMapping()
      {
        Handle = "Back",
        Target = InputControlType.Select,
        Source = UnityInputDeviceProfile.Button12
      },
      new InputControlMapping()
      {
        Handle = "Start",
        Target = InputControlType.Start,
        Source = UnityInputDeviceProfile.Button11
      }
    };
    this.AnalogMappings = new InputControlMapping[12]
    {
      UnityInputDeviceProfile.LeftStickLeftMapping(UnityInputDeviceProfile.Analog0),
      UnityInputDeviceProfile.LeftStickRightMapping(UnityInputDeviceProfile.Analog0),
      UnityInputDeviceProfile.LeftStickUpMapping(UnityInputDeviceProfile.Analog1),
      UnityInputDeviceProfile.LeftStickDownMapping(UnityInputDeviceProfile.Analog1),
      UnityInputDeviceProfile.RightStickLeftMapping(UnityInputDeviceProfile.Analog2),
      UnityInputDeviceProfile.RightStickRightMapping(UnityInputDeviceProfile.Analog2),
      UnityInputDeviceProfile.RightStickUpMapping(UnityInputDeviceProfile.Analog3),
      UnityInputDeviceProfile.RightStickDownMapping(UnityInputDeviceProfile.Analog3),
      UnityInputDeviceProfile.DPadLeftMapping(UnityInputDeviceProfile.Analog4),
      UnityInputDeviceProfile.DPadRightMapping(UnityInputDeviceProfile.Analog4),
      UnityInputDeviceProfile.DPadUpMapping(UnityInputDeviceProfile.Analog5),
      UnityInputDeviceProfile.DPadDownMapping(UnityInputDeviceProfile.Analog5)
    };
  }
}
