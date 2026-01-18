using System;
using System.IO;

#nullable disable
namespace InControl
{
    public struct UnknownDeviceControl : IEquatable<UnknownDeviceControl>
    {
        public static readonly UnknownDeviceControl None = new UnknownDeviceControl(InputControlType.None, InputRangeType.None);
        public InputControlType Control;
        public InputRangeType SourceRange;
        public bool IsButton;
        public bool IsAnalog;

        public UnknownDeviceControl(InputControlType control, InputRangeType sourceRange)
        {
            this.Control = control;
            this.SourceRange = sourceRange;
            this.IsButton = Utility.TargetIsButton(control);
            this.IsAnalog = !this.IsButton;
        }

        internal float GetValue(InputDevice device)
        {
            return device == null ? 0.0f : InputRange.Remap(device.GetControl(this.Control).Value, this.SourceRange, InputRangeType.ZeroToOne);
        }

        public int Index => (int) (this.Control - (!this.IsButton ? 400 : 500));

        public static bool operator ==(UnknownDeviceControl a, UnknownDeviceControl b)
        {
            return object.ReferenceEquals((object) null, (object) a) ? object.ReferenceEquals((object) null, (object) b) : a.Equals(b);
        }

        public static bool operator !=(UnknownDeviceControl a, UnknownDeviceControl b) => !(a == b);

        public bool Equals(UnknownDeviceControl other)
        {
            return this.Control == other.Control && this.SourceRange == other.SourceRange;
        }

        public override bool Equals(object other) => this.Equals((UnknownDeviceControl) other);

        public override int GetHashCode() => this.Control.GetHashCode() ^ this.SourceRange.GetHashCode();

        public static implicit operator bool(UnknownDeviceControl control)
        {
            return control.Control != InputControlType.None;
        }

        public override string ToString()
        {
            return $"UnknownDeviceControl( {this.Control.ToString()}, {this.SourceRange.ToString()} )";
        }

        internal void Save(BinaryWriter writer)
        {
            writer.Write((int) this.Control);
            writer.Write((int) this.SourceRange);
        }

        internal void Load(BinaryReader reader, bool upgrade)
        {
            if (upgrade)
            {
                this.Control = (InputControlType) BindingSource.UpgradeInputControlType(reader.ReadInt32());
                this.SourceRange = (InputRangeType) BindingSource.UpgradeInputRangeType(reader.ReadInt32());
            }
            else
            {
                this.Control = (InputControlType) reader.ReadInt32();
                this.SourceRange = (InputRangeType) reader.ReadInt32();
            }
            this.IsButton = Utility.TargetIsButton(this.Control);
            this.IsAnalog = !this.IsButton;
        }
    }
}
