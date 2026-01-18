#nullable disable
namespace InControl.NativeProfile
{
    public class MKKlassikFightStickMacProfile : Xbox360DriverMacProfile
    {
        public MKKlassikFightStickMacProfile()
        {
            this.Name = "MK Klassik Fight Stick";
            this.Meta = "MK Klassik Fight Stick on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 4779),
                    ProductID = new ushort?((ushort) 771)
                }
            };
        }
    }
}
