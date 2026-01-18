#nullable disable
namespace InControl.NativeProfile
{
  public class ProEXXboxOneControllerMacProfile : XboxOneDriverMacProfile
  {
    public ProEXXboxOneControllerMacProfile()
    {
      this.Name = "Pro EX Xbox One Controller";
      this.Meta = "Pro EX Xbox One Controller on Mac";
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
