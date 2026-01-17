// Decompiled with JetBrains decompiler
// Type: InControl.EightBitdoNES30ProUSBMacNativeProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl;

[AutoDiscover]
public class EightBitdoNES30ProUSBMacNativeProfile : NativeInputDeviceProfile
{
  public EightBitdoNES30ProUSBMacNativeProfile()
  {
    this.Name = "8Bitdo NES30 Pro Controller";
    this.Meta = "8Bitdo NES30 Pro Controller on Mac";
    this.DeviceClass = InputDeviceClass.Controller;
    this.DeviceStyle = InputDeviceStyle.NintendoNES;
    this.Matchers = new NativeInputDeviceMatcher[1]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 8194),
        ProductID = new ushort?((ushort) 36864 /*0x9000*/)
      }
    };
    this.ButtonMappings = new InputControlMapping[16 /*0x10*/]
    {
      new InputControlMapping()
      {
        Handle = "A",
        Target = InputControlType.Action2,
        Source = NativeInputDeviceProfile.Button(0)
      },
      new InputControlMapping()
      {
        Handle = "B",
        Target = InputControlType.Action1,
        Source = NativeInputDeviceProfile.Button(1)
      },
      new InputControlMapping()
      {
        Handle = "X",
        Target = InputControlType.Action4,
        Source = NativeInputDeviceProfile.Button(3)
      },
      new InputControlMapping()
      {
        Handle = "Y",
        Target = InputControlType.Action3,
        Source = NativeInputDeviceProfile.Button(4)
      },
      new InputControlMapping()
      {
        Handle = "L1",
        Target = InputControlType.LeftBumper,
        Source = NativeInputDeviceProfile.Button(6)
      },
      new InputControlMapping()
      {
        Handle = "R1",
        Target = InputControlType.RightBumper,
        Source = NativeInputDeviceProfile.Button(7)
      },
      new InputControlMapping()
      {
        Handle = "L2",
        Target = InputControlType.LeftTrigger,
        Source = NativeInputDeviceProfile.Button(8)
      },
      new InputControlMapping()
      {
        Handle = "R2",
        Target = InputControlType.RightTrigger,
        Source = NativeInputDeviceProfile.Button(9)
      },
      new InputControlMapping()
      {
        Handle = "Select",
        Target = InputControlType.Select,
        Source = NativeInputDeviceProfile.Button(10)
      },
      new InputControlMapping()
      {
        Handle = "Start",
        Target = InputControlType.Start,
        Source = NativeInputDeviceProfile.Button(11)
      },
      new InputControlMapping()
      {
        Handle = "Left Stick Button",
        Target = InputControlType.LeftStickButton,
        Source = NativeInputDeviceProfile.Button(13)
      },
      new InputControlMapping()
      {
        Handle = "Right Stick Button",
        Target = InputControlType.RightStickButton,
        Source = NativeInputDeviceProfile.Button(14)
      },
      new InputControlMapping()
      {
        Handle = "DPad Up",
        Target = InputControlType.DPadUp,
        Source = NativeInputDeviceProfile.Button(15)
      },
      new InputControlMapping()
      {
        Handle = "DPad Down",
        Target = InputControlType.DPadDown,
        Source = NativeInputDeviceProfile.Button(16 /*0x10*/)
      },
      new InputControlMapping()
      {
        Handle = "DPad Left",
        Target = InputControlType.DPadLeft,
        Source = NativeInputDeviceProfile.Button(17)
      },
      new InputControlMapping()
      {
        Handle = "DPad Right",
        Target = InputControlType.DPadRight,
        Source = NativeInputDeviceProfile.Button(18)
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
