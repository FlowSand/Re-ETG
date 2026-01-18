#nullable disable
namespace InControl.NativeProfile
{
  public class LogitechChillStreamControllerMacProfile : Xbox360DriverMacProfile
  {
    public LogitechChillStreamControllerMacProfile()
    {
      this.Name = "Logitech Chill Stream Controller";
      this.Meta = "Logitech Chill Stream Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1133),
          ProductID = new ushort?((ushort) 49730)
        }
      };
    }
  }
}
