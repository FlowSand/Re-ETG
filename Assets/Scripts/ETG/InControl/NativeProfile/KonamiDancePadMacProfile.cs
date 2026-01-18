#nullable disable
namespace InControl.NativeProfile
{
    public class KonamiDancePadMacProfile : Xbox360DriverMacProfile
    {
        public KonamiDancePadMacProfile()
        {
            this.Name = "Konami Dance Pad";
            this.Meta = "Konami Dance Pad on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 4779),
                    ProductID = new ushort?((ushort) 4)
                }
            };
        }
    }
}
