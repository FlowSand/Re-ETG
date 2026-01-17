// Decompiled with JetBrains decompiler
// Type: InControl.GameCubeMayflashWinProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl
{
  [AutoDiscover]
  public class GameCubeMayflashWinProfile : UnityInputDeviceProfile
  {
    public GameCubeMayflashWinProfile()
    {
      this.Name = "GameCube Controller";
      this.Meta = "GameCube Controller on Windows via MAYFLASH adapter";
      this.DeviceClass = InputDeviceClass.Controller;
      this.DeviceStyle = InputDeviceStyle.NintendoGameCube;
      this.UpperDeadZone = 0.7f;
      this.IncludePlatforms = new string[1]{ "Windows" };
      this.JoystickNames = new string[1]
      {
        "MAYFLASH GameCube Controller Adapter"
      };
      this.ButtonMappings = new InputControlMapping[12]
      {
        new InputControlMapping()
        {
          Handle = "X",
          Target = InputControlType.Action3,
          Source = UnityInputDeviceProfile.Button(0)
        },
        new InputControlMapping()
        {
          Handle = "A",
          Target = InputControlType.Action1,
          Source = UnityInputDeviceProfile.Button(1)
        },
        new InputControlMapping()
        {
          Handle = "B",
          Target = InputControlType.Action2,
          Source = UnityInputDeviceProfile.Button(2)
        },
        new InputControlMapping()
        {
          Handle = "Y",
          Target = InputControlType.Action4,
          Source = UnityInputDeviceProfile.Button(3)
        },
        new InputControlMapping()
        {
          Handle = "Left Trigger Button",
          Target = InputControlType.Action5,
          Source = UnityInputDeviceProfile.Button(4)
        },
        new InputControlMapping()
        {
          Handle = "Right Trigger Button",
          Target = InputControlType.Action6,
          Source = UnityInputDeviceProfile.Button(5)
        },
        new InputControlMapping()
        {
          Handle = "Z",
          Target = InputControlType.RightBumper,
          Source = UnityInputDeviceProfile.Button(7)
        },
        new InputControlMapping()
        {
          Handle = "Start",
          Target = InputControlType.Start,
          Source = UnityInputDeviceProfile.Button(9)
        },
        new InputControlMapping()
        {
          Handle = "DPad Up",
          Target = InputControlType.DPadUp,
          Source = UnityInputDeviceProfile.Button(12)
        },
        new InputControlMapping()
        {
          Handle = "DPad Right",
          Target = InputControlType.DPadRight,
          Source = UnityInputDeviceProfile.Button(13)
        },
        new InputControlMapping()
        {
          Handle = "DPad Down",
          Target = InputControlType.DPadDown,
          Source = UnityInputDeviceProfile.Button(14)
        },
        new InputControlMapping()
        {
          Handle = "DPad Left",
          Target = InputControlType.DPadLeft,
          Source = UnityInputDeviceProfile.Button(15)
        }
      };
      this.AnalogMappings = new InputControlMapping[14]
      {
        new InputControlMapping()
        {
          Handle = "Left Stick Left",
          Target = InputControlType.LeftStickLeft,
          Source = UnityInputDeviceProfile.Analog(0),
          SourceRange = InputRange.ZeroToMinusOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "Left Stick Right",
          Target = InputControlType.LeftStickRight,
          Source = UnityInputDeviceProfile.Analog(0),
          SourceRange = InputRange.ZeroToOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "Left Stick Up",
          Target = InputControlType.LeftStickUp,
          Source = UnityInputDeviceProfile.Analog(1),
          SourceRange = InputRange.ZeroToMinusOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "Left Stick Down",
          Target = InputControlType.LeftStickDown,
          Source = UnityInputDeviceProfile.Analog(1),
          SourceRange = InputRange.ZeroToOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "C Up",
          Target = InputControlType.RightStickUp,
          Source = UnityInputDeviceProfile.Analog(2),
          SourceRange = InputRange.ZeroToMinusOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "C Down",
          Target = InputControlType.RightStickDown,
          Source = UnityInputDeviceProfile.Analog(2),
          SourceRange = InputRange.ZeroToOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "C Left",
          Target = InputControlType.RightStickLeft,
          Source = UnityInputDeviceProfile.Analog(5),
          SourceRange = InputRange.ZeroToMinusOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "C Right",
          Target = InputControlType.RightStickRight,
          Source = UnityInputDeviceProfile.Analog(5),
          SourceRange = InputRange.ZeroToOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "Left Trigger",
          Target = InputControlType.LeftTrigger,
          Source = UnityInputDeviceProfile.Analog(3),
          SourceRange = InputRange.MinusOneToOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "Right Trigger",
          Target = InputControlType.RightTrigger,
          Source = UnityInputDeviceProfile.Analog(4),
          SourceRange = InputRange.MinusOneToOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "DPad Left",
          Target = InputControlType.DPadLeft,
          Source = UnityInputDeviceProfile.Analog(6),
          SourceRange = InputRange.ZeroToMinusOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "DPad Right",
          Target = InputControlType.DPadRight,
          Source = UnityInputDeviceProfile.Analog(6),
          SourceRange = InputRange.ZeroToOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "DPad Up",
          Target = InputControlType.DPadUp,
          Source = UnityInputDeviceProfile.Analog(7),
          SourceRange = InputRange.ZeroToOne,
          TargetRange = InputRange.ZeroToOne
        },
        new InputControlMapping()
        {
          Handle = "DPad Down",
          Target = InputControlType.DPadDown,
          Source = UnityInputDeviceProfile.Analog(7),
          SourceRange = InputRange.ZeroToMinusOne,
          TargetRange = InputRange.ZeroToOne
        }
      };
    }
  }
}
