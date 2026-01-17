// Decompiled with JetBrains decompiler
// Type: InControl.NativeProfile.BuffaloClassicWindowsNativeProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl.NativeProfile;

[AutoDiscover]
public class BuffaloClassicWindowsNativeProfile : NativeInputDeviceProfile
{
  public BuffaloClassicWindowsNativeProfile()
  {
    this.Name = "iBuffalo Classic Controller";
    this.Meta = "iBuffalo Classic Controller on Windows";
    this.DeviceClass = InputDeviceClass.Controller;
    this.IncludePlatforms = new string[1]{ "Windows" };
    this.Matchers = new NativeInputDeviceMatcher[1]
    {
      new NativeInputDeviceMatcher()
      {
        VendorID = new ushort?((ushort) 1411),
        ProductID = new ushort?((ushort) 8288)
      }
    };
    this.ButtonMappings = new InputControlMapping[8]
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
        Source = NativeInputDeviceProfile.Button(2)
      },
      new InputControlMapping()
      {
        Handle = "Y",
        Target = InputControlType.Action3,
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
        Handle = "Select",
        Target = InputControlType.Select,
        Source = NativeInputDeviceProfile.Button(6)
      },
      new InputControlMapping()
      {
        Handle = "Start",
        Target = InputControlType.Start,
        Source = NativeInputDeviceProfile.Button(7)
      }
    };
    this.AnalogMappings = new InputControlMapping[4]
    {
      new InputControlMapping()
      {
        Handle = "DPad Up",
        Target = InputControlType.DPadUp,
        Source = NativeInputDeviceProfile.Analog(0),
        SourceRange = InputRange.ZeroToMinusOne,
        TargetRange = InputRange.ZeroToOne
      },
      new InputControlMapping()
      {
        Handle = "DPad Down",
        Target = InputControlType.DPadDown,
        Source = NativeInputDeviceProfile.Analog(0),
        SourceRange = InputRange.ZeroToOne,
        TargetRange = InputRange.ZeroToOne
      },
      new InputControlMapping()
      {
        Handle = "DPad Left",
        Target = InputControlType.DPadLeft,
        Source = NativeInputDeviceProfile.Analog(1),
        SourceRange = InputRange.ZeroToMinusOne,
        TargetRange = InputRange.ZeroToOne
      },
      new InputControlMapping()
      {
        Handle = "DPad Right",
        Target = InputControlType.DPadRight,
        Source = NativeInputDeviceProfile.Analog(1),
        SourceRange = InputRange.ZeroToOne,
        TargetRange = InputRange.ZeroToOne
      }
    };
  }
}
