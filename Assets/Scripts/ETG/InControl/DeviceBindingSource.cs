using System.IO;

using UnityEngine;

#nullable disable
namespace InControl
{
    public class DeviceBindingSource : BindingSource
    {
        internal DeviceBindingSource() => this.Control = InputControlType.None;

        public DeviceBindingSource(InputControlType control) => this.Control = control;

        public InputControlType Control { get; protected set; }

        public bool ForceRawInput { get; set; }

        public override float GetValue(InputDevice inputDevice)
        {
            if (inputDevice == null)
                return 0.0f;
            return this.ForceRawInput ? inputDevice.GetControl(this.Control).RawValue : inputDevice.GetControl(this.Control).Value;
        }

        public override bool GetState(InputDevice inputDevice)
        {
            return inputDevice != null && inputDevice.GetControl(this.Control).State;
        }

        public override string Name
        {
            get
            {
                if (this.BoundTo == null)
                    return string.Empty;
                InputDevice device = this.BoundTo.Device;
                return device.GetControl(this.Control) == InputControl.Null ? this.Control.ToString() : device.GetControl(this.Control).Handle;
            }
        }

        public override string DeviceName
        {
            get
            {
                if (this.BoundTo == null)
                    return string.Empty;
                InputDevice device = this.BoundTo.Device;
                return device == InputDevice.Null ? "Controller" : device.Name;
            }
        }

        public override InputDeviceClass DeviceClass
        {
            get => this.BoundTo == null ? InputDeviceClass.Unknown : this.BoundTo.Device.DeviceClass;
        }

        public override InputDeviceStyle DeviceStyle
        {
            get => this.BoundTo == null ? InputDeviceStyle.Unknown : this.BoundTo.Device.DeviceStyle;
        }

        public override bool Equals(BindingSource other)
        {
            if (other == (BindingSource) null)
                return false;
            DeviceBindingSource deviceBindingSource = other as DeviceBindingSource;
            return (BindingSource) deviceBindingSource != (BindingSource) null && this.Control == deviceBindingSource.Control;
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            DeviceBindingSource deviceBindingSource = other as DeviceBindingSource;
            return (BindingSource) deviceBindingSource != (BindingSource) null && this.Control == deviceBindingSource.Control;
        }

        public override int GetHashCode() => this.Control.GetHashCode();

        public override BindingSourceType BindingSourceType => BindingSourceType.DeviceBindingSource;

        internal override void Save(BinaryWriter writer) => writer.Write((int) this.Control);

        internal override void Load(BinaryReader reader, ushort dataFormatVersion, bool upgrade)
        {
            if (upgrade)
                this.Control = (InputControlType) BindingSource.UpgradeInputControlType(reader.ReadInt32());
            else
                this.Control = (InputControlType) reader.ReadInt32();
        }

        internal override bool IsValid
        {
            get
            {
                if (this.BoundTo == null)
                {
                    Debug.LogError((object) "Cannot query property 'IsValid' for unbound BindingSource.");
                    return false;
                }
                return this.BoundTo.Device.HasControl(this.Control) || Utility.TargetIsStandard(this.Control);
            }
        }
    }
}
