#nullable disable
namespace InControl.NativeProfile
{
  public class GuitarHeroControllerMacProfile : Xbox360DriverMacProfile
  {
    public GuitarHeroControllerMacProfile()
    {
      this.Name = "Guitar Hero Controller";
      this.Meta = "Guitar Hero Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 5168),
          ProductID = new ushort?((ushort) 18248)
        }
      };
    }
  }
}
