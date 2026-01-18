#nullable disable
namespace InControl.NativeProfile
{
  public class PDPTronControllerMacProfile : Xbox360DriverMacProfile
  {
    public PDPTronControllerMacProfile()
    {
      this.Name = "PDP Tron Controller";
      this.Meta = "PDP Tron Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 63747)
        }
      };
    }
  }
}
