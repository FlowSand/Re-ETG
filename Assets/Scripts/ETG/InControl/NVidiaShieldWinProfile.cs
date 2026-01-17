// Decompiled with JetBrains decompiler
// Type: InControl.NVidiaShieldWinProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl;

[AutoDiscover]
public class NVidiaShieldWinProfile : UnityInputDeviceProfile
{
  public NVidiaShieldWinProfile()
  {
    this.Name = "NVIDIA Shield Controller";
    this.Meta = "NVIDIA Shield Controller on Windows";
    this.DeviceClass = InputDeviceClass.Controller;
    this.DeviceStyle = InputDeviceStyle.NVIDIAShield;
    this.IncludePlatforms = new string[2]
    {
      "Windows 7",
      "Windows 8"
    };
    this.JoystickRegex = new string[1]
    {
      "NVIDIA Controller"
    };
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
        Handle = "Back",
        Target = InputControlType.Back,
        Source = UnityInputDeviceProfile.Button6
      },
      new InputControlMapping()
      {
        Handle = "Start",
        Target = InputControlType.Start,
        Source = UnityInputDeviceProfile.Button7
      }
    };
    this.AnalogMappings = new InputControlMapping[16 /*0x10*/]
    {
      UnityInputDeviceProfile.LeftStickLeftMapping(UnityInputDeviceProfile.Analog0),
      UnityInputDeviceProfile.LeftStickRightMapping(UnityInputDeviceProfile.Analog0),
      UnityInputDeviceProfile.LeftStickUpMapping(UnityInputDeviceProfile.Analog1),
      UnityInputDeviceProfile.LeftStickDownMapping(UnityInputDeviceProfile.Analog1),
      UnityInputDeviceProfile.RightStickLeftMapping(UnityInputDeviceProfile.Analog3),
      UnityInputDeviceProfile.RightStickRightMapping(UnityInputDeviceProfile.Analog3),
      UnityInputDeviceProfile.RightStickUpMapping(UnityInputDeviceProfile.Analog4),
      UnityInputDeviceProfile.RightStickDownMapping(UnityInputDeviceProfile.Analog4),
      UnityInputDeviceProfile.DPadLeftMapping(UnityInputDeviceProfile.Analog5),
      UnityInputDeviceProfile.DPadRightMapping(UnityInputDeviceProfile.Analog5),
      UnityInputDeviceProfile.DPadUpMapping2(UnityInputDeviceProfile.Analog6),
      UnityInputDeviceProfile.DPadDownMapping2(UnityInputDeviceProfile.Analog6),
      new InputControlMapping()
      {
        Handle = "Left Trigger",
        Target = InputControlType.LeftTrigger,
        Source = UnityInputDeviceProfile.Analog2,
        SourceRange = InputRange.ZeroToOne,
        TargetRange = InputRange.ZeroToOne
      },
      new InputControlMapping()
      {
        Handle = "Right Trigger",
        Target = InputControlType.RightTrigger,
        Source = UnityInputDeviceProfile.Analog2,
        SourceRange = InputRange.ZeroToMinusOne,
        TargetRange = InputRange.ZeroToOne
      },
      new InputControlMapping()
      {
        Handle = "Left Trigger",
        Target = InputControlType.LeftTrigger,
        Source = UnityInputDeviceProfile.Analog8
      },
      new InputControlMapping()
      {
        Handle = "Right Trigger",
        Target = InputControlType.RightTrigger,
        Source = UnityInputDeviceProfile.Analog9
      }
    };
  }
}
