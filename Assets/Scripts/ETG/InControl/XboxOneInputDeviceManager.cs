using System.Collections.Generic;

using UnityEngine;

#nullable disable
namespace InControl
{
    public class XboxOneInputDeviceManager : InputDeviceManager
    {
        private const int maxDevices = 8;
        private bool[] deviceConnected = new bool[8];

        public XboxOneInputDeviceManager()
        {
            for (uint joystickId = 1; joystickId <= 8U; ++joystickId)
                this.devices.Add((InputDevice) new XboxOneInputDevice(joystickId));
            this.UpdateInternal(0UL, 0.0f);
        }

        private void UpdateInternal(ulong updateTick, float deltaTime)
        {
            for (int index = 0; index < 8; ++index)
            {
                XboxOneInputDevice device = this.devices[index] as XboxOneInputDevice;
                if (device.IsConnected != this.deviceConnected[index])
                {
                    if (device.IsConnected)
                        InputManager.AttachDevice((InputDevice) device);
                    else
                        InputManager.DetachDevice((InputDevice) device);
                    this.deviceConnected[index] = device.IsConnected;
                }
            }
        }

        public override void Update(ulong updateTick, float deltaTime)
        {
            this.UpdateInternal(updateTick, deltaTime);
        }

        public override void Destroy()
        {
        }

        public static bool CheckPlatformSupport(ICollection<string> errors)
        {
            return Application.platform == RuntimePlatform.XboxOne;
        }

        internal static bool Enable()
        {
            List<string> errors = new List<string>();
            if (XboxOneInputDeviceManager.CheckPlatformSupport((ICollection<string>) errors))
            {
                InputManager.AddDeviceManager<XboxOneInputDeviceManager>();
                return true;
            }
            foreach (string text in errors)
                Logger.LogError(text);
            return false;
        }
    }
}
