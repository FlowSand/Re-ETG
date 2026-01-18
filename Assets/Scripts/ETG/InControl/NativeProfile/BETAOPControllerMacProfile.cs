#nullable disable
namespace InControl.NativeProfile
{
  public class BETAOPControllerMacProfile : Xbox360DriverMacProfile
  {
    public BETAOPControllerMacProfile()
    {
      this.Name = "BETAOP Controller";
      this.Meta = "BETAOP Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 4544),
          ProductID = new ushort?((ushort) 21766)
        }
      };
    }
  }
}
