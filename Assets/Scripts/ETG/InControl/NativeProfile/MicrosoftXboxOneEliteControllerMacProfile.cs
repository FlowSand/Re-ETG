#nullable disable
namespace InControl.NativeProfile
{
  public class MicrosoftXboxOneEliteControllerMacProfile : XboxOneDriverMacProfile
  {
    public MicrosoftXboxOneEliteControllerMacProfile()
    {
      this.Name = "Microsoft Xbox One Elite Controller";
      this.Meta = "Microsoft Xbox One Elite Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1118),
          ProductID = new ushort?((ushort) 739)
        }
      };
    }
  }
}
