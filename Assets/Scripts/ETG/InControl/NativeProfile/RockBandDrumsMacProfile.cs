#nullable disable
namespace InControl.NativeProfile
{
    public class RockBandDrumsMacProfile : Xbox360DriverMacProfile
    {
        public RockBandDrumsMacProfile()
        {
            this.Name = "Rock Band Drums";
            this.Meta = "Rock Band Drums on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 3)
                }
            };
        }
    }
}
