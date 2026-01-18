#nullable disable
namespace InControl
{
    [AutoDiscover]
    public class OuyaAmazonProfile : UnityInputDeviceProfile
    {
        public OuyaAmazonProfile()
        {
            this.Name = "OUYA Controller";
            this.Meta = "OUYA Controller on Amazon Fire TV";
            this.DeviceClass = InputDeviceClass.Controller;
            this.DeviceStyle = InputDeviceStyle.Ouya;
            this.IncludePlatforms = new string[1]{ "Amazon AFT" };
            this.JoystickNames = new string[1]
            {
                "OUYA Game Controller"
            };
            this.LowerDeadZone = 0.3f;
            this.ButtonMappings = new InputControlMapping[8]
            {
                new InputControlMapping()
                {
                    Handle = "O",
                    Target = InputControlType.Action1,
                    Source = UnityInputDeviceProfile.Button0
                },
                new InputControlMapping()
                {
                    Handle = "A",
                    Target = InputControlType.Action2,
                    Source = UnityInputDeviceProfile.Button1
                },
                new InputControlMapping()
                {
                    Handle = "U",
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
                    Handle = "Left Stick Button",
                    Target = InputControlType.LeftStickButton,
                    Source = UnityInputDeviceProfile.Button8
                },
                new InputControlMapping()
                {
                    Handle = "Right Stick Button",
                    Target = InputControlType.RightStickButton,
                    Source = UnityInputDeviceProfile.Button9
                }
            };
            this.AnalogMappings = new InputControlMapping[14]
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
}
