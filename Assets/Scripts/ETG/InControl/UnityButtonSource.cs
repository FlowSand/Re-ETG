#nullable disable
namespace InControl
{
    public class UnityButtonSource : InputControlSource
    {
        public int ButtonIndex;

        public UnityButtonSource(int buttonIndex) => this.ButtonIndex = buttonIndex;

        public float GetValue(InputDevice inputDevice) => this.GetState(inputDevice) ? 1f : 0.0f;

        public bool GetState(InputDevice inputDevice)
        {
            return (inputDevice as UnityInputDevice).ReadRawButtonState(this.ButtonIndex);
        }
    }
}
