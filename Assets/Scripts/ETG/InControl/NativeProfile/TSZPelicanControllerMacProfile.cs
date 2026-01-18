#nullable disable
namespace InControl.NativeProfile
{
  public class TSZPelicanControllerMacProfile : Xbox360DriverMacProfile
  {
    public TSZPelicanControllerMacProfile()
    {
      this.Name = "TSZ Pelican Controller";
      this.Meta = "TSZ Pelican Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 513)
        }
      };
    }
  }
}
