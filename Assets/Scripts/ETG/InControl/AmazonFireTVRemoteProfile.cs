// Decompiled with JetBrains decompiler
// Type: InControl.AmazonFireTVRemoteProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl;

[AutoDiscover]
public class AmazonFireTVRemoteProfile : UnityInputDeviceProfile
{
  public AmazonFireTVRemoteProfile()
  {
    this.Name = "Amazon Fire TV Remote";
    this.Meta = "Amazon Fire TV Remote on Amazon Fire TV";
    this.DeviceClass = InputDeviceClass.Remote;
    this.DeviceStyle = InputDeviceStyle.AmazonFireTV;
    this.IncludePlatforms = new string[1]{ "Amazon AFT" };
    this.JoystickNames = new string[2]
    {
      string.Empty,
      "Amazon Fire TV Remote"
    };
    this.ButtonMappings = new InputControlMapping[3]
    {
      new InputControlMapping()
      {
        Handle = "A",
        Target = InputControlType.Action1,
        Source = UnityInputDeviceProfile.Button0
      },
      new InputControlMapping()
      {
        Handle = "Back",
        Target = InputControlType.Back,
        Source = UnityInputDeviceProfile.EscapeKey
      },
      new InputControlMapping()
      {
        Handle = "Menu",
        Target = InputControlType.Menu,
        Source = UnityInputDeviceProfile.MenuKey
      }
    };
    this.AnalogMappings = new InputControlMapping[4]
    {
      UnityInputDeviceProfile.DPadLeftMapping(UnityInputDeviceProfile.Analog4),
      UnityInputDeviceProfile.DPadRightMapping(UnityInputDeviceProfile.Analog4),
      UnityInputDeviceProfile.DPadUpMapping(UnityInputDeviceProfile.Analog5),
      UnityInputDeviceProfile.DPadDownMapping(UnityInputDeviceProfile.Analog5)
    };
  }
}
