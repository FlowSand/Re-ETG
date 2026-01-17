// Decompiled with JetBrains decompiler
// Type: InControl.NintendoSwitchProMacNativeProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl;

[AutoDiscover]
public class NintendoSwitchProMacNativeProfile : NativeInputDeviceProfile
{
  public NintendoSwitchProMacNativeProfile()
  {
    this.Name = "Nintendo Switch Pro Controller";
    this.Meta = "Nintendo Switch Pro Controller on Mac";
    this.DeviceClass = InputDeviceClass.Controller;
    this.DeviceStyle = InputDeviceStyle.NintendoSwitch;
    this.UpperDeadZone = 0.7f;
    this.IncludePlatforms = new string[1]{ "OS X" };
    this.Matchers = new NativeInputDeviceMatcher[1]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 1406),
        ProductID = new ushort?((ushort) 8201)
      }
    };
    this.ButtonMappings = new InputControlMapping[18]
    {
      new InputControlMapping()
      {
        Handle = "B",
        Target = InputControlType.Action1,
        Source = NativeInputDeviceProfile.Button(0)
      },
      new InputControlMapping()
      {
        Handle = "A",
        Target = InputControlType.Action2,
        Source = NativeInputDeviceProfile.Button(1)
      },
      new InputControlMapping()
      {
        Handle = "Y",
        Target = InputControlType.Action3,
        Source = NativeInputDeviceProfile.Button(2)
      },
      new InputControlMapping()
      {
        Handle = "X",
        Target = InputControlType.Action4,
        Source = NativeInputDeviceProfile.Button(3)
      },
      new InputControlMapping()
      {
        Handle = "L",
        Target = InputControlType.LeftBumper,
        Source = NativeInputDeviceProfile.Button(4)
      },
      new InputControlMapping()
      {
        Handle = "R",
        Target = InputControlType.RightBumper,
        Source = NativeInputDeviceProfile.Button(5)
      },
      new InputControlMapping()
      {
        Handle = "ZL",
        Target = InputControlType.LeftTrigger,
        Source = NativeInputDeviceProfile.Button(6)
      },
      new InputControlMapping()
      {
        Handle = "ZR",
        Target = InputControlType.RightTrigger,
        Source = NativeInputDeviceProfile.Button(7)
      },
      new InputControlMapping()
      {
        Handle = "Minus",
        Target = InputControlType.Minus,
        Source = NativeInputDeviceProfile.Button(8)
      },
      new InputControlMapping()
      {
        Handle = "Plus",
        Target = InputControlType.Plus,
        Source = NativeInputDeviceProfile.Button(9)
      },
      new InputControlMapping()
      {
        Handle = "Left Stick Button",
        Target = InputControlType.LeftStickButton,
        Source = NativeInputDeviceProfile.Button(10)
      },
      new InputControlMapping()
      {
        Handle = "Right Stick Button",
        Target = InputControlType.RightStickButton,
        Source = NativeInputDeviceProfile.Button(11)
      },
      new InputControlMapping()
      {
        Handle = "Home",
        Target = InputControlType.Home,
        Source = NativeInputDeviceProfile.Button(12)
      },
      new InputControlMapping()
      {
        Handle = "Capture",
        Target = InputControlType.Capture,
        Source = NativeInputDeviceProfile.Button(13)
      },
      new InputControlMapping()
      {
        Handle = "DPad Up",
        Target = InputControlType.DPadUp,
        Source = NativeInputDeviceProfile.Button(16 /*0x10*/)
      },
      new InputControlMapping()
      {
        Handle = "DPad Down",
        Target = InputControlType.DPadDown,
        Source = NativeInputDeviceProfile.Button(17)
      },
      new InputControlMapping()
      {
        Handle = "DPad Left",
        Target = InputControlType.DPadLeft,
        Source = NativeInputDeviceProfile.Button(18)
      },
      new InputControlMapping()
      {
        Handle = "DPad Right",
        Target = InputControlType.DPadRight,
        Source = NativeInputDeviceProfile.Button(19)
      }
    };
    this.AnalogMappings = new InputControlMapping[8]
    {
      new InputControlMapping()
      {
        Handle = "Left Stick Left",
        Target = InputControlType.LeftStickLeft,
        Source = NativeInputDeviceProfile.Analog(0),
        SourceRange = InputRange.ZeroToMinusOne,
        TargetRange = InputRange.ZeroToOne
      },
      new InputControlMapping()
      {
        Handle = "Left Stick Right",
        Target = InputControlType.LeftStickRight,
        Source = NativeInputDeviceProfile.Analog(0),
        SourceRange = InputRange.ZeroToOne,
        TargetRange = InputRange.ZeroToOne
      },
      new InputControlMapping()
      {
        Handle = "Left Stick Up",
        Target = InputControlType.LeftStickUp,
        Source = NativeInputDeviceProfile.Analog(1),
        SourceRange = InputRange.ZeroToMinusOne,
        TargetRange = InputRange.ZeroToOne
      },
      new InputControlMapping()
      {
        Handle = "Left Stick Down",
        Target = InputControlType.LeftStickDown,
        Source = NativeInputDeviceProfile.Analog(1),
        SourceRange = InputRange.ZeroToOne,
        TargetRange = InputRange.ZeroToOne
      },
      new InputControlMapping()
      {
        Handle = "Right Stick Left",
        Target = InputControlType.RightStickLeft,
        Source = NativeInputDeviceProfile.Analog(2),
        SourceRange = InputRange.ZeroToMinusOne,
        TargetRange = InputRange.ZeroToOne
      },
      new InputControlMapping()
      {
        Handle = "Right Stick Right",
        Target = InputControlType.RightStickRight,
        Source = NativeInputDeviceProfile.Analog(2),
        SourceRange = InputRange.ZeroToOne,
        TargetRange = InputRange.ZeroToOne
      },
      new InputControlMapping()
      {
        Handle = "Right Stick Up",
        Target = InputControlType.RightStickUp,
        Source = NativeInputDeviceProfile.Analog(3),
        SourceRange = InputRange.ZeroToMinusOne,
        TargetRange = InputRange.ZeroToOne
      },
      new InputControlMapping()
      {
        Handle = "Right Stick Down",
        Target = InputControlType.RightStickDown,
        Source = NativeInputDeviceProfile.Analog(3),
        SourceRange = InputRange.ZeroToOne,
        TargetRange = InputRange.ZeroToOne
      }
    };
  }
}
