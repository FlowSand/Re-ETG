#nullable disable
namespace InControl
{
  [AutoDiscover]
  public class XTR55_G2_WindowsNativeProfile : NativeInputDeviceProfile
  {
    public XTR55_G2_WindowsNativeProfile()
    {
      this.Name = "SAILI Simulator XTR5.5 G2 FMS Controller";
      this.Meta = "SAILI Simulator XTR5.5 G2 FMS Controller on Windows";
      this.DeviceClass = InputDeviceClass.Controller;
      this.IncludePlatforms = new string[1]{ "Windows" };
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 2971),
          ProductID = new ushort?((ushort) 16402),
          NameLiterals = new string[1]
          {
            "SAILI Simulator --- XTR5.5+G2+FMS Controller"
          }
        }
      };
    }
  }
}
