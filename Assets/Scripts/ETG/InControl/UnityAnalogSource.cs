#nullable disable
namespace InControl
{
  public class UnityAnalogSource : InputControlSource
  {
    public int AnalogIndex;

    public UnityAnalogSource(int analogIndex) => this.AnalogIndex = analogIndex;

    public float GetValue(InputDevice inputDevice)
    {
      return (inputDevice as UnityInputDevice).ReadRawAnalogValue(this.AnalogIndex);
    }

    public bool GetState(InputDevice inputDevice) => Utility.IsNotZero(this.GetValue(inputDevice));
  }
}
