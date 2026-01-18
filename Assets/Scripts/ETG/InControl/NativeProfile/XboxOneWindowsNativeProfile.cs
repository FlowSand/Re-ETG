#nullable disable
namespace InControl.NativeProfile
{
  [AutoDiscover]
  public class XboxOneWindowsNativeProfile : NativeInputDeviceProfile
  {
    public XboxOneWindowsNativeProfile()
    {
      this.Name = "Xbox One Controller";
      this.Meta = "Xbox One Controller on Windows";
      this.DeviceClass = InputDeviceClass.Controller;
      this.DeviceStyle = InputDeviceStyle.XboxOne;
      this.IncludePlatforms = new string[2]
      {
        "Windows 7",
        "Windows 8"
      };
      this.Matchers = new NativeInputDeviceMatcher[2]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1118),
          ProductID = new ushort?((ushort) 721)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1118),
          ProductID = new ushort?((ushort) 746)
        }
      };
      this.ButtonMappings = new InputControlMapping[10]
      {
        new InputControlMapping()
        {
          Handle = "A",
          Target = InputControlType.Action1,
          Source = NativeInputDeviceProfile.Button(0)
        },
        new InputControlMapping()
        {
          Handle = "B",
          Target = InputControlType.Action2,
          Source = NativeInputDeviceProfile.Button(1)
        },
        new InputControlMapping()
        {
          Handle = "X",
          Target = InputControlType.Action3,
          Source = NativeInputDeviceProfile.Button(2)
        },
        new InputControlMapping()
        {
          Handle = "Y",
          Target = InputControlType.Action4,
          Source = NativeInputDeviceProfile.Button(3)
        },
        new InputControlMapping()
        {
          Handle = "Left Bumper",
          Target = InputControlType.LeftBumper,
          Source = NativeInputDeviceProfile.Button(4)
        },
        new InputControlMapping()
        {
          Handle = "Right Bumper",
          Target = InputControlType.RightBumper,
          Source = NativeInputDeviceProfile.Button(5)
        },
        new InputControlMapping()
        {
          Handle = "View",
          Target = InputControlType.View,
          Source = NativeInputDeviceProfile.Button(6)
        },
        new InputControlMapping()
        {
          Handle = "Menu",
          Target = InputControlType.Menu,
          Source = NativeInputDeviceProfile.Button(7)
        },
        new InputControlMapping()
        {
          Handle = "Left Stick Button",
          Target = InputControlType.LeftStickButton,
          Source = NativeInputDeviceProfile.Button(8)
        },
        new InputControlMapping()
        {
          Handle = "Right Stick Button",
          Target = InputControlType.RightStickButton,
          Source = NativeInputDeviceProfile.Button(9)
        }
      };
      this.AnalogMappings = new InputControlMapping[14]
      {
        new InputControlMapping()
        {
          Handle = "Left Stick Up",
          Target = InputControlType.LeftStickUp,
          Source = NativeInputDeviceProfile.Analog(0),
          SourceRange = InputRange.ZeroToMinusOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "Left Stick Down",
          Target = InputControlType.LeftStickDown,
          Source = NativeInputDeviceProfile.Analog(0),
          SourceRange = InputRange.ZeroToOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "Left Stick Left",
          Target = InputControlType.LeftStickLeft,
          Source = NativeInputDeviceProfile.Analog(1),
          SourceRange = InputRange.ZeroToMinusOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "Left Stick Right",
          Target = InputControlType.LeftStickRight,
          Source = NativeInputDeviceProfile.Analog(1),
          SourceRange = InputRange.ZeroToOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "Right Stick Up",
          Target = InputControlType.RightStickUp,
          Source = NativeInputDeviceProfile.Analog(2),
          SourceRange = InputRange.ZeroToMinusOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "Right Stick Down",
          Target = InputControlType.RightStickDown,
          Source = NativeInputDeviceProfile.Analog(2),
          SourceRange = InputRange.ZeroToOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "Right Stick Left",
          Target = InputControlType.RightStickLeft,
          Source = NativeInputDeviceProfile.Analog(3),
          SourceRange = InputRange.ZeroToMinusOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "Right Stick Right",
          Target = InputControlType.RightStickRight,
          Source = NativeInputDeviceProfile.Analog(3),
          SourceRange = InputRange.ZeroToOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "Left Trigger",
          Target = InputControlType.LeftTrigger,
          Source = NativeInputDeviceProfile.Analog(4),
          SourceRange = InputRange.ZeroToOne,
          TargetRange = InputRange.ZeroToOne,
          IgnoreInitialZeroValue = true
        },
        new InputControlMapping()
        {
          Handle = "Right Trigger",
          Target = InputControlType.RightTrigger,
          Source = NativeInputDeviceProfile.Analog(4),
          SourceRange = InputRange.ZeroToMinusOne,
          TargetRange = InputRange.ZeroToOne,
          IgnoreInitialZeroValue = true
        },
        new InputControlMapping()
        {
          Handle = "DPad Left",
          Target = InputControlType.DPadLeft,
          Source = NativeInputDeviceProfile.Analog(5),
          SourceRange = InputRange.ZeroToMinusOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "DPad Right",
          Target = InputControlType.DPadRight,
          Source = NativeInputDeviceProfile.Analog(5),
          SourceRange = InputRange.ZeroToOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "DPad Up",
          Target = InputControlType.DPadUp,
          Source = NativeInputDeviceProfile.Analog(6),
          SourceRange = InputRange.ZeroToOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "DPad Down",
          Target = InputControlType.DPadDown,
          Source = NativeInputDeviceProfile.Analog(6),
          SourceRange = InputRange.ZeroToMinusOne,
          TargetRange = InputRange.ZeroToOne
        }
      };
    }
  }
}
