using System.IO;

#nullable disable
namespace InControl
{
    public class KeyBindingSource : BindingSource
    {
        internal KeyBindingSource()
        {
        }

        public KeyBindingSource(KeyCombo keyCombo) => this.Control = keyCombo;

        public KeyBindingSource(params Key[] keys) => this.Control = new KeyCombo(keys);

        public KeyCombo Control { get; protected set; }

        public override float GetValue(InputDevice inputDevice) => this.GetState(inputDevice) ? 1f : 0.0f;

        public override bool GetState(InputDevice inputDevice) => this.Control.IsPressed;

        public override string Name => this.Control.ToString();

        public override string DeviceName => "Keyboard";

        public override InputDeviceClass DeviceClass => InputDeviceClass.Keyboard;

        public override InputDeviceStyle DeviceStyle => InputDeviceStyle.Unknown;

        public override bool Equals(BindingSource other)
        {
            if (other == (BindingSource) null)
                return false;
            KeyBindingSource keyBindingSource = other as KeyBindingSource;
            return (BindingSource) keyBindingSource != (BindingSource) null && this.Control == keyBindingSource.Control;
        }

        public override bool Equals(object other)
        {
            if (other == null)
                return false;
            KeyBindingSource keyBindingSource = other as KeyBindingSource;
            return (BindingSource) keyBindingSource != (BindingSource) null && this.Control == keyBindingSource.Control;
        }

        public override int GetHashCode() => this.Control.GetHashCode();

        public override BindingSourceType BindingSourceType => BindingSourceType.KeyBindingSource;

        internal override void Load(BinaryReader reader, ushort dataFormatVersion, bool upgrade)
        {
            KeyCombo keyCombo = new KeyCombo();
            keyCombo.Load(reader, dataFormatVersion, upgrade);
            this.Control = keyCombo;
        }

        internal override void Save(BinaryWriter writer) => this.Control.Save(writer);
    }
}
