// Decompiled with JetBrains decompiler
// Type: InControl.EightBitdoSFC30WindowsNativeProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl
{
  [AutoDiscover]
  public class EightBitdoSFC30WindowsNativeProfile : NativeInputDeviceProfile
  {
    public EightBitdoSFC30WindowsNativeProfile()
    {
      this.Name = "8Bitdo SFC30 Controller";
      this.Meta = "8Bitdo SFC30 Controller on Windows";
      this.DeviceClass = InputDeviceClass.Controller;
      this.DeviceStyle = InputDeviceStyle.NintendoSNES;
      this.Matchers = new NativeInputDeviceMatcher[2]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 11720),
          ProductID = new ushort?((ushort) 43809)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 11720),
          ProductID = new ushort?((ushort) 10288)
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
          Handle = "Left Trigger",
          Target = InputControlType.LeftTrigger,
          Source = NativeInputDeviceProfile.Button(6)
        },
        new InputControlMapping()
        {
          Handle = "Right Trigger",
          Target = InputControlType.RightTrigger,
          Source = NativeInputDeviceProfile.Button(7)
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
        }
      };
      this.AnalogMappings = new InputControlMapping[4]
      {
        new InputControlMapping()
        {
          Handle = "DPad Up",
          Target = InputControlType.DPadUp,
          Source = NativeInputDeviceProfile.Analog(2),
          SourceRange = InputRange.ZeroToMinusOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "DPad Down",
          Target = InputControlType.DPadDown,
          Source = NativeInputDeviceProfile.Analog(2),
          SourceRange = InputRange.ZeroToOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "DPad Left",
          Target = InputControlType.DPadLeft,
          Source = NativeInputDeviceProfile.Analog(3),
          SourceRange = InputRange.ZeroToMinusOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "DPad Right",
          Target = InputControlType.DPadRight,
          Source = NativeInputDeviceProfile.Analog(3),
          SourceRange = InputRange.ZeroToOne,
          TargetRange = InputRange.ZeroToOne
        }
      };
    }
  }
}
