#nullable disable
namespace InControl.NativeProfile
{
  public class MadCatzSSF4FightStickTEMacProfile : Xbox360DriverMacProfile
  {
    public MadCatzSSF4FightStickTEMacProfile()
    {
      this.Name = "Mad Catz SSF4 Fight Stick TE";
      this.Meta = "Mad Catz SSF4 Fight Stick TE on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 1848),
          ProductID = new ushort?((ushort) 63288)
        }
      };
    }
  }
}
