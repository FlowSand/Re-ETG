// Decompiled with JetBrains decompiler
// Type: InControl.NexusPlayerRemoteProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

#nullable disable
namespace InControl
{
  [AutoDiscover]
  public class NexusPlayerRemoteProfile : UnityInputDeviceProfile
  {
    public NexusPlayerRemoteProfile()
    {
      this.Name = "Nexus Player Remote";
      this.Meta = "Nexus Player Remote";
      this.DeviceClass = InputDeviceClass.Remote;
      this.IncludePlatforms = new string[1]{ "Android" };
      this.JoystickNames = new string[1]
      {
        "Google Nexus Remote"
      };
      this.ButtonMappings = new InputControlMapping[2]
      {
        new InputControlMapping()
        {
          Handle = "A",
          Target = InputControlType.Action1,
          Source = UnityInputDeviceProfile.Button0
        },
        new InputControlMapping()
        {
          Handle = "Back",
          Target = InputControlType.Back,
          Source = UnityInputDeviceProfile.EscapeKey
        }
      };
      this.AnalogMappings = new InputControlMapping[4]
      {
        UnityInputDeviceProfile.DPadLeftMapping(UnityInputDeviceProfile.Analog4),
        UnityInputDeviceProfile.DPadRightMapping(UnityInputDeviceProfile.Analog4),
        UnityInputDeviceProfile.DPadUpMapping(UnityInputDeviceProfile.Analog5),
        UnityInputDeviceProfile.DPadDownMapping(UnityInputDeviceProfile.Analog5)
      };
    }
  }
}
