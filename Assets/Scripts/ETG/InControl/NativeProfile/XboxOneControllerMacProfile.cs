#nullable disable
namespace InControl.NativeProfile
{
  public class XboxOneControllerMacProfile : XboxOneDriverMacProfile
  {
    public XboxOneControllerMacProfile()
    {
      this.Name = "Xbox One Controller";
      this.Meta = "Xbox One Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[2]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 9414),
          ProductID = new ushort?((ushort) 22042)
        },
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 9414),
          ProductID = new ushort?((ushort) 21786)
        }
      };
    }
  }
}
