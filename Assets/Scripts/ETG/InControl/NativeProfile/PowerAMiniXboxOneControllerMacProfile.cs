#nullable disable
namespace InControl.NativeProfile
{
  public class PowerAMiniXboxOneControllerMacProfile : XboxOneDriverMacProfile
  {
    public PowerAMiniXboxOneControllerMacProfile()
    {
      this.Name = "Power A Mini Xbox One Controller";
      this.Meta = "Power A Mini Xbox One Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 9414),
          ProductID = new ushort?((ushort) 21562)
        }
      };
    }
  }
}
