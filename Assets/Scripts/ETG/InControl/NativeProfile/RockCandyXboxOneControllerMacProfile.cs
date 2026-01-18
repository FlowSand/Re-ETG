#nullable disable
namespace InControl.NativeProfile
{
  public class RockCandyXboxOneControllerMacProfile : XboxOneDriverMacProfile
  {
    public RockCandyXboxOneControllerMacProfile()
    {
      this.Name = "Rock Candy Xbox One Controller";
      this.Meta = "Rock Candy Xbox One Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[3]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 326)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 582)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 838)
        }
      };
    }
  }
}
