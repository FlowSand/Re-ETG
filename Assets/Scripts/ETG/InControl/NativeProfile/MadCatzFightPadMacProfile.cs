#nullable disable
namespace InControl.NativeProfile
{
    public class MadCatzFightPadMacProfile : Xbox360DriverMacProfile
    {
        public MadCatzFightPadMacProfile()
        {
            this.Name = "Mad Catz FightPad";
            this.Meta = "Mad Catz FightPad on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 61486)
                }
            };
        }
    }
}
