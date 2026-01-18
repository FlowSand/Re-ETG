using UnityEngine;

#nullable disable
namespace InControl
{
    public class UnityKeyCodeComboSource : InputControlSource
    {
        public KeyCode[] KeyCodeList;

        public UnityKeyCodeComboSource()
        {
        }

        public UnityKeyCodeComboSource(params KeyCode[] keyCodeList) => this.KeyCodeList = keyCodeList;

        public float GetValue(InputDevice inputDevice) => this.GetState(inputDevice) ? 1f : 0.0f;

        public bool GetState(InputDevice inputDevice)
        {
            for (int index = 0; index < this.KeyCodeList.Length; ++index)
            {
                if (!Input.GetKey(this.KeyCodeList[index]))
                    return false;
            }
            return true;
        }
    }
}
