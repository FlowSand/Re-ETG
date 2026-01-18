#nullable disable
namespace InControl.NativeProfile
{
  public class MadCatzSaitekAV8R02MacProfile : Xbox360DriverMacProfile
  {
    public MadCatzSaitekAV8R02MacProfile()
    {
      this.Name = "Mad Catz Saitek AV8R02";
      this.Meta = "Mad Catz Saitek AV8R02 on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1848),
          ProductID = new ushort?((ushort) 52009)
        }
      };
    }
  }
}
