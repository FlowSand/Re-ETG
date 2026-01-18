using UnityEngine;

#nullable disable
namespace InControl
{
  public class UnityMouseAxisSource : InputControlSource
  {
    public string MouseAxisQuery;

    public UnityMouseAxisSource()
    {
    }

    public UnityMouseAxisSource(string axis) => this.MouseAxisQuery = "mouse " + axis;

    public float GetValue(InputDevice inputDevice) => Input.GetAxisRaw(this.MouseAxisQuery);

    public bool GetState(InputDevice inputDevice) => Utility.IsNotZero(this.GetValue(inputDevice));
  }
}
