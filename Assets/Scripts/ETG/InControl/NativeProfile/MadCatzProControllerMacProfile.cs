#nullable disable
namespace InControl.NativeProfile
{
  public class MadCatzProControllerMacProfile : Xbox360DriverMacProfile
  {
    public MadCatzProControllerMacProfile()
    {
      this.Name = "Mad Catz Pro Controller";
      this.Meta = "Mad Catz Pro Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1848),
          ProductID = new ushort?((ushort) 18214)
        }
      };
    }
  }
}
