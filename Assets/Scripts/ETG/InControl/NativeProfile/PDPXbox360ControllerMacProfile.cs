#nullable disable
namespace InControl.NativeProfile
{
  public class PDPXbox360ControllerMacProfile : Xbox360DriverMacProfile
  {
    public PDPXbox360ControllerMacProfile()
    {
      this.Name = "PDP Xbox 360 Controller";
      this.Meta = "PDP Xbox 360 Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 1281)
        }
      };
    }
  }
}
