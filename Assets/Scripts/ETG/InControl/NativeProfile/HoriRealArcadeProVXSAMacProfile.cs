#nullable disable
namespace InControl.NativeProfile
{
    public class HoriRealArcadeProVXSAMacProfile : Xbox360DriverMacProfile
    {
        public HoriRealArcadeProVXSAMacProfile()
        {
            this.Name = "Hori Real Arcade Pro VX SA";
            this.Meta = "Hori Real Arcade Pro VX SA on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 62722)
                }
            };
        }
    }
}
