#nullable disable
namespace InControl
{
    [AutoDiscover]
    public class BuffaloClassicAmazonProfile : UnityInputDeviceProfile
    {
        public BuffaloClassicAmazonProfile()
        {
            this.Name = "Buffalo Class Gamepad";
            this.Meta = "Buffalo Class Gamepad on Amazon Fire TV";
            this.DeviceClass = InputDeviceClass.Controller;
            this.IncludePlatforms = new string[1]{ "Amazon AFT" };
            this.JoystickNames = new string[1]
            {
                "USB,2-axis 8-button gamepad  "
            };
            this.ButtonMappings = new InputControlMapping[5]
            {
                new InputControlMapping()
                {
                    Handle = "A",
                    Target = InputControlType.Action2,
                    Source = UnityInputDeviceProfile.Button15
                },
                new InputControlMapping()
                {
                    Handle = "B",
                    Target = InputControlType.Action1,
                    Source = UnityInputDeviceProfile.Button16
                },
                new InputControlMapping()
                {
                    Handle = "X",
                    Target = InputControlType.Action4,
                    Source = UnityInputDeviceProfile.Button17
                },
                new InputControlMapping()
                {
                    Handle = "Y",
                    Target = InputControlType.Action3,
                    Source = UnityInputDeviceProfile.Button18
                },
                new InputControlMapping()
                {
                    Handle = "Left Bumper",
                    Target = InputControlType.LeftBumper,
                    Source = UnityInputDeviceProfile.Button19
                }
            };
            this.AnalogMappings = new InputControlMapping[4]
            {
                UnityInputDeviceProfile.DPadLeftMapping(UnityInputDeviceProfile.Analog0),
                UnityInputDeviceProfile.DPadRightMapping(UnityInputDeviceProfile.Analog0),
                UnityInputDeviceProfile.DPadUpMapping(UnityInputDeviceProfile.Analog1),
                UnityInputDeviceProfile.DPadDownMapping(UnityInputDeviceProfile.Analog1)
            };
        }
    }
}
