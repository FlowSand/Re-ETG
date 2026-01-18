using UnityEngine;

#nullable disable
namespace InControl
{
    public class UnityMouseButtonSource : InputControlSource
    {
        public int ButtonId;

        public UnityMouseButtonSource()
        {
        }

        public UnityMouseButtonSource(int buttonId) => this.ButtonId = buttonId;

        public float GetValue(InputDevice inputDevice) => this.GetState(inputDevice) ? 1f : 0.0f;

        public bool GetState(InputDevice inputDevice) => Input.GetMouseButton(this.ButtonId);
    }
}
