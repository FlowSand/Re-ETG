#nullable disable
namespace InControl.NativeProfile
{
  public class PowerAMiniControllerMacProfile : Xbox360DriverMacProfile
  {
    public PowerAMiniControllerMacProfile()
    {
      this.Name = "PowerA Mini Controller";
      this.Meta = "PowerA Mini Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 9414),
          ProductID = new ushort?((ushort) 21530)
        }
      };
    }
  }
}
