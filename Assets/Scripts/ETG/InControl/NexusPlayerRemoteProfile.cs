#nullable disable
namespace InControl
{
    [AutoDiscover]
    public class NexusPlayerRemoteProfile : UnityInputDeviceProfile
    {
        public NexusPlayerRemoteProfile()
        {
            this.Name = "Nexus Player Remote";
            this.Meta = "Nexus Player Remote";
            this.DeviceClass = InputDeviceClass.Remote;
            this.IncludePlatforms = new string[1]{ "Android" };
            this.JoystickNames = new string[1]
            {
                "Google Nexus Remote"
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
