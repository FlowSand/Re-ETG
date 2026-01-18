#nullable disable
namespace InControl
{
  [AutoDiscover]
  public class XTR_G2_MacNativeProfile : NativeInputDeviceProfile
  {
    public XTR_G2_MacNativeProfile()
    {
      this.Name = "KMODEL Simulator XTR G2 FMS Controller";
      this.Meta = "KMODEL Simulator XTR G2 FMS Controller on OS X";
      this.DeviceClass = InputDeviceClass.Controller;
      this.IncludePlatforms = new string[1]{ "OS X" };
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 2971),
          ProductID = new ushort?((ushort) 16402),
          NameLiterals = new string[1]
          {
            "KMODEL Simulator - XTR+G2+FMS Controller"
          }
        }
      };
    }
  }
}
