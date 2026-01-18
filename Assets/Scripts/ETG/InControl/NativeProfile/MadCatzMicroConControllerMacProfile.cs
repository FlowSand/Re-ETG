#nullable disable
namespace InControl.NativeProfile
{
  public class MadCatzMicroConControllerMacProfile : Xbox360DriverMacProfile
  {
    public MadCatzMicroConControllerMacProfile()
    {
      this.Name = "Mad Catz MicroCon Controller";
      this.Meta = "Mad Catz MicroCon Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1848),
          ProductID = new ushort?((ushort) 18230)
        }
      };
    }
  }
}
