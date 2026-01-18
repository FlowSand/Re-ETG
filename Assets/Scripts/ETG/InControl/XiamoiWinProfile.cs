#nullable disable
namespace InControl
{
    [AutoDiscover]
    public class XiamoiWinProfile : UnityInputDeviceProfile
    {
        public XiamoiWinProfile()
        {
            this.Name = "Xiamoi Bluetooth Controller";
            this.Meta = "Xiamoi Bluetooth Controller on Windows";
            this.DeviceClass = InputDeviceClass.Controller;
            this.DeviceStyle = InputDeviceStyle.Xbox360;
            this.IncludePlatforms = new string[1]{ "Windows" };
            this.JoystickNames = new string[1]
            {
                "XiaoMi Bluetooth Wireless GameController"
            };
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
                },
                new InputControlMapping()
                {
                    Handle = "Back",
                    Target = InputControlType.Back,
                    Source = UnityInputDeviceProfile.Button10
                },
                new InputControlMapping()
                {
                    Handle = "Left Trigger",
                    Target = InputControlType.LeftTrigger,
                    Source = UnityInputDeviceProfile.Button8
                },
                new InputControlMapping()
                {
                    Handle = "Right Trigger",
                    Target = InputControlType.RightTrigger,
                    Source = UnityInputDeviceProfile.Button9
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
                UnityInputDeviceProfile.RightStickUpMapping(UnityInputDeviceProfile.Analog5),
                UnityInputDeviceProfile.RightStickDownMapping(UnityInputDeviceProfile.Analog5),
                UnityInputDeviceProfile.DPadLeftMapping(UnityInputDeviceProfile.Analog6),
                UnityInputDeviceProfile.DPadRightMapping(UnityInputDeviceProfile.Analog6),
                UnityInputDeviceProfile.DPadUpMapping2(UnityInputDeviceProfile.Analog7),
                UnityInputDeviceProfile.DPadDownMapping2(UnityInputDeviceProfile.Analog7)
            };
        }
    }
}
