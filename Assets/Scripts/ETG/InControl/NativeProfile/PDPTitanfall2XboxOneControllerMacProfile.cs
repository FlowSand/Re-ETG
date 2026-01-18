#nullable disable
namespace InControl.NativeProfile
{
  public class PDPTitanfall2XboxOneControllerMacProfile : XboxOneDriverMacProfile
  {
    public PDPTitanfall2XboxOneControllerMacProfile()
    {
      this.Name = "PDP Titanfall 2 Xbox One Controller";
      this.Meta = "PDP Titanfall 2 Xbox One Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 357)
        }
      };
    }
  }
}
