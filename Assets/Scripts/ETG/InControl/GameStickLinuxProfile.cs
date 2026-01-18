#nullable disable
namespace InControl
{
    [AutoDiscover]
    public class GameStickLinuxProfile : UnityInputDeviceProfile
    {
        public GameStickLinuxProfile()
        {
            this.Name = "GameStick Controller";
            this.Meta = "GameStick Controller on Linux";
            this.DeviceClass = InputDeviceClass.Controller;
            this.IncludePlatforms = new string[1]{ "Linux" };
            this.JoystickNames = new string[1]
            {
                "GameStick Controller"
            };
            this.MaxUnityVersion = new VersionInfo(4, 9, 0, 0);
            this.ButtonMappings = new InputControlMapping[9]
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
                    Source = UnityInputDeviceProfile.Button3
                },
                new InputControlMapping()
                {
                    Handle = "Y",
                    Target = InputControlType.Action4,
                    Source = UnityInputDeviceProfile.Button4
                },
                new InputControlMapping()
                {
                    Handle = "Left Bumper",
                    Target = InputControlType.LeftBumper,
                    Source = UnityInputDeviceProfile.Button6
                },
                new InputControlMapping()
                {
                    Handle = "Right Bumper",
                    Target = InputControlType.RightBumper,
                    Source = UnityInputDeviceProfile.Button7
                },
                new InputControlMapping()
                {
                    Handle = "Left Stick Button",
                    Target = InputControlType.LeftStickButton,
                    Source = UnityInputDeviceProfile.Button13
                },
                new InputControlMapping()
                {
                    Handle = "Right Stick Button",
                    Target = InputControlType.RightStickButton,
                    Source = UnityInputDeviceProfile.Button14
                },
                new InputControlMapping()
                {
                    Handle = "Start",
                    Target = InputControlType.Start,
                    Source = UnityInputDeviceProfile.Button11
                }
            };
            this.AnalogMappings = new InputControlMapping[12]
            {
                UnityInputDeviceProfile.LeftStickLeftMapping(UnityInputDeviceProfile.Analog0),
                UnityInputDeviceProfile.LeftStickRightMapping(UnityInputDeviceProfile.Analog0),
                UnityInputDeviceProfile.LeftStickUpMapping(UnityInputDeviceProfile.Analog1),
                UnityInputDeviceProfile.LeftStickDownMapping(UnityInputDeviceProfile.Analog1),
                UnityInputDeviceProfile.RightStickLeftMapping(UnityInputDeviceProfile.Analog2),
                UnityInputDeviceProfile.RightStickRightMapping(UnityInputDeviceProfile.Analog2),
                UnityInputDeviceProfile.RightStickUpMapping(UnityInputDeviceProfile.Analog3),
                UnityInputDeviceProfile.RightStickDownMapping(UnityInputDeviceProfile.Analog3),
                UnityInputDeviceProfile.DPadLeftMapping(UnityInputDeviceProfile.Analog6),
                UnityInputDeviceProfile.DPadRightMapping(UnityInputDeviceProfile.Analog6),
                UnityInputDeviceProfile.DPadUpMapping(UnityInputDeviceProfile.Analog7),
                UnityInputDeviceProfile.DPadDownMapping(UnityInputDeviceProfile.Analog7)
            };
        }
    }
}
