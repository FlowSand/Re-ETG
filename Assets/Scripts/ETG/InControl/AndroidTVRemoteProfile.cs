#nullable disable
namespace InControl
{
    [AutoDiscover]
    public class AndroidTVRemoteProfile : UnityInputDeviceProfile
    {
        public AndroidTVRemoteProfile()
        {
            this.Name = "Android TV Remote";
            this.Meta = "Android TV Remote on Android TV";
            this.DeviceClass = InputDeviceClass.Remote;
            this.IncludePlatforms = new string[1]{ "Android" };
            this.JoystickNames = new string[3]
            {
                string.Empty,
                "touch-input",
                "navigation-input"
            };
            this.ButtonMappings = new InputControlMapping[2]
            {
                new InputControlMapping()
                {
                    Handle = "A",
                    Target = InputControlType.Action1,
                    Source = UnityInputDeviceProfile.Button0
                },
                new InputControlMapping()
                {
                    Handle = "Back",
                    Target = InputControlType.Back,
                    Source = UnityInputDeviceProfile.EscapeKey
                }
            };
            this.AnalogMappings = new InputControlMapping[4]
            {
                UnityInputDeviceProfile.DPadLeftMapping(UnityInputDeviceProfile.Analog4),
                UnityInputDeviceProfile.DPadRightMapping(UnityInputDeviceProfile.Analog4),
                UnityInputDeviceProfile.DPadUpMapping(UnityInputDeviceProfile.Analog5),
                UnityInputDeviceProfile.DPadDownMapping(UnityInputDeviceProfile.Analog5)
            };
        }
    }
}
