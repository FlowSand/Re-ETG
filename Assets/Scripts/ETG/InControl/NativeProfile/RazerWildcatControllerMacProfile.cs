#nullable disable
namespace InControl.NativeProfile
{
  public class RazerWildcatControllerMacProfile : Xbox360DriverMacProfile
  {
    public RazerWildcatControllerMacProfile()
    {
      this.Name = "Razer Wildcat Controller";
      this.Meta = "Razer Wildcat Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 5426),
          ProductID = new ushort?((ushort) 2563)
        }
      };
    }
  }
}
