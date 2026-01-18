#nullable disable
namespace InControl
{
  public class NativeAnalogSource : InputControlSource
  {
    public int AnalogIndex;

    public NativeAnalogSource(int analogIndex) => this.AnalogIndex = analogIndex;

    public float GetValue(InputDevice inputDevice)
    {
      return (inputDevice as NativeInputDevice).ReadRawAnalogValue(this.AnalogIndex);
    }

    public bool GetState(InputDevice inputDevice) => Utility.IsNotZero(this.GetValue(inputDevice));
  }
}
