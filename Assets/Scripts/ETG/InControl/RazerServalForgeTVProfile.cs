// Decompiled with JetBrains decompiler
// Type: InControl.RazerServalForgeTVProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl;

[AutoDiscover]
public class RazerServalForgeTVProfile : UnityInputDeviceProfile
{
  public RazerServalForgeTVProfile()
  {
    this.Name = "Razer Serval Controller";
    this.Meta = "Razer Serval Controller on Razer Forge TV";
    this.DeviceClass = InputDeviceClass.Controller;
    this.IncludePlatforms = new string[1]{ "Razer Forge" };
    this.JoystickNames = new string[1]{ "Razer Serval" };
    this.ButtonMappings = new InputControlMapping[12]
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
        Source = UnityInputDeviceProfile.EscapeKey
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
        Handle = "Power",
        Target = InputControlType.Power,
        Source = UnityInputDeviceProfile.Button12
      }
    };
    this.AnalogMappings = new InputControlMapping[16 /*0x10*/]
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
      UnityInputDeviceProfile.DPadDownMapping(UnityInputDeviceProfile.Analog5),
      new InputControlMapping()
      {
        Handle = "Left Trigger",
        Target = InputControlType.LeftTrigger,
        Source = UnityInputDeviceProfile.Analog6
      },
      new InputControlMapping()
      {
        Handle = "Right Trigger",
        Target = InputControlType.RightTrigger,
        Source = UnityInputDeviceProfile.Analog7
      },
      new InputControlMapping()
      {
        Handle = "Left Trigger",
        Target = InputControlType.LeftTrigger,
        Source = UnityInputDeviceProfile.Analog12
      },
      new InputControlMapping()
      {
        Handle = "Right Trigger",
        Target = InputControlType.RightTrigger,
        Source = UnityInputDeviceProfile.Analog11
      }
    };
  }
}
