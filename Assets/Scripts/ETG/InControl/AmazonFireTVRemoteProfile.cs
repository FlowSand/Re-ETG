#nullable disable
namespace InControl
{
    [AutoDiscover]
    public class AmazonFireTVRemoteProfile : UnityInputDeviceProfile
    {
        public AmazonFireTVRemoteProfile()
        {
            this.Name = "Amazon Fire TV Remote";
            this.Meta = "Amazon Fire TV Remote on Amazon Fire TV";
            this.DeviceClass = InputDeviceClass.Remote;
            this.DeviceStyle = InputDeviceStyle.AmazonFireTV;
            this.IncludePlatforms = new string[1]{ "Amazon AFT" };
            this.JoystickNames = new string[2]
            {
                string.Empty,
                "Amazon Fire TV Remote"
            };
            this.ButtonMappings = new InputControlMapping[3]
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
                },
                new InputControlMapping()
                {
                    Handle = "Menu",
                    Target = InputControlType.Menu,
                    Source = UnityInputDeviceProfile.MenuKey
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
