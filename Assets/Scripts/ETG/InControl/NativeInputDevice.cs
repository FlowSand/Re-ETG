using System;
using System.Runtime.InteropServices;

using UnityEngine;

#nullable disable
namespace InControl
{
    public class NativeInputDevice : InputDevice
    {
        private const int maxUnknownButtons = 20;
        private const int maxUnknownAnalogs = 20;
        private short[] buttons;
        private short[] analogs;
        private NativeInputDeviceProfile profile;
        private int skipUpdateFrames;
        private int numUnknownButtons;
        private int numUnknownAnalogs;

        internal NativeInputDevice()
        {
        }

        internal uint Handle { get; private set; }

        internal NativeDeviceInfo Info { get; private set; }

        internal void Initialize(
            uint deviceHandle,
            NativeDeviceInfo deviceInfo,
            NativeInputDeviceProfile deviceProfile)
        {
            this.Handle = deviceHandle;
            this.Info = deviceInfo;
            this.profile = deviceProfile;
            this.SortOrder = 1000 + (int) this.Handle;
            this.numUnknownButtons = Math.Min((int) this.Info.numButtons, 20);
            this.numUnknownAnalogs = Math.Min((int) this.Info.numAnalogs, 20);
            this.buttons = new short[(IntPtr) this.Info.numButtons];
            this.analogs = new short[(IntPtr) this.Info.numAnalogs];
            this.AnalogSnapshot = (InputDevice.AnalogSnapshotEntry[]) null;
            this.ClearInputState();
            this.ClearControls();
            if (this.IsKnown)
            {
                this.Name = this.profile.Name ?? this.Info.name;
                this.Meta = this.profile.Meta ?? this.Info.name;
                this.DeviceClass = this.profile.DeviceClass;
                this.DeviceStyle = this.profile.DeviceStyle;
                int analogCount = this.profile.AnalogCount;
                for (int index = 0; index < analogCount; ++index)
                {
                    InputControlMapping analogMapping = this.profile.AnalogMappings[index];
                    InputControl inputControl = this.AddControl(analogMapping.Target, analogMapping.Handle);
                    inputControl.Sensitivity = Mathf.Min(this.profile.Sensitivity, analogMapping.Sensitivity);
                    inputControl.LowerDeadZone = Mathf.Max(this.profile.LowerDeadZone, analogMapping.LowerDeadZone);
                    inputControl.UpperDeadZone = Mathf.Min(this.profile.UpperDeadZone, analogMapping.UpperDeadZone);
                    inputControl.Raw = analogMapping.Raw;
                    inputControl.Passive = analogMapping.Passive;
                }
                int buttonCount = this.profile.ButtonCount;
                for (int index = 0; index < buttonCount; ++index)
                {
                    InputControlMapping buttonMapping = this.profile.ButtonMappings[index];
                    this.AddControl(buttonMapping.Target, buttonMapping.Handle).Passive = buttonMapping.Passive;
                }
            }
            else
            {
                this.Name = "Unknown Device";
                this.Meta = this.Info.name;
                for (int index = 0; index < this.NumUnknownButtons; ++index)
                    this.AddControl((InputControlType) (500 + index), "Button " + (object) index);
                for (int index = 0; index < this.NumUnknownAnalogs; ++index)
                    this.AddControl((InputControlType) (400 + index), "Analog " + (object) index, 0.2f, 0.9f);
            }
            this.skipUpdateFrames = 1;
        }

        internal void Initialize(uint deviceHandle, NativeDeviceInfo deviceInfo)
        {
            this.Initialize(deviceHandle, deviceInfo, this.profile);
        }

        public override void Update(ulong updateTick, float deltaTime)
        {
            if (this.skipUpdateFrames > 0)
            {
                --this.skipUpdateFrames;
            }
            else
            {
                IntPtr deviceState;
                if (Native.GetDeviceState(this.Handle, out deviceState))
                {
                    Marshal.Copy(deviceState, this.buttons, 0, this.buttons.Length);
                    deviceState = new IntPtr(deviceState.ToInt64() + (long) (this.buttons.Length * 2));
                    Marshal.Copy(deviceState, this.analogs, 0, this.analogs.Length);
                }
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
        }

        internal override bool ReadRawButtonState(int index)
        {
            return index < this.buttons.Length && this.buttons[index] > (short) -32767;
        }

        internal override float ReadRawAnalogValue(int index)
        {
            return index < this.analogs.Length ? (float) this.analogs[index] / (float) short.MaxValue : 0.0f;
        }

        private byte FloatToByte(float value)
        {
            return (byte) ((double) Mathf.Clamp01(value) * (double) byte.MaxValue);
        }

        public override void Vibrate(float leftMotor, float rightMotor)
        {
            Native.SetHapticState(this.Handle, this.FloatToByte(leftMotor), this.FloatToByte(rightMotor));
        }

        public override void SetLightColor(float red, float green, float blue)
        {
            Native.SetLightColor(this.Handle, this.FloatToByte(red), this.FloatToByte(green), this.FloatToByte(blue));
        }

        public override void SetLightFlash(float flashOnDuration, float flashOffDuration)
        {
            Native.SetLightFlash(this.Handle, this.FloatToByte(flashOnDuration), this.FloatToByte(flashOffDuration));
        }

        public bool HasSameVendorID(NativeDeviceInfo deviceInfo) => this.Info.HasSameVendorID(deviceInfo);

        public bool HasSameProductID(NativeDeviceInfo deviceInfo)
        {
            return this.Info.HasSameProductID(deviceInfo);
        }

        public bool HasSameVersionNumber(NativeDeviceInfo deviceInfo)
        {
            return this.Info.HasSameVersionNumber(deviceInfo);
        }

        public bool HasSameLocation(NativeDeviceInfo deviceInfo) => this.Info.HasSameLocation(deviceInfo);

        public bool HasSameSerialNumber(NativeDeviceInfo deviceInfo)
        {
            return this.Info.HasSameSerialNumber(deviceInfo);
        }

        public override bool IsSupportedOnThisPlatform
        {
            get => this.profile == null || this.profile.IsSupportedOnThisPlatform;
        }

        public override bool IsKnown => this.profile != null;

        internal override int NumUnknownButtons => this.numUnknownButtons;

        internal override int NumUnknownAnalogs => this.numUnknownAnalogs;
    }
}
