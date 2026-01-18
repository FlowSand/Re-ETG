#nullable disable
namespace InControl.NativeProfile
{
  public class HoriPadEXTurboControllerMacProfile : Xbox360DriverMacProfile
  {
    public HoriPadEXTurboControllerMacProfile()
    {
      this.Name = "Hori Pad EX Turbo Controller";
      this.Meta = "Hori Pad EX Turbo Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3853),
          ProductID = new ushort?((ushort) 12)
        }
      };
    }
  }
}
