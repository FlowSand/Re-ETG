// Decompiled with JetBrains decompiler
// Type: InControl.BuffaloClassicMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl
{
  [AutoDiscover]
  public class BuffaloClassicMacProfile : UnityInputDeviceProfile
  {
    public BuffaloClassicMacProfile()
    {
      this.Name = "iBuffalo Classic Controller";
      this.Meta = "iBuffalo Classic Controller on Mac";
      this.DeviceClass = InputDeviceClass.Controller;
      this.IncludePlatforms = new string[1]{ "OS X" };
      this.JoystickNames = new string[1]
      {
        " USB,2-axis 8-button gamepad"
      };
      this.ButtonMappings = new InputControlMapping[8]
      {
        new InputControlMapping()
        {
          Handle = "A",
          Target = InputControlType.Action2,
          Source = UnityInputDeviceProfile.Button0
        },
        new InputControlMapping()
        {
          Handle = "B",
          Target = InputControlType.Action1,
          Source = UnityInputDeviceProfile.Button1
        },
        new InputControlMapping()
        {
          Handle = "X",
          Target = InputControlType.Action4,
          Source = UnityInputDeviceProfile.Button2
        },
        new InputControlMapping()
        {
          Handle = "Y",
          Target = InputControlType.Action3,
          Source = UnityInputDeviceProfile.Button3
        },
        new InputControlMapping()
        {
          Handle = "Left Bumper",
          Target = InputControlType.LeftBumper,
          Source = UnityInputDeviceProfile.Button4
        },
        new InputControlMapping()
        {
          Handle = "Right Bumper",
          Target = InputControlType.RightBumper,
          Source = UnityInputDeviceProfile.Button5
        },
        new InputControlMapping()
        {
          Handle = "Select",
          Target = InputControlType.Select,
          Source = UnityInputDeviceProfile.Button6
        },
        new InputControlMapping()
        {
          Handle = "Start",
          Target = InputControlType.Start,
          Source = UnityInputDeviceProfile.Button7
        }
      };
      this.AnalogMappings = new InputControlMapping[4]
      {
        UnityInputDeviceProfile.DPadLeftMapping(UnityInputDeviceProfile.Analog0),
        UnityInputDeviceProfile.DPadRightMapping(UnityInputDeviceProfile.Analog0),
        UnityInputDeviceProfile.DPadUpMapping(UnityInputDeviceProfile.Analog1),
        UnityInputDeviceProfile.DPadDownMapping(UnityInputDeviceProfile.Analog1)
      };
    }
  }
}
