#nullable disable
namespace InControl
{
  [AutoDiscover]
  public class XTR55_G2_MacUnityProfile : UnityInputDeviceProfile
  {
    public XTR55_G2_MacUnityProfile()
    {
      this.Name = "SAILI Simulator XTR5.5 G2 FMS Controller";
      this.Meta = "SAILI Simulator XTR5.5 G2 FMS Controller on OS X";
      this.DeviceClass = InputDeviceClass.Controller;
      this.IncludePlatforms = new string[1]{ "OS X" };
      this.JoystickNames = new string[1]
      {
        "              SAILI Simulator --- XTR5.5+G2+FMS Controller"
      };
    }
  }
}
