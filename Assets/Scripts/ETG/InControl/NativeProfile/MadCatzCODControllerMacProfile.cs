#nullable disable
namespace InControl.NativeProfile
{
  public class MadCatzCODControllerMacProfile : Xbox360DriverMacProfile
  {
    public MadCatzCODControllerMacProfile()
    {
      this.Name = "Mad Catz COD Controller";
      this.Meta = "Mad Catz COD Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 61477)
        }
      };
    }
  }
}
