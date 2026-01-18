#nullable disable
namespace InControl.NativeProfile
{
    public class HoriRealArcadeProEXSEMacProfile : Xbox360DriverMacProfile
    {
        public HoriRealArcadeProEXSEMacProfile()
        {
            this.Name = "Hori Real Arcade Pro EX SE";
            this.Meta = "Hori Real Arcade Pro EX SE on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 3853),
                    ProductID = new ushort?((ushort) 22)
                }
            };
        }
    }
}
