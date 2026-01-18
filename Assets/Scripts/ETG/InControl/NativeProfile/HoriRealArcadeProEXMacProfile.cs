#nullable disable
namespace InControl.NativeProfile
{
    public class HoriRealArcadeProEXMacProfile : Xbox360DriverMacProfile
    {
        public HoriRealArcadeProEXMacProfile()
        {
            this.Name = "Hori Real Arcade Pro EX";
            this.Meta = "Hori Real Arcade Pro EX on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 62724)
                }
            };
        }
    }
}
