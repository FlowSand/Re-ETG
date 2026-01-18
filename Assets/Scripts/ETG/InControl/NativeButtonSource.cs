#nullable disable
namespace InControl
{
  public class NativeButtonSource : InputControlSource
  {
    public int ButtonIndex;

    public NativeButtonSource(int buttonIndex) => this.ButtonIndex = buttonIndex;

    public float GetValue(InputDevice inputDevice) => this.GetState(inputDevice) ? 1f : 0.0f;

    public bool GetState(InputDevice inputDevice)
    {
      return (inputDevice as NativeInputDevice).ReadRawButtonState(this.ButtonIndex);
    }
  }
}
