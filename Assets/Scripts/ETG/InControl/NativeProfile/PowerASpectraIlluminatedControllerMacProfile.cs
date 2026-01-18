#nullable disable
namespace InControl.NativeProfile
{
  public class PowerASpectraIlluminatedControllerMacProfile : Xbox360DriverMacProfile
  {
    public PowerASpectraIlluminatedControllerMacProfile()
    {
      this.Name = "PowerA Spectra Illuminated Controller";
      this.Meta = "PowerA Spectra Illuminated Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 9414),
          ProductID = new ushort?((ushort) 21546)
        }
      };
    }
  }
}
