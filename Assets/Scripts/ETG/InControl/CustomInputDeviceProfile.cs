using System;
using UnityEngine;

#nullable disable
namespace InControl
{
  [Obsolete("Custom profiles are deprecated. Use the bindings API instead.", false)]
  public class CustomInputDeviceProfile : UnityInputDeviceProfileBase
  {
    protected static InputControlSource MouseButton0 = (InputControlSource) new UnityMouseButtonSource(0);
    protected static InputControlSource MouseButton1 = (InputControlSource) new UnityMouseButtonSource(1);
    protected static InputControlSource MouseButton2 = (InputControlSource) new UnityMouseButtonSource(2);
    protected static InputControlSource MouseXAxis = (InputControlSource) new UnityMouseAxisSource("x");
    protected static InputControlSource MouseYAxis = (InputControlSource) new UnityMouseAxisSource("y");
    protected static InputControlSource MouseScrollWheel = (InputControlSource) new UnityMouseAxisSource("z");

    public CustomInputDeviceProfile()
    {
      this.Name = "Custom Device Profile";
      this.Meta = "Custom Device Profile";
      this.IncludePlatforms = new string[3]
      {
        "Windows",
        "Mac",
        "Linux"
      };
      this.Sensitivity = 1f;
      this.LowerDeadZone = 0.0f;
      this.UpperDeadZone = 1f;
    }

    public sealed override bool IsJoystick => false;

    public sealed override bool HasJoystickName(string joystickName) => false;

    public sealed override bool HasLastResortRegex(string joystickName) => false;

    public sealed override bool HasJoystickOrRegexName(string joystickName) => false;

    protected static InputControlSource KeyCodeButton(params KeyCode[] keyCodeList)
    {
      return (InputControlSource) new UnityKeyCodeSource(keyCodeList);
    }

    protected static InputControlSource KeyCodeComboButton(params KeyCode[] keyCodeList)
    {
      return (InputControlSource) new UnityKeyCodeComboSource(keyCodeList);
    }
  }
}
