#nullable disable
namespace InControl
{
  public class OuyaEverywhereDeviceManager : InputDeviceManager
  {
    private bool[] deviceConnected = new bool[4];

    public OuyaEverywhereDeviceManager()
    {
      for (int deviceIndex = 0; deviceIndex < 4; ++deviceIndex)
        this.devices.Add((InputDevice) new OuyaEverywhereDevice(deviceIndex));
    }

    public override void Update(ulong updateTick, float deltaTime)
    {
      for (int index = 0; index < 4; ++index)
      {
        OuyaEverywhereDevice device = this.devices[index] as OuyaEverywhereDevice;
        if (device.IsConnected != this.deviceConnected[index])
        {
          if (device.IsConnected)
          {
            device.BeforeAttach();
            InputManager.AttachDevice((InputDevice) device);
          }
          else
            InputManager.DetachDevice((InputDevice) device);
          this.deviceConnected[index] = device.IsConnected;
        }
      }
    }

    public static void Enable()
    {
    }
  }
}
