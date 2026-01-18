#nullable disable
namespace InControl
{
  [AutoDiscover]
  public class XTR_G2_MacUnityProfile : UnityInputDeviceProfile
  {
    public XTR_G2_MacUnityProfile()
    {
      this.Name = "KMODEL Simulator XTR G2 FMS Controller";
      this.Meta = "KMODEL Simulator XTR G2 FMS Controller on OS X";
      this.DeviceClass = InputDeviceClass.Controller;
      this.IncludePlatforms = new string[1]{ "OS X" };
      this.JoystickNames = new string[1]
      {
        "FeiYing Model KMODEL Simulator - XTR+G2+FMS Controller"
      };
    }
  }
}
