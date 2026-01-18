using InControl;
using UnityEngine;

#nullable disable
namespace TouchExample
{
  public class CubeController : MonoBehaviour
  {
    private Renderer cachedRenderer;

    private void Start() => this.cachedRenderer = this.GetComponent<Renderer>();

    private void Update()
    {
      InputDevice activeDevice = InputManager.ActiveDevice;
      if (activeDevice != InputDevice.Null && activeDevice != TouchManager.Device)
        TouchManager.ControlsEnabled = false;
      this.cachedRenderer.material.color = this.GetColorFromActionButtons(activeDevice);
      this.transform.Rotate(Vector3.down, 500f * Time.deltaTime * activeDevice.Direction.X, Space.World);
      this.transform.Rotate(Vector3.right, 500f * Time.deltaTime * activeDevice.Direction.Y, Space.World);
    }

    private Color GetColorFromActionButtons(InputDevice inputDevice)
    {
      if ((bool) (OneAxisInputControl) inputDevice.Action1)
        return Color.green;
      if ((bool) (OneAxisInputControl) inputDevice.Action2)
        return Color.red;
      if ((bool) (OneAxisInputControl) inputDevice.Action3)
        return Color.blue;
      return (bool) (OneAxisInputControl) inputDevice.Action4 ? Color.yellow : Color.white;
    }

    private void OnGUI()
    {
      float y = 10f;
      int touchCount = TouchManager.TouchCount;
      for (int touchIndex = 0; touchIndex < touchCount; ++touchIndex)
      {
        InControl.Touch touch = TouchManager.GetTouch(touchIndex);
        GUI.Label(new Rect(10f, y, 500f, y + 15f), $"{string.Empty}{(object) touchIndex}: fingerId = {(object) touch.fingerId}, phase = {touch.phase.ToString()}, position = {(object) touch.position}");
        y += 20f;
      }
    }
  }
}
