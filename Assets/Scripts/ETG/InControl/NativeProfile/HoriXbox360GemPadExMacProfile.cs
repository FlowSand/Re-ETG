#nullable disable
namespace InControl.NativeProfile
{
  public class HoriXbox360GemPadExMacProfile : Xbox360DriverMacProfile
  {
    public HoriXbox360GemPadExMacProfile()
    {
      this.Name = "Hori Xbox 360 Gem Pad Ex";
      this.Meta = "Hori Xbox 360 Gem Pad Ex on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 9414),
          ProductID = new ushort?((ushort) 21773)
        }
      };
    }
  }
}
