#nullable disable
namespace InControl
{
  [AutoDiscover]
  public class XTR_G2_WindowsUnityProfile : UnityInputDeviceProfile
  {
    public XTR_G2_WindowsUnityProfile()
    {
      this.Name = "KMODEL Simulator XTR G2 FMS Controller";
      this.Meta = "KMODEL Simulator XTR G2 FMS Controller on Windows";
      this.DeviceClass = InputDeviceClass.Controller;
      this.IncludePlatforms = new string[1]{ "Windows" };
      this.JoystickNames = new string[1]
      {
        "KMODEL Simulator - XTR+G2+FMS Controller"
      };
    }
  }
}
