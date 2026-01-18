#nullable disable
namespace InControl.NativeProfile
{
  public class MadCatzSF4FightStickRound2TEMacProfile : Xbox360DriverMacProfile
  {
    public MadCatzSF4FightStickRound2TEMacProfile()
    {
      this.Name = "Mad Catz SF4 Fight Stick Round 2 TE";
      this.Meta = "Mad Catz SF4 Fight Stick Round 2 TE on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 7085),
          ProductID = new ushort?((ushort) 61496)
        }
      };
    }
  }
}
