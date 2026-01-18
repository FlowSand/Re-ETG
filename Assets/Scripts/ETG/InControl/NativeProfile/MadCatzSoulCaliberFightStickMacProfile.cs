#nullable disable
namespace InControl.NativeProfile
{
    public class MadCatzSoulCaliberFightStickMacProfile : Xbox360DriverMacProfile
    {
        public MadCatzSoulCaliberFightStickMacProfile()
        {
            this.Name = "Mad Catz Soul Caliber Fight Stick";
            this.Meta = "Mad Catz Soul Caliber Fight Stick on Mac";
            this.Matchers = new NativeInputDeviceMatcher[1]
            {
                new NativeInputDeviceMatcher()
                {
                    VendorID = new ushort?((ushort) 7085),
                    ProductID = new ushort?((ushort) 61503)
                }
            };
        }
    }
}
