#nullable disable
namespace InControl
{
    [AutoDiscover]
    public class XboxOneWin10AEProfile : UnityInputDeviceProfile
    {
        public XboxOneWin10AEProfile()
        {
            this.Name = "XBox One Controller";
            this.Meta = "XBox One Controller on Windows";
            this.DeviceClass = InputDeviceClass.Controller;
            this.DeviceStyle = InputDeviceStyle.XboxOne;
            this.IncludePlatforms = new string[1]{ "Windows" };
            this.ExcludePlatforms = new string[2]
            {
                "Windows 7",
                "Windows 8"
            };
            this.MinSystemBuildNumber = 14393;
            this.JoystickNames = new string[2]
            {
                "Controller (Xbox One For Windows)",
                "Xbox Bluetooth Gamepad"
            };
            this.ButtonMappings = new InputControlMapping[11]
            {
                new InputControlMapping()
                {
                    Handle = "A",
                    Target = InputControlType.Action1,
                    Source = UnityInputDeviceProfile.Button(0)
                },
                new InputControlMapping()
                {
                    Handle = "B",
                    Target = InputControlType.Action2,
                    Source = UnityInputDeviceProfile.Button(1)
                },
                new InputControlMapping()
                {
                    Handle = "X",
                    Target = InputControlType.Action3,
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
                    Handle = "Left Bumper",
                    Target = InputControlType.LeftBumper,
                    Source = UnityInputDeviceProfile.Button(4)
                },
                new InputControlMapping()
                {
                    Handle = "Right Bumper",
                    Target = InputControlType.RightBumper,
                    Source = UnityInputDeviceProfile.Button(5)
                },
                new InputControlMapping()
                {
                    Handle = "View",
                    Target = InputControlType.View,
                    Source = UnityInputDeviceProfile.Button(6)
                },
                new InputControlMapping()
                {
                    Handle = "Menu",
                    Target = InputControlType.Menu,
                    Source = UnityInputDeviceProfile.Button(7)
                },
                new InputControlMapping()
                {
                    Handle = "Left Stick Button",
                    Target = InputControlType.LeftStickButton,
                    Source = UnityInputDeviceProfile.Button(8)
                },
                new InputControlMapping()
                {
                    Handle = "Right Stick Button",
                    Target = InputControlType.RightStickButton,
                    Source = UnityInputDeviceProfile.Button(9)
                },
                new InputControlMapping()
                {
                    Handle = "Guide",
                    Target = InputControlType.System,
                    Source = UnityInputDeviceProfile.Button(10)
                }
            };
            this.AnalogMappings = new InputControlMapping[16]
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
                    Handle = "Right Stick Left",
                    Target = InputControlType.RightStickLeft,
                    Source = UnityInputDeviceProfile.Analog(3),
                    SourceRange = InputRange.ZeroToMinusOne,
                    TargetRange = InputRange.ZeroToOne
                },
                new InputControlMapping()
                {
                    Handle = "Right Stick Right",
                    Target = InputControlType.RightStickRight,
                    Source = UnityInputDeviceProfile.Analog(3),
                    SourceRange = InputRange.ZeroToOne,
                    TargetRange = InputRange.ZeroToOne
                },
                new InputControlMapping()
                {
                    Handle = "Right Stick Up",
                    Target = InputControlType.RightStickUp,
                    Source = UnityInputDeviceProfile.Analog(4),
                    SourceRange = InputRange.ZeroToMinusOne,
                    TargetRange = InputRange.ZeroToOne
                },
                new InputControlMapping()
                {
                    Handle = "Right Stick Down",
                    Target = InputControlType.RightStickDown,
                    Source = UnityInputDeviceProfile.Analog(4),
                    SourceRange = InputRange.ZeroToOne,
                    TargetRange = InputRange.ZeroToOne
                },
                new InputControlMapping()
                {
                    Handle = "DPad Left",
                    Target = InputControlType.DPadLeft,
                    Source = UnityInputDeviceProfile.Analog(5),
                    SourceRange = InputRange.ZeroToMinusOne,
                    TargetRange = InputRange.ZeroToOne
                },
                new InputControlMapping()
                {
                    Handle = "DPad Right",
                    Target = InputControlType.DPadRight,
                    Source = UnityInputDeviceProfile.Analog(5),
                    SourceRange = InputRange.ZeroToOne,
                    TargetRange = InputRange.ZeroToOne
                },
                new InputControlMapping()
                {
                    Handle = "DPad Up",
                    Target = InputControlType.DPadUp,
                    Source = UnityInputDeviceProfile.Analog(6),
                    SourceRange = InputRange.ZeroToOne,
                    TargetRange = InputRange.ZeroToOne
                },
                new InputControlMapping()
                {
                    Handle = "DPad Down",
                    Target = InputControlType.DPadDown,
                    Source = UnityInputDeviceProfile.Analog(6),
                    SourceRange = InputRange.ZeroToMinusOne,
                    TargetRange = InputRange.ZeroToOne
                },
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
                    Source = UnityInputDeviceProfile.Analog(8),
                    SourceRange = InputRange.ZeroToOne,
                    TargetRange = InputRange.ZeroToOne
                },
                new InputControlMapping()
                {
                    Handle = "Right Trigger",
                    Target = InputControlType.RightTrigger,
                    Source = UnityInputDeviceProfile.Analog(9),
                    SourceRange = InputRange.ZeroToOne,
                    TargetRange = InputRange.ZeroToOne
                }
            };
        }
    }
}
