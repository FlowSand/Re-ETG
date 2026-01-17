// Decompiled with JetBrains decompiler
// Type: InControl.RazerServalMacProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl
{
  [AutoDiscover]
  public class RazerServalMacProfile : UnityInputDeviceProfile
  {
    public RazerServalMacProfile()
    {
      this.Name = "Razer Serval Controller";
      this.Meta = "Razer Serval Controller on Mac";
      this.DeviceClass = InputDeviceClass.Controller;
      this.IncludePlatforms = new string[1]{ "OS X" };
      this.JoystickNames = new string[2]
      {
        "Razer Razer Serval",
        "Unknown Razer Serval"
      };
      this.ButtonMappings = new InputControlMapping[13]
      {
        new InputControlMapping()
        {
          Handle = "A",
          Target = InputControlType.Action1,
          Source = UnityInputDeviceProfile.Button0
        },
        new InputControlMapping()
        {
          Handle = "B",
          Target = InputControlType.Action2,
          Source = UnityInputDeviceProfile.Button1
        },
        new InputControlMapping()
        {
          Handle = "X",
          Target = InputControlType.Action3,
          Source = UnityInputDeviceProfile.Button2
        },
        new InputControlMapping()
        {
          Handle = "Y",
          Target = InputControlType.Action4,
          Source = UnityInputDeviceProfile.Button3
        },
        new InputControlMapping()
        {
          Handle = "Back",
          Target = InputControlType.Back,
          Source = UnityInputDeviceProfile.Button6
        },
        new InputControlMapping()
        {
          Handle = "Home",
          Target = InputControlType.Home,
          Source = UnityInputDeviceProfile.Button11
        },
        new InputControlMapping()
        {
          Handle = "Left Stick Button",
          Target = InputControlType.LeftStickButton,
          Source = UnityInputDeviceProfile.Button8
        },
        new InputControlMapping()
        {
          Handle = "Right Stick Button",
          Target = InputControlType.RightStickButton,
          Source = UnityInputDeviceProfile.Button9
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
          Source = UnityInputDeviceProfile.Button12
        },
        new InputControlMapping()
        {
          Handle = "Start",
          Target = InputControlType.Start,
          Source = UnityInputDeviceProfile.Button7
        },
        new InputControlMapping()
        {
          Handle = "Power",
          Target = InputControlType.Power,
          Source = UnityInputDeviceProfile.Button10
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
}
