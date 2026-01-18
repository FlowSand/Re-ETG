using UnityEngine;

#nullable disable
namespace InControl
{
    public class UnityInputDevice : InputDevice
    {
        private static string[,] analogQueries;
        private static string[,] buttonQueries;
        public const int MaxDevices = 10;
        public const int MaxButtons = 20;
        public const int MaxAnalogs = 20;
        private UnityInputDeviceProfileBase profile;

        public UnityInputDevice(UnityInputDeviceProfileBase deviceProfile)
            : this(deviceProfile, 0, string.Empty)
        {
        }

        public UnityInputDevice(int joystickId, string joystickName)
            : this((UnityInputDeviceProfileBase) null, joystickId, joystickName)
        {
        }

        public UnityInputDevice(
            UnityInputDeviceProfileBase deviceProfile,
            int joystickId,
            string joystickName)
        {
            this.profile = deviceProfile;
            this.JoystickId = joystickId;
            if (joystickId != 0)
                this.SortOrder = 100 + joystickId;
            UnityInputDevice.SetupAnalogQueries();
            UnityInputDevice.SetupButtonQueries();
            this.AnalogSnapshot = (InputDevice.AnalogSnapshotEntry[]) null;
            if (this.IsKnown)
            {
                this.Name = this.profile.Name;
                this.Meta = this.profile.Meta;
                this.ControllerSymbology = this.profile.ControllerSymbology;
                this.DeviceClass = this.profile.DeviceClass;
                this.DeviceStyle = this.profile.DeviceStyle;
                int analogCount = this.profile.AnalogCount;
                for (int index = 0; index < analogCount; ++index)
                {
                    InputControlMapping analogMapping = this.profile.AnalogMappings[index];
                    if (Utility.TargetIsAlias(analogMapping.Target))
                    {
                        Debug.LogError((object) $"Cannot map control \"{analogMapping.Handle}\" as InputControlType.{(object) analogMapping.Target} in profile \"{deviceProfile.Name}\" because this target is reserved as an alias. The mapping will be ignored.");
                    }
                    else
                    {
                        InputControl inputControl = this.AddControl(analogMapping.Target, analogMapping.Handle);
                        inputControl.Sensitivity = Mathf.Min(this.profile.Sensitivity, analogMapping.Sensitivity);
                        inputControl.LowerDeadZone = Mathf.Max(this.profile.LowerDeadZone, analogMapping.LowerDeadZone);
                        inputControl.UpperDeadZone = Mathf.Min(this.profile.UpperDeadZone, analogMapping.UpperDeadZone);
                        inputControl.Raw = analogMapping.Raw;
                        inputControl.Passive = analogMapping.Passive;
                    }
                }
                int buttonCount = this.profile.ButtonCount;
                for (int index = 0; index < buttonCount; ++index)
                {
                    InputControlMapping buttonMapping = this.profile.ButtonMappings[index];
                    if (Utility.TargetIsAlias(buttonMapping.Target))
                        Debug.LogError((object) $"Cannot map control \"{buttonMapping.Handle}\" as InputControlType.{(object) buttonMapping.Target} in profile \"{deviceProfile.Name}\" because this target is reserved as an alias. The mapping will be ignored.");
                    else
                        this.AddControl(buttonMapping.Target, buttonMapping.Handle).Passive = buttonMapping.Passive;
                }
            }
            else
            {
                this.Name = "Unknown Device";
                this.Meta = $"\"{joystickName}\"";
                for (int index = 0; index < this.NumUnknownButtons; ++index)
                    this.AddControl((InputControlType) (500 + index), "Button " + (object) index);
                for (int index = 0; index < this.NumUnknownAnalogs; ++index)
                    this.AddControl((InputControlType) (400 + index), "Analog " + (object) index, 0.2f, 0.9f);
            }
        }

        internal int JoystickId { get; private set; }

        public override void Update(ulong updateTick, float deltaTime)
        {
            if (this.IsKnown)
            {
                int analogCount = this.profile.AnalogCount;
                for (int index = 0; index < analogCount; ++index)
                {
                    InputControlMapping analogMapping = this.profile.AnalogMappings[index];
                    float num1 = analogMapping.Source.GetValue((InputDevice) this);
                    InputControl control = this.GetControl(analogMapping.Target);
                    if (!analogMapping.IgnoreInitialZeroValue || !control.IsOnZeroTick || !Utility.IsZero(num1))
                    {
                        float num2 = analogMapping.MapValue(num1);
                        control.UpdateWithValue(num2, updateTick, deltaTime);
                    }
                }
                int buttonCount = this.profile.ButtonCount;
                for (int index = 0; index < buttonCount; ++index)
                {
                    InputControlMapping buttonMapping = this.profile.ButtonMappings[index];
                    bool state = buttonMapping.Source.GetState((InputDevice) this);
                    this.UpdateWithState(buttonMapping.Target, state, updateTick, deltaTime);
                }
            }
            else
            {
                for (int index = 0; index < this.NumUnknownButtons; ++index)
                    this.UpdateWithState((InputControlType) (500 + index), this.ReadRawButtonState(index), updateTick, deltaTime);
                for (int index = 0; index < this.NumUnknownAnalogs; ++index)
                    this.UpdateWithValue((InputControlType) (400 + index), this.ReadRawAnalogValue(index), updateTick, deltaTime);
            }
        }

        private static void SetupAnalogQueries()
        {
            if (UnityInputDevice.analogQueries != null)
                return;
            UnityInputDevice.analogQueries = new string[10, 20];
            for (int index1 = 1; index1 <= 10; ++index1)
            {
                for (int index2 = 0; index2 < 20; ++index2)
                    UnityInputDevice.analogQueries[index1 - 1, index2] = $"joystick {(object) index1} analog {(object) index2}";
            }
        }

        private static void SetupButtonQueries()
        {
            if (UnityInputDevice.buttonQueries != null)
                return;
            UnityInputDevice.buttonQueries = new string[10, 20];
            for (int index1 = 1; index1 <= 10; ++index1)
            {
                for (int index2 = 0; index2 < 20; ++index2)
                    UnityInputDevice.buttonQueries[index1 - 1, index2] = $"joystick {(object) index1} button {(object) index2}";
            }
        }

        private static string GetAnalogKey(int joystickId, int analogId)
        {
            return UnityInputDevice.analogQueries[joystickId - 1, analogId];
        }

        private static string GetButtonKey(int joystickId, int buttonId)
        {
            return UnityInputDevice.buttonQueries[joystickId - 1, buttonId];
        }

        internal override bool ReadRawButtonState(int index)
        {
            return index < 20 && Input.GetKey(UnityInputDevice.buttonQueries[this.JoystickId - 1, index]);
        }

        internal override float ReadRawAnalogValue(int index)
        {
            return index < 20 ? Input.GetAxisRaw(UnityInputDevice.analogQueries[this.JoystickId - 1, index]) : 0.0f;
        }

        public override bool IsSupportedOnThisPlatform
        {
            get => this.profile == null || this.profile.IsSupportedOnThisPlatform;
        }

        public override bool IsKnown => this.profile != null;

        internal override int NumUnknownButtons => 20;

        internal override int NumUnknownAnalogs => 20;
    }
}
