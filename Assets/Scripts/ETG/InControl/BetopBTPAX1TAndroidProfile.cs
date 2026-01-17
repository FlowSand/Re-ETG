// Decompiled with JetBrains decompiler
// Type: InControl.BetopBTPAX1TAndroidProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl
{
  [AutoDiscover]
  public class BetopBTPAX1TAndroidProfile : UnityInputDeviceProfile
  {
    public BetopBTPAX1TAndroidProfile()
    {
      this.Name = "BETOP BTP-AX1T Controller";
      this.Meta = "BETOP BTP-AX1T Controller on Android";
      this.DeviceClass = InputDeviceClass.Controller;
      this.DeviceStyle = InputDeviceStyle.Xbox360;
      this.IncludePlatforms = new string[1]{ "Android" };
      this.JoystickNames = new string[1]{ "BETOP AX1 BFM" };
      this.ButtonMappings = new InputControlMapping[10]
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
          Handle = "Select",
          Target = InputControlType.Select,
          Source = UnityInputDeviceProfile.Button11
        },
        new InputControlMapping()
        {
          Handle = "Start",
          Target = InputControlType.Start,
          Source = UnityInputDeviceProfile.Button10
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
          Handle = "Left Trigger",
          Target = InputControlType.LeftTrigger,
          Source = UnityInputDeviceProfile.Button6
        },
        new InputControlMapping()
        {
          Handle = "Right Trigger",
          Target = InputControlType.RightTrigger,
          Source = UnityInputDeviceProfile.Button7
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
