#nullable disable
namespace InControl.NativeProfile
{
    public class MadCatzFightPadControllerMacProfile : Xbox360DriverMacProfile
    {
        public MadCatzFightPadControllerMacProfile()
        {
            this.Name = "Mad Catz FightPad Controller";
            this.Meta = "Mad Catz FightPad Controller on Mac";
            this.Matchers = new NativeInputDeviceMatcher[2]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 61480)
                },
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 1848),
                    ProductID = new ushort?((ushort) 18216)
                }
            };
        }
    }
}
