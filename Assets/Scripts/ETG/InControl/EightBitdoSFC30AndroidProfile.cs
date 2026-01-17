// Decompiled with JetBrains decompiler
// Type: InControl.EightBitdoSFC30AndroidProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl
{
  [AutoDiscover]
  public class EightBitdoSFC30AndroidProfile : UnityInputDeviceProfile
  {
    public EightBitdoSFC30AndroidProfile()
    {
      this.Name = "8Bitdo SFC30 Controller";
      this.Meta = "8Bitdo SFC30 Controller on Android";
      this.DeviceClass = InputDeviceClass.Controller;
      this.DeviceStyle = InputDeviceStyle.NintendoSNES;
      this.IncludePlatforms = new string[1]{ "Android" };
      this.JoystickNames = new string[1]
      {
        "8Bitdo SFC30 GamePad"
      };
      this.ButtonMappings = new InputControlMapping[8]
      {
        new InputControlMapping()
        {
          Handle = "A",
          Target = InputControlType.Action2,
          Source = UnityInputDeviceProfile.Button(0)
        },
        new InputControlMapping()
        {
          Handle = "B",
          Target = InputControlType.Action1,
          Source = UnityInputDeviceProfile.Button(1)
        },
        new InputControlMapping()
        {
          Handle = "X",
          Target = InputControlType.Action4,
          Source = UnityInputDeviceProfile.Button(2)
        },
        new InputControlMapping()
        {
          Handle = "Y",
          Target = InputControlType.Action3,
          Source = UnityInputDeviceProfile.Button(3)
        },
        new InputControlMapping()
        {
          Handle = "L",
          Target = InputControlType.LeftBumper,
          Source = UnityInputDeviceProfile.Button(4)
        },
        new InputControlMapping()
        {
          Handle = "R",
          Target = InputControlType.RightBumper,
          Source = UnityInputDeviceProfile.Button(5)
        },
        new InputControlMapping()
        {
          Handle = "Select",
          Target = InputControlType.Select,
          Source = UnityInputDeviceProfile.Button(11)
        },
        new InputControlMapping()
        {
          Handle = "Start",
          Target = InputControlType.Start,
          Source = UnityInputDeviceProfile.Button(10)
        }
      };
      this.AnalogMappings = new InputControlMapping[4]
      {
        new InputControlMapping()
        {
          Handle = "DPad Left",
          Target = InputControlType.DPadLeft,
          Source = UnityInputDeviceProfile.Analog(0),
          SourceRange = InputRange.ZeroToMinusOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "DPad Right",
          Target = InputControlType.DPadRight,
          Source = UnityInputDeviceProfile.Analog(0),
          SourceRange = InputRange.ZeroToOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "DPad Up",
          Target = InputControlType.DPadUp,
          Source = UnityInputDeviceProfile.Analog(1),
          SourceRange = InputRange.ZeroToMinusOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "DPad Down",
          Target = InputControlType.DPadDown,
          Source = UnityInputDeviceProfile.Analog(1),
          SourceRange = InputRange.ZeroToOne,
          TargetRange = InputRange.ZeroToOne
        }
      };
    }
  }
}
