// Decompiled with JetBrains decompiler
// Type: InControl.AppleTVRemoteProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl;

[AutoDiscover]
public class AppleTVRemoteProfile : UnityInputDeviceProfile
{
  public AppleTVRemoteProfile()
  {
    this.Name = "Apple TV Remote";
    this.Meta = "Apple TV Remote on tvOS";
    this.DeviceClass = InputDeviceClass.Remote;
    this.DeviceStyle = InputDeviceStyle.AppleMFi;
    this.IncludePlatforms = new string[1]{ "AppleTV" };
    this.JoystickRegex = new string[1]{ "Remote" };
    this.LowerDeadZone = 0.05f;
    this.UpperDeadZone = 0.95f;
    this.ButtonMappings = new InputControlMapping[3]
    {
      new InputControlMapping()
      {
        Handle = "TouchPad Click",
        Target = InputControlType.Action1,
        Source = UnityInputDeviceProfile.Button14
      },
      new InputControlMapping()
      {
        Handle = "Play/Pause",
        Target = InputControlType.Action2,
        Source = UnityInputDeviceProfile.Button15
      },
      new InputControlMapping()
      {
        Handle = "Menu",
        Target = InputControlType.Menu,
        Source = UnityInputDeviceProfile.Button0
      }
    };
    this.AnalogMappings = new InputControlMapping[11]
    {
      UnityInputDeviceProfile.LeftStickLeftMapping(UnityInputDeviceProfile.Analog0),
      UnityInputDeviceProfile.LeftStickRightMapping(UnityInputDeviceProfile.Analog0),
      UnityInputDeviceProfile.LeftStickUpMapping(UnityInputDeviceProfile.Analog1),
      UnityInputDeviceProfile.LeftStickDownMapping(UnityInputDeviceProfile.Analog1),
      new InputControlMapping()
      {
        Handle = "TouchPad X",
        Target = InputControlType.TouchPadXAxis,
        Source = UnityInputDeviceProfile.Analog0,
        Raw = true
      },
      new InputControlMapping()
      {
        Handle = "TouchPad Y",
        Target = InputControlType.TouchPadYAxis,
        Source = UnityInputDeviceProfile.Analog1,
        Raw = true
      },
      new InputControlMapping()
      {
        Handle = "Orientation X",
        Target = InputControlType.TiltX,
        Source = UnityInputDeviceProfile.Analog15,
        Passive = true
      },
      new InputControlMapping()
      {
        Handle = "Orientation Y",
        Target = InputControlType.TiltY,
        Source = UnityInputDeviceProfile.Analog16,
        Passive = true
      },
      new InputControlMapping()
      {
        Handle = "Orientation Z",
        Target = InputControlType.TiltZ,
        Source = UnityInputDeviceProfile.Analog17,
        Passive = true
      },
      new InputControlMapping()
      {
        Handle = "Acceleration X",
        Target = InputControlType.Analog0,
        Source = UnityInputDeviceProfile.Analog18,
        Passive = true
      },
      new InputControlMapping()
      {
        Handle = "Acceleration Y",
        Target = InputControlType.Analog1,
        Source = UnityInputDeviceProfile.Analog19,
        Passive = true
      }
    };
  }
}
