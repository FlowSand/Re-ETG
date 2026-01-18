#nullable disable
namespace InControl.NativeProfile
{
  public class AfterglowPrismaticXboxOneControllerMacProfile : XboxOneDriverMacProfile
  {
    public AfterglowPrismaticXboxOneControllerMacProfile()
    {
      this.Name = "Afterglow Prismatic Xbox One Controller";
      this.Meta = "Afterglow Prismatic Xbox One Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 3695),
          ProductID = new ushort?((ushort) 313)
        }
      };
    }
  }
}
