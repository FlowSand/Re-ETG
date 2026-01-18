#nullable disable
namespace InControl.NativeProfile
{
  public class POWERAFUS1ONTournamentControllerMacProfile : Xbox360DriverMacProfile
  {
    public POWERAFUS1ONTournamentControllerMacProfile()
    {
      this.Name = "POWER A FUS1ON Tournament Controller";
      this.Meta = "POWER A FUS1ON Tournament Controller on Mac";
      this.Matchers = new NativeInputDeviceMatcher[1]
      {
        new NativeInputDeviceMatcher()
        {
          VendorID = new ushort?((ushort) 9414),
          ProductID = new ushort?((ushort) 21399)
        }
      };
    }
  }
}
