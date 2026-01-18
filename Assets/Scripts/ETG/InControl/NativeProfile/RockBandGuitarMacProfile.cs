#nullable disable
namespace InControl.NativeProfile
{
  public class RockBandGuitarMacProfile : Xbox360DriverMacProfile
  {
    public RockBandGuitarMacProfile()
    {
      this.Name = "Rock Band Guitar";
      this.Meta = "Rock Band Guitar on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 2)
        }
      };
    }
  }
}
