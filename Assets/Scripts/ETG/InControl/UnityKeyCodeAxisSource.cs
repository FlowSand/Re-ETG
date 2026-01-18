using UnityEngine;

#nullable disable
namespace InControl
{
  public class UnityKeyCodeAxisSource : InputControlSource
  {
    public KeyCode NegativeKeyCode;
    public KeyCode PositiveKeyCode;

    public UnityKeyCodeAxisSource()
    {
    }

    public UnityKeyCodeAxisSource(KeyCode negativeKeyCode, KeyCode positiveKeyCode)
    {
      this.NegativeKeyCode = negativeKeyCode;
      this.PositiveKeyCode = positiveKeyCode;
    }

    public float GetValue(InputDevice inputDevice)
    {
      int num = 0;
      if (Input.GetKey(this.NegativeKeyCode))
        --num;
      if (Input.GetKey(this.PositiveKeyCode))
        ++num;
      return (float) num;
    }

    public bool GetState(InputDevice inputDevice) => Utility.IsNotZero(this.GetValue(inputDevice));
  }
}
