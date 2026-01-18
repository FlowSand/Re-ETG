#nullable disable
namespace InControl.NativeProfile
{
  public class QanbaFightStickPlusMacProfile : Xbox360DriverMacProfile
  {
    public QanbaFightStickPlusMacProfile()
    {
      this.Name = "Qanba Fight Stick Plus";
      this.Meta = "Qanba Fight Stick Plus on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1848),
          ProductID = new ushort?((ushort) 48879)
        }
      };
    }
  }
}
