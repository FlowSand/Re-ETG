using InControl;
using UnityEngine;

#nullable disable
namespace BasicExample
{
  public class BasicExample : MonoBehaviour
  {
    private void Update()
    {
      InputDevice activeDevice = InputManager.ActiveDevice;
      this.transform.Rotate(Vector3.down, 500f * Time.deltaTime * (float) (OneAxisInputControl) activeDevice.LeftStickX, Space.World);
      this.transform.Rotate(Vector3.right, 500f * Time.deltaTime * (float) (OneAxisInputControl) activeDevice.LeftStickY, Space.World);
      Color a = !activeDevice.Action1.IsPressed ? Color.white : Color.red;
      Color b = !activeDevice.Action2.IsPressed ? Color.white : Color.green;
      this.GetComponent<Renderer>().material.color = Color.Lerp(a, b, 0.5f);
    }
  }
}
